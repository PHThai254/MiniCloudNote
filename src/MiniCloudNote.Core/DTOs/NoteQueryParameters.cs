namespace MiniCloudNote.Core.DTOs
{
    public class NoteQueryParameters
    {
        // Mặc định trang 1, lấy 10 cái
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Từ khóa tìm kiếm (Optional)
    public string? SearchTerm { get; set; }
    
    // Sắp xếp: "creat_desc" (mới nhất), "title_asc" (A-Z)...
    public string? SortBy { get; set; } = "created_desc";
    }
}