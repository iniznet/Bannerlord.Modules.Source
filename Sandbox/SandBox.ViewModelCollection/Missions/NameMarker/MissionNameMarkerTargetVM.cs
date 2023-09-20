using System;
using System.Collections.Generic;
using SandBox.Objects;
using SandBox.Objects.AreaMarkers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Missions.NameMarker
{
	// Token: 0x02000025 RID: 37
	public class MissionNameMarkerTargetVM : ViewModel
	{
		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x0000E292 File Offset: 0x0000C492
		// (set) Token: 0x060002E6 RID: 742 RVA: 0x0000E29A File Offset: 0x0000C49A
		public bool IsAdditionalTargetAgent { get; private set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x0000E2A3 File Offset: 0x0000C4A3
		public bool IsMovingTarget { get; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x0000E2AB File Offset: 0x0000C4AB
		public Agent TargetAgent { get; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x0000E2B3 File Offset: 0x0000C4B3
		public Alley TargetAlley { get; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060002EA RID: 746 RVA: 0x0000E2BB File Offset: 0x0000C4BB
		// (set) Token: 0x060002EB RID: 747 RVA: 0x0000E2C3 File Offset: 0x0000C4C3
		public PassageUsePoint TargetPassageUsePoint { get; private set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060002EC RID: 748 RVA: 0x0000E2CC File Offset: 0x0000C4CC
		public Vec3 WorldPosition
		{
			get
			{
				return this._getPosition();
			}
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0000E2DC File Offset: 0x0000C4DC
		public MissionNameMarkerTargetVM(CommonAreaMarker commonAreaMarker)
		{
			this.IsMovingTarget = false;
			this.NameType = "Passage";
			this.IconType = "common_area";
			this.Quests = new MBBindingList<QuestMarkerVM>();
			this.TargetAlley = Hero.MainHero.CurrentSettlement.Alleys[commonAreaMarker.AreaIndex - 1];
			this.UpdateAlleyStatus();
			this._getPosition = () => commonAreaMarker.GetPosition();
			this._getMarkerObjectName = () => commonAreaMarker.GetName().ToString();
			CampaignEvents.AlleyOwnerChanged.AddNonSerializedListener(this, new Action<Alley, Hero, Hero>(this.OnAlleyOwnerChanged));
			this.RefreshValues();
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0000E3F4 File Offset: 0x0000C5F4
		public MissionNameMarkerTargetVM(WorkshopType workshopType, Vec3 signPosition)
		{
			this.IsMovingTarget = false;
			this.NameType = "Passage";
			this.IconType = workshopType.StringId;
			this.Quests = new MBBindingList<QuestMarkerVM>();
			this._getPosition = () => signPosition;
			this._getMarkerObjectName = () => workshopType.Name.ToString();
			this.RefreshValues();
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0000E4D4 File Offset: 0x0000C6D4
		public MissionNameMarkerTargetVM(PassageUsePoint passageUsePoint)
		{
			this.TargetPassageUsePoint = passageUsePoint;
			this.IsMovingTarget = false;
			this.NameType = "Passage";
			this.IconType = passageUsePoint.ToLocation.StringId;
			this.Quests = new MBBindingList<QuestMarkerVM>();
			this._getPosition = () => passageUsePoint.GameEntity.GlobalPosition;
			this._getMarkerObjectName = () => passageUsePoint.ToLocation.Name.ToString();
			this.RefreshValues();
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x0000E5C0 File Offset: 0x0000C7C0
		public MissionNameMarkerTargetVM(Agent agent, bool isAdditionalTargetAgent)
		{
			this.IsMovingTarget = true;
			this.TargetAgent = agent;
			this.NameType = "Normal";
			this.IconType = "character";
			this.IsAdditionalTargetAgent = isAdditionalTargetAgent;
			this.Quests = new MBBindingList<QuestMarkerVM>();
			CharacterObject characterObject = (CharacterObject)agent.Character;
			if (characterObject != null)
			{
				Hero heroObject = characterObject.HeroObject;
				if (heroObject != null && heroObject.IsLord)
				{
					this.IconType = "noble";
					this.NameType = "Noble";
					if (FactionManager.IsAtWarAgainstFaction(characterObject.HeroObject.MapFaction, Hero.MainHero.MapFaction))
					{
						this.NameType = "Enemy";
						this.IsEnemy = true;
					}
					else if (FactionManager.IsAlliedWithFaction(characterObject.HeroObject.MapFaction, Hero.MainHero.MapFaction))
					{
						this.NameType = "Friendly";
						this.IsFriendly = true;
					}
				}
				if (characterObject.HeroObject != null && characterObject.HeroObject.IsPrisoner)
				{
					this.IconType = "prisoner";
				}
				if (agent.IsHuman && characterObject.IsHero && agent != Agent.Main && !this.IsAdditionalTargetAgent)
				{
					this.UpdateQuestStatus();
				}
				CharacterObject characterObject2 = characterObject;
				Settlement currentSettlement = Settlement.CurrentSettlement;
				object obj;
				if (currentSettlement == null)
				{
					obj = null;
				}
				else
				{
					CultureObject culture = currentSettlement.Culture;
					obj = ((culture != null) ? culture.Barber : null);
				}
				if (characterObject2 == obj)
				{
					this.IconType = "barber";
				}
				else
				{
					CharacterObject characterObject3 = characterObject;
					Settlement currentSettlement2 = Settlement.CurrentSettlement;
					object obj2;
					if (currentSettlement2 == null)
					{
						obj2 = null;
					}
					else
					{
						CultureObject culture2 = currentSettlement2.Culture;
						obj2 = ((culture2 != null) ? culture2.Blacksmith : null);
					}
					if (characterObject3 == obj2)
					{
						this.IconType = "blacksmith";
					}
					else
					{
						CharacterObject characterObject4 = characterObject;
						Settlement currentSettlement3 = Settlement.CurrentSettlement;
						object obj3;
						if (currentSettlement3 == null)
						{
							obj3 = null;
						}
						else
						{
							CultureObject culture3 = currentSettlement3.Culture;
							obj3 = ((culture3 != null) ? culture3.TavernGamehost : null);
						}
						if (characterObject4 == obj3)
						{
							this.IconType = "game_host";
						}
						else if (characterObject.StringId == "sp_hermit")
						{
							this.IconType = "hermit";
						}
					}
				}
			}
			this._getPosition = delegate
			{
				Vec3 position = agent.Position;
				position.z = agent.GetEyeGlobalPosition().Z;
				return position;
			};
			this._getMarkerObjectName = () => agent.Name;
			this.RefreshValues();
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0000E838 File Offset: 0x0000CA38
		public MissionNameMarkerTargetVM(Vec3 position, string name, string iconType)
		{
			this.NameType = "Passage";
			this.IconType = iconType;
			this.Quests = new MBBindingList<QuestMarkerVM>();
			this._getPosition = () => position;
			this._getMarkerObjectName = () => name;
			this.RefreshValues();
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0000E906 File Offset: 0x0000CB06
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._getMarkerObjectName();
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x0000E91F File Offset: 0x0000CB1F
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.AlleyOccupiedByPlayer.ClearListeners(this);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0000E932 File Offset: 0x0000CB32
		private void OnAlleyOwnerChanged(Alley alley, Hero newOwner, Hero oldOwner)
		{
			if (this.TargetAlley == alley && (newOwner == Hero.MainHero || oldOwner == Hero.MainHero))
			{
				this.UpdateAlleyStatus();
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000E954 File Offset: 0x0000CB54
		private void UpdateAlleyStatus()
		{
			if (this.TargetAlley != null)
			{
				Hero owner = this.TargetAlley.Owner;
				if (owner != null)
				{
					if (owner == Hero.MainHero)
					{
						this.NameType = "Friendly";
						this.IsFriendly = true;
						this.IsEnemy = false;
						return;
					}
					this.NameType = "Passage";
					this.IsFriendly = false;
					this.IsEnemy = true;
					return;
				}
				else
				{
					this.NameType = "Normal";
					this.IsFriendly = false;
					this.IsEnemy = false;
				}
			}
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0000E9D0 File Offset: 0x0000CBD0
		public void UpdateQuestStatus(SandBoxUIHelper.IssueQuestFlags issueQuestFlags)
		{
			SandBoxUIHelper.IssueQuestFlags[] issueQuestFlagsValues = SandBoxUIHelper.IssueQuestFlagsValues;
			for (int i = 0; i < issueQuestFlagsValues.Length; i++)
			{
				SandBoxUIHelper.IssueQuestFlags questFlag = issueQuestFlagsValues[i];
				if (questFlag != SandBoxUIHelper.IssueQuestFlags.None && (issueQuestFlags & questFlag) != SandBoxUIHelper.IssueQuestFlags.None && LinQuick.AllQ<QuestMarkerVM>(this.Quests, (QuestMarkerVM q) => q.IssueQuestFlag != questFlag))
				{
					this.Quests.Add(new QuestMarkerVM(questFlag));
					if ((questFlag & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) != SandBoxUIHelper.IssueQuestFlags.None && (questFlag & SandBoxUIHelper.IssueQuestFlags.AvailableIssue) != SandBoxUIHelper.IssueQuestFlags.None && (questFlag & SandBoxUIHelper.IssueQuestFlags.TrackedIssue) != SandBoxUIHelper.IssueQuestFlags.None)
					{
						this.IsTracked = true;
					}
					else if ((questFlag & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) != SandBoxUIHelper.IssueQuestFlags.None && (questFlag & SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest) != SandBoxUIHelper.IssueQuestFlags.None && (questFlag & SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest) != SandBoxUIHelper.IssueQuestFlags.None)
					{
						this.IsQuestMainStory = true;
					}
				}
			}
			this.Quests.Sort(new MissionNameMarkerTargetVM.QuestMarkerComparer());
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000EAA8 File Offset: 0x0000CCA8
		public void UpdateQuestStatus()
		{
			this.Quests.Clear();
			SandBoxUIHelper.IssueQuestFlags issueQuestFlags = SandBoxUIHelper.IssueQuestFlags.None;
			if (this.TargetAgent != null && (CharacterObject)this.TargetAgent.Character != null && ((CharacterObject)this.TargetAgent.Character).HeroObject != null)
			{
				List<ValueTuple<SandBoxUIHelper.IssueQuestFlags, TextObject, TextObject>> questStateOfHero = SandBoxUIHelper.GetQuestStateOfHero(((CharacterObject)this.TargetAgent.Character).HeroObject);
				for (int i = 0; i < questStateOfHero.Count; i++)
				{
					issueQuestFlags |= questStateOfHero[i].Item1;
				}
			}
			Agent targetAgent = this.TargetAgent;
			if (targetAgent != null && !targetAgent.IsHero)
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				bool flag;
				if (currentSettlement == null)
				{
					flag = false;
				}
				else
				{
					LocationComplex locationComplex = currentSettlement.LocationComplex;
					bool? flag2;
					if (locationComplex == null)
					{
						flag2 = null;
					}
					else
					{
						LocationCharacter locationCharacter = locationComplex.FindCharacter(this.TargetAgent);
						flag2 = ((locationCharacter != null) ? new bool?(locationCharacter.IsVisualTracked) : null);
					}
					bool? flag3 = flag2;
					bool flag4 = true;
					flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
				}
				if (flag)
				{
					issueQuestFlags |= SandBoxUIHelper.IssueQuestFlags.TrackedIssue;
				}
			}
			foreach (SandBoxUIHelper.IssueQuestFlags issueQuestFlags2 in SandBoxUIHelper.IssueQuestFlagsValues)
			{
				if (issueQuestFlags2 != SandBoxUIHelper.IssueQuestFlags.None && (issueQuestFlags & issueQuestFlags2) != SandBoxUIHelper.IssueQuestFlags.None)
				{
					this.Quests.Add(new QuestMarkerVM(issueQuestFlags2));
					if ((issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) != SandBoxUIHelper.IssueQuestFlags.None && (issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.AvailableIssue) != SandBoxUIHelper.IssueQuestFlags.None && (issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.TrackedIssue) != SandBoxUIHelper.IssueQuestFlags.None)
					{
						this.IsTracked = true;
					}
					else if ((issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.ActiveIssue) != SandBoxUIHelper.IssueQuestFlags.None && (issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.ActiveStoryQuest) != SandBoxUIHelper.IssueQuestFlags.None && (issueQuestFlags2 & SandBoxUIHelper.IssueQuestFlags.TrackedStoryQuest) != SandBoxUIHelper.IssueQuestFlags.None)
					{
						this.IsQuestMainStory = true;
					}
				}
			}
			this.Quests.Sort(new MissionNameMarkerTargetVM.QuestMarkerComparer());
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060002F8 RID: 760 RVA: 0x0000EC2A File Offset: 0x0000CE2A
		// (set) Token: 0x060002F9 RID: 761 RVA: 0x0000EC32 File Offset: 0x0000CE32
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

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060002FA RID: 762 RVA: 0x0000EC50 File Offset: 0x0000CE50
		// (set) Token: 0x060002FB RID: 763 RVA: 0x0000EC58 File Offset: 0x0000CE58
		[DataSourceProperty]
		public Vec2 ScreenPosition
		{
			get
			{
				return this._screenPosition;
			}
			set
			{
				if (value.x != this._screenPosition.x || value.y != this._screenPosition.y)
				{
					this._screenPosition = value;
					base.OnPropertyChangedWithValue(value, "ScreenPosition");
				}
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060002FC RID: 764 RVA: 0x0000EC93 File Offset: 0x0000CE93
		// (set) Token: 0x060002FD RID: 765 RVA: 0x0000EC9B File Offset: 0x0000CE9B
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060002FE RID: 766 RVA: 0x0000ECBE File Offset: 0x0000CEBE
		// (set) Token: 0x060002FF RID: 767 RVA: 0x0000ECC6 File Offset: 0x0000CEC6
		[DataSourceProperty]
		public string IconType
		{
			get
			{
				return this._iconType;
			}
			set
			{
				if (value != this._iconType)
				{
					this._iconType = value;
					base.OnPropertyChangedWithValue<string>(value, "IconType");
				}
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000300 RID: 768 RVA: 0x0000ECE9 File Offset: 0x0000CEE9
		// (set) Token: 0x06000301 RID: 769 RVA: 0x0000ECF1 File Offset: 0x0000CEF1
		[DataSourceProperty]
		public string NameType
		{
			get
			{
				return this._nameType;
			}
			set
			{
				if (value != this._nameType)
				{
					this._nameType = value;
					base.OnPropertyChangedWithValue<string>(value, "NameType");
				}
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000302 RID: 770 RVA: 0x0000ED14 File Offset: 0x0000CF14
		// (set) Token: 0x06000303 RID: 771 RVA: 0x0000ED1C File Offset: 0x0000CF1C
		[DataSourceProperty]
		public int Distance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if (value != this._distance)
				{
					this._distance = value;
					base.OnPropertyChangedWithValue(value, "Distance");
				}
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000304 RID: 772 RVA: 0x0000ED3A File Offset: 0x0000CF3A
		// (set) Token: 0x06000305 RID: 773 RVA: 0x0000ED42 File Offset: 0x0000CF42
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000306 RID: 774 RVA: 0x0000ED60 File Offset: 0x0000CF60
		// (set) Token: 0x06000307 RID: 775 RVA: 0x0000ED68 File Offset: 0x0000CF68
		[DataSourceProperty]
		public bool IsTracked
		{
			get
			{
				return this._isTracked;
			}
			set
			{
				if (value != this._isTracked)
				{
					this._isTracked = value;
					base.OnPropertyChangedWithValue(value, "IsTracked");
				}
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000308 RID: 776 RVA: 0x0000ED86 File Offset: 0x0000CF86
		// (set) Token: 0x06000309 RID: 777 RVA: 0x0000ED8E File Offset: 0x0000CF8E
		[DataSourceProperty]
		public bool IsQuestMainStory
		{
			get
			{
				return this._isQuestMainStory;
			}
			set
			{
				if (value != this._isQuestMainStory)
				{
					this._isQuestMainStory = value;
					base.OnPropertyChangedWithValue(value, "IsQuestMainStory");
				}
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x0600030A RID: 778 RVA: 0x0000EDAC File Offset: 0x0000CFAC
		// (set) Token: 0x0600030B RID: 779 RVA: 0x0000EDB4 File Offset: 0x0000CFB4
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

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x0600030C RID: 780 RVA: 0x0000EDD2 File Offset: 0x0000CFD2
		// (set) Token: 0x0600030D RID: 781 RVA: 0x0000EDDA File Offset: 0x0000CFDA
		[DataSourceProperty]
		public bool IsFriendly
		{
			get
			{
				return this._isFriendly;
			}
			set
			{
				if (value != this._isFriendly)
				{
					this._isFriendly = value;
					base.OnPropertyChangedWithValue(value, "IsFriendly");
				}
			}
		}

		// Token: 0x04000171 RID: 369
		public const string NameTypeNeutral = "Normal";

		// Token: 0x04000172 RID: 370
		public const string NameTypeFriendly = "Friendly";

		// Token: 0x04000173 RID: 371
		public const string NameTypeEnemy = "Enemy";

		// Token: 0x04000174 RID: 372
		public const string NameTypeNoble = "Noble";

		// Token: 0x04000175 RID: 373
		public const string NameTypePassage = "Passage";

		// Token: 0x04000176 RID: 374
		public const string NameTypeEnemyPassage = "Passage";

		// Token: 0x04000177 RID: 375
		public const string IconTypeCommonArea = "common_area";

		// Token: 0x04000178 RID: 376
		public const string IconTypeCharacter = "character";

		// Token: 0x04000179 RID: 377
		public const string IconTypePrisoner = "prisoner";

		// Token: 0x0400017A RID: 378
		public const string IconTypeNoble = "noble";

		// Token: 0x0400017B RID: 379
		public const string IconTypeBarber = "barber";

		// Token: 0x0400017C RID: 380
		public const string IconTypeBlacksmith = "blacksmith";

		// Token: 0x0400017D RID: 381
		public const string IconTypeGameHost = "game_host";

		// Token: 0x0400017E RID: 382
		public const string IconTypeHermit = "hermit";

		// Token: 0x0400017F RID: 383
		private Func<Vec3> _getPosition = () => Vec3.Zero;

		// Token: 0x04000180 RID: 384
		private Func<string> _getMarkerObjectName = () => string.Empty;

		// Token: 0x04000186 RID: 390
		private MBBindingList<QuestMarkerVM> _quests;

		// Token: 0x04000187 RID: 391
		private Vec2 _screenPosition;

		// Token: 0x04000188 RID: 392
		private int _distance;

		// Token: 0x04000189 RID: 393
		private string _name;

		// Token: 0x0400018A RID: 394
		private string _iconType = string.Empty;

		// Token: 0x0400018B RID: 395
		private string _nameType = string.Empty;

		// Token: 0x0400018C RID: 396
		private bool _isEnabled;

		// Token: 0x0400018D RID: 397
		private bool _isTracked;

		// Token: 0x0400018E RID: 398
		private bool _isQuestMainStory;

		// Token: 0x0400018F RID: 399
		private bool _isEnemy;

		// Token: 0x04000190 RID: 400
		private bool _isFriendly;

		// Token: 0x02000080 RID: 128
		private class QuestMarkerComparer : IComparer<QuestMarkerVM>
		{
			// Token: 0x0600052C RID: 1324 RVA: 0x000143F8 File Offset: 0x000125F8
			public int Compare(QuestMarkerVM x, QuestMarkerVM y)
			{
				return x.QuestMarkerType.CompareTo(y.QuestMarkerType);
			}
		}
	}
}
