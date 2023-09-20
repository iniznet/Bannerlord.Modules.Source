using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission
{
	// Token: 0x02000028 RID: 40
	public abstract class MissionGauntletEscapeMenuBase : MissionEscapeMenuView
	{
		// Token: 0x060001CF RID: 463 RVA: 0x00009EA6 File Offset: 0x000080A6
		protected MissionGauntletEscapeMenuBase(string viewFile)
		{
			base.OnMissionScreenInitialize();
			this._viewFile = viewFile;
			this.ViewOrderPriority = 50;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00009EC3 File Offset: 0x000080C3
		protected virtual List<EscapeMenuItemVM> GetEscapeMenuItems()
		{
			return null;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00009EC6 File Offset: 0x000080C6
		public override void OnMissionScreenFinalize()
		{
			this.DataSource.OnFinalize();
			this.DataSource = null;
			this._gauntletLayer = null;
			this._movie = null;
			base.OnMissionScreenFinalize();
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00009EEE File Offset: 0x000080EE
		public override bool OnEscape()
		{
			if (!this._isRenderingStarted)
			{
				return false;
			}
			if (!base.IsActive)
			{
				this.DataSource.RefreshItems(this.GetEscapeMenuItems());
			}
			return this.OnEscapeMenuToggled(!base.IsActive);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00009F24 File Offset: 0x00008124
		protected bool OnEscapeMenuToggled(bool isOpened)
		{
			if (base.IsActive == isOpened)
			{
				return false;
			}
			base.IsActive = isOpened;
			if (isOpened)
			{
				this.DataSource.RefreshValues();
				if (!GameNetwork.IsMultiplayer)
				{
					MBCommon.PauseGameEngine();
					Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
				}
				this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
				this._gauntletLayer.IsFocusLayer = true;
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
				this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
				this._movie = this._gauntletLayer.LoadMovie(this._viewFile, this.DataSource);
				base.MissionScreen.AddLayer(this._gauntletLayer);
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
			else
			{
				if (!GameNetwork.IsMultiplayer)
				{
					MBCommon.UnPauseGameEngine();
					Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
				}
				this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				base.MissionScreen.RemoveLayer(this._gauntletLayer);
				this._movie = null;
				this._gauntletLayer = null;
			}
			return true;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000A048 File Offset: 0x00008248
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.IsActive && (this._gauntletLayer.Input.IsHotKeyReleased("ToggleEscapeMenu") || this._gauntletLayer.Input.IsHotKeyReleased("Exit")))
			{
				this.OnEscapeMenuToggled(false);
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000A09A File Offset: 0x0000829A
		public override void OnSceneRenderingStarted()
		{
			base.OnSceneRenderingStarted();
			this._isRenderingStarted = true;
		}

		// Token: 0x040000D6 RID: 214
		protected EscapeMenuVM DataSource;

		// Token: 0x040000D7 RID: 215
		private GauntletLayer _gauntletLayer;

		// Token: 0x040000D8 RID: 216
		private IGauntletMovie _movie;

		// Token: 0x040000D9 RID: 217
		private string _viewFile;

		// Token: 0x040000DA RID: 218
		private bool _isRenderingStarted;
	}
}
