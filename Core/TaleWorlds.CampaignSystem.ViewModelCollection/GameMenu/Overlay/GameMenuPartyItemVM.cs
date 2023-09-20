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
	// Token: 0x020000A3 RID: 163
	public class GameMenuPartyItemVM : ViewModel
	{
		// Token: 0x06001023 RID: 4131 RVA: 0x0003FB67 File Offset: 0x0003DD67
		public GameMenuPartyItemVM()
		{
			this.Visual = new ImageIdentifierVM(ImageIdentifierType.Null);
		}

		// Token: 0x06001024 RID: 4132 RVA: 0x0003FB8C File Offset: 0x0003DD8C
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

		// Token: 0x06001025 RID: 4133 RVA: 0x0003FC40 File Offset: 0x0003DE40
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

		// Token: 0x06001026 RID: 4134 RVA: 0x0003FCBC File Offset: 0x0003DEBC
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

		// Token: 0x06001027 RID: 4135 RVA: 0x0003FDCE File Offset: 0x0003DFCE
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RefreshProperties();
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x0003FDDC File Offset: 0x0003DFDC
		public void ExecuteSetAsContextMenuItem()
		{
			Action<GameMenuPartyItemVM> onSetAsContextMenuActiveItem = this._onSetAsContextMenuActiveItem;
			if (onSetAsContextMenuActiveItem == null)
			{
				return;
			}
			onSetAsContextMenuActiveItem(this);
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x0003FDF0 File Offset: 0x0003DFF0
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

		// Token: 0x0600102A RID: 4138 RVA: 0x0003FEA6 File Offset: 0x0003E0A6
		public void ExecuteCloseTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x0003FEB0 File Offset: 0x0003E0B0
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

		// Token: 0x0600102C RID: 4140 RVA: 0x0003FF64 File Offset: 0x0003E164
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
				this.DescriptionText = GameTexts.FindText("str_character_in_town", null).ToString();
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

		// Token: 0x0600102D RID: 4141 RVA: 0x00040314 File Offset: 0x0003E514
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

		// Token: 0x0600102E RID: 4142 RVA: 0x00040524 File Offset: 0x0003E724
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

		// Token: 0x0600102F RID: 4143 RVA: 0x0004065C File Offset: 0x0003E85C
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

		// Token: 0x06001030 RID: 4144 RVA: 0x000406D8 File Offset: 0x0003E8D8
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

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06001031 RID: 4145 RVA: 0x00040885 File Offset: 0x0003EA85
		// (set) Token: 0x06001032 RID: 4146 RVA: 0x0004088D File Offset: 0x0003EA8D
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

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06001033 RID: 4147 RVA: 0x000408AB File Offset: 0x0003EAAB
		// (set) Token: 0x06001034 RID: 4148 RVA: 0x000408B3 File Offset: 0x0003EAB3
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

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06001035 RID: 4149 RVA: 0x000408D1 File Offset: 0x0003EAD1
		// (set) Token: 0x06001036 RID: 4150 RVA: 0x000408D9 File Offset: 0x0003EAD9
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

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06001037 RID: 4151 RVA: 0x000408F7 File Offset: 0x0003EAF7
		// (set) Token: 0x06001038 RID: 4152 RVA: 0x000408FF File Offset: 0x0003EAFF
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

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06001039 RID: 4153 RVA: 0x0004091D File Offset: 0x0003EB1D
		// (set) Token: 0x0600103A RID: 4154 RVA: 0x00040925 File Offset: 0x0003EB25
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

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x0600103B RID: 4155 RVA: 0x00040943 File Offset: 0x0003EB43
		// (set) Token: 0x0600103C RID: 4156 RVA: 0x0004094B File Offset: 0x0003EB4B
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

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x0600103D RID: 4157 RVA: 0x00040968 File Offset: 0x0003EB68
		// (set) Token: 0x0600103E RID: 4158 RVA: 0x00040970 File Offset: 0x0003EB70
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

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x0600103F RID: 4159 RVA: 0x0004098E File Offset: 0x0003EB8E
		// (set) Token: 0x06001040 RID: 4160 RVA: 0x00040996 File Offset: 0x0003EB96
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

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06001041 RID: 4161 RVA: 0x000409B4 File Offset: 0x0003EBB4
		// (set) Token: 0x06001042 RID: 4162 RVA: 0x000409BC File Offset: 0x0003EBBC
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

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06001043 RID: 4163 RVA: 0x000409DA File Offset: 0x0003EBDA
		// (set) Token: 0x06001044 RID: 4164 RVA: 0x000409E2 File Offset: 0x0003EBE2
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

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x06001045 RID: 4165 RVA: 0x00040A00 File Offset: 0x0003EC00
		// (set) Token: 0x06001046 RID: 4166 RVA: 0x00040A08 File Offset: 0x0003EC08
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

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06001047 RID: 4167 RVA: 0x00040A2B File Offset: 0x0003EC2B
		// (set) Token: 0x06001048 RID: 4168 RVA: 0x00040A33 File Offset: 0x0003EC33
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

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06001049 RID: 4169 RVA: 0x00040A56 File Offset: 0x0003EC56
		// (set) Token: 0x0600104A RID: 4170 RVA: 0x00040A5E File Offset: 0x0003EC5E
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

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x0600104B RID: 4171 RVA: 0x00040A81 File Offset: 0x0003EC81
		// (set) Token: 0x0600104C RID: 4172 RVA: 0x00040A89 File Offset: 0x0003EC89
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

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x0600104D RID: 4173 RVA: 0x00040AAC File Offset: 0x0003ECAC
		// (set) Token: 0x0600104E RID: 4174 RVA: 0x00040AB4 File Offset: 0x0003ECB4
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

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x0600104F RID: 4175 RVA: 0x00040AD7 File Offset: 0x0003ECD7
		// (set) Token: 0x06001050 RID: 4176 RVA: 0x00040ADF File Offset: 0x0003ECDF
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

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06001051 RID: 4177 RVA: 0x00040B02 File Offset: 0x0003ED02
		// (set) Token: 0x06001052 RID: 4178 RVA: 0x00040B0A File Offset: 0x0003ED0A
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

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06001053 RID: 4179 RVA: 0x00040B28 File Offset: 0x0003ED28
		// (set) Token: 0x06001054 RID: 4180 RVA: 0x00040B30 File Offset: 0x0003ED30
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

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06001055 RID: 4181 RVA: 0x00040B4E File Offset: 0x0003ED4E
		// (set) Token: 0x06001056 RID: 4182 RVA: 0x00040B56 File Offset: 0x0003ED56
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

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06001057 RID: 4183 RVA: 0x00040B74 File Offset: 0x0003ED74
		// (set) Token: 0x06001058 RID: 4184 RVA: 0x00040B7C File Offset: 0x0003ED7C
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

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06001059 RID: 4185 RVA: 0x00040B9A File Offset: 0x0003ED9A
		// (set) Token: 0x0600105A RID: 4186 RVA: 0x00040BA2 File Offset: 0x0003EDA2
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

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x0600105B RID: 4187 RVA: 0x00040BC5 File Offset: 0x0003EDC5
		// (set) Token: 0x0600105C RID: 4188 RVA: 0x00040BCD File Offset: 0x0003EDCD
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

		// Token: 0x0400077D RID: 1917
		public CharacterObject Character;

		// Token: 0x0400077E RID: 1918
		public PartyBase Party;

		// Token: 0x0400077F RID: 1919
		public Settlement Settlement;

		// Token: 0x04000780 RID: 1920
		private readonly bool _canShowQuest = true;

		// Token: 0x04000781 RID: 1921
		private readonly bool _useCivilianEquipment;

		// Token: 0x04000782 RID: 1922
		private readonly Action<GameMenuPartyItemVM> _onSetAsContextMenuActiveItem;

		// Token: 0x04000783 RID: 1923
		private MBBindingList<QuestMarkerVM> _quests;

		// Token: 0x04000784 RID: 1924
		private int _partySize;

		// Token: 0x04000785 RID: 1925
		private int _partyWoundedSize;

		// Token: 0x04000786 RID: 1926
		private int _relation = -101;

		// Token: 0x04000787 RID: 1927
		private ImageIdentifierVM _visual;

		// Token: 0x04000788 RID: 1928
		private ImageIdentifierVM _banner_9;

		// Token: 0x04000789 RID: 1929
		private string _settlementPath;

		// Token: 0x0400078A RID: 1930
		private string _partySizeLbl;

		// Token: 0x0400078B RID: 1931
		private string _nameText;

		// Token: 0x0400078C RID: 1932
		private string _locationText;

		// Token: 0x0400078D RID: 1933
		private string _descriptionText;

		// Token: 0x0400078E RID: 1934
		private string _professionText;

		// Token: 0x0400078F RID: 1935
		private string _powerText;

		// Token: 0x04000790 RID: 1936
		private bool _isIdle;

		// Token: 0x04000791 RID: 1937
		private bool _isPlayer;

		// Token: 0x04000792 RID: 1938
		private bool _isEnemy;

		// Token: 0x04000793 RID: 1939
		private bool _isAlly;

		// Token: 0x04000794 RID: 1940
		private bool _isNeutral;

		// Token: 0x04000795 RID: 1941
		private bool _isHighlightEnabled;

		// Token: 0x04000796 RID: 1942
		private bool _isLeader;

		// Token: 0x04000797 RID: 1943
		private bool _isMergedWithArmy;

		// Token: 0x04000798 RID: 1944
		private bool _isCharacterInPrison;

		// Token: 0x020001E2 RID: 482
		private class QuestMarkerComparer : IComparer<QuestMarkerVM>
		{
			// Token: 0x06002063 RID: 8291 RVA: 0x0006FA80 File Offset: 0x0006DC80
			public int Compare(QuestMarkerVM x, QuestMarkerVM y)
			{
				return x.QuestMarkerType.CompareTo(y.QuestMarkerType);
			}
		}
	}
}
