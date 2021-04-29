using HtmlAgilityPack;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quewaner.Crawler.ParserHtml.Configs;
using quewaner.Crawler.ParserHtml.HtmlModels.www.meitu131.com;
using System.Text.Json;
using Newtonsoft.Json;

namespace quewaner.Crawler.ParserHtml.www.meitu131.com
{
    public class Meitu131ParserHtml
    {

        /// <summary>
        /// json数据文件夹存放位置
        /// </summary>
        private static string _dataDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), Meitu131Configs.JsonDataPath);

        /// <summary>
        /// 分类json数据文件存放位置
        /// </summary>
        private static string _categoriesDataPath = Path.Combine(_dataDirectoryPath, "categories.json");

        public async Task StartAsync()
        {
            HtmlDocument htmlDocument = await LoadHtmlHelper.LoadHtmlFromUrlAsync(Meitu131Configs.Index);


            FileInfo fileInfo = new FileInfo(_categoriesDataPath);
            //缓存逻辑请自行处理，这里只是演示，每回都重新爬取类别信息
            List<Category> categories = new List<Category>();
            categories = await GetPageUrlsByCategoryAsync(htmlDocument);
            if (categories is not null)
            {
                //获取到写真列表数据的集合
                List<CategoryPhotoAlbum> categoryPhotoAlbums = new List<CategoryPhotoAlbum>();

                //没有获取到的
                List<CategoryPhotoAlbum> noDataCategoryPhotoAlbums = new List<CategoryPhotoAlbum>();
                foreach (Category category in categories)
                {
                    CategoryPhotoAlbum categoryPhotoAlbum = new CategoryPhotoAlbum();
                    categoryPhotoAlbum.ParentTitle = category.Title;
                    CategoryPhotoAlbum noDataCategoryPhotoAlbum = new CategoryPhotoAlbum();
                    noDataCategoryPhotoAlbum.ParentTitle = category.Title;



                    foreach (CategoryItem categoryItem in category.Items)
                    {

                        ChildCategoryPhotoAlbum childCategoryPhotoAlbum = new ChildCategoryPhotoAlbum();
                        childCategoryPhotoAlbum.ChildTitle = categoryItem.Title;

                        ChildCategoryPhotoAlbum noDataChildCategoryPhotoAlbum = new ChildCategoryPhotoAlbum();
                        noDataChildCategoryPhotoAlbum.ChildTitle = categoryItem.Title;

                        int page = 1;
                        foreach (string url in categoryItem.PageUrls)
                        {
                            //当前页面所有的写真集合
                            List<PhotoAlbum> photoAlbums = await GetPhotoAlbumsByUrlAsync(url);
                            if (photoAlbums.Count != 0)
                            {
                                childCategoryPhotoAlbum.PagePhotoAlbums.Add(new PagePhotoAlbum
                                {
                                    Page = page,
                                    PhotoAlbums = photoAlbums
                                });
                                
                            }
                            else
                            {
                                noDataChildCategoryPhotoAlbum.PagePhotoAlbums.Add(new PagePhotoAlbum
                                {
                                    Page = page,
                                    PhotoAlbums = photoAlbums
                                });                  
                            }
                            page++;
                        }
                        categoryPhotoAlbum.childCategoryPhotoAblums.Add(childCategoryPhotoAlbum);
                        noDataCategoryPhotoAlbum.childCategoryPhotoAblums.Add(noDataChildCategoryPhotoAlbum);
                    }
                    categoryPhotoAlbums.Add(categoryPhotoAlbum);
                    noDataCategoryPhotoAlbums.Add(noDataCategoryPhotoAlbum);
                }

                string categoryPhotoAlbumsDataPath = Path.Combine(_dataDirectoryPath, "categoryPhotoAlbums.json");
                await WriteInfoAsync(categoryPhotoAlbums, categoryPhotoAlbumsDataPath);
                string noDataCategoryPhotoAlbumsDataPath = Path.Combine(_dataDirectoryPath, "noDataCategoryPhotoAlbums.json");
                await WriteInfoAsync(categoryPhotoAlbums, noDataCategoryPhotoAlbumsDataPath);
                //写真集合都已经拿到，该怎么保存
            }
        }


        /// <summary>
        /// 根据列表地址获取当前页的写真列表
        /// </summary>
        /// <param name="url">列表地址</param>
        /// <returns></returns>
        public async ValueTask<List<PhotoAlbum>> GetPhotoAlbumsByUrlAsync(string url)
        {
            HtmlDocument htmlDocument = await LoadHtmlHelper.LoadHtmlFromUrlAsync(url);
            //获取所有的li标签
            HtmlNodeCollection htmlNodes = htmlDocument.GetNodes(Meitu131Configs.PhotoAlbumsLisXPath);
            ///html/body/div[1]/div[2]/ul
            if (htmlNodes is not null and { Count: > 0 })
            {
                List<PhotoAlbum> photoAlbums = new List<PhotoAlbum>();
                //遍历写真列表
                foreach (var item in htmlNodes)
                {
                    HtmlNode aNode = item.GetSingleNode(item.XPath + @"/div[1]/a");
                    HtmlNode imgNode = item.GetSingleNode(item.XPath + @"/div[1]/a/img");
                    photoAlbums.Add(new PhotoAlbum
                    {
                        Title = imgNode.GetAttributeValue("alt", "未找到"),
                        Url = aNode.GetAttributeValue("href", "未找到"),
                        Img = imgNode.GetAttributeValue("src", "未找到")
                    });
                }
                return photoAlbums;
            }

            return new List<PhotoAlbum>();
        }

        /// <summary>
        /// 根据分类的全部地址
        /// </summary>
        /// <param name="htmlDocument">文档对象</param>
        /// <returns></returns>
        private async ValueTask<List<Category>> GetPageUrlsByCategoryAsync(HtmlDocument htmlDocument)
        {
            //获取写真分类
            List<Category> categories = await GetCategoriesAsync(htmlDocument);
            if (categories is not null and { Count: > 0 })
            {
                try
                {
                    foreach (Category category in categories)
                    {
                        //这里遍历所属的子类访问URL获取HTML文档
                        foreach (CategoryItem item in category?.Items)
                        {
                            //完善分页列表信息
                            await GetPageUrlsByFirstUrlAsync(item);
                        }
                    }
                    await WriteInfoAsync(categories, _categoriesDataPath);
                }
                catch (Exception ex)
                {

                    throw new Exception("分页地址获取异常", ex);
                }
            }
            return categories;
        }

        /// <summary>
        /// 根据路径获取其他分页路径地址
        /// </summary>
        /// <param name="categoryItem">当前分类</param>
        /// <returns></returns>
        public async Task GetPageUrlsByFirstUrlAsync(CategoryItem categoryItem)
        {

            HtmlDocument htmlDocument = await
                  LoadHtmlHelper.LoadHtmlFromUrlAsync(categoryItem.Url);
            //找到分页的div盒子，获取地址
            HtmlNodeCollection htmlNodes = htmlDocument.GetNodes(Meitu131Configs.PageDivXPath);
            if (htmlNodes is null)
            {

                Console.WriteLine(categoryItem.Title + ":" + categoryItem.Url);
            }
            else
            {
                categoryItem.PageUrls.Add(categoryItem?.Url);
                categoryItem.PageUrls.AddRange(htmlNodes.Select(m => Meitu131Configs.Index + m?.GetAttributeValue("href", "未找到")));
                categoryItem.PageUrls = categoryItem.PageUrls.Distinct().ToList();
            }

        }

        /// <summary>
        /// 获取写真分类
        /// </summary>
        /// <param name="htmlDocument">文档对象</param>
        /// <returns></returns>
        private async ValueTask<List<Category>> GetCategoriesAsync(HtmlDocument htmlDocument)
        {

            //json文件存放位置
            if (!Directory.Exists(_dataDirectoryPath))
            {
                Directory.CreateDirectory(_dataDirectoryPath);
            }

            try
            {
                //获取写真一级分类节点
                HtmlNodeCollection categoriesHtmlNodes = htmlDocument.GetNodes(Meitu131Configs.CategoriesXPath);
                List<Category> categories = new List<Category>(); //一级类别
                if (categoriesHtmlNodes is not null and { Count: > 0 })
                {
                    int i = 0;
                    foreach (var item in categoriesHtmlNodes)
                    {
                        //获取相对应的子类列表
                        HtmlNodeCollection childCategoriesHtmlNodes = htmlDocument.GetNodes(string.Format(Meitu131Configs.ChildCategoriesXPath, i));
                        List<CategoryItem> categoryItems = new List<CategoryItem>(); //子类别
                        if (childCategoriesHtmlNodes is not null and { Count: > 0 })
                        {
                            foreach (var child in childCategoriesHtmlNodes)
                            {
                                //子类别地址
                                var urlNode = child?.GetSingleNode(child.XPath + "/a");
                                //子类别图片
                                var imgNode = child?.GetSingleNode(child?.XPath + "/a/img");
                                categoryItems.Add(new CategoryItem
                                {
                                    Title = child?.InnerText,
                                    Img = imgNode?.GetAttributeValue("src", "未找到"),
                                    Url = Meitu131Configs.Index + urlNode?.GetAttributeValue("href", "未找到")
                                });
                            }
                        }
                        else
                        {
                            childCategoriesHtmlNodes = htmlDocument.GetNodes(string.Format(Meitu131Configs.OrganizationDivXPath));
                            foreach (var child in childCategoriesHtmlNodes)
                            {            
                                categoryItems.Add(new CategoryItem
                                {
                                    Title = child?.InnerText,
                                    Img = child?.GetAttributeValue("src", "未找到"),
                                    Url = Meitu131Configs.Index + child?.GetAttributeValue("href", "未找到")
                                });
                            }

                        }
                        categories.Add(new Category
                        {
                            Title = item.InnerText,
                            Items = categoryItems

                        });
                        i++;
                    }
                }
        
                await WriteInfoAsync(categories, _categoriesDataPath);
                return categories;
            }
            catch (Exception ex)
            {

                throw new Exception("类别爬取异常", ex);
            }


        }


        /// <summary>
        /// 写入分类信息
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        private async Task WriteInfoAsync<T>(T data, string path)
        {
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(data), encoding: Encoding.Default);
        }

        /// <summary>
        /// 读取分类信息
        /// </summary>
        /// <returns></returns>
        private async ValueTask<T> ReadInfoAsync<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path, Encoding.Default));
        }
    }



}
