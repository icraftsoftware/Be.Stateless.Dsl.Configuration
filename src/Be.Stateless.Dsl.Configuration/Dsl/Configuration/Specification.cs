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
using Be.Stateless.Dsl.Configuration.Resolver;
using Be.Stateless.Extensions;

namespace Be.Stateless.Dsl.Configuration
{
	public class Specification
	{
		#region Nested Type: Annotations

		public static class Annotations
		{
			#region Nested Type: Attributes

			[SuppressMessage("ReSharper", "InconsistentNaming")]
			public static class Attributes
			{
				public static readonly XName BACKUP_CONFIGURATION_FILE = (XNamespace) NAMESPACE + "backupConfigurationFile";
				public static readonly XName DISCRIMINANT = (XNamespace) NAMESPACE + nameof(DISCRIMINANT).ToLowerInvariant();
				public static readonly XName KEY = (XNamespace) NAMESPACE + nameof(KEY).ToLowerInvariant();
				public static readonly XName[] KEY_AND_ALIASES = { KEY, DISCRIMINANT };
				public static readonly XName OPERATION = (XNamespace) NAMESPACE + nameof(OPERATION).ToLowerInvariant();
				public static readonly XName SCRAP = (XNamespace) NAMESPACE + nameof(SCRAP).ToLowerInvariant();
				public static readonly XName TARGET_CONFIGURATION_FILES = (XNamespace) NAMESPACE + "targetConfigurationFiles";
			}

			#endregion

			#region Nested Type: Operation

			public static class Operation
			{
				public const string DELETE = "delete";
				public const string INSERT = "insert";
				public const string NONE = "none"; // allows to pick an element, if found, that will be used as a pivot to enforce document order
				public const string UPDATE = "update";
				public const string UPSERT = "upsert"; // default operation

				[SuppressMessage("ReSharper", "InconsistentNaming")]
				internal static readonly string[] ALL = { DELETE, INSERT, NONE, UPDATE, UPSERT };
			}

			#endregion

			public const string NAMESPACE = "urn:schemas.stateless.be:dsl:configuration:annotations:2020";

			[SuppressMessage("ReSharper", "InconsistentNaming")]
			public static readonly char[] SEPARATORS = { ' ', '|', ',', ';' };
		}

		#endregion

		#region Operators

		public static explicit operator XDocument(Specification specification)
		{
			return specification.Document;
		}

		public static implicit operator Specification(string text)
		{
			return new(XDocument.Parse(text));
		}

		public static implicit operator Specification(XDocument document)
		{
			return new(document);
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public Specification(XDocument specification)
		{
			Document = specification ?? throw new ArgumentNullException(nameof(specification));
		}

		public string BackupFilePath
		{
			get => Document.Root?.Attribute(Annotations.Attributes.BACKUP_CONFIGURATION_FILE)?.Value;
			set
			{
				if (value.IsNullOrWhiteSpace()) Document.Root?.Attribute(Annotations.Attributes.BACKUP_CONFIGURATION_FILE)?.Remove();
				else Document.Root?.SetAttributeValue(Annotations.Attributes.BACKUP_CONFIGURATION_FILE, value);
			}
		}

		public SpecificationElement Root => new SpecificationRootElement(Document.Root);

		private XDocument Document { get; }

		public Specification Clone()
		{
			return new(new(Document));
		}

		internal IEnumerable<string> GetTargetConfigurationFiles(IEnumerable<IConfigurationFileResolverStrategy> configurationFileResolvers = null)
		{
			var configurationFileResolver = new ConfigurationFileResolver(
				configurationFileResolvers ?? new IConfigurationFileResolverStrategy[] {
					new ClrConfigurationFileResolverStrategy(), new ConfigurationFileResolverStrategy()
				});

			var monikerExtractor = new ConfigurationFileMonikerExtractor(Document);
			return monikerExtractor.Extract()
				.SelectMany(configurationFileResolver.Resolve)
				.Distinct();
		}

		public void Save(string path)
		{
			Document.Save(path);
		}

		public void SetTargetConfigurationFiles(string moniker)
		{
			if (moniker.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(moniker));
			Document.Root?.SetAttributeValue(Annotations.Attributes.TARGET_CONFIGURATION_FILES, moniker);
		}
	}
}
