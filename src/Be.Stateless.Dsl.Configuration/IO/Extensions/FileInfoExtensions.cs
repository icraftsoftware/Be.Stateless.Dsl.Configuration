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
using System.IO;
using System.Xml;
using Be.Stateless.Dsl.Configuration;
using Be.Stateless.Dsl.Configuration.Resolver;
using Be.Stateless.Dsl.Configuration.Specification;

namespace Be.Stateless.IO.Extensions
{
	public static class FileInfoExtensions
	{
		public static XmlDocument AsXmlDocument(this FileInfo fileInfo)
		{
			if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
			if (!fileInfo.Exists) throw new FileNotFoundException("The file does not exist.", fileInfo.FullName);

			var document = new XmlDocument();
			document.Load(fileInfo.FullName);
			return document;
		}

		public static IEnumerable<ConfigurationSpecification> AsConfigurationSpecifications(
			this FileInfo configurationSpecificationFile,
			IEnumerable<IConfigurationFilesResolverStrategy> configurationFileResolverStrategies)
		{
			var configurationSpecifications = new ConfigurationSpecificationReader(configurationSpecificationFile.AsXmlDocument(), configurationFileResolverStrategies).Read();
			foreach (var configurationSpecification in configurationSpecifications)
			{
				configurationSpecification.SpecificationSourceFile = configurationSpecificationFile;
				yield return configurationSpecification;
			}
		}
	}
}
