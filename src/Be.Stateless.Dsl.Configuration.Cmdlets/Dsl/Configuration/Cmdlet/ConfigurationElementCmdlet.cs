﻿#region Copyright & License

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
using System.Linq;
using System.Management.Automation;
using Be.Stateless.Dsl.Configuration.Command;
using Be.Stateless.Dsl.Configuration.Resolver;
using Be.Stateless.Dsl.Configuration.Specification;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Cmdlet parameter")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
	public abstract class ConfigurationElementCmdlet : ConfigurationSpecificationCmdlet
	{
		#region Base Class Member Overrides

		[SuppressMessage("ReSharper", "InvertIf")]
		protected override void ProcessRecord()
		{
			var targetConfigurationFiles = TargetConfigurationFile
				.Split(new[] { Constants.FILE_MONIKER_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries)
				.SelectMany(ConfigurationFileResolver.Default.Resolve)
				.Distinct();
			foreach (var specification in targetConfigurationFiles.Select(f => new ConfigurationSpecification(f, new[] { CreateCommand() }, false)))
			{
				ProcessConfigurationSpecification(Action, specification);
			}
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[Alias("ConfigurationFile", "ConfigFile", "File")]
		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string TargetConfigurationFile { get; set; }

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string XPath { get; set; }

		protected abstract string Action { get; }

		protected abstract ConfigurationCommand CreateCommand();
	}
}
