# Foto Main

This repository contains the Foto Main Angular client (`client/`) and ASP.NET Core API (`api/`).

## Configuration

Local settings, deployment files, personal media, and private seed records are intentionally not tracked. Configure the API through environment variables before starting it:

- `ConnectionStrings__SQLConnection`: database connection string
- `TokenKey`: secret key used to sign JWTs
- `SeedAdmin__Email` and `SeedAdmin__Password`: credentials for the initial admin account on an empty database

The API fails to start on an empty database if the initial admin credentials are missing or invalid. Use unique, strong credentials and do not commit them.

## Run locally

Install the .NET 10 SDK to build and run the API with `dotnet run --project api/fotoservice.csproj`. ImageSharp 4.x requires a valid Six Labors license key for Release builds; provide it to MSBuild through the `SixLaborsLicenseKey` property (for example, as an environment variable) and do not commit it. Install the client dependencies with `npm ci` in `client/`, then run `npm start` there. Build the client with `npm run build`; its output is written to `api/wwwroot/`.
