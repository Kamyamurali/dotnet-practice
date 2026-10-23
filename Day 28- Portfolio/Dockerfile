# Optional: run the whole site in Docker (same idea as SQL Server in Docker on Day 7)
#   docker build -t kamya-portfolio .
#   docker run -p 8080:8080 kamya-portfolio            -> http://localhost:8080
#   add -e ConnectionStrings__PortfolioDb="..." to use Azure SQL instead of in-memory

# 1) Build the React app
FROM node:22-alpine AS web
WORKDIR /src/portfolio-web
COPY portfolio-web/package*.json ./
RUN npm ci
COPY portfolio-web/ ./
RUN npm run build            # writes into ../Portfolio.Api/wwwroot

# 2) Publish the .NET API (with the React files in wwwroot)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api
WORKDIR /src
COPY Portfolio.Api/ Portfolio.Api/
COPY --from=web /src/Portfolio.Api/wwwroot Portfolio.Api/wwwroot
RUN dotnet publish Portfolio.Api/Portfolio.Api.csproj -c Release -o /app

# 3) Small runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=api /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Portfolio.Api.dll"]
