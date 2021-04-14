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
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Be.Stateless.Dsl.Configuration.Command;
using Be.Stateless.Dsl.Configuration.Resolver;
using Be.Stateless.Dsl.Configuration.Specification;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Cmdlet.")]
	[Cmdlet(VerbsCommon.Add, "ConfigurationElement", SupportsShouldProcess = true)]
	[OutputType(typeof(void))]
	public class AddConfigurationElement : System.Management.Automation.Cmdlet
	{
		#region Base Class Member Overrides

		[SuppressMessage("ReSharper", "InvertIf")]
		protected override void ProcessRecord()
		{
			var targetConfigurationFiles = TargetConfigurationFile
				.Split(new[] { Constants.FILE_MONIKER_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries)
				.SelectMany(ConfigurationFileResolver.Default.Resolve)
				.Distinct();
			var command = new ElementInsertionCommand(
				XPath,
				new ElementSpecification(
					ElementName,
					null,
					Attributes?.Keys.Cast<string>().Select(name => new AttributeSpecification { Name = name, Value = (string) Attributes[name] }),
					ElementName + Attributes?.Keys.Cast<string>().Aggregate(string.Empty, (k, name) => $"{k}[@{name}='{Attributes[name]}']")
				));
			foreach (var specification in targetConfigurationFiles.Select(f => new ConfigurationSpecification(f, new[] { command }, false)))
			{
				var result = specification.Apply();
				WriteDebug("Result:{Environment.NewLine}{result.Configuration.OuterXml}");
				if (ShouldProcess($"'{specification.TargetConfigurationFilePath}'", $"Adding configuration element at '{XPath}'"))
				{
					WriteVerbose($"Configuration element at '{XPath}' is being added to '{specification.TargetConfigurationFilePath}'...");
					using (var fileStream = new FileStream(specification.TargetConfigurationFilePath, FileMode.Truncate))
					{
						result.Configuration.Save(fileStream);
					}
					WriteVerbose($"Configuration element at '{XPath}' has been added to '{specification.TargetConfigurationFilePath}'.");
				}
			}
		}

		#endregion

		[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public Hashtable Attributes { get; set; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[Alias("Name")]
		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string ElementName { get; set; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[Alias("ConfigurationFile", "ConfigFile", "File")]
		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string TargetConfigurationFile { get; set; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string XPath { get; set; }
	}
}
