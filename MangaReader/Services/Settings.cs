﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;
using MangaReader.Manga.Acomic;
using MangaReader.Manga.Grouple;
using NHibernate.Linq;

namespace MangaReader.Services
{
  public static class Settings
  {
    /// <summary>
    /// Язык манги.
    /// </summary>
    public static Languages Language = Languages.English;

    /// <summary>
    /// Сворачивать в трей.
    /// </summary>
    public static bool MinimizeToTray { get; set; }

    /// <summary>
    /// Частота автообновления библиотеки в часах.
    /// </summary>
    public static int AutoUpdateInHours = 0;

    /// <summary>
    /// Время последнего обновления.
    /// </summary>
    public static DateTime LastUpdate = DateTime.Now;

    /// <summary>
    /// Версия базы данных.
    /// </summary>
    public static Version DatabaseVersion = new Version(1, 0, 0, 0);

    /// <summary>
    /// Версия приложения.
    /// </summary>
    public static Version AppVersion = Assembly.GetExecutingAssembly().GetName().Version;

    /// <summary>
    /// Папка программы.
    /// </summary>
    public static readonly string WorkFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    /// <summary>
    /// Настройки программы.
    /// </summary>
    private static readonly string SettingsPath = WorkFolder + "\\settings.xml";

    /// <summary>
    /// Автообновление программы.
    /// </summary>
    public static bool UpdateReader { get; set; }

    /// <summary>
    /// Папка загрузки.
    /// </summary>
    public static string DownloadFolder = WorkFolder + "\\Download\\";

    /// <summary>
    /// Префикс папки томов.
    /// </summary>
    public static string VolumePrefix = "Volume ";

    /// <summary>
    /// Префикс папки глав.
    /// </summary>
    public static string ChapterPrefix = "Chapter ";

    [XmlIgnore]
    public static List<MangaSetting> MangaSettings
    {
      get
      {
        if (mangaSettings == null)
        {
          if (Mapping.Environment.Initialized)
          {
            var query = Mapping.Environment.Session.Query<MangaSetting>().ToList();
            mangaSettings = CreateDefaultMangaSettings(query);
          }
          else
          {
            Log.Exception("Запрос MangaSettings до инициализации соединения с базой.");
            return new List<MangaSetting>();
          }
        }
        return mangaSettings;
      }
      set { }
    }

    private static List<MangaSetting> mangaSettings;

    /// <summary>
    /// Состояние окна.
    /// </summary>
    public static object[] WindowsState;

    /// <summary>
    /// Сохранить настройки.
    /// </summary>
    public static void Save()
    {
      MangaSettings.ForEach(a => a.Save());
      object[] settings = 
            {
                Language,
                null, // Update
                UpdateReader,
                null,
                null, // CompressManga
                WindowsState,
                new object[] {null, null},
                MinimizeToTray,
                AutoUpdateInHours,
                DatabaseVersion.ToString()
            };
      Serializer<object[]>.Save(SettingsPath, settings);
    }

    /// <summary>
    /// Загрузить настройки.
    /// </summary>
    public static void Load()
    {
      var settings = Serializer<object[]>.Load(SettingsPath);
      if (settings == null)
        return;

      try
      {
        Language = (Languages)settings[0];
        Console.WriteLine("Language {0}", Language);
        UpdateReader = (bool)settings[2];
        Console.WriteLine("Autoupdate Mangareader {0}", UpdateReader);
        WindowsState = (object[])settings[5];
        MinimizeToTray = (bool)settings[7];
        Console.WriteLine("Minimize to tray {0}", MinimizeToTray);
        AutoUpdateInHours = (int)settings[8];
        Console.WriteLine("Update mangas ever {0} hours, if its not zero.", AutoUpdateInHours);
        DatabaseVersion = new Version((string)settings[9]);
        Console.WriteLine("Curent database version = {0}.", DatabaseVersion);
      }
      catch (IndexOutOfRangeException) { }
    }

    private static List<MangaSetting> CreateDefaultMangaSettings(List<MangaSetting> query)
    {
      var baseClass = typeof(Manga.Mangas);
      var types = Assembly.GetAssembly(baseClass).GetTypes()
        .Where(type => type.IsSubclassOf(baseClass));

      foreach (var type in types)
      {
        if (query.Any(s => Equals(s.Manga, type.MangaType())))
          continue;

        var setting = new MangaSetting
        {
          Folder = Settings.DownloadFolder,
          Manga = type.MangaType(),
          MangaName = type.Name,
          DefaultCompression = Compression.CompressionMode.Manga
        };

        setting.Save();
        query.Add(setting);
      }
      return query;
    }

    public static void Convert()
    {
      var settings = Serializer<object[]>.Load(SettingsPath);
      if (settings == null)
        return;

      try
      {
        var query = Mapping.Environment.Session.Query<MangaSetting>().ToList();
        if (settings[3] is object[])
        {
          var setting = settings[3] as object[];
          if (query.FirstOrDefault(a => a.Manga == Readmanga.Type) == null)
            new MangaSetting() 
            { 
              Folder = setting[0] as string, 
              Manga = Readmanga.Type, 
              MangaName = "Readmanga", 
              DefaultCompression = Compression.CompressionMode.Volume
            }.Save();
          if (query.FirstOrDefault(a => a.Manga == Acomics.Type) == null)
            new MangaSetting() 
            { 
              Folder = setting[1] as string, 
              Manga = Acomics.Type, 
              MangaName = "Acomics", 
              DefaultCompression = Compression.CompressionMode.Manga 
            }.Save();
        }
        if (settings[1] is bool)
          query.ForEach(ms => ms.OnlyUpdate = (bool)settings[1]);
        if (settings[4] is bool)
          query.ForEach(ms => ms.CompressManga = (bool)settings[4]);
        query.ForEach(ms => ms.Save());
        Serializer<object[]>.Save(SettingsPath, settings);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    /// <summary>
    /// Загрузить положение и размеры окна.
    /// </summary>
    /// <param name="main">Окно.</param>
    public static void UpdateWindowsState(Window main)
    {
      if (WindowsState == null)
        return;
      try
      {
        main.Top = (double)WindowsState[0];
        main.Left = (double)WindowsState[1];
        main.Width = (double)WindowsState[2];
        main.Height = (double)WindowsState[3];
        main.WindowState = (WindowState)WindowsState[4];
      }
      catch (IndexOutOfRangeException) { }
    }

    /// <summary>
    /// Доступные языки.
    /// </summary>
    public enum Languages
    {
      English,
      Russian,
      Japanese
    }

    static Settings()
    {
      Load();
    }
  }
}
