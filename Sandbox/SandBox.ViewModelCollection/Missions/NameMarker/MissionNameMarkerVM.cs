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
	// Token: 0x02000026 RID: 38
	public class MissionNameMarkerVM : ViewModel
	{
		// Token: 0x170000FD RID: 253
		// (get) Token: 0x0600030E RID: 782 RVA: 0x0000EDF8 File Offset: 0x0000CFF8
		// (set) Token: 0x0600030F RID: 783 RVA: 0x0000EE00 File Offset: 0x0000D000
		public bool IsTargetsAdded { get; private set; }

		// Token: 0x06000310 RID: 784 RVA: 0x0000EE0C File Offset: 0x0000D00C
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

		// Token: 0x06000311 RID: 785 RVA: 0x0000EEB1 File Offset: 0x0000D0B1
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Targets.ApplyActionOnAllItems(delegate(MissionNameMarkerTargetVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0000EEE3 File Offset: 0x0000D0E3
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Targets.ApplyActionOnAllItems(delegate(MissionNameMarkerTargetVM x)
			{
				x.OnFinalize();
			});
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0000EF18 File Offset: 0x0000D118
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

		// Token: 0x06000314 RID: 788 RVA: 0x0000F2E8 File Offset: 0x0000D4E8
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

		// Token: 0x06000315 RID: 789 RVA: 0x0000F3EC File Offset: 0x0000D5EC
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

		// Token: 0x06000316 RID: 790 RVA: 0x0000F488 File Offset: 0x0000D688
		public void OnAgentBuild(Agent agent)
		{
			this.AddAgentTarget(agent, false);
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0000F492 File Offset: 0x0000D692
		public void OnAgentRemoved(Agent agent)
		{
			this.RemoveAgentTarget(agent);
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000F49B File Offset: 0x0000D69B
		public void OnAgentDeleted(Agent agent)
		{
			this.RemoveAgentTarget(agent);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0000F4A4 File Offset: 0x0000D6A4
		public void UpdateAdditionalTargetAgentQuestStatus(Agent agent, SandBoxUIHelper.IssueQuestFlags issueQuestFlags)
		{
			MissionNameMarkerTargetVM missionNameMarkerTargetVM = this.Targets.FirstOrDefault((MissionNameMarkerTargetVM t) => t.TargetAgent == agent);
			if (missionNameMarkerTargetVM == null)
			{
				return;
			}
			missionNameMarkerTargetVM.UpdateQuestStatus(issueQuestFlags);
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0000F4E0 File Offset: 0x0000D6E0
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

		// Token: 0x0600031B RID: 795 RVA: 0x0000F548 File Offset: 0x0000D748
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

		// Token: 0x0600031C RID: 796 RVA: 0x0000F5A4 File Offset: 0x0000D7A4
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

		// Token: 0x0600031D RID: 797 RVA: 0x0000F774 File Offset: 0x0000D974
		public void RemoveAgentTarget(Agent agent)
		{
			if (this.Targets.SingleOrDefault((MissionNameMarkerTargetVM t) => t.TargetAgent == agent) != null)
			{
				this.Targets.Remove(this.Targets.Single((MissionNameMarkerTargetVM t) => t.TargetAgent == agent));
			}
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0000F7CC File Offset: 0x0000D9CC
		private void UpdateTargetStates(bool state)
		{
			foreach (MissionNameMarkerTargetVM missionNameMarkerTargetVM in this.Targets)
			{
				missionNameMarkerTargetVM.IsEnabled = state;
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x0600031F RID: 799 RVA: 0x0000F818 File Offset: 0x0000DA18
		// (set) Token: 0x06000320 RID: 800 RVA: 0x0000F820 File Offset: 0x0000DA20
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

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000321 RID: 801 RVA: 0x0000F83E File Offset: 0x0000DA3E
		// (set) Token: 0x06000322 RID: 802 RVA: 0x0000F846 File Offset: 0x0000DA46
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

		// Token: 0x04000192 RID: 402
		private readonly Camera _missionCamera;

		// Token: 0x04000193 RID: 403
		private readonly Mission _mission;

		// Token: 0x04000194 RID: 404
		private Vec3 _agentHeightOffset = new Vec3(0f, 0f, 0.35f, -1f);

		// Token: 0x04000195 RID: 405
		private Vec3 _defaultHeightOffset = new Vec3(0f, 0f, 2f, -1f);

		// Token: 0x04000196 RID: 406
		private bool _prevEnabledState;

		// Token: 0x04000197 RID: 407
		private bool _fadeOutTimerStarted;

		// Token: 0x04000198 RID: 408
		private float _fadeOutTimer;

		// Token: 0x04000199 RID: 409
		private Dictionary<Agent, SandBoxUIHelper.IssueQuestFlags> _additionalTargetAgents;

		// Token: 0x0400019A RID: 410
		private Dictionary<string, ValueTuple<Vec3, string, string>> _additionalGenericTargets;

		// Token: 0x0400019B RID: 411
		private Dictionary<string, MissionNameMarkerTargetVM> _genericTargets;

		// Token: 0x0400019C RID: 412
		private readonly MissionNameMarkerVM.MarkerDistanceComparer _distanceComparer;

		// Token: 0x0400019D RID: 413
		private readonly List<string> PassagePointFilter = new List<string> { "Empty Shop" };

		// Token: 0x0400019E RID: 414
		private MBBindingList<MissionNameMarkerTargetVM> _targets;

		// Token: 0x0400019F RID: 415
		private bool _isEnabled;

		// Token: 0x02000088 RID: 136
		private class MarkerDistanceComparer : IComparer<MissionNameMarkerTargetVM>
		{
			// Token: 0x0600054B RID: 1355 RVA: 0x00014574 File Offset: 0x00012774
			public int Compare(MissionNameMarkerTargetVM x, MissionNameMarkerTargetVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
