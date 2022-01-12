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
using System.Xml.XPath;
using Be.Stateless.Resources;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Dsl.Configuration.Action
{
	public class ConfigurationElementInsertionActionFixture
	{
		[Fact]
		public void ExecuteSucceeds()
		{
			var document = ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", XDocument.Load);
			var action = new ConfigurationElementInsertionAction("/configuration", "test");
			action.Execute(document);
			document.XPathSelectElement("/configuration/test").Should().NotBeNull();
		}

		[Fact]
		public void ExecuteSucceedsWithAttributeUpdate()
		{
			var document = ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", XDocument.Load);
			var action = new ConfigurationElementInsertionAction("/configuration", "test", new[] { new XAttribute("{urn:test}test", "value1"), new XAttribute("test", "value2") });
			action.Execute(document);
			document.XPathSelectElement("/configuration/test").Should().NotBeNull();
			document.XPathSelectElement("/configuration/test[@*[local-name() = 'test' and namespace-uri()='urn:test']='value1']").Should().NotBeNull();
			document.XPathSelectElement("/configuration/test[@test='value2']").Should().NotBeNull();
		}

		[Fact]
		public void ExecuteThrowsWhenElementAlreadyExists()
		{
			var action = new ConfigurationElementInsertionAction("/configuration", "appSettings");
			Invoking(() => action.Execute(ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", XDocument.Load)))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("The configuration element already exists at '/configuration/appSettings'.");
		}
	}
}
