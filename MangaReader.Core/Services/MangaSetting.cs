﻿using System;
using MangaReader.Core.Account;

namespace MangaReader.Core.Services
{
  public class MangaSetting : Entity.Entity
  {
    public Guid Manga { get; set; }

    public string MangaName { get; set; }

    public string Folder { get; set; }

    /// <summary>
    /// Сжимать скачанную мангу.
    /// </summary>
    public bool CompressManga { get; set; }

    /// <summary>
    /// Обновлять при скачивании (true) или скачивать целиком(false).
    /// </summary>
    public bool OnlyUpdate { get; set; }

    public virtual Login Login
    {
      get { return login ?? (login = Login.Create(this.Manga)); }
      set { login = value; }
    }

    private Login login;

    public virtual Compression.CompressionMode DefaultCompression { get; set; }

    public MangaSetting()
    {
      this.CompressManga = true;
      this.OnlyUpdate = true;
    }
  }
}
