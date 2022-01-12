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
	public class ConfigurationElementDeletionActionFixture
	{
		[Fact]
		public void ExecuteFailedWhenElementIsNotEmpty()
		{
			var document = ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", XDocument.Load);
			var action = new ConfigurationElementDeletionAction("/configuration/appSettings");
			Invoking(() => action.Execute(document))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("The configuration element '/configuration/appSettings' has at least one child element.");
		}

		[Fact]
		public void ExecuteSucceeds()
		{
			var document = ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", XDocument.Load);
			var action = new ConfigurationElementDeletionAction("/configuration/appSettings/add[@key='first_setting']");
			action.Execute(document);
			document.XPathSelectElement("/configuration/appSettings/add[@key='first_setting']").Should().BeNull();
		}
	}
}
