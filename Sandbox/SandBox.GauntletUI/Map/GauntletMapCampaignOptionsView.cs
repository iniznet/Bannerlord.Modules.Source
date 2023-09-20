using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000024 RID: 36
	[OverrideView(typeof(MapCampaignOptionsView))]
	public class GauntletMapCampaignOptionsView : MapView
	{
		// Token: 0x06000153 RID: 339 RVA: 0x0000A7B4 File Offset: 0x000089B4
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new CampaignOptionsVM(new Action(this.OnClose));
			base.Layer = new GauntletLayer(4401, "GauntletLayer", false)
			{
				IsFocusLayer = true
			};
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._layerAsGauntletLayer.LoadMovie("CampaignOptions", this._dataSource);
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			base.MapScreen.AddLayer(base.Layer);
			base.MapScreen.PauseAmbientSounds();
			ScreenManager.TrySetFocus(base.Layer);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000A876 File Offset: 0x00008A76
		private void OnClose()
		{
			MapScreen.Instance.CloseCampaignOptions();
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000A882 File Offset: 0x00008A82
		protected override void OnIdleTick(float dt)
		{
			base.OnFrameTick(dt);
			if (base.Layer.Input.IsHotKeyReleased("Exit"))
			{
				this._dataSource.ExecuteDone();
			}
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000A8B0 File Offset: 0x00008AB0
		protected override void OnFinalize()
		{
			base.OnFinalize();
			base.Layer.InputRestrictions.ResetInputRestrictions();
			base.MapScreen.RemoveLayer(base.Layer);
			base.MapScreen.RestartAmbientSounds();
			ScreenManager.TryLoseFocus(base.Layer);
			base.Layer = null;
			this._dataSource = null;
			this._layerAsGauntletLayer = null;
		}

		// Token: 0x040000AF RID: 175
		private CampaignOptionsVM _dataSource;

		// Token: 0x040000B0 RID: 176
		private GauntletLayer _layerAsGauntletLayer;
	}
}
