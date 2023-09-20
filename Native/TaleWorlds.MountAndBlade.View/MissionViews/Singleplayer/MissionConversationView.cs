using System;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	// Token: 0x02000068 RID: 104
	public class MissionConversationView : MissionView
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x0002207C File Offset: 0x0002027C
		public static MissionConversationView Current
		{
			get
			{
				return Mission.Current.GetMissionBehavior<MissionConversationView>();
			}
		}
	}
}
