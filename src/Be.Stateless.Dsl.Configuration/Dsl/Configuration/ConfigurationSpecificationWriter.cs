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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using Be.Stateless.Dsl.Configuration.Command;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.Dsl.Configuration.Xml;
using Be.Stateless.Xml.Extensions;

namespace Be.Stateless.Dsl.Configuration
{
	public class ConfigurationSpecificationWriter
	{
		public ConfigurationSpecificationWriter(XmlDocument configurationSpecificationDocument)
		{
			_configurationSpecificationDocument = configurationSpecificationDocument ?? throw new ArgumentNullException(nameof(configurationSpecificationDocument));
		}

		public void Write(ConfigurationSpecification configurationSpecification)
		{
			if (configurationSpecification == null) throw new ArgumentNullException(nameof(configurationSpecification));
			foreach (dynamic command in configurationSpecification.Commands) Write(command);
			_configurationSpecificationDocument.DocumentElement.AppendAttribute(
				XmlAttributeNames.FILES,
				Constants.NAMESPACE_URI,
				Constants.NAMESPACE_URI_PREFIX,
				$"file://{configurationSpecification.TargetConfigurationFilePath}");
			_configurationSpecificationDocument.DocumentElement.AppendAttribute(
				XmlAttributeNames.UNDO,
				Constants.NAMESPACE_URI,
				Constants.NAMESPACE_URI_PREFIX,
				XmlConvert.ToString(configurationSpecification.IsUndo));
		}

		private void Write(ElementInsertionCommand command)
		{
			Write(CommandTypeNames.INSERT, $"{command.ConfigurationElementSelector}/{command.ElementSpecification.Selector}", command.ElementSpecification.AttributeUpdates);
		}

		private void Write(ElementUpdateCommand command)
		{
			Write(CommandTypeNames.UPDATE, $"{command.ConfigurationElementSelector}", command.AttributeSpecifications);
		}

		[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
		private void Write(ElementDeletionCommand command)
		{
			Write(CommandTypeNames.DELETE, $"{command.ConfigurationElementSelector}");
		}

		private void Write(string action, string xpath, IEnumerable<AttributeSpecification> attributeUpdates = null)
		{
			var element = _configurationSpecificationDocument.CreatePath(xpath);
			element.AppendAttribute(XmlAttributeNames.ACTION, Constants.NAMESPACE_URI, Constants.NAMESPACE_URI_PREFIX, action);
			foreach (var attributeUpdate in attributeUpdates ?? Enumerable.Empty<AttributeSpecification>()) attributeUpdate.Execute(element);
		}

		private readonly XmlDocument _configurationSpecificationDocument;
	}
}
