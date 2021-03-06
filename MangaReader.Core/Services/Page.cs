﻿using System;
using System.Net;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Core.Exception;
using MangaReader.Core.Properties;

namespace MangaReader.Core.Services
{
  public class Page
  {
    /// <summary>
    /// 4 minutes delay for 429 error
    /// </summary>
    private const int Delay = 240000;

    public bool HasContent { get { return !string.IsNullOrWhiteSpace(this.Content); } }

    public string Content { get; set; }

    public Uri ResponseUri { get; set; }

    public static async Task<bool> DelayOnExpectationFailed(WebException ex)
    {
      if (ex.Response is HttpWebResponse response && (response.StatusCode == HttpStatusCode.ExpectationFailed || response.StatusCode == (HttpStatusCode)429))
      {
        Log.Error($"Доступ к {response.ResponseUri} будет повторно проверен через 4 минуты.");
        await Task.Delay(Delay).ConfigureAwait(false);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Получить текст страницы.
    /// </summary>
    /// <param name="url">Ссылка на страницу.</param>
    /// <param name="client">Клиент.</param>
    /// <param name="restartCounter">Попыток скачивания.</param>
    /// <returns>Исходный код страницы.</returns>
    public static async Task<Page> GetPageAsync(Uri url, CookieClient client, int restartCounter = 0)
    {
      try
      {
        if (restartCounter > 3)
          throw new DownloadAttemptFailed(restartCounter, url);

        using (await ThrottleService.WaitAsync().ConfigureAwait(false))
        {
          var task = client.DownloadStringTaskAsync(url).ConfigureAwait(false);
          return new Page(await task, client.ResponseUri);
        }
      }
      catch (UriFormatException ex)
      {
        Log.Exception(ex, $"Некорректная ссылка: {url}");
        return new Page(url);
      }
      catch (WebException ex)
      {
        if (ex.Status != WebExceptionStatus.Timeout && !(await DelayOnExpectationFailed(ex).ConfigureAwait(false)) &&
            ex.HResult != -2146893023 && ex.HResult != 2147012721)
        {
          Log.Exception(ex, $"{Strings.Page_GetPage_SiteOff}, ссылка: {url}, попытка номер - {restartCounter}");
          return new Page(url);
        }
        ++restartCounter;
        return await GetPageAsync(url, client, restartCounter).ConfigureAwait(false);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, $"Не удалось получить страницу: {url}");
        return new Page(url);
      }
    }

    public Page()
    {

    }

    public Page(Uri response)
    {
      this.ResponseUri = response;
    }

    public Page(string content, Uri response) : this(response)
    {
      this.Content = content;
    }
  }
}
