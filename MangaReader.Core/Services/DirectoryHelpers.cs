﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using MangaReader.Core.Exception;

namespace MangaReader.Core.Services
{
  public static class DirectoryHelpers
  {
    public static bool Equals(string one, string two)
    {
      return string.Compare(new DirectoryInfo(one).FullName.TrimEnd('\\'),
        new DirectoryInfo(two).FullName.TrimEnd('\\'),
        StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    private static void CopyDirectory(string sourceFolder, string destFolder)
    {
      try
      {
        if (!Directory.Exists(destFolder))
          Directory.CreateDirectory(destFolder);
        var files = Directory.GetFiles(sourceFolder);
        foreach (var file in files)
        {
          var name = Path.GetFileName(file);
          var dest = Path.Combine(destFolder, name);
          File.Copy(file, dest);
        }
        var folders = Directory.GetDirectories(sourceFolder);
        foreach (var folder in folders)
        {
          var name = Path.GetFileName(folder);
          var dest = Path.Combine(destFolder, name);
          CopyDirectory(folder, dest);
        }
      }
      catch (System.Exception e)
      {
        throw new CopyDirectoryFailed("Не удалось скопировать файл или папку. См InnerException.", sourceFolder, destFolder, e);
      }
    }

    internal static void MoveDirectory(string sourceFolder, string destFolder)
    {
      try
      {
        CopyDirectory(sourceFolder, destFolder);
        Directory.Delete(sourceFolder, true);
      }
      catch (System.Exception ex)
      {
        throw new CopyDirectoryFailed("Не удалось переместить папку. См InnerException.", sourceFolder, destFolder, ex);
      }
    }

    /// <summary>
    /// Очистка пути от недопустимых символов.
    /// </summary>
    /// <param name="name">Путь.</param>
    /// <returns>Исправленный путь.</returns>
    public static string MakeValidPath(string name)
    {
      const string replacement = ".";
      var matchesCount = Regex.Matches(name, @":\\").Count;
      string correctName;
      if (matchesCount > 0)
      {
        var regex = new Regex(@":", RegexOptions.RightToLeft);
        correctName = regex.Replace(name, replacement, regex.Matches(name).Count - matchesCount);
      }
      else
        correctName = name.Replace(":", replacement);

      var invalidChars = Regex.Escape(string.Concat(new string(Path.GetInvalidPathChars()), "?", "/", "*"));
      var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

      return Regex.Replace(correctName, invalidRegStr, replacement);
    }
  }
}