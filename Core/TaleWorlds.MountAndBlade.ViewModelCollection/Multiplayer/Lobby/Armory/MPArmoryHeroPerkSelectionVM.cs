using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Armory
{
	public class MPArmoryHeroPerkSelectionVM : ViewModel
	{
		public MultiplayerClassDivisions.MPHeroClass CurrentHeroClass { get; private set; }

		public List<IReadOnlyPerkObject> CurrentSelectedPerks { get; private set; }

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

		private readonly Action<HeroPerkVM, MPPerkVM> _onPerkSelection;

		private readonly Action _forceRefreshCharacter;

		private List<string> _availableGameModes = new List<string> { "Skirmish", "Captain", "Siege", "TeamDeathmatch", "Duel" };

		private MBBindingList<HeroPerkVM> _perks;

		private SelectorVM<SelectorItemVM> _gameModes;
	}
}
