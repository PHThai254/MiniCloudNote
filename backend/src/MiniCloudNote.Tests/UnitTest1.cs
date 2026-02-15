using System.Reflection;
using Xunit;
using MiniCloudNote.Core;

namespace MiniCloudNote.Tests;

public class UnitTest1
{
    [Fact]
    public void Tinh_Thue_Luong_Cao_Phai_Dung()
    {
        // Arrange
        var calc = new TaxCalculator();
        decimal luong = 20_000_000; // 20 triệu

        // Act (Hành động gọi sang Core)
        decimal thue = calc.CalculateTax(luong);

        // Assert (20 triệu * 10% = 2 triệu)
        Assert.Equal(2_000_000, thue);
    }

    [Fact]
    public void Tinh_Thue_Luong_Thap_Duoc_Mien_Thue()
    {
        // Arrange
        var calc = new TaxCalculator();
        decimal luong = 5_000_000; // 5 triệu (Nhỏ hơn 10tr -> Sẽ đi vào nhánh False)

        // Act 
        decimal thue = calc.CalculateTax(luong);

        // Assert
        Assert.Equal(0, thue); // Kỳ vọng dòng 'return 0' sẽ chạy
    }
}