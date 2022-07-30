#region Copyright & License

// Copyright © 2012 - 2022 François Chabot & Emmanuel Benitez
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
using System.IO;
using System.Management.Automation;
using System.Xml.Linq;
using Be.Stateless.Collections.Generic.Extensions;
using Be.Stateless.Dsl.Configuration.Action;
using Be.Stateless.Dsl.Configuration.Resolver;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	[SuppressMessage("ReSharper", "MemberCanBeProtected.Global", Justification = "Cmdlet parameter")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
	public abstract class ConfigurationElementCmdlet : System.Management.Automation.Cmdlet
	{
		#region Base Class Member Overrides

		[SuppressMessage("ReSharper", "InvertIf")]
		protected override void ProcessRecord()
		{
			var action = CreateAction();
			foreach (var configurationFilePath in TargetConfigurationFile.Resolve(ConfigurationFileResolver))
			{
				var configuration = XDocument.Load(configurationFilePath);
				action.Execute(configuration);
				if (ShouldProcess($"'{configurationFilePath}'", Action))
				{
					using var fileStream = new FileStream(configurationFilePath, FileMode.Truncate);
					configuration.Save(fileStream);
				}
			}
		}

		#endregion

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[Alias("ConfigurationFileResolvers")]
		[Parameter(Mandatory = false)]
		public IConfigurationFileResolverStrategy[] ConfigurationFileResolver { get; set; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[Alias("ConfigurationFile", "ConfigFile", "File", "Target")]
		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string[] TargetConfigurationFile { get; set; }

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string XPath { get; set; }

		protected abstract string Action { get; }

		protected abstract ConfigurationElementAction CreateAction();
	}
}
