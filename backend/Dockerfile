# GIAI ĐOẠN 1: BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# 1. Chỉ copy 3 dự án cốt lõi (Bỏ qua file .sln và các thư mục Test)
COPY src/MiniCloudNote.Core/*.csproj ./src/MiniCloudNote.Core/
COPY src/MiniCloudNote.Infrastructure/*.csproj ./src/MiniCloudNote.Infrastructure/
COPY src/MiniCloudNote.API/*.csproj ./src/MiniCloudNote.API/

# 2. Restore trực tiếp vào API Project
# (Nó sẽ tự động tìm Core và Infra, và KHÔNG quan tâm đến Test nữa)
RUN dotnet restore src/MiniCloudNote.API/MiniCloudNote.API.csproj

# 3. Copy source code của 3 dự án này
COPY src/MiniCloudNote.Core/. ./src/MiniCloudNote.Core/
COPY src/MiniCloudNote.Infrastructure/. ./src/MiniCloudNote.Infrastructure/
COPY src/MiniCloudNote.API/. ./src/MiniCloudNote.API/

# 4. Build và Publish
WORKDIR /app/src/MiniCloudNote.API
RUN dotnet publish -c Release -o /app/out

# GIAI ĐOẠN 2: RUN (Chạy App)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MiniCloudNote.API.dll"]