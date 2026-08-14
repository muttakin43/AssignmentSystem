# syntax=docker/dockerfile:1

# ---------------------------------------------------------------------------
# Build stage
# ---------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project files first so `dotnet restore` is cached independently
# of source changes. The test project is deliberately excluded — it is not part
# of the API's reference graph.
COPY AssignmentSystem.API/AssignmentSystem.API.csproj                       AssignmentSystem.API/
COPY AssignmentSystem.Application/AssignmentSystem.Application.csproj       AssignmentSystem.Application/
COPY AssignmentSystem.Domain/AssignmentSystem.Domain.csproj                 AssignmentSystem.Domain/
COPY AssignmentSystem.Infrastructure/AssignmentSystem.Infrastructure.csproj AssignmentSystem.Infrastructure/

RUN dotnet restore AssignmentSystem.API/AssignmentSystem.API.csproj

COPY AssignmentSystem.API/            AssignmentSystem.API/
COPY AssignmentSystem.Application/    AssignmentSystem.Application/
COPY AssignmentSystem.Domain/         AssignmentSystem.Domain/
COPY AssignmentSystem.Infrastructure/ AssignmentSystem.Infrastructure/

# Publishing the .csproj rather than the .sln avoids the solution's x64/x86
# platform mappings and skips the test project.
RUN dotnet publish AssignmentSystem.API/AssignmentSystem.API.csproj \
        -c Release \
        -o /app/publish \
        --no-restore \
        /p:UseAppHost=false

# ---------------------------------------------------------------------------
# Runtime stage
# ---------------------------------------------------------------------------
# The ASP.NET image is required: AssignmentSystem.Application declares a
# FrameworkReference on Microsoft.AspNetCore.App.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# The base image already sets ASPNETCORE_HTTP_PORTS=8080, so the listen port needs
# no configuration here. Program.cs overrides it when the host injects PORT.
ENV ASPNETCORE_ENVIRONMENT=Production \
    FileStorage__RootPath=/app/uploads

COPY --from=build /app/publish .

# The app runs as a non-root user, so the directories it writes to must be owned
# by that user up front: /app/uploads for submissions and /app/logs for the
# Serilog rolling file sink configured in Program.cs.
RUN mkdir -p /app/uploads /app/logs \
    && chown -R $APP_UID:$APP_UID /app/uploads /app/logs

USER $APP_UID

EXPOSE 8080

ENTRYPOINT ["dotnet", "AssignmentSystem.API.dll"]