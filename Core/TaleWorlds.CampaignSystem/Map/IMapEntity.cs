using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Map
{
	public interface IMapEntity
	{
		Vec2 InteractionPosition { get; }

		TextObject Name { get; }

		bool OnMapClick(bool followModifierUsed);

		void OnHover();

		void OnOpenEncyclopedia();

		bool IsMobileEntity { get; }

		IMapEntity AttachedEntity { get; }

		IPartyVisual PartyVisual { get; }

		bool ShowCircleAroundEntity { get; }

		bool IsMainEntity();

		bool IsEnemyOf(IFaction faction);

		bool IsAllyOf(IFaction faction);

		void GetMountAndHarnessVisualIdsForPartyIcon(out string mountStringId, out string harnessStringId);

		void OnPartyInteraction(MobileParty mobileParty);
	}
}
