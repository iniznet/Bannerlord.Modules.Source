using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MarriageOfferPopup;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MarriageOfferPopupView))]
	public class GauntletMarriageOfferPopupView : MapView
	{
		public GauntletMarriageOfferPopupView(Hero suitor, Hero maiden)
		{
			this._suitor = suitor;
			this._maiden = maiden;
		}

		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new MarriageOfferPopupVM(this._suitor, this._maiden);
			this.InitializeKeyVisuals();
			base.Layer = new GauntletLayer(201, "GauntletLayer", false);
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
			this._movie = this._layerAsGauntletLayer.LoadMovie("MarriageOfferPopup", this._dataSource);
			base.MapScreen.AddLayer(base.Layer);
			base.MapScreen.SetIsMarriageOfferPopupActive(true);
			Campaign.Current.TimeControlMode = 0;
			Campaign.Current.SetTimeControlModeLock(true);
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this.HandleInput();
			MarriageOfferPopupVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.Update();
		}

		protected override void OnMenuModeTick(float dt)
		{
			base.OnMenuModeTick(dt);
			this.HandleInput();
			MarriageOfferPopupVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.Update();
		}

		protected override void OnIdleTick(float dt)
		{
			base.OnIdleTick(dt);
			this.HandleInput();
			MarriageOfferPopupVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.Update();
		}

		protected override void OnFinalize()
		{
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			base.MapScreen.RemoveLayer(base.Layer);
			this._movie = null;
			this._dataSource = null;
			base.Layer = null;
			this._layerAsGauntletLayer = null;
			base.MapScreen.SetIsMarriageOfferPopupActive(false);
			Campaign.Current.SetTimeControlModeLock(false);
			base.OnFinalize();
		}

		protected override bool IsEscaped()
		{
			MarriageOfferPopupVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.ExecuteDeclineOffer();
			}
			return true;
		}

		protected override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return false;
		}

		private void HandleInput()
		{
			if (this._dataSource != null)
			{
				if (base.Layer.Input.IsGameKeyPressed(39))
				{
					base.MapScreen.OpenEncyclopedia();
					return;
				}
				if (base.Layer.Input.IsHotKeyReleased("Confirm"))
				{
					UISoundsHelper.PlayUISound("event:/ui/panels/next");
					this._dataSource.ExecuteAcceptOffer();
					return;
				}
				if (base.Layer.Input.IsHotKeyReleased("Exit"))
				{
					UISoundsHelper.PlayUISound("event:/ui/panels/next");
					this._dataSource.ExecuteDeclineOffer();
				}
			}
		}

		private void InitializeKeyVisuals()
		{
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
		}

		private GauntletLayer _layerAsGauntletLayer;

		private MarriageOfferPopupVM _dataSource;

		private IGauntletMovie _movie;

		private Hero _suitor;

		private Hero _maiden;
	}
}
