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
using Be.Stateless.Dsl.Configuration.Commands;
using Be.Stateless.Dsl.Configuration.Resources;
using Be.Stateless.Dsl.Configuration.Specifications;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration.Factories
{
    public class ConfigurationCommandFactoryFixture
    {
        [Fact]
        public void CreateFailedWhenActionIsMissing()
        {
            Action act = () => {
                ConfigurationCommandFactory.Create(
                    Files.Load("create-node-change.config")
                        .AsXPathNavigator()
                        .SelectSingleNode("/configuration"));
            };
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void CreateSucceedsForElementDeletionChange()
        {
            ConfigurationCommandFactory.Create(
                    Files
                        .Load("delete-element-command.config")
                        .AsXPathNavigator()
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
                    Files
                        .Load("create-node-change.config")
                        .AsXPathNavigator()
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
                    Files.Load("create-node-change-with-discriminant.config")
                        .AsXPathNavigator()
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
                    Files.Load("update-node-change-with-attribute-update.config")
                        .AsXPathNavigator()
                        .SelectSingleNode("/configuration/system.net"))
                .Should().NotBeNull()
                .And.BeOfType<ElementUpdateCommand>()
                .Subject.AttributeSpecifications.Should()
                .ContainSingle(update => update.Name == "test" && update.Value == "true");
        }

        [Fact]
        public void CreateUndoCommandSucceedsForDeletion()
        {
            var elementToUpdate = Files.Load("web-original.config").AsXmlDocument().SelectSingleNode("/configuration/appSettings/add[@key='first_setting']") as XmlElement;
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
            var elementToUpdate = Files.Load("web-original.config").AsXmlDocument().SelectSingleNode("/configuration/appSettings/add[@key='first_setting']") as XmlElement;
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
