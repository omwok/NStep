using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SGU.NStep.Core
{
    public static class Utilities
    {
        public static DateTime GetGMT()
        {
            const string ntpServer = "id.pool.ntp.org";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)
            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            SocketError errorCode = SocketError.Success;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);
                socket.Send(ntpData);
                socket.Receive(ntpData, 0, ntpData.Length, SocketFlags.None, out errorCode);
                socket.Close();
                if (errorCode != SocketError.Success)
                    throw new SocketException();
            }
            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
            return networkDateTime;
        }

        public static DateTime GetNetworkTime()
        {
            return GetGMT().ToLocalTime();
        }

        private static readonly char[] Alphabet =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', '2', '3', '4', '5', '6', '7'
        };

        public static string Base32Encode(this byte[] input)
        {
            if (input.Length == 0) return string.Empty;
            var bits = input.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')).Aggregate((x, y) => x + y);
            if (bits.Length % 5 != 0) bits += string.Concat(Enumerable.Repeat('0', 5 - bits.Length % 5));
            var output = new StringBuilder(bits.Length / 5);
            for (int i = 0, j = bits.Length / 5; i < j; i++) output.Append(Alphabet[Convert.ToInt32(bits.Substring(5 * i, 5), 2)]);
            while (output.Length % 8 != 0) output.Append('=');
            return output.ToString();
        }

        public static byte[] Base32Decode(this string input)
        {
            if (string.IsNullOrEmpty(input)) return new byte[0];
            input = input.TrimEnd('=').Replace(" ", string.Empty).ToUpper();
            var bits = input.Select(x => Convert.ToString(Array.IndexOf(Alphabet, x), 2).PadLeft(5, '0')).Aggregate((x, y) => x + y);
            var output = new byte[bits.Length / 8];
            for (int i = 0, j = bits.Length / 8; i < j; i++) output[i] = Convert.ToByte(bits.Substring(8 * i, 8), 2);
            return output;
        }
    }
}
