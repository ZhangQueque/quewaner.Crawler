using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quewaner.Crawler.ParserHtml.HtmlModels.www.meitu131.com
{
    /// <summary>
    /// 写真集合归纳
    /// </summary>
    public class  CategoryPhotoAlbum
    {
        /// <summary>
        /// 一级类别名称
        /// </summary>
        public string ParentTitle { get; set; }

        /// <summary>
        /// 子类别数据
        /// </summary>
        public List<ChildCategoryPhotoAlbum> childCategoryPhotoAblums { get; set; } = new List<ChildCategoryPhotoAlbum>();
    }

    /// <summary>
    /// 二级类别
    /// </summary>
    public class ChildCategoryPhotoAlbum
    {
        /// <summary>
        /// 二级类别名称
        /// </summary>
        public string ChildTitle { get; set; }

        /// <summary>
        /// 二级类别数据
        /// </summary>
        public List<PagePhotoAlbum> PagePhotoAlbums { get; set; } = new List<PagePhotoAlbum>();
    }

    /// <summary>
    /// 数据
    /// </summary>
    public class PagePhotoAlbum
    {
        /// <summary>
        /// 页索引
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 页面的写真集合
        /// </summary>
        public List<PhotoAlbum> PhotoAlbums { get; set; } = new List<PhotoAlbum>();
    }
}
