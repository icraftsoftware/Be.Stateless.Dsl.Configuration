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

using System.IO;
using Be.Stateless.Dsl.Configuration.Commands;
using Be.Stateless.Dsl.Configuration.Specifications;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration
{
    public class ConfigurationSpecificationProcessorFixture
    {
        [Fact]
        public void ProcessSucceeds()
        {
            using (var scope = new TemporaryFileScope("web-original.config"))
            {
                var result = new ConfigurationSpecificationProcessor(
                        new ConfigurationSpecification(
                            new FileInfo(scope.FilePath),
                            new[] {
                                new ElementInsertionCommand(
                                    "/configuration",
                                    new ElementSpecification(
                                        "system.net",
                                        string.Empty,
                                        new AttributeSpecification[0],
                                        "system.net")),
                            },
                            false))
                    .Process();
                result.ModifiedConfiguration.SelectSingleNode("/configuration/system.net").Should().NotBeNull();
            }
        }

        [Fact]
        public void ProcessSucceedsForUndo()
        {
            using (var scope = new TemporaryFileScope("web-modified.config"))
            {
                var result = new ConfigurationSpecificationProcessor(
                        new ConfigurationSpecification(
                            new FileInfo(scope.FilePath),
                            new[] {
                                new ElementDeletionCommand("/configuration/system.net"),
                                new ElementDeletionCommand("/configuration/system.net/settings"),
                                new ElementDeletionCommand("/configuration/system.web"),
                                new ElementDeletionCommand("/configuration/system.web/authorization")
                            },
                            true))
                    .Process();
                result.ModifiedConfiguration.SelectNodes("/configuration/*").Should().BeEmpty();
            }
        }
    }
}
