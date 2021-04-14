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
using Be.Stateless.Xml.XPath;

namespace Be.Stateless.Xml.Extensions
{
	internal static class XmlDocumentExtensions
	{
		internal static NamespaceAffinitiveXPathNavigator CreateDslNamespaceAffinitiveXPathNavigator(this XmlDocument document)
		{
			var navigator = document.CreateNamespaceAffinitiveXPathNavigator();
			navigator.NamespaceManager.AddNamespace(Constants.NAMESPACE_URI_PREFIX, Constants.NAMESPACE_URI);
			return navigator;
		}

		internal static IEnumerable<string> GetTargetConfigurationFiles(this XmlDocument document, IEnumerable<IConfigurationFileResolverStrategy> configurationFileResolvers)
		{
			var configurationFileResolver = new ConfigurationFileResolver(
				configurationFileResolvers ?? new IConfigurationFileResolverStrategy[] {
					new ClrConfigurationFileResolverStrategy(), new ConfigurationFileResolverStrategy()
				});

			var monikerExtractor = new ConfigurationFileMonikersExtractor(document);
			return monikerExtractor.Extract()
				.SelectMany(configurationFileResolver.Resolve)
				.Distinct();
		}

		internal static XmlElement PatchDomAfterXPath(this XmlDocument document, string xpath)
		{
			if (xpath.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(xpath), $"'{nameof(xpath)}' cannot be null or empty.");
			return (XmlElement) new XPathExpression(xpath)
				.GetLocationSteps()
				.Aggregate<XPathLocationStep, XmlNode>(document, (current, locationStep) => current.SelectOrAppendElement(locationStep));
		}
	}
}
