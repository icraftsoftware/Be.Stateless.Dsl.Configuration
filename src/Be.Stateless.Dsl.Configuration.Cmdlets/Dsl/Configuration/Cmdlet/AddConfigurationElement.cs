﻿#region Copyright & License

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

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using Be.Stateless.Collections.Extensions;
using Be.Stateless.Dsl.Configuration.Action;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	/// <summary>
	/// Adds a configuration element.
	/// </summary>
	/// <example>
	/// <code>
	/// PS> Add-ConfigurationElement -TargetConfigurationFile global:clr4:machine.config -XPath /configuration/appSettings -ElementName add -Attribute @{ key = 'setting'; value = 'value' }
	/// </code>
	/// </example>
	/// <example>
	/// <code>
	/// PS> Add-ConfigurationElement -TargetConfigurationFile global:clr4:machine.config -XPath /configuration -ElementName element -Attribute @{ '{urn:custom:namespace}setting' = 'value' }
	/// </code>
	/// </example>
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Cmdlet.")]
	[Cmdlet(VerbsCommon.Add, "ConfigurationElement", SupportsShouldProcess = true)]
	[OutputType(typeof(void))]
	public class AddConfigurationElement : ConfigurationElementCmdlet
	{
		#region Base Class Member Overrides

		protected override string Action => $"Appending configuration element at '{XPath}'";

		protected override ConfigurationElementAction CreateAction()
		{
			return new ConfigurationElementInsertionAction(XPath, ElementName, Attribute?.AsXAttributes());
		}

		#endregion

		[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[Alias("Attributes")]
		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public Hashtable Attribute { get; set; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Cmdlet parameter")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Cmdlet parameter")]
		[Alias("Name")]
		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string ElementName { get; set; }
	}
}
