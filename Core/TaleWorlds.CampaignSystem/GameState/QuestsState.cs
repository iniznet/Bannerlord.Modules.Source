using System;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x02000344 RID: 836
	public class QuestsState : GameState
	{
		// Token: 0x17000B18 RID: 2840
		// (get) Token: 0x06002EB6 RID: 11958 RVA: 0x000C04DD File Offset: 0x000BE6DD
		// (set) Token: 0x06002EB7 RID: 11959 RVA: 0x000C04E5 File Offset: 0x000BE6E5
		public IssueBase InitialSelectedIssue { get; private set; }

		// Token: 0x17000B19 RID: 2841
		// (get) Token: 0x06002EB8 RID: 11960 RVA: 0x000C04EE File Offset: 0x000BE6EE
		// (set) Token: 0x06002EB9 RID: 11961 RVA: 0x000C04F6 File Offset: 0x000BE6F6
		public QuestBase InitialSelectedQuest { get; private set; }

		// Token: 0x17000B1A RID: 2842
		// (get) Token: 0x06002EBA RID: 11962 RVA: 0x000C04FF File Offset: 0x000BE6FF
		// (set) Token: 0x06002EBB RID: 11963 RVA: 0x000C0507 File Offset: 0x000BE707
		public JournalLogEntry InitialSelectedLog { get; private set; }

		// Token: 0x17000B1B RID: 2843
		// (get) Token: 0x06002EBC RID: 11964 RVA: 0x000C0510 File Offset: 0x000BE710
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B1C RID: 2844
		// (get) Token: 0x06002EBD RID: 11965 RVA: 0x000C0513 File Offset: 0x000BE713
		// (set) Token: 0x06002EBE RID: 11966 RVA: 0x000C051B File Offset: 0x000BE71B
		public IQuestsStateHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		// Token: 0x06002EBF RID: 11967 RVA: 0x000C0524 File Offset: 0x000BE724
		public QuestsState()
		{
		}

		// Token: 0x06002EC0 RID: 11968 RVA: 0x000C052C File Offset: 0x000BE72C
		public QuestsState(IssueBase initialSelectedIssue)
		{
			this.InitialSelectedIssue = initialSelectedIssue;
		}

		// Token: 0x06002EC1 RID: 11969 RVA: 0x000C053B File Offset: 0x000BE73B
		public QuestsState(QuestBase initialSelectedQuest)
		{
			this.InitialSelectedQuest = initialSelectedQuest;
		}

		// Token: 0x06002EC2 RID: 11970 RVA: 0x000C054A File Offset: 0x000BE74A
		public QuestsState(JournalLogEntry initialSelectedLog)
		{
			this.InitialSelectedLog = initialSelectedLog;
		}

		// Token: 0x04000DFE RID: 3582
		private IQuestsStateHandler _handler;
	}
}
