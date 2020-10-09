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
using System.Text.RegularExpressions;

namespace Be.Stateless.Dsl.Configuration.Resolvers
{
    public sealed class FileConfigurationFileResolver : IConfigurationFileResolver
    {
        private static bool CanResolve(string moniker, out Match result)
        {
            result = _filePathPattern.Match(moniker);
            return result.Success;
        }

        #region IConfigurationFileResolver Members

        public bool CanResolve(string moniker)
        {
            return CanResolve(moniker, out _);
        }

        public IEnumerable<string> Resolve(string moniker)
        {
            if (!CanResolve(moniker, out var result)) throw new ArgumentException($"The moniker '{moniker}' cannot be converted.", nameof(moniker));

            yield return result.Groups["path"].Value;
        }

        #endregion

        private static readonly Regex _filePathPattern = new Regex(@"^file://(?<path>.+)$");
    }
}
