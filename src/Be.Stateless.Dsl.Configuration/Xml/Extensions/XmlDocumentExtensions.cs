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
using System.Linq;
using System.Xml;
using Be.Stateless.Dsl.Configuration;
using Be.Stateless.Dsl.Configuration.Resolver;
using Be.Stateless.Dsl.Configuration.Xml.XPath;
using Be.Stateless.Extensions;

namespace Be.Stateless.Xml.Extensions
{
	public static class XmlDocumentExtensions
	{
		public static XmlElement CreatePath(this XmlDocument document, string xpath)
		{
			if (xpath.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(xpath), $"'{nameof(xpath)}' cannot be null or empty.");
			return (XmlElement) new XPathExpression(xpath)
				.GetLocationSteps()
				.Aggregate<XPathLocationStep, XmlNode>(document, (current, locationStep) => current.SelectOrAppendElement(locationStep));
		}

		public static IEnumerable<string> GetTargetConfigurationFiles(this XmlDocument document, IEnumerable<IConfigurationFilesResolverStrategy> configurationFileResolvers)
		{
			var monikerExtractor = new ConfigurationFileMonikersExtractor(document);
			var configurationFileResolver = new ConfigurationFilesResolver(
				configurationFileResolvers ?? new IConfigurationFilesResolverStrategy[] {
					new ClrConfigurationFilesResolverStrategy(), new FilesConfigurationFilesResolverStrategy()
				});

			return monikerExtractor.Extract()
				.SelectMany(configurationFileResolver.Resolve)
				.Distinct();
		}
	}
}
