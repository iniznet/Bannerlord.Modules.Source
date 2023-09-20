using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.AuxiliaryKeys
{
	public class AuxiliaryKeyOptionVM : KeyOptionVM
	{
		public HotKey CurrentHotKey { get; private set; }

		public AuxiliaryKeyOptionVM(HotKey hotKey, Action<KeyOptionVM> onKeybindRequest, Action<AuxiliaryKeyOptionVM, InputKey> onKeySet)
			: base(hotKey.GroupId, hotKey.Id, onKeybindRequest)
		{
			this._onKeySet = onKeySet;
			this.CurrentHotKey = hotKey;
			Key key;
			if (!Input.IsGamepadActive)
			{
				key = this.CurrentHotKey.Keys.FirstOrDefault((Key x) => !x.IsControllerInput);
			}
			else
			{
				key = this.CurrentHotKey.Keys.FirstOrDefault((Key x) => x.IsControllerInput);
			}
			base.Key = key;
			if (base.Key == null)
			{
				base.Key = new Key(InputKey.Invalid);
			}
			base.CurrentKey = new Key(base.Key.InputKey);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			string text = this.CurrentHotKey.Id;
			TextObject textObject;
			if (Module.CurrentModule.GlobalTextManager.TryGetText("str_hotkey_name", this._groupId + "_" + this._id, out textObject))
			{
				text = textObject.ToString();
			}
			base.Name = text;
			string text2 = "";
			TextObject textObject2;
			if (Module.CurrentModule.GlobalTextManager.TryGetText("str_hotkey_description", this._groupId + "_" + this._id, out textObject2))
			{
				text2 = textObject2.ToString();
			}
			GameTextManager globalTextManager = Module.CurrentModule.GlobalTextManager;
			base.OptionValueText = globalTextManager.FindText("str_game_key_text", base.CurrentKey.ToString().ToLower()).ToString();
			string text3 = base.OptionValueText;
			foreach (HotKey.Modifiers modifiers in new List<HotKey.Modifiers>
			{
				HotKey.Modifiers.Alt,
				HotKey.Modifiers.Shift,
				HotKey.Modifiers.Control
			})
			{
				if (this.CurrentHotKey.HasModifier(modifiers))
				{
					MBTextManager.SetTextVariable("KEY", text3, false);
					MBTextManager.SetTextVariable("MODIFIER", globalTextManager.FindText("str_game_key_text", "any" + modifiers.ToString().ToLower()).ToString(), false);
					text3 = globalTextManager.FindText("str_hot_key_with_modifier", null).ToString();
				}
			}
			TextObject textObject3 = new TextObject("{=ol0rBSrb}{STR1}{newline}{STR2}", null);
			textObject3.SetTextVariable("STR1", text3);
			textObject3.SetTextVariable("STR2", text2);
			textObject3.SetTextVariable("newline", "\n \n");
			base.Description = textObject3.ToString();
		}

		private void ExecuteKeybindRequest()
		{
			this._onKeybindRequest(this);
		}

		public override void Set(InputKey newKey)
		{
			this._onKeySet(this, newKey);
			this.RefreshValues();
		}

		public override void Update()
		{
			Key key;
			if (!Input.IsGamepadActive)
			{
				key = this.CurrentHotKey.Keys.FirstOrDefault((Key x) => !x.IsControllerInput);
			}
			else
			{
				key = this.CurrentHotKey.Keys.FirstOrDefault((Key x) => x.IsControllerInput);
			}
			base.Key = key;
			if (base.Key == null)
			{
				base.Key = new Key(InputKey.Invalid);
			}
			base.CurrentKey = new Key(base.Key.InputKey);
			this.RefreshValues();
		}

		public override void OnDone()
		{
			base.Key.ChangeKey(base.CurrentKey.InputKey);
		}

		internal override bool IsChanged()
		{
			return base.CurrentKey != base.Key;
		}

		private readonly Action<AuxiliaryKeyOptionVM, InputKey> _onKeySet;
	}
}
