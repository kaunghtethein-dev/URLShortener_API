using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Shared.DTOs
{
    public class Dto_ClickSummary
    {
        public long TotalShortUrls { get; set; }
        public long TotalClicks { get; set; }
        public double AverageClicks { get; set; }
        public long TotalActive { get; set; }
        public long TotalExpired { get; set; }
    }
}
