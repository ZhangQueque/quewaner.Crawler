using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quewaner.Crawler.ParserHtml.HtmlModels.www.meitu131.com
{
    /// <summary>
    /// 类别
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 所有的子类
        /// </summary>
        public List<CategoryItem> Items { get; set; }
    }

    /// <summary>
    /// 子类详情
    /// </summary>
    public class CategoryItem
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// d地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Img { get; set; }

        /// <summary>
        /// 所有的分页地址
        /// </summary>
        public List<string> PageUrls { get; set; } = new List<string>();
    }
}
