using System.Threading.Tasks;
using System.Xml;
public class MusicKuwo : MusicBase {
    protected override async Task ParseInfo(string id) {
        var result = await HttpUtil.Get("http://player.kuwo.cn/webmusic/st/getNewMuiseByRid?rid=MUSIC_" + id);
        var doc = new XmlDocument();
        doc.LoadXml(result.XMLEncode());
        var root = doc.DocumentElement;
        Name = root.GetValue("name");
        Singer.Add(root.GetValue("singer"));
        Album = root.GetValue("special");
        CoverUrl = root.GetValue("artist_pic");
        var mp3dl = root.GetValue("mp3dl").XMLDecode();
        var mp3path = root.GetValue("mp3path").XMLDecode();
        Mp3Urls.Add($"http://{mp3dl}{mp3path}");
        //var mp3dl = root.GetValue("mp3dl");
        //var mp3path = root.GetValue("mp3path");
        //Mp3Url = $"http://ra01.sycdn.kuwo.cn/resource/{mp3path}";
        //Mp3Url = $"http://{mp3dl}{mp3path}";
        // http://antiserver.kuwo.cn/anti.s?useless=/resource/{mp3path}
    }
}
