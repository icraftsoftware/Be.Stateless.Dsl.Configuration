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
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Collections.Extensions
{
	public class HashtableExtensionsFixture
	{
		[Fact]
		public void AsXAttributesFailedWhenHashtableHasNullValue()
		{
			Invoking(() => new Hashtable { { "test", null } }.AsXAttributes().ToArray())
				.Should().Throw<ArgumentException>().WithMessage("The value of attribute 'test' is null.*");
		}

		[Fact]
		public void AsXAttributesSucceeds()
		{
			new Hashtable { { "test", "value1" }, { "{urn:test}test", "value2" } }.AsXAttributes().Should()
				.BeEquivalentTo(new[] { new XAttribute("test", "value1"), new XAttribute(XName.Get("test", "urn:test"), "value2") });
		}

		[Fact]
		public void AsXAttributesSucceedsWhenHashtableIsEmpty()
		{
			new Hashtable().AsXAttributes().Should().NotBeNull().And.BeEmpty();
		}
	}
}
