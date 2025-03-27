using System;
using System.Collections.Generic;
using System.Linq;

namespace TCPDataValidator
{
    /// <summary>
    /// Сравнение данных от разных серверов
    /// </summary>
    public static class DataComparer
    {
        /// <summary>
        /// Сравнение списка данных
        /// </summary>
        public static (string Data1, string Data2) CompareData(IReadOnlyCollection<(string Data1, string Data2)> dataList)
        {
            if (dataList == null || dataList.Count == 0)
                return ("NoRead", "NoRead");

            string data1 = AllEqual(dataList.Select(x => x.Data1)) ? dataList.First().Data1 : "NoRead";
            string data2 = AllEqual(dataList.Select(x => x.Data2)) ? dataList.First().Data2 : "NoRead";

            return (data1, data2);
        }

        private static bool AllEqual(IEnumerable<string> values)
        {
            var first = values.FirstOrDefault();
            return !string.IsNullOrEmpty(first) && values.All(v => v == first);
        }
    }
}