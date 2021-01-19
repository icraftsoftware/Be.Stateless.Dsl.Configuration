#region Copyright & License

// Copyright © 2012 - 2021 François Chabot & Emmanuel Benitez
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using Be.Stateless.Argument.Validation;
using Be.Stateless.Dsl.Configuration.Extensions;

namespace Be.Stateless.Dsl.Configuration.XPath
{
    public sealed class XPathBuilder
    {
        private static string BuildNodePath(XPathNavigator navigator, XPathFormat format)
        {
            switch (format)
            {
                case XPathFormat.Name:
                    return navigator.Name;
                case XPathFormat.LocalName:
                    var builder = new StringBuilder("*[");
                    builder.Append($"{XpathFunctionNames.LOCAL_NAME}()='{navigator.LocalName}'");
                    if (!string.IsNullOrWhiteSpace(navigator.NamespaceURI)) builder.AppendFormat(" and namespace-uri()='{0}'", navigator.NamespaceURI);
                    var discriminants = navigator.GetDiscriminants().ToArray();
                    if (discriminants.Any()) builder.AppendFormat(" and ({0})", BuildDiscriminantsSelector(navigator, discriminants));
                    builder.Append("]");
                    return builder.ToString();
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        private static string BuildDiscriminantsSelector(XPathNavigator navigator, IEnumerable<string> discriminants)
        {
            return string.Join(
                " and ",
                discriminants.Select(d => $"@{d} = '{navigator.SelectSingleNode($"@{d}")?.Value}'"));
        }

        private static IEnumerable<XPathNavigator> BuildHierarchy(XPathNavigator navigator)
        {
            var hierarchy = new List<XPathNavigator>();

            while (true)
            {
                hierarchy.Add(navigator);
                var parent = navigator.SelectSingleNode("..");
                if (parent != null && parent.NodeType != XPathNodeType.Root)
                {
                    navigator = parent;
                    continue;
                }

                break;
            }

            hierarchy.Reverse();
            return hierarchy;
        }

        public XPathBuilder(XPathNavigator navigator)
        {
            Arguments.Validation.Constraints
                .IsNotNull(navigator, nameof(navigator))
                .Check();

            _navigator = navigator;
        }

        public string BuildAbsolutePath(XPathFormat format = XPathFormat.LocalName)
        {
            var hierarchy = BuildHierarchy(_navigator);
            return $"/{string.Join("/", hierarchy.Select(navigator => BuildNodePath(navigator, format)))}";
        }

        public string BuildCurrentNodePath(XPathFormat format = XPathFormat.LocalName)
        {
            return BuildNodePath(_navigator, format);
        }

        private readonly XPathNavigator _navigator;
    }
}
