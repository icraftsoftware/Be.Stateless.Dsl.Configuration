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
using System.Text.RegularExpressions;

namespace Be.Stateless.Dsl.Configuration.Xml.XPath
{
	public class XPathExpression
	{
		public XPathExpression(string value)
		{
			_value = value;
		}

		public IEnumerable<XPathLocationStep> GetLocationSteps()
		{
			var result = _xpathPattern.Match(_value);
			if (!result.Success) throw new InvalidOperationException($"The XPath '{_value}' is not valid.");
			return result.Groups[LOCATION_STEP_GROUP_NAME].Captures.Cast<Capture>().Select(capture => new XPathLocationStep(capture.Value));
		}

		private const string LOCATION_STEP_GROUP_NAME = "step";
		private static readonly Regex _xpathPattern = new Regex($@"^(?:/(?<{LOCATION_STEP_GROUP_NAME}>(?:\*|\w+)(?:\[[^/]+\])?))+$");
		private readonly string _value;
	}
}
