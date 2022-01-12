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
using System.Linq;
using System.Text.RegularExpressions;
using Be.Stateless.Dsl.Configuration;

namespace Be.Stateless.Text.RegularExpressions.Extensions
{
	internal static class GroupExtensions
	{
		internal static Version AsClrVersion(this Group value)
		{
			if (!value.Success) return null;
			var result = _clrMonikerPattern.Match(value.Value);
			return !result.Success ? null : new Version(Convert.ToInt32(result.Groups["major"].Value), 0);
		}

		internal static IEnumerable<ClrBitness> AsClrBitness(this Group value)
		{
			return !value.Success
				? typeof(ClrBitness).GetEnumValues().OfType<ClrBitness>()
				: value.Value switch {
					"32bits" => new[] { ClrBitness.Bitness32 },
					"64bits" => new[] { ClrBitness.Bitness64 },
					_ => throw new ArgumentOutOfRangeException(nameof(value), value.Value, "Unexpected CLR bitness value.")
				};
		}

		private static readonly Regex _clrMonikerPattern = new(@"^clr(?<major>\d)");
	}
}
