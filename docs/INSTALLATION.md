# Installation and Development

## End-user Requirements

- Windows 10 or Windows 11
- Microsoft Edge WebView2 Runtime
- .NET 9 Desktop Runtime when using a framework-dependent build
- A Windows printer for actual printing

SBPS has no installer project in the repository. A developer or distributor must create and verify a deployment package before presenting it as an official release.

## Development Requirements

- Visual Studio 2022 17.12 or later with **.NET desktop development**, or
- .NET 9 SDK and a compatible editor

The confirmed development environment includes Visual Studio 2022 17.14.

## Restore and Build

```powershell
dotnet restore .\SBPS.sln
dotnet build .\SBPS.sln -c Debug
dotnet build .\SBPS.sln -c Release
```

Run from source:

```powershell
dotnet run --project .\src\SBPS\SBPS.csproj
```

## Optional Publishing

The following command creates a self-contained Windows x64 folder, but the output must be tested before distribution:

```powershell
dotnet publish .\src\SBPS\SBPS.csproj -c Release -r win-x64 --self-contained true -o .\publish\win-x64
```

`publish/` is excluded from source control.

## Source-control Hygiene

Do not commit:

- `.vs/`
- `bin/`
- `obj/`
- `publish/`
- `*.user` and `*.suo`
- logs, local prompts, credentials, or environment-specific secrets

For SVN, set equivalent ignore properties for generated directories. These artifacts are recreated by restore/build operations.
