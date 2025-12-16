using System.Reflection;
using Xunit;

namespace MiniCloudNote.Tests;

public class UnitTest1
{
    [Fact]
    public void Test_1_Cong_1_Bang_2()
    {
        // 1. Arrange (Chuẩn bị)
        int a = 1;
        int b = 1;

        // 2. Act (Hành động)
        int result = a + b;

        // 3. Assert (Khẳng định kết quả)
        // Nếu result không phải là 2 -> Test Failed -> Jenkins Báo lỗi
        Assert.Equal(3, result);
    }
}