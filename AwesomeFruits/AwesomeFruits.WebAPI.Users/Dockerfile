#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app2
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src2
COPY ["../AwesomeFruits.sln", "."]
COPY ["../AwesomeFruits.WebAPI.Users/AwesomeFruits.WebAPI.Users.csproj", "AwesomeFruits.WebAPI.Users/"]
COPY ["../AwesomeFruits.Application/AwesomeFruits.Application.csproj", "AwesomeFruits.Application/"]
COPY ["../AwesomeFruits.Domain/AwesomeFruits.Domain.csproj", "AwesomeFruits.Domain/"]
COPY ["../AwesomeFruits.Infrastructure/AwesomeFruits.Infrastructure.csproj", "AwesomeFruits.Infrastructure/"]

RUN dotnet restore "AwesomeFruits.WebAPI.Users/AwesomeFruits.WebAPI.Users.csproj"

COPY . .
WORKDIR "/src2/AwesomeFruits.WebAPI.Users"
RUN dotnet build "AwesomeFruits.WebAPI.Users.csproj" -c Release -o /app2/build

FROM build AS publish
RUN dotnet publish "AwesomeFruits.WebAPI.Users.csproj" -c Release -o /app2/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app2
COPY --from=publish /app2/publish .
ENTRYPOINT ["dotnet", "AwesomeFruits.WebAPI.Users.dll"]

