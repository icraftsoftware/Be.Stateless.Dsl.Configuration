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
using Microsoft.Win32;

namespace Be.Stateless.Dsl.Configuration
{
	internal static class ClrBitnessExtensions
	{
		public static RegistryView ToRegistryView(this ClrBitness bitness)
		{
			return bitness switch {
				ClrBitness.Bitness32 => RegistryView.Registry32,
				ClrBitness.Bitness64 => RegistryView.Registry64,
				_ => throw new ArgumentOutOfRangeException(nameof(bitness), bitness, null)
			};
		}
	}
}
