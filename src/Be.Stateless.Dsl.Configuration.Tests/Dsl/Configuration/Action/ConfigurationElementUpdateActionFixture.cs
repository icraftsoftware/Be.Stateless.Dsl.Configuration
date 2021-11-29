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
using System.Xml.Linq;
using System.Xml.XPath;
using Be.Stateless.Resources;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration.Action
{
	public class ConfigurationElementUpdateActionFixture
	{
		[Fact]
		public void ExecuteSucceeds()
		{
			var document = ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", XDocument.Load);
			var action = new ConfigurationElementUpdateAction(
				"/configuration",
				new[] { new XAttribute("{urn:test}test", "value") });
			action.Execute(document);
			document.XPathSelectElement("/configuration[@*[local-name() = 'test' and namespace-uri()='urn:test']]").Should().NotBeNull();
		}

		[Fact]
		public void ExecuteSucceedsWithKey()
		{
			var document = ResourceManager.Load(Assembly.GetExecutingAssembly(), "Be.Stateless.Resources.web-original.config", XDocument.Load);
			var action = new ConfigurationElementUpdateAction(
				"/configuration/appSettings/add[@key='first_setting']",
				new[] { new XAttribute("value", "updated-value") });
			action.Execute(document);
			document.XPathSelectElement("/configuration/appSettings/add[@key='first_setting' and @value='updated-value']").Should().NotBeNull();
		}
	}
}
