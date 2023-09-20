using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.AuxiliaryKeys
{
	// Token: 0x02000104 RID: 260
	public class AuxiliaryKeyOptionVM : KeyOptionVM
	{
		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x060016E9 RID: 5865 RVA: 0x0004A81B File Offset: 0x00048A1B
		// (set) Token: 0x060016EA RID: 5866 RVA: 0x0004A823 File Offset: 0x00048A23
		public HotKey CurrentHotKey { get; private set; }

		// Token: 0x060016EB RID: 5867 RVA: 0x0004A82C File Offset: 0x00048A2C
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
			base.CurrentKey = new Key(base.Key.InputKey);
			this.RefreshValues();
		}

		// Token: 0x060016EC RID: 5868 RVA: 0x0004A8E4 File Offset: 0x00048AE4
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

		// Token: 0x060016ED RID: 5869 RVA: 0x0004AABC File Offset: 0x00048CBC
		private void ExecuteKeybindRequest()
		{
			this._onKeybindRequest(this);
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x0004AACA File Offset: 0x00048CCA
		public override void Set(InputKey newKey)
		{
			this._onKeySet(this, newKey);
			this.RefreshValues();
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x0004AAE0 File Offset: 0x00048CE0
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
			base.CurrentKey = new Key(base.Key.InputKey);
			this.RefreshValues();
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x0004AB76 File Offset: 0x00048D76
		public override void OnDone()
		{
			base.Key.ChangeKey(base.CurrentKey.InputKey);
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x0004AB8E File Offset: 0x00048D8E
		internal override bool IsChanged()
		{
			return base.CurrentKey != base.Key;
		}

		// Token: 0x060016F2 RID: 5874 RVA: 0x0004ABA4 File Offset: 0x00048DA4
		internal override void OnGamepadActiveStateChanged(GamepadActiveStateChangedEvent obj)
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
			base.CurrentKey = new Key(base.Key.InputKey);
			this.RefreshValues();
		}

		// Token: 0x04000ADC RID: 2780
		private readonly Action<AuxiliaryKeyOptionVM, InputKey> _onKeySet;
	}
}
