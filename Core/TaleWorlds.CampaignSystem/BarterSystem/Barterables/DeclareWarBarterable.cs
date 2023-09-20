using System;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.BarterSystem.Barterables
{
	// Token: 0x02000412 RID: 1042
	public class DeclareWarBarterable : Barterable
	{
		// Token: 0x17000D0D RID: 3341
		// (get) Token: 0x06003DB4 RID: 15796 RVA: 0x0012784E File Offset: 0x00125A4E
		public override string StringID
		{
			get
			{
				return "declare_war_barterable";
			}
		}

		// Token: 0x17000D0E RID: 3342
		// (get) Token: 0x06003DB5 RID: 15797 RVA: 0x00127855 File Offset: 0x00125A55
		// (set) Token: 0x06003DB6 RID: 15798 RVA: 0x0012785D File Offset: 0x00125A5D
		public IFaction DeclaringFaction { get; private set; }

		// Token: 0x17000D0F RID: 3343
		// (get) Token: 0x06003DB7 RID: 15799 RVA: 0x00127866 File Offset: 0x00125A66
		// (set) Token: 0x06003DB8 RID: 15800 RVA: 0x0012786E File Offset: 0x00125A6E
		public IFaction OtherFaction { get; private set; }

		// Token: 0x17000D10 RID: 3344
		// (get) Token: 0x06003DB9 RID: 15801 RVA: 0x00127877 File Offset: 0x00125A77
		public override TextObject Name
		{
			get
			{
				TextObject textObject = new TextObject("{=GZwNgIon}Declare war against {OTHER_FACTION}", null);
				textObject.SetTextVariable("OTHER_FACTION", this.OtherFaction.Name);
				return textObject;
			}
		}

		// Token: 0x06003DBA RID: 15802 RVA: 0x0012789B File Offset: 0x00125A9B
		public DeclareWarBarterable(IFaction declaringFaction, IFaction otherFaction)
			: base(declaringFaction.Leader, null)
		{
			this.DeclaringFaction = declaringFaction;
			this.OtherFaction = otherFaction;
		}

		// Token: 0x06003DBB RID: 15803 RVA: 0x001278B8 File Offset: 0x00125AB8
		public override void Apply()
		{
			DeclareWarAction.ApplyByDefault(base.OriginalOwner.MapFaction, this.OtherFaction.MapFaction);
		}

		// Token: 0x06003DBC RID: 15804 RVA: 0x001278D8 File Offset: 0x00125AD8
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

		// Token: 0x06003DBD RID: 15805 RVA: 0x0012798A File Offset: 0x00125B8A
		public override ImageIdentifier GetVisualIdentifier()
		{
			return null;
		}
	}
}
