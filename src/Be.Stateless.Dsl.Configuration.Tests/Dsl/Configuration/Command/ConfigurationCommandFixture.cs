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
using System.Reflection;
using System.Xml;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Resources;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Dsl.Configuration.Command
{
	public class ConfigurationCommandFixture
	{
		[Fact]
		public void ExecuteSucceeds()
		{
			var mockedCommand = new Mock<ConfigurationCommand>("/configuration");
			mockedCommand.Object.Execute(ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", stream => stream.AsXmlDocument()));
			mockedCommand.Protected().Verify("Execute", Times.Once(), ItExpr.IsAny<XmlElement>());
		}

		[Fact]
		public void ExecuteThrowsWhenConfigurationElementIsNotFound()
		{
			var mockedCommand = new Mock<ConfigurationCommand>("/test");
			Invoking(
					() => mockedCommand.Object.Execute(
						ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", stream => stream.AsXmlDocument())))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("No configuration element matching '/test' has been found.");
		}

		[Fact]
		public void ExecuteThrowsWhenDocumentIsNull()
		{
			var mockedCommand = new Mock<ConfigurationCommand>("/");
			Invoking(() => mockedCommand.Object.Execute((XmlDocument) null))
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("configurationDocument");
		}

		[Fact]
		public void ExecuteThrowsWhenSeveralNodesAreFound()
		{
			var mockedCommand = new Mock<ConfigurationCommand>("/configuration/appSettings/add");
			Invoking(
					() => mockedCommand.Object.Execute(
						ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", stream => stream.AsXmlDocument())))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("More than one configuration element matching '/configuration/appSettings/add' have been found.");
		}
	}
}
