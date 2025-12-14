using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Application.Services.Helpers
{
    public class CountryClickCount
    {
        public string Country { get; set; } = string.Empty;
        public long ClickCount { get; set; }
    }
}
