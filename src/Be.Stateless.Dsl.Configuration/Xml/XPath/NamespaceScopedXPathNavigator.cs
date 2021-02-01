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
using System.Xml;
using System.Xml.XPath;

namespace Be.Stateless.Xml.XPath
{
	internal sealed class NamespaceScopedXPathNavigator : XPathNavigatorDecorator
	{
		public NamespaceScopedXPathNavigator(XPathNavigator decoratedNavigator, XmlNamespaceManager xmlNamespaceManager) : base(decoratedNavigator)
		{
			XmlNamespaceManager = xmlNamespaceManager ?? throw new ArgumentNullException(nameof(xmlNamespaceManager));
		}

		#region Base Class Member Overrides

		protected override XPathNavigator CreateXPathNavigatorDecorator(XPathNavigator decoratedNavigator)
		{
			return decoratedNavigator == null ? null : new NamespaceScopedXPathNavigator(decoratedNavigator, XmlNamespaceManager);
		}

		public override object Evaluate(string xpath)
		{
			return base.Evaluate(xpath, XmlNamespaceManager);
		}

		public override XPathNodeIterator Select(string xpath)
		{
			return base.Select(xpath, XmlNamespaceManager);
		}

		public override XPathNavigator SelectSingleNode(string xpath)
		{
			return base.SelectSingleNode(xpath, XmlNamespaceManager);
		}

		#endregion

		#region Base Class Member Overrides

		public override bool Matches(string xpath)
		{
			return base.Matches(XPathExpression.Compile(xpath, XmlNamespaceManager));
		}

		#endregion

		private XmlNamespaceManager XmlNamespaceManager { get; }
	}
}
