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
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Be.Stateless.Dsl.Configuration.Command;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.Dsl.Configuration.Xml.XPath;
using Be.Stateless.Xml.XPath.Extensions;

namespace Be.Stateless.Dsl.Configuration.Factory
{
	public static class ConfigurationCommandFactory
	{
		public static ConfigurationCommand Create(XPathNavigator navigator)
		{
			var namespaceScopedNavigator = navigator.AsNamespaceScopedNavigator();
			var commandType = namespaceScopedNavigator.GetCommandType();
			return commandType switch {
				CommandTypeNames.INSERT => CreateElementInsertionCommand(namespaceScopedNavigator),
				CommandTypeNames.UPDATE => CreateElementUpdateCommand(namespaceScopedNavigator),
				CommandTypeNames.UPSERT => CreateElementUpsertionCommand(namespaceScopedNavigator),
				CommandTypeNames.DELETE => CreateElementDeletionCommand(namespaceScopedNavigator),
				_ => throw new InvalidOperationException($"The command '{commandType}' is not supported.")
			};
		}

		public static ElementInsertionCommand CreateUndoCommandForDeletion(XmlElement configurationElement)
		{
			return new ElementInsertionCommand(
				new XPathBuilder(
						(configurationElement.ParentNode ?? throw new InvalidOperationException("The parent node is null.")).CreateNavigator())
					.BuildAbsolutePath(XPathFormat.Name),
				new ElementSpecification(
					configurationElement.LocalName,
					configurationElement.NamespaceURI,
					configurationElement.Attributes.OfType<XmlAttribute>().Select(
						attribute => new AttributeSpecification {
							Name = attribute.Name,
							NamespaceUri = attribute.NamespaceURI,
							Value = attribute.Value
						}),
					new XPathBuilder(configurationElement.CreateNavigator()).BuildCurrentNodePath(XPathFormat.Name)));
		}

		public static ElementDeletionCommand CreateUndoCommandForInsertion(ElementInsertionCommand command)
		{
			return new ElementDeletionCommand(
				string.Join("/", command.ConfigurationElementSelector, command.ElementSpecification.Selector));
		}

		public static ConfigurationCommand CreateUndoCommandForUpdate(ElementUpdateCommand command, XmlElement configurationElement)
		{
			return new ElementUpdateCommand(
				command.ConfigurationElementSelector,
				command.AttributeSpecifications.Select(
					specification => new AttributeSpecification {
						Name = specification.Name,
						NamespaceUri = specification.NamespaceUri,
						Value = configurationElement.GetAttribute(specification.Name, specification.NamespaceUri)
					}));
		}

		private static ElementDeletionCommand CreateElementDeletionCommand(XPathNavigator navigator)
		{
			return new ElementDeletionCommand(new XPathBuilder(navigator).BuildAbsolutePath());
		}

		private static ElementInsertionCommand CreateElementInsertionCommand(XPathNavigator navigator)
		{
			return new ElementInsertionCommand(
				new XPathBuilder(navigator.SelectSingleNode("..")).BuildAbsolutePath(),
				new ElementSpecification(
					navigator.LocalName,
					navigator.NamespaceURI,
					navigator.GetAttributeUpdates(),
					new XPathBuilder(navigator).BuildCurrentNodePath()
				));
		}

		private static ElementUpdateCommand CreateElementUpdateCommand(XPathNavigator navigator)
		{
			return new ElementUpdateCommand(
				new XPathBuilder(navigator).BuildAbsolutePath(),
				navigator.GetAttributeUpdates() ?? Enumerable.Empty<AttributeSpecification>());
		}

		private static ElementUpsertionCommand CreateElementUpsertionCommand(XPathNavigator navigator)
		{
			return new ElementUpsertionCommand(
				new XPathBuilder(navigator).BuildAbsolutePath(),
				CreateElementInsertionCommand(navigator),
				CreateElementUpdateCommand(navigator));
		}
	}
}
