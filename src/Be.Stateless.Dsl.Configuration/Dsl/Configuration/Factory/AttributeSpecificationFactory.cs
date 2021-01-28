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
using System.Xml.XPath;
using Be.Stateless.Dsl.Configuration.Specification;

namespace Be.Stateless.Dsl.Configuration.Factory
{
	public static class AttributeSpecificationFactory
	{
		public static AttributeSpecification Create(XPathNavigator navigator)
		{
			if (navigator == null) throw new ArgumentNullException(nameof(navigator));
			if (navigator.NodeType != XPathNodeType.Attribute)
				throw new ArgumentException(
					$"The navigator's current node must be an attribute (navigator type:'{(navigator != null ? navigator.NodeType : (XPathNodeType?) null)}')",
					nameof(navigator));

			return new AttributeSpecification {
				Name = navigator.LocalName,
				NamespaceUri = navigator.NamespaceURI,
				Value = navigator.Value
			};
		}
	}
}
