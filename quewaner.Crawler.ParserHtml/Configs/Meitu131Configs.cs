using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace quewaner.Crawler.ParserHtml.Configs
{
    /// <summary>
    /// https://www.meitu131.com/
    /// </summary>
    public  static  class Meitu131Configs
    {
        public static string www_meitu131_com { get; set; } = "https://file.ertuba.com/2020/1205/13bc7bc571a87a893bf4a5a11e268d8a.jpg";

        public const string JsonDataPath = "www.meitu131.com/data";

        public const string Index = "https://www.meitu131.com";

        /// <summary>
        /// 主页类别XPath
        /// </summary>
        public const string CategoriesXPath = @"/html/body/div[3]/div[1]/ul/li";
        
        /// <summary>
        /// 子类类别XPath
        /// </summary>
        public const string ChildCategoriesXPath = @"//*[@id=""tab1_div_{0}""]/ul/li";

        /// <summary>
        /// 机构类别
        /// </summary>
        public const string OrganizationDivXPath = @"//*[@id=""tab1_div_5""]/div/a";

        /// <summary>
        /// 分页盒子所在的XPath
        /// </summary>
        public const string PageDivXPath = @"//*[@id=""pages""]/a";
       

        public const string PageDivXPath2 = @"//*[@id=""pages""]/a";

        /// <summary>
        /// 写真列表Li标签集合
        /// </summary>
        public const string PhotoAlbumsLisXPath = @"/html/body/div[1]/div[2]/ul/li";
    }
}
