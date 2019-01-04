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
                var type = command.GetValue("-type");
                if (string.IsNullOrWhiteSpace(ids)) {
                    throw new Exception("请添加id");
                }
                path = System.IO.Path.Combine(Environment.CurrentDirectory, path ?? "");
                var t = Task.Run(async () => {
                    var strs = ids.Split(",");
                    foreach (var id in strs) {
                        if (type == "kuwo") {
                            await new MusicKuwo().Download(id, path);
                        } else {
                            await new MusicCloud().Download(id, path);
                        }
                    }
                });
                t.Wait();
            } catch (Exception e) {
                Logger.error("Error : " + e.ToString());
            }
        }
    }
}
