using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200023D RID: 573
	internal class OnPlatformRequestedMultiplayerJob : Job
	{
		// Token: 0x06001F5A RID: 8026 RVA: 0x0006F1B4 File Offset: 0x0006D3B4
		public override void DoJob(float dt)
		{
			base.DoJob(dt);
			if (MBGameManager.Current != null)
			{
				MBGameManager.Current.OnPlatformRequestedMultiplayer();
			}
			else if (GameStateManager.Current != null && GameStateManager.Current.ActiveState != null)
			{
				GameStateManager.Current.CleanStates(0);
			}
			base.Finished = true;
		}
	}
}
