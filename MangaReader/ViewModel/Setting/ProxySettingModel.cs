﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Core.Account;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Setting;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Setting
{
  public class ProxySettingModel : BaseViewModel
  {
    public ProxySettingType SettingType { get; set; }

    public int Id
    {
      get => id;
      set
      {
        id = value;
        OnPropertyChanged();
      }
    }

    private int id;

    public string Name
    {
      get => name;
      set
      {
        name = value;
        OnPropertyChanged();
      }
    }

    private string name;

    public Uri Address
    {
      get => address;
      set
      {
        address = value;
        OnPropertyChanged();
      }
    }

    private Uri address;

    public string UserName
    {
      get => userName;
      set
      {
        userName = value;
        OnPropertyChanged();
      }
    }

    private string userName;

    public string Password
    {
      get => password;
      set
      {
        password = value;
        OnPropertyChanged();
      }
    }

    private string password;

    public bool IsManual { get; private set; }

    public ProxySettingModel(ProxySetting setting)
    {
      Id = setting.Id;
      Name = setting.Name;
      Address = setting.Address;
      UserName = setting.UserName;
      Password = setting.Password;
      SettingType = setting.SettingType;
      IsManual = setting.SettingType == ProxySettingType.Manual;
    }

  }
}
