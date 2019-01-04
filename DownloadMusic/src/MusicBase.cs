using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Scorpio.Commons;
using System.IO;
using System.Drawing;

public class MusicBase {
    private List<string> _Singer = new List<string>();
    /// <summary>
    /// 歌曲ID, 不同音乐平台的ID可能相同
    /// </summary>
    public string ID { get; private set; }
    /// <summary>
    /// 歌曲名字
    /// </summary>
    public string Name { get; protected set; }
    /// <summary>
    /// 专辑名字
    /// </summary>
    public string Album { get; protected set; }
    /// <summary>
    /// 演唱者
    /// </summary>
    public List<string> Singer { get { return _Singer; } }
    /// <summary>
    /// 封面图片地址
    /// </summary>
    public string CoverUrl { get; protected set; }
    /// <summary>
    /// mp3下载地址
    /// </summary>
    public string Mp3Url { get; protected set; }

    private string SavePath = "";
    protected virtual async Task ParseInfo(string id, string path) {
        await Task.Yield();
    }
    public async Task Download(string id, string path) {
        ID = id;
        SavePath = path;
        await ParseInfo(id, path);
        await DownloadFile();
    }
    async Task DownloadFile() {
        FileUtil.CreateDirectory(SavePath);
        Logger.info("解析完成,开始下载 id:{0} 名字:{1}  歌手:{2}  专辑:{3}", ID, Name, Singer.GetValue(), Album);
        var fileName = $"{Singer.GetValue()}-{Name}.mp3";
        var filePath = Path.Combine(SavePath, fileName);
        await HttpUtil.Download(Mp3Url, filePath);
        Logger.info("下载音频文件完成,文件大小:{1}", fileName, Util.GetMemory(new FileInfo(filePath).Length));
        var file = TagLib.File.Create(filePath);
        file.Tag.Title = Name;
        file.Tag.Performers = Singer.ToArray();
        file.Tag.Album = Album;
        if (!string.IsNullOrEmpty(CoverUrl)) {
            var imageName = $"{Singer.GetValue()}-{Name}.jpg";
            var imagePath = Path.Combine(SavePath, imageName);
            Logger.info("正在下载封面图片...");
            await HttpUtil.Download(CoverUrl, imagePath);
            ResizeImage(imagePath, 512);
            file.Tag.Pictures = new TagLib.IPicture[] { new TagLib.Picture(imagePath) };
            FileUtil.DeleteFile(imagePath);
        }
        file.Save();
        Logger.info("写入封面完成 文件名:{0}  文件大小:{1}", fileName, Util.GetMemory(new FileInfo(filePath).Length));
    }
    private void ResizeImage(string filePath, int size) {
        using (var bitmap = new Bitmap(size, size)) {
            using (var image = Image.FromFile(filePath)) {
                int sourceWidth = image.Width;
                int sourceHeight = image.Height;
                using (var graphics = Graphics.FromImage(bitmap)) {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(image, 0, 0, size, size);
                }
            }
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
