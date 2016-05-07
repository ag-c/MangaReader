﻿using System;
using System.Threading;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Core.Update;

namespace MangaReader.Core
{
  public static class Client
  {
    private static Mutex mutex;

    public static void Init()
    {
      AppDomain.CurrentDomain.UnhandledException += (o, a) => Log.Exception(a.ExceptionObject as System.Exception);
      AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly.ResolveInternalAssembly;
    }

    public static void Start(IProcess process)
    {
      Updater.Initialize(process);

      var isSingle = false;
      mutex = new Mutex(false, "5197317b-a6f6-4a6c-a336-6fbf8642b7bc", out isSingle);
      if (!isSingle)
      {
        try
        {
          Log.Exception(new ApplicationException("Программа уже запущена."));
        }
        finally
        {
          Environment.Exit(1);
        }
      }

      ResolveAssembly.LoadSql();
      NHibernate.Mapping.Initialize(process);
      Services.Converter.Convert(process);
      Log.AddFormat("Found {0} manga type settings:", ConfigStorage.Instance.DatabaseConfig.MangaSettings.Count);
      ConfigStorage.Instance.DatabaseConfig.MangaSettings.ForEach(s => Log.AddFormat("Load settings for {0}, guid {1}.", s.MangaName, s.Manga));
    }

    public static void Close()
    {
//      Library.ThreadAbort();
      ConfigStorage.Instance.Save();
      ConfigStorage.Close();
      NHibernate.Mapping.Close();

      if (mutex != null && !mutex.SafeWaitHandle.IsClosed)
        mutex.Close();
    }
  }
}