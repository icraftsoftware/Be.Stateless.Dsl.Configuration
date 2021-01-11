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

namespace Be.Stateless.Dsl.Configuration.Extensions
{
	internal static class GroupExtensions
	{
		public static Version AsVersion(this Group value)
		{
			if (!value.Success) return null;

			var result = _dotNetFrameworkMonikerPattern.Match(value.Value);

			if (!result.Success) return null;
			return result.Groups["build"].Success
				? new Version(
					Convert.ToInt32(result.Groups["major"].Value),
					Convert.ToInt32(result.Groups["minor"].Value),
					Convert.ToInt32(result.Groups["build"].Value))
				: new Version(
					Convert.ToInt32(result.Groups["major"].Value),
					Convert.ToInt32(result.Groups["minor"].Value));
		}

		public static FrameworkArchitecture? AsFrameworkArchitecture(this Group value)
		{
			if (!value.Success) return null;

			switch (value.Value)
			{
				case "32bits":
					return FrameworkArchitecture.Bitness32;
				case "64bits":
					return FrameworkArchitecture.Bitness64;
				default:
					throw new ArgumentOutOfRangeException(nameof(value), value.Value, "The framework architecture is not supported.");
			}
		}

		private static readonly Regex _dotNetFrameworkMonikerPattern = new Regex(@"^net(?<major>\d)(?<minor>\d)(?<build>\d)?$");
	}
}
