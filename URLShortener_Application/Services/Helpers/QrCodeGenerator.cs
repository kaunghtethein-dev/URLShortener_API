using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;
using URLShortener_Application.Interfaces.Services.Helpers;


namespace URLShortener_Application.Services.Helpers
{
    public class QrCodeGenerator : IQrCodeGenerator
    {
        public byte[] GenerateQrCode(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var pngQrCode = new PngByteQRCode(qrData);
            return pngQrCode.GetGraphic(20);
        }
    }
}
