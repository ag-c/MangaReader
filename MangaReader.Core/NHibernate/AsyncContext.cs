﻿using System;
using System.Threading;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Context;

namespace MangaReader.Core.NHibernate
{
  // Session contextes are serializable while not actually serializing any data. Others contextes just retrieve it back
  // from their context, if it does still live when/where they are de-serialized. For having that with AsyncLocal,
  // we would need to store it as static, and then we need to use a MapBasedSessionContext.
  // But this would cause bindings operations done in inner flows to be potentially propagated to outer flows, depending
  // on which flow has initialized the map. This is undesirable.
  // So current implementation just loses its context in case of serialization, since AsyncLocal is not serializable.
  // Another option would be to change MapBasedSessionContext for recreating a new dictionary from the
  // previous one at each change, essentially using those dictionaries as immutable objects.
  /// <summary>
  /// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
  /// for current asynchronous flow.
  /// </summary>
  /// <remarks>https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/Context/AsyncLocalSessionContext.cs</remarks>
  [Serializable]
  public class AsyncLocalSessionContext : CurrentSessionContext
  {
    private readonly AsyncLocal<ISession> session = new AsyncLocal<ISession>();

    // Constructor signature required for dynamic invocation code.
    public AsyncLocalSessionContext(ISessionFactoryImplementor factory) { }

    protected override ISession Session
    {
      get { return session.Value; }
      set { session.Value = value; }
    }
  }
}