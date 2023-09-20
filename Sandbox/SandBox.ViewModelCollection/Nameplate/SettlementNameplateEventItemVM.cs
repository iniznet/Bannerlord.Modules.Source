using System;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate
{
	// Token: 0x02000017 RID: 23
	public class SettlementNameplateEventItemVM : ViewModel
	{
		// Token: 0x0600023F RID: 575 RVA: 0x0000B729 File Offset: 0x00009929
		public SettlementNameplateEventItemVM(SettlementNameplateEventItemVM.SettlementEventType eventType)
		{
			this.EventType = eventType;
			this.Type = (int)eventType;
			this.AdditionalParameters = "";
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000B74A File Offset: 0x0000994A
		public SettlementNameplateEventItemVM(string productionIconId = "")
		{
			this.EventType = SettlementNameplateEventItemVM.SettlementEventType.Production;
			this.Type = (int)this.EventType;
			this.AdditionalParameters = productionIconId;
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000241 RID: 577 RVA: 0x0000B76C File Offset: 0x0000996C
		// (set) Token: 0x06000242 RID: 578 RVA: 0x0000B774 File Offset: 0x00009974
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

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000243 RID: 579 RVA: 0x0000B792 File Offset: 0x00009992
		// (set) Token: 0x06000244 RID: 580 RVA: 0x0000B79A File Offset: 0x0000999A
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

		// Token: 0x04000112 RID: 274
		public readonly SettlementNameplateEventItemVM.SettlementEventType EventType;

		// Token: 0x04000113 RID: 275
		private int _type;

		// Token: 0x04000114 RID: 276
		private string _additionalParameters;

		// Token: 0x02000066 RID: 102
		public enum SettlementEventType
		{
			// Token: 0x040002D0 RID: 720
			Tournament,
			// Token: 0x040002D1 RID: 721
			AvailableIssue,
			// Token: 0x040002D2 RID: 722
			ActiveQuest,
			// Token: 0x040002D3 RID: 723
			ActiveStoryQuest,
			// Token: 0x040002D4 RID: 724
			TrackedIssue,
			// Token: 0x040002D5 RID: 725
			TrackedStoryQuest,
			// Token: 0x040002D6 RID: 726
			Production
		}
	}
}
