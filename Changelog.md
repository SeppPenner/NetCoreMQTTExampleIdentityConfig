# Change history

* **Version 1.0.17.0 (2026-08-15)** : Renewed the example certificate (now the same one in all four MQTT example repositories, valid until 2036-12-31), added the missing check of the client identifier against the client identifier prefix of a user, removed the plaintext password from the log and named the rejected connections there, took the time zone offset from the local time zone, extended the tests from 13 to 37.
* **Version 1.0.16.0 (2026-08-15)** : Removed the dead Kestrel MQTT endpoint, fixed the TLS endpoint that never completed a handshake, renewed the example certificate.
* **Version 1.0.15.0 (2026-08-14)** : Fixed the MQTT service sharing one database context across all its callbacks.
* **Version 1.0.14.0 (2026-08-14)** : Removed AutoMapper and replaced it with manual mapping (security fix), updated NuGet packages, moved to Net 10.0, fixed the unreachable REST API, the never filled duplicate client id check, the failing user update and further defects.
* **Version 1.0.13.0 (2025-01-07)** : Updated Nuget packages, added audit mode, moved to Net9.0.
* **Version 1.0.12.0 (2024-01-04)** : Updated nuget packages, moved to Net 8.0.
* **Version 1.0.11.0 (2022-11-23)** : Updated nuget packages, moved to Net 7.0.
* **Version 1.0.10.0 (2022-08-31)** : Updated nuget packages.
* **Version 1.0.9.0 (2022-02-16)** : Added nullable checks, added editorconfig, added file scoped namespaces, added global usings.
* **Version 1.0.8.0 (2021-11-25)** : Updated nuget packages, moved to Net 6.0.
* **Version 1.0.7.0 (2021-01-02)** : Removed license year, moved changelog to extra file, adjusted folder structure, updated nuget packages, moved to Net 5.0.
* **Version 1.0.6.0 (2020-06-04)** : Updated license year information, updated nuget packages.
* **Version 1.0.5.0 (2020-02-03)** : Added possibility to lock users based on their data limit per month.
* **Version 1.0.4.0 (2020-02-01)** : Updated nuget packages, moved to NetCore 3.1.
* **Version 1.0.2.0 (2019-11-11)** : Added possibility to use all client ids with one user, added better logging.
* **Version 1.0.1.0 (2019-09-29)** : Updated swagger documentation and code style.
* **Version 1.0.0.1 (2019-09-27)** : Updated nuget packages, moved to NetCore 3.0.
* **Version 1.0.0.0 (2019-08-06)** : 1.0 release.