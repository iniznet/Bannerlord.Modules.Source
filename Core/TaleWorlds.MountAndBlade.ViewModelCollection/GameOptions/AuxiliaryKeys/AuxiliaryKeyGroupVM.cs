using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.AuxiliaryKeys
{
	// Token: 0x02000103 RID: 259
	public class AuxiliaryKeyGroupVM : ViewModel
	{
		// Token: 0x060016DD RID: 5853 RVA: 0x0004A52B File Offset: 0x0004872B
		public AuxiliaryKeyGroupVM(string categoryId, IEnumerable<HotKey> keys, Action<KeyOptionVM> onKeybindRequest)
		{
			this._onKeybindRequest = onKeybindRequest;
			this._categoryId = categoryId;
			this._hotKeys = new MBBindingList<AuxiliaryKeyOptionVM>();
			this._keys = keys;
			this.PopulateHotKeys();
			this.RefreshValues();
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x0004A560 File Offset: 0x00048760
		private void PopulateHotKeys()
		{
			this.HotKeys.Clear();
			foreach (HotKey hotKey in this._keys)
			{
				bool flag;
				if (!Input.IsGamepadActive)
				{
					if (hotKey == null)
					{
						flag = false;
					}
					else
					{
						flag = hotKey.DefaultKeys.Any((Key x) => x != null && x.IsKeyboardInput && x.InputKey != InputKey.Invalid);
					}
				}
				else if (hotKey == null)
				{
					flag = false;
				}
				else
				{
					flag = hotKey.DefaultKeys.Any((Key x) => x != null && x.IsControllerInput && x.InputKey != InputKey.Invalid);
				}
				if (flag)
				{
					this.HotKeys.Add(new AuxiliaryKeyOptionVM(hotKey, this._onKeybindRequest, new Action<AuxiliaryKeyOptionVM, InputKey>(this.SetHotKey)));
				}
			}
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x0004A648 File Offset: 0x00048848
		public override void RefreshValues()
		{
			base.RefreshValues();
			string text = this._categoryId;
			TextObject textObject;
			if (Module.CurrentModule.GlobalTextManager.TryGetText("str_hotkey_category_name", this._categoryId, out textObject))
			{
				text = textObject.ToString();
			}
			this.Description = text;
			this.HotKeys.ApplyActionOnAllItems(delegate(AuxiliaryKeyOptionVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x0004A6B8 File Offset: 0x000488B8
		private void SetHotKey(AuxiliaryKeyOptionVM option, InputKey newKey)
		{
			option.CurrentKey.ChangeKey(newKey);
			option.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", option.CurrentKey.ToString().ToLower()).ToString();
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x0004A6F8 File Offset: 0x000488F8
		internal void Update()
		{
			foreach (AuxiliaryKeyOptionVM auxiliaryKeyOptionVM in this.HotKeys)
			{
				auxiliaryKeyOptionVM.Update();
			}
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x0004A744 File Offset: 0x00048944
		public void OnDone()
		{
			foreach (AuxiliaryKeyOptionVM auxiliaryKeyOptionVM in this.HotKeys)
			{
				auxiliaryKeyOptionVM.OnDone();
			}
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x0004A790 File Offset: 0x00048990
		internal bool IsChanged()
		{
			return this.HotKeys.Any((AuxiliaryKeyOptionVM k) => k.IsChanged());
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x0004A7BC File Offset: 0x000489BC
		public void OnGamepadActiveStateChanged()
		{
			this.Update();
			this.OnDone();
		}

		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x060016E5 RID: 5861 RVA: 0x0004A7CA File Offset: 0x000489CA
		// (set) Token: 0x060016E6 RID: 5862 RVA: 0x0004A7D2 File Offset: 0x000489D2
		[DataSourceProperty]
		public MBBindingList<AuxiliaryKeyOptionVM> HotKeys
		{
			get
			{
				return this._hotKeys;
			}
			set
			{
				if (value != this._hotKeys)
				{
					this._hotKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<AuxiliaryKeyOptionVM>>(value, "HotKeys");
				}
			}
		}

		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x060016E7 RID: 5863 RVA: 0x0004A7F0 File Offset: 0x000489F0
		// (set) Token: 0x060016E8 RID: 5864 RVA: 0x0004A7F8 File Offset: 0x000489F8
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

		// Token: 0x04000AD6 RID: 2774
		private readonly Action<KeyOptionVM> _onKeybindRequest;

		// Token: 0x04000AD7 RID: 2775
		private readonly string _categoryId;

		// Token: 0x04000AD8 RID: 2776
		private IEnumerable<HotKey> _keys;

		// Token: 0x04000AD9 RID: 2777
		private string _description;

		// Token: 0x04000ADA RID: 2778
		private MBBindingList<AuxiliaryKeyOptionVM> _hotKeys;
	}
}
