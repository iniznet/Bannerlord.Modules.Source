using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000336 RID: 822
	public class CaptureTheFlagCapturePoint
	{
		// Token: 0x170007EA RID: 2026
		// (get) Token: 0x06002C2F RID: 11311 RVA: 0x000AB846 File Offset: 0x000A9A46
		// (set) Token: 0x06002C30 RID: 11312 RVA: 0x000AB84E File Offset: 0x000A9A4E
		public float Progress { get; set; }

		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x06002C31 RID: 11313 RVA: 0x000AB857 File Offset: 0x000A9A57
		// (set) Token: 0x06002C32 RID: 11314 RVA: 0x000AB85F File Offset: 0x000A9A5F
		public CaptureTheFlagFlagDirection Direction { get; set; }

		// Token: 0x170007EC RID: 2028
		// (get) Token: 0x06002C33 RID: 11315 RVA: 0x000AB868 File Offset: 0x000A9A68
		// (set) Token: 0x06002C34 RID: 11316 RVA: 0x000AB870 File Offset: 0x000A9A70
		public float Speed { get; set; }

		// Token: 0x170007ED RID: 2029
		// (get) Token: 0x06002C35 RID: 11317 RVA: 0x000AB879 File Offset: 0x000A9A79
		// (set) Token: 0x06002C36 RID: 11318 RVA: 0x000AB881 File Offset: 0x000A9A81
		public MatrixFrame InitialFlagFrame { get; private set; }

		// Token: 0x170007EE RID: 2030
		// (get) Token: 0x06002C37 RID: 11319 RVA: 0x000AB88A File Offset: 0x000A9A8A
		// (set) Token: 0x06002C38 RID: 11320 RVA: 0x000AB892 File Offset: 0x000A9A92
		public GameEntity FlagEntity { get; private set; }

		// Token: 0x170007EF RID: 2031
		// (get) Token: 0x06002C39 RID: 11321 RVA: 0x000AB89B File Offset: 0x000A9A9B
		// (set) Token: 0x06002C3A RID: 11322 RVA: 0x000AB8A3 File Offset: 0x000A9AA3
		public SynchedMissionObject FlagHolder { get; private set; }

		// Token: 0x170007F0 RID: 2032
		// (get) Token: 0x06002C3B RID: 11323 RVA: 0x000AB8AC File Offset: 0x000A9AAC
		// (set) Token: 0x06002C3C RID: 11324 RVA: 0x000AB8B4 File Offset: 0x000A9AB4
		public GameEntity FlagBottomBoundary { get; private set; }

		// Token: 0x170007F1 RID: 2033
		// (get) Token: 0x06002C3D RID: 11325 RVA: 0x000AB8BD File Offset: 0x000A9ABD
		// (set) Token: 0x06002C3E RID: 11326 RVA: 0x000AB8C5 File Offset: 0x000A9AC5
		public GameEntity FlagTopBoundary { get; private set; }

		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x06002C3F RID: 11327 RVA: 0x000AB8CE File Offset: 0x000A9ACE
		public BattleSideEnum BattleSide { get; }

		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x06002C40 RID: 11328 RVA: 0x000AB8D6 File Offset: 0x000A9AD6
		public int Index { get; }

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x06002C41 RID: 11329 RVA: 0x000AB8DE File Offset: 0x000A9ADE
		// (set) Token: 0x06002C42 RID: 11330 RVA: 0x000AB8E6 File Offset: 0x000A9AE6
		public bool UpdateFlag { get; set; }

		// Token: 0x06002C43 RID: 11331 RVA: 0x000AB8F0 File Offset: 0x000A9AF0
		public CaptureTheFlagCapturePoint(GameEntity flagPole, BattleSideEnum battleSide, int index)
		{
			this.Reset();
			this.BattleSide = battleSide;
			this.Index = index;
			this.FlagHolder = flagPole.CollectChildrenEntitiesWithTag("score_stand").SingleOrDefault<GameEntity>().GetFirstScriptOfType<SynchedMissionObject>();
			this.FlagEntity = this.FlagHolder.GameEntity.GetChildren().Single((GameEntity q) => q.HasTag("flag"));
			this.FlagHolder.GameEntity.EntityFlags |= EntityFlags.NoOcclusionCulling;
			this.FlagEntity.EntityFlags |= EntityFlags.NoOcclusionCulling;
			this.FlagBottomBoundary = flagPole.GetChildren().Single((GameEntity q) => q.HasTag("flag_raising_bottom"));
			this.FlagTopBoundary = flagPole.GetChildren().Single((GameEntity q) => q.HasTag("flag_raising_top"));
			MatrixFrame globalFrame = this.FlagHolder.GameEntity.GetGlobalFrame();
			globalFrame.origin.z = this.FlagBottomBoundary.GetGlobalFrame().origin.z;
			this.InitialFlagFrame = globalFrame;
		}

		// Token: 0x06002C44 RID: 11332 RVA: 0x000ABA38 File Offset: 0x000A9C38
		public void Reset()
		{
			this.Progress = 0f;
			this.Direction = CaptureTheFlagFlagDirection.None;
			this.Speed = 0f;
			this.UpdateFlag = false;
		}
	}
}
