using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	// Token: 0x02000064 RID: 100
	public class FaceGeneratorMissionView : MissionView
	{
		// Token: 0x0600043E RID: 1086 RVA: 0x000219BF File Offset: 0x0001FBBF
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (base.Input.IsGameKeyPressed(37) && !GameNetwork.IsSessionActive)
			{
				LoadingWindow.EnableGlobalLoadingWindow();
				ScreenManager.PushScreen(ViewCreator.CreateMBFaceGeneratorScreen(Game.Current.PlayerTroop, false, null));
			}
		}
	}
}
