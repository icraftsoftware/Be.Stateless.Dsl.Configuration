#region Copyright & License

// Copyright © 2012 - 2020 François Chabot & Emmanuel Benitez
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
using System.Linq;
using System.Xml;
using Be.Stateless.Argument.Validation;
using Be.Stateless.Dsl.Configuration.Xml;
using Be.Stateless.Dsl.Configuration.XPath;

namespace Be.Stateless.Dsl.Configuration.Extensions
{
    public static class XmlNodeExtensions
    {
        public static XmlElement SelectOrAppendElement(this XmlNode parentNode, XPathLocationStep locationStep)
        {
            Arguments.Validation.Constraints
                .IsNotNull(parentNode, nameof(parentNode))
                .Check();

            return (XmlElement) parentNode.SelectSingleNode(locationStep.Value) ?? parentNode.AppendElement(locationStep);
        }

        private static XmlElement AppendElement(this XmlNode parentNode, XPathLocationStep locationStep)
        {
            if (!locationStep.IsValid) throw new InvalidOperationException($"The XPath location step '{locationStep.Value}' is not valid.");
            var element = (parentNode.OwnerDocument ?? (XmlDocument) parentNode).CreateElement(locationStep.ElementName);
            parentNode.AppendChild(element);
            if (locationStep.AttributeSpecifications.Any())
            {
                foreach (var attributeSpecification in locationStep.AttributeSpecifications) attributeSpecification.Execute(element);
                element.AppendAttribute(
                    XmlAttributeNames.DISCRIMINANT,
                    Constants.NAMESPACE_URI,
                    Constants.NAMESPACE_URI_PREFIX,
                    string.Join(Constants.DISCRIMINANT_SEPARATOR.ToString(), locationStep.AttributeSpecifications.Select(specification => specification.Name)));
            }
            return element;
        }
    }
}
