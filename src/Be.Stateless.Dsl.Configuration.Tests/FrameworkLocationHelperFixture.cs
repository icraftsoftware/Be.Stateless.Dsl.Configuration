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
using FluentAssertions;
using Xunit;

namespace Be.Stateless.Dsl.Configuration
{
	public class FrameworkLocationHelperFixture
	{
		[Theory]
		[MemberData(nameof(ValidFrameworkVersions))]
		public void GetPathSucceedsFor32Bits(Version version)
		{
			FrameworkLocationHelper.GetPathToDotNetFramework(version, FrameworkArchitecture.Bitness32).Should()
				.MatchRegex(@"C:\\Windows\\Microsoft.NET\\Framework\\v4\.0\.\d*");
		}

		[Theory]
		[MemberData(nameof(ValidFrameworkVersions))]
		public void GetPathSucceedsFor64Bits(Version version)
		{
			FrameworkLocationHelper.GetPathToDotNetFramework(version, FrameworkArchitecture.Bitness64).Should()
				.MatchRegex(@"C:\\Windows\\Microsoft.NET\\Framework64\\v4\.0\.\d*");
		}

		public static IEnumerable<object[]> ValidFrameworkVersions
		{
			get
			{
				yield return new object[] { new Version(4, 0) };
				yield return new object[] { new Version(4, 5) };
				yield return new object[] { new Version(4, 5, 1) };
				yield return new object[] { new Version(4, 5, 2) };
				yield return new object[] { new Version(4, 6) };
				yield return new object[] { new Version(4, 6, 1) };
				yield return new object[] { new Version(4, 6, 2) };
				yield return new object[] { new Version(4, 7) };
				yield return new object[] { new Version(4, 7, 1) };
				yield return new object[] { new Version(4, 7, 2) };
				yield return new object[] { new Version(4, 8) };
			}
		}
	}
}
