using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	public class SpectatorCameraView : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("MultiplayerHotkeyCategory"));
		}

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

		private List<MatrixFrame> _spectateCamerFrames = new List<MatrixFrame>();
	}
}
