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
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration.Resolvers
{
	public class DotNetFrameworkFileResolverFixture
	{
		[Theory]
		[InlineData("v4.5:32bits:machine.config")]
		[InlineData("global:v4.5:32bits")]
		[InlineData("global:net45:test.config")]
		[InlineData("global:32bits:web.config")]
		[InlineData("global:web.config")]
		[InlineData("global:")]
		public void CannotResolve(string fileUri)
		{
			_converter.CanResolve(fileUri).Should().BeFalse();
		}

		[Theory]
		[InlineData("global:net45:32bits:machine.config")]
		[InlineData("global:net45:machine.config")]
		public void CanResolve(string fileUri)
		{
			_converter.CanResolve(fileUri).Should().BeTrue();
		}

		[Theory]
		[MemberData(nameof(ValidMonikerResolution))]
		public void ResolveSucceeds(string moniker, int expectedCount)
		{
			_converter.Resolve(moniker).Should().HaveCount(expectedCount);
		}

		private readonly DotNetFrameworkConfigurationFileResolver _converter = new DotNetFrameworkConfigurationFileResolver(new List<Version> { new Version(4, 5) });

		public static IEnumerable<object[]> ValidMonikerResolution
		{
			get
			{
				yield return new object[] { "global:net45:32bits:machine.config", 1 };
				yield return new object[] { "global:net45:machine.config", 2 };
			}
		}
	}
}
