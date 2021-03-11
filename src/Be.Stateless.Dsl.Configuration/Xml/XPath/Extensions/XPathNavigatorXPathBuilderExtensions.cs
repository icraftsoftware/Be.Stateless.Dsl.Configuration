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
using Be.Stateless.Dsl.Configuration.Xml.XPath;
using Be.Stateless.Extensions;

namespace Be.Stateless.Xml.XPath.Extensions
{
	internal static class XPathNavigatorXPathBuilderExtensions
	{
		internal static string BuildAbsolutePath(this XPathNavigator navigator, XPathFormat format = XPathFormat.LocalName)
		{
			var hierarchy = (navigator ?? throw new ArgumentNullException(nameof(navigator)))
				.CreateNavigator()
				.BuildAncestorNavigationHierarchy();
			return $"/{string.Join("/", hierarchy.Select(n => n.BuildNodePath(format)))}";
		}

		internal static string BuildCurrentNodeRelativePath(this XPathNavigator navigator, XPathFormat format = XPathFormat.LocalName)
		{
			return (navigator ?? throw new ArgumentNullException(nameof(navigator)))
				.CreateNavigator()
				.BuildNodePath(format);
		}

		private static IEnumerable<XPathNavigator> BuildAncestorNavigationHierarchy(this XPathNavigator navigator)
		{
			// TODO return a IEnumerable of node and not XPathNavigators
			var hierarchy = new Stack<XPathNavigator>();
			while (true)
			{
				hierarchy.Push(navigator);
				var parent = navigator.SelectSingleNode("..");
				if (parent == null || parent.NodeType == XPathNodeType.Root) break;
				navigator = parent;
			}
			return hierarchy;
		}

		private static string BuildNodePath(this XPathNavigator navigator, XPathFormat format)
		{
			switch (format)
			{
				// TODO explain the difference
				case XPathFormat.Name:
					return navigator.Name;
				case XPathFormat.LocalName:
					var builder = new StringBuilder("*[");
					builder.Append($"{XpathFunctionNames.LOCAL_NAME}()='{navigator.LocalName}'");
					if (!navigator.NamespaceURI.IsNullOrWhiteSpace()) builder.AppendFormat($" and {XpathFunctionNames.NAMESPACE_URI}()='{0}'", navigator.NamespaceURI);
					var discriminants = navigator.GetDiscriminants().ToArray();
					if (discriminants.Any()) builder.AppendFormat(" and ({0})", navigator.BuildDiscriminantSelector(discriminants));
					builder.Append(']');
					return builder.ToString();
				default:
					throw new ArgumentOutOfRangeException(nameof(format), format, null);
			}
		}

		private static string BuildDiscriminantSelector(this XPathNavigator navigator, IEnumerable<string> discriminants)
		{
			return string.Join(
				" and ",
				discriminants.Select(d => $"@{d} = '{navigator.SelectSingleNode($"@{d}")?.Value}'"));
		}
	}
}
