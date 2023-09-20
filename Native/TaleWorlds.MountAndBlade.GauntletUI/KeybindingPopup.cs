using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class KeybindingPopup
	{
		public bool IsActive { get; private set; }

		public KeybindingPopup(Action<Key> onDone, ScreenBase targetScreen)
		{
			this._onDone = onDone;
			this._messageStr = new BindingListStringItem(new TextObject("{=hvaDkG4w}Press any key.", null).ToString());
			this._targetScreen = targetScreen;
			this._gauntletLayer = new GauntletLayer(4005, "GauntletLayer", false);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
		}

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

		private bool _isActiveFirstFrame;

		private GauntletLayer _gauntletLayer;

		private IGauntletMovie _movie;

		private BindingListStringItem _messageStr;

		private ScreenBase _targetScreen;

		private Action<Key> _onDone;
	}
}
