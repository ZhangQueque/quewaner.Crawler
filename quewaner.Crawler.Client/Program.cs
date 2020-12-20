using System;
using System.Threading.Tasks;
using System.IO;
using quewaner.Crawler.ParserHtml.www.meitu131.com;
using System.Text;

namespace quewaner.Crawler.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
             Meitu131ParserHtml meitu131ParserHtml = new Meitu131ParserHtml();
            await meitu131ParserHtml.StartAsync();
               Console.ReadKey();
        }
    }
}
