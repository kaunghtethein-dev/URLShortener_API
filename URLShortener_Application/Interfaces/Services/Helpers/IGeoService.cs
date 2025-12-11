using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Application.Interfaces.Services.Helpers
{
    public interface IGeoService
    {
        Task<string?> ResolveCountryAsync(string? ip);
    }
}
