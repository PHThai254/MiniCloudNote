namespace MiniCloudNote.Core.DTOs
{
    // Class Generic <T> để dùng chung cho Note, User, hay bất cứ cái gì sau này
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; } // Tổng số bản ghi tìm thất
        public int PageIndex { get; set; } // Trang hiện tại
        public int PageSize { get; set; } // Kich thước trang
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize); //Tính toán tổng số trang
    }
}