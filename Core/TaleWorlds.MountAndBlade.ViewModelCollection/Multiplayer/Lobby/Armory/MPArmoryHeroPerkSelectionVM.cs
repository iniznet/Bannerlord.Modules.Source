using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory
{
	// Token: 0x020000A7 RID: 167
	public class MPArmoryHeroPerkSelectionVM : ViewModel
	{
		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06001019 RID: 4121 RVA: 0x00035740 File Offset: 0x00033940
		// (set) Token: 0x0600101A RID: 4122 RVA: 0x00035748 File Offset: 0x00033948
		public MultiplayerClassDivisions.MPHeroClass CurrentHeroClass { get; private set; }

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x0600101B RID: 4123 RVA: 0x00035751 File Offset: 0x00033951
		// (set) Token: 0x0600101C RID: 4124 RVA: 0x00035759 File Offset: 0x00033959
		public List<IReadOnlyPerkObject> CurrentSelectedPerks { get; private set; }

		// Token: 0x0600101D RID: 4125 RVA: 0x00035764 File Offset: 0x00033964
		public MPArmoryHeroPerkSelectionVM(Action<HeroPerkVM, MPPerkVM> onPerkSelection, Action forceRefreshCharacter)
		{
			this._onPerkSelection = onPerkSelection;
			this._forceRefreshCharacter = forceRefreshCharacter;
			this.Perks = new MBBindingList<HeroPerkVM>();
			this.GameModes = new SelectorVM<SelectorItemVM>(0, new Action<SelectorVM<SelectorItemVM>>(this.OnGameModeSelectionChanged));
			foreach (string text in this._availableGameModes)
			{
				this.GameModes.AddItem(new SelectorItemVM(GameTexts.FindText("str_multiplayer_official_game_type_name", text)));
			}
			this.GameModes.SelectedIndex = 0;
			this.RefreshValues();
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x00035858 File Offset: 0x00033A58
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.GameModes.RefreshValues();
			this.GameModes.SelectedIndex = 0;
			this.Perks.ApplyActionOnAllItems(delegate(HeroPerkVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x000358AC File Offset: 0x00033AAC
		public void RefreshPerksListWithHero(MultiplayerClassDivisions.MPHeroClass heroClass)
		{
			this.Perks = new MBBindingList<HeroPerkVM>();
			this.CurrentHeroClass = heroClass;
			MBBindingList<HeroPerkVM> mbbindingList = new MBBindingList<HeroPerkVM>();
			List<HeroPerkVM> list = new List<HeroPerkVM>();
			List<List<IReadOnlyPerkObject>> allPerksForHeroClass = MultiplayerClassDivisions.GetAllPerksForHeroClass(this.CurrentHeroClass, this._availableGameModes[this.GameModes.SelectedIndex]);
			for (int i = 0; i < allPerksForHeroClass.Count; i++)
			{
				if (allPerksForHeroClass[i].Count > 0)
				{
					IReadOnlyPerkObject readOnlyPerkObject = allPerksForHeroClass[i][0];
					HeroPerkVM heroPerkVM = new HeroPerkVM(new Action<HeroPerkVM, MPPerkVM>(this.OnPerkSelection), readOnlyPerkObject, allPerksForHeroClass[i], i);
					mbbindingList.Add(heroPerkVM);
					list.Add(heroPerkVM);
				}
			}
			this.Perks = mbbindingList;
			if (this.CurrentSelectedPerks == null)
			{
				this.CurrentSelectedPerks = new List<IReadOnlyPerkObject>();
			}
			else
			{
				this.CurrentSelectedPerks.Clear();
			}
			foreach (HeroPerkVM heroPerkVM2 in list)
			{
				this.OnPerkSelection(heroPerkVM2, heroPerkVM2.SelectedPerkItem);
			}
		}

		// Token: 0x06001020 RID: 4128 RVA: 0x000359C8 File Offset: 0x00033BC8
		private void OnGameModeSelectionChanged(SelectorVM<SelectorItemVM> selector)
		{
			if (this.GameModes.SelectedIndex == -1)
			{
				this.GameModes.SelectedIndex = 0;
			}
			if (this.CurrentHeroClass != null)
			{
				this.RefreshPerksListWithHero(this.CurrentHeroClass);
				Action forceRefreshCharacter = this._forceRefreshCharacter;
				if (forceRefreshCharacter == null)
				{
					return;
				}
				forceRefreshCharacter();
			}
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x00035A08 File Offset: 0x00033C08
		private void OnPerkSelection(HeroPerkVM heroPerk, MPPerkVM candidate)
		{
			this.CurrentSelectedPerks = this.Perks.Select((HeroPerkVM x) => x.SelectedPerk).ToList<IReadOnlyPerkObject>();
			Action<HeroPerkVM, MPPerkVM> onPerkSelection = this._onPerkSelection;
			if (onPerkSelection == null)
			{
				return;
			}
			onPerkSelection(heroPerk, candidate);
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06001022 RID: 4130 RVA: 0x00035A5C File Offset: 0x00033C5C
		// (set) Token: 0x06001023 RID: 4131 RVA: 0x00035A64 File Offset: 0x00033C64
		[DataSourceProperty]
		public MBBindingList<HeroPerkVM> Perks
		{
			get
			{
				return this._perks;
			}
			set
			{
				if (value != this._perks)
				{
					this._perks = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroPerkVM>>(value, "Perks");
				}
			}
		}

		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06001024 RID: 4132 RVA: 0x00035A82 File Offset: 0x00033C82
		// (set) Token: 0x06001025 RID: 4133 RVA: 0x00035A8A File Offset: 0x00033C8A
		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> GameModes
		{
			get
			{
				return this._gameModes;
			}
			set
			{
				if (value != this._gameModes)
				{
					this._gameModes = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "GameModes");
				}
			}
		}

		// Token: 0x040007A0 RID: 1952
		private readonly Action<HeroPerkVM, MPPerkVM> _onPerkSelection;

		// Token: 0x040007A1 RID: 1953
		private readonly Action _forceRefreshCharacter;

		// Token: 0x040007A2 RID: 1954
		private List<string> _availableGameModes = new List<string> { "Skirmish", "Captain", "Siege", "TeamDeathmatch", "Duel" };

		// Token: 0x040007A3 RID: 1955
		private MBBindingList<HeroPerkVM> _perks;

		// Token: 0x040007A4 RID: 1956
		private SelectorVM<SelectorItemVM> _gameModes;
	}
}
