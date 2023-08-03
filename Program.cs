#region Imports / usings

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

#endregion


namespace UrlParse
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TODO Regex document : https://uibakery.io/regex-library/url-regex-csharp

            string[] data = {
                "https://ja.wikipedia.org/wiki/",
                "https://disk.yandex.com",
                "C:/Users/user_name/Desktop/20190509113102186.pdf",
                "xbox.com/tr-tr/consoles/xbox-series-s?icid=mscom_marcom_H2a_XboxSeriesS",
                "google.com/search?gs_ssp=eJzj4tDP1TcwKS4zUWA0YHRg8OLMzUwuyi_OTysBAFGjBxo&q=microsoft&oq=mic&aqs=chrome.1.69i57j46i199i433i465i512j0i433i512l7j0i271.2234j0j7&sourceid=chrome&ie=UTF-8",
                "yandex.com/maps/200/los-angeles/?ll=-118.199357%2C34.052768&z=13",                
                "learn.microsoft.com/en-us/windows/win32/api/sysinfoapi/nf-sysinfoapi-getversion",
                "stackoverflow.com/questions",
                "www.mediamarkt.com.tr/tr/category/_mac-645068.html?searchParams=&sort=price&view=&page=",
                "https://www.youtube.com/results?search_query=ariana+grande",
                "https://www.google.com/search?q=facebook&ei=32rLZNXsMNqtxc8PrJybuA4&ved=0ahUKEwiV2JW8jsCAAxXaVvEDHSzOBucQ4dUDCA4&uact=5&oq=facebook&gs_lp=Egxnd3Mtd2l6LXNlcnAiCGZhY2Vib29rMg0QABiKBRixAxiDARhDMgsQABiABBixAxiDATILEAAYgAQYsQMYgwEyCxAAGIAEGLEDGIMBMggQABiABBixAzILEAAYgAQYsQMYgwEyCBAAGIAEGLEDMgsQABiABBixAxiDATILEAAYgAQYsQMYgwEyCxAAGIAEGLEDGIMBSNYQUM4HWJ0PcAF4AZABAJgBgQGgAfcGqgEDMy41uAEDyAEA-AEBwgIKEAAYRxjWBBiwA8ICCxAuGIMBGLEDGIAEwgIFEAAYgATCAhEQLhiABBixAxiDARjHARjRA8ICGhAuGIMBGLEDGIAEGJcFGNwEGN4EGOAE2AEBwgIHEAAYigUYQ8ICCxAuGIAEGMcBGK8BwgIFEC4YgATCAggQLhiABBixA-IDBBgAIEGIBgGQBgi6BgYIARABGBQ&sclient=gws-wiz-serp",
                "about:blank",
                "https://192.168.1.189:7233/swagger/index.html",
                "x8x.xx9.1xx.801:8880/login_up.php?success_redirect_url=%2Fadmin%2Fhome%2F",
                "file:///C:/Users/user_name/Desktop/example.pdf",
                "chrome://settings/",
                "search.brave.com/settings",
                "data: data:text/plain;base64,SGVsbG8gV29ybGQh",
                "ftp://ftp.exampleftp.com/files/file.txt",
                "mailto:info@exampleinfo.com",
                "tel:+1234567890",
                "ssh://username@examplessh.com",
                "git://github.com/user/repo.git",
                "sftp://sftp.examplesftp.com/files/file.txt",
                "irc://irc.exampleirc.com/channe",
                "news:alt.example.news",
                "magnet:?xt=urn:btih:67y23n4k56y78h90uj32n56",
            };
            
            List<string> urls        = ExtractUrls(data);
            List<string> domainNames = ExtractDomainNames(urls);

            Console.WriteLine($"URL address; total: {urls.Count}");
            foreach (string url in urls)
            {
                Console.WriteLine(url);
            }

            Console.WriteLine($"\nDomain Names; total: {domainNames.Count}");
            foreach (string domain in domainNames)
            {
                Console.WriteLine(domain);
            }

            Console.Read();
        }

        static List<string> ExtractUrls(string[] data)
        {
            List<string> urls = new List<string>();

            foreach (string item in data)
            {
                var uri = item.Trim();

                var stat1 = GetWithStartHttp(uri);
                if (stat1)
                {
                    urls.Add(uri);
                    continue;
                }

                var stat2 = GetNotStartWithHttp(uri);
                if (stat2)
                {
                    urls.Add($"https://{uri}");
                }
                else continue;
            }
            return urls;
        }

        static bool GetWithStartHttp(string url)
        {
            url = ChangeTurksihChar(url);

            // Validate URL
            Regex validateDateRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
            return validateDateRegex.IsMatch(url);
        }

        static bool GetNotStartWithHttp(string url)
        {
            url = ChangeTurksihChar(url);

            // Validate URL without protocol
            Regex validateDateRegex = new Regex("^[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
            return validateDateRegex.IsMatch(url);
        }

        static List<string> ExtractDomainNames(List<string> urls)
        {
            List<string> domainNames = new List<string>();
            Uri newUri = null;
            foreach (string url in urls)
            {
                Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out newUri);

                if (newUri != null)
                {
                    var domainName = newUri.Host;
                    object? port = (newUri.Port != 443) ? $":{newUri.Port}" : string.Empty;
                    
                    if (domainName.StartsWith("www."))
                    {
                        domainName = domainName.Replace("www.", "");
                    }

                    domainNames.Add($"{domainName}{port}");
                }

            }

            return domainNames;
        }


        static string ChangeTurksihChar(string uri)
        {
            char[] turkishWord     = { 'ö', 'ç', 'ş', 'ğ', 'ü', 'ı', 'Ö', 'Ç', 'Ş', 'Ğ', 'Ü', 'İ'};
            char[] changeLatinWord = { 'o', 'c', 's', 'g', 'u', 'i', 'O', 'C', 'S', 'G', 'U', 'I'};

            for (int i = 0; i < turkishWord.Length; i++)
            {
                uri = uri.Replace(turkishWord[i], changeLatinWord[i]);
            }

            return uri;
        }

    }
}