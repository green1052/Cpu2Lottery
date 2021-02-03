using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

public class Client
{
    private static readonly Encoding DefaultEncoding = Encoding.GetEncoding(51949);
    private readonly CookieContainer _cookieContainer = new CookieContainer();

    public Client(string url, string id, string pw, Encoding encoding = null)
    {
        if (encoding == null) Encoding = DefaultEncoding;

        HttpWebRequest webRequest = GetWebRequest(url);
        QueryStringBuilder qsb = new QueryStringBuilder();

        qsb.Add("mb_id", id);
        qsb.Add("mb_password", pw);

        Stream stream = webRequest.GetRequestStream();
        byte[] bytes = qsb.GetBytes();
        stream.Write(bytes, 0, bytes.Length);

        using HttpWebResponse webResponse = (HttpWebResponse) webRequest.GetResponse();
        using StreamReader sr = new StreamReader(webResponse.GetResponseStream(), Encoding);
        Match match = Regex.Match(sr.ReadToEnd(), @"alert\('(?<msg>.*)'\);");
        if (match.Success) ErrorMessage = match.Groups["msg"].Value.Replace("\\n", "\n");
    }

    private static Encoding Encoding { get; set; }
    public string ErrorMessage { get; }

    public HtmlDocument GetHtml(string url, Dictionary<string, string> query = null)
    {
        HttpWebRequest webRequest = GetWebRequest(url);
        QueryStringBuilder qsb = new QueryStringBuilder();

        if (query != null)
        {
            foreach (KeyValuePair<string, string> keyValuePair in query)
                qsb.Add(keyValuePair.Key, keyValuePair.Value);
        }

        Stream stream = webRequest.GetRequestStream();
        byte[] bytes = qsb.GetBytes();
        stream.Write(bytes, 0, bytes.Length);

        using HttpWebResponse webResponse = (HttpWebResponse) webRequest.GetResponse();
        using StreamReader sr = new StreamReader(webResponse.GetResponseStream()!, Encoding);

        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(sr.ReadToEnd());

        return htmlDocument;
    }

    private HttpWebRequest GetWebRequest(string url)
    {
        HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(url);

        webRequest.Method = "POST";
        webRequest.ContentType = "application/x-www-form-urlencoded";
        webRequest.KeepAlive = true;
        webRequest.CookieContainer = _cookieContainer;

        return webRequest;
    }

    private class QueryStringBuilder
    {
        private readonly StringBuilder _queryString;

        public QueryStringBuilder()
        {
            _queryString = new StringBuilder();
        }

        public void Add(string name, string value)
        {
            if (_queryString.Length > 0) _queryString.Append("&");

            _queryString.Append(HttpUtility.UrlEncode(name));
            _queryString.Append("=");
            _queryString.Append(HttpUtility.UrlEncode(value));
        }

        public byte[] GetBytes()
        {
            return Encoding.GetBytes(_queryString.ToString());
        }
    }
}