using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000170 RID: 368
	public struct TacticalDecision
	{
		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x0600130E RID: 4878 RVA: 0x0004A087 File Offset: 0x00048287
		// (set) Token: 0x0600130F RID: 4879 RVA: 0x0004A08F File Offset: 0x0004828F
		public TacticComponent DecidingComponent { get; private set; }

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06001310 RID: 4880 RVA: 0x0004A098 File Offset: 0x00048298
		// (set) Token: 0x06001311 RID: 4881 RVA: 0x0004A0A0 File Offset: 0x000482A0
		public byte DecisionCode { get; private set; }

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06001312 RID: 4882 RVA: 0x0004A0A9 File Offset: 0x000482A9
		// (set) Token: 0x06001313 RID: 4883 RVA: 0x0004A0B1 File Offset: 0x000482B1
		public Formation SubjectFormation { get; private set; }

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06001314 RID: 4884 RVA: 0x0004A0BA File Offset: 0x000482BA
		// (set) Token: 0x06001315 RID: 4885 RVA: 0x0004A0C2 File Offset: 0x000482C2
		public Formation TargetFormation { get; private set; }

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06001316 RID: 4886 RVA: 0x0004A0CB File Offset: 0x000482CB
		// (set) Token: 0x06001317 RID: 4887 RVA: 0x0004A0D3 File Offset: 0x000482D3
		public WorldPosition? TargetPosition { get; private set; }

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06001318 RID: 4888 RVA: 0x0004A0DC File Offset: 0x000482DC
		// (set) Token: 0x06001319 RID: 4889 RVA: 0x0004A0E4 File Offset: 0x000482E4
		public MissionObject TargetObject { get; private set; }

		// Token: 0x0600131A RID: 4890 RVA: 0x0004A0ED File Offset: 0x000482ED
		public TacticalDecision(TacticComponent decidingComponent, byte decisionCode, Formation subjectFormation = null, Formation targetFormation = null, WorldPosition? targetPosition = null, MissionObject targetObject = null)
		{
			this.DecidingComponent = decidingComponent;
			this.DecisionCode = decisionCode;
			this.SubjectFormation = subjectFormation;
			this.TargetFormation = targetFormation;
			this.TargetPosition = targetPosition;
			this.TargetObject = targetObject;
		}
	}
}
