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

using System.Linq;
using System.Xml.Linq;
using Be.Stateless.Dsl.Configuration;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Xml.Linq.Extensions
{
	public class XElementExtensionsFixture
	{
		[Fact]
		public void XPath()
		{
			var document = XDocument.Parse(
				$@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' config:key='name' />
		<sectionGroup name='system.net' config:operation='none' />
	</configSections>
</configuration>");

			var element = document.Root;
			element.XPath().Should().Be("/configuration");
			element = element!.Elements().First();
			element.XPath().Should().Be("/configuration/configSections");
			element = element!.Elements().First();
			element.XPath().Should().Be("/configuration/configSections/sectionGroup[1]");
			element = element.ElementsAfterSelf().First();
			element.XPath().Should().Be("/configuration/configSections/sectionGroup[2]");
		}
	}
}
