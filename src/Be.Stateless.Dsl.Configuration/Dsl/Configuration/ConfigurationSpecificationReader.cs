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
using System.Xml.XPath;
using Be.Stateless.Dsl.Configuration.Command;
using Be.Stateless.Dsl.Configuration.Factory;
using Be.Stateless.Dsl.Configuration.Resolver;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.Dsl.Configuration.Xml;
using Be.Stateless.Xml.Extensions;
using Be.Stateless.Xml.XPath.Extensions;

namespace Be.Stateless.Dsl.Configuration
{
	public sealed class ConfigurationSpecificationReader
	{
		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
		public ConfigurationSpecificationReader(XmlDocument configurationSpecificationDocument, IEnumerable<IConfigurationFilesResolverStrategy> configurationFileResolvers)
		{
			_configurationSpecificationDocument = configurationSpecificationDocument ?? throw new ArgumentNullException(nameof(configurationSpecificationDocument));
			_configurationFileResolvers = configurationFileResolvers?.ToArray() ?? Array.Empty<IConfigurationFilesResolverStrategy>();
		}

		public IEnumerable<ConfigurationSpecification> Read()
		{
			return _configurationSpecificationDocument.GetTargetConfigurationFiles(_configurationFileResolvers)
				.Select(targetFile => new ConfigurationSpecification(targetFile, ReadCommands(), ReadUndo()));
		}

		private IEnumerable<ConfigurationCommand> ReadCommands()
		{
			return _configurationSpecificationDocument.CreateNavigator()
				.AsNamespaceScopedNavigator()
				.Select($"//*[@{Constants.NAMESPACE_URI_PREFIX}:{XmlAttributeNames.ACTION}]")
				.Cast<XPathNavigator>()
				.Select(ConfigurationCommandFactory.Create);
		}

		private bool ReadUndo()
		{
			return _configurationSpecificationDocument.CreateNavigator().AsNamespaceScopedNavigator()
				.SelectSingleNode($"/*/@{Constants.NAMESPACE_URI_PREFIX}:{XmlAttributeNames.UNDO}")?.ValueAsBoolean ?? false;
		}

		private readonly IConfigurationFilesResolverStrategy[] _configurationFileResolvers;
		private readonly XmlDocument _configurationSpecificationDocument;
	}
}
