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

using Be.Stateless.Dsl.Configuration.Resources;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration.XPath
{
    public class XPathBuilderFixture
    {
        [Fact]
        public void BuildAbsolutePathSucceedsWithDiscriminants()
        {
            new XPathBuilder(
                    Files.Load("create-node-change-with-discriminant.config")
                        .AsXPathNavigator()
                        .SelectSingleNode("/configuration/system.net/connectionManagement/add"))
                .BuildAbsolutePath().Should()
                .Be(
                    "/*[local-name()='configuration']/*[local-name()='system.net']/*[local-name()='connectionManagement']/*[local-name()='add' and (@address = '*')]");
        }

        [Fact]
        public void BuildAbsolutePathSucceedsWithoutDiscriminants()
        {
            new XPathBuilder(
                    Files.Load("create-node-change-with-discriminant.config")
                        .AsXPathNavigator()
                        .SelectSingleNode("/configuration/system.net/connectionManagement"))
                .BuildAbsolutePath().Should()
                .Be(
                    "/*[local-name()='configuration']/*[local-name()='system.net']/*[local-name()='connectionManagement']");
        }

        [Fact]
        public void BuildCurrentNodePathSucceedsWithDiscriminants()
        {
            new XPathBuilder(
                    Files.Load("create-node-change-with-discriminant.config")
                        .AsXPathNavigator()
                        .SelectSingleNode("/configuration/system.net/connectionManagement/add"))
                .BuildCurrentNodePath().Should().Be("*[local-name()='add' and (@address = '*')]");
        }

        [Fact]
        public void BuildCurrentNodePathSucceedsWithoutDiscriminants()
        {
            new XPathBuilder(
                    Files.Load("create-node-change-with-discriminant.config")
                        .AsXPathNavigator()
                        .SelectSingleNode("/configuration/system.net/connectionManagement"))
                .BuildCurrentNodePath().Should().Be("*[local-name()='connectionManagement']");
        }
    }
}
