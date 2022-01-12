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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Be.Stateless.Extensions;

namespace Be.Stateless.Dsl.Configuration.Resolver
{
	public sealed class ConfigurationFileResolver
	{
		public static ConfigurationFileResolver Default { get; } = new(
			new IConfigurationFileResolverStrategy[] { new ClrConfigurationFileResolverStrategy(), new ConfigurationFileResolverStrategy() });

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public ConfigurationFileResolver(IEnumerable<IConfigurationFileResolverStrategy> resolverStrategies)
		{
			_resolverStrategies = resolverStrategies?.ToArray() ?? Array.Empty<IConfigurationFileResolverStrategy>();
		}

		public IEnumerable<string> Resolve(string moniker)
		{
			if (moniker.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(moniker), $"'{nameof(moniker)}' cannot be null or empty.");
			return _resolverStrategies.Single(resolverStrategy => resolverStrategy.CanResolve(moniker)).Resolve(moniker);
		}

		private readonly IEnumerable<IConfigurationFileResolverStrategy> _resolverStrategies;
	}
}
