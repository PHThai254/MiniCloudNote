using System.IO;
using System.Threading.Tasks;

namespace MiniCloudNote.Core.Interfaces
{
    public interface IStorageService
    {
       // Nhận vào: Tên file, Dòng dữ liệu (Stream), Loại file (ContentType)
       // Trả về: Đường dẫn (URL) của file sau khi lưu
       Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType);
        
    }
}