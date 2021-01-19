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
using System.Linq;
using System.Xml;
using Be.Stateless.Dsl.Configuration.Factory;
using Be.Stateless.Dsl.Configuration.Specification;

namespace Be.Stateless.Dsl.Configuration.Command
{
	public sealed class ElementUpdateCommand : ConfigurationCommand
	{
		public ElementUpdateCommand(string configurationElementSelector, IEnumerable<AttributeSpecification> attributeUpdates)
			: base(configurationElementSelector)
		{
			AttributeSpecifications = attributeUpdates ?? Enumerable.Empty<AttributeSpecification>();
		}

		#region Base Class Member Overrides

		internal override ConfigurationCommand Execute(XmlElement configurationElement)
		{
			var undoCommand = ConfigurationCommandFactory.CreateUndoCommandForUpdate(this, configurationElement);
			foreach (var attributeSpecification in AttributeSpecifications) attributeSpecification.Execute(configurationElement);
			return undoCommand;
		}

		#endregion

		public IEnumerable<AttributeSpecification> AttributeSpecifications { get; }
	}
}
