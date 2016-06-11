﻿using System;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Mangas
{
  public class From37To38 : MangasConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) &&
        this.Version.CompareTo(ConfigStorage.Instance.DatabaseConfig.Version) > 0 &&
        process.Version.CompareTo(this.Version) >= 0;
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var mainHosts = ConfigStorage.Instance.DatabaseConfig.MangaSettings.Select(s => s.MainUri.Host).Distinct().ToList();
      foreach (var manga in Repository.Get<Manga.Mangas>())
      {
        if (mainHosts.Contains(manga.Uri.Host))
          continue;

        var newHost = manga.Setting.MainUri.Host;
        var builder = new UriBuilder(manga.Uri) { Host = newHost, Port = -1 };
        manga.Uri = builder.Uri;
        manga.Save();
      }
    }

    public From37To38()
    {
      this.Version = new Version(1, 37, 3);
    }
  }
}