using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Application.Services.Helpers
{
    public static class ShortCodeGenerator
    {

        private const string Base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz";
        private const int Base = 36;


        private const long XorSalt = 9796209103;

        public static string Encode(long id)
        {

            long salted = id ^ XorSalt;

            var sb = new StringBuilder();

            if (salted == 0)
            {
                sb.Append(Base36Chars[0]);
            }
            else
            {
                while (salted > 0)
                {
                    int remainder = (int)(salted % Base);
                    sb.Append(Base36Chars[remainder]);
                    salted /= Base;
                }
            }

            // Reverse to get correct order
            var charArray = sb.ToString().ToCharArray();
            Array.Reverse(charArray);
            string base36 = new string(charArray);

            // Ensure minimum length of 5 characters 
            return base36.PadLeft(5, '0');
        }
    }
}
