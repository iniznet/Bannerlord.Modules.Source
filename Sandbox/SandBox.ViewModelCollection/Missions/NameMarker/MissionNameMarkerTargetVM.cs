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
	public class MissionNameMarkerTargetVM : ViewModel
	{
		public bool IsAdditionalTargetAgent { get; private set; }

		public bool IsMovingTarget { get; }

		public Agent TargetAgent { get; }

		public Alley TargetAlley { get; }

		public PassageUsePoint TargetPassageUsePoint { get; private set; }

		public Vec3 WorldPosition
		{
			get
			{
				return this._getPosition();
			}
		}

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
				if (agent.IsHuman && agent != Agent.Main && !this.IsAdditionalTargetAgent)
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

		public MissionNameMarkerTargetVM(Vec3 position, string name, string iconType)
		{
			this.NameType = "Passage";
			this.IconType = iconType;
			this.Quests = new MBBindingList<QuestMarkerVM>();
			this._getPosition = () => position;
			this._getMarkerObjectName = () => name;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._getMarkerObjectName();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.AlleyOwnerChanged.ClearListeners(this);
		}

		private void OnAlleyOwnerChanged(Alley alley, Hero newOwner, Hero oldOwner)
		{
			if (this.TargetAlley == alley && (newOwner == Hero.MainHero || oldOwner == Hero.MainHero))
			{
				this.UpdateAlleyStatus();
			}
		}

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

		public void UpdateQuestStatus()
		{
			this.Quests.Clear();
			SandBoxUIHelper.IssueQuestFlags issueQuestFlags = SandBoxUIHelper.IssueQuestFlags.None;
			Agent targetAgent = this.TargetAgent;
			CharacterObject characterObject = (CharacterObject)((targetAgent != null) ? targetAgent.Character : null);
			Hero hero = ((characterObject != null) ? characterObject.HeroObject : null);
			if (hero != null)
			{
				List<ValueTuple<SandBoxUIHelper.IssueQuestFlags, TextObject, TextObject>> questStateOfHero = SandBoxUIHelper.GetQuestStateOfHero(hero);
				for (int i = 0; i < questStateOfHero.Count; i++)
				{
					issueQuestFlags |= questStateOfHero[i].Item1;
				}
			}
			if (this.TargetAgent != null)
			{
				CharacterObject characterObject2 = this.TargetAgent.Character as CharacterObject;
				Hero hero2;
				if (characterObject2 == null)
				{
					hero2 = null;
				}
				else
				{
					Hero heroObject = characterObject2.HeroObject;
					if (heroObject == null)
					{
						hero2 = null;
					}
					else
					{
						Clan clan = heroObject.Clan;
						hero2 = ((clan != null) ? clan.Leader : null);
					}
				}
				if (hero2 != Hero.MainHero)
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

		public const string NameTypeNeutral = "Normal";

		public const string NameTypeFriendly = "Friendly";

		public const string NameTypeEnemy = "Enemy";

		public const string NameTypeNoble = "Noble";

		public const string NameTypePassage = "Passage";

		public const string NameTypeEnemyPassage = "Passage";

		public const string IconTypeCommonArea = "common_area";

		public const string IconTypeCharacter = "character";

		public const string IconTypePrisoner = "prisoner";

		public const string IconTypeNoble = "noble";

		public const string IconTypeBarber = "barber";

		public const string IconTypeBlacksmith = "blacksmith";

		public const string IconTypeGameHost = "game_host";

		public const string IconTypeHermit = "hermit";

		private Func<Vec3> _getPosition = () => Vec3.Zero;

		private Func<string> _getMarkerObjectName = () => string.Empty;

		private MBBindingList<QuestMarkerVM> _quests;

		private Vec2 _screenPosition;

		private int _distance;

		private string _name;

		private string _iconType = string.Empty;

		private string _nameType = string.Empty;

		private bool _isEnabled;

		private bool _isTracked;

		private bool _isQuestMainStory;

		private bool _isEnemy;

		private bool _isFriendly;

		private class QuestMarkerComparer : IComparer<QuestMarkerVM>
		{
			public int Compare(QuestMarkerVM x, QuestMarkerVM y)
			{
				return x.QuestMarkerType.CompareTo(y.QuestMarkerType);
			}
		}
	}
}
