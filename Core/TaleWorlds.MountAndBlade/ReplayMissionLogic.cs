using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200027B RID: 635
	public class ReplayMissionLogic : MissionLogic
	{
		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x060021CE RID: 8654 RVA: 0x0007B62B File Offset: 0x0007982B
		// (set) Token: 0x060021CF RID: 8655 RVA: 0x0007B633 File Offset: 0x00079833
		public string FileName { get; private set; }

		// Token: 0x060021D0 RID: 8656 RVA: 0x0007B63C File Offset: 0x0007983C
		public ReplayMissionLogic(bool isMultiplayer, string fileName = "")
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				this.FileName = fileName;
			}
			this._isMultiplayer = isMultiplayer;
		}

		// Token: 0x060021D1 RID: 8657 RVA: 0x0007B65A File Offset: 0x0007985A
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			if (this._isMultiplayer)
			{
				GameNetwork.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
			}
			MBCommon.CurrentGameType = MBCommon.GameType.SingleReplay;
			GameNetwork.InitializeClientSide(null, 0, -1, -1);
			base.Mission.Recorder.RestoreRecordFromFile(this.FileName);
		}

		// Token: 0x060021D2 RID: 8658 RVA: 0x0007B695 File Offset: 0x00079895
		public override void OnRemoveBehavior()
		{
			if (this._isMultiplayer)
			{
				GameNetwork.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
				GameNetwork.EndReplay();
			}
			GameNetwork.TerminateClientSide();
			base.Mission.Recorder.ClearRecordBuffers();
			base.OnRemoveBehavior();
		}

		// Token: 0x04000CA0 RID: 3232
		private bool _isMultiplayer;
	}
}
