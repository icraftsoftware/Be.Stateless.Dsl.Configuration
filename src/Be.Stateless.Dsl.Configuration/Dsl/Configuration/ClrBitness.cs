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

namespace Be.Stateless.Dsl.Configuration
{
	/// <summary>
	/// Used to specify the targeted bitness of the .NET Framework.
	/// </summary>
	/// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.build.utilities.dotnetframeworkarchitecture">DotNetFrameworkArchitecture Enum</seealso>
	public enum ClrBitness
	{
		/// <summary>
		/// Indicates the 32-bit .NET Framework.
		/// </summary>
		Bitness32,

		/// <summary>
		/// Indicates the 64-bit .NET Framework.
		/// </summary>
		Bitness64
	}
}
