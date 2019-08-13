using System.Collections.Generic;
using System.Threading.Tasks;
using Scorpio.Commons;
using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
public class MusicFactory {
    public static MusicBase Create(string type) {
        switch (type.ToLower()) {
            case "kuwo": return new MusicKuwo();
            default: return new MusicCloud();
        }
    }
}
public class MusicBase {
    /// <summary> 歌曲ID, 不同音乐平台的ID可能相同 </summary>
    public string ID { get; private set; }
    /// <summary> 歌曲名字 </summary>
    public string Name { get; protected set; }
    /// <summary> 专辑名字 </summary>
    public string Album { get; protected set; }
    /// <summary> 演唱者 </summary>
    public List<string> Singer { get; } = new List<string>();
    /// <summary> 封面图片地址 </summary>
    public string CoverUrl { get; protected set; }
    /// <summary> mp3下载地址,可能有多个地址,顺序尝试下载 </summary>
    public List<string> Mp3Urls { get; } = new List<string>();

    //下载文件
    public async Task Download(string id, string path) {
        ID = id;
        await ParseInfo(id);
        await DownloadFile(path);
    }
    //解析信息
    protected virtual async Task ParseInfo(string id) {
        await Task.Yield();
    }
    async Task DownloadFile(string savePath) {
        FileUtil.CreateDirectory(savePath);
        Logger.info("解析完成,开始下载 id:{0} 名字:{1}  歌手:{2}  专辑:{3}", ID, Name, Singer.GetValue(), Album);
        var fileName = $"{Singer.GetValue()} - {Name}.mp3";
        var filePath = Path.Combine(savePath, fileName);
        foreach (var mp3Url in Mp3Urls) {
            try {
                Logger.info($"尝试下载文件 : {mp3Url}");
                await HttpUtil.Download(mp3Url, filePath);
                break;
            } catch (Exception) { }
        }
        if (!File.Exists(filePath)) { throw new Exception("音频文件下载失败"); }
        Logger.info("下载音频文件完成,文件大小:{1}", fileName, Util.GetMemory(new FileInfo(filePath).Length));
        var file = TagLib.File.Create(filePath);
        file.Tag.Title = Name;
        file.Tag.Performers = Singer.ToArray();
        file.Tag.Album = Album;
        if (!string.IsNullOrEmpty(CoverUrl)) {
            var imageName = $"{Singer.GetValue()} - {Name}.jpg";
            var imagePath = Path.Combine(savePath, imageName);
            Logger.info("正在下载封面图片...");
            await HttpUtil.Download(CoverUrl, imagePath);
            ResizeImage(imagePath, 512);
            file.Tag.Pictures = new TagLib.IPicture[] { new TagLib.Picture(imagePath) };
            FileUtil.DeleteFile(imagePath);
        }
        file.Save();
        Logger.info("写入封面完成 文件名:{0}  文件大小:{1}", fileName, Util.GetMemory(new FileInfo(filePath).Length));
    }
    //重置封面大小
    void ResizeImage(string filePath, int size) {
        using (Image<Rgba32> image = Image.Load(filePath)) {
            image.Mutate(x => x
                .Resize(size, size)
                .Grayscale());
            image.Save(filePath); // Automatic encoder selected based on extension.
        }
        // using (var bitmap = new Bitmap(size, size)) {
        //     using (var image = Image.FromFile(filePath)) {
        //         int sourceWidth = image.Width;
        //         int sourceHeight = image.Height;
        //         using (var graphics = Graphics.FromImage(bitmap)) {
        //             graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //             graphics.DrawImage(image, 0, 0, size, size);
        //         }
        //     }
        //     bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        // }
    }
}
