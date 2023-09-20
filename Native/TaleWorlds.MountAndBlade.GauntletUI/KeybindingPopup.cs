using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000011 RID: 17
	public class KeybindingPopup
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00004BC7 File Offset: 0x00002DC7
		// (set) Token: 0x0600007A RID: 122 RVA: 0x00004BCF File Offset: 0x00002DCF
		public bool IsActive { get; private set; }

		// Token: 0x0600007B RID: 123 RVA: 0x00004BD8 File Offset: 0x00002DD8
		public KeybindingPopup(Action<Key> onDone, ScreenBase targetScreen)
		{
			this._onDone = onDone;
			this._messageStr = new BindingListStringItem(new TextObject("{=hvaDkG4w}Press any key.", null).ToString());
			this._targetScreen = targetScreen;
			this._gauntletLayer = new GauntletLayer(4005, "GauntletLayer", false);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004C3C File Offset: 0x00002E3C
		public void Tick()
		{
			if (!this.IsActive)
			{
				return;
			}
			if (!this._isActiveFirstFrame)
			{
				InputKey firstKeyReleasedInRange = Input.GetFirstKeyReleasedInRange(0);
				if (firstKeyReleasedInRange != -1)
				{
					this._onDone(new Key(firstKeyReleasedInRange));
					return;
				}
			}
			else
			{
				this._isActiveFirstFrame = false;
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004C80 File Offset: 0x00002E80
		public void OnToggle(bool isActive)
		{
			if (this.IsActive != isActive)
			{
				this.IsActive = isActive;
				if (this.IsActive)
				{
					ScreenManager.TrySetFocus(this._gauntletLayer);
					this._gauntletLayer.IsFocusLayer = true;
					this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
					ScreenManager.SetSuspendLayer(this._gauntletLayer, false);
					this._movie = this._gauntletLayer.LoadMovie("KeybindingPopup", this._messageStr);
					this._targetScreen.AddLayer(this._gauntletLayer);
					this._isActiveFirstFrame = true;
					return;
				}
				ScreenManager.TryLoseFocus(this._gauntletLayer);
				this._gauntletLayer.IsFocusLayer = false;
				this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
				if (this._movie != null)
				{
					this._gauntletLayer.ReleaseMovie(this._movie);
					this._movie = null;
				}
				this._targetScreen.RemoveLayer(this._gauntletLayer);
			}
		}

		// Token: 0x0400004E RID: 78
		private bool _isActiveFirstFrame;

		// Token: 0x0400004F RID: 79
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000050 RID: 80
		private IGauntletMovie _movie;

		// Token: 0x04000051 RID: 81
		private BindingListStringItem _messageStr;

		// Token: 0x04000052 RID: 82
		private ScreenBase _targetScreen;

		// Token: 0x04000053 RID: 83
		private Action<Key> _onDone;
	}
}
