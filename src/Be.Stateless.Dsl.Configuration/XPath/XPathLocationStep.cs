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
using System.Text.RegularExpressions;
using Be.Stateless.Argument.Validation;
using Be.Stateless.Dsl.Configuration.Specifications;

namespace Be.Stateless.Dsl.Configuration.XPath
{
    public class XPathLocationStep
    {
        private static IEnumerable<AttributeSpecification> GetAttributeSpecification(Match result)
        {
            for (var index = 0; index < result.Groups[ATTRIBUTE_NAME_GROUP].Captures.Count; index++)
            {
                var attributeName = result.Groups[ATTRIBUTE_NAME_GROUP].Captures[index].Value;
                if (attributeName == XpathFunctionNames.LOCAL_NAME) continue;

                yield return new AttributeSpecification() { Name = attributeName, Value = result.Groups[ATTRIBUTE_VALUE_GROUP].Captures[index].Value };
            }
        }

        private static string GetElementName(Match result)
        {
            if (result.Groups[NAME_GROUP].Success) return result.Groups[NAME_GROUP].Value;
            for (var index = 0; index < result.Groups[ATTRIBUTE_NAME_GROUP].Captures.Count; index++)
            {
                if (result.Groups[ATTRIBUTE_NAME_GROUP].Captures[index].Value == XpathFunctionNames.LOCAL_NAME)
                    return result.Groups[ATTRIBUTE_VALUE_GROUP].Captures[index].Value;
            }
            throw new InvalidOperationException("Neither the name nor the predicate related to local-name is not found.");
        }

        public XPathLocationStep(string value)
        {
            Arguments.Validation.Constraints
                .IsNotNullOrWhiteSpace(value, nameof(value))
                .Check();

            Value = value;
            var result = _xpathLocationStepPattern.Match(Value);
            IsValid = result.Success;
            if (!IsValid) return;
            ElementName = GetElementName(result);
            AttributeSpecifications = GetAttributeSpecification(result);
        }

        public IEnumerable<AttributeSpecification> AttributeSpecifications { get; }

        public string ElementName { get; }

        public bool IsValid { get; }

        public string Value { get; }

        private const string ATTRIBUTE_NAME_GROUP = "attributeName";
        private const string ATTRIBUTE_VALUE_GROUP = "attributeValue";
        private const string NAME_GROUP = "name";

        private static readonly Regex _xpathLocationStepPattern = new Regex(
            @"^(?:\*|(?:(?<prefix>\w+):)?(?<name>\w+))(?:\[(?:\(?\@?(?<attributeName>[\w-]+)(?:\(\))?\s?=\s?'(?<attributeValue>[^']*)'(?:and|or|\s|\))*)+\])?$");
    }
}
