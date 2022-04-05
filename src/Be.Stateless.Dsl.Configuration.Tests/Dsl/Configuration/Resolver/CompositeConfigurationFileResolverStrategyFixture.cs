#region Copyright & License

// Copyright © 2012 - 2022 François Chabot & Emmanuel Benitez
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
using System.Linq;
using System.Runtime.CompilerServices;
using Be.Stateless.Collections.Generic.Extensions;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Dsl.Configuration.Resolver
{
	public class CompositeConfigurationFileResolverStrategyFixture
	{
		[Fact]
		public void ResolveDelegatesToCustomResolver()
		{
			var resolverStrategyMock = new Mock<IConfigurationFileResolverStrategy>();
			resolverStrategyMock.Setup(m => m.CanResolve(It.IsAny<string>())).Returns(true);
			resolverStrategyMock.Setup(m => m.Resolve(It.IsAny<string>())).Returns((Func<string, IEnumerable<string>>) (moniker => new[] { moniker }));

			new[] { "moniker" }.Resolve(resolverStrategyMock.Object).Should().NotBeEmpty();

			resolverStrategyMock.Verify(m => m.CanResolve("moniker"), Times.Once);
			resolverStrategyMock.Verify(m => m.Resolve("moniker"), Times.Once);
		}

		[Fact]
		public void ResolveHasDefaultResolvers()
		{
			new[] { "global:clr4:32bits:machine.config", GetSourceFilePath() }.Resolve()
				.Should().BeEquivalentTo(new ClrConfigurationFileResolverStrategy().Resolve("global:clr4:32bits:machine.config").Append(GetSourceFilePath()));
		}

		[Fact]
		public void ResolveSuppressDuplicates()
		{
			new[] { GetSourceFilePath(), GetSourceFilePath() }.Resolve().Should().HaveCount(1).And.BeEquivalentTo(GetSourceFilePath());
		}

		[Fact]
		public void ResolveThrowsWhenAllResolutionsFail()
		{
			Invoking(() => new[] { "moniker" }.Resolve().ToArray()).Should().ThrowExactly<InvalidOperationException>().WithMessage("Sequence contains no matching element");
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public void ResolveThrowsWhenMonikersAreNullOrEmpty(string moniker)
		{
			Invoking(() => new[] { moniker }.Resolve()).Should().ThrowExactly<ArgumentNullException>().WithParameterName("monikers");
		}

		[Fact]
		public void ResolveThrowsWhenNoMonikers()
		{
			Invoking(() => new string[] { }.Resolve()).Should().ThrowExactly<ArgumentNullException>().WithParameterName("monikers");
		}

		private string GetSourceFilePath([CallerFilePath] string filePath = null)
		{
			return filePath;
		}
	}
}
