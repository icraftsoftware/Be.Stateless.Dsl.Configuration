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
using System.Xml.XPath;
using Be.Stateless.Dsl.Configuration;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.Dsl.Configuration.Xml;

namespace Be.Stateless.Xml.XPath.Extensions
{
	internal static class XPathNavigatorExtensions
	{
		public static IEnumerable<AttributeSpecification> GetAttributeUpdates(this XPathNavigator navigator)
		{
			return navigator.AsNamespaceScopedNavigator()
				.Select($"@*[namespace-uri()!='{Constants.NAMESPACE_URI}']")
				.Cast<XPathNavigator>()
				.Select(AttributeSpecificationFactory.Create);
		}

		public static string GetCommandType(this XPathNavigator navigator)
		{
			return navigator.AsNamespaceScopedNavigator().SelectSingleNode($"@{Constants.NAMESPACE_URI_PREFIX}:{XmlAttributeNames.ACTION}")?.Value;
		}

		public static IEnumerable<string> GetDiscriminants(this XPathNavigator navigator)
		{
			return navigator.AsNamespaceScopedNavigator()
					.SelectSingleNode($"@{Constants.NAMESPACE_URI_PREFIX}:{XmlAttributeNames.DISCRIMINANT}")?.Value
					.Split(new[] { Constants.DISCRIMINANT_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries)
				?? Enumerable.Empty<string>();
		}

		internal static NamespaceScopedXPathNavigator AsNamespaceScopedNavigator(this XPathNavigator navigator)
		{
			return navigator is NamespaceScopedXPathNavigator namespaceManagedXPathNavigator
				? namespaceManagedXPathNavigator
				: new NamespaceScopedXPathNavigator(navigator, navigator.BuildNamespaceManager());
		}

		private static XmlNamespaceManager BuildNamespaceManager(this XPathNavigator navigator)
		{
			var namespaceManager = navigator.GetNamespaceManager();
			namespaceManager.AddNamespace(Constants.NAMESPACE_URI_PREFIX, Constants.NAMESPACE_URI);
			return namespaceManager;
		}
	}
}
