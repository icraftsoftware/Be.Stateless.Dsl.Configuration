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
using System.Xml.Linq;
using Be.Stateless.Resources;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Dsl.Configuration.Action
{
	public class ConfigurationElementActionFixture
	{
		[Fact]
		public void ExecuteSucceeds()
		{
			var actionMock = new Mock<ConfigurationElementAction>("/configuration");
			actionMock.Object.Execute(ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", XDocument.Load));
			actionMock.Verify(action => action.Execute(It.IsNotNull<XElement>()));
		}

		[Fact]
		public void ExecuteThrowsWhenConfigurationElementIsNotFound()
		{
			var actionMock = new Mock<ConfigurationElementAction>("/test");
			Invoking(() => actionMock.Object.Execute(ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", XDocument.Load)))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("No configuration element matching '/test' has been found.");
		}

		[Fact]
		public void ExecuteThrowsWhenDocumentIsNull()
		{
			var actionMock = new Mock<ConfigurationElementAction>("/");
			Invoking(() => actionMock.Object.Execute((XDocument) null))
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("configurationDocument");
		}

		[Fact]
		public void ExecuteThrowsWhenSeveralNodesAreFound()
		{
			var actionMock = new Mock<ConfigurationElementAction>("/configuration/appSettings/add");
			Invoking(() => actionMock.Object.Execute(ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", XDocument.Load)))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("More than one configuration element matching '/configuration/appSettings/add' have been found.");
		}
	}
}
