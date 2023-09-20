using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x0200002D RID: 45
	[OverrideView(typeof(MapNotificationView))]
	public class GauntletMapNotificationView : MapNotificationView
	{
		// Token: 0x0600019F RID: 415 RVA: 0x0000BDA8 File Offset: 0x00009FA8
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._mapNavigationHandler = new MapNavigationHandler();
			this._dataSource = new MapNotificationVM(this._mapNavigationHandler, new Action<Vec2>(base.MapScreen.FastMoveCameraToPosition));
			this._dataSource.ReceiveNewNotification += this.OnReceiveNewNotification;
			this._dataSource.SetRemoveInputKey(HotKeyManager.GetCategory("MapNotificationHotKeyCategory").GetHotKey("RemoveNotification"));
			base.Layer = new GauntletLayer(100, "GauntletLayer", false);
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("MapNotificationHotKeyCategory"));
			base.Layer.InputRestrictions.SetInputRestrictions(false, 5);
			this._movie = this._layerAsGauntletLayer.LoadMovie("MapNotificationUI", this._dataSource);
			base.MapScreen.AddLayer(base.Layer);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000BE9B File Offset: 0x0000A09B
		private void OnReceiveNewNotification(MapNotificationItemBaseVM newNotification)
		{
			if (!string.IsNullOrEmpty(newNotification.SoundId))
			{
				SoundEvent.PlaySound2D(newNotification.SoundId);
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000BEB6 File Offset: 0x0000A0B6
		public override void RegisterMapNotificationType(Type data, Type item)
		{
			this._dataSource.RegisterMapNotificationType(data, item);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000BEC5 File Offset: 0x0000A0C5
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._dataSource.OnFinalize();
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000BED8 File Offset: 0x0000A0D8
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._dataSource.OnFrameTick(dt);
			this.HandleInput();
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000BEF3 File Offset: 0x0000A0F3
		protected override void OnMenuModeTick(float dt)
		{
			base.OnMenuModeTick(dt);
			this._dataSource.OnMenuModeTick(dt);
			this.HandleInput();
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000BF10 File Offset: 0x0000A110
		private void HandleInput()
		{
			if (!this._isHoveringOnNotification && this._dataSource.FocusedNotificationItem != null)
			{
				this._isHoveringOnNotification = true;
				base.Layer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(base.Layer);
			}
			else if (this._isHoveringOnNotification && this._dataSource.FocusedNotificationItem == null)
			{
				this._isHoveringOnNotification = false;
				base.Layer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(base.Layer);
			}
			if (this._isHoveringOnNotification && this._dataSource.FocusedNotificationItem != null && base.Layer.Input.IsHotKeyReleased("RemoveNotification"))
			{
				this._dataSource.FocusedNotificationItem.ExecuteRemove();
			}
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000BFC1 File Offset: 0x0000A1C1
		public override void ResetNotifications()
		{
			base.ResetNotifications();
			MapNotificationVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.RemoveAllNotifications();
		}

		// Token: 0x040000D6 RID: 214
		private MapNotificationVM _dataSource;

		// Token: 0x040000D7 RID: 215
		private IGauntletMovie _movie;

		// Token: 0x040000D8 RID: 216
		private MapNavigationHandler _mapNavigationHandler;

		// Token: 0x040000D9 RID: 217
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x040000DA RID: 218
		private bool _isHoveringOnNotification;
	}
}
