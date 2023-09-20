using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	public class BarterData
	{
		public IFaction OffererMapFaction
		{
			get
			{
				Hero offererHero = this.OffererHero;
				return ((offererHero != null) ? offererHero.MapFaction : null) ?? this.OffererParty.MapFaction;
			}
		}

		public IFaction OtherMapFaction
		{
			get
			{
				Hero otherHero = this.OtherHero;
				return ((otherHero != null) ? otherHero.MapFaction : null) ?? this.OtherParty.MapFaction;
			}
		}

		public bool IsAiBarter { get; }

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

		public void AddBarterGroup(BarterGroup barterGroup)
		{
			this._barterGroups.Add(barterGroup);
		}

		public List<BarterGroup> GetBarterGroups()
		{
			return this._barterGroups;
		}

		public List<Barterable> GetBarterables()
		{
			return this._barterables;
		}

		public BarterGroup GetBarterGroup<T>()
		{
			IEnumerable<T> enumerable = this._barterGroups.OfType<T>();
			if (enumerable.IsEmpty<T>())
			{
				return null;
			}
			return enumerable.First<T>() as BarterGroup;
		}

		public List<Barterable> GetOfferedBarterables()
		{
			return (from barterable in this.GetBarterables()
				where barterable.IsOffered
				select barterable).ToList<Barterable>();
		}

		public readonly Hero OffererHero;

		public readonly Hero OtherHero;

		public readonly PartyBase OffererParty;

		public readonly PartyBase OtherParty;

		private List<Barterable> _barterables;

		private List<BarterGroup> _barterGroups;

		public readonly BarterManager.BarterContextInitializer ContextInitializer;

		public readonly int PersuasionCostReduction;
	}
}
