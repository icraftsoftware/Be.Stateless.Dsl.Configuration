# Be.Stateless.Dsl.Configuration

[![Build Status](https://dev.azure.com/icraftsoftware/be.stateless/_apis/build/status/Be.Stateless.Dsl.Configuration%20Manual%20Release?branchName=master)](https://dev.azure.com/icraftsoftware/be.stateless/_build/latest?definitionId=79&branchName=master)
[![GitHub Release](https://img.shields.io/github/v/release/icraftsoftware/Be.Stateless.Dsl.Configuration?label=Release&logo=github)](https://github.com/icraftsoftware/Be.Stateless.Dsl.Configuration/releases/latest)
[![PowerShell Gallery Version](https://img.shields.io/powershellgallery/v/Dsl.Configuration.svg?style=flat&logo=powershell)](https://www.powershellgallery.com/packages/Dsl.Configuration)

Configuration DSL library and PowerShell commands for general purpose configuration file edition.

## NuGet Packages

[![NuGet Version](https://img.shields.io/nuget/v/Be.Stateless.Dsl.Configuration.svg?label=Be.Stateless.Dsl.Configuration&style=flat&logo=nuget)](https://www.nuget.org/packages/Be.Stateless.Dsl.Configuration/)


## Module Installation

Notice that to be able to install this PowerShell module right from the PowerShell Gallery you will need to trust the certificate that was used to sign it. Run the following PowerShell commands (they merely download and install the certificate's public key in the 'Trusted People' store underneath the 'Local Machine' certifcate store):

```PowerShell
Invoke-WebRequest -Uri https://github.com/icraftsoftware/Be.Stateless.Build.Scripts/raw/master/be.stateless.public.cer -OutFile "$($env:TEMP)\be.stateless.public.cer"
Import-Certificate -FilePath "$($env:TEMP)\be.stateless.public.cer" -CertStoreLocation Cert:\LocalMachine\TrustedPeople\
```

Notice that if the `ExecutionPolicy` is set to `AllSigned` you also need to run the following PowerShell command (it merely installs the certificate's public key in the 'Trusted Publishers' store underneath the 'Local Machine' certifcate store):

```PowerShell
Import-Certificate -FilePath "$($env:TEMP)\be.stateless.public.cer" -CertStoreLocation Cert:\LocalMachine\TrustedPublisher\
```
