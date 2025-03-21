using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPDataValidator
{
    public static class DataComparer 
    {
        // Сравнивает данные из списка. Если все значения Data1 или Data2 одинаковы, возвращает их, иначе "NoRead".
        public static (string Data1, string Data2) CompareData(List<(string Data1, string Data2)> dataList) 
        {
            string data1 = dataList.Select(d => d.Data1).Distinct().Count() == 1 ? 
                dataList[0].Data1 : "NoRead"; 

            string data2 = dataList.Select(d => d.Data2).Distinct().Count() == 1 ? dataList[0].Data2 : "NoRead"; 
            return (data1, data2); 
        }
    }
}
