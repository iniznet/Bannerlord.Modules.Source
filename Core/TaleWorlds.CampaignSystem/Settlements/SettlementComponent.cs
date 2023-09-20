﻿using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Settlements
{
	public abstract class SettlementComponent : MBObjectBase
	{
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		internal static object AutoGeneratedGetMemberValueGold(object o)
		{
			return ((SettlementComponent)o).Gold;
		}

		internal static object AutoGeneratedGetMemberValueIsOwnerUnassigned(object o)
		{
			return ((SettlementComponent)o).IsOwnerUnassigned;
		}

		[SaveableProperty(50)]
		public int Gold { get; private set; }

		public virtual SettlementComponent.ProsperityLevel GetProsperityLevel()
		{
			return SettlementComponent.ProsperityLevel.Mid;
		}

		public float BackgroundCropPosition { get; protected set; }

		public string BackgroundMeshName { get; protected set; }

		public string WaitMeshName { get; protected set; }

		public string CastleBackgroundMeshName { get; protected set; }

		public PartyBase Owner
		{
			get
			{
				return this._owner;
			}
			internal set
			{
				if (this._owner != value)
				{
					if (this._owner != null)
					{
						this._owner.ItemRoster.RosterUpdatedEvent -= this.OnInventoryUpdated;
					}
					this._owner = value;
					if (this._owner != null)
					{
						this._owner.ItemRoster.RosterUpdatedEvent += this.OnInventoryUpdated;
					}
				}
			}
		}

		public Settlement Settlement
		{
			get
			{
				return this._owner.Settlement;
			}
		}

		protected abstract void OnInventoryUpdated(ItemRosterElement item, int count);

		public TextObject Name
		{
			get
			{
				return this.Owner.Name;
			}
		}

		[SaveableProperty(80)]
		public bool IsOwnerUnassigned { get; set; }

		public virtual void OnPartyEntered(MobileParty mobileParty)
		{
		}

		public virtual void OnPartyLeft(MobileParty mobileParty)
		{
		}

		public virtual void OnInit()
		{
		}

		public void ChangeGold(int changeAmount)
		{
			this.Gold += changeAmount;
			if (this.Gold < 0)
			{
				this.Gold = 0;
			}
		}

		public int GetNumberOfTroops()
		{
			int num = 0;
			foreach (MobileParty mobileParty in this.Owner.Settlement.Parties)
			{
				if (mobileParty.IsMilitia || mobileParty.IsGarrison)
				{
					num += mobileParty.Party.NumberOfAllMembers;
				}
			}
			return num;
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
		}

		public virtual int GetItemPrice(ItemObject item, MobileParty tradingParty = null, bool isSelling = false)
		{
			return 0;
		}

		public virtual int GetItemPrice(EquipmentElement itemRosterElement, MobileParty tradingParty = null, bool isSelling = false)
		{
			return 0;
		}

		public virtual bool IsTown
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsCastle
		{
			get
			{
				return false;
			}
		}

		public virtual void OnRelatedPartyRemoved(MobileParty mobileParty)
		{
		}

		public List<CharacterObject> GetPrisonerHeroes()
		{
			List<PartyBase> list = new List<PartyBase> { this.Owner };
			foreach (MobileParty mobileParty in this.Owner.Settlement.Parties)
			{
				if (mobileParty.IsGarrison)
				{
					list.Add(mobileParty.Party);
				}
			}
			List<CharacterObject> list2 = new List<CharacterObject>();
			foreach (PartyBase partyBase in list)
			{
				for (int i = 0; i < partyBase.PrisonRoster.Count; i++)
				{
					for (int j = 0; j < partyBase.PrisonRoster.GetElementNumber(i); j++)
					{
						CharacterObject character = partyBase.PrisonRoster.GetElementCopyAtIndex(i).Character;
						if (character.IsHero)
						{
							list2.Add(character);
						}
					}
				}
			}
			return list2;
		}

		private PartyBase _owner;

		public enum ProsperityLevel
		{
			Low,
			Mid,
			High,
			NumberOfLevels
		}
	}
}
