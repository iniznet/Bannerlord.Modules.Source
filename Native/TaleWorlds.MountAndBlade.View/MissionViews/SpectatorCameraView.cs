using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	// Token: 0x02000055 RID: 85
	public class SpectatorCameraView : MissionView
	{
		// Token: 0x060003B7 RID: 951 RVA: 0x0001F886 File Offset: 0x0001DA86
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("MultiplayerHotkeyCategory"));
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0001F8B0 File Offset: 0x0001DAB0
		public override void AfterStart()
		{
			for (int i = 0; i < 10; i++)
			{
				this._spectateCamerFrames.Add(MatrixFrame.Identity);
			}
			for (int j = 0; j < 10; j++)
			{
				string text = "spectate_cam_" + j.ToString();
				List<GameEntity> list = Mission.Current.Scene.FindEntitiesWithTag(text).ToList<GameEntity>();
				if (list.Count > 0)
				{
					this._spectateCamerFrames[j] = list[0].GetGlobalFrame();
				}
			}
		}

		// Token: 0x0400026A RID: 618
		private List<MatrixFrame> _spectateCamerFrames = new List<MatrixFrame>();
	}
}
