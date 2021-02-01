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

using System.Reflection;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Resources;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration.Command
{
	public class ElementUpdateCommandFixture
	{
		[Fact]
		public void ExecuteSucceeds()
		{
			var document = ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", stream => stream.AsXmlDocument());
			var command = new ElementUpdateCommand(
				"/configuration",
				new[] {
					new AttributeSpecification {
						Name = "test",
						NamespaceUri = "urn:test",
						Value = "value"
					}
				});
			command.Execute(document);
			document.SelectSingleNode("/configuration/@*[local-name() = 'test' and namespace-uri()='urn:test']")
				.Should().NotBeNull()
				.And.Subject.Value.Should().Be("value");
		}

		[Fact]
		public void ExecuteSucceedsWithDiscriminants()
		{
			var document = ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", stream => stream.AsXmlDocument());
			var command = new ElementUpdateCommand(
				"/configuration/appSettings/add[@key='first_setting']",
				new[] {
					new AttributeSpecification {
						Name = "value",
						Value = "updated-value"
					}
				});
			command.Execute(document);
			document.SelectSingleNode("/configuration/appSettings/add[@key='first_setting']/@value")
				.Should().NotBeNull()
				.And.Subject.Value.Should().Be("updated-value");
		}
	}
}
