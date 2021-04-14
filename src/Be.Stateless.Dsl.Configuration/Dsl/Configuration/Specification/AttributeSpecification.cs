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
using System.Xml;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Extensions;

namespace Be.Stateless.Dsl.Configuration.Specification
{
	public sealed class AttributeSpecification
	{
		public AttributeSpecification(string name, string value)
		{
			if (name.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(name), $"'{nameof(name)}' cannot be null or empty.");
			Name = name;
			Value = value ?? throw new ArgumentNullException(nameof(value));
		}

		public AttributeSpecification(string namespaceUri, string name, string value) : this(name, value)
		{
			NamespaceUri = namespaceUri ?? throw new ArgumentNullException(nameof(namespaceUri));
		}

		public string Name { get; }

		public string NamespaceUri { get; }

		public string Value { get; }

		public void Execute(XmlElement configurationElement)
		{
			var attribute = NamespaceUri == null
				? configurationElement.GetAttributeNode(Name) ?? configurationElement.AppendAttribute(Name)
				: configurationElement.GetAttributeNode(Name, NamespaceUri) ?? configurationElement.AppendAttribute(Name, NamespaceUri);
			attribute.Value = Value;
		}
	}
}
