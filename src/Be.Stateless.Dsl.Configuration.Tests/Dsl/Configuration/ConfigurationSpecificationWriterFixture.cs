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

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using Be.Stateless.Dsl.Configuration.Command;
using Be.Stateless.Dsl.Configuration.Resolver;
using Be.Stateless.Dsl.Configuration.Specification;
using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.Dsl.Configuration
{
	public class ConfigurationSpecificationWriterFixture
	{
		[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
		[Fact]
		public void WriteSucceeds()
		{
			const string targetConfigurationFile = "c:\\web.config";
			var configurationSpecification = new ConfigurationSpecification(
				targetConfigurationFile,
				new ConfigurationCommand[] {
					new ElementInsertionCommand(
						"/*[local-name()='configuration']",
						new(string.Empty, "appSettings", Enumerable.Empty<AttributeSpecification>(), "*[local-name()='appSettings']")),
					new ElementUpdateCommand(
						"/*[local-name()='configuration']/*[local-name()='appSettings']/*[local-name()='add' and (@key = 'first_setting')]",
						new[] {
							new AttributeSpecification(string.Empty, "key", "first_setting"),
							new AttributeSpecification(string.Empty, "value", "updated-value")
						}),
					new ElementDeletionCommand("/*[local-name()='configuration']/*[local-name()='appSettings']/*[local-name()='add' and (@key = 'second_setting')]")
				},
				false);
			var document = new XmlDocument();
			new ConfigurationSpecificationWriter(document).Write(configurationSpecification);

			var mockedResolver = new Mock<IConfigurationFileResolverStrategy>();
			mockedResolver.Setup(resolver => resolver.CanResolve(It.IsAny<string>())).Returns(true);
			mockedResolver.Setup(resolver => resolver.Resolve(It.IsAny<string>())).Returns(new[] { targetConfigurationFile });
			var specification = new ConfigurationSpecificationReader(document, new[] { mockedResolver.Object }).Read().Should().ContainSingle().Subject;
			specification.SpecificationSourceFilePath.Should().BeNull();
			specification.TargetConfigurationFilePath.Should().Be(configurationSpecification.TargetConfigurationFilePath);
			specification.Commands.Should().BeEquivalentTo(configurationSpecification.Commands);
		}
	}
}
