using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Shared.DTOs
{
    public class Dto_DeviceTypeStats
    {
        public string DeviceType { get; set; } = string.Empty;
        public long ClickCount { get; set; }
    }
}
