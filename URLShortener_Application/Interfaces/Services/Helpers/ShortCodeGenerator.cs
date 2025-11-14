using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Application.Interfaces.Services.Helpers
{
    public static  class ShortCodeGenerator
    {
        private const string Base62Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const long XorSalt = 979620910123; 

        public static string Encode(long id)
        {
            long salted = id ^ XorSalt;

            var sb = new StringBuilder();
            if (salted == 0)
            {
                sb.Append(Base62Chars[0]);
            }
            else
            {
                while (salted > 0)
                {
                    int remainder = (int)(salted % 62);
                    sb.Append(Base62Chars[remainder]);
                    salted /= 62;
                }
            }

            var charArray = sb.ToString().ToCharArray();
            Array.Reverse(charArray);
            string base62 = new string(charArray);

            // Ensure minimum length of 6 characters
            return base62.PadLeft(6, '0');
        }
    }
}
