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

using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration.Resolvers
{
	public class FileConfigurationFileResolverFixture
	{
		[Theory]
		[InlineData(@"file:\\c:\web.config")]
		[InlineData(@"file:/c:\web.config")]
		[InlineData(@"file://")]
		public void CannotResolve(string moniker)
		{
			new FileConfigurationFileResolver().CanResolve(moniker).Should().BeFalse();
		}

		[Fact]
		public void CanResolve()
		{
			new FileConfigurationFileResolver().CanResolve(@"file://c:\web.config").Should().BeTrue();
		}
	}
}
