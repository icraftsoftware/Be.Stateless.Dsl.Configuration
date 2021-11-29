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
using System.Xml.Linq;

namespace Be.Stateless.Dsl.Configuration
{
	public class ConfigurationRootElement : ConfigurationElement
	{
		internal ConfigurationRootElement(XElement rootElement, Configuration configuration) : base(rootElement, configuration) { }

		#region Base Class Member Overrides

		public override IEnumerable<ConfigurationElement> ConfigurationElements()
		{
			return new[] { new ConfigurationElement(Element, Configuration) };
		}

		#endregion
	}
}
