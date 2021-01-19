﻿#region Copyright & License

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

using System.Collections.Generic;
using Be.Stateless.Dsl.Configuration.Resources;
using Be.Stateless.Dsl.Configuration.Specifications;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration.Commands
{
    public class ElementUpsertionCommandFixture
    {
        [Fact]
        public void ExecuteSucceedsForInsert()
        {
            var command = new ElementUpsertionCommand(
                "/configuration",
                new ElementInsertionCommand(
                    "/configuration",
                    new ElementSpecification("test", null, null, "test")),
                new ElementUpdateCommand(
                    "/configuration/test",
                    new List<AttributeSpecification>()));
            var document = Files.Load("web-original.config").AsXmlDocument();
            command.Execute(document);
            document.SelectSingleNode("/configuration/test")
                .Should().NotBeNull();
        }

        [Fact]
        public void ExecuteSucceedsForUpdate()
        {
            var command = new ElementUpsertionCommand(
                "/configuration",
                new ElementInsertionCommand(
                    "/configuration",
                    new ElementSpecification("appSettings", null, null, "appSettings")),
                new ElementUpdateCommand(
                    "/configuration/appSettings",
                    new[] { new AttributeSpecification { Name = "test", Value = "value" } }));
            var document = Files.Load("web-original.config").AsXmlDocument();
            command.Execute(document);
            document.SelectSingleNode("/configuration/appSettings[@test='value']")
                .Should().NotBeNull();
        }
    }
}
