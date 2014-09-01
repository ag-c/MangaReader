﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace MangaReader.Manga.Grouple
{
  public static class Getter
  {
    /// <summary>
    /// Получить название манги.
    /// </summary>
    /// <param name="mangaMainPage">Содержимое страницы манги.</param>
    /// <returns>Мультиязыковый класс с именем манги.</returns>
    public static MangaName GetMangaName(string mangaMainPage)
    {
      var name = new MangaName();
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(mangaMainPage);
        var japNode = document.DocumentNode.SelectSingleNode("//div[@class=\"leftContent manga-page\"]//span[@class='jp-name']");
        if (japNode != null)
          name.Japanese = System.Net.WebUtility.HtmlDecode(japNode.InnerText);
        var engNode = document.DocumentNode.SelectSingleNode("//div[@class=\"leftContent manga-page\"]//span[@class='eng-name']");
        if (engNode != null)
          name.English = System.Net.WebUtility.HtmlDecode(engNode.InnerText);
        var rusNode = document.DocumentNode.SelectSingleNode("//div[@class=\"leftContent manga-page\"]//span[@class='name']");
        if (rusNode != null)
          name.Russian = System.Net.WebUtility.HtmlDecode(rusNode.InnerText);
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      return name;
    }

    public static string GetTranslateStatus(string mangaMainPage)
    {
      var status = string.Empty;
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(mangaMainPage);
        var nodes = document.DocumentNode.SelectNodes("//div[@class=\"subject-meta\"]//p");
        if (nodes != null)
          status = nodes.Aggregate(status, (current, node) =>
              current + Regex.Replace(node.InnerText.Trim(), @"\s+", " ").Replace("\n", "") + Environment.NewLine);
      }
      catch (NullReferenceException ex) { Log.Exception(ex); }
      return status;
    }

    /// <summary>
    /// Получить обложку манги.
    /// </summary>
    /// <param name="mangaMainPage">Содержимое страницы манги.</param>
    /// <returns>Картинка для обложки.</returns>
    public static byte[] GetMangaCover(string mangaMainPage)
    {
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(mangaMainPage);
        var links = document.DocumentNode.SelectNodes("//div[@class=\"subject-cower\"]//img[@src]")
            .ToList()
            .ConvertAll(r => r.Attributes.ToList().ConvertAll(i => i.Value))
            .SelectMany(j => j)
            .Where(k => k != string.Empty && k.StartsWith("http"))
            .ToList();
        return links.Any() ?
            Page.GetThumbnail(Page.DownloadFile(links.OrderBy(x => Guid.NewGuid()).FirstOrDefault())) :
            null;
      }
      catch (NullReferenceException ex)
      {
        Log.Exception(ex);
        return null;
      }
      catch (ArgumentNullException)
      {
        return null;
      }
    }

    /// <summary>
    /// Получить ссылки на главы манги.
    /// </summary>
    /// <param name="mangaMainPage">Содержимое страницы манги.</param>
    /// <param name="url">Ссылка на мангу.</param>
    /// <returns>Словарь (ссылка, описание).</returns>
    public static Dictionary<string, string> GetLinksOfMangaChapters(string mangaMainPage, string url)
    {
      var dic = new Dictionary<string, string>();
      var links = new List<string> { };
      var description = new List<string> { };
      try
      {
        var document = new HtmlDocument();
        document.LoadHtml(mangaMainPage);
        links = document.DocumentNode
            .SelectNodes("//div[@class=\"expandable chapters-link\"]//td[@class=\" \"]//a[@href]")
            .ToList()
            .ConvertAll(r => r.Attributes.ToList().ConvertAll(i => i.Value))
            .SelectMany(j => j)
            .Where(k => k != string.Empty)
            .Select(s => @"http://" + new Uri(url).Host + s + "?mature=1")
            .Reverse()
            .ToList();
        description = document.DocumentNode
            .SelectNodes("//div[@class=\"expandable chapters-link\"]//tr//td[@class=\" \"]")
            .Reverse()
            .ToList()
            .ConvertAll(r => r.InnerText.Replace("\r\n", string.Empty).Trim())
            .ToList();
      }
      catch (NullReferenceException ex) { Log.Exception(ex, "Ошибка получения списка глав.", url); }
      catch (ArgumentNullException ex) { Log.Exception(ex, "Главы не найдены.", url); }

      for (var i = 0; i < links.Count; i++)
      {
        dic.Add(links[i], description[i]);
      }

      return dic;
    }

    /// <summary>
    /// Получить ссылки на все изображения в главе.
    /// </summary>
    /// <param name="url">Ссылка на главу.</param>
    /// <returns>Список ссылок на изображения главы.</returns>
    public static List<string> GetImagesLink(string url)
    {
      var chapterLinksList = new List<string>();
      var document = new HtmlDocument();
      document.LoadHtml(Page.GetPage(url));

      var firstOrDefault = document.DocumentNode
          .SelectNodes("//div[@class=\"pageBlock reader-bottom\"]")
          .FirstOrDefault();

      if (firstOrDefault == null)
        return chapterLinksList;

      chapterLinksList = Regex
          .Matches(firstOrDefault.OuterHtml, @"{url:""(.*?)""", RegexOptions.IgnoreCase)
          .OfType<Group>()
          .Select(g => g.Captures[0])
          .OfType<Match>()
          .Select(m => m.Groups[1].Value)
          .Select(s => (!Uri.IsWellFormedUriString(s, UriKind.Absolute)) ? (@"http://" + new Uri(url).Host + s) : s)
          .ToList();
      return chapterLinksList;
    }
  }
}