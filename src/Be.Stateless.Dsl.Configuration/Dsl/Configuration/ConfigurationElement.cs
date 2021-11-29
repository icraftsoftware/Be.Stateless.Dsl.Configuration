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
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Xml.Linq.Extensions;
using static Be.Stateless.Dsl.Configuration.Specification.Annotations;

namespace Be.Stateless.Dsl.Configuration
{
	public class ConfigurationElement
	{
		#region Nested Type: AfterLastChildConfigurationElement

		private class AfterLastChildConfigurationElement : ConfigurationElement
		{
			internal AfterLastChildConfigurationElement(ConfigurationElement parentConfigurationElement)
				: base(parentConfigurationElement.Element, parentConfigurationElement.Configuration) { }

			#region Base Class Member Overrides

			private protected override void AddBeforeSelf(ConfigurationElement configurationElement)
			{
				Element.Add((XElement) configurationElement);
			}

			#endregion
		}

		#endregion

		#region Operators

		public static explicit operator XElement(ConfigurationElement configurationElement)
		{
			return configurationElement.Element;
		}

		#endregion

		internal ConfigurationElement(XElement element, Configuration configuration)
		{
			Element = element ?? throw new ArgumentNullException(nameof(element));
			Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Publi API.")]
		public bool HasConfigurationElements => Element.HasElements;

		public XName Name => Element.Name;

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Publi API.")]
		public string Path => Element.XPath();

		private protected Configuration Configuration { get; }

		private protected XElement Element { get; }

		public void Apply(SpecificationElement specificationElement)
		{
			ConfigurationElement pivotChildConfigurationElement = new AfterLastChildConfigurationElement(this);
			// reverse order of SpecificationElements() to satisfy configuration element order constraint
			foreach (var childSpecificationElement in specificationElement.SpecificationElements().Reverse())
			{
				var childConfigurationElement = SatisfyingConfigurationElement(childSpecificationElement);
				switch (childSpecificationElement.Operation)
				{
					case Operation.INSERT when childConfigurationElement is null:
					case Operation.UPSERT when childConfigurationElement is null:
						childSpecificationElement.Operation = Operation.DELETE;
						childConfigurationElement = new(
							new(childSpecificationElement.Name, childSpecificationElement.SpecificationAttributes().Select(sa => (XAttribute) sa)),
							Configuration);
						pivotChildConfigurationElement.AddBeforeSelf(childConfigurationElement);
						Configuration.IsDirty = true;
						break;

					case Operation.UPDATE when childConfigurationElement is null:
						throw new InvalidOperationException($"Cannot find a configuration element to update that corresponds to specification '{childSpecificationElement.Path}'.");

					case Operation.DELETE when childConfigurationElement is null:
					case Operation.INSERT when childConfigurationElement.IsEquating(childSpecificationElement):
						childSpecificationElement.Operation = Operation.NONE;
						break;

					case Operation.NONE:
						break;

					case Operation.UPDATE when childConfigurationElement.IsEquating(childSpecificationElement):
					case Operation.UPSERT when childConfigurationElement.IsEquating(childSpecificationElement):
						if (childConfigurationElement.ScrapAttributes(childSpecificationElement))
						{
							childSpecificationElement.Operation = Operation.UPDATE;
							Configuration.IsDirty = true;
						}
						else
						{
							childSpecificationElement.Operation = Operation.NONE;
						}
						break;

					case Operation.UPDATE:
					case Operation.UPSERT:
						if (childConfigurationElement.UpdateAttributes(childSpecificationElement))
						{
							childSpecificationElement.Operation = Operation.UPDATE;
							Configuration.IsDirty = true;
						}
						else
						{
							childSpecificationElement.Operation = Operation.NONE;
						}
						break;

					case Operation.DELETE when childConfigurationElement.IsEquating(childSpecificationElement):
						childSpecificationElement.Operation = Operation.INSERT;
						// makes sure the element will be inserted at the same place in DOM when applying the inverse specification
						var followingSiblingConfigurationElement = childConfigurationElement.FollowingSiblingConfigurationElement();
						if (followingSiblingConfigurationElement is not null && followingSiblingConfigurationElement.Element != pivotChildConfigurationElement.Element)
							childSpecificationElement.AddAfterSelf(followingSiblingConfigurationElement).Operation = Operation.NONE;
						// rebuild subtree that was deleted if necessary
						if (childConfigurationElement.HasConfigurationElements) childSpecificationElement.Add(childConfigurationElement.ConfigurationElements());
						childConfigurationElement.Remove();
						childConfigurationElement = null; // don't mess followingChildConfigurationElement
						Configuration.IsDirty = true;
						break;

					case Operation.DELETE:
						throw new InvalidOperationException(
							$"Cannot delete configuration element '{childConfigurationElement.Path}' corresponding to specification '{childSpecificationElement.Path}' because it conflicts with specification.");

					case Operation.INSERT:
						throw new InvalidOperationException(
							$"Cannot insert configuration element '{childConfigurationElement.Path}' corresponding to specification '{childSpecificationElement.Path}' because it conflicts with an existing one.");

					default:
						throw new InvalidOperationException(
							$"Specification element '{childSpecificationElement.Path}' has an invalid operation annotation value '{childSpecificationElement.Operation}'.");
				}
				childConfigurationElement?.Apply(childSpecificationElement);
				pivotChildConfigurationElement = childConfigurationElement ?? pivotChildConfigurationElement;
			}
		}

		public ConfigurationAttribute ConfigurationAttribute(XName name)
		{
			return Element.Attribute(name).IfNotNull(a => new ConfigurationAttribute(a));
		}

		public IEnumerable<ConfigurationAttribute> ConfigurationAttributes()
		{
			return Element.Attributes().Where(a => !a.IsNamespaceDeclaration).Select(a => new ConfigurationAttribute(a));
		}

		private protected virtual void AddBeforeSelf(ConfigurationElement configurationElement)
		{
			Element.AddBeforeSelf((XElement) configurationElement);
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		private bool ScrapAttributes(SpecificationElement specificationElement)
		{
			var isDirty = false;
			foreach (var attribute in specificationElement.ScrapAttributes())
			{
				var configurationAttribute = ConfigurationAttribute(attribute.Name);
				if (configurationAttribute is not null)
				{
					if (configurationAttribute.Value != attribute.Value)
						throw new InvalidOperationException(
							$"Cannot scrap attributes of configuration element '{Path}' corresponding to specification '{specificationElement.Path}' because they conflict with specification ones.");
					specificationElement.Add(configurationAttribute);
					configurationAttribute.Remove();
					isDirty = true;
				}
			}
			specificationElement.ScrapAttributeNames = Array.Empty<XName>();
			return isDirty;
		}

		private bool UpdateAttributes(SpecificationElement specificationElement)
		{
			var isDirty = false;
			var inverseScrapAttributeNames = new LinkedList<XName>();
			foreach (var specificationAttribute in specificationElement.SpecificationAttributes().Except(specificationElement.ScrapAttributes()))
			{
				var configurationAttribute = ConfigurationAttribute(specificationAttribute.Name);
				if (configurationAttribute is null)
				{
					inverseScrapAttributeNames.AddLast(specificationAttribute.Name);
					Add(specificationAttribute);
					isDirty = true;
				}
				else if (configurationAttribute.Value != specificationAttribute.Value)
				{
					(configurationAttribute.Value, specificationAttribute.Value) = (specificationAttribute.Value, configurationAttribute.Value);
					isDirty = true;
				}
			}
			if (ScrapAttributes(specificationElement))
			{
				isDirty = true;
			}
			specificationElement.ScrapAttributeNames = inverseScrapAttributeNames.ToArray();
			return isDirty;
		}

		private void Add(SpecificationAttribute specificationAttribute)
		{
			Element.Add(new XAttribute(specificationAttribute.Name, specificationAttribute.Value));
		}

		public virtual IEnumerable<ConfigurationElement> ConfigurationElements()
		{
			return Element.Elements().Select(element => new ConfigurationElement(element, Configuration));
		}

		private ConfigurationElement FollowingSiblingConfigurationElement()
		{
			return Element.ElementsAfterSelf().FirstOrDefault().IfNotNull(e => new ConfigurationElement(e, Configuration));
		}

		private bool IsEquating(SpecificationElement specificationElement)
		{
			return specificationElement.IsEquatedBy(this);
		}

		private bool IsSatisfying(SpecificationElement specificationElement)
		{
			return specificationElement.IsSatisfiedBy(this);
		}

		private void Remove()
		{
			Element.Remove();
		}

		private ConfigurationElement SatisfyingConfigurationElement(SpecificationElement specificationElement)
		{
			return ConfigurationElements().SingleOrDefault(
				ce => ce.IsSatisfying(specificationElement),
				() => new InvalidOperationException(
					$"Configuration element '{Path}' contains more than one child configuration elements matching specification '{specificationElement.Path}'."));
		}
	}
}
