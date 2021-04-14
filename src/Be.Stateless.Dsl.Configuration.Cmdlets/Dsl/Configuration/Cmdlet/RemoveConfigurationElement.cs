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

using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using Be.Stateless.Dsl.Configuration.Command;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Cmdlet.")]
	[Cmdlet(VerbsCommon.Remove, "ConfigurationElement", SupportsShouldProcess = true)]
	[OutputType(typeof(void))]
	public class RemoveConfigurationElement : ConfigurationElementCmdlet
	{
		#region Base Class Member Overrides

		protected override string Action => $"Deleting configuration element at '{XPath}'";

		protected override ConfigurationCommand CreateCommand()
		{
			return new ElementDeletionCommand(XPath);
		}

		#endregion
	}
}
