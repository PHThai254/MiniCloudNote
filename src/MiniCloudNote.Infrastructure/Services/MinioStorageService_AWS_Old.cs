// using Amazon.S3;
// using Amazon.S3.Model;
// using Amazon.S3.Transfer;
// using Microsoft.Extensions.Configuration;
// using MiniCloudNote.Core.Interfaces;
// using System;
// using System.IO;
// using System.Threading.Tasks;

// namespace MiniCloudNote.Infrastructure.Services
// {
//     public class MinioStorageService : IStorageService
//     {
//         private readonly IAmazonS3 _s3Client;
//         private readonly string _bucketName;

//         public MinioStorageService(IAmazonS3 s3Client, IConfiguration configuration)
//         {
//             _s3Client = s3Client;
//             // Lấy tên Bucket từ cấu hình (File .env hoặc appsettinhs)
//             _bucketName = configuration["Minio:BucketName"] ?? "minicloud-uploads";
//         }

//         public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType)
//         {
//             // 1. Tạo tên file duy nhất (để tránh trùng lặp)
//             // Ví dụ: avatar.jpg -> guid-avatar.jpg
//             var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";

//             // 2. Tạo yêu cầu upload
//             var putRequest = new PutObjectRequest
//             {
//                 BucketName = _bucketName,
//                 Key = uniqueFileName, // Tên file trên MinIO
//                 InputStream = fileStream,  // Dữ liệu file
//                 ContentType = contentType, // Loại file (image/jpeg, pdf...)
//                 AutoCloseStream = false // Không đóng stream vội
//             };

//             // 3. Gửi lệnh upload
//             await _s3Client.PutObjectAsync(putRequest);

//             // 4. Tạo đường dẫn trả về(Giả định MinIO chạy ở localhost:9000)
//             // Cấu trúc: http://localhost:9000/bucket-name/file-name
//             // Lưu ý: Sau này ra Prod sẽ cấu hình domain khác
//             // return $"/minio/{_bucketName}/{uniqueFileName}";

//             // Thay vì trả về URL fake "/minio/...", ta trả về Tên File (Object Key)
//             // Vì Client cần Tên File này để xin Presigned URL sau này.
//             return uniqueFileName;
//         }
//         // === 1. CHỨC NĂNG LẤY LINK (PRESIGNED URL) ===
//         public Task<string> GetFileUrlAsync(string fileName)
//         {
//             // Tạo yêu cầu xin vé
//             var request = new GetPreSignedUrlRequest
//             {
//                 BucketName = _bucketName,
//                 Key = fileName,
//                 Expires = DateTime.UtcNow.AddMinutes(60) // Vé hết hạn sau 60 phút
//             };

//             // Nhờ AWS SDK ký tên và trả về đường dẫn full
//             string url = _s3Client.GetPreSignedURL(request);
//             // === THÊM DÒNG NÀY: Hack fix cho môi trường Dev ===
//             // Nếu SDK lỡ tạo https thì mình sửa lại thành http
//             url = url.Replace("https://localhost:9000", "http://localhost:9000");   
//             return Task.FromResult(url);
//         }
//         // === 2. CHỨC NĂNG XOÁ FILE ===
//         public async Task DeleteFileAsync(string fileName)
//         {
//             var deleteRequest = new DeleteObjectRequest
//             {
//                 BucketName = _bucketName,
//                 Key = fileName
//             };

//             await _s3Client.DeleteObjectAsync(deleteRequest);
//         }
//     }
// }