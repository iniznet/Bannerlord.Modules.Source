using System;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate
{
	public class SettlementNameplateEventItemVM : ViewModel
	{
		public SettlementNameplateEventItemVM(SettlementNameplateEventItemVM.SettlementEventType eventType)
		{
			this.EventType = eventType;
			this.Type = (int)eventType;
			this.AdditionalParameters = "";
		}

		public SettlementNameplateEventItemVM(string productionIconId = "")
		{
			this.EventType = SettlementNameplateEventItemVM.SettlementEventType.Production;
			this.Type = (int)this.EventType;
			this.AdditionalParameters = productionIconId;
		}

		[DataSourceProperty]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue(value, "Type");
				}
			}
		}

		[DataSourceProperty]
		public string AdditionalParameters
		{
			get
			{
				return this._additionalParameters;
			}
			set
			{
				if (value != this._additionalParameters)
				{
					this._additionalParameters = value;
					base.OnPropertyChangedWithValue<string>(value, "AdditionalParameters");
				}
			}
		}

		public readonly SettlementNameplateEventItemVM.SettlementEventType EventType;

		private int _type;

		private string _additionalParameters;

		public enum SettlementEventType
		{
			Tournament,
			AvailableIssue,
			ActiveQuest,
			ActiveStoryQuest,
			TrackedIssue,
			TrackedStoryQuest,
			Production
		}
	}
}
