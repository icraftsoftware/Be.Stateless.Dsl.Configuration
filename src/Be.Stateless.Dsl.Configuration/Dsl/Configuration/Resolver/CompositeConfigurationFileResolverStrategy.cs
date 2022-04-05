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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Be.Stateless.Extensions;

namespace Be.Stateless.Dsl.Configuration.Resolver
{
	internal sealed class CompositeConfigurationFileResolverStrategy
	{
		internal CompositeConfigurationFileResolverStrategy(IEnumerable<IConfigurationFileResolverStrategy> customStrategies)
		{
			_strategies = (customStrategies as IConfigurationFileResolverStrategy[]
					?? customStrategies?.ToArray()
					?? Array.Empty<IConfigurationFileResolverStrategy>())
				.Concat(new IConfigurationFileResolverStrategy[] { new ClrConfigurationFileResolverStrategy(), new ConfigurationFileResolverStrategy() });
		}

		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
		public IEnumerable<string> Resolve(IEnumerable<string> monikers)
		{
			if (monikers?.Any(m => !m.IsNullOrWhiteSpace()) != true) throw new ArgumentNullException(nameof(monikers));
			return monikers
				.SelectMany(moniker => _strategies.Single(resolverStrategy => resolverStrategy.CanResolve(moniker)).Resolve(moniker))
				.Distinct();
		}

		private readonly IEnumerable<IConfigurationFileResolverStrategy> _strategies;
	}
}
