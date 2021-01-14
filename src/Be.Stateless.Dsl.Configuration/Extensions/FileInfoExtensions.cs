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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using Be.Stateless.Argument.Validation;
using Be.Stateless.Dsl.Configuration.Constraints;
using Be.Stateless.Dsl.Configuration.Resolvers;
using Be.Stateless.Dsl.Configuration.Specifications;

namespace Be.Stateless.Dsl.Configuration.Extensions
{
    public static class FileInfoExtensions
    {
        public static XmlDocument AsXmlDocument(this FileInfo fileInfo)
        {
            Arguments.Validation.Constraints
                .IsNotNull(fileInfo, nameof(fileInfo))
                .Check();

            var document = new XmlDocument();
            document.Load(fileInfo.FullName);
            return document;
        }

        public static IEnumerable<ConfigurationSpecification> AsConfigurationSpecifications(
            this FileInfo configurationSpecificationFile,
            IEnumerable<IConfigurationFilesResolverStrategy> configurationFileResolverStrategies)
        {
            Arguments.Validation.Constraints
                .IsNotNull(configurationSpecificationFile, nameof(configurationSpecificationFile))
                .Exists(configurationSpecificationFile, nameof(configurationSpecificationFile))
                .Check();

            var configurationSpecifications = new ConfigurationSpecificationReader(configurationSpecificationFile.AsXmlDocument(), configurationFileResolverStrategies).Read();
            foreach (var configurationSpecification in configurationSpecifications)
            {
                configurationSpecification.SpecificationSourceFile = configurationSpecificationFile;
                yield return configurationSpecification;
            }
        }
    }
}
