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
using System.Diagnostics.Contracts;
using System.IO;
using Be.Stateless.Argument.Validation;

namespace Be.Stateless.Dsl.Configuration.Constraints
{
    internal static class FileInfoConstraints
    {
        [Pure]
        public static TV Exists<TV>(this TV validator, FileInfo parameter, string parameterName)
            where TV : IArgumentConstraint
        {
            return parameter?.Exists ?? true
                ? validator
                : validator.AddException(new ArgumentException($"The file '{parameter.FullName}' does not exist.", parameterName));
        }
    }
}
