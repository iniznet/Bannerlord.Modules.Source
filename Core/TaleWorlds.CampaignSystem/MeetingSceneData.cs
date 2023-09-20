using System;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000083 RID: 131
	public struct MeetingSceneData
	{
		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06001045 RID: 4165 RVA: 0x00048F98 File Offset: 0x00047198
		// (set) Token: 0x06001046 RID: 4166 RVA: 0x00048FA0 File Offset: 0x000471A0
		public string SceneID { get; private set; }

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06001047 RID: 4167 RVA: 0x00048FA9 File Offset: 0x000471A9
		// (set) Token: 0x06001048 RID: 4168 RVA: 0x00048FB1 File Offset: 0x000471B1
		public string CultureString { get; private set; }

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06001049 RID: 4169 RVA: 0x00048FBA File Offset: 0x000471BA
		public CultureObject Culture
		{
			get
			{
				return MBObjectManager.Instance.GetObject<CultureObject>(this.CultureString);
			}
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x00048FCC File Offset: 0x000471CC
		public MeetingSceneData(string sceneID, string cultureString)
		{
			this.SceneID = sceneID;
			this.CultureString = cultureString;
		}
	}
}
