NetCoreMQTTExampleIdentityConfig
====================================

NetCoreMQTTExampleIdentityConfig is a project to check user credentials and topic restrictions from [MQTTnet](https://github.com/chkr1011/MQTTnet)
from a database using [Asp.Net Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-2.2&tabs=visual-studio).
The project was written and tested in .NetCore 2.2.

[![Build status](https://ci.appveyor.com/api/projects/status/6pfhxv7lglc2mvho?svg=true)](https://ci.appveyor.com/project/SeppPenner/netcoremqttexampleidentityconfig)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/NetCoreMQTTExampleIdentityConfig.svg)](https://github.com/SeppPenner/NetCoreMQTTExampleIdentityConfig/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/NetCoreMQTTExampleIdentityConfig.svg)](https://github.com/SeppPenner/NetCoreMQTTExampleIdentityConfig/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/NetCoreMQTTExampleIdentityConfig.svg)](https://github.com/SeppPenner/NetCoreMQTTExampleIdentityConfig/stargazers)
[![GitHub license](https://img.shields.io/badge/license-AGPL-blue.svg)](https://raw.githubusercontent.com/SeppPenner/NetCoreMQTTExampleIdentityConfig/master/License.txt)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/NetCoreMQTTExampleIdentityConfig/badge.svg)](https://snyk.io/test/github/SeppPenner/NetCoreMQTTExampleIdentityConfig)

## Main code:
```csharp
Todo
```

## Attention:
* The project currently only matches topics exactly. I want to provide a regex later, check: https://github.com/eclipse/mosquitto/issues/1317.
* The project only works properly when the ClientId is properly set in the clients (and in the config.json, of course).

## Create an openssl certificate:
```bash
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365
openssl pkcs12 -export -out certificate.pfx -inkey key.pem -in cert.pem
```

An example certificate is in the folder. Password for all is `test`.

Change history
--------------

* **Version 1.0.0.0 (2019-06-21)** : 1.0 release.