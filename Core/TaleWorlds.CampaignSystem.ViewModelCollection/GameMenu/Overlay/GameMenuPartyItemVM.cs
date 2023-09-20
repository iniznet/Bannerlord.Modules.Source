using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.ViewModelCollection.Quests;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay
{
	public class GameMenuPartyItemVM : ViewModel
	{
		public GameMenuPartyItemVM()
		{
			this.Visual = new ImageIdentifierVM(ImageIdentifierType.Null);
		}

		public GameMenuPartyItemVM(Action<GameMenuPartyItemVM> onSetAsContextMenuActiveItem, Settlement settlement)
		{
			this._onSetAsContextMenuActiveItem = onSetAsContextMenuActiveItem;
			this.Settlement = settlement;
			SettlementComponent settlementComponent = settlement.SettlementComponent;
			this.SettlementPath = ((settlementComponent == null) ? "placeholder" : (settlementComponent.BackgroundMeshName + "_t"));
			this.Visual = new ImageIdentifierVM(ImageIdentifierType.Null);
			this.NameText = settlement.Name.ToString();
			this.PartySize = -1;
			this.PartyWoundedSize = -1;
			this.PartySizeLbl = "";
			this.IsPlayer = false;
			this.IsAlly = false;
			this.IsEnemy = false;
			this.Quests = new MBBindingList<QuestMarkerVM>();
			this.RefreshProperties();
		}

		public GameMenuPartyItemVM(Action<GameMenuPartyItemVM> onSetAsContextMenuActiveItem, PartyBase item, bool canShowQuest)
		{
			this._onSetAsContextMenuActiveItem = onSetAsContextMenuActiveItem;
			this.Party = item;
			CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(this.Party);
			if (visualPartyLeader != null)
			{
				CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(visualPartyLeader, false);
				this.Visual = new ImageIdentifierVM(characterCode);
			}
			else
			{
				this.Visual = new ImageIdentifierVM(ImageIdentifierType.Null);
			}
			this.Quests = new MBBindingList<QuestMarkerVM>();
			this._canShowQuest = canShowQuest;
			this.RefreshProperties();
		}

		public GameMenuPartyItemVM(Action<GameMenuPartyItemVM> onSetAsContextMenuActiveItem, CharacterObject character, bool useCivilianEquipment)
		{
			this._onSetAsContextMenuActiveItem = onSetAsContextMenuActiveItem;
			this.Character = character;
			this._useCivilianEquipment = useCivilianEquipment;
			CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(character, useCivilianEquipment);
			this.Visual = new ImageIdentifierVM(characterCode);
			Hero heroObject = this.Character.HeroObject;
			this.Banner_9 = (((heroObject != null && heroObject.IsLord) || (this.Character.IsHero && this.Character.HeroObject.Clan == Clan.PlayerClan && character.HeroObject.IsLord)) ? new ImageIdentifierVM(BannerCode.CreateFrom(this.Character.HeroObject.ClanBanner), true) : new ImageIdentifierVM(ImageIdentifierType.Null));
			this.NameText = this.Character.Name.ToString();
			this.PartySize = -1;
			this.PartyWoundedSize = -1;
			this.PartySizeLbl = "";
			this.IsPlayer = character.IsPlayerCharacter;
			this.Quests = new MBBindingList<QuestMarkerVM>();
			this.RefreshProperties();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RefreshProperties();
		}

		public void ExecuteSetAsContextMenuItem()
		{
			Action<GameMenuPartyItemVM> onSetAsContextMenuActiveItem = this._onSetAsContextMenuActiveItem;
			if (onSetAsContextMenuActiveItem == null)
			{
				return;
			}
			onSetAsContextMenuActiveItem(this);
		}

		public void ExecuteOpenEncyclopedia()
		{
			if (this.Character != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this.Character.EncyclopediaLink);
				return;
			}
			if (this.Party != null)
			{
				if (this.Party.LeaderHero != null)
				{
					Campaign.Current.EncyclopediaManager.GoToLink(this.Party.LeaderHero.EncyclopediaLink);
					return;
				}
				if (this.Party.Owner != null)
				{
					Campaign.Current.EncyclopediaManager.GoToLink(this.Party.Owner.EncyclopediaLink);
					return;
				}
				CharacterObject visualPartyLeader = CampaignUIHelper.GetVisualPartyLeader(this.Party);
				if (visualPartyLeader != null)
				{
					Campaign.Current.EncyclopediaManager.GoToLink(visualPartyLeader.EncyclopediaLink);
				}
			}
		}

		public void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

		public void ExecuteOpenTooltip()
		{
			PartyBase party = this.Party;
			if (((party != null) ? party.MobileParty : null) != null)
			{
				InformationManager.ShowTooltip(typeof(MobileParty), new object[]
				{
					this.Party.MobileParty,
					true,
					false
				});
				return;
			}
			if (this.Settlement != null)
			{
				InformationManager.ShowTooltip(typeof(Settlement), new object[] { this.Settlement, true });
				return;
			}
			InformationManager.ShowTooltip(typeof(Hero), new object[]
			{
				this.Character.HeroObject,
				true
			});
		}

		public void RefreshProperties()
		{
			if (this.Party != null)
			{
				this.PartyWoundedSize = this.Party.NumberOfAllMembers - this.Party.NumberOfHealthyMembers;
				this.PartySize = this.Party.NumberOfHealthyMembers;
				this.PartySizeLbl = this.Party.NumberOfHealthyMembers.ToString();
				this.Relation = HeroVM.GetRelation(this.Party.LeaderHero);
				this.LocationText = " ";
				TextObject textObject = this.Party.Name;
				if (this.Party.IsMobile)
				{
					textObject = this.Party.MobileParty.Name;
					if (this.Party.MobileParty.Position2D.DistanceSquared(MobileParty.MainParty.Position2D) > 9f)
					{
						if (this.Party.MobileParty.MapEvent == null)
						{
							GameTexts.SetVariable("LEFT", GameTexts.FindText("str_distance_to_army_leader", null));
							GameTexts.SetVariable("RIGHT", CampaignUIHelper.GetPartyDistanceByTimeText((float)((int)Campaign.Current.Models.MapDistanceModel.GetDistance(this.Party.MobileParty, MobileParty.MainParty)), this.Party.MobileParty.Speed));
							this.LocationText = GameTexts.FindText("str_LEFT_colon_RIGHT_wSpaceAfterColon", null).ToString();
						}
						else
						{
							TextObject textObject2 = GameTexts.FindText("str_at_map_event", null);
							TextObject textObject3 = new TextObject("{=zawBaxl5}Distance : {DISTANCE}", null);
							textObject3.SetTextVariable("DISTANCE", textObject2);
							this.LocationText = textObject3.ToString();
						}
					}
					this.DescriptionText = this.GetPartyDescriptionTextFromValues();
					this.IsMergedWithArmy = true;
					if (this.Party.MobileParty.Army != null)
					{
						this.IsMergedWithArmy = this.Party.MobileParty.Army.DoesLeaderPartyAndAttachedPartiesContain(this.Party.MobileParty);
					}
				}
				this.NameText = textObject.ToString();
				this.ProfessionText = " ";
			}
			else if (this.Character != null)
			{
				this.Relation = HeroVM.GetRelation(this.Character.HeroObject);
				Hero heroObject = this.Character.HeroObject;
				this.IsCharacterInPrison = heroObject != null && heroObject.IsPrisoner;
				GameTexts.SetVariable("PROFESSION", HeroHelper.GetCharacterTypeName(this.Character.HeroObject));
				string text = "LOCATION";
				Hero heroObject2 = this.Character.HeroObject;
				GameTexts.SetVariable(text, (((heroObject2 != null) ? heroObject2.CurrentSettlement : null) != null) ? this.Character.HeroObject.CurrentSettlement.Name.ToString() : "");
				Hero heroObject3 = this.Character.HeroObject;
				this.DescriptionText = ((heroObject3 != null && !heroObject3.IsSpecial) ? GameTexts.FindText("str_character_in_town", null).ToString() : string.Empty);
				string text2 = "LOCATION";
				LocationComplex locationComplex = LocationComplex.Current;
				TextObject textObject4;
				if (locationComplex == null)
				{
					textObject4 = null;
				}
				else
				{
					Location locationOfCharacter = locationComplex.GetLocationOfCharacter(this.Character.HeroObject);
					textObject4 = ((locationOfCharacter != null) ? locationOfCharacter.Name : null);
				}
				GameTexts.SetVariable(text2, textObject4 ?? TextObject.Empty);
				this.LocationText = GameTexts.FindText("str_location_colon", null).ToString();
				GameTexts.SetVariable("PROFESSION", HeroHelper.GetCharacterTypeName(this.Character.HeroObject));
				this.ProfessionText = GameTexts.FindText("str_profession_colon", null).ToString();
				if (this.Character.IsHero && this.Character.HeroObject.IsNotable)
				{
					GameTexts.SetVariable("POWER", Campaign.Current.Models.NotablePowerModel.GetPowerRankName(this.Character.HeroObject).ToString());
					this.PowerText = GameTexts.FindText("str_power_colon", null).ToString();
				}
				this.NameText = this.Character.Name.ToString();
			}
			this.RefreshQuestStatus();
			this.RefreshRelationStatus();
		}

		public void RefreshQuestStatus()
		{
			this.Quests.Clear();
			PartyBase party = this.Party;
			Hero hero;
			if ((hero = ((party != null) ? party.LeaderHero : null)) == null)
			{
				CharacterObject character = this.Character;
				hero = ((character != null) ? character.HeroObject : null);
			}
			Hero hero2 = hero;
			if (hero2 != null)
			{
				GameMenuPartyItemVM.<>c__DisplayClass16_0 CS$<>8__locals1 = new GameMenuPartyItemVM.<>c__DisplayClass16_0();
				CS$<>8__locals1.questTypes = CampaignUIHelper.GetQuestStateOfHero(hero2);
				int k;
				int i;
				for (i = 0; i < CS$<>8__locals1.questTypes.Count; i = k + 1)
				{
					if (!this.Quests.Any((QuestMarkerVM q) => q.QuestMarkerType == (int)CS$<>8__locals1.questTypes[i].Item1))
					{
						this.Quests.Add(new QuestMarkerVM(CS$<>8__locals1.questTypes[i].Item1, CS$<>8__locals1.questTypes[i].Item2, CS$<>8__locals1.questTypes[i].Item3));
					}
					k = i;
				}
			}
			else
			{
				PartyBase party2 = this.Party;
				if (((party2 != null) ? party2.MobileParty : null) != null)
				{
					List<QuestBase> questsRelatedToParty = CampaignUIHelper.GetQuestsRelatedToParty(this.Party.MobileParty);
					for (int j = 0; j < questsRelatedToParty.Count; j++)
					{
						TextObject textObject = ((questsRelatedToParty[j].JournalEntries.Count > 0) ? questsRelatedToParty[j].JournalEntries[0].LogText : TextObject.Empty);
						CampaignUIHelper.IssueQuestFlags issueQuestFlags;
						if (hero2 != null && questsRelatedToParty[j].QuestGiver == hero2)
						{
							issueQuestFlags = (questsRelatedToParty[j].IsSpecialQuest ? CampaignUIHelper.IssueQuestFlags.ActiveStoryQuest : CampaignUIHelper.IssueQuestFlags.ActiveIssue);
						}
						else
						{
							issueQuestFlags = (questsRelatedToParty[j].IsSpecialQuest ? CampaignUIHelper.IssueQuestFlags.TrackedStoryQuest : CampaignUIHelper.IssueQuestFlags.TrackedIssue);
						}
						this.Quests.Add(new QuestMarkerVM(issueQuestFlags, questsRelatedToParty[j].Title, textObject));
					}
				}
			}
			this.Quests.Sort(new GameMenuPartyItemVM.QuestMarkerComparer());
		}

		private void RefreshRelationStatus()
		{
			this.IsEnemy = false;
			this.IsAlly = false;
			this.IsNeutral = false;
			IFaction faction = null;
			bool flag = false;
			if (this.Character != null)
			{
				this.IsPlayer = this.Character.IsPlayerCharacter;
				flag = this.Character.IsHero && this.Character.HeroObject.IsNotable;
				IFaction faction2;
				if (!this.IsPlayer)
				{
					CharacterObject character = this.Character;
					faction2 = ((character != null) ? character.HeroObject.MapFaction : null);
				}
				else
				{
					faction2 = null;
				}
				faction = faction2;
			}
			else if (this.Party != null)
			{
				bool flag2;
				if (this.Party.IsMobile)
				{
					MobileParty mobileParty = this.Party.MobileParty;
					flag2 = mobileParty != null && mobileParty.IsMainParty;
				}
				else
				{
					flag2 = false;
				}
				this.IsPlayer = flag2;
				flag = false;
				IFaction faction3;
				if (!this.IsPlayer)
				{
					PartyBase party = this.Party;
					if (party == null)
					{
						faction3 = null;
					}
					else
					{
						MobileParty mobileParty2 = party.MobileParty;
						faction3 = ((mobileParty2 != null) ? mobileParty2.MapFaction : null);
					}
				}
				else
				{
					faction3 = null;
				}
				faction = faction3;
			}
			if (this.IsPlayer || faction == null || flag)
			{
				this.IsNeutral = true;
				return;
			}
			if (FactionManager.IsAtWarAgainstFaction(faction, Hero.MainHero.MapFaction))
			{
				this.IsEnemy = true;
				return;
			}
			if (FactionManager.IsAlliedWithFaction(faction, Hero.MainHero.MapFaction))
			{
				this.IsAlly = true;
				return;
			}
			this.IsNeutral = true;
		}

		public void RefreshVisual()
		{
			if (this.Visual.IsEmpty)
			{
				if (this.Character != null)
				{
					CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(this.Character, this._useCivilianEquipment);
					this.Visual = new ImageIdentifierVM(characterCode);
					return;
				}
				if (this.Party != null)
				{
					CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(this.Party);
					if (visualPartyLeader != null)
					{
						CharacterCode characterCode2 = CampaignUIHelper.GetCharacterCode(visualPartyLeader, false);
						this.Visual = new ImageIdentifierVM(characterCode2);
						return;
					}
					this.Visual = new ImageIdentifierVM(ImageIdentifierType.Null);
				}
			}
		}

		public string GetPartyDescriptionTextFromValues()
		{
			GameTexts.SetVariable("newline", "\n");
			string text = ((this.Party.MobileParty.CurrentSettlement != null && this.Party.MobileParty.MapEvent == null) ? "" : CampaignUIHelper.GetMobilePartyBehaviorText(this.Party.MobileParty));
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_food", null).ToString());
			GameTexts.SetVariable("RIGHT", this.Party.MobileParty.Food);
			string text2 = GameTexts.FindText("str_LEFT_colon_RIGHT", null).ToString();
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_map_tooltip_speed", null).ToString());
			GameTexts.SetVariable("RIGHT", this.Party.MobileParty.Speed.ToString("F"));
			string text3 = GameTexts.FindText("str_LEFT_colon_RIGHT", null).ToString();
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_seeing_range", null).ToString());
			GameTexts.SetVariable("RIGHT", this.Party.MobileParty.SeeingRange);
			string text4 = GameTexts.FindText("str_LEFT_colon_RIGHT", null).ToString();
			GameTexts.SetVariable("STR1", text);
			GameTexts.SetVariable("STR2", text2);
			string text5 = GameTexts.FindText("str_string_newline_string", null).ToString();
			GameTexts.SetVariable("STR1", text5);
			GameTexts.SetVariable("STR2", text3);
			text5 = GameTexts.FindText("str_string_newline_string", null).ToString();
			GameTexts.SetVariable("STR1", text5);
			GameTexts.SetVariable("STR2", text4);
			return GameTexts.FindText("str_string_newline_string", null).ToString();
		}

		[DataSourceProperty]
		public int Relation
		{
			get
			{
				return this._relation;
			}
			set
			{
				if (value != this._relation)
				{
					this._relation = value;
					base.OnPropertyChangedWithValue(value, "Relation");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<QuestMarkerVM> Quests
		{
			get
			{
				return this._quests;
			}
			set
			{
				if (value != this._quests)
				{
					this._quests = value;
					base.OnPropertyChangedWithValue<MBBindingList<QuestMarkerVM>>(value, "Quests");
				}
			}
		}

		[DataSourceProperty]
		public bool IsHighlightEnabled
		{
			get
			{
				return this._isHighlightEnabled;
			}
			set
			{
				if (value != this._isHighlightEnabled)
				{
					this._isHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsHighlightEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCharacterInPrison
		{
			get
			{
				return this._isCharacterInPrison;
			}
			set
			{
				if (value != this._isCharacterInPrison)
				{
					this._isCharacterInPrison = value;
					base.OnPropertyChangedWithValue(value, "IsCharacterInPrison");
				}
			}
		}

		[DataSourceProperty]
		public bool IsIdle
		{
			get
			{
				return this._isIdle;
			}
			set
			{
				if (value != this._isIdle)
				{
					this._isIdle = value;
					base.OnPropertyChangedWithValue(value, "IsIdle");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPlayer
		{
			get
			{
				return this._isPlayer;
			}
			set
			{
				if (value != this._isPlayer)
				{
					this._isPlayer = value;
					base.OnPropertyChanged("IsPlayerParty");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEnemy
		{
			get
			{
				return this._isEnemy;
			}
			set
			{
				if (value != this._isEnemy)
				{
					this._isEnemy = value;
					base.OnPropertyChangedWithValue(value, "IsEnemy");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAlly
		{
			get
			{
				return this._isAlly;
			}
			set
			{
				if (value != this._isAlly)
				{
					this._isAlly = value;
					base.OnPropertyChangedWithValue(value, "IsAlly");
				}
			}
		}

		[DataSourceProperty]
		public bool IsNeutral
		{
			get
			{
				return this._isNeutral;
			}
			set
			{
				if (value != this._isNeutral)
				{
					this._isNeutral = value;
					base.OnPropertyChangedWithValue(value, "IsNeutral");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMergedWithArmy
		{
			get
			{
				return this._isMergedWithArmy;
			}
			set
			{
				if (value != this._isMergedWithArmy)
				{
					this._isMergedWithArmy = value;
					base.OnPropertyChangedWithValue(value, "IsMergedWithArmy");
				}
			}
		}

		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		[DataSourceProperty]
		public string SettlementPath
		{
			get
			{
				return this._settlementPath;
			}
			set
			{
				if (value != this._settlementPath)
				{
					this._settlementPath = value;
					base.OnPropertyChangedWithValue<string>(value, "SettlementPath");
				}
			}
		}

		[DataSourceProperty]
		public string LocationText
		{
			get
			{
				return this._locationText;
			}
			set
			{
				if (value != this._locationText)
				{
					this._locationText = value;
					base.OnPropertyChangedWithValue<string>(value, "LocationText");
				}
			}
		}

		[DataSourceProperty]
		public string PowerText
		{
			get
			{
				return this._powerText;
			}
			set
			{
				if (value != this._powerText)
				{
					this._powerText = value;
					base.OnPropertyChangedWithValue<string>(value, "PowerText");
				}
			}
		}

		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public string ProfessionText
		{
			get
			{
				return this._professionText;
			}
			set
			{
				if (value != this._professionText)
				{
					this._professionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProfessionText");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Banner_9
		{
			get
			{
				return this._banner_9;
			}
			set
			{
				if (value != this._banner_9)
				{
					this._banner_9 = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Banner_9");
				}
			}
		}

		[DataSourceProperty]
		public int PartySize
		{
			get
			{
				return this._partySize;
			}
			set
			{
				if (value != this._partySize)
				{
					this._partySize = value;
					base.OnPropertyChangedWithValue(value, "PartySize");
				}
			}
		}

		[DataSourceProperty]
		public int PartyWoundedSize
		{
			get
			{
				return this._partyWoundedSize;
			}
			set
			{
				if (value != this._partySize)
				{
					this._partyWoundedSize = value;
					base.OnPropertyChangedWithValue(value, "PartyWoundedSize");
				}
			}
		}

		[DataSourceProperty]
		public string PartySizeLbl
		{
			get
			{
				return this._partySizeLbl;
			}
			set
			{
				if (value != this._partySizeLbl)
				{
					this._partySizeLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "PartySizeLbl");
				}
			}
		}

		[DataSourceProperty]
		public bool IsLeader
		{
			get
			{
				return this._isLeader;
			}
			set
			{
				if (value != this._isLeader)
				{
					this._isLeader = value;
					base.OnPropertyChangedWithValue(value, "IsLeader");
				}
			}
		}

		public CharacterObject Character;

		public PartyBase Party;

		public Settlement Settlement;

		private readonly bool _canShowQuest = true;

		private readonly bool _useCivilianEquipment;

		private readonly Action<GameMenuPartyItemVM> _onSetAsContextMenuActiveItem;

		private MBBindingList<QuestMarkerVM> _quests;

		private int _partySize;

		private int _partyWoundedSize;

		private int _relation = -101;

		private ImageIdentifierVM _visual;

		private ImageIdentifierVM _banner_9;

		private string _settlementPath;

		private string _partySizeLbl;

		private string _nameText;

		private string _locationText;

		private string _descriptionText;

		private string _professionText;

		private string _powerText;

		private bool _isIdle;

		private bool _isPlayer;

		private bool _isEnemy;

		private bool _isAlly;

		private bool _isNeutral;

		private bool _isHighlightEnabled;

		private bool _isLeader;

		private bool _isMergedWithArmy;

		private bool _isCharacterInPrison;

		private class QuestMarkerComparer : IComparer<QuestMarkerVM>
		{
			public int Compare(QuestMarkerVM x, QuestMarkerVM y)
			{
				return x.QuestMarkerType.CompareTo(y.QuestMarkerType);
			}
		}
	}
}
