using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args; // Dùng cho bản Minio mới
using MiniCloudNote.Core.Interfaces;

namespace MiniCloudNote.Infrastructure.Services
{
    public class MinioStorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;

        public MinioStorageService(IConfiguration configuration)
        {
            // Đọc cấu hình (Lát nữa ta sẽ thêm vào appsettings.json)
            var endpoint = configuration["Minio:Endpoint"];
            var accessKey = configuration["Minio:AccessKey"];
            var secretKey = configuration["Minio:SecretKey"];
            _bucketName = configuration["Minio:BucketName"] ?? "minicloud-uploads";
            var useSSL = false; // Môi trường dev thường không dùng SSL

            // Khởi tạo Minio Client (Native)
            _minioClient = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(useSSL)
                .Build();
        }

        // Implement đúng theo thứ tự tham số trong Interface cũ của bạn
        public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType)
        {
            // 1. Kiểm tra và tạo Bucket nếu chưa có
            var beArgs = new BucketExistsArgs().WithBucket(_bucketName);
            bool found = await _minioClient.BucketExistsAsync(beArgs);
            if (!found)
            {
                var mbArgs = new MakeBucketArgs().WithBucket(_bucketName);
                await _minioClient.MakeBucketAsync(mbArgs);
            }

            // 2. Tạo tên file duy nhất (Guid + Tên gốc)
            // Ví dụ: avatar.jpg -> 1234-5678-avatar.jpg
            var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";

            // 3. Upload
            // Reset stream về đầu để đảm bảo đọc đủ dữ liệu
            if (fileStream.CanSeek) fileStream.Position = 0;

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(uniqueFileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs);

            // 4. Trả về tên file đã lưu
            return uniqueFileName;
        }

        public async Task<string> GetFileUrlAsync(string fileName)
        {
            // Kiểm tra tên file có hợp lệ không
            if (string.IsNullOrEmpty(fileName)) return string.Empty;

            // Khi Frontend test UI bằng Emulator: 10.0.2.2 là "cánh cửa thần kỳ" để máy ảo Android nhìn thấy Windows
            // Khi Frontend test UI bằng điện thoại thật: 192.168.x.x (IP của máy tính trong mạng LAN)
            // Khi test bằng điện thoại thật qua cáp USB
            string minioBaseUrl = "http://127.0.0.1:9000";

            // Ráp lại thành link xịn (Dùng luôn biến _bucketName đã có sẵn trong class của bạn)
            string publicUrl = $"{minioBaseUrl}/{_bucketName}/{fileName}";

            // Trả về trực tiếp luôn
            return await Task.FromResult(publicUrl);
        }

        public Task DeleteFileAsync(string fileName)
        {
             // Tạm thời chưa cần implement
             return Task.CompletedTask;
        }
    }
}