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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.Dsl.Configuration;

// ReSharper disable once IdentifierTypo
namespace Be.Stateless.Xunit
{
	public class TestCaseData
	{
		#region Operators

		public static implicit operator object[](TestCaseData testCaseData)
		{
			return new object[] { testCaseData };
		}

		#endregion

		public TestCaseData(string name)
		{
			Name = name;
		}

		#region Base Class Member Overrides

		public override string ToString()
		{
			return Name;
		}

		#endregion

		public Specification BackwardSpecification { get; set; }

		public string Error { get; set; }

		public Specification ForwardSpecification { get; set; }

		public Configuration InputConfiguration { get; set; }

		public bool IsDirty => _outputConfiguration != null;

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public string Name { get; }

		public Configuration OutputConfiguration
		{
			get => _outputConfiguration ?? InputConfiguration;
			set => _outputConfiguration = value;
		}

		private Configuration _outputConfiguration;
	}
}
