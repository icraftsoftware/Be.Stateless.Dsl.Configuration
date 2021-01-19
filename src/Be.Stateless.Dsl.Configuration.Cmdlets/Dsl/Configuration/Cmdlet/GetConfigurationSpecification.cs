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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Be.Stateless.Dsl.Configuration.Resolver;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.IO.Extensions;
using Be.Stateless.Management.Automation;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	[Cmdlet(VerbsCommon.Get, nameof(ConfigurationSpecification))]
	[OutputType(typeof(ConfigurationSpecification[]))]
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerShell CmdLet.")]
	public class GetConfigurationSpecification : System.Management.Automation.Cmdlet
	{
		#region Base Class Member Overrides

		protected override void ProcessRecord()
		{
			var resolverStrategies = _defaultConfigurationFileResolverStrategies.Concat(ConfigurationFileResolvers ?? Enumerable.Empty<IConfigurationFilesResolverStrategy>());
			WriteObject(Path.SelectMany(file => file.AsConfigurationSpecifications(resolverStrategies)), true);
		}

		#endregion

		[Parameter(Mandatory = false)]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		public IEnumerable<IConfigurationFilesResolverStrategy> ConfigurationFileResolvers { get; set; }

		[Parameter(Mandatory = true, ValueFromPipeline = true)]
		[ValidateNotNullOrEmpty]
		[ValidateFileExist]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		public FileInfo[] Path { get; set; }

		private static readonly IEnumerable<IConfigurationFilesResolverStrategy> _defaultConfigurationFileResolverStrategies
			= new IConfigurationFilesResolverStrategy[] { new ClrConfigurationFilesResolverStrategy(), new FilesConfigurationFilesResolverStrategy() };
	}
}
