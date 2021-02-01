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
using System.Reflection;
using System.Xml;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Resources;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.Dsl.Configuration.Command
{
	public class ConfigurationCommandFactoryFixture
	{
		[Fact]
		public void CreateFailedWhenActionIsMissing()
		{
			Action(
					() => {
						ConfigurationCommandFactory.Create(
							ResourceManager
								.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.create-node-change.config", stream => stream.AsXmlDocument().CreateNavigator())
								.SelectSingleNode("/configuration"));
					})
				.Should().ThrowExactly<InvalidOperationException>();
		}

		[Fact]
		public void CreateSucceedsForElementDeletionChange()
		{
			ConfigurationCommandFactory.Create(
					ResourceManager
						.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.delete-element-command.config", stream => stream.AsXmlDocument().CreateNavigator())
						.SelectSingleNode("/configuration/appSettings/add[@key='second_setting']"))
				.Should().NotBeNull()
				.And.BeOfType<ElementDeletionCommand>()
				.Which.ConfigurationElementSelector
				.Should().Be("/*[local-name()='configuration']/*[local-name()='appSettings']/*[local-name()='add' and (@key = 'second_setting')]");
		}

		[Fact]
		public void CreateSucceedsForNodeInsertionChange()
		{
			var elementInsertionDefinition = ConfigurationCommandFactory.Create(
					ResourceManager
						.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.create-node-change.config", stream => stream.AsXmlDocument().CreateNavigator())
						.SelectSingleNode("/configuration/system.net"))
				.Should().NotBeNull()
				.And.BeOfType<ElementInsertionCommand>().Subject.ElementSpecification;
			elementInsertionDefinition.Name.Should().Be("system.net");
			elementInsertionDefinition.NamespaceUri.Should().BeNullOrEmpty();
			elementInsertionDefinition.Selector.Should().Be("*[local-name()='system.net']");
		}

		[Fact]
		public void CreateSucceedsForNodeInsertionChangeWithDiscriminant()
		{
			var elementInsertionDefinition = ConfigurationCommandFactory
				.Create(
					ResourceManager
						.Load(
							Assembly.GetExecutingAssembly(),
							"Be.Stateless.Resources.create-node-change-with-discriminant.config",
							stream => stream.AsXmlDocument().CreateNavigator())
						.SelectSingleNode("/configuration/system.net/connectionManagement/add"))
				.Should().NotBeNull()
				.And.BeOfType<ElementInsertionCommand>().Subject
				.ElementSpecification;
			elementInsertionDefinition.Name.Should().Be("add");
			elementInsertionDefinition.NamespaceUri.Should().BeNullOrEmpty();
			elementInsertionDefinition.Selector.Should().Be("*[local-name()='add' and (@address = '*')]");
		}

		[Fact]
		public void CreateSucceedsForNodeUpdateChangeWithAttributeUpdate()
		{
			ConfigurationCommandFactory
				.Create(
					ResourceManager
						.Load(
							Assembly.GetExecutingAssembly(),
							"Be.Stateless.Resources.update-node-change-with-attribute-update.config",
							stream => stream.AsXmlDocument().CreateNavigator())
						.SelectSingleNode("/configuration/system.net"))
				.Should().NotBeNull()
				.And.BeOfType<ElementUpdateCommand>()
				.Subject.AttributeSpecifications.Should()
				.ContainSingle(update => update.Name == "test" && update.Value == "true");
		}

		[Fact]
		public void CreateUndoCommandSucceedsForDeletion()
		{
			var elementToUpdate = ResourceManager
				.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", stream => stream.AsXmlDocument())
				.SelectSingleNode("/configuration/appSettings/add[@key='first_setting']") as XmlElement;
			var command = ConfigurationCommandFactory.CreateUndoCommandForDeletion(elementToUpdate);
			command.Should().BeEquivalentTo(
				new ElementInsertionCommand(
					"/configuration/appSettings",
					new ElementSpecification(
						"add",
						string.Empty,
						new[] {
							new AttributeSpecification { Name = "key", Value = "first_setting", NamespaceUri = string.Empty },
							new AttributeSpecification { Name = "value", Value = "", NamespaceUri = string.Empty }
						},
						"add")),
				options => options.IncludingAllDeclaredProperties());
		}

		[Fact]
		public void CreateUndoCommandSucceedsForInsertion()
		{
			var command = ConfigurationCommandFactory.CreateUndoCommandForInsertion(
				new ElementInsertionCommand(
					"/configuration",
					new ElementSpecification("appSettings", string.Empty, Enumerable.Empty<AttributeSpecification>(), "appSettings")));
			command.Should().BeEquivalentTo(new ElementDeletionCommand("/configuration/appSettings"));
		}

		[Fact]
		public void CreateUndoCommandSucceedsForUpdate()
		{
			var elementToUpdate = ResourceManager
				.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", stream => stream.AsXmlDocument())
				.SelectSingleNode("/configuration/appSettings/add[@key='first_setting']") as XmlElement;
			var command = ConfigurationCommandFactory.CreateUndoCommandForUpdate(
				new ElementUpdateCommand(
					"/configuration/appSettings/add[@key='first_setting']",
					new[] {
						new AttributeSpecification { Name = "value", Value = "test", NamespaceUri = string.Empty }
					}),
				elementToUpdate);
			command.Should().BeEquivalentTo(
				new ElementUpdateCommand(
					"/configuration/appSettings/add[@key='first_setting']",
					new[] {
						new AttributeSpecification { Name = "value", Value = "", NamespaceUri = string.Empty }
					}));
		}
	}
}
