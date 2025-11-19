using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Shared.DTOs
{
    public class Dto_RefreshRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
