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

		/// <summary>
		/// Apply the children <see cref="SpecificationElement"/>s of the <see cref="SpecificationElement"/> <paramref
		/// name="specificationElement"/> to the children of the current <see cref="ConfigurationElement"/> while transforming
		/// &#8212;rewriting in place&#8212; the <see cref="SpecificationElement"/>s into their inverse &#8212;undo&#8212; <see
		/// cref="SpecificationElement"/>s on the fly.
		/// </summary>
		/// <param name="specificationElement">
		/// The <see cref="SpecificationElement"/> whose children <see cref="SpecificationElement"/>s will be applied to the
		/// current <see cref="ConfigurationElement"/>.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// If a child <see cref="SpecificationElement"/> of the <paramref name="specificationElement"/> cannot be applied
		/// because it conflicts with an existing child of the current  <see cref="ConfigurationElement"/>.
		/// </exception>
		/// <remarks>
		/// <para>
		/// The children of the <see cref="SpecificationElement"/> <paramref name="specificationElement"/> being applied are
		/// crawled according to a depth-first search path. Then, for each child <see cref="SpecificationElement"/> being
		/// applied, its processing will happen according to its <see cref="SpecificationElement.Operation"/> and whether the
		/// current <see cref="ConfigurationElement"/> has a child <see cref="ConfigurationElement"/> satisfying or equating
		/// this child <see cref="SpecificationElement"/>.
		/// </para>
		/// <para>
		/// If the current <see cref="ConfigurationElement"/> has no child <see cref="ConfigurationElement"/> that satisfies or
		/// equates the child <see cref="SpecificationElement"/> being applied and the <see
		/// cref="SpecificationElement.Operation">SpecificationElement.Operation</see> is
		/// <list type="bullet">
		/// <item>
		/// an <see cref="Operation.INSERT"/> or <see cref="Operation.UPSERT"/> operation, then a new <see
		/// cref="ConfigurationElement"/> with the specified name and attributes will be inserted and the child <see
		/// cref="SpecificationElement"/> being applied will be transformed into its inverse <see cref="SpecificationElement"/>
		/// with a <see cref="Operation.DELETE"/> operation.
		/// </item>
		/// <item>
		/// an <see cref="Operation.UPDATE"/> operation, then an <see cref="InvalidOperationException"/> will be thrown.
		/// </item>
		/// <item>
		/// a <see cref="Operation.DELETE"/> operation, then no child <see cref="ConfigurationElement"/> will be deleted and
		/// the child <see cref="SpecificationElement"/> being applied will be transformed into its inverse <see
		/// cref="SpecificationElement"/> with a <see cref="Operation.NONE"/> operation.
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// If the current <see cref="ConfigurationElement"/> has one child <see cref="ConfigurationElement"/> that equates the
		/// child <see cref="SpecificationElement"/> being applied and the <see
		/// cref="SpecificationElement.Operation">SpecificationElement.Operation</see> is
		/// <list type="bullet">
		/// <item>
		/// an <see cref="Operation.INSERT"/> operation, then the equating child <see cref="ConfigurationElement"/> will be
		/// left untouched and the child <see cref="SpecificationElement"/> being applied will be transformed into its inverse
		/// <see cref="SpecificationElement"/> with a <see cref="Operation.NONE"/> operation.
		/// </item>
		/// <item>
		/// an <see cref="Operation.UPDATE"/> or <see cref="Operation.UPSERT"/> operation, then, depending on whether
		/// attributes of the equating child <see cref="ConfigurationElement"/> would have to be scrapped or not, the child
		/// <see cref="SpecificationElement"/> being applied will be transformed into its inverse <see
		/// cref="SpecificationElement"/> with either an <see cref="Operation.UPDATE"/> or a <see cref="Operation.NONE"/>
		/// operation.
		/// </item>
		/// <item>
		/// a <see cref="Operation.DELETE"/> operation, then the equating child <see cref="ConfigurationElement"/> will be
		/// deleted and the child <see cref="SpecificationElement"/> being applied will be transformed into its inverse <see
		/// cref="SpecificationElement"/> with a <see cref="Operation.INSERT"/> operation.
		/// </item>
		/// </list>
		/// <para>
		/// </para>
		/// If the current <see cref="ConfigurationElement"/> has one child <see cref="ConfigurationElement"/> that satisfies
		/// but does not equate the child <see cref="SpecificationElement"/> being applied and the <see
		/// cref="SpecificationElement.Operation">SpecificationElement.Operation</see> is
		/// <list type="bullet">
		/// <item>
		/// an <see cref="Operation.INSERT"/> or a <see cref="Operation.DELETE"/> operation, then an <see
		/// cref="InvalidOperationException"/> will be thrown.
		/// </item>
		/// <item>
		/// an <see cref="Operation.UPDATE"/> or <see cref="Operation.UPSERT"/> operation, then, depending on whether
		/// attributes of the equating child <see cref="ConfigurationElement"/> would have to be updated or not, the child <see
		/// cref="SpecificationElement"/> being applied will be transformed into its inverse <see cref="SpecificationElement"/>
		/// with either an <see cref="Operation.UPDATE"/> or a <see cref="Operation.NONE"/> operation.
		/// </item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <seealso cref="SpecificationElement.IsEquatedBy"/>
		/// <seealso cref="SpecificationElement.IsSatisfiedBy"/>
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
							// edge case happening when the SpecificationElement specifies only a subset of the attributes of
                            // the satisfying ConfigurationElement; generally, attributes will be updated though 
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
