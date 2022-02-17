#region Copyright & License

# Copyright © 2020 - 2022 François Chabot
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

#endregion

@{
   RootModule            = 'Be.Stateless.Dsl.Configuration.Cmdlets.dll'
   ModuleVersion         = '2.1.0.0'
   GUID                  = '99128609-dd5b-43d7-b834-6bc0ca537f02'
   Author                = 'François Chabot'
   CompanyName           = 'be.stateless'
   Copyright             = '(c) 2020 - 2022 be.stateless. All rights reserved.'
   Description           = 'Configuration DSL library and PowerShell commands for general purpose XML configuration file edition.'
   ProcessorArchitecture = 'None'
   PowerShellVersion     = '5.0'
   NestedModules         = @()
   RequiredAssemblies    = @()
   RequiredModules       = @()
   AliasesToExport       = @()
   CmdletsToExport       = @(
      'Add-ConfigurationElement',
      'Merge-ConfigurationSpecification',
      'Remove-ConfigurationElement',
      'Set-ConfigurationElement'
   )
   FunctionsToExport     = @()
   VariablesToExport     = @()
   PrivateData           = @{
      PSData = @{
         Tags       = @('be.stateless.be', 'icraftsoftware', 'Configuration', 'XML')
         LicenseUri = 'https://github.com/icraftsoftware/Be.Stateless.Dsl.Configuration/blob/master/LICENSE'
         ProjectUri = 'https://github.com/icraftsoftware/Be.Stateless.Dsl.Configuration'
      }
   }
}
