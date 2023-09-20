using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame
{
	// Token: 0x0200008B RID: 139
	public class MPCustomGameFiltersVM : ViewModel
	{
		// Token: 0x06000CBB RID: 3259 RVA: 0x0002CAB4 File Offset: 0x0002ACB4
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

		// Token: 0x06000CBC RID: 3260 RVA: 0x0002CC1C File Offset: 0x0002AE1C
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

		// Token: 0x06000CBD RID: 3261 RVA: 0x0002CC88 File Offset: 0x0002AE88
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

		// Token: 0x06000CBE RID: 3262 RVA: 0x0002CD1F File Offset: 0x0002AF1F
		private bool FilterByCompatibleModules(GameServerEntry serverEntry)
		{
			return NetworkMain.GameClient.LoadedUnofficialModules.IsCompatibleWith(serverEntry.LoadedModules, serverEntry.AllowsOptionalModules);
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x0002CD3C File Offset: 0x0002AF3C
		private void OnAnyFilterChange()
		{
			Action onFiltersApplied = this.OnFiltersApplied;
			if (onFiltersApplied == null)
			{
				return;
			}
			onFiltersApplied();
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06000CC0 RID: 3264 RVA: 0x0002CD4E File Offset: 0x0002AF4E
		// (set) Token: 0x06000CC1 RID: 3265 RVA: 0x0002CD56 File Offset: 0x0002AF56
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

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06000CC2 RID: 3266 RVA: 0x0002CD79 File Offset: 0x0002AF79
		// (set) Token: 0x06000CC3 RID: 3267 RVA: 0x0002CD81 File Offset: 0x0002AF81
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

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06000CC4 RID: 3268 RVA: 0x0002CDA4 File Offset: 0x0002AFA4
		// (set) Token: 0x06000CC5 RID: 3269 RVA: 0x0002CDAC File Offset: 0x0002AFAC
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

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06000CC6 RID: 3270 RVA: 0x0002CDD5 File Offset: 0x0002AFD5
		// (set) Token: 0x06000CC7 RID: 3271 RVA: 0x0002CDDD File Offset: 0x0002AFDD
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

		// Token: 0x04000618 RID: 1560
		public Action OnFiltersApplied;

		// Token: 0x04000619 RID: 1561
		private string _titleText;

		// Token: 0x0400061A RID: 1562
		private string _searchInitialText;

		// Token: 0x0400061B RID: 1563
		private string _searchText;

		// Token: 0x0400061C RID: 1564
		private MBBindingList<MPCustomGameFilterItemVM> _items;

		// Token: 0x020001CE RID: 462
		public enum CustomGameFilterType
		{
			// Token: 0x04000DCE RID: 3534
			Name,
			// Token: 0x04000DCF RID: 3535
			NotFull,
			// Token: 0x04000DD0 RID: 3536
			HasPlayers,
			// Token: 0x04000DD1 RID: 3537
			HasPasswordProtection,
			// Token: 0x04000DD2 RID: 3538
			IsOfficial,
			// Token: 0x04000DD3 RID: 3539
			ModuleCompatible
		}
	}
}
