using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame
{
	public class MPCustomGameFiltersVM : ViewModel
	{
		public MPCustomGameFiltersVM()
		{
			this.SearchText = string.Empty;
			MBBindingList<MPCustomGameFilterItemVM> mbbindingList = new MBBindingList<MPCustomGameFilterItemVM>();
			mbbindingList.Add(new MPCustomGameFilterItemVM(MPCustomGameFiltersVM.CustomGameFilterType.IsOfficial, new TextObject("{=Tlc2buKG}Is Official", null), (GameServerEntry x) => x.IsOfficial, new Action(this.OnAnyFilterChange)));
			mbbindingList.Add(new MPCustomGameFilterItemVM(MPCustomGameFiltersVM.CustomGameFilterType.HasPlayers, new TextObject("{=aB4Md0if}Has players", null), (GameServerEntry x) => x.PlayerCount > 0, new Action(this.OnAnyFilterChange)));
			mbbindingList.Add(new MPCustomGameFilterItemVM(MPCustomGameFiltersVM.CustomGameFilterType.HasPasswordProtection, new TextObject("{=v6J8ILV3}No password", null), (GameServerEntry x) => !x.PasswordProtected, new Action(this.OnAnyFilterChange)));
			mbbindingList.Add(new MPCustomGameFilterItemVM(MPCustomGameFiltersVM.CustomGameFilterType.NotFull, new TextObject("{=W4DLzPSb}Server not full", null), (GameServerEntry x) => x.MaxPlayerCount - x.PlayerCount > 0, new Action(this.OnAnyFilterChange)));
			mbbindingList.Add(new MPCustomGameFilterItemVM(MPCustomGameFiltersVM.CustomGameFilterType.ModuleCompatible, new TextObject("{=CNR4cZwZ}Modules compatible", null), new Func<GameServerEntry, bool>(this.FilterByCompatibleModules), new Action(this.OnAnyFilterChange)));
			this.Items = mbbindingList;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=OwqFpPwa}Filters", null).ToString();
			this.SearchInitialText = new TextObject("{=NLKmdNbt}Search", null).ToString();
			this.Items.ApplyActionOnAllItems(delegate(MPCustomGameFilterItemVM x)
			{
				x.RefreshValues();
			});
		}

		public List<GameServerEntry> GetFilteredServerList(IEnumerable<GameServerEntry> unfilteredList)
		{
			List<GameServerEntry> list = unfilteredList.ToList<GameServerEntry>();
			IEnumerable<MPCustomGameFilterItemVM> enabledFilterItems = this.Items.Where((MPCustomGameFilterItemVM filterItem) => filterItem.IsSelected);
			if (enabledFilterItems.Any<MPCustomGameFilterItemVM>())
			{
				list.RemoveAll((GameServerEntry s) => enabledFilterItems.Any((MPCustomGameFilterItemVM fi) => !fi.GetIsApplicaple(s)));
			}
			if (!string.IsNullOrEmpty(this.SearchText))
			{
				list = list.Where((GameServerEntry i) => i.ServerName.IndexOf(this.SearchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList<GameServerEntry>();
			}
			return list;
		}

		private bool FilterByCompatibleModules(GameServerEntry serverEntry)
		{
			return NetworkMain.GameClient.LoadedUnofficialModules.IsCompatibleWith(serverEntry.LoadedModules, serverEntry.AllowsOptionalModules);
		}

		private void OnAnyFilterChange()
		{
			Action onFiltersApplied = this.OnFiltersApplied;
			if (onFiltersApplied == null)
			{
				return;
			}
			onFiltersApplied();
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string SearchInitialText
		{
			get
			{
				return this._searchInitialText;
			}
			set
			{
				if (value != this._searchInitialText)
				{
					this._searchInitialText = value;
					base.OnPropertyChangedWithValue<string>(value, "SearchInitialText");
				}
			}
		}

		[DataSourceProperty]
		public string SearchText
		{
			get
			{
				return this._searchText;
			}
			set
			{
				if (value != this._searchText)
				{
					this._searchText = value;
					base.OnPropertyChangedWithValue<string>(value, "SearchText");
					this.OnAnyFilterChange();
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPCustomGameFilterItemVM> Items
		{
			get
			{
				return this._items;
			}
			set
			{
				if (value != this._items)
				{
					this._items = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPCustomGameFilterItemVM>>(value, "Items");
				}
			}
		}

		public Action OnFiltersApplied;

		private string _titleText;

		private string _searchInitialText;

		private string _searchText;

		private MBBindingList<MPCustomGameFilterItemVM> _items;

		public enum CustomGameFilterType
		{
			Name,
			NotFull,
			HasPlayers,
			HasPasswordProtection,
			IsOfficial,
			ModuleCompatible
		}
	}
}
