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
using System.IO;
using System.Linq;
using System.Management.Automation;
using Be.Stateless.Extensions;

namespace Be.Stateless.Management.Automation
{
	public sealed class ValidateFileExistAttribute : ValidateArgumentsAttribute
	{
		#region Base Class Member Overrides

		protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
		{
			switch (arguments)
			{
				case null:
					throw new ArgumentNullException(nameof(arguments));
				case FileInfo file:
					if (!file.Exists) throw new FileNotFoundException("Toto Unable to find the specified file.", file.FullName);
					break;
				case IEnumerable<FileInfo> files:
					files.FirstOrDefault(f => !f.Exists).IfNotNull(f => throw new FileNotFoundException("Tutu Unable to find the specified file.", f.FullName));
					break;
				default:
					throw new ArgumentException($"The parameter type '{arguments.GetType().Name}' is not supported.", nameof(arguments));
			}
		}

		#endregion
	}
}
