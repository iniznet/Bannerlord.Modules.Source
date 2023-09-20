using System;
using SandBox.View.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000022 RID: 34
	[OverrideView(typeof(MapBasicView))]
	public class GauntletMapBasicView : MapView
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000146 RID: 326 RVA: 0x0000A404 File Offset: 0x00008604
		// (set) Token: 0x06000147 RID: 327 RVA: 0x0000A40C File Offset: 0x0000860C
		public GauntletLayer GauntletLayer { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000148 RID: 328 RVA: 0x0000A415 File Offset: 0x00008615
		// (set) Token: 0x06000149 RID: 329 RVA: 0x0000A41D File Offset: 0x0000861D
		public GauntletLayer GauntletNameplateLayer { get; private set; }

		// Token: 0x0600014A RID: 330 RVA: 0x0000A428 File Offset: 0x00008628
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

		// Token: 0x0600014B RID: 331 RVA: 0x0000A4B7 File Offset: 0x000086B7
		protected override void OnMapConversationStart()
		{
			base.OnMapConversationStart();
			this.GauntletLayer._twoDimensionView.SetEnable(false);
			this.GauntletNameplateLayer._twoDimensionView.SetEnable(false);
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000A4E1 File Offset: 0x000086E1
		protected override void OnMapConversationOver()
		{
			base.OnMapConversationOver();
			this.GauntletLayer._twoDimensionView.SetEnable(true);
			this.GauntletNameplateLayer._twoDimensionView.SetEnable(true);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000A50B File Offset: 0x0000870B
		protected override void OnFinalize()
		{
			base.MapScreen.RemoveLayer(this.GauntletLayer);
			this.GauntletLayer = null;
			base.OnFinalize();
		}
	}
}
