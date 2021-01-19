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
using Be.Stateless.Argument.Validation;
using Be.Stateless.Dsl.Configuration.Extensions;

namespace Be.Stateless.Dsl.Configuration.Commands
{
    public abstract class ConfigurationCommand
    {
        protected ConfigurationCommand(string configurationElementSelector)
        {
            Arguments.Validation.Constraints
                .IsNotNullOrEmpty(configurationElementSelector, nameof(configurationElementSelector))
                .Check();
            ConfigurationElementSelector = configurationElementSelector;
        }

        public string ConfigurationElementSelector { get; }

        public ConfigurationCommand Execute(XmlDocument configurationDocument)
        {
            Arguments.Validation.Constraints
                .IsNotNull(configurationDocument, nameof(configurationDocument))
                .Check();
            var configurationElement = FindConfigurationElement(configurationDocument);
            return Execute(configurationElement);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private XmlElement FindConfigurationElement(XmlDocument configurationDocument)
        {
            var configurationElements = (configurationDocument.SelectNodes(ConfigurationElementSelector) ?? throw new InvalidOperationException())
                .OfType<XmlElement>()
                .ToArray();
            if (configurationElements.IsMultiple()) throw new InvalidOperationException($"Found multiple configuration elements at '{ConfigurationElementSelector}'.");
            return configurationElements.SingleOrDefault() ?? throw new InvalidOperationException($"Cannot find configuration element at '{ConfigurationElementSelector}'.");
        }

        internal abstract ConfigurationCommand Execute(XmlElement configurationElement);
    }
}
