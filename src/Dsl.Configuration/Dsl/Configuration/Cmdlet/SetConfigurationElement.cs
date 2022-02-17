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

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using Be.Stateless.Collections.Extensions;
using Be.Stateless.Dsl.Configuration.Action;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	/// <summary>
	/// Set the attributes of an existing configuration element.
	/// </summary>
	/// <example>
	/// <code>
	/// PS> Set-ConfigurationElement -TargetConfigurationFile global:machine.config -XPath "/configuration/appSettings/add[@key='setting1']" -Attributes @{ value = 'value1' }
	/// </code>
	/// </example>
	/// <example>
	/// <code>
	/// PS> Set-ConfigurationElement -TargetConfigurationFile global:machine.config -XPath '/configuration/element' -Attributes @{ '{urn:custom:namespace}attribute' = 'setting1' }
	/// </code>
	/// </example>
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Cmdlet.")]
	[Cmdlet(VerbsCommon.Set, "ConfigurationElement", SupportsShouldProcess = true)]
	[OutputType(typeof(void))]
	public class SetConfigurationElement : ConfigurationElementCmdlet
	{
		#region Base Class Member Overrides

		protected override string Action => $"Updating configuration element at '{XPath}'";

		protected override ConfigurationElementAction CreateAction()
		{
			return new ConfigurationElementUpdateAction(XPath, Attributes.AsXAttributes());
		}

		#endregion

		[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public Hashtable Attributes { get; set; }
	}
}
