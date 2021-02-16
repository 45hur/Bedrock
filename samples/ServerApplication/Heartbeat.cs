using Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    internal static class Heartbeat
    {
        
        private static readonly Message request;
        static Heartbeat()
        {
            string hexString = "68 20 00 10 04 00 05 71 00 01 12 34 56 78 02 01 64 64 32 30 31 38 30 35 30 39 32 30 33 30 35 35 61 61 30 30 30 30 31 31 31 31 32 32 32 32 00 00 60 16";
            HexConverter converter = new HexConverter();
            byte[] payload = null;
            char[] separator = new char[] { ' ' };

            converter.TryConvertToByteArray(hexString, false, out payload, separator);
            Heartbeat.request = new Message(payload);
        }

        public static Message Request => request;
    }
}
