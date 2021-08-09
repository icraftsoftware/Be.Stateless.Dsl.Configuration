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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using Be.Stateless.Dsl.Configuration.Specification;
using Be.Stateless.Dsl.Configuration.Specification.Extensions;
using Be.Stateless.Extensions;

namespace Be.Stateless.Dsl.Configuration.Cmdlet
{
	public abstract class ConfigurationSpecificationCmdlet : System.Management.Automation.Cmdlet
	{
		[SuppressMessage("ReSharper", "InvertIf")]
		protected XmlDocument ProcessConfigurationSpecification(
			string action,
			ConfigurationSpecification configurationSpecification,
			string backupConfigurationFilePath = null,
			string undoConfigurationSpecificationFilePath = null)
		{
			var result = configurationSpecification.Apply();
			if (ShouldProcess($"'{configurationSpecification.TargetConfigurationFilePath}'", action))
			{
				backupConfigurationFilePath.IfNotNullOrEmpty(p => File.Copy(configurationSpecification.TargetConfigurationFilePath, p));
				using (var fileStream = new FileStream(configurationSpecification.TargetConfigurationFilePath, FileMode.Truncate))
				{
					result.Configuration.Save(fileStream);
				}
				undoConfigurationSpecificationFilePath.IfNotNullOrEmpty(p => result.UndoConfigurationSpecification.AsXmlDocument().Save(p));
			}
			return result.Configuration;
		}
	}
}
