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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Be.Stateless.Dsl.Configuration.Action
{
	public class ConfigurationElementUpsertionAction : ConfigurationElementAction
	{
		public ConfigurationElementUpsertionAction(
			string configurationElementSelector,
			ConfigurationElementInsertionAction insertionAction,
			ConfigurationElementUpdateAction updateAction) :
			base(configurationElementSelector)
		{
			InsertionAction = insertionAction ?? throw new ArgumentNullException(nameof(insertionAction));
			UpdateAction = updateAction ?? throw new ArgumentNullException(nameof(updateAction));
		}

		#region Base Class Member Overrides

		protected internal override void Execute(XElement configurationElement)
		{
			if (InsertionAction.Exists(configurationElement)) UpdateAction.Execute(configurationElement.XPathSelectElement(InsertionAction.Selector));
			else InsertionAction.Execute(configurationElement);
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public ConfigurationElementInsertionAction InsertionAction { get; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public ConfigurationElementUpdateAction UpdateAction { get; }
	}
}
