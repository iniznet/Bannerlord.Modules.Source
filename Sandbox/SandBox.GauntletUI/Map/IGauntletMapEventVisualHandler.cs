using System;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000029 RID: 41
	public interface IGauntletMapEventVisualHandler
	{
		// Token: 0x06000180 RID: 384
		void OnNewEventStarted(GauntletMapEventVisual newEvent);

		// Token: 0x06000181 RID: 385
		void OnInitialized(GauntletMapEventVisual newEvent);

		// Token: 0x06000182 RID: 386
		void OnEventEnded(GauntletMapEventVisual newEvent);

		// Token: 0x06000183 RID: 387
		void OnEventVisibilityChanged(GauntletMapEventVisual visibilityChangedEvent);
	}
}
