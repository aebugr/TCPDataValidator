using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TCPDataValidator
{
    public static class PacketProcessor
    {
        public static (string Data1, string Data2) ProcessPacket(string packet) // Метод для обработки строки данных
        {
            var match = Regex.Match(packet, @"#90#010102#27(\d{6}|\d{8});(\d{6}|\d{8})#91");
            if (match.Success)
            {
                return (match.Groups[1].Value, match.Groups[2].Value);
            }
            throw new FormatException("Неверный формат пакета");
        }
    }
}
