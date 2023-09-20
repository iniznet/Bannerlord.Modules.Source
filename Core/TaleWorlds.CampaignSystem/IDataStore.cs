using System;

namespace TaleWorlds.CampaignSystem
{
	public interface IDataStore
	{
		bool SyncData<T>(string key, ref T data);

		bool IsSaving { get; }

		bool IsLoading { get; }
	}
}
