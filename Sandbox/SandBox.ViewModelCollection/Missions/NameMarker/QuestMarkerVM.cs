using System;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Missions.NameMarker
{
	// Token: 0x02000028 RID: 40
	public class QuestMarkerVM : ViewModel
	{
		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000327 RID: 807 RVA: 0x0000F8E3 File Offset: 0x0000DAE3
		public SandBoxUIHelper.IssueQuestFlags IssueQuestFlag { get; }

		// Token: 0x06000328 RID: 808 RVA: 0x0000F8EB File Offset: 0x0000DAEB
		public QuestMarkerVM(SandBoxUIHelper.IssueQuestFlags issueQuestFlag)
		{
			this.QuestMarkerType = (int)issueQuestFlag;
			this.IssueQuestFlag = issueQuestFlag;
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000329 RID: 809 RVA: 0x0000F901 File Offset: 0x0000DB01
		// (set) Token: 0x0600032A RID: 810 RVA: 0x0000F909 File Offset: 0x0000DB09
		[DataSourceProperty]
		public int QuestMarkerType
		{
			get
			{
				return this._questMarkerType;
			}
			set
			{
				if (value != this._questMarkerType)
				{
					this._questMarkerType = value;
					base.OnPropertyChangedWithValue(value, "QuestMarkerType");
				}
			}
		}

		// Token: 0x040001A2 RID: 418
		private int _questMarkerType;
	}
}
