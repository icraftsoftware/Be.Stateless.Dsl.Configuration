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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Be.Stateless.Argument.Validation;
using Microsoft.Win32;

namespace Be.Stateless.Dsl.Configuration
{
	public static class FrameworkLocationHelper
	{
		#region Nested Type: FrameworkSpecification

		private class FrameworkSpecification
		{
			private static string GetInstallationRootPath(FrameworkArchitecture architecture)
			{
				var keyPath = GetRegistryKeyPath(architecture);

				var installRootKey = Registry.LocalMachine.OpenSubKey(keyPath);
				if (installRootKey == null) throw new InvalidOperationException($"The key '{keyPath}' does not exist.");
				return installRootKey.GetValue("InstallRoot") as string;
			}

			private static string GetRegistryKeyPath(FrameworkArchitecture architecture)
			{
				string keyPath;

				switch (architecture)
				{
					case FrameworkArchitecture.Bitness32:
						keyPath = NET_FRAMEWORK_32_BITS_REGISTRY_KEY_NAME;
						break;
					case FrameworkArchitecture.Bitness64:
						keyPath = NET_FRAMEWORK_64_BITS_REGISTRY_KEY_NAME;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(architecture), architecture, null);
				}
				return keyPath;
			}

			public FrameworkSpecification(Version version, string folderPrefix)
			{
				Arguments.Validation.Constraints
					.IsNotNull(version, nameof(version))
					.IsNotNullOrWhiteSpace(folderPrefix, nameof(folderPrefix))
					.Check();

				Version = version;
				FolderPrefix = folderPrefix;
			}

			[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
			public string FolderPrefix { get; }

			public Version Version { get; }

			public string GetPath(FrameworkArchitecture architecture)
			{
				var installationRootPath = GetInstallationRootPath(architecture);
				if (!Directory.Exists(installationRootPath)) throw new DirectoryNotFoundException($"The installation root path '{installationRootPath}' is not found.");

				var searchDirectoryPattern = $"{FolderPrefix}*";
				var directories = Directory.GetDirectories(installationRootPath, searchDirectoryPattern, SearchOption.TopDirectoryOnly)
					.OrderBy(d => d, StringComparer.OrdinalIgnoreCase);

				return directories.Single();
			}

			private const string NET_FRAMEWORK_32_BITS_REGISTRY_KEY_NAME = "SOFTWARE\\WOW6432Node\\Microsoft\\.NETFramework";
			private const string NET_FRAMEWORK_64_BITS_REGISTRY_KEY_NAME = "SOFTWARE\\Microsoft\\.NETFramework";
		}

		#endregion

		private static FrameworkSpecification CreateFrameworkSpecificationForV4(Version version)
		{
			return new FrameworkSpecification(version, "v4.0");
		}

		public static string GetPathToDotNetFramework(Version version, FrameworkArchitecture architecture)
		{
			var frameworkSpecification = _frameworkSpecifications.SingleOrDefault(s => s.Version == version);

			return frameworkSpecification?.GetPath(architecture);
		}

		private static readonly FrameworkSpecification[] _frameworkSpecifications = {
			CreateFrameworkSpecificationForV4(new Version(4, 0)),
			CreateFrameworkSpecificationForV4(new Version(4, 5)),
			CreateFrameworkSpecificationForV4(new Version(4, 5, 1)),
			CreateFrameworkSpecificationForV4(new Version(4, 5, 2)),
			CreateFrameworkSpecificationForV4(new Version(4, 6)),
			CreateFrameworkSpecificationForV4(new Version(4, 6, 1)),
			CreateFrameworkSpecificationForV4(new Version(4, 6, 2)),
			CreateFrameworkSpecificationForV4(new Version(4, 7)),
			CreateFrameworkSpecificationForV4(new Version(4, 7, 1)),
			CreateFrameworkSpecificationForV4(new Version(4, 7, 2)),
			CreateFrameworkSpecificationForV4(new Version(4, 8))
		};
	}
}
