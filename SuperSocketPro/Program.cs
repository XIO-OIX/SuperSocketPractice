using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using SuperSocket;
using SuperSocket.ProtoBase;
using SuperSocketIOTJiankuang.PipelineFilters;
using SuperSocketPro;
using System;
using System.Net.Sockets;
using System.Text;

namespace SuperSocketIOTJiankuang
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Start Run");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls("http://localhost:51222");

                })
                
                .ConfigureLogging(logging =>
                {
                    //logging.ClearProviders(); //移除已经注册的其他日志处理程序
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace); //设置最小的日志级别
                })
                .UseNLog() // 使用NLog;
                .AsSuperSocketHostBuilder<StringPackageInfo, MyPipelineFilter>()
                .ConfigureSocketOptions(socket =>
                {
                    socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 10);
                    socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 5);
                    socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 7);
                })
                .UsePackageHandler(async (s, p) =>
                {
                    //Console.WriteLine(p);
                    var logger = s.GetDefaultLogger();
                    logger.LogInformation(p.Key.ToString());

                    // echo message back to client
                    await s.SendAsync(Encoding.UTF8.GetBytes("1" + "\r\n"));
                });
    }
}
