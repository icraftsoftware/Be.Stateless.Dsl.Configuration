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
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerShell CmdLet.")]
	[Cmdlet(VerbsData.Merge, nameof(ConfigurationSpecification), SupportsShouldProcess = true)]
	[OutputType(typeof(void))]
	public class MergeConfigurationSpecification : System.Management.Automation.Cmdlet
	{
		#region Base Class Member Overrides

		[SuppressMessage("ReSharper", "InvertIf")]
		protected override void ProcessRecord()
		{
			foreach (var specification in Specification)
			{
				var result = specification.Apply();
				WriteDebug("Result:{Environment.NewLine}{result.Configuration.OuterXml}");
				if (ShouldProcess($"'{specification.TargetConfigurationFilePath}'", $"Merging '{specification.SpecificationSourceFilePath}'"))
				{
					WriteVerbose($"Configuration '{specification.SpecificationSourceFilePath}' is being merged into '{specification.TargetConfigurationFilePath}'...");
					// ReSharper disable once StringLiteralTypo
					var token = DateTime.UtcNow.ToString("yyyyMMddHHmmssfffffff");
					if (CreateBackup) File.Copy(specification.TargetConfigurationFilePath, $"{specification.TargetConfigurationFilePath}.{token}.bak");
					using (var fileStream = new FileStream(specification.TargetConfigurationFilePath, FileMode.Truncate))
					{
						result.Configuration.Save(fileStream);
					}
					if (CreateUndo) result.UndoConfigurationSpecification.AsXmlDocument().Save($"{specification.SpecificationSourceFilePath}.{token}.undo");
					WriteVerbose($"Configuration '{specification.SpecificationSourceFilePath}' has been merged into '{specification.TargetConfigurationFilePath}'.");
				}
			}
		}

		#endregion

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[Parameter]
		public SwitchParameter CreateBackup { get; set; }

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[Parameter]
		public SwitchParameter CreateUndo { get; set; }

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
		[ValidateNotNullOrEmpty]
		public ConfigurationSpecification[] Specification { get; set; }
	}
}
