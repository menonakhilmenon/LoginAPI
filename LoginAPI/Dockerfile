#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["LoginAPI/LoginAPI.csproj", "LoginAPI/"]
COPY ["SessionKeyManager/SessionKeyManager.csproj", "SessionKeyManager/"]
COPY ["JwtTokenGenerator/JwtHelpers.csproj", "JwtTokenGenerator/"]
RUN dotnet restore "LoginAPI/LoginAPI.csproj"
COPY . .
WORKDIR "/src/LoginAPI"
RUN dotnet build "LoginAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoginAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoginAPI.dll"]