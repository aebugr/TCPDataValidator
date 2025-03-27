using Xunit;
using System.Collections.Generic;
using TCPDataValidator;

public class TestProject1
{
    public class DataComparerTests
    {
        [Fact]
        public void CompareData_AllValuesEqual_ReturnsCorrectData()
        {
            // Arrange
            var dataList = new List<(string, string)>
            {
                ("123456", "654321"),
                ("123456", "654321")
            };

            // Act
            var result = DataComparer.CompareData(dataList);

            // Assert
            Assert.Equal("123456", result.Data1);
            Assert.Equal("654321", result.Data2);
        }

        [Fact]
        public void CompareData_ValuesNotEqual_ReturnsNoRead()
        {
            // Arrange
            var dataList = new List<(string, string)>
            {
                ("123456", "654321"),
                ("654321", "123456")
            };

            // Act
            var result = DataComparer.CompareData(dataList);

            // Assert
            Assert.Equal("NoRead", result.Data1);
            Assert.Equal("NoRead", result.Data2);
        }
    }

    public class PacketProcessorTests
    {
        [Fact]
        public void ProcessPacket_ValidPacket_ReturnsCorrectData()
        {
            // Arrange
            string validPacket = "#90#010102#27123456;654321#91";

            // Act
            var result = PacketProcessor.ProcessPacket(validPacket);

            // Assert
            Assert.Equal("123456", result.Data1);
            Assert.Equal("654321", result.Data2);
        }

        [Fact]
        public void ProcessPacket_InvalidPacket_ThrowsFormatException()
        {
            // Arrange
            string invalidPacket = "invalid_packet_format";

            // Act & Assert
            Assert.Throws<FormatException>(() => PacketProcessor.ProcessPacket(invalidPacket));
        }
    }
}