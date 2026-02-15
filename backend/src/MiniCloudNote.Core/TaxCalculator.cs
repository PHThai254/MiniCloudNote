namespace MiniCloudNote.Core;

public class TaxCalculator
{
    // Hàm tính thuế: Nếu lương > 10 triệu thì đóng 10%, ngược lại miễn thuế
    public decimal CalculateTax(decimal income)
    {
        if (income > 10_000_000)
        {
            return income * 0.1m;
        }
        
        return 0;
    }
}