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
using Be.Stateless.Dsl.Configuration.Command;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Resources;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration
{
	public class ConfigurationSpecificationProcessorFixture
	{
		[Fact]
		public void ProcessSucceeds()
		{
			var wipConfiguration = ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", stream => stream.AsXmlDocument());
			var result = new ConfigurationSpecificationProcessor(
					new ConfigurationSpecification(
						"non-existent-file",
						new[] {
							new ElementInsertionCommand(
								"/configuration",
								new ElementSpecification("system.net", string.Empty, new AttributeSpecification[0], "system.net"))
						},
						false))
				.Process(wipConfiguration);
			result.Configuration.SelectSingleNode("/configuration/system.net").Should().NotBeNull();
		}

		[Fact]
		public void ProcessSucceedsForUndo()
		{
			var wipConfiguration = ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-modified.config", stream => stream.AsXmlDocument());
			var result = new ConfigurationSpecificationProcessor(
					new ConfigurationSpecification(
						"non-existent-file",
						new[] {
							new ElementDeletionCommand("/configuration/system.net"),
							new ElementDeletionCommand("/configuration/system.net/settings"),
							new ElementDeletionCommand("/configuration/system.web"),
							new ElementDeletionCommand("/configuration/system.web/authorization")
						},
						true))
				.Process(wipConfiguration);
			result.Configuration.SelectNodes("/configuration/*").Should().BeEmpty();
		}
	}
}
