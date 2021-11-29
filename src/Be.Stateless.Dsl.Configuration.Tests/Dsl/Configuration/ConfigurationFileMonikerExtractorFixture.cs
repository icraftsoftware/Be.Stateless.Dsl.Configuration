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
using System.Xml.Linq;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Dsl.Configuration
{
	public class ConfigurationFileMonikerExtractorFixture
	{
		[Fact]
		public void ExtractFailed()
		{
			Invoking(
					() => new ConfigurationFileMonikerExtractor(XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'/>")).Extract())
				.Should().ThrowExactly<InvalidOperationException>()
				.WithMessage($"The attribute '{{{Specification.Annotations.NAMESPACE}}}targetConfigurationFiles' does not exist on the root element 'configuration'");
		}

		[Fact]
		public void ExtractsSucceedsForNoneMoniker()
		{
			new ConfigurationFileMonikerExtractor(XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles=''/>"))
				.Extract()
				.Should().BeEmpty();
		}

		[Fact]
		public void ExtractSucceedsForSeveralMonikers()
		{
			new ConfigurationFileMonikerExtractor(
					XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='moniker,moniker1|moniker moniker2'/>"))
				.Extract()
				.Should().BeEquivalentTo("moniker", "moniker1", "moniker2");
		}

		[Fact]
		public void ExtractSucceedsForSingleMoniker()
		{
			new ConfigurationFileMonikerExtractor(
					XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='moniker'/>"))
				.Extract()
				.Should().BeEquivalentTo("moniker");
		}
	}
}
