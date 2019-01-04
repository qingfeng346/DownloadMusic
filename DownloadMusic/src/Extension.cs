using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
public static class Extension {
    public static string GetValue(this XmlElement xml, string key) {
        if (xml == null) { return null; }
        var el = xml.GetElementsByTagName(key);
        if (el == null || el.Count == 0) { return null; }
        var bytes = Encoding.Default.GetBytes(el[0].InnerXml);
        var b = Encoding.Convert(Encoding.Default, Encoding.UTF8, bytes);
        return Encoding.UTF8.GetString(b);
    }
    public static string GetValue(this List<string> value) {
        return string.Join("&", value);
    }
}
