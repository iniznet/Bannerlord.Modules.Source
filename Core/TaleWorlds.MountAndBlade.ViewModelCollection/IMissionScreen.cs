using System;

namespace TaleWorlds.MountAndBlade.ViewModelCollection
{
	// Token: 0x02000006 RID: 6
	public interface IMissionScreen
	{
		// Token: 0x0600006C RID: 108
		bool GetDisplayDialog();

		// Token: 0x0600006D RID: 109
		float GetCameraElevation();

		// Token: 0x0600006E RID: 110
		void SetOrderFlagVisibility(bool value);

		// Token: 0x0600006F RID: 111
		string GetFollowText();

		// Token: 0x06000070 RID: 112
		string GetFollowPartyText();
	}
}
