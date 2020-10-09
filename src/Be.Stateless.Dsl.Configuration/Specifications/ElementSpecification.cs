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

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Be.Stateless.Argument.Validation;

namespace Be.Stateless.Dsl.Configuration.Specifications
{
    public class ElementSpecification
    {
        public ElementSpecification(
            string name,
            string namespaceUri,
            IEnumerable<AttributeSpecification> attributeUpdates,
            string selector)
        {
            Arguments.Validation.Constraints
                .IsNotNullOrEmpty(name, nameof(name))
                .Check()
                .IsNotNullOrWhiteSpace(selector, nameof(selector))
                .Check();

            Name = name;
            NamespaceUri = namespaceUri;
            Selector = selector;
            AttributeUpdates = attributeUpdates ?? Enumerable.Empty<AttributeSpecification>();
        }

        public IEnumerable<AttributeSpecification> AttributeUpdates { get; }

        public string Name { get; }

        public string NamespaceUri { get; }

        public string Selector { get; }

        public XmlElement Execute(XmlDocument configurationDocument)
        {
            Arguments.Validation.Constraints
                .IsNotNull(configurationDocument, nameof(configurationDocument))
                .Check();

            var element = configurationDocument.CreateElement(null, Name, NamespaceUri);
            foreach (var attributeUpdate in AttributeUpdates) attributeUpdate.Execute(element);
            return element;
        }

        public bool Exists(XmlElement parentElement)
        {
            Arguments.Validation.Constraints
                .IsNotNull(parentElement, nameof(parentElement))
                .Check();

            return parentElement.SelectSingleNode(Selector) != null;
        }
    }
}
