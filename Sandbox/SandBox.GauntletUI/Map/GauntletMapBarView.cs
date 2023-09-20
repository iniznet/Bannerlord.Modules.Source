using System;
using SandBox.View.Map;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MapBarView))]
	public class GauntletMapBarView : MapView
	{
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._gauntletMapBarGlobalLayer = new GauntletMapBarGlobalLayer();
			this._gauntletMapBarGlobalLayer.Initialize(base.MapScreen, 8.5f);
			ScreenManager.AddGlobalLayer(this._gauntletMapBarGlobalLayer, true);
		}

		protected override void OnFinalize()
		{
			this._gauntletMapBarGlobalLayer.OnFinalize();
			ScreenManager.RemoveGlobalLayer(this._gauntletMapBarGlobalLayer);
			base.OnFinalize();
		}

		protected override void OnResume()
		{
			base.OnResume();
			this._gauntletMapBarGlobalLayer.Refresh();
		}

		protected override bool IsEscaped()
		{
			return this._gauntletMapBarGlobalLayer.IsEscaped();
		}

		protected override void OnMapConversationStart()
		{
			this._gauntletMapBarGlobalLayer.OnMapConversationStart();
		}

		protected override void OnMapConversationOver()
		{
			this._gauntletMapBarGlobalLayer.OnMapConversationEnd();
		}

		private GauntletMapBarGlobalLayer _gauntletMapBarGlobalLayer;
	}
}
