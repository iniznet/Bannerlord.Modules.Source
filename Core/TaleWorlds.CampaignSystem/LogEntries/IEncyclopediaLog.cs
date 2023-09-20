using System;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public interface IEncyclopediaLog
	{
		bool IsVisibleInEncyclopediaPageOf<T>(T obj) where T : MBObjectBase;

		TextObject GetEncyclopediaText();

		CampaignTime GameTime { get; }
	}
}
