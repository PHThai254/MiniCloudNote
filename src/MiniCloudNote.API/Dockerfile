# === GIAI ĐOẠN 1: CHẾ BIẾN (BUILD STAGE) ===
# Dùng ảnh chứa đầy đủ công cụ (SDK) để biên dịch code
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Copy các file dự án (.csproj) vào trước để restore (Tối ưu cache)
# Vì cấu trúc của chúng ta có 3 Project, phải copy cả 3
COPY ["src/MiniCloudNote.API/MiniCloudNote.API.csproj", "src/MiniCloudNote.API/"]
COPY ["src/MiniCloudNote.Core/MiniCloudNote.Core.csproj", "src/MiniCloudNote.Core/"]
COPY ["src/MiniCloudNote.Infrastructure/MiniCloudNote.Infrastructure.csproj", "src/MiniCloudNote.Infrastructure/"]

# 2. Tải thư viện về (Restore)
RUN dotnet restore "src/MiniCloudNote.API/MiniCloudNote.API.csproj"

# 3. Copy toàn bộ code còn lại vào
COPY . .

# 4. Biên dịch và Xuất bản (Publish) ra thư mục /app/publish
# -c Release: Chế độ tối ưu cho chạy thật (nhẹ hơn Debug)
WORKDIR "/src/src/MiniCloudNote.API"
RUN dotnet publish "MiniCloudNote.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# === GIAI ĐOẠN 2: ĐÓNG HỘP (RUNTIME STAGE) ===
# Dùng ảnh siêu nhẹ (chỉ có Runtime, không có SDK) để chạy
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Mở cổng 8080 (Cổng mặc định mới của .NET Container)
EXPOSE 8080

# Copy "món ăn đã nấu" từ giai đoạn 1 sang giai đoạn 2
COPY --from=build /app/publish .

# Lệnh chạy khi mở hộp: "Khởi động file .dll này lên!"
ENTRYPOINT ["dotnet", "MiniCloudNote.API.dll"]

