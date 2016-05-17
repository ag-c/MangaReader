﻿using System.Collections.ObjectModel;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.ViewModel.Commands.Manga;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Manga
{
  public class MangaBaseModel : BaseViewModel
  {
    public readonly Mangas Manga;

    private ObservableCollection<ContentMenuItem> mangaMenu;

    public ObservableCollection<ContentMenuItem> MangaMenu
    {
      get { return mangaMenu; }
      set
      {
        mangaMenu = value;
        OnPropertyChanged();
      }
    }


    public MangaBaseModel(Mangas manga)
    {
      this.Manga = manga;
      this.MangaMenu = new ObservableCollection<ContentMenuItem>
      {
        new OpenFolderCommand(),
        new ChangeUpdateMangaCommand(),
        new UpdateMangaCommand(),
        new CompressMangaCommand(),
        new OpenUrlMangaCommand(),
        new HistoryClearMangaCommand(),
        new DeleteMangaCommand(),
        new ShowPropertiesMangaCommand()
      };
      this.MangaMenu.First().IsDefault = true;
    }
  }
}