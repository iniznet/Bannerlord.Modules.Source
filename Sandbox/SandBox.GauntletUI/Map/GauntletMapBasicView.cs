using System;
using SandBox.View.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MapBasicView))]
	public class GauntletMapBasicView : MapView
	{
		public GauntletLayer GauntletLayer { get; private set; }

		public GauntletLayer GauntletNameplateLayer { get; private set; }

		protected override void CreateLayout()
		{
			base.CreateLayout();
			this.GauntletLayer = new GauntletLayer(100, "GauntletLayer", false);
			this.GauntletLayer.InputRestrictions.SetInputRestrictions(false, 7);
			this.GauntletLayer.Name = "BasicLayer";
			base.MapScreen.AddLayer(this.GauntletLayer);
			this.GauntletNameplateLayer = new GauntletLayer(90, "GauntletLayer", false);
			this.GauntletNameplateLayer.InputRestrictions.SetInputRestrictions(false, 5);
			base.MapScreen.AddLayer(this.GauntletNameplateLayer);
		}

		protected override void OnMapConversationStart()
		{
			base.OnMapConversationStart();
			this.GauntletLayer._twoDimensionView.SetEnable(false);
			this.GauntletNameplateLayer._twoDimensionView.SetEnable(false);
		}

		protected override void OnMapConversationOver()
		{
			base.OnMapConversationOver();
			this.GauntletLayer._twoDimensionView.SetEnable(true);
			this.GauntletNameplateLayer._twoDimensionView.SetEnable(true);
		}

		protected override void OnFinalize()
		{
			base.MapScreen.RemoveLayer(this.GauntletLayer);
			this.GauntletLayer = null;
			base.OnFinalize();
		}
	}
}
