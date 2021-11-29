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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Xml.Linq.Extensions;
using static Be.Stateless.Dsl.Configuration.Specification;

namespace Be.Stateless.Dsl.Configuration
{
	public class SpecificationElement
	{
		#region Operators

		public static explicit operator XElement(SpecificationElement specificationElement)
		{
			return specificationElement.Element;
		}

		#endregion

		internal SpecificationElement(XElement element)
		{
			Element = element ?? throw new ArgumentNullException(nameof(element));
		}

		[SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global", Justification = "To avoid possible multiple enumerations.")]
		public XName[] KeyAttributeNames => GetAnnotationAttribute(Annotations.Attributes.KEY_AND_ALIASES)?.Value.Disassemble(Element) ?? SpecificationAttributeNames;

		public XName Name => Element.Name;

		public string Operation
		{
			get => Element.Attribute(Annotations.Attributes.OPERATION)?.Value.ToLowerInvariant() ?? Annotations.Operation.UPSERT;
			internal set
			{
				if (!Annotations.Operation.ALL.Contains(value)) throw new ArgumentException($"Invalid operation value '{value}'.");
				Element.SetAttributeValue(Annotations.Attributes.OPERATION, value);
			}
		}

		public string Path => Element.XPath();

		public XName[] ScrapAttributeNames
		{
			get => _scrappedAttributeNames ??= Element.Attribute(Annotations.Attributes.SCRAP)?.Value.Disassemble(Element) ?? Array.Empty<XName>();
			set
			{
				if ((_scrappedAttributeNames = value)?.Any() == true) Element.SetAttributeValue(Annotations.Attributes.SCRAP, value.Assemble());
				else Element.Attribute(Annotations.Attributes.SCRAP)?.Remove();
			}
		}

		// take all non config annotation attributes into account, i.e. including the ones to scrap too
		internal XName[] SpecificationAttributeNames => Element.Attributes().Where(a => !a.IsNamespaceDeclaration && !a.IsAnnotation()).Select(a => a.Name).ToArray();

		private protected XElement Element { get; }

		public bool IsEquatedBy(ConfigurationElement configurationElement)
		{
			var configurationAttributeNames = configurationElement.ConfigurationAttributes().Select(a => a.Name).ToArray();
			var specificationAttributeNames = SpecificationAttributeNames;
			if (configurationAttributeNames.Except(specificationAttributeNames).Any() || specificationAttributeNames.Except(configurationAttributeNames).Any()) return false;
			return configurationAttributeNames.All(name => configurationElement.ConfigurationAttribute(name).Value == Element.Attribute(name)!.Value);
		}

		public bool IsSatisfiedBy(ConfigurationElement configurationElement)
		{
			if (configurationElement.Name != Element.Name) return false;
			foreach (var name in KeyAttributeNames)
			{
				var keyConfigurationAttribute = configurationElement.ConfigurationAttribute(name);
				if (keyConfigurationAttribute == null) return false;
				var specificationKeyAttribute = SpecificationAttribute(name) ?? throw new InvalidOperationException(
					$"Specification attribute '{name}' was not found among the attributes of the specification element '{Path}' although it was specified as a key attribute.");
				if (keyConfigurationAttribute.Value != specificationKeyAttribute.Value) return false;
			}
			return true;
		}

		internal SpecificationAttribute SpecificationAttribute(XName name)
		{
			return SpecificationAttributes().SingleOrDefault(
				sa => sa.Name == name,
				() => throw new InvalidOperationException(
					$"Specification attribute '{name}' was not found among the attributes of the specification element '{Path}'."));
		}

		public IEnumerable<SpecificationAttribute> ScrapAttributes()
		{
			return ScrapAttributeNames.Select(SpecificationAttribute);
		}

		public IEnumerable<SpecificationAttribute> SpecificationAttributes()
		{
			return Element.Attributes()
				.Where(a => !a.IsNamespaceDeclaration && !a.IsAnnotation())
				.Select(a => new SpecificationAttribute(a));
		}

		public virtual IEnumerable<SpecificationElement> SpecificationElements()
		{
			return Element.Elements().Select(element => new SpecificationElement(element));
		}

		public SpecificationElement AddAfterSelf(ConfigurationElement configurationElement)
		{
			var element = new XElement((XElement) configurationElement);
			Element.AddAfterSelf(element);
			return new(element);
		}

		public void Add(IEnumerable<ConfigurationElement> configurationElements)
		{
			Element.Add(configurationElements.Select(ce => new XElement((XElement) ce)));
		}

		public void Add(ConfigurationAttribute configurationAttribute)
		{
			Element.SetAttributeValue(configurationAttribute.Name, configurationAttribute.Value);
		}

		private XAttribute GetAnnotationAttribute(params XName[] nameOrAliases)
		{
			return nameOrAliases.Select(n => Element.Attribute(n)).SingleOrDefault(a => a is not null);
		}

		private XName[] _scrappedAttributeNames;
	}
}
