#region Copyright & License

// Copyright © 2012 - 2020 François Chabot & Emmanuel Benitez
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
using System.Xml;
using Be.Stateless.Dsl.Configuration.Factories;

namespace Be.Stateless.Dsl.Configuration.Commands
{
    public class ElementDeletionCommand : ConfigurationCommand
    {
        public ElementDeletionCommand(string configurationElementSelector) : base(configurationElementSelector) { }

        #region Base Class Member Overrides

        internal override ConfigurationCommand Execute(XmlElement configurationElement)
        {
            if (!configurationElement.IsEmpty)
                throw new InvalidOperationException($"The configuration element '{configurationElement.Name}' is not empty (Content: '{configurationElement.InnerXml}').");

            var undoCommand = ConfigurationCommandFactory.CreateUndoCommandForDeletion(configurationElement);
            var parent = configurationElement.ParentNode
                ?? throw new InvalidOperationException($"The configuration element '{configurationElement.Name}' does not have a parent.");
            parent.RemoveChild(configurationElement);
            return undoCommand;
        }

        #endregion
    }
}
