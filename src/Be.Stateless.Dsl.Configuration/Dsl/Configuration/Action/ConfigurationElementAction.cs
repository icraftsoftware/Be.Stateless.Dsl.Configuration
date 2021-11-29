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
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.Dsl.Configuration.Action
{
	public abstract class ConfigurationElementAction
	{
		protected ConfigurationElementAction(string configurationElementSelector)
		{
			if (configurationElementSelector.IsNullOrWhiteSpace())
				throw new ArgumentNullException(nameof(configurationElementSelector), $"'{nameof(configurationElementSelector)}' cannot be null or empty.");
			ConfigurationElementSelector = configurationElementSelector;
		}

		[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
		public string ConfigurationElementSelector { get; }

		public void Execute(XDocument configurationDocument)
		{
			if (configurationDocument == null) throw new ArgumentNullException(nameof(configurationDocument));
			var configurationElement = FindConfigurationElement(configurationDocument);
			Execute(configurationElement);
		}

		[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
		private XElement FindConfigurationElement(XDocument configurationDocument)
		{
			return configurationDocument.XPathSelectElements(ConfigurationElementSelector).Single(
				() => new InvalidOperationException($"No configuration element matching '{ConfigurationElementSelector}' has been found."),
				() => new InvalidOperationException($"More than one configuration element matching '{ConfigurationElementSelector}' have been found."));
		}

		protected internal abstract void Execute(XElement configurationElement);
	}
}
