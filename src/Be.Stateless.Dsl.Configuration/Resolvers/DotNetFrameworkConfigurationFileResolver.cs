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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Be.Stateless.Dsl.Configuration.Extensions;

namespace Be.Stateless.Dsl.Configuration.Resolvers
{
	public class DotNetFrameworkConfigurationFileResolver : IConfigurationFileResolver
	{
		private static bool CanResolve(string moniker, out Match result)
		{
			result = _globalPattern.Match(moniker);
			return result.Success;
		}

		public DotNetFrameworkConfigurationFileResolver(IList<Version> defaultVersions = null)
		{
			_defaultVersions = defaultVersions ?? new List<Version>();
		}

		#region IConfigurationFileResolver Members

		public bool CanResolve(string moniker)
		{
			return CanResolve(moniker, out _);
		}

		public IEnumerable<string> Resolve(string moniker)
		{
			if (!CanResolve(moniker, out var result)) throw new ArgumentException($"The moniker '{moniker}' cannot be converted.", nameof(moniker));

			var parsedVersion = result.Groups["version"].AsVersion();
			var parsedArchitecture = result.Groups["architecture"].AsFrameworkArchitecture();

			return GetPathsToDotNetFramework(parsedVersion, parsedArchitecture)
				.Select(path => Path.Combine(path, "Config", result.Groups["file"].Value))
				.Distinct();
		}

		#endregion

		private IEnumerable<string> GetPathsToDotNetFramework(Version version, FrameworkArchitecture? frameworkArchitecture)
		{
			var versions = version == null ? _defaultVersions : new List<Version> { version };
			var frameworkArchitectures = !frameworkArchitecture.HasValue
				? EnumHelper.GetValues<FrameworkArchitecture>()
				: new List<FrameworkArchitecture> { frameworkArchitecture.Value };

			foreach (var currentVersion in versions)
			{
				foreach (var currentFrameworkArchitecture in frameworkArchitectures)
				{
					yield return FrameworkLocationHelper.GetPathToDotNetFramework(currentVersion, currentFrameworkArchitecture);
				}
			}
		}

		private static readonly Regex _globalPattern = new Regex(@"^global:(?<version>net(\d){2,3})(?::(?<architecture>(32|64)bits))?:(?<file>(machine|web)\.config)$");
		private readonly IList<Version> _defaultVersions;
	}
}
