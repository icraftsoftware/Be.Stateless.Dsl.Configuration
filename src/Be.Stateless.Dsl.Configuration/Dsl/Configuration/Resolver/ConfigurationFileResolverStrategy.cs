﻿#region Copyright & License

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
using System.IO;

namespace Be.Stateless.Dsl.Configuration.Resolver
{
	public sealed class ConfigurationFileResolverStrategy : IConfigurationFileResolverStrategy
	{
		#region IConfigurationFileResolverStrategy Members

		public bool CanResolve(string moniker)
		{
			return File.Exists(moniker);
		}

		public IEnumerable<string> Resolve(string moniker)
		{
			if (!File.Exists(moniker)) throw new ArgumentException($"The configuration file '{moniker}' does not exist.", nameof(moniker));
			return new[] { moniker };
		}

		#endregion
	}
}
