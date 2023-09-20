using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.AuxiliaryKeys
{
	public class AuxiliaryKeyGroupVM : ViewModel
	{
		public AuxiliaryKeyGroupVM(string categoryId, IEnumerable<HotKey> keys, Action<KeyOptionVM> onKeybindRequest)
		{
			this._onKeybindRequest = onKeybindRequest;
			this._categoryId = categoryId;
			this._hotKeys = new MBBindingList<AuxiliaryKeyOptionVM>();
			this._keys = keys;
			this.PopulateHotKeys();
			this.RefreshValues();
		}

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

		private void SetHotKey(AuxiliaryKeyOptionVM option, InputKey newKey)
		{
			option.CurrentKey.ChangeKey(newKey);
			option.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", option.CurrentKey.ToString().ToLower()).ToString();
		}

		internal void Update()
		{
			foreach (AuxiliaryKeyOptionVM auxiliaryKeyOptionVM in this.HotKeys)
			{
				auxiliaryKeyOptionVM.Update();
			}
		}

		public void OnDone()
		{
			foreach (AuxiliaryKeyOptionVM auxiliaryKeyOptionVM in this.HotKeys)
			{
				auxiliaryKeyOptionVM.OnDone();
			}
		}

		internal bool IsChanged()
		{
			return this.HotKeys.Any((AuxiliaryKeyOptionVM k) => k.IsChanged());
		}

		public void OnGamepadActiveStateChanged()
		{
			this.Update();
			this.OnDone();
		}

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

		private readonly string _categoryId;

		private IEnumerable<HotKey> _keys;

		private string _description;

		private MBBindingList<AuxiliaryKeyOptionVM> _hotKeys;
	}
}
