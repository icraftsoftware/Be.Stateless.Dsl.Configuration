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
using System.Linq;
using System.Xml;
using Be.Stateless.Argument.Validation;
using Be.Stateless.Dsl.Configuration.Resolvers;
using Be.Stateless.Dsl.Configuration.XPath;

namespace Be.Stateless.Dsl.Configuration.Extensions
{
    public static class XmlDocumentExtensions
    {
        public static XmlElement CreatePath(this XmlDocument document, string xpath)
        {
            Arguments.Validation.Constraints
                .IsNotNullOrWhiteSpace(xpath, nameof(xpath))
                .Check();
            var locationSteps = new XPathExpression(xpath).GetLocationSteps();
            XmlNode node = document;
            foreach (var locationStep in locationSteps) node = node.SelectOrAppendElement(locationStep);
            return (XmlElement) node;
        }

        public static IEnumerable<FileInfo> GetTargetConfigurationFiles(this XmlDocument document, IEnumerable<IConfigurationFilesResolverStrategy> configurationFileResolvers)
        {
            var monikerExtractor = new ConfigurationFileMonikersExtractor(document);

            var configurationFileResolver = new ConfigurationFilesResolver(
                configurationFileResolvers ?? new IConfigurationFilesResolverStrategy[]
                    { new ClrConfigurationFilesResolverStrategy(), new FilesConfigurationFilesResolverStrategy() });

            return monikerExtractor.Extract()
                .SelectMany(configurationFileResolver.Resolve)
                .Distinct()
                .Select(path => new FileInfo(path));
        }
    }
}
