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
using System.Management.Automation;
using Be.Stateless.Dsl.Configuration.Specification;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerShell CmdLet.")]
	[Cmdlet(VerbsData.Merge, nameof(ConfigurationSpecification), SupportsShouldProcess = true)]
	[OutputType(typeof(void))]
	public class MergeConfigurationSpecification : ConfigurationSpecificationCmdlet
	{
		#region Base Class Member Overrides

		protected override void ProcessRecord()
		{
			foreach (var specification in Specification)
			{
				var token = DateTime.UtcNow.ToString(TOKEN_FORMAT);
				var result = ProcessConfigurationSpecification(
					$"Merging '{specification.SpecificationSourceFilePath}'",
					specification,
					CreateBackup ? $"{specification.TargetConfigurationFilePath}.{token}.bak" : null,
					CreateUndo ? $"{specification.SpecificationSourceFilePath}.{token}.undo" : null);
				if (PassThru) WriteObject(result);
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
		[Parameter]
		public SwitchParameter PassThru { get; set; }

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
		[ValidateNotNullOrEmpty]
		public ConfigurationSpecification[] Specification { get; set; }

		private const string TOKEN_FORMAT = "yyyyMMddHHmmssfffffff";
	}
}
