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
using System.Text.RegularExpressions;

namespace Be.Stateless.Dsl.Configuration.Resolver
{
	public sealed class ConfigurationFileResolverStrategy : IConfigurationFileResolverStrategy
	{
		#region IConfigurationFileResolverStrategy Members

		public bool CanResolve(string moniker)
		{
			return TryResolve(moniker, out _);
		}

		public IEnumerable<string> Resolve(string moniker)
		{
			if (!TryResolve(moniker, out var match)) throw new ArgumentException($"The moniker '{moniker}' cannot be converted.", nameof(moniker));
			return new[] { match.Groups["path"].Value };
		}

		#endregion

		private bool TryResolve(string moniker, out Match match)
		{
			match = _filePathPattern.Match(moniker);
			return match.Success;
		}

		private static readonly Regex _filePathPattern = new Regex(@"^file://(?<path>.+)$");
	}
}
