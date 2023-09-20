using System;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Missions.NameMarker
{
	public class QuestMarkerVM : ViewModel
	{
		public SandBoxUIHelper.IssueQuestFlags IssueQuestFlag { get; }

		public QuestMarkerVM(SandBoxUIHelper.IssueQuestFlags issueQuestFlag)
		{
			this.QuestMarkerType = (int)issueQuestFlag;
			this.IssueQuestFlag = issueQuestFlag;
		}

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

		private int _questMarkerType;
	}
}
