using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using Scorpio.Commons;
using System.IO;

public class HttpUtil {
    public static async Task<string> Get(string url) {
        var request = HttpWebRequest.Create(url);
        request.Method = "GET";
        var response = await request.GetResponseAsync();
        using (var responseStream = response.GetResponseStream()) {
            using (var reader = new StreamReader(responseStream)) {
                return await reader.ReadToEndAsync();
            }
        }
    }
    public static async Task Download(string url, string file) {
        FileUtil.CreateDirectoryByFile(file);
        var bytes = new byte[8192];
        var request = HttpWebRequest.Create(url);
        var response = await request.GetResponseAsync();
        using (var responseStream = response.GetResponseStream()) {
            FileUtil.DeleteFile(file);
            using (var fileStream = new FileStream(file, FileMode.Create)) {
                while (true) {
                    var size = await responseStream.ReadAsync(bytes, 0, 8192);
                    if (size <= 0) { break; }
                    fileStream.Write(bytes, 0, size);
                }
            }
        }
    }
}
