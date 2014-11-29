﻿using NHibernate.Id;

namespace MangaReader.Entity
{
  public class Entity
  {
    public virtual int Id
    {
      set
      {
        if (id == 0 || value == 0)
          id = value;
        else
          throw new IdentifierGenerationException("Нельзя изменять ID сущности.");
      }
      get { return id; }
    }

    private int id = 0;

    /// <summary>
    /// Сохранить в базу.
    /// </summary>
    public virtual void Save()
    {
      var session = Mapping.Environment.Session;
      using (var tranc = session.BeginTransaction())
      {
        if (this.Id == 0)
          session.Save(this);
        else
          session.Update(this);
        tranc.Commit();
      }
    }

    /// <summary>
    ///  Загрузить свежую информацию из базы.
    /// </summary>
    public virtual void Update()
    {
      if (this.Id == 0)
      {
        // TODO: надо бы обнулить всю сущность.
        return;
      }

      Mapping.Environment.Session.Refresh(this);
    }

    /// <summary>
    /// Удалить из базы. Сохранение такой сущности создаст новую в базе.
    /// </summary>
    /// <returns>False, если сущности в базе ещё не было.</returns>
    public virtual bool Delete()
    {
      if (this.Id == 0)
        return false;

      var session = Mapping.Environment.Session;
      using (var tranc = session.BeginTransaction())
      {
        session.Delete(this);
        this.Id = 0;
        tranc.Commit();
      }
      return true;
    }
  }
}