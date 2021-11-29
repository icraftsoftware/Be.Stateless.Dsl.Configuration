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
using System.Xml.Linq;
using Be.Stateless.Dsl.Configuration.Resolver;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Dsl.Configuration
{
	public class SpecificationFixture
	{
		[Fact]
		public void GetBackupFilePathSucceeds()
		{
			Specification specification = XDocument.Parse(
				$"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='moniker' config:backupConfigurationFile='backup'/>");
			specification.BackupFilePath.Should().Be("backup");
		}

		[Fact]
		public void GetBackupFilePathSucceedsWhenAttributeIsMissing()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='moniker' />");
			specification.BackupFilePath.Should().BeNullOrEmpty();
		}

		[Fact]
		public void GetTargetConfigurationFilesSucceeds()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='moniker'/>");
			var resolverStrategyMock = new Mock<IConfigurationFileResolverStrategy>();
			resolverStrategyMock.Setup(strategy => strategy.CanResolve(It.IsAny<string>())).Returns(true);
			resolverStrategyMock.Setup(strategy => strategy.Resolve(It.IsAny<string>())).Returns((Func<string, IEnumerable<string>>) (moniker => new[] { moniker }));
			specification.GetTargetConfigurationFiles(new[] { resolverStrategyMock.Object }).Should().BeEquivalentTo("moniker");
		}

		[Fact]
		public void GetTargetConfigurationFilesSucceedsWhenFilesAttributeIsEmpty()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles=''/>");
			specification.GetTargetConfigurationFiles(Array.Empty<IConfigurationFileResolverStrategy>()).Should().BeEmpty();
		}

		[Fact]
		public void GetTargetConfigurationFilesSucceedsWhenFilesAttributeIsMissing()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'/>");
			Invoking(() => specification.GetTargetConfigurationFiles(Array.Empty<IConfigurationFileResolverStrategy>())).Should().ThrowExactly<InvalidOperationException>();
		}

		[Theory]
		[InlineData((string) null)]
		[InlineData("")]
		[InlineData(" ")]
		public void RemoveBackupFilePathSucceeds(string value)
		{
			Specification specification = XDocument.Parse(
				$"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='moniker' config:backupConfigurationFile='backup'/>");
			specification.BackupFilePath = value;
			((XDocument) specification).Root!.Attribute(Specification.Annotations.Attributes.BACKUP_CONFIGURATION_FILE).Should().BeNull();
		}

		[Fact]
		public void SetBackupFilePathSucceeds()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='moniker'/>");
			const string specificationBackupFilePath = "backup-file-path";
			specification.BackupFilePath = specificationBackupFilePath;
			((XDocument) specification).Root!.Attribute(Specification.Annotations.Attributes.BACKUP_CONFIGURATION_FILE)!.Value.Should().Be(specificationBackupFilePath);
		}

		[Theory]
		[InlineData((string) null)]
		[InlineData("")]
		[InlineData(" ")]
		public void SetFilesMonikerFailed(string value)
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='moniker'/>");
			Invoking(() => specification.SetTargetConfigurationFiles(value)).Should().ThrowExactly<ArgumentNullException>().WithParameterName("moniker");
		}

		[Fact]
		public void SetFilesMonikerSucceeds()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='moniker'/>");
			specification.SetTargetConfigurationFiles("new-moniker");
			((XDocument) specification).Root!.Attribute(Specification.Annotations.Attributes.TARGET_CONFIGURATION_FILES)!.Value.Should().Be("new-moniker");
		}
	}
}
