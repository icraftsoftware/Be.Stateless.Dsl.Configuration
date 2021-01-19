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
using System.Text.RegularExpressions;
using Be.Stateless.Dsl.Configuration;

namespace Be.Stateless.Text.RegularExpressions
{
	internal static class GroupExtensions
	{
		public static Version AsClrVersion(this Group value)
		{
			if (!value.Success) return null;
			var result = _clrMonikerPattern.Match(value.Value);
			return !result.Success ? null : new Version(Convert.ToInt32(result.Groups["major"].Value), 0);
		}

		public static ClrBitness? AsClrBitness(this Group value)
		{
			if (!value.Success) return null;
			return value.Value switch {
				"32bits" => ClrBitness.Bitness32,
				"64bits" => ClrBitness.Bitness64,
				_ => throw new ArgumentOutOfRangeException(nameof(value), value.Value, "This bitness is not supported.")
			};
		}

		private static readonly Regex _clrMonikerPattern = new Regex(@"^clr(?<major>\d)");
	}
}
