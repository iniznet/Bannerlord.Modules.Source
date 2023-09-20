using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.AuxiliaryKeys;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys
{
	// Token: 0x02000101 RID: 257
	public class GameKeyOptionCategoryVM : ViewModel
	{
		// Token: 0x060016BA RID: 5818 RVA: 0x0004998C File Offset: 0x00047B8C
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

		// Token: 0x060016BB RID: 5819 RVA: 0x00049CC8 File Offset: 0x00047EC8
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

		// Token: 0x060016BC RID: 5820 RVA: 0x00049D5C File Offset: 0x00047F5C
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

		// Token: 0x060016BD RID: 5821 RVA: 0x00049DFC File Offset: 0x00047FFC
		public bool IsChanged()
		{
			if (!this.GameKeyGroups.Any((GameKeyGroupVM x) => x.IsChanged()))
			{
				return this.AuxiliaryKeyGroups.Any((AuxiliaryKeyGroupVM x) => x.IsChanged());
			}
			return true;
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x00049E64 File Offset: 0x00048064
		public void ExecuteResetToDefault()
		{
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=4gCU2ykB}Reset all keys to default", null).ToString(), new TextObject("{=YjbNtFcw}This will reset ALL keys to their default states. You won't be able to undo this action. {newline} {newline}Are you sure?", null).ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), delegate
			{
				this.ResetToDefault();
			}, null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x00049EDC File Offset: 0x000480DC
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

		// Token: 0x060016C0 RID: 5824 RVA: 0x00049F9C File Offset: 0x0004819C
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

		// Token: 0x060016C1 RID: 5825 RVA: 0x0004A00D File Offset: 0x0004820D
		public override void OnFinalize()
		{
			base.OnFinalize();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.OnGamepadActiveStateChanged));
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x0004A035 File Offset: 0x00048235
		private Key FindValidInputKey(GameKey gameKey)
		{
			if (!Input.IsGamepadActive)
			{
				return gameKey.KeyboardKey;
			}
			return gameKey.ControllerKey;
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x0004A04C File Offset: 0x0004824C
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

		// Token: 0x060016C4 RID: 5828 RVA: 0x0004A144 File Offset: 0x00048344
		public void Cancel()
		{
			this.GameKeyGroups.ApplyActionOnAllItems(delegate(GameKeyGroupVM g)
			{
				g.Cancel();
			});
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x0004A170 File Offset: 0x00048370
		public void ApplyValues()
		{
			this.GameKeyGroups.ApplyActionOnAllItems(delegate(GameKeyGroupVM g)
			{
				g.ApplyValues();
			});
		}

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x060016C6 RID: 5830 RVA: 0x0004A19C File Offset: 0x0004839C
		// (set) Token: 0x060016C7 RID: 5831 RVA: 0x0004A1A4 File Offset: 0x000483A4
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

		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x060016C8 RID: 5832 RVA: 0x0004A1C7 File Offset: 0x000483C7
		// (set) Token: 0x060016C9 RID: 5833 RVA: 0x0004A1CF File Offset: 0x000483CF
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

		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x060016CA RID: 5834 RVA: 0x0004A1ED File Offset: 0x000483ED
		// (set) Token: 0x060016CB RID: 5835 RVA: 0x0004A1F5 File Offset: 0x000483F5
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

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x060016CC RID: 5836 RVA: 0x0004A218 File Offset: 0x00048418
		// (set) Token: 0x060016CD RID: 5837 RVA: 0x0004A220 File Offset: 0x00048420
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

		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x060016CE RID: 5838 RVA: 0x0004A23E File Offset: 0x0004843E
		// (set) Token: 0x060016CF RID: 5839 RVA: 0x0004A246 File Offset: 0x00048446
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

		// Token: 0x04000ACA RID: 2762
		private readonly Action<KeyOptionVM> _onKeybindRequest;

		// Token: 0x04000ACB RID: 2763
		private Dictionary<string, List<GameKey>> _gameKeyCategories;

		// Token: 0x04000ACC RID: 2764
		private Dictionary<string, List<HotKey>> _auxiliaryKeyCategories;

		// Token: 0x04000ACD RID: 2765
		private Dictionary<GameKey, InputKey> _keysToChangeOnDone = new Dictionary<GameKey, InputKey>();

		// Token: 0x04000ACE RID: 2766
		private string _name;

		// Token: 0x04000ACF RID: 2767
		private string _resetText;

		// Token: 0x04000AD0 RID: 2768
		private bool _isEnabled;

		// Token: 0x04000AD1 RID: 2769
		private MBBindingList<GameKeyGroupVM> _gameKeyGroups;

		// Token: 0x04000AD2 RID: 2770
		private MBBindingList<AuxiliaryKeyGroupVM> _auxiliaryKeyGroups;
	}
}
