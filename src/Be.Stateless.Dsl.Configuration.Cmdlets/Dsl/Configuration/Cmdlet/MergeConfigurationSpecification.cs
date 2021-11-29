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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml.Linq;
using Be.Stateless.Dsl.Configuration.Resolver;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	/// <summary>
	/// Merges specification file(s) to one ore more configuration files.
	/// </summary>
	/// <example>
	/// <code>
	/// PS> Merge-ConfigurationSpecification -Path ./machine.config
	/// </code>
	/// </example>
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerShell CmdLet.")]
	[Cmdlet(VerbsData.Merge, "ConfigurationSpecification", SupportsShouldProcess = true)]
	[OutputType(typeof(void))]
	public class MergeConfigurationSpecification : PSCmdlet
	{
		#region Base Class Member Overrides

		protected override void BeginProcessing()
		{
			ResolvedFilePaths = Path.SelectMany(ResolvePath).ToArray();
		}

		protected override void ProcessRecord()
		{
			var resolverStrategies = _defaultConfigurationFileResolverStrategies.Concat(ConfigurationFileResolvers).ToArray();
			foreach (var specificationFilePath in ResolvedFilePaths)
			{
				Specification specification = XDocument.Load(specificationFilePath);
				foreach (var configurationFilePath in specification.GetTargetConfigurationFiles(resolverStrategies))
				{
					var token = DateTime.UtcNow.ToString(TOKEN_FORMAT);
					Configuration configuration = XDocument.Load(configurationFilePath);
					var result = configuration.Apply(specification);
					if (result.Configuration.IsDirty && ShouldProcess($"'{configurationFilePath}'", $"Merging '{specificationFilePath}'"))
					{
						if (CreateBackup)
						{
							var backupFilePath = $"{configurationFilePath}.{token}.bak";
							File.Copy(configurationFilePath, backupFilePath);
							result.InverseSpecification.BackupFilePath = backupFilePath;
						}
						if (CreateUndo)
						{
							result.InverseSpecification.SetTargetConfigurationFiles(configurationFilePath);
							result.InverseSpecification.Save($"{specificationFilePath}.{token}.undo");
						}
						result.Configuration.Save(configurationFilePath);
						if (File.Exists(specification.BackupFilePath)) File.Delete(specification.BackupFilePath);
					}
					if (PassThru) WriteObject(result);
				}
			}
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Cmdlet parameter")]
		[Parameter(Mandatory = false)]
		public IConfigurationFileResolverStrategy[] ConfigurationFileResolvers
		{
			get => _configurationFileResolvers ?? Array.Empty<IConfigurationFileResolverStrategy>();
			set => _configurationFileResolvers = value;
		}

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[Parameter]
		public SwitchParameter CreateBackup { get; set; }

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[Parameter]
		public SwitchParameter CreateUndo { get; set; }

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Cmdlet parameter")]
		[Alias("PSPath")]
		[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = nameof(LiteralPath))]
		[ValidateNotNullOrEmpty]
		public string[] LiteralPath
		{
			get => Path;
			set
			{
				Path = value;
				_suppressWildcardExpansion = true;
			}
		}

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[Parameter]
		public SwitchParameter PassThru { get; set; }

		[Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = nameof(Path))]
		[ValidateNotNullOrEmpty]
		public string[] Path { get; set; }

		private string[] ResolvedFilePaths { get; set; }

		private IEnumerable<string> ResolvePath(string path)
		{
			return _suppressWildcardExpansion ? new[] { GetUnresolvedProviderPathFromPSPath(path) } : GetResolvedProviderPathFromPSPath(path, out _);
		}

		private const string TOKEN_FORMAT = "yyyyMMddHHmmssfffffff";

		private static readonly IEnumerable<IConfigurationFileResolverStrategy> _defaultConfigurationFileResolverStrategies
			= new IConfigurationFileResolverStrategy[] { new ClrConfigurationFileResolverStrategy(), new ConfigurationFileResolverStrategy() };

		private IConfigurationFileResolverStrategy[] _configurationFileResolvers;
		private bool _suppressWildcardExpansion;
	}
}
