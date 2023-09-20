﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Settlements
{
	public class Hideout : SettlementComponent, ISpottable
	{
		internal static void AutoGeneratedStaticCollectObjectsHideout(object o, List<object> collectedObjects)
		{
			((Hideout)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this._nextPossibleAttackTime, collectedObjects);
		}

		internal static object AutoGeneratedGetMemberValue_nextPossibleAttackTime(object o)
		{
			return ((Hideout)o)._nextPossibleAttackTime;
		}

		internal static object AutoGeneratedGetMemberValue_isSpotted(object o)
		{
			return ((Hideout)o)._isSpotted;
		}

		public CampaignTime NextPossibleAttackTime
		{
			get
			{
				return this._nextPossibleAttackTime;
			}
		}

		public static MBReadOnlyList<Hideout> All
		{
			get
			{
				return Campaign.Current.AllHideouts;
			}
		}

		public void UpdateNextPossibleAttackTime()
		{
			this._nextPossibleAttackTime = CampaignTime.Now + CampaignTime.Hours(12f);
		}

		public bool IsInfested
		{
			get
			{
				return base.Owner.Settlement.Parties.CountQ((MobileParty x) => x.IsBandit) >= Campaign.Current.Models.BanditDensityModel.NumberOfMinimumBanditPartiesInAHideoutToInfestIt;
			}
		}

		public IEnumerable<PartyBase> GetDefenderParties(MapEvent.BattleTypes battleType)
		{
			yield return base.Settlement.Party;
			foreach (MobileParty mobileParty in base.Settlement.Parties)
			{
				if (mobileParty.IsBandit || mobileParty.IsBanditBossParty)
				{
					yield return mobileParty.Party;
				}
			}
			List<MobileParty>.Enumerator enumerator = default(List<MobileParty>.Enumerator);
			yield break;
			yield break;
		}

		public PartyBase GetNextDefenderParty(ref int partyIndex, MapEvent.BattleTypes battleType)
		{
			partyIndex++;
			if (partyIndex == 0)
			{
				return base.Settlement.Party;
			}
			for (int i = partyIndex - 1; i < base.Settlement.Parties.Count; i++)
			{
				MobileParty mobileParty = base.Settlement.Parties[i];
				if (mobileParty.IsBandit || mobileParty.IsBanditBossParty)
				{
					partyIndex = i + 1;
					return mobileParty.Party;
				}
			}
			return null;
		}

		public string SceneName { get; private set; }

		public IFaction MapFaction
		{
			get
			{
				foreach (MobileParty mobileParty in base.Settlement.Parties)
				{
					if (mobileParty.IsBandit)
					{
						return mobileParty.ActualClan;
					}
				}
				foreach (Clan clan in Clan.All)
				{
					if (clan.IsBanditFaction)
					{
						return clan;
					}
				}
				return null;
			}
		}

		public bool IsSpotted
		{
			get
			{
				return this._isSpotted;
			}
			set
			{
				this._isSpotted = value;
			}
		}

		public void SetScene(string sceneName)
		{
			this.SceneName = sceneName;
		}

		public Hideout()
		{
			this.IsSpotted = false;
		}

		public override void OnPartyEntered(MobileParty mobileParty)
		{
			base.OnPartyEntered(mobileParty);
			this.UpdateOwnership();
			if (mobileParty.MapFaction.IsBanditFaction)
			{
				mobileParty.BanditPartyComponent.SetHomeHideout(base.Owner.Settlement.Hideout);
			}
		}

		public override void OnPartyLeft(MobileParty mobileParty)
		{
			this.UpdateOwnership();
			if (base.Owner.Settlement.Parties.Count == 0)
			{
				this.OnHideoutIsEmpty();
			}
		}

		public override void OnRelatedPartyRemoved(MobileParty mobileParty)
		{
			if (base.Owner.Settlement.Parties.Count == 0)
			{
				this.OnHideoutIsEmpty();
			}
		}

		private void OnHideoutIsEmpty()
		{
			this.IsSpotted = false;
			base.Owner.Settlement.IsVisible = false;
			CampaignEventDispatcher.Instance.OnHideoutDeactivated(base.Settlement);
		}

		public override void OnInit()
		{
			base.Owner.Settlement.IsVisible = false;
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.BackgroundCropPosition = float.Parse(node.Attributes["background_crop_position"].Value);
			base.BackgroundMeshName = node.Attributes["background_mesh"].Value;
			base.WaitMeshName = node.Attributes["wait_mesh"].Value;
			base.Deserialize(objectManager, node);
			if (node.Attributes["scene_name"] != null)
			{
				this.SceneName = node.Attributes["scene_name"].InnerText;
			}
		}

		private void UpdateOwnership()
		{
			if (base.Owner.MemberRoster.Count == 0 || base.Owner.Settlement.Parties.All((MobileParty x) => x.Party.Owner != base.Owner.Owner))
			{
				base.Owner.Settlement.Party.SetVisualAsDirty();
			}
		}

		protected override void OnInventoryUpdated(ItemRosterElement item, int count)
		{
		}

		[SaveableField(200)]
		private CampaignTime _nextPossibleAttackTime;

		[SaveableField(201)]
		private bool _isSpotted;
	}
}
