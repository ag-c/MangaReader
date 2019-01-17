﻿using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Avalonia.Properties;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class OpenUrlMangaCommand : MultipleMangasBaseCommand
  {
    public override void Execute(IEnumerable<IManga> mangas)
    {
      foreach (var manga in mangas)
      {
        if (true)
          Helper.StartUseShell(manga.Uri.OriginalString);
        else
        {
          var lastVolume = manga.Volumes.OrderByDescending(v => v.Number).FirstOrDefault();
          var lastChapter = (lastVolume?.Container ?? manga.Chapters).OrderByDescending(v => v.Number).FirstOrDefault();
          var lastPage = (lastChapter?.Container ?? manga.Pages).OrderByDescending(v => v.Number).FirstOrDefault();
          var downloadable = lastPage ?? (IDownloadable)lastChapter ?? lastVolume;
          var s = lastVolume?.Number + lastChapter?.Number + lastPage?.Number;
          Helper.StartUseShell(downloadable?.Uri?.OriginalString ?? manga.Uri.OriginalString);
        }
      }
    }

    public override bool CanExecute(object parameter)
    {
      return CanExecuteMangaCommand();
    }

    public OpenUrlMangaCommand(Explorer.LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_View;
      this.Icon = "pack://application:,,,/Icons/Manga/www.png";
      this.NeedRefresh = false;
    }
  }
}