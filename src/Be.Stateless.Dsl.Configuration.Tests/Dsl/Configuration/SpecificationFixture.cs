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
using System.Xml.Linq;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Dsl.Configuration
{
	public class SpecificationFixture
	{
		[Fact]
		public void GetBackupFilePathSucceeds()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:backupConfigurationFile='backup' />");
			specification.BackupFilePath.Should().Be("backup");
		}

		[Fact]
		public void GetBackupFilePathSucceedsWhenAttributeIsMissing()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' />");
			specification.BackupFilePath.Should().BeNullOrEmpty();
		}

		[Fact]
		public void GetTargetConfigurationFilesSucceedsForSeveralMonikers()
		{
			Specification specification = XDocument.Parse(
				@$"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='c:\program files\moniker|c:\file\moniker|moniker'/>");
			specification.GetTargetConfigurationFiles()
				.Should().BeEquivalentTo(@"c:\program files\moniker", @"c:\file\moniker", "moniker");
		}

		[Fact]
		public void GetTargetConfigurationFilesSucceedsForSingleMoniker()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles='moniker'/>");
			specification.GetTargetConfigurationFiles()
				.Should().BeEquivalentTo("moniker");
		}

		[Fact]
		public void GetTargetConfigurationFilesThrowsWhenEmpty()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' config:targetConfigurationFiles=''/>");
			Invoking(() => specification.GetTargetConfigurationFiles())
				.Should().ThrowExactly<InvalidOperationException>()
				.WithMessage($"The attribute '{Specification.Annotations.Attributes.TARGET_CONFIGURATION_FILES}' is missing or empty.");
		}

		[Fact]
		public void GetTargetConfigurationFilesThrowsWhenMissing()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'/>");
			Invoking(() => specification.GetTargetConfigurationFiles())
				.Should().ThrowExactly<InvalidOperationException>()
				.WithMessage($"The attribute '{Specification.Annotations.Attributes.TARGET_CONFIGURATION_FILES}' is missing or empty.");
		}

		[Fact]
		public void SetBackupFilePathSucceeds()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' />");
			const string specificationBackupFilePath = "backup-file-path";
			specification.BackupFilePath = specificationBackupFilePath;
			((XDocument) specification).Root!.Attribute(Specification.Annotations.Attributes.BACKUP_CONFIGURATION_FILE)!.Value.Should().Be(specificationBackupFilePath);
		}

		[Theory]
		[InlineData((string) null)]
		[InlineData("")]
		[InlineData(" ")]
		public void SetBackupFilePathToNullSucceeds(string value)
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' />");
			specification.BackupFilePath = value;
			((XDocument) specification).Root!.Attribute(Specification.Annotations.Attributes.BACKUP_CONFIGURATION_FILE).Should().BeNull();
		}

		[Fact]
		public void SetTargetConfigurationFilesSucceeds()
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' />");
			specification.SetTargetConfigurationFiles("new-moniker");
			((XDocument) specification).Root!.Attribute(Specification.Annotations.Attributes.TARGET_CONFIGURATION_FILES)!.Value.Should().Be("new-moniker");
		}

		[Theory]
		[InlineData((string) null)]
		[InlineData("")]
		[InlineData(" ")]
		public void SetTargetConfigurationFilesThrows(string value)
		{
			Specification specification = XDocument.Parse($"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' />");
			Invoking(() => specification.SetTargetConfigurationFiles(value)).Should().ThrowExactly<ArgumentNullException>().WithParameterName("moniker");
		}
	}
}
