﻿using System;

namespace MangaReader.Core.Manga
{
  public interface IDownloadable
  {

    #region Свойства

    /// <summary>
    /// Статус загрузки.
    /// </summary>
    bool IsDownloaded { get; }

    /// <summary>
    /// Процент загрузки манги.
    /// </summary>
    double Downloaded { get; set; }

    /// <summary>
    /// Ссылка на загружаемое содержимое.
    /// </summary>
    Uri Uri { get; set; }

    /// <summary>
    /// Папка с мангой.
    /// </summary>
    string Folder { get; }

    #endregion

    #region DownloadProgressChanged

    event EventHandler<Mangas> DownloadProgressChanged;

    #endregion

    #region Методы

    /// <summary>
    /// Скачать все главы.
    /// </summary>
    void Download(string folder = null);

    #endregion

  }
}
