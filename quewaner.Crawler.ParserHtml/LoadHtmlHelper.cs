using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace quewaner.Crawler.ParserHtml
{
    class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return request;
        }
    }
    /// <summary>
    /// 下载HTML帮助类
    /// </summary>
    public static class LoadHtmlHelper
    {
        /// <summary>
        /// 从Url地址下载页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static ValueTask<HtmlDocument> LoadHtmlFromUrlAsync(string url)
        {
            var data = new MyWebClient()?.DownloadString(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(data);
            return doc;
        }

        /// <summary>
        /// 获取单个节点扩展方法
        /// </summary>
        /// <param name="htmlDocument">文档对象</param>
        /// <param name="xPath">xPath路径</param>
        /// <returns></returns>
        public static HtmlNode GetSingleNode(this HtmlDocument htmlDocument, string xPath)
        {
          return  htmlDocument?.DocumentNode?.SelectSingleNode(xPath);
        }

        /// <summary>
        /// 获取多个节点扩展方法
        /// </summary>
        /// <param name="htmlDocument">文档对象</param>
        /// <param name="xPath">xPath路径</param>
        /// <returns></returns>
        public static HtmlNodeCollection GetNodes(this HtmlDocument htmlDocument, string xPath)
        {
            return htmlDocument?.DocumentNode?.SelectNodes(xPath);
        }

     

        /// <summary>
        /// 获取多个节点扩展方法
        /// </summary>
        /// <param name="htmlDocument">文档对象</param>
        /// <param name="xPath">xPath路径</param>
        /// <returns></returns>
        public static HtmlNodeCollection GetNodes(this HtmlNode htmlNode, string xPath)
        {
            return htmlNode?.SelectNodes(xPath);
        }


        /// <summary>
        /// 获取单个节点扩展方法
        /// </summary>
        /// <param name="htmlDocument">文档对象</param>
        /// <param name="xPath">xPath路径</param>
        /// <returns></returns>
        public static HtmlNode GetSingleNode(this HtmlNode htmlNode, string xPath)
        {
            return htmlNode?.SelectSingleNode(xPath);
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="filpath">文件路径</param>
        /// <returns></returns>
        public async static ValueTask<bool> DownloadImg(string url ,string filpath)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                var bytes = await httpClient.GetByteArrayAsync(url);
                using (FileStream fs = File.Create(filpath))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
                return File.Exists(filpath);
            }
            catch (Exception ex)
            {
             
                throw new Exception("下载图片异常", ex);
            }
            
        }
    }
}
