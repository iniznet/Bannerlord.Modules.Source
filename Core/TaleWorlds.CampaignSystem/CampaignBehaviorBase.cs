using System;

namespace TaleWorlds.CampaignSystem
{
	public abstract class CampaignBehaviorBase : ICampaignBehavior
	{
		public CampaignBehaviorBase(string stringId)
		{
			this.StringId = stringId;
		}

		public CampaignBehaviorBase()
		{
			this.StringId = base.GetType().Name;
		}

		public abstract void RegisterEvents();

		public static T GetCampaignBehavior<T>()
		{
			return Campaign.Current.GetCampaignBehavior<T>();
		}

		public abstract void SyncData(IDataStore dataStore);

		public readonly string StringId;
	}
}
