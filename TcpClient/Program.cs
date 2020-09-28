using SuperSocket;
using SuperSocket.Client;
using SuperSocket.ProtoBase;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpIpClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Encoding Utf8Encoding = new UTF8Encoding();

            var pipelineFilter = new CommandLinePipelineFilter
            {
                Decoder = new DefaultStringPackageDecoder()
            };
            IEasyClient<StringPackageInfo> client = new EasyClient<StringPackageInfo>(pipelineFilter);

            StringPackageInfo package = null;

            client.PackageHandler += async (s, p) =>
            {
                package = p;
                if (string.IsNullOrWhiteSpace(p.Key))
                {
                    Console.WriteLine("unexpected result ---------");
                }
                else
                {
                    Console.WriteLine(p.Key);
                }
                await Task.CompletedTask;
            };

            var connected = await client.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 8899));

            Console.WriteLine(connected);

            client.StartReceive();

            byte[] bytes1 = Utf8Encoding.GetBytes(new char[] { (char)0x00, (char)0x00, (char)0x01, (char)0x61 });
            byte[] bytes2 = Utf8Encoding.GetBytes(new char[] { (char)0x00, (char)0x00, (char)0x01, (char)0x61, (char)0x62 });
            byte[] bytes3 = Utf8Encoding.GetBytes(new char[] { (char)0x00, (char)0x00, (char)0x01, (char)0x61 });
            byte[] bytes4 = Utf8Encoding.GetBytes(new char[] { (char)0x00, (char)0x00, (char)0x01, (char)0x61 });

            await client.SendAsync(bytes1); //expected result: a, true result: a
            await Task.Delay(1000);
            await client.SendAsync(bytes2); //expected result: a, true result: a
            await Task.Delay(1000);
            await client.SendAsync(bytes3); //expected result: a, true result: null
            await Task.Delay(1000);
            await client.SendAsync(bytes4); //expected result: a, tree result: null
            await Task.Delay(1000);

            await client.CloseAsync();
        }
    }
}
