using Xunit;
using System.Collections.Generic;
using TCPDataValidator;

public class TestProject1
{
    [Fact]
    public void TestPacketProcessor_ProcessPacket()
    {
        // Arrange
        string validPacket = "#90#010102#27123456;789012#91";
        string invalidPacket = "#90#010102#27InvalidData#91";

        // Act & Assert
        var result = PacketProcessor.ProcessPacket(validPacket);
        Assert.Equal("123456", result.Data1);
        Assert.Equal("789012", result.Data2);

        // Проверка на неверный формат пакета
        var exception = Assert.Throws<FormatException>(() => PacketProcessor.ProcessPacket(invalidPacket));
        Assert.Equal("Неверный формат пакета", exception.Message);
    }

    [Fact]
    public void TestDataComparer_CompareData()
    {
        // Arrange
        var dataList = new List<(string Data1, string Data2)>
        {
            ("123456", "789012"),
            ("123456", "789012"),
            ("123456", "789012")
        };

        var mixedDataList = new List<(string Data1, string Data2)>
        {
            ("123456", "789012"),
            ("654321", "789012"),
            ("123456", "210987")
        };

        // Act & Assert
        var result = DataComparer.CompareData(dataList);
        Assert.Equal("123456", result.Data1);
        Assert.Equal("789012", result.Data2);

        var mixedResult = DataComparer.CompareData(mixedDataList);
        Assert.Equal("NoRead", mixedResult.Data1);
        Assert.Equal("NoRead", mixedResult.Data2);
    }
}