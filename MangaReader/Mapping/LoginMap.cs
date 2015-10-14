﻿using FluentNHibernate.Mapping;

namespace MangaReader.Mapping
{
  public class LoginMap : ClassMap<Account.Login>
  {
    public LoginMap()
    {
      Id(x => x.Id);
      Map(x => x.Name).Not.LazyLoad();
      Map(x => x.Password).Not.LazyLoad();
      DiscriminateSubClassesOnColumn("Type", Account.Login.Type.ToString());
    }
  }

  public class HentaichanLoginMap : SubclassMap<Manga.Hentaichan.HentaichanLogin>
  {
    public HentaichanLoginMap()
    {
      Map(x => x.UserId).Not.LazyLoad();
      Map(x => x.PasswordHash).Not.LazyLoad();
      DiscriminatorValue(Manga.Hentaichan.HentaichanLogin.Type.ToString());
    }
  }
}
