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

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.IO.Extensions;

namespace Be.Stateless.Dsl.Configuration
{
	public class ConfigurationSpecificationProcessor
	{
		#region Nested Type: Result

		public class Result
		{
			internal Result(XmlDocument configuration, ConfigurationSpecification undoConfigurationSpecification)
			{
				Configuration = configuration;
				UndoConfigurationSpecification = undoConfigurationSpecification;
			}

			public XmlDocument Configuration { get; }

			public ConfigurationSpecification UndoConfigurationSpecification { get; }
		}

		#endregion

		public ConfigurationSpecificationProcessor(ConfigurationSpecification configurationSpecification)
		{
			_configurationSpecification = configurationSpecification;
		}

		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Assert that the enumeration is not null.")]
		public Result Process()
		{
			return Process(_configurationSpecification.TargetConfigurationFilePath.AsXmlDocument());
		}

		internal Result Process(XmlDocument wipConfiguration)
		{
			var commands = _configurationSpecification.IsUndo ? _configurationSpecification.Commands.Reverse() : _configurationSpecification.Commands;
			var undoConfigurationSpecification = new ConfigurationSpecification(
				_configurationSpecification.TargetConfigurationFilePath,
				commands.Select(command => command.Execute(wipConfiguration)).ToArray(),
				true);
			return new Result(wipConfiguration, undoConfigurationSpecification);
		}

		private readonly ConfigurationSpecification _configurationSpecification;
	}
}
