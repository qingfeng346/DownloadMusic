using System;
using Scorpio.Commons;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;
namespace DownloadMusic {
    class LoggerHelper : ILogger {
        public void error(string value) {
            Debugger.Log(0, null, value + "\n");
            Console.WriteLine(value);
        }
        public void info(string value) {
            Debugger.Log(0, null, value + "\n");
            Console.WriteLine(value);
        }
        public void warn(string value) {
            Debugger.Log(0, null, value + "\n");
            Console.WriteLine(value);
        }
    }
    class Program {
        static void Main(string[] args) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Logger.SetLogger(new LoggerHelper());
            try {
                var command = CommandLine.Parse(args);
                var ids = command.GetValue("-id");
                var path = command.GetValue("-path");
                var type = command.GetValueDefault("-type", "");
                if (string.IsNullOrWhiteSpace(ids)) {
                    throw new Exception("请添加id");
                }
                path = System.IO.Path.Combine(Environment.CurrentDirectory, path ?? "");
                Task.Run(async () => {
                    foreach (var id in ids.Split(",")) {
                        try {
                            await MusicFactory.Create(type).Download(id, path);
                        } catch (Exception e) {
                            Logger.error($"下载 {id} 失败 : " + e.ToString());
                        }
                    }
                }).Wait();
            } catch (Exception e) {
                Logger.error("Error : " + e.ToString());
            }
        }
    }
}
