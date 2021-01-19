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

namespace Be.Stateless.Dsl.Configuration.Validators
{
    public sealed class ValidateFileExistAttribute : ValidateArgumentsAttribute
    {
        #region Base Class Member Overrides

        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (arguments == null) return;
            var files = new List<FileInfo>();
            if (arguments is FileInfo file) files.Add(file);
            else
            {
                if (!(arguments is IEnumerable<FileInfo> filesToProcess)) throw new InvalidOperationException("The parameter is not valid.");
                files.AddRange(filesToProcess);
            }

            foreach (var fileInfo in files.Where(fileInfo => !fileInfo.Exists))
            {
                throw new FileNotFoundException("The file is not found", fileInfo.FullName);
            }
        }

        #endregion
    }
}
