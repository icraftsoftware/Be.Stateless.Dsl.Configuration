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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Management.Automation;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.Dsl.Configuration.Specification.Extensions;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	[Cmdlet(VerbsData.Merge, nameof(ConfigurationSpecification), SupportsShouldProcess = true)]
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerShell CmdLet.")]
	public class MergeConfigurationSpecification : System.Management.Automation.Cmdlet
	{
		#region Base Class Member Overrides

		[SuppressMessage("ReSharper", "InvertIf")]
		protected override void ProcessRecord()
		{
			foreach (var specification in Specification)
			{
				var result = specification.Apply();
				if (ShouldProcess(
					$"Merging '{specification.SpecificationSourceFile}' on target '{specification.TargetConfigurationFile}'. Result:{Environment.NewLine}{result.ModifiedConfiguration.OuterXml}",
					string.Empty,
					string.Empty))
				{
					WriteVerbose($"Merging configuration '{specification.SpecificationSourceFile.FullName}' into '{specification.TargetConfigurationFile.FullName}'");
					// ReSharper disable once StringLiteralTypo
					var token = DateTime.UtcNow.ToString("yyyyMMddHHmmssfffffff");
					if (CreateBackup) File.Copy(specification.TargetConfigurationFile.FullName, $"{specification.TargetConfigurationFile.FullName}.{token}.bak");
					using (var fileStream = new FileStream(specification.TargetConfigurationFile.FullName, FileMode.Truncate))
					{
						result.ModifiedConfiguration.Save(fileStream);
					}
					if (CreateUndo) result.UndoConfigurationSpecification.AsXmlDocument().Save($"{specification.SpecificationSourceFile.FullName}.{token}.undo");
					WriteVerbose($"'{specification.TargetConfigurationFile.FullName}' has been updated.");
				}
			}
		}

		#endregion

		[Parameter]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		public SwitchParameter CreateBackup { get; set; }

		[Parameter]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		public SwitchParameter CreateUndo { get; set; }

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
		[ValidateNotNullOrEmpty]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		public ConfigurationSpecification[] Specification { get; set; }
	}
}
