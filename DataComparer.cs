using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPDataValidator
{
    public static class DataComparer 
    {
        public static (string Data1, string Data2) CompareData(List<(string Data1, string Data2)> dataList) 
        {
            string data1 = dataList.Select(d => d.Data1).Distinct().Count() == 1 ? 
                dataList[0].Data1 : "NoRead"; // Если все значения Data1 одинаковые, взять их, иначе "NoRead"

            string data2 = dataList.Select(d => d.Data2).Distinct().Count() == 1 ? dataList[0].Data2 : "NoRead"; // Аналогично для Data2
            return (data1, data2); 
        }
    }
}
