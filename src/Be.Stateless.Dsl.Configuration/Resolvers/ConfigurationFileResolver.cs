#region Copyright & License

// Copyright © 2012 - 2020 François Chabot & Emmanuel Benitez
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
using System.Linq;
using Be.Stateless.Argument.Validation;
using Be.Stateless.Dsl.Configuration.Extensions;

namespace Be.Stateless.Dsl.Configuration.Resolvers
{
    public sealed class ConfigurationFileResolver
    {
        public ConfigurationFileResolver(IEnumerable<IConfigurationFileResolver> configurationFileResolvers)
        {
            _fileUriConverters = configurationFileResolvers?.ToList() ?? new List<IConfigurationFileResolver>();
        }

        public IEnumerable<string> Resolve(string moniker)
        {
            Arguments.Validation.Constraints
                .IsNotNullOrWhiteSpace(moniker, nameof(moniker))
                .Check();
            var converters = _fileUriConverters.Where(c => c.CanResolve(moniker)).ToList();
            if (converters.IsMultiple()) throw new InvalidOperationException($"Found multiple moniker resolvers for '{moniker}'.");
            return (converters.SingleOrDefault() ?? throw new InvalidOperationException($"Cannot find moniker resolver for '{moniker}'.")).Resolve(moniker);
        }

        private readonly List<IConfigurationFileResolver> _fileUriConverters;
    }
}
