using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Shared.DTOs
{
    public class Dto_CountryAnalytics
    {
        public string Country { get; set; } = string.Empty;
        public long Clicks { get; set; }
    }
}
