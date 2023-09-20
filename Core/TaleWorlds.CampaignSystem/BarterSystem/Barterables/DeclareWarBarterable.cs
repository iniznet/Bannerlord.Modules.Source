using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.BarterSystem.Barterables
{
	public class DeclareWarBarterable : Barterable
	{
		public override string StringID
		{
			get
			{
				return "declare_war_barterable";
			}
		}

		public IFaction DeclaringFaction { get; private set; }

		public IFaction OtherFaction { get; private set; }

		public override TextObject Name
		{
			get
			{
				TextObject textObject = new TextObject("{=GZwNgIon}Declare war against {OTHER_FACTION}", null);
				textObject.SetTextVariable("OTHER_FACTION", this.OtherFaction.Name);
				return textObject;
			}
		}

		public DeclareWarBarterable(IFaction declaringFaction, IFaction otherFaction)
			: base(declaringFaction.Leader, null)
		{
			this.DeclaringFaction = declaringFaction;
			this.OtherFaction = otherFaction;
		}

		public override void Apply()
		{
			DeclareWarAction.ApplyByDefault(base.OriginalOwner.MapFaction, this.OtherFaction.MapFaction);
		}

		public override int GetUnitValueForFaction(IFaction faction)
		{
			int num = 0;
			Clan clan = ((faction is Clan) ? ((Clan)faction) : ((Kingdom)faction).RulingClan);
			if (faction.MapFaction == base.OriginalOwner.MapFaction)
			{
				TextObject textObject;
				num = (int)Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(base.OriginalOwner.MapFaction, this.OtherFaction.MapFaction, clan, out textObject);
			}
			else if (faction.MapFaction == this.OtherFaction.MapFaction)
			{
				TextObject textObject;
				num = (int)Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(this.OtherFaction.MapFaction, base.OriginalOwner.MapFaction, clan, out textObject);
			}
			return num;
		}

		public override ImageIdentifier GetVisualIdentifier()
		{
			return null;
		}
	}
}
