#!/bin/bash
# entrypoint.sh

# Wait for SQL Server to start
# Simple wait loop using nc (netcat), adjust according to your needs
while ! nc -z sqlserver 1433; do   
  echo "Waiting for SQL Server to start..."
  sleep 1
done

# Run EF Core migrations
dotnet ef database update --startup-project AwesomeFruits.WebAPI --project AwesomeFruits.Infrastructure

# Start the application
dotnet AwesomeFruits.WebAPI.dll
