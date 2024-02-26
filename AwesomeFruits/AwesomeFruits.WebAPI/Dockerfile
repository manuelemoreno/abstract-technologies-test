#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# Use the .NET 5 SDK as the base image for building the project
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src


COPY ["../AwesomeFruits.sln", "."]
COPY ["../AwesomeFruits.WebAPI/AwesomeFruits.WebAPI.csproj", "AwesomeFruits.WebAPI/"]
COPY ["../AwesomeFruits.Application/AwesomeFruits.Application.csproj", "AwesomeFruits.Application/"]
COPY ["../AwesomeFruits.Domain/AwesomeFruits.Domain.csproj", "AwesomeFruits.Domain/"]
COPY ["../AwesomeFruits.Infrastructure/AwesomeFruits.Infrastructure.csproj", "AwesomeFruits.Infrastructure/"]

RUN dotnet restore "AwesomeFruits.WebAPI/AwesomeFruits.WebAPI.csproj"


COPY . .
WORKDIR "/src/AwesomeFruits.WebAPI"
RUN dotnet build "AwesomeFruits.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AwesomeFruits.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AwesomeFruits.WebAPI.dll"]