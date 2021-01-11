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

using System.Xml;
using Be.Stateless.Dsl.Configuration.Extensions;

namespace Be.Stateless.Dsl.Configuration.Specifications
{
	public sealed class AttributeSpecification
	{
		public string Name { get; set; }

		public string NamespaceUri { get; set; }

		public string Value { get; set; }

		public void Execute(XmlElement configurationElement)
		{
			var attribute = configurationElement.Attributes[Name, NamespaceUri];
			if (attribute == null)
			{
				attribute = configurationElement.AppendAttribute(Name, NamespaceUri);
				configurationElement.Attributes.Append(attribute);
			}

			attribute.Value = Value;
		}
	}
}
