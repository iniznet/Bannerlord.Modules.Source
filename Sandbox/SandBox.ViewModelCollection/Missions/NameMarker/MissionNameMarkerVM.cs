using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Missions.MissionLogics.Towns;
using SandBox.Objects;
using SandBox.Objects.AreaMarkers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Missions.NameMarker
{
	public class MissionNameMarkerVM : ViewModel
	{
		public bool IsTargetsAdded { get; private set; }

		public MissionNameMarkerVM(Mission mission, Camera missionCamera, Dictionary<Agent, SandBoxUIHelper.IssueQuestFlags> additionalTargetAgents, Dictionary<string, ValueTuple<Vec3, string, string>> additionalGenericTargets)
		{
			this.Targets = new MBBindingList<MissionNameMarkerTargetVM>();
			this._distanceComparer = new MissionNameMarkerVM.MarkerDistanceComparer();
			this._missionCamera = missionCamera;
			this._additionalTargetAgents = additionalTargetAgents;
			this._additionalGenericTargets = additionalGenericTargets;
			this._genericTargets = new Dictionary<string, MissionNameMarkerTargetVM>();
			this._mission = mission;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Targets.ApplyActionOnAllItems(delegate(MissionNameMarkerTargetVM x)
			{
				x.RefreshValues();
			});
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Targets.ApplyActionOnAllItems(delegate(MissionNameMarkerTargetVM x)
			{
				x.OnFinalize();
			});
		}

		public void Tick(float dt)
		{
			if (!this.IsTargetsAdded)
			{
				if (this._mission.MainAgent != null)
				{
					if (this._additionalTargetAgents != null)
					{
						foreach (KeyValuePair<Agent, SandBoxUIHelper.IssueQuestFlags> keyValuePair in this._additionalTargetAgents)
						{
							this.AddAgentTarget(keyValuePair.Key, true);
							this.UpdateAdditionalTargetAgentQuestStatus(keyValuePair.Key, keyValuePair.Value);
						}
					}
					if (this._additionalGenericTargets != null)
					{
						foreach (KeyValuePair<string, ValueTuple<Vec3, string, string>> keyValuePair2 in this._additionalGenericTargets)
						{
							this.AddGenericMarker(keyValuePair2.Key, keyValuePair2.Value.Item1, keyValuePair2.Value.Item2, keyValuePair2.Value.Item3);
						}
					}
					foreach (Agent agent in this._mission.Agents)
					{
						this.AddAgentTarget(agent, false);
					}
					if (Hero.MainHero.CurrentSettlement != null)
					{
						List<CommonAreaMarker> list = (from x in MBExtensions.FindAllWithType<CommonAreaMarker>(this._mission.ActiveMissionObjects)
							where x.GameEntity.HasTag("alley_marker")
							select x).ToList<CommonAreaMarker>();
						if (Hero.MainHero.CurrentSettlement.Alleys.Count > 0)
						{
							foreach (CommonAreaMarker commonAreaMarker in list)
							{
								Alley alley = commonAreaMarker.GetAlley();
								if (alley != null && alley.Owner != null)
								{
									this.Targets.Add(new MissionNameMarkerTargetVM(commonAreaMarker));
								}
							}
						}
						foreach (PassageUsePoint passageUsePoint in from passage in MBExtensions.FindAllWithType<PassageUsePoint>(this._mission.ActiveMissionObjects).ToList<PassageUsePoint>()
							where passage.ToLocation != null && !this.PassagePointFilter.Exists((string s) => passage.ToLocation.Name.Contains(s))
							select passage)
						{
							if (!passageUsePoint.ToLocation.CanBeReserved || passageUsePoint.ToLocation.IsReserved)
							{
								this.Targets.Add(new MissionNameMarkerTargetVM(passageUsePoint));
							}
						}
						if (this._mission.HasMissionBehavior<WorkshopMissionHandler>())
						{
							foreach (Tuple<Workshop, GameEntity> tuple in from s in this._mission.GetMissionBehavior<WorkshopMissionHandler>().WorkshopSignEntities.ToList<Tuple<Workshop, GameEntity>>()
								where s.Item1.WorkshopType != null
								select s)
							{
								this.Targets.Add(new MissionNameMarkerTargetVM(tuple.Item1.WorkshopType, tuple.Item2.GlobalPosition - this._defaultHeightOffset));
							}
						}
					}
				}
				this.IsTargetsAdded = true;
			}
			if (this.IsEnabled)
			{
				this.UpdateTargetScreenPositions();
				this._fadeOutTimerStarted = false;
				this._fadeOutTimer = 0f;
				this._prevEnabledState = this.IsEnabled;
			}
			else
			{
				if (this._prevEnabledState)
				{
					this._fadeOutTimerStarted = true;
				}
				if (this._fadeOutTimerStarted)
				{
					this._fadeOutTimer += dt;
				}
				if (this._fadeOutTimer < 2f)
				{
					this.UpdateTargetScreenPositions();
				}
				else
				{
					this._fadeOutTimerStarted = false;
				}
			}
			this._prevEnabledState = this.IsEnabled;
		}

		private void UpdateTargetScreenPositions()
		{
			foreach (MissionNameMarkerTargetVM missionNameMarkerTargetVM in this.Targets)
			{
				float num = -100f;
				float num2 = -100f;
				float num3 = 0f;
				Vec3 vec = ((missionNameMarkerTargetVM.TargetAgent != null) ? this._agentHeightOffset : this._defaultHeightOffset);
				MBWindowManager.WorldToScreenInsideUsableArea(this._missionCamera, missionNameMarkerTargetVM.WorldPosition + vec, ref num, ref num2, ref num3);
				if (num3 > 0f)
				{
					missionNameMarkerTargetVM.ScreenPosition = new Vec2(num, num2);
					missionNameMarkerTargetVM.Distance = (int)(missionNameMarkerTargetVM.WorldPosition - this._missionCamera.Position).Length;
				}
				else
				{
					missionNameMarkerTargetVM.Distance = -1;
					missionNameMarkerTargetVM.ScreenPosition = new Vec2(-100f, -100f);
				}
			}
			this.Targets.Sort(this._distanceComparer);
		}

		public void OnConversationEnd()
		{
			foreach (Agent agent in this._mission.Agents)
			{
				this.AddAgentTarget(agent, false);
			}
			foreach (MissionNameMarkerTargetVM missionNameMarkerTargetVM in this.Targets)
			{
				if (!missionNameMarkerTargetVM.IsAdditionalTargetAgent)
				{
					missionNameMarkerTargetVM.UpdateQuestStatus();
				}
			}
		}

		public void OnAgentBuild(Agent agent)
		{
			this.AddAgentTarget(agent, false);
		}

		public void OnAgentRemoved(Agent agent)
		{
			this.RemoveAgentTarget(agent);
		}

		public void OnAgentDeleted(Agent agent)
		{
			this.RemoveAgentTarget(agent);
		}

		public void UpdateAdditionalTargetAgentQuestStatus(Agent agent, SandBoxUIHelper.IssueQuestFlags issueQuestFlags)
		{
			MissionNameMarkerTargetVM missionNameMarkerTargetVM = this.Targets.FirstOrDefault((MissionNameMarkerTargetVM t) => t.TargetAgent == agent);
			if (missionNameMarkerTargetVM == null)
			{
				return;
			}
			missionNameMarkerTargetVM.UpdateQuestStatus(issueQuestFlags);
		}

		public void AddGenericMarker(string markerIdentifier, Vec3 markerPosition, string name, string iconType)
		{
			MissionNameMarkerTargetVM missionNameMarkerTargetVM;
			if (this._genericTargets.TryGetValue(markerIdentifier, out missionNameMarkerTargetVM))
			{
				Debug.FailedAssert("Marker with identifier: " + markerIdentifier + " already exists", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\Missions\\NameMarker\\MissionNameMarkerVM.cs", "AddGenericMarker", 229);
				return;
			}
			MissionNameMarkerTargetVM missionNameMarkerTargetVM2 = new MissionNameMarkerTargetVM(markerPosition, name, iconType);
			this._genericTargets.Add(markerIdentifier, missionNameMarkerTargetVM2);
			this.Targets.Add(missionNameMarkerTargetVM2);
		}

		public void RemoveGenericMarker(string markerIdentifier)
		{
			MissionNameMarkerTargetVM missionNameMarkerTargetVM;
			if (this._genericTargets.TryGetValue(markerIdentifier, out missionNameMarkerTargetVM))
			{
				this._genericTargets.Remove(markerIdentifier);
				this.Targets.Remove(missionNameMarkerTargetVM);
				return;
			}
			Debug.FailedAssert("Marker with identifier: " + markerIdentifier + " does not exist", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\Missions\\NameMarker\\MissionNameMarkerVM.cs", "RemoveGenericMarker", 248);
		}

		public void AddAgentTarget(Agent agent, bool isAdditional = false)
		{
			Agent agent2 = agent;
			if (((agent2 != null) ? agent2.Character : null) != null && agent != Agent.Main && agent.IsActive() && !this.Targets.Any((MissionNameMarkerTargetVM t) => t.TargetAgent == agent))
			{
				bool flag5;
				if (!isAdditional && !agent.Character.IsHero)
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
							LocationCharacter locationCharacter = locationComplex.FindCharacter(agent);
							flag2 = ((locationCharacter != null) ? new bool?(locationCharacter.IsVisualTracked) : null);
						}
						bool? flag3 = flag2;
						bool flag4 = true;
						flag = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
					}
					CharacterObject characterObject;
					if (!flag && ((characterObject = agent.Character as CharacterObject) == null || (characterObject.Occupation != 9 && characterObject.Occupation != 1)))
					{
						BasicCharacterObject character = agent.Character;
						Settlement currentSettlement2 = Settlement.CurrentSettlement;
						object obj;
						if (currentSettlement2 == null)
						{
							obj = null;
						}
						else
						{
							CultureObject culture = currentSettlement2.Culture;
							obj = ((culture != null) ? culture.Blacksmith : null);
						}
						if (character != obj)
						{
							BasicCharacterObject character2 = agent.Character;
							Settlement currentSettlement3 = Settlement.CurrentSettlement;
							object obj2;
							if (currentSettlement3 == null)
							{
								obj2 = null;
							}
							else
							{
								CultureObject culture2 = currentSettlement3.Culture;
								obj2 = ((culture2 != null) ? culture2.Barber : null);
							}
							if (character2 != obj2)
							{
								BasicCharacterObject character3 = agent.Character;
								Settlement currentSettlement4 = Settlement.CurrentSettlement;
								object obj3;
								if (currentSettlement4 == null)
								{
									obj3 = null;
								}
								else
								{
									CultureObject culture3 = currentSettlement4.Culture;
									obj3 = ((culture3 != null) ? culture3.TavernGamehost : null);
								}
								if (character3 != obj3)
								{
									flag5 = agent.Character.StringId == "sp_hermit";
									goto IL_1A3;
								}
							}
						}
					}
				}
				flag5 = true;
				IL_1A3:
				if (flag5)
				{
					MissionNameMarkerTargetVM missionNameMarkerTargetVM = new MissionNameMarkerTargetVM(agent, isAdditional);
					this.Targets.Add(missionNameMarkerTargetVM);
				}
			}
		}

		public void RemoveAgentTarget(Agent agent)
		{
			if (this.Targets.SingleOrDefault((MissionNameMarkerTargetVM t) => t.TargetAgent == agent) != null)
			{
				this.Targets.Remove(this.Targets.Single((MissionNameMarkerTargetVM t) => t.TargetAgent == agent));
			}
		}

		private void UpdateTargetStates(bool state)
		{
			foreach (MissionNameMarkerTargetVM missionNameMarkerTargetVM in this.Targets)
			{
				missionNameMarkerTargetVM.IsEnabled = state;
			}
		}

		[DataSourceProperty]
		public MBBindingList<MissionNameMarkerTargetVM> Targets
		{
			get
			{
				return this._targets;
			}
			set
			{
				if (value != this._targets)
				{
					this._targets = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionNameMarkerTargetVM>>(value, "Targets");
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
					this.UpdateTargetStates(value);
					Game.Current.EventManager.TriggerEvent<MissionNameMarkerToggleEvent>(new MissionNameMarkerToggleEvent(value));
				}
			}
		}

		private readonly Camera _missionCamera;

		private readonly Mission _mission;

		private Vec3 _agentHeightOffset = new Vec3(0f, 0f, 0.35f, -1f);

		private Vec3 _defaultHeightOffset = new Vec3(0f, 0f, 2f, -1f);

		private bool _prevEnabledState;

		private bool _fadeOutTimerStarted;

		private float _fadeOutTimer;

		private Dictionary<Agent, SandBoxUIHelper.IssueQuestFlags> _additionalTargetAgents;

		private Dictionary<string, ValueTuple<Vec3, string, string>> _additionalGenericTargets;

		private Dictionary<string, MissionNameMarkerTargetVM> _genericTargets;

		private readonly MissionNameMarkerVM.MarkerDistanceComparer _distanceComparer;

		private readonly List<string> PassagePointFilter = new List<string> { "Empty Shop" };

		private MBBindingList<MissionNameMarkerTargetVM> _targets;

		private bool _isEnabled;

		private class MarkerDistanceComparer : IComparer<MissionNameMarkerTargetVM>
		{
			public int Compare(MissionNameMarkerTargetVM x, MissionNameMarkerTargetVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
