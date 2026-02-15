# === GIAI ĐOẠN 1: CHẾ BIẾN (BUILD STAGE) ===
# Dùng ảnh chứa đầy đủ công cụ (SDK) để biên dịch code
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# 1. COPY CÁC FILE DỰ ÁN (.csproj) TRƯỚC 
# Mẹo: Dùng *.csproj để đỡ phải gõ tên file dài dòng, và copy giữ nguyên cấu trúc thư mục
# Lưu ý: Lệnh COPY này giả định Build Context là thư mục gốc MiniCloudNote
COPY src/MiniCloudNote.API/*.csproj ./src/MiniCloudNote.API/
COPY src/MiniCloudNote.Core/*.csproj ./src/MiniCloudNote.Core/
COPY src/MiniCloudNote.Infrastructure/*.csproj ./src/MiniCloudNote.Infrastructure/

# 2. Tải thư viện về (Restore)
RUN dotnet restore "src/MiniCloudNote.API/MiniCloudNote.API.csproj"

# 3. Copy toàn bộ code còn lại vào
COPY . .

# 4. Biên dịch và Xuất bản (Publish) ra thư mục /app/publish
# -c Release: Chế độ tối ưu cho chạy thật (nhẹ hơn Debug)
WORKDIR /app/src/MiniCloudNote.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# === GIAI ĐOẠN 2: ĐÓNG HỘP (RUNTIME STAGE) ===
# Dùng ảnh siêu nhẹ (chỉ có Runtime, không có SDK) để chạy
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Biến môi trường để ép .NET luôn lắng nghe port 8080 (Docker chuẩn)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Copy "món ăn đã nấu" từ giai đoạn 1 sang giai đoạn 2
COPY --from=build /app/publish .

# Lệnh chạy khi mở hộp: "Khởi động file .dll này lên!"
ENTRYPOINT ["dotnet", "MiniCloudNote.API.dll"]

