using System.Reflection;
using Xunit;
using MiniCloudNote.Core;

namespace MiniCloudNote.Tests;

public class UnitTest1
{
    [Fact]
    public void Tinh_Thue_Luong_Cao_Phai_Dung()
    {
        // 1. Arrange
        var calc = new TaxCalculator();
        decimal luong = 20_000_000; // 20 triệu

        // 2. Act (Hành động gọi sang Core)
        decimal thue = calc.CalculateTax(luong);

        // 3. Assert (20 triệu * 10% = 2 triệu)
        Assert.Equal(2_000_000, thue);
    }
}