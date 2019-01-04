﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class MusicCloud : MusicBase {
    //歌曲下载地址 : http://music.163.com/song/media/outer/url?id=ID数字.mp3
    protected override async Task ParseInfo(string id, string path) {
        var detailResult = await HttpUtil.Get($"https://api.imjad.cn/cloudmusic/?type=detail&id={id}");
        var detailData = JObject.Parse(detailResult);
        var songResult = await HttpUtil.Get($"https://api.imjad.cn/cloudmusic/?type=song&id={id}");
        var songData = JObject.Parse(songResult);
        var songRoot = songData["data"][0];
        var detailRoot = detailData["songs"][0];
        Mp3Url = songRoot.Value<string>("url");
        Name = detailRoot.Value<string>("name");
        foreach (var token in detailRoot["ar"]) {
            Singer.Add(token.Value<string>("name"));
        }
        Album = detailRoot["al"].Value<string>("name");
        CoverUrl = detailRoot["al"].Value<string>("picUrl");
        if (string.IsNullOrEmpty(Mp3Url)) {
            throw new Exception($"歌曲 {Name} 暂时不支持下载,也不支持试听");
        }
    }
}
