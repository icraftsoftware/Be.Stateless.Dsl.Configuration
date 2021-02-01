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
using Be.Stateless.Text.RegularExpressions;

namespace Be.Stateless.Dsl.Configuration.Resolver
{
	public class ClrConfigurationFilesResolverStrategy : IConfigurationFilesResolverStrategy
	{
		private static bool CanResolve(string moniker, out Match result)
		{
			result = _globalPattern.Match(moniker);
			return result.Success;
		}

		private static IEnumerable<string> GetPaths(Version version, ClrBitness? clrBitness)
		{
			var bitnesses = !clrBitness.HasValue
				? Enum.GetValues(typeof(ClrBitness)).OfType<ClrBitness>()
				: new[] { clrBitness.Value };
			return bitnesses.Select(b => ClrLocationHelper.GetPath(version, b));
		}

		#region IConfigurationFilesResolverStrategy Members

		public bool CanResolve(string moniker)
		{
			return CanResolve(moniker, out _);
		}

		public IEnumerable<string> Resolve(string moniker)
		{
			if (!CanResolve(moniker, out var result)) throw new ArgumentException($"The moniker '{moniker}' cannot be resolved.", nameof(moniker));
			var version = result.Groups["version"].AsClrVersion();
			var bitness = result.Groups["bitness"].AsClrBitness();
			return GetPaths(version, bitness)
				.Select(path => Path.Combine(path, "Config", result.Groups["file"].Value))
				.Distinct();
		}

		#endregion

		private static readonly Regex _globalPattern = new Regex(@"^global:(?<version>clr\d)(?::(?<bitness>(32|64)bits))?:(?<file>(machine|web)\.config)$");
	}
}
