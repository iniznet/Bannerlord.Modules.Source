using System;
using SandBox.View.Map;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x0200001F RID: 31
	[OverrideView(typeof(MapBarView))]
	public class GauntletMapBarView : MapView
	{
		// Token: 0x06000131 RID: 305 RVA: 0x00009919 File Offset: 0x00007B19
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._gauntletMapBarGlobalLayer = new GauntletMapBarGlobalLayer();
			this._gauntletMapBarGlobalLayer.Initialize(base.MapScreen, 8.5f);
			ScreenManager.AddGlobalLayer(this._gauntletMapBarGlobalLayer, true);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000994E File Offset: 0x00007B4E
		protected override void OnFinalize()
		{
			this._gauntletMapBarGlobalLayer.OnFinalize();
			ScreenManager.RemoveGlobalLayer(this._gauntletMapBarGlobalLayer);
			base.OnFinalize();
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000996C File Offset: 0x00007B6C
		protected override void OnResume()
		{
			base.OnResume();
			this._gauntletMapBarGlobalLayer.Refresh();
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000997F File Offset: 0x00007B7F
		protected override bool IsEscaped()
		{
			return this._gauntletMapBarGlobalLayer.IsEscaped();
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000998C File Offset: 0x00007B8C
		protected override void OnMapConversationStart()
		{
			this._gauntletMapBarGlobalLayer.OnMapConversationStart();
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00009999 File Offset: 0x00007B99
		protected override void OnMapConversationOver()
		{
			this._gauntletMapBarGlobalLayer.OnMapConversationEnd();
		}

		// Token: 0x04000091 RID: 145
		private GauntletMapBarGlobalLayer _gauntletMapBarGlobalLayer;
	}
}
