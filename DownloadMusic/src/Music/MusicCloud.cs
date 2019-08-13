using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class MusicCloud : MusicBase {
    //歌曲下载地址 : http://music.163.com/song/media/outer/url?id=ID数字.mp3
    protected override async Task ParseInfo(string id) {
        var detailResult = await HttpUtil.Get($"https://api.imjad.cn/cloudmusic/?type=detail&id={id}");
        var detailData = JObject.Parse(detailResult);
        var songResult = await HttpUtil.Get($"https://api.imjad.cn/cloudmusic/?type=song&id={id}");
        var songData = JObject.Parse(songResult);
        var songRoot = songData["data"][0];
        var detailRoot = detailData["songs"][0];
        Name = detailRoot.Value<string>("name");
        Album = detailRoot["al"].Value<string>("name");
        CoverUrl = detailRoot["al"].Value<string>("picUrl");
        foreach (var token in detailRoot["ar"]) {
            Singer.Add(token.Value<string>("name"));
        }
        Mp3Urls.Add(songRoot.Value<string>("url"));
        Mp3Urls.RemoveAll((_) => string.IsNullOrEmpty(_));
        if (Mp3Urls.Count == 0) {
            throw new Exception($"歌曲 {Name} 暂时不支持下载,也不支持试听");
        }
    }
}
