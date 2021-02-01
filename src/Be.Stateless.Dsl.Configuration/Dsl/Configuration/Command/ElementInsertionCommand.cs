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
using System.Xml;
using Be.Stateless.Dsl.Configuration.Specification;

namespace Be.Stateless.Dsl.Configuration.Command
{
	public sealed class ElementInsertionCommand : ConfigurationCommand
	{
		public ElementInsertionCommand(string configurationElementSelector, ElementSpecification elementSpecification)
			: base(configurationElementSelector)
		{
			ElementSpecification = elementSpecification ?? throw new ArgumentNullException(nameof(elementSpecification));
		}

		#region Base Class Member Overrides

		internal override ConfigurationCommand Execute(XmlElement configurationElement)
		{
			if (ElementSpecification.Exists(configurationElement))
				throw new InvalidOperationException(
					$"The configuration element already exists at '{string.Join("/", ConfigurationElementSelector, ElementSpecification.Selector)}'.");
			var undoCommand = ConfigurationCommandFactory.CreateUndoCommandForInsertion(this);
			configurationElement.AppendChild(ElementSpecification.Execute(configurationElement.OwnerDocument));
			return undoCommand;
		}

		#endregion

		public ElementSpecification ElementSpecification { get; }
	}
}
