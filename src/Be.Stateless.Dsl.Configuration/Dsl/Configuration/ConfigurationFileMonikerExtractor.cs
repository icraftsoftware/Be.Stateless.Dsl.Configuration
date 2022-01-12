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
using System.Xml.Linq;

namespace Be.Stateless.Dsl.Configuration
{
	public class ConfigurationFileMonikerExtractor
	{
		public ConfigurationFileMonikerExtractor(XDocument document)
		{
			_document = document ?? throw new ArgumentNullException(nameof(document));
		}

		/// <summary>
		/// Extracts the file monikers from the attribute <c>{urn:schemas.stateless.be:dsl:configuration:annotations:2020}files</c>. This attribute must be present on the root element.
		/// </summary>
		/// <returns>
		/// The list of distinct file moniker.
		/// </returns>
		/// <exception cref="InvalidOperationException">When the attribute is not set on the root element.</exception>
		/// <remarks>
		/// Multiple files can be separated by a <c>|</c> character.
		/// </remarks>
		public IEnumerable<string> Extract()
		{
			var filesAttribute = _document.Root?.Attribute(Specification.Annotations.Attributes.TARGET_CONFIGURATION_FILES)
				?? throw new InvalidOperationException(
					$"The attribute '{Specification.Annotations.Attributes.TARGET_CONFIGURATION_FILES}' does not exist on the root element '{_document.Root?.Name}'");
			return filesAttribute.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Distinct();
		}

		private readonly XDocument _document;
	}
}
