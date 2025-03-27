using System;
using System.Text.RegularExpressions;

namespace TCPDataValidator
{
    /// <summary>
    /// Обработчик входящих пакетов данных
    /// </summary>
    public static class PacketProcessor
    {
        private static readonly Regex PacketRegex = new Regex(
            @"#90#010102#27(?<Data1>\d{6}|\d{8});(?<Data2>\d{6}|\d{8})#91",
            RegexOptions.Compiled);

        /// <summary>
        /// Разбор пакета данных
        /// </summary>
        public static (string Data1, string Data2) ProcessPacket(string packet)
        {
            if (string.IsNullOrWhiteSpace(packet))
                throw new ArgumentException("Пустой пакет данных");

            var match = PacketRegex.Match(packet);
            if (!match.Success)
                throw new FormatException($"Неверный формат пакета: {packet}");

            return (match.Groups["Data1"].Value, match.Groups["Data2"].Value);
        }
    }
}