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
using System.Linq;
using System.Xml;
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.Dsl.Configuration.Command
{
	public abstract class ConfigurationCommand
	{
		protected ConfigurationCommand(string configurationElementSelector)
		{
			if (configurationElementSelector.IsNullOrWhiteSpace())
				throw new ArgumentNullException(nameof(configurationElementSelector), $"'{nameof(configurationElementSelector)}' cannot be null or empty.");
			ConfigurationElementSelector = configurationElementSelector;
		}

		public string ConfigurationElementSelector { get; }

		public ConfigurationCommand Execute(XmlDocument configurationDocument)
		{
			if (configurationDocument == null) throw new ArgumentNullException(nameof(configurationDocument));
			var configurationElement = FindConfigurationElement(configurationDocument);
			return Execute(configurationElement);
		}

		[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
		private XmlElement FindConfigurationElement(XmlDocument configurationDocument)
		{
			var configurationElements = configurationDocument.SelectNodes(ConfigurationElementSelector)?.OfType<XmlElement>();
			if (configurationElements == null || !configurationElements.Any())
				throw new InvalidOperationException($"No configuration element found matching '{ConfigurationElementSelector}'.");
			if (configurationElements.Multiple()) throw new InvalidOperationException($"Multiple configuration elements found matching '{ConfigurationElementSelector}'.");
			return configurationElements.Single();
		}

		internal abstract ConfigurationCommand Execute(XmlElement configurationElement);
	}
}
