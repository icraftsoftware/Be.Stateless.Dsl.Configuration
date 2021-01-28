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
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Be.Stateless
{
	public static class StreamExtensions
	{
		internal static XmlDocument AsXmlDocument(this Stream stream)
		{
			var document = new XmlDocument();
			document.Load(stream ?? throw new ArgumentNullException(nameof(stream)));
			return document;
		}

		public static XPathNavigator AsXPathNavigator(this Stream stream)
		{
			return stream.AsXmlDocument()
				.CreateNavigator();
		}
	}
}
