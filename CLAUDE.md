# Project rules for Claude

## What this is

NetCoreMQTTExampleIdentityConfig is an example MQTT broker built on
[MQTTnet](https://github.com/dotnet/MQTTnet) that takes its users, their passwords and their topic
whitelists and blacklists from a PostgreSQL database through
[ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
and Entity Framework Core. On top of the broker it exposes a REST API for user and claim
management, documented with Swagger through NSwag. The repository is a code example, it is **not**
published as a NuGet package and there is **no** installer: no `GeneratePackageOnBuild`, no push
script, no `.iss` file, no `Setup` folder.

The sibling repository `D:\Projekte\Github\CSharpUndVB\NetCoreMQTTExampleDapperConfig` is the same
broker on Dapper instead of Identity and EF Core. It shares the `TopicCheck` project, the topic
character table in the readme and most of the host code, so it is the first place to look when a
question here has no answer.

One solution `src/NetCoreMQTTExampleIdentityConfig.sln` with exactly five projects:

- `src/NetCoreMQTTExampleIdentityConfig/NetCoreMQTTExampleIdentityConfig.csproj`, SDK
  `Microsoft.NET.Sdk.Web`, `OutputType` `Exe`. The host: `Program.cs` (Kestrel, Serilog, Windows
  service and systemd integration), `Startup.cs` (DI, Identity, Swagger, certificate),
  `MqttService.cs` (the broker itself as a `BackgroundService`), `LoggerConfig.cs`, the two
  extension classes `DateTimeExtensions.cs` and `DateTimeOffsetExtensions.cs`, and `Controllers/`
  with `UserController` and `ClaimController` plus
  `Controllers/Extensions/ControllerBaseExtensions.cs` and
  `Controllers/Extensions/IdentityErrorExtensions.cs`.
- `src/Storage/Storage.csproj`, `OutputType` `Library`. The data layer: `MqttContext.cs` is the
  `IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>`,
  `Database/` holds the eight entities (`User`, `Role`, `RoleClaim`, `UserClaim`, `UserLogin`,
  `UserRole`, `UserToken`, `DbVersion`), `Dto/` the four transfer objects,
  `Mappings/` the manual mapping between entities and transfer objects,
  `Enumerations/ClaimType.cs` the four claim types, and `DatabaseConnectionSettings.cs` plus
  `MqttSettings.cs` the two configuration objects.
- `src/TopicCheck/TopicCheck.csproj`, `OutputType` `Library`, a single static class `TopicChecker`
  with the single method `Regex` that matches an MQTT topic against an allowed topic pattern. No
  package references except GitVersion.
- `src/TopicCheckerTest/TopicCheckerTest.csproj`, MSTest, the only test project. Three files,
  `TopicCheckerTestsPlusOperator.cs`, `TopicCheckerTestsCrossOperator.cs` and
  `TopicCheckerTestsMultipleOperators.cs`, 13 tests in total, all against `TopicChecker.Regex`.
  Nothing else in the repository is covered by tests.
- `src/DatabaseSetup/DatabaseSetup.csproj`, `OutputType` `Exe`, a console program that drops and
  recreates the database and seeds one `DbVersion`, one user `Hans` with password `Hans` and his
  four claims. Its connection settings are hard coded in `Program.cs`, they are not read from
  `appsettings.json`, and they differ from the ones in `appsettings.json` only in that both use the
  `postgres` user.

Repository root: `README.md` (the only user documentation, including the full table of topic
characters the broker supports and the openssl commands for the certificate), `Changelog.md`,
`License.txt` (MIT), `.gitattributes` and `.gitignore`. There is no `Updating.md`, no `HowToUse.md`
and no screenshots. The empty `.github` folder is untracked, there is no pipeline in this
repository.

## Build

```powershell
dotnet build src/NetCoreMQTTExampleIdentityConfig.sln -c Release
```

```powershell
dotnet test src/NetCoreMQTTExampleIdentityConfig.sln -c Release
```

- Single target framework `net10.0` in all five projects, no multi-targeting, no
  `RuntimeIdentifiers`. Nothing in the code is Windows specific, the host registers both
  `UseWindowsService` and `UseSystemd`.
- All build properties live directly in the five `.csproj` files and are duplicated there. There is
  **no** `Directory.Build.props` in this repository.
- `TreatWarningsAsErrors` is enabled in all projects, so every warning breaks the build, NuGet
  warnings (`NU****`) from restore included. A clean build reports zero warnings, keep it that way.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. Fix
  warnings instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on, so a
  vulnerable transitive package fails the **restore**, before a single file is compiled. That is
  not a hypothetical: version 1.0.13.0 could not be built at all because `AutoMapper` 13.0.1 picked
  up a published advisory, see the known quirks below.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.0.14-1` for the first
  commit after tag `1.0.13`. Never edit a version property or an assembly version by hand.
- The main project writes its XML documentation to `NetCoreMQTTExampleIdentityConfig.xml` **next to
  the project file**, in both Debug and Release, and that file is tracked in git. A build therefore
  shows up as a change to a tracked file. Commit it along with the code that changed it instead of
  reverting it.
- Restore needs nuget.org. If a private feed is configured globally on the machine and answers 404
  for public packages, restore fails with `NU1301`. Then build with an explicit source:
  `dotnet build src/NetCoreMQTTExampleIdentityConfig.sln --source https://api.nuget.org/v3/index.json`.
- `dotnet test` runs the 13 `TopicChecker` tests. They need no network and no database. Everything
  else in the repository has no test coverage at all, so a behaviour change in the broker, the
  controllers or the database layer is **not** covered by a green test run. Never claim a test run
  happened without running it.
- Beyond the tests, a behaviour change in the API or the database layer is verified against a
  throwaway PostgreSQL container, see "Verifying against a real database" below.

## Verifying against a real database

Nothing except `TopicChecker` can be verified without PostgreSQL. Use a throwaway container, never
a database that holds anything.

**Check what owns port 5432 first.** This machine runs a local PostgreSQL 17 service
(`postgresql-x64-17`) that listens on 5432. Publishing a container to `-p 5432:5432` still
succeeds, both end up in `netstat`, and a client connecting to `localhost:5432` reaches the
**local service**, not the container. Since `DatabaseSetup` starts with
`context.Database.EnsureDeleted()`, a run in that state drops the local `mqtt` database. Publish to
a free port instead and check who is listening before connecting:

```powershell
netstat -ano | Select-String ":5433"
docker run -d --rm --name mqtt-verify -e POSTGRES_PASSWORD=postgres -p 5433:5432 postgres:17
```

`DatabaseSetup` hard codes host, port and credentials in `Program.cs` and the main project reads
them from `appsettings.json`, so a verification run on another port means editing both, and both
are tracked files. Restore them with `git checkout --` before committing anything.

Then run `DatabaseSetup` to create the schema and the seed data, start the main project and call
the API on `http://localhost:5000`. `DatabaseSetup` ends in `Console.ReadKey()`, which throws
`InvalidOperationException` when stdin is redirected. That happens after the seeding is committed,
so the exception on a scripted run is expected and means nothing. Remove the container afterwards
with `docker rm -f mqtt-verify`.

The broker itself is verified with a throwaway MQTTnet client against port 8883, not with
`mosquitto_pub`: the certificate is self signed, so the client needs
`.WithTlsOptions(o => o.UseTls().WithSslProtocols(SslProtocols.Tls12).WithCertificateValidationHandler(_ => true))`.
The seeded user `Hans` has `ValidateClientId = true` and an empty `ClientId`, so set
`ValidateClientId` to false in the database first or no client id will ever match. The seed data
makes `d`, `e/+` and `f/#` the whitelist and `a`, `b/+` and `c/#` the blacklist for both
subscribing and publishing, which is enough to exercise all three broker callbacks.

## Code conventions

Follow the surrounding code, it is consistent throughout every file:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` and a
  `<summary>`, then the file-scoped namespace.
- XML doc comments on every type and every member, private members included, no exceptions.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled.
- New `using` directives go into the `GlobalUsings.cs` of the respective project, inside the
  existing `#pragma warning disable IDE0065` block, never at the top of a file. The editorconfig
  requires usings inside the namespace (`csharp_using_directive_placement=inside_namespace:warning`),
  which global usings cannot satisfy, that is what the pragma is for. Do not add other pragmas
  except the `Serilog004` pair that already exists in `MqttService.LogMessage`. The comment text in
  those blocks is German because Visual Studio generated it, leave it alone.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`).
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning. Analyzer
  warnings are fixed, not silenced.
- The indentation inside `MqttContext.OnModelCreating` is visibly broken, the comment lines sit four
  spaces further right than the statements they belong to. That is how Visual Studio left it, it is
  not worth a commit of its own.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **AutoMapper is gone on purpose.** Up to version 1.0.13.0 the mapping between the entities and
  the DTOs went through AutoMapper, with `Storage/Mappings/*Profile.cs` and four `ITypeConverter`
  implementations in `Storage/Converter/`. AutoMapper 13.0.1 is affected by
  [GHSA-rvv3-g6hj-g44x](https://github.com/advisories/GHSA-rvv3-g6hj-g44x), a high severity denial
  of service through uncontrolled recursion. The advisory covers everything below 15.1.1, and
  15.x and 16.x are no longer MIT licensed, they ship a `LICENSE.md` and pull in
  `Microsoft.IdentityModel.JsonWebTokens` to validate a license key. The dependency was therefore
  removed rather than updated, and the mapping now lives in `Storage/Mappings/UserMapper.cs` and
  `Storage/Mappings/UserClaimMapper.cs` as plain extension methods. The sibling repository
  `NetCoreMQTTExampleDapperConfig` did the same in its commit `1993811`. Do not reintroduce
  AutoMapper.
- **The DTOs are deliberately narrower than the entities.** `DtoReadUser` and `DtoCreateUpdateUser`
  have no `ValidateClientId`, no `ThrottleUser` and no `MonthlyByteLimit`, although `User` has all
  three. AutoMapper mapped by name and therefore silently ignored them, and `UserMapper` keeps
  exactly that behaviour so the API contract does not change. A user **created** through the REST
  API therefore always gets `ValidateClientId = false`, `ThrottleUser = false` and
  `MonthlyByteLimit = null`, only `DatabaseSetup` or a direct database write can set those three.
  An **update** leaves them alone, see the next point.
- **`UserController.UpdateUser` writes onto the tracked entity.** It loads the user without
  `AsNoTracking` and copies the request body onto that instance with `UserMapper.ApplyTo`. Do not
  "simplify" that back into building a fresh `User` and handing it to `UserManager.UpdateAsync`:
  the user manager validates before it writes, its duplicate user name check runs a tracking query
  that puts the stored user into the change tracker, and attaching a second instance with the same
  key then throws. That is what made the endpoint answer 500 until version 1.0.14.0.
- **Identity validation failures come back as 500.** Both controllers funnel an unsuccessful
  `IdentityResult` into `ControllerBaseExtensions.InternalServerError`. Posting a user name that
  already exists is a client error but answers 500 with the identity error list in the body. Left
  as it is, changing it would change the documented `ProducesResponseType` set of every endpoint.
- **The claim type goes over the wire as a number.** `DtoReadUserClaim.ClaimType` and
  `DtoCreateUpdateUserClaim.ClaimType` carry `[JsonConverter(typeof(StringEnumConverter))]`, but
  that is the **Newtonsoft** attribute, and `services.AddMvc()` is not followed by
  `AddNewtonsoftJson`, so MVC serializes with System.Text.Json and ignores it. Requests and
  responses therefore use `0` to `3`, not `"SubscriptionBlacklist"`. Sending the name answers 400.
  The `ToString()` in the controllers and in `UserClaimMapper` is unaffected, it never goes through
  a serializer.
- **`ClaimController.CreateOrUpdateClaim` appends, it does not replace.** On an existing claim of
  the same type for the same user it deserializes the stored list, adds the submitted values and
  writes back the `Distinct()` union. There is no way to remove a single topic through that
  endpoint, only `UpdateClaim` with the claim id replaces the list.
- **The claim type is stored as a string.** `UserClaim.ClaimType` is the `string` that
  `IdentityUserClaim<long>` defines, `ClaimType` the enumeration is only the DTO side.
  `UserClaimMapper` converts with `ToString()` in one direction and `Enum.TryParse` in the other,
  and an unparseable value silently becomes `ClaimType.SubscriptionBlacklist`, the first member.
  `MqttService` compares against `ClaimType.SubscriptionBlacklist.ToString()` and friends, so the
  strings in the database must match the enumeration member names exactly.
- **The claim values are a JSON array inside a column.** `UserClaim.ClaimValue` holds
  `["a","b/+","c/#"]` as text. Every read goes through `JsonConvert.DeserializeObject<List<string>>`.
- **`MqttContext.OnModelCreating` must keep calling `base.OnModelCreating`.** The method configures
  all eight entities by hand, which looks like the base call is redundant. It is not: ASP.NET Core
  Identity 10 brings passkey types into the model on its own, nothing here configures them, and
  without the base call the model fails validation with "The entity type 'IdentityPasskeyData'
  requires a primary key to be defined." The hand written configuration runs afterwards and still
  wins, table names (`Users`, `Roles`, ... instead of `AspNetUsers`) included.
- **`MqttContext.OnConfiguring` overrides the DI registration.** `Startup.ConfigureServices` calls
  `AddDbContext` with `DatabaseConnectionSettings.ToConnectionString()`, which carries `Pooling` and
  `Timezone`, but `OnConfiguring` runs afterwards and calls `UseNpgsql` with a second connection
  string built inline from the same settings **without** those two values. The inline string wins.
  Changing `ToConnectionString()` alone therefore has no effect on the context.
- **`MqttService` opens a scope per callback.** The service is a singleton and `MqttContext` is
  scoped, so it takes an `IServiceScopeFactory` and each of `ValidateConnectionAsync`,
  `InterceptSubscriptionAsync` and `InterceptApplicationMessagePublishAsync` starts with

  ```csharp
  using var scope = this.scopeFactory.CreateScope();
  var databaseContext = scope.ServiceProvider.GetRequiredService<MqttContext>();
  ```

  Do not "simplify" that back into a context field. Until version 1.0.15.0 the service held one
  `MqttContext` that `Startup` had built with `new`, shared across every callback of every client.
  A `DbContext` is not thread safe, and the change tracker of a context that lives as long as the
  process also kept handing out the user rows it had already seen, so an API write did not reach
  the broker. `GetClientIdPrefix` takes the context as a parameter for the same reason.
- **There is exactly one MQTT server and Kestrel is not it.** Kestrel serves only the REST API on
  port 5000, every MQTT endpoint belongs to the server `MqttService.StartMqttServer` builds with
  `new MqttServerFactory().CreateMqttServer(...)`. Until version 1.0.16.0 `Program` additionally
  bound Kestrel to 1883 with `UseMqtt()` and `Startup` registered `AddMqttConnectionHandler()`, but
  that endpoint was never wired to this server, so nothing there applied the user, whitelist or
  blacklist checks and a client connecting to 1883 was simply dropped. Do not add it back. The
  project therefore references `MQTTnet.Server`, not `MQTTnet.AspNetCore`.
- **Hand the encryption certificate to MQTTnet as an object, never as exported bytes.**
  `WithEncryptionCertificate` has an overload taking a `byte[]`. Feeding it
  `certificate.Export(X509ContentType.Pfx)` produces a PFX without a password whose private key
  Windows cannot use for server authentication, and every TLS handshake then dies server side
  before any callback runs, while the client only sees the connection being reset. Nothing is
  logged, MQTTnet's internal logger is not wired to Serilog. Pass `this.certificate` directly. That
  is what made port 8883 unreachable until version 1.0.16.0.
- **The example certificate is self signed and valid until 2036-08-12.** It was regenerated in
  version 1.0.16.0, the one shipped before that had expired on 2020-06-20. A client still has to
  trust it explicitly or skip validation, which is normal for a self signed example. Regenerating
  it means replacing all three tracked files `certificate.pfx`, `cert.pem` and `key.pem` together,
  with the openssl commands from the `README.md`, the subject
  `/C=DE/ST=BY/L=Test/O=Test/OU=Test/CN=Test/emailAddress=Test` and the password `test` that both
  `Startup` and the `README.md` hard code. In Git Bash the `-subj` argument needs
  `MSYS_NO_PATHCONV=1`, otherwise MSYS rewrites it into a Windows path.
- **`GetClientIdPrefix` depends on row order.** It walks all users with a non null
  `ClientIdPrefix` and returns the first one the client id starts with. `ClientIdPrefix` defaults
  to the empty string, not to null, so every user with the default passes the filter and
  `clientId.StartsWith("")` matches. Whether a client with a real prefix finds it therefore depends
  on the order PostgreSQL returns the rows in.
- **`IsUserThrottled` keys the cache by client id, not by user.** A user connecting under several
  client ids gets a separate monthly budget per client id. `DataLimitCacheMonth` is
  `MemoryCache.Default`, a process wide static, and the entries expire at the end of the current
  month through `DateTimeOffsetExtensions.EndOfCurrentMonth`.
- **`DateTimeExtensions.GetTimeZoneOffset` hard codes Central European Time.** It returns two hours
  in daylight saving time and one hour otherwise, regardless of the machine's actual time zone. It
  is only used to give the cache entries an expiry.
- **`Npgsql.EnableLegacyTimestampBehavior` is switched on** in both `Program.Main` and
  `DatabaseSetup.Program.Main`, so `DateTimeOffset` columns keep the pre Npgsql 6 behaviour. The
  entities use `DateTimeOffset` throughout, without it the writes fail.
- **AppVeyor badge without CI in the repository.** `README.md` links an AppVeyor build that is
  configured outside of this repository.
- **`src/NetCoreMQTTExampleIdentityConfig.sln.DotSettings`** is tracked and holds nothing but a
  ReSharper user dictionary. Leave it alone.
- **`.gitattributes` sets `* text=auto`**, every rule of the Visual Studio template below it is
  commented out. `certificate.pfx` is therefore covered by the automatic binary detection, not by
  an explicit rule. Any further binary file that is not detected reliably needs its own rule.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.0.14.0 (2026-08-14)** : Short description.`
3. Commit that.
4. Tag the commit with the plain version number, no `v` prefix (`1.0.13`, `1.0.12`, ...). The
   existing tags are lightweight tags, create new ones the same way.
5. Push the commits and the tag.

The version in the `Changelog.md` has four parts (`1.0.14.0`), the tag has three (`1.0.14`).
GitVersion turns the tag into the assembly version, so an untagged commit produces something like
`1.0.14-1+Branch.master.Sha...`. There is no installer to build and no package to push, so the
release ends with the push.

The oldest tag is `1.0.0.1` with four parts, every tag after it has three. Do not take that one as
the pattern.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
