using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000033 RID: 51
	public class MBCampaignEvent
	{
		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000358 RID: 856 RVA: 0x00019492 File Offset: 0x00017692
		// (set) Token: 0x06000359 RID: 857 RVA: 0x0001949A File Offset: 0x0001769A
		public CampaignTime TriggerPeriod { get; private set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600035A RID: 858 RVA: 0x000194A3 File Offset: 0x000176A3
		// (set) Token: 0x0600035B RID: 859 RVA: 0x000194AB File Offset: 0x000176AB
		public CampaignTime InitialWait { get; private set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600035C RID: 860 RVA: 0x000194B4 File Offset: 0x000176B4
		// (set) Token: 0x0600035D RID: 861 RVA: 0x000194BC File Offset: 0x000176BC
		public bool isEventDeleted { get; set; }

		// Token: 0x0600035E RID: 862 RVA: 0x000194C5 File Offset: 0x000176C5
		public MBCampaignEvent(string eventName)
		{
			this.description = eventName;
		}

		// Token: 0x0600035F RID: 863 RVA: 0x000194DF File Offset: 0x000176DF
		public MBCampaignEvent(CampaignTime triggerPeriod, CampaignTime initialWait)
		{
			this.TriggerPeriod = triggerPeriod;
			this.InitialWait = initialWait;
			this.NextTriggerTime = CampaignTime.Now + this.InitialWait;
			this.isEventDeleted = false;
		}

		// Token: 0x06000360 RID: 864 RVA: 0x0001951D File Offset: 0x0001771D
		public void AddHandler(MBCampaignEvent.CampaignEventDelegate gameEventDelegate)
		{
			this.handlers.Add(gameEventDelegate);
		}

		// Token: 0x06000361 RID: 865 RVA: 0x0001952C File Offset: 0x0001772C
		public void RunHandlers(params object[] delegateParams)
		{
			for (int i = 0; i < this.handlers.Count; i++)
			{
				this.handlers[i](this, delegateParams);
			}
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00019564 File Offset: 0x00017764
		public void Unregister(object instance)
		{
			for (int i = 0; i < this.handlers.Count; i++)
			{
				if (this.handlers[i].Target == instance)
				{
					this.handlers.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06000363 RID: 867 RVA: 0x000195AC File Offset: 0x000177AC
		public void CheckUpdate()
		{
			while (this.NextTriggerTime.IsPast && !this.isEventDeleted)
			{
				this.RunHandlers(new object[] { CampaignTime.Now });
				this.NextTriggerTime += this.TriggerPeriod;
			}
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00019600 File Offset: 0x00017800
		public void DeletePeriodicEvent()
		{
			this.isEventDeleted = true;
		}

		// Token: 0x04000182 RID: 386
		public string description;

		// Token: 0x04000183 RID: 387
		protected List<MBCampaignEvent.CampaignEventDelegate> handlers = new List<MBCampaignEvent.CampaignEventDelegate>();

		// Token: 0x04000184 RID: 388
		[CachedData]
		protected CampaignTime NextTriggerTime;

		// Token: 0x02000488 RID: 1160
		// (Invoke) Token: 0x0600400A RID: 16394
		public delegate void CampaignEventDelegate(MBCampaignEvent campaignEvent, params object[] delegateParams);
	}
}
