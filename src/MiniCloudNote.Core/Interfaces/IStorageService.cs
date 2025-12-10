using System.IO;
using System.Threading.Tasks;

namespace MiniCloudNote.Core.Interfaces
{
    public interface IStorageService
    {
       // Upload: Nhận vào tên file, Dòng dữ liệu (Stream), Loại file (ContentType)
       // Trả về: Đường dẫn (URL) của file sau khi lưu
       Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType);

       // 1. Lấy đường dẫn xem file (Có hạn sử dụng)
       Task<string> GetFileUrlAsync(string fileName);

       // 2. Xoá file
       Task DeleteFileAsync(string fileName);
        
    }
}