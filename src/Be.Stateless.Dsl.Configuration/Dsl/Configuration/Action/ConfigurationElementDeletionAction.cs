﻿#region Copyright & License

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
using System.Xml.Linq;

namespace Be.Stateless.Dsl.Configuration.Action
{
	public class ConfigurationElementDeletionAction : ConfigurationElementAction
	{
		public ConfigurationElementDeletionAction(string configurationElementSelector) : base(configurationElementSelector) { }

		#region Base Class Member Overrides

		protected internal override void Execute(XElement configurationElement)
		{
			if (!configurationElement.IsEmpty)
			{
				using var reader = configurationElement.CreateReader();
				reader.MoveToContent();
				// TODO xpath and not inner xml
				throw new InvalidOperationException($"The configuration element '{configurationElement.Name}' has child nodes (Content: '{reader.ReadInnerXml()}').");
			}
			configurationElement.Remove();
		}

		#endregion
	}
}
