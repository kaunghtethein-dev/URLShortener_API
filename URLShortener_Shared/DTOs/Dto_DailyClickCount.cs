using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Shared.DTOs
{
    public class Dto_DailyClickCount
    {
        public DateTime Date { get; set; }
        public long ClickCount { get; set; }
    }
}
