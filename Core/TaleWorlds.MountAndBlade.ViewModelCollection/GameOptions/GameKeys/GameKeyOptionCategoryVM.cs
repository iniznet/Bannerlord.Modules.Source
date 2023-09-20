using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.AuxiliaryKeys;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys
{
	public class GameKeyOptionCategoryVM : ViewModel
	{
		public GameKeyOptionCategoryVM(Action<KeyOptionVM> onKeybindRequest, IEnumerable<string> gameKeyCategories)
		{
			this._gameKeyCategories = new Dictionary<string, List<GameKey>>();
			foreach (string text in gameKeyCategories)
			{
				this._gameKeyCategories.Add(text, new List<GameKey>());
			}
			this._onKeybindRequest = onKeybindRequest;
			this.GameKeyGroups = new MBBindingList<GameKeyGroupVM>();
			this._auxiliaryKeyCategories = new Dictionary<string, List<HotKey>>();
			this.AuxiliaryKeyGroups = new MBBindingList<AuxiliaryKeyGroupVM>();
			foreach (GameKeyContext gameKeyContext in HotKeyManager.GetAllCategories())
			{
				if (gameKeyContext.Type == GameKeyContext.GameKeyContextType.AuxiliarySerializedAndShownInOptions)
				{
					this._auxiliaryKeyCategories.Add(gameKeyContext.GameKeyCategoryId, new List<HotKey>());
					using (Dictionary<string, HotKey>.ValueCollection.Enumerator enumerator3 = gameKeyContext.RegisteredHotKeys.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							HotKey hotKey = enumerator3.Current;
							List<HotKey> list;
							if (hotKey != null && this._auxiliaryKeyCategories.TryGetValue(hotKey.GroupId, out list) && !list.Contains(hotKey))
							{
								list.Add(hotKey);
							}
						}
						continue;
					}
				}
				if (gameKeyContext.Type == GameKeyContext.GameKeyContextType.Default)
				{
					foreach (GameKey gameKey in gameKeyContext.RegisteredGameKeys)
					{
						List<GameKey> list2;
						if (gameKey != null && this._gameKeyCategories.TryGetValue(gameKey.MainCategoryId, out list2) && !list2.Contains(gameKey))
						{
							list2.Add(gameKey);
						}
					}
				}
			}
			foreach (KeyValuePair<string, List<GameKey>> keyValuePair in this._gameKeyCategories)
			{
				if (keyValuePair.Value.Count > 0)
				{
					this.GameKeyGroups.Add(new GameKeyGroupVM(keyValuePair.Key, keyValuePair.Value, this._onKeybindRequest, new Action<int, InputKey>(this.UpdateKeysOfGamekeysWithID)));
				}
			}
			foreach (KeyValuePair<string, List<HotKey>> keyValuePair2 in this._auxiliaryKeyCategories)
			{
				if (keyValuePair2.Value.Count > 0)
				{
					this.AuxiliaryKeyGroups.Add(new AuxiliaryKeyGroupVM(keyValuePair2.Key, keyValuePair2.Value, this._onKeybindRequest));
				}
			}
			this.RefreshValues();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
			this.IsEnabled = !Input.IsGamepadActive;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = new TextObject("{=qmNeO8FG}Keybindings", null).ToString();
			this.ResetText = new TextObject("{=RVIKFCno}Reset to Defaults", null).ToString();
			this.GameKeyGroups.ApplyActionOnAllItems(delegate(GameKeyGroupVM x)
			{
				x.RefreshValues();
			});
			this.AuxiliaryKeyGroups.ApplyActionOnAllItems(delegate(AuxiliaryKeyGroupVM x)
			{
				x.RefreshValues();
			});
		}

		private void OnGamepadActiveStateChanged()
		{
			MBBindingList<GameKeyGroupVM> gameKeyGroups = this.GameKeyGroups;
			if (gameKeyGroups != null)
			{
				gameKeyGroups.ApplyActionOnAllItems(delegate(GameKeyGroupVM g)
				{
					g.OnGamepadActiveStateChanged();
				});
			}
			this.AuxiliaryKeyGroups.ApplyActionOnAllItems(delegate(AuxiliaryKeyGroupVM x)
			{
				x.OnGamepadActiveStateChanged();
			});
			this.IsEnabled = !Input.IsGamepadActive;
			Debug.Print("KEYBINDS TAB ENABLED: " + this.IsEnabled.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public bool IsChanged()
		{
			if (!this.GameKeyGroups.Any((GameKeyGroupVM x) => x.IsChanged()))
			{
				return this.AuxiliaryKeyGroups.Any((AuxiliaryKeyGroupVM x) => x.IsChanged());
			}
			return true;
		}

		public void ExecuteResetToDefault()
		{
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=4gCU2ykB}Reset all keys to default", null).ToString(), new TextObject("{=YjbNtFcw}This will reset ALL keys to their default states. You won't be able to undo this action. {newline} {newline}Are you sure?", null).ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), delegate
			{
				this.ResetToDefault();
			}, null, "", 0f, null, null, null), false, false);
		}

		public void OnDone()
		{
			this.GameKeyGroups.ApplyActionOnAllItems(delegate(GameKeyGroupVM x)
			{
				x.OnDone();
			});
			this.AuxiliaryKeyGroups.ApplyActionOnAllItems(delegate(AuxiliaryKeyGroupVM x)
			{
				x.OnDone();
			});
			foreach (KeyValuePair<GameKey, InputKey> keyValuePair in this._keysToChangeOnDone)
			{
				Key key = this.FindValidInputKey(keyValuePair.Key);
				if (key != null)
				{
					key.ChangeKey(keyValuePair.Value);
				}
			}
		}

		private void ResetToDefault()
		{
			HotKeyManager.Reset();
			this.GameKeyGroups.ApplyActionOnAllItems(delegate(GameKeyGroupVM x)
			{
				x.Update();
			});
			this.AuxiliaryKeyGroups.ApplyActionOnAllItems(delegate(AuxiliaryKeyGroupVM x)
			{
				x.Update();
			});
			this._keysToChangeOnDone.Clear();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		private Key FindValidInputKey(GameKey gameKey)
		{
			if (!Input.IsGamepadActive)
			{
				return gameKey.KeyboardKey;
			}
			return gameKey.ControllerKey;
		}

		private void UpdateKeysOfGamekeysWithID(int givenId, InputKey newKey)
		{
			Func<GameKey, bool> <>9__0;
			foreach (GameKeyContext gameKeyContext in HotKeyManager.GetAllCategories())
			{
				if (gameKeyContext.Type == GameKeyContext.GameKeyContextType.Default)
				{
					IEnumerable<GameKey> registeredGameKeys = gameKeyContext.RegisteredGameKeys;
					Func<GameKey, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (GameKey k) => k != null && k.Id == givenId);
					}
					foreach (GameKey gameKey in registeredGameKeys.Where(func))
					{
						if (this._keysToChangeOnDone.ContainsKey(gameKey))
						{
							this._keysToChangeOnDone[gameKey] = newKey;
						}
						else
						{
							this._keysToChangeOnDone.Add(gameKey, newKey);
						}
					}
				}
			}
		}

		public void Cancel()
		{
			this.GameKeyGroups.ApplyActionOnAllItems(delegate(GameKeyGroupVM g)
			{
				g.Cancel();
			});
		}

		public void ApplyValues()
		{
			this.GameKeyGroups.ApplyActionOnAllItems(delegate(GameKeyGroupVM g)
			{
				g.ApplyValues();
			});
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string ResetText
		{
			get
			{
				return this._resetText;
			}
			set
			{
				if (value != this._resetText)
				{
					this._resetText = value;
					base.OnPropertyChangedWithValue<string>(value, "ResetText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<GameKeyGroupVM> GameKeyGroups
		{
			get
			{
				return this._gameKeyGroups;
			}
			set
			{
				if (value != this._gameKeyGroups)
				{
					this._gameKeyGroups = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameKeyGroupVM>>(value, "GameKeyGroups");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<AuxiliaryKeyGroupVM> AuxiliaryKeyGroups
		{
			get
			{
				return this._auxiliaryKeyGroups;
			}
			set
			{
				if (value != this._auxiliaryKeyGroups)
				{
					this._auxiliaryKeyGroups = value;
					base.OnPropertyChangedWithValue<MBBindingList<AuxiliaryKeyGroupVM>>(value, "AuxiliaryKeyGroups");
				}
			}
		}

		private readonly Action<KeyOptionVM> _onKeybindRequest;

		private Dictionary<string, List<GameKey>> _gameKeyCategories;

		private Dictionary<string, List<HotKey>> _auxiliaryKeyCategories;

		private Dictionary<GameKey, InputKey> _keysToChangeOnDone = new Dictionary<GameKey, InputKey>();

		private string _name;

		private string _resetText;

		private bool _isEnabled;

		private MBBindingList<GameKeyGroupVM> _gameKeyGroups;

		private MBBindingList<AuxiliaryKeyGroupVM> _auxiliaryKeyGroups;
	}
}
