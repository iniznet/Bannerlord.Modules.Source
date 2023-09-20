using System;
using System.Collections.Generic;
using SandBox.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection.Map.Cheat
{
	public class GameplayCheatsVM : ViewModel
	{
		public GameplayCheatsVM(Action onClose, IEnumerable<GameplayCheatBase> cheats)
		{
			this._onClose = onClose;
			this._initialCheatList = cheats;
			this.Cheats = new MBBindingList<CheatItemBaseVM>();
			this._activeCheatGroups = new List<CheatGroupItemVM>();
			this._mainTitleText = new TextObject("{=OYtysXzk}Cheats", null);
			this.FillWithCheats(cheats);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			for (int i = 0; i < this.Cheats.Count; i++)
			{
				this.Cheats[i].RefreshValues();
			}
			if (this._activeCheatGroups.Count > 0)
			{
				TextObject textObject = new TextObject("{=1tiF5JhE}{TITLE} > {SUBTITLE}", null);
				for (int j = 0; j < this._activeCheatGroups.Count; j++)
				{
					if (j == 0)
					{
						textObject.SetTextVariable("TITLE", this._mainTitleText.ToString());
					}
					else
					{
						textObject.SetTextVariable("TITLE", textObject.ToString());
					}
					textObject.SetTextVariable("SUBTITLE", this._activeCheatGroups[j].Name.ToString());
				}
				this.Title = textObject.ToString();
				this.ButtonCloseLabel = GameTexts.FindText("str_back", null).ToString();
				return;
			}
			this.Title = this._mainTitleText.ToString();
			this.ButtonCloseLabel = GameTexts.FindText("str_close", null).ToString();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM closeInputKey = this.CloseInputKey;
			if (closeInputKey == null)
			{
				return;
			}
			closeInputKey.OnFinalize();
		}

		private void FillWithCheats(IEnumerable<GameplayCheatBase> cheats)
		{
			this.Cheats.Clear();
			foreach (GameplayCheatBase gameplayCheatBase in cheats)
			{
				GameplayCheatItem gameplayCheatItem;
				GameplayCheatGroup gameplayCheatGroup;
				if ((gameplayCheatItem = gameplayCheatBase as GameplayCheatItem) != null)
				{
					this.Cheats.Add(new CheatActionItemVM(gameplayCheatItem, new Action<CheatActionItemVM>(this.OnCheatActionExecuted)));
				}
				else if ((gameplayCheatGroup = gameplayCheatBase as GameplayCheatGroup) != null)
				{
					this.Cheats.Add(new CheatGroupItemVM(gameplayCheatGroup, new Action<CheatGroupItemVM>(this.OnCheatGroupSelected)));
				}
			}
			this.RefreshValues();
		}

		private void OnCheatActionExecuted(CheatActionItemVM cheatItem)
		{
			this._activeCheatGroups.Clear();
			this.FillWithCheats(this._initialCheatList);
			TextObject textObject = new TextObject("{=1QAEyN2V}Cheat Used: {CHEAT}", null);
			textObject.SetTextVariable("CHEAT", cheatItem.Name.ToString());
			InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
		}

		private void OnCheatGroupSelected(CheatGroupItemVM cheatGroup)
		{
			this._activeCheatGroups.Add(cheatGroup);
			IEnumerable<GameplayCheatBase> enumerable;
			if (cheatGroup == null)
			{
				enumerable = null;
			}
			else
			{
				GameplayCheatGroup cheatGroup2 = cheatGroup.CheatGroup;
				enumerable = ((cheatGroup2 != null) ? cheatGroup2.GetCheats() : null);
			}
			this.FillWithCheats(enumerable ?? this._initialCheatList);
		}

		public void ExecuteClose()
		{
			if (this._activeCheatGroups.Count > 0)
			{
				this._activeCheatGroups.RemoveAt(this._activeCheatGroups.Count - 1);
				if (this._activeCheatGroups.Count > 0)
				{
					this.FillWithCheats(this._activeCheatGroups[this._activeCheatGroups.Count - 1].CheatGroup.GetCheats());
					return;
				}
				this.FillWithCheats(this._initialCheatList);
				return;
			}
			else
			{
				Action onClose = this._onClose;
				if (onClose == null)
				{
					return;
				}
				onClose();
				return;
			}
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		[DataSourceProperty]
		public string ButtonCloseLabel
		{
			get
			{
				return this._buttonCloseLabel;
			}
			set
			{
				if (value != this._buttonCloseLabel)
				{
					this._buttonCloseLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "ButtonCloseLabel");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CheatItemBaseVM> Cheats
		{
			get
			{
				return this._cheats;
			}
			set
			{
				if (value != this._cheats)
				{
					this._cheats = value;
					base.OnPropertyChangedWithValue<MBBindingList<CheatItemBaseVM>>(value, "Cheats");
				}
			}
		}

		public void SetCloseInputKey(HotKey hotKey)
		{
			this.CloseInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		[DataSourceProperty]
		public InputKeyItemVM CloseInputKey
		{
			get
			{
				return this._closeInputKey;
			}
			set
			{
				if (value != this._closeInputKey)
				{
					this._closeInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CloseInputKey");
				}
			}
		}

		private readonly Action _onClose;

		private readonly IEnumerable<GameplayCheatBase> _initialCheatList;

		private readonly TextObject _mainTitleText;

		private List<CheatGroupItemVM> _activeCheatGroups;

		private string _title;

		private string _buttonCloseLabel;

		private MBBindingList<CheatItemBaseVM> _cheats;

		private InputKeyItemVM _closeInputKey;
	}
}
