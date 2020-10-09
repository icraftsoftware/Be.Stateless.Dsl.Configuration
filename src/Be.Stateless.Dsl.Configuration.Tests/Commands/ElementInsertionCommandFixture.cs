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
using Be.Stateless.Dsl.Configuration.Resources;
using Be.Stateless.Dsl.Configuration.Specifications;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration.Commands
{
    public class ElementInsertionCommandFixture
    {
        [Fact]
        public void ExecuteSucceeds()
        {
            var command = new ElementInsertionCommand(
                "/configuration",
                new ElementSpecification("test", null, null, "test"));
            var document = Files.Load("web-original.config").AsXmlDocument();
            command.Execute(document);
            document.SelectSingleNode("/configuration/test")
                .Should().NotBeNull();
        }

        [Fact]
        public void ExecuteSucceedsWithAttributeUpdate()
        {
            var command = new ElementInsertionCommand(
                "/configuration",
                new ElementSpecification(
                    "test",
                    null,
                    new[] {
                        new AttributeSpecification {
                            Name = "test",
                            NamespaceUri = "urn:test",
                            Value = "value"
                        },
                        new AttributeSpecification {
                            Name = "test",
                            Value = "value"
                        }
                    },
                    "test"));

            var document = Files.Load("web-original.config").AsXmlDocument();
            command.Execute(document);
            document.SelectSingleNode("/configuration/test")
                .Should().NotBeNull();
            document.SelectSingleNode("/configuration/test/@*[local-name() = 'test' and namespace-uri()='urn:test']")
                .Should().NotBeNull()
                .And.Subject.Value.Should().Be("value");
            document.SelectSingleNode("/configuration/test/@test")
                .Should().NotBeNull()
                .And.Subject.Value.Should().Be("value");
        }

        [Fact]
        public void ExecuteThrowsWhenElementAlreadyExists()
        {
            var command = new ElementInsertionCommand(
                "/configuration",
                new ElementSpecification("appSettings", null, null, "appSettings"));
            Action act = () => command.Execute(Files.Load("web-original.config").AsXmlDocument());
            act.Should().ThrowExactly<InvalidOperationException>()
                .WithMessage("The configuration element already exists at '/configuration/appSettings'.");
        }
    }
}
