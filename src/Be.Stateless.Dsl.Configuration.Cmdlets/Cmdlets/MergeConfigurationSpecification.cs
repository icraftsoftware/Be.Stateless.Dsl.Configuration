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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Management.Automation;
using System.Xml;
using Be.Stateless.Dsl.Configuration.Extensions;
using Be.Stateless.Dsl.Configuration.Specifications;

namespace Be.Stateless.Dsl.Configuration.Cmdlets
{
    [Cmdlet(VerbsData.Merge, nameof(ConfigurationSpecification), SupportsShouldProcess = true)]
    public class MergeConfigurationSpecification : Cmdlet
    {
        private static string GenerateFilePath(FileInfo sourceFile, string purpose)
        {
            return Path.Combine(
                (sourceFile.Directory ?? throw new InvalidOperationException("The directory is null.")).FullName,
                $"{Path.GetFileNameWithoutExtension(sourceFile.Name)}.{purpose}.{DateTime.Now:yyyyMMddhhmmssffff}{sourceFile.Extension}");
        }

        #region Base Class Member Overrides

        protected override void ProcessRecord()
        {
            foreach (var specification in Specification)
            {
                WriteVerbose($"Merging configuration from '{specification.SpecificationSourceFile.FullName}' into '{specification.TargetConfigurationFile.FullName}'");
                var result = specification.Apply();
                if (ShouldProcess(
                    $"Merging '{specification.SpecificationSourceFile}' on target '{specification.TargetConfigurationFile}'. Result:{Environment.NewLine}{result.ModifiedConfiguration.OuterXml}",
                    string.Empty,
                    string.Empty))
                {
                    if (SaveBeforeMerge) File.Copy(specification.TargetConfigurationFile.FullName, GenerateFilePath(specification.TargetConfigurationFile, "before"));
                    using (var fileStream = specification.TargetConfigurationFile.OpenWrite()) result.ModifiedConfiguration.Save(fileStream);
                    WriteVerbose($"'{specification.TargetConfigurationFile.FullName}' has been updated.");
                    if (GenerateUndo)
                    {
                        var undoConfigurationSpecificationDocument = new XmlDocument();
                        var writer = new ConfigurationSpecificationWriter(undoConfigurationSpecificationDocument);
                        writer.Write(result.UndoConfigurationSpecification);
                        using (var fileStream = File.OpenWrite(GenerateFilePath(specification.SpecificationSourceFile, "undo")))
                            undoConfigurationSpecificationDocument.Save(fileStream);
                    }
                }
            }
        }

        #endregion

        [Parameter]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
        public SwitchParameter GenerateUndo { get; set; }

        [Parameter]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
        public SwitchParameter SaveBeforeMerge { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
        [ValidateNotNullOrEmpty]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
        public ConfigurationSpecification[] Specification { get; set; }
    }
}
