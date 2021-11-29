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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using Be.Stateless.Extensions;

namespace Be.Stateless.Dsl.Configuration
{
	public class Configuration
	{
		#region Operators

		public static explicit operator XDocument(Configuration configuration)
		{
			return configuration.Document;
		}

		public static implicit operator Configuration(string text)
		{
			return new(XDocument.Parse(text));
		}

		public static implicit operator Configuration(XDocument document)
		{
			return new(document);
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public Configuration(XDocument configuration)
		{
			Document = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		public bool IsDirty { get; internal set; }

		public ConfigurationElement Root => new ConfigurationRootElement(Document.Root, this);

		private XDocument Document { get; }

		public SpecificationApplicationResult Apply(Specification specification)
		{
			// result.InverseSpecification is the input specification's clone that will be edited during application to become the inverse specification
			var result = new SpecificationApplicationResult(this, specification);
			result.Configuration.Root.Apply(result.InverseSpecification.Root);
			return result;
		}

		public Configuration Clone()
		{
			return new(new(Document));
		}

		public void Save(string path)
		{
			if (path.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(path));
			Document.Save(path);
		}
	}
}
