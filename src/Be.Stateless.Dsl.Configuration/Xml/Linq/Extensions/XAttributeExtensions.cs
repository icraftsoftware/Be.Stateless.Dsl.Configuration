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
using Be.Stateless.Dsl.Configuration;
using Be.Stateless.Extensions;

namespace Be.Stateless.Xml.Linq.Extensions
{
	internal static class XAttributeExtensions
	{
		internal static string Assemble(this IEnumerable<XName> names)
		{
			return string.Join(Specification.Annotations.SEPARATORS[0].ToString(), names.Select(n => n.ToString()));
		}

		internal static XName[] Disassemble(this string names, XElement namespaceManager)
		{
			return names?.Split(Specification.Annotations.SEPARATORS, StringSplitOptions.RemoveEmptyEntries)
				.Select(n => n.TryParseQName(out var prefix, out var name) && !prefix.IsNullOrEmpty() ? namespaceManager.GetNamespaceOfPrefix(prefix) + name : n)
				.ToArray();
		}

		internal static bool IsAnnotation(this XAttribute attribute)
		{
			return attribute.IsNamespaceDeclaration && attribute.Value == Specification.Annotations.NAMESPACE
				|| attribute.Name.NamespaceName == Specification.Annotations.NAMESPACE;
		}

		internal static string AsPredicate(this IEnumerable<XAttribute> attributes)
		{
			return attributes.Aggregate(string.Empty, (k, attr) => $"{k}[@*[local-name()='{attr.Name.LocalName}' and namespace-uri()='{attr.Name.NamespaceName}']='{attr.Value}']");
		}
	}
}
