﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Convertation
{
  public class HentaichanFrom38To39 : ConfigConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
        if (setting != null)
        {
          setting.MainUri = new Uri("http://henchan.me");
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public HentaichanFrom38To39()
    {
      this.Version = new Version(1, 38, 6);
      this.CanReportProcess = true;
    }
  }
}