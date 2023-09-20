using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	// Token: 0x02000407 RID: 1031
	public class BarterData
	{
		// Token: 0x17000CF5 RID: 3317
		// (get) Token: 0x06003D58 RID: 15704 RVA: 0x00126C99 File Offset: 0x00124E99
		public IFaction OffererMapFaction
		{
			get
			{
				Hero offererHero = this.OffererHero;
				return ((offererHero != null) ? offererHero.MapFaction : null) ?? this.OffererParty.MapFaction;
			}
		}

		// Token: 0x17000CF6 RID: 3318
		// (get) Token: 0x06003D59 RID: 15705 RVA: 0x00126CBC File Offset: 0x00124EBC
		public IFaction OtherMapFaction
		{
			get
			{
				Hero otherHero = this.OtherHero;
				return ((otherHero != null) ? otherHero.MapFaction : null) ?? this.OtherParty.MapFaction;
			}
		}

		// Token: 0x17000CF7 RID: 3319
		// (get) Token: 0x06003D5A RID: 15706 RVA: 0x00126CDF File Offset: 0x00124EDF
		public bool IsAiBarter { get; }

		// Token: 0x06003D5B RID: 15707 RVA: 0x00126CE8 File Offset: 0x00124EE8
		public BarterData(Hero offerer, Hero other, PartyBase offererParty, PartyBase otherParty, BarterManager.BarterContextInitializer contextInitializer = null, int persuasionCostReduction = 0, bool isAiBarter = false)
		{
			this.OffererParty = offererParty;
			this.OtherParty = otherParty;
			this.OffererHero = offerer;
			this.OtherHero = other;
			this.ContextInitializer = contextInitializer;
			this.PersuasionCostReduction = persuasionCostReduction;
			this._barterables = new List<Barterable>(16);
			this._barterGroups = Campaign.Current.Models.DiplomacyModel.GetBarterGroups().ToList<BarterGroup>();
			this.IsAiBarter = isAiBarter;
		}

		// Token: 0x06003D5C RID: 15708 RVA: 0x00126D5C File Offset: 0x00124F5C
		public void AddBarterable<T>(Barterable barterable, bool isContextDependent = false)
		{
			foreach (BarterGroup barterGroup in this._barterGroups)
			{
				if (barterGroup is T)
				{
					barterable.Initialize(barterGroup, isContextDependent);
					this._barterables.Add(barterable);
					break;
				}
			}
		}

		// Token: 0x06003D5D RID: 15709 RVA: 0x00126DC8 File Offset: 0x00124FC8
		public void AddBarterGroup(BarterGroup barterGroup)
		{
			this._barterGroups.Add(barterGroup);
		}

		// Token: 0x06003D5E RID: 15710 RVA: 0x00126DD6 File Offset: 0x00124FD6
		public List<BarterGroup> GetBarterGroups()
		{
			return this._barterGroups;
		}

		// Token: 0x06003D5F RID: 15711 RVA: 0x00126DDE File Offset: 0x00124FDE
		public List<Barterable> GetBarterables()
		{
			return this._barterables;
		}

		// Token: 0x06003D60 RID: 15712 RVA: 0x00126DE8 File Offset: 0x00124FE8
		public BarterGroup GetBarterGroup<T>()
		{
			IEnumerable<T> enumerable = this._barterGroups.OfType<T>();
			if (enumerable.IsEmpty<T>())
			{
				return null;
			}
			return enumerable.First<T>() as BarterGroup;
		}

		// Token: 0x06003D61 RID: 15713 RVA: 0x00126E1B File Offset: 0x0012501B
		public List<Barterable> GetOfferedBarterables()
		{
			return (from barterable in this.GetBarterables()
				where barterable.IsOffered
				select barterable).ToList<Barterable>();
		}

		// Token: 0x04001275 RID: 4725
		public readonly Hero OffererHero;

		// Token: 0x04001276 RID: 4726
		public readonly Hero OtherHero;

		// Token: 0x04001277 RID: 4727
		public readonly PartyBase OffererParty;

		// Token: 0x04001278 RID: 4728
		public readonly PartyBase OtherParty;

		// Token: 0x04001279 RID: 4729
		private List<Barterable> _barterables;

		// Token: 0x0400127A RID: 4730
		private List<BarterGroup> _barterGroups;

		// Token: 0x0400127B RID: 4731
		public readonly BarterManager.BarterContextInitializer ContextInitializer;

		// Token: 0x0400127C RID: 4732
		public readonly int PersuasionCostReduction;
	}
}
