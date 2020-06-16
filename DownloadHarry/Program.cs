using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace DownloadHarry
{
    class Program
    {
        /// <summary>
        /// Скачивает полную коллекцию Гарри Поттера в txt формате с указанных сайтов. Переходя от главы к главе, находя ссылку на следующую страницу.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine($"Скачивание полной коллекции Гарри Поттера");
            SaveHarry("https://potter1.bib.bz/glava-1-malchik-kotoryy-vyzhil", "Harry1.txt");
            SaveHarry("https://potter-2.bib.bz/glava-1-den-rozhdeniya-huzhe-nekuda", "Harry2.txt");
            SaveHarry("https://potter3.bib.bz/glava-1-sovinaya-pochta", "Harry3.txt");
            SaveHarry("https://potter4.bib.bz/glava-1-dom-reddlov", "Harry4.txt");
            SaveHarry("https://potter-5.bib.bz/glava-1-dadli-dostalos", "Harry5.txt");
            SaveHarry("https://potter6.bib.bz/glava-1-drugoy-ministr", "Harry6.txt");
            SaveHarry("https://potter7.bib.bz/glava-1-vozvyshenie-temnogo-lorda", "Harry7.txt");
        }


        /// <summary>
        /// Скачать по указанной ссылке книгу и сохранить её на диск
        /// </summary>
        /// <param name="link">Ссылка на 1 главу книги</param>
        /// <param name="outputFile">Имя выходного файла</param>
        static void SaveHarry(string link, string outputFile)
        {
            Console.WriteLine($"Скачать {link}");
            var Client = new WebClient();
            var FirstUri = new Uri(link);

            string HTML = null;
            string NextLink = link;            
            StringBuilder Result = new StringBuilder();

            //скачать 1 главу, распарсить, добавить к Result, перейти к 2 главе
            //скачать 2 главу, распарсить, добавить к Result, перейти к 3 главе
            //...
            //и так пока есть ссылки на следующую главу
            do
            {
                HTML = GetHTMLFromLink(Client, NextLink);
                string ChapterOfBook = GetChapterOfBookFromHTML(HTML);
                Result.AppendLine(ChapterOfBook);
                
                //найти ссылку на следующую главу                
                NextLink = GetHTMLInsideTag(HTML, "<div id=\"next\"><a href=\"", "\"");
                if (NextLink != null)                                
                    NextLink = FirstUri.Scheme + "://" + FirstUri.Host + NextLink;                                    
                else                
                    //если ссылки на следующую главу нет, закончить 
                    break;
            } while (true);


            File.WriteAllText(outputFile, Result.ToString());
            Console.WriteLine($"Сохранено в {outputFile}");
            Console.WriteLine("=================================\r\n");
        }


        /// <summary>
        /// Получить HTML по ссылке
        /// </summary>
        /// <param name="client">Экземляр WebClient</param>
        /// <param name="link">Ссылка на страницу</param>
        /// <returns></returns>
        static string GetHTMLFromLink(WebClient client, string link)
        {
            var Data = client.DownloadData(link);
            return Encoding.UTF8.GetString(Data);
        }


        /// <summary>
        /// Получить текст одной главы книги из HTML
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        static string GetChapterOfBookFromHTML(string html)
        {
            string RawChapter = GetHTMLInsideTag(html, "<section id=\"main\">", "</article>") + "\r\n\r\n\r\n";
            if (RawChapter == null)            
                Debugger.Break();
            
            StringBuilder ChapterOfBook = new StringBuilder(RawChapter);
            ChapterOfBook = ChapterOfBook.Replace("<p>", "     ").
                Replace("</p>", "").
                Replace("&shy;", "").
                Replace("<article><header><h1 id=\"chapter\">", "").
                Replace("</h1></header>", "\r\n\r\n");
            return ChapterOfBook.ToString();
        }


        /// <summary>
        /// Получить HTML содержимое внутри указанных тегов
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tagBegin">начальный тег</param>
        /// <param name="tagEnd">конечный тег</param>
        /// <returns></returns>
        static string GetHTMLInsideTag(string html, string tagBegin, string tagEnd) 
        {            
            int b = html.IndexOf(tagBegin);
            if (b == -1) return null;
            b += tagBegin.Length;
            int e = html.IndexOf(tagEnd, b);
            if (e == -1) return null;
            return html.Substring(b, e - b);
        }
    }
}
