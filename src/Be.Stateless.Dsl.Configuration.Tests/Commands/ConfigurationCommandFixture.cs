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
using System.Xml;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;

namespace Be.Stateless.Dsl.Configuration.Commands
{
	public class ConfigurationCommandFixture
	{
		[Fact]
		public void ExecuteSucceeds()
		{
			var mockedCommand = new Mock<ConfigurationCommand>("/configuration");
			mockedCommand.Object.Execute(Resources.Files.Load("web-original.config").AsXmlDocument());
			mockedCommand.Protected().Verify("Execute", Times.Once(), ItExpr.IsAny<XmlElement>());
		}

		[Fact]
		public void ExecuteThrowsWhenConfigurationElementIsNotFound()
		{
			var mockedCommand = new Mock<ConfigurationCommand>("/test");
			Action act = () => mockedCommand.Object.Execute(Resources.Files.Load("web-original.config").AsXmlDocument());
			act.Should().Throw<InvalidOperationException>()
				.WithMessage("Cannot find configuration element at '/test'.");
		}

		[Fact]
		public void ExecuteThrowsWhenDocumentIsNull()
		{
			var mockedCommand = new Mock<ConfigurationCommand>("/");
			Action act = () => mockedCommand.Object.Execute(null);
			act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("configurationDocument");
		}

		[Fact]
		public void ExecuteThrowsWhenSeveralNodesAreFound()
		{
			var mockedCommand = new Mock<ConfigurationCommand>("/configuration/appSettings/add");
			Action act = () => mockedCommand.Object.Execute(Resources.Files.Load("web-original.config").AsXmlDocument());
			act.Should().Throw<InvalidOperationException>()
				.WithMessage("Found multiple configuration elements at '/configuration/appSettings/add'.");
		}
	}
}
