using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys
{
	public class GameKeyGroupVM : ViewModel
	{
		public GameKeyGroupVM(string categoryId, IEnumerable<GameKey> keys, Action<KeyOptionVM> onKeybindRequest, Action<int, InputKey> setAllKeysOfId)
		{
			this._onKeybindRequest = onKeybindRequest;
			this._setAllKeysOfId = setAllKeysOfId;
			this._categoryId = categoryId;
			this._gameKeys = new MBBindingList<GameKeyOptionVM>();
			this._keys = keys;
			this.PopulateGameKeys();
			this.RefreshValues();
		}

		private void PopulateGameKeys()
		{
			this.GameKeys.Clear();
			foreach (GameKey gameKey in this._keys)
			{
				if (Input.IsGamepadActive ? (((gameKey != null) ? gameKey.DefaultControllerKey : null) != null && (gameKey == null || gameKey.DefaultControllerKey.InputKey != InputKey.Invalid)) : (((gameKey != null) ? gameKey.DefaultKeyboardKey : null) != null && (gameKey == null || gameKey.DefaultKeyboardKey.InputKey != InputKey.Invalid)))
				{
					this.GameKeys.Add(new GameKeyOptionVM(gameKey, this._onKeybindRequest, new Action<GameKeyOptionVM, InputKey>(this.SetGameKey)));
				}
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Description = Module.CurrentModule.GlobalTextManager.FindText("str_key_category_name", this._categoryId).ToString();
			this.GameKeys.ApplyActionOnAllItems(delegate(GameKeyOptionVM x)
			{
				x.RefreshValues();
			});
		}

		private void SetGameKey(GameKeyOptionVM option, InputKey newKey)
		{
			option.CurrentKey.ChangeKey(newKey);
			option.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", option.CurrentKey.ToString().ToLower()).ToString();
			this._setAllKeysOfId(option.CurrentGameKey.Id, newKey);
		}

		internal void Update()
		{
			foreach (GameKeyOptionVM gameKeyOptionVM in this.GameKeys)
			{
				gameKeyOptionVM.Update();
			}
		}

		public void OnDone()
		{
			foreach (GameKeyOptionVM gameKeyOptionVM in this.GameKeys)
			{
				gameKeyOptionVM.OnDone();
			}
		}

		internal bool IsChanged()
		{
			return this.GameKeys.Any((GameKeyOptionVM k) => k.IsChanged());
		}

		public void OnGamepadActiveStateChanged()
		{
			this.Update();
			this.OnDone();
		}

		public void Cancel()
		{
			this.GameKeys.ApplyActionOnAllItems(delegate(GameKeyOptionVM g)
			{
				g.Revert();
			});
		}

		public void ApplyValues()
		{
			this.GameKeys.ApplyActionOnAllItems(delegate(GameKeyOptionVM g)
			{
				g.Apply();
			});
		}

		[DataSourceProperty]
		public MBBindingList<GameKeyOptionVM> GameKeys
		{
			get
			{
				return this._gameKeys;
			}
			set
			{
				if (value != this._gameKeys)
				{
					this._gameKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameKeyOptionVM>>(value, "GameKeys");
				}
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		private readonly Action<KeyOptionVM> _onKeybindRequest;

		private readonly Action<int, InputKey> _setAllKeysOfId;

		private readonly string _categoryId;

		private IEnumerable<GameKey> _keys;

		private string _description;

		private MBBindingList<GameKeyOptionVM> _gameKeys;
	}
}
