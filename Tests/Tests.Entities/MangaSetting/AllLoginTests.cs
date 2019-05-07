﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Acomics;
using Grouple;
using Hentai2Read.com;
using Hentaichan;
using Hentaichan.Mangachan;
using MangaReader.Core.Account;
using NUnit.Framework;

namespace Tests.Entities.MangaSetting
{
  [TestFixture]
  public class AllLoginTests : TestClass
  {
    public static IEnumerable<ILogin> GetLogins()
    {
      yield return new GroupleLogin() { Name = "alex+grouple@antistarforce.com", Password = "JUadiSHrosiv" };
      yield return new AcomicsLogin() { Name = "v924147", Password = "ocUsigairtYL" };
      yield return new HentaichanLogin() { Name = "v924147", Password = "OViLKHoTCHBL" };
      yield return new MangachanLogin() { Name = "v924147", Password = "gOWElaTERSIt" };
      yield return new Hentai2ReadLogin() { Name = "v924147", Password = "ToRTUbiCONDe" };
    }

    [Test, TestCaseSource(nameof(GetLogins))]
    public async Task LoginUnlogin(ILogin login)
    {
      Assert.IsTrue(login.CanLogin);
      Assert.IsFalse(login.IsLogined);
      var loginResult = await login.DoLogin().ConfigureAwait(false);
      Assert.IsTrue(loginResult);
      Assert.IsTrue(login.CanLogin);
      Assert.IsTrue(login.IsLogined);
      var logoutResult = await login.Logout().ConfigureAwait(false);
      Assert.IsTrue(logoutResult);
      Assert.IsTrue(login.CanLogin);
      Assert.IsFalse(login.IsLogined);
    }

    [Test, TestCaseSource(nameof(GetLogins))]
    public async Task GetBookmarks(ILogin login)
    {
      await login.DoLogin().ConfigureAwait(false);
      Assert.IsTrue(login.IsLogined);

      var bookmarks = await login.GetBookmarks().ConfigureAwait(false);
      Assert.AreEqual(2, bookmarks.Count);
      foreach (var manga in bookmarks)
      {
        Assert.IsNotNull(manga.Name);
        Assert.IsNotNull(manga.Uri);
      }
    }
  }
}
