# ASP.NET Core API Skeleton

The following application is an ASP.NET Core API Skeleton. This application is intended to provide you with a starting point for building a basic API in ASP.NET core, with sensible and secure defaults. This API is designed to run on any .Net Core platform (Windows, OSX, and Ubuntu). For production purposes, I recommend running this behind Nginx on Ubuntu 16.04.

## Installation

1. Install [.Net Core 1.1](https://www.microsoft.com/net/core)
2. Clone this repository
    ```bash
    git clone https://github.com/charlesportwoodii/aspnetcore-api.git
    cd aspnetcore-api
    ```
3. Install the dependencies
    ```bash
    dotnet restore
    ```
4. Run the migrations
    ```bash
    dotnet ef database update
    ```

## Running & Developing

The API can be run one of three different ways:

- In general `dotnet run` works.
- For development `dotnet watch run` is better.
- For production, consider using the provider `Dockerfile`.