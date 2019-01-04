using System.Threading.Tasks;
using System.Xml;
public class MusicKuwo : MusicBase {
    protected override async Task ParseInfo(string id, string path) {
        var result = await HttpUtil.Get("http://player.kuwo.cn/webmusic/st/getNewMuiseByRid?rid=MUSIC_" + id);
        result = result.Replace("=", "等于").Replace("&", "并且");
        var doc = new XmlDocument();
        doc.LoadXml(result);
        var root = doc.DocumentElement;
        Name = root.GetValue("name");
        Singer.Add(root.GetValue("singer"));
        Album = root.GetValue("special");
        var mp3path = root.GetValue("mp3path");
        Mp3Url = $"http://ra01.sycdn.kuwo.cn/resource/{mp3path}";
        CoverUrl = root.GetValue("artist_pic");
    }
}
