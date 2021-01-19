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
using System.Xml;
using Be.Stateless.Argument.Validation;

namespace Be.Stateless.Dsl.Configuration.Command
{
	public class ElementUpsertionCommand : ConfigurationCommand
	{
		public ElementUpsertionCommand(string configurationElementSelector, ElementInsertionCommand insertionCommand, ElementUpdateCommand updateCommand) :
			base(configurationElementSelector)
		{
			Arguments.Validation.Constraints
				.IsNotNull(insertionCommand, nameof(insertionCommand))
				.Check()
				.IsNotNull(updateCommand, nameof(updateCommand))
				.Check();

			InsertionCommand = insertionCommand;
			UpdateCommand = updateCommand;
		}

		#region Base Class Member Overrides

		internal override ConfigurationCommand Execute(XmlElement configurationElement)
		{
			return InsertionCommand.ElementSpecification.Exists(configurationElement)
				? UpdateCommand.Execute((XmlElement) configurationElement.SelectSingleNode(InsertionCommand.ElementSpecification.Selector))
				: InsertionCommand.Execute(configurationElement);
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public ElementInsertionCommand InsertionCommand { get; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public ElementUpdateCommand UpdateCommand { get; }
	}
}
