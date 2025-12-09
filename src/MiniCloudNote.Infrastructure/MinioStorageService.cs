using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using MiniCloudNote.Core.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MiniCloudNote.Infrastructure
{
    public class MinioStorageService : IStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public MinioStorageService(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            // Lấy tên Bucket từ cấu hình (File .env hoặc appsettinhs)
            _bucketName = configuration["Minio:BucketName"] ?? "minicloud-uploads";
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType)
        {
            // 1. Tạo tên file duy nhất (để tránh trùng lặp)
            // Ví dụ: avatar.jpg -> guid-avatar.jpg
            var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";

            // 2. Tạo yêu cầu upload
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = uniqueFileName, // Tên file trên MinIO
                InputStream = fileStream,  // Dữ liệu file
                ContentType = contentType, // Loại file (image/jpeg, pdf...)
                AutoCloseStream = false // Không đóng stream vội
            };

            // 3. Gửi lệnh upload
            await _s3Client.PutObjectAsync(putRequest);

            // 4. Tạo đường dẫn trả về(Giả định MinIO chạy ở localhost:9000)
            // Cấu trúc: http://localhost:9000/bucket-name/file-name
            // Lưu ý: Sau này ra Prod sẽ cấu hình domain khác
            return $"/minio/{_bucketName}/{uniqueFileName}";
        }
    }
}