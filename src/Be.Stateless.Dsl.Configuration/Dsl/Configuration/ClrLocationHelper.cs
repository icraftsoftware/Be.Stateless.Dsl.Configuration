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
using Be.Stateless.Dsl.Configuration.Extensions;
using Microsoft.Win32;

namespace Be.Stateless.Dsl.Configuration
{
	public static class ClrLocationHelper
	{
		[SuppressMessage("ReSharper", "ConvertToUsingDeclaration")]
		private static string GetInstallationRootPath(ClrBitness bitness)
		{
			using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, bitness.ToRegistryView()))
			{
				using (var installRootKey = baseKey.OpenSubKey(NET_FRAMEWORK_REGISTRY_KEY_NAME)
					?? throw new InvalidOperationException($"The key '{NET_FRAMEWORK_REGISTRY_KEY_NAME}' does not exist."))
				{
					return installRootKey.GetValue("InstallRoot") as string;
				}
			}
		}

		public static string GetPath(Version version, ClrBitness bitness)
		{
			var installationRootPath = GetInstallationRootPath(bitness);
			if (!Directory.Exists(installationRootPath)) throw new DirectoryNotFoundException($"The installation root path '{installationRootPath}' is not found.");

			var searchDirectoryPattern = $"v{version.Major}.{version.Minor}*";
			var directories = Directory.GetDirectories(installationRootPath, searchDirectoryPattern, SearchOption.TopDirectoryOnly)
				.OrderBy(d => d, StringComparer.OrdinalIgnoreCase);

			return directories.Single();
		}

		private const string NET_FRAMEWORK_REGISTRY_KEY_NAME = "SOFTWARE\\Microsoft\\.NETFramework";
	}
}
