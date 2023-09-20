using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem
{
	public class MBCampaignEvent
	{
		public CampaignTime TriggerPeriod { get; private set; }

		public CampaignTime InitialWait { get; private set; }

		public bool isEventDeleted { get; set; }

		public MBCampaignEvent(string eventName)
		{
			this.description = eventName;
		}

		public MBCampaignEvent(CampaignTime triggerPeriod, CampaignTime initialWait)
		{
			this.TriggerPeriod = triggerPeriod;
			this.InitialWait = initialWait;
			this.NextTriggerTime = CampaignTime.Now + this.InitialWait;
			this.isEventDeleted = false;
		}

		public void AddHandler(MBCampaignEvent.CampaignEventDelegate gameEventDelegate)
		{
			this.handlers.Add(gameEventDelegate);
		}

		public void RunHandlers(params object[] delegateParams)
		{
			for (int i = 0; i < this.handlers.Count; i++)
			{
				this.handlers[i](this, delegateParams);
			}
		}

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

		public void CheckUpdate()
		{
			while (this.NextTriggerTime.IsPast && !this.isEventDeleted)
			{
				this.RunHandlers(new object[] { CampaignTime.Now });
				this.NextTriggerTime += this.TriggerPeriod;
			}
		}

		public void DeletePeriodicEvent()
		{
			this.isEventDeleted = true;
		}

		public string description;

		protected List<MBCampaignEvent.CampaignEventDelegate> handlers = new List<MBCampaignEvent.CampaignEventDelegate>();

		[CachedData]
		protected CampaignTime NextTriggerTime;

		public delegate void CampaignEventDelegate(MBCampaignEvent campaignEvent, params object[] delegateParams);
	}
}
