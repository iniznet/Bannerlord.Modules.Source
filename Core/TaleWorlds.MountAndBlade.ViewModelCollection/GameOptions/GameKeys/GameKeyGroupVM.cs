using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys
{
	// Token: 0x02000100 RID: 256
	public class GameKeyGroupVM : ViewModel
	{
		// Token: 0x060016AC RID: 5804 RVA: 0x0004962C File Offset: 0x0004782C
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

		// Token: 0x060016AD RID: 5805 RVA: 0x00049668 File Offset: 0x00047868
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

		// Token: 0x060016AE RID: 5806 RVA: 0x0004974C File Offset: 0x0004794C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Description = Module.CurrentModule.GlobalTextManager.FindText("str_key_category_name", this._categoryId).ToString();
			this.GameKeys.ApplyActionOnAllItems(delegate(GameKeyOptionVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x000497B0 File Offset: 0x000479B0
		private void SetGameKey(GameKeyOptionVM option, InputKey newKey)
		{
			option.CurrentKey.ChangeKey(newKey);
			option.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", option.CurrentKey.ToString().ToLower()).ToString();
			this._setAllKeysOfId(option.CurrentGameKey.Id, newKey);
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x00049810 File Offset: 0x00047A10
		internal void Update()
		{
			foreach (GameKeyOptionVM gameKeyOptionVM in this.GameKeys)
			{
				gameKeyOptionVM.Update();
			}
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x0004985C File Offset: 0x00047A5C
		public void OnDone()
		{
			foreach (GameKeyOptionVM gameKeyOptionVM in this.GameKeys)
			{
				gameKeyOptionVM.OnDone();
			}
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x000498A8 File Offset: 0x00047AA8
		internal bool IsChanged()
		{
			return this.GameKeys.Any((GameKeyOptionVM k) => k.IsChanged());
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x000498D4 File Offset: 0x00047AD4
		public void OnGamepadActiveStateChanged()
		{
			this.Update();
			this.OnDone();
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x000498E2 File Offset: 0x00047AE2
		public void Cancel()
		{
			this.GameKeys.ApplyActionOnAllItems(delegate(GameKeyOptionVM g)
			{
				g.Revert();
			});
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x0004990E File Offset: 0x00047B0E
		public void ApplyValues()
		{
			this.GameKeys.ApplyActionOnAllItems(delegate(GameKeyOptionVM g)
			{
				g.Apply();
			});
		}

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x060016B6 RID: 5814 RVA: 0x0004993A File Offset: 0x00047B3A
		// (set) Token: 0x060016B7 RID: 5815 RVA: 0x00049942 File Offset: 0x00047B42
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

		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x060016B8 RID: 5816 RVA: 0x00049960 File Offset: 0x00047B60
		// (set) Token: 0x060016B9 RID: 5817 RVA: 0x00049968 File Offset: 0x00047B68
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

		// Token: 0x04000AC4 RID: 2756
		private readonly Action<KeyOptionVM> _onKeybindRequest;

		// Token: 0x04000AC5 RID: 2757
		private readonly Action<int, InputKey> _setAllKeysOfId;

		// Token: 0x04000AC6 RID: 2758
		private readonly string _categoryId;

		// Token: 0x04000AC7 RID: 2759
		private IEnumerable<GameKey> _keys;

		// Token: 0x04000AC8 RID: 2760
		private string _description;

		// Token: 0x04000AC9 RID: 2761
		private MBBindingList<GameKeyOptionVM> _gameKeys;
	}
}
