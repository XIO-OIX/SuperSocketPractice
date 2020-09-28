using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SuperSocket;
using SuperSocket.Command;
using SuperSocket.ProtoBase;
using SuperSocketPro.PackageDeoders;
using SuperSocketPro.PipelineFilters;
using SuperSocketPro.SuperSocketCommands;
using System.Text;

namespace SuperSocketPro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .AsSuperSocketHostBuilder<MyPackage, MyPipelineFilter>()
                .UsePackageDecoder<MyPackageDecoder>()
                .UsePackageHandler(async (s, p) =>
                {
                    // echo message back to client
                    await s.SendAsync(Encoding.UTF8.GetBytes(p.Body + "\r\n"));
                })
                ;
    }
}
