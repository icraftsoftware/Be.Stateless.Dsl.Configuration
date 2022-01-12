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
using System.Xml.Linq;
using System.Xml.XPath;
using Be.Stateless.Xml.Linq.Extensions;

namespace Be.Stateless.Dsl.Configuration.Action
{
	public sealed class ConfigurationElementInsertionAction : ConfigurationElementAction
	{
		public ConfigurationElementInsertionAction(string configurationElementSelector, string elementName, IEnumerable<XAttribute> attributes = null)
			: base(configurationElementSelector)
		{
			ElementName = elementName ?? throw new ArgumentNullException(nameof(elementName));
			Attributes = attributes ?? Enumerable.Empty<XAttribute>();
		}

		#region Base Class Member Overrides

		protected internal override void Execute(XElement configurationElement)
		{
			if (Exists(configurationElement))
				throw new InvalidOperationException($"The configuration element already exists at '{string.Join("/", ConfigurationElementSelector, Selector)}'.");
			configurationElement.Add(new XElement(ElementName, Attributes));
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public IEnumerable<XAttribute> Attributes { get; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public XName ElementName { get; }

		public string Selector => $"{ElementName}{Attributes.AsPredicate()}";

		public bool Exists(XElement parentElement)
		{
			if (parentElement == null) throw new ArgumentNullException(nameof(parentElement));
			return parentElement.XPathSelectElement(Selector) != null;
		}
	}
}
