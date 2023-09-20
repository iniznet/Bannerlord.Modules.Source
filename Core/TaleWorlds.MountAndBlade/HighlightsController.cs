using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class HighlightsController : MissionLogic
	{
		private protected static List<HighlightsController.HighlightType> HighlightTypes { protected get; private set; }

		public static bool IsHighlightsInitialized { get; private set; }

		public bool IsAnyHighlightSaved
		{
			get
			{
				return this._savedHighlightGroups.Count > 0;
			}
		}

		public static void RemoveHighlights()
		{
			if (HighlightsController.IsHighlightsInitialized)
			{
				foreach (HighlightsController.HighlightType highlightType in HighlightsController.HighlightTypes)
				{
					Highlights.RemoveHighlight(highlightType.Id);
				}
			}
		}

		public HighlightsController.HighlightType GetHighlightTypeWithId(string highlightId)
		{
			return HighlightsController.HighlightTypes.First((HighlightsController.HighlightType h) => h.Id == highlightId);
		}

		private void SaveVideo(string highlightID, string groupID, int startDelta, int endDelta)
		{
			Highlights.SaveVideo(highlightID, groupID, startDelta, endDelta);
			if (!this._savedHighlightGroups.Contains(groupID))
			{
				this._savedHighlightGroups.Add(groupID);
			}
		}

		public override void AfterStart()
		{
			if (!HighlightsController.IsHighlightsInitialized)
			{
				HighlightsController.HighlightTypes = new List<HighlightsController.HighlightType>
				{
					new HighlightsController.HighlightType("hlid_killing_spree", "Killing Spree", "grpid_incidents", -2010, 3000, 0.25f, float.MaxValue, true),
					new HighlightsController.HighlightType("hlid_high_ranged_shot_difficulty", "Sharpshooter", "grpid_incidents", -5000, 3000, 0.25f, float.MaxValue, true),
					new HighlightsController.HighlightType("hlid_archer_salvo_kills", "Death from Above", "grpid_incidents", -5004, 3000, 0.5f, 150f, false),
					new HighlightsController.HighlightType("hlid_couched_lance_against_mounted_opponent", "Lance A Lot", "grpid_incidents", -5000, 3000, 0.25f, float.MaxValue, true),
					new HighlightsController.HighlightType("hlid_cavalry_charge_first_impact", "Cavalry Charge First Impact", "grpid_incidents", -5000, 5000, 0.25f, float.MaxValue, false),
					new HighlightsController.HighlightType("hlid_headshot_kill", "Headshot!", "grpid_incidents", -5000, 3000, 0.25f, 150f, true),
					new HighlightsController.HighlightType("hlid_burning_ammunition_kill", "Burn Baby", "grpid_incidents", -5000, 3000, 0.25f, 100f, true),
					new HighlightsController.HighlightType("hlid_throwing_weapon_kill_against_charging_enemy", "Throwing Weapon Kill Against Charging Enemy", "grpid_incidents", -5000, 3000, 0.25f, 150f, true)
				};
				Highlights.Initialize();
				foreach (HighlightsController.HighlightType highlightType in HighlightsController.HighlightTypes)
				{
					Highlights.AddHighlight(highlightType.Id, highlightType.Description);
				}
				HighlightsController.IsHighlightsInitialized = true;
			}
			foreach (string text in this._highlightGroupIds)
			{
				Highlights.OpenGroup(text);
			}
			this._highlightSaveQueue = new List<HighlightsController.Highlight>();
			this._playerKillTimes = new List<float>();
			this._archerSalvoKillTimes = new List<float>();
			this._cavalryChargeHitTimes = new List<float>();
			this._savedHighlightGroups = new List<string>();
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectorAgent != null && affectedAgent != null && affectorAgent.IsHuman && affectedAgent.IsHuman && (agentState == AgentState.Killed || agentState == AgentState.Unconscious))
			{
				bool flag = affectorAgent.Team != null && affectorAgent.Team.IsPlayerTeam;
				bool isMainAgent = affectorAgent.IsMainAgent;
				if ((((isMainAgent || flag) && !affectedAgent.Team.IsPlayerAlly && killingBlow.WeaponClass == 12) || killingBlow.WeaponClass == 13) && this.CanSaveHighlight(this.GetHighlightTypeWithId("hlid_archer_salvo_kills"), affectedAgent.Position))
				{
					if (!this._isArcherSalvoHappening)
					{
						this._archerSalvoKillTimes.RemoveAll((float ht) => ht + 4f < Mission.Current.CurrentTime);
					}
					this._archerSalvoKillTimes.Add(Mission.Current.CurrentTime);
					if (this._archerSalvoKillTimes.Count >= 5)
					{
						this._isArcherSalvoHappening = true;
					}
				}
				if (isMainAgent && this.CanSaveHighlight(this.GetHighlightTypeWithId("hlid_killing_spree"), affectedAgent.Position))
				{
					if (!this._isKillingSpreeHappening)
					{
						this._playerKillTimes.RemoveAll((float ht) => ht + 10f < Mission.Current.CurrentTime);
					}
					this._playerKillTimes.Add(Mission.Current.CurrentTime);
					if (this._playerKillTimes.Count >= 4)
					{
						this._isKillingSpreeHappening = true;
					}
				}
				HighlightsController.Highlight highlight = default(HighlightsController.Highlight);
				highlight.Start = Mission.Current.CurrentTime;
				highlight.End = Mission.Current.CurrentTime;
				bool flag2 = false;
				if (isMainAgent && killingBlow.WeaponRecordWeaponFlags.HasAllFlags(WeaponFlags.Burning))
				{
					highlight.HighlightType = this.GetHighlightTypeWithId("hlid_burning_ammunition_kill");
					flag2 = true;
				}
				if (isMainAgent && killingBlow.IsMissile && killingBlow.IsHeadShot())
				{
					highlight.HighlightType = this.GetHighlightTypeWithId("hlid_headshot_kill");
					flag2 = true;
				}
				if (isMainAgent && killingBlow.IsMissile && affectedAgent.HasMount && affectedAgent.IsDoingPassiveAttack && (killingBlow.WeaponClass == 19 || killingBlow.WeaponClass == 20))
				{
					highlight.HighlightType = this.GetHighlightTypeWithId("hlid_throwing_weapon_kill_against_charging_enemy");
					flag2 = true;
				}
				if (this._isFirstImpact && affectorAgent.Formation != null && affectorAgent.Formation.IsCavalry() && affectorAgent.Formation.GetReadonlyMovementOrderReference() == MovementOrder.MovementOrderCharge && this.CanSaveHighlight(this.GetHighlightTypeWithId("hlid_cavalry_charge_first_impact"), affectedAgent.Position))
				{
					this._cavalryChargeHitTimes.RemoveAll((float ht) => ht + 3f < Mission.Current.CurrentTime);
					this._cavalryChargeHitTimes.Add(Mission.Current.CurrentTime);
					if (this._cavalryChargeHitTimes.Count >= 5)
					{
						highlight.HighlightType = this.GetHighlightTypeWithId("hlid_cavalry_charge_first_impact");
						highlight.Start = this._cavalryChargeHitTimes[0];
						highlight.End = this._cavalryChargeHitTimes[this._cavalryChargeHitTimes.Count - 1];
						flag2 = true;
						this._isFirstImpact = false;
						this._cavalryChargeHitTimes.Clear();
					}
				}
				if (flag2)
				{
					this.SaveHighlight(highlight, affectedAgent.Position);
				}
			}
		}

		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			bool flag = affectorAgent != null && affectorAgent.IsMainAgent;
			if (affectorAgent != null && affectedAgent != null && affectorAgent.IsHuman && affectedAgent.IsHuman)
			{
				HighlightsController.Highlight highlight = default(HighlightsController.Highlight);
				highlight.Start = Mission.Current.CurrentTime;
				highlight.End = Mission.Current.CurrentTime;
				bool flag2 = false;
				if (flag && shotDifficulty >= 7.5f)
				{
					highlight.HighlightType = this.GetHighlightTypeWithId("hlid_high_ranged_shot_difficulty");
					flag2 = true;
				}
				if (flag && affectedAgent.HasMount && blow.AttackType == AgentAttackType.Standard && affectorAgent.HasMount && affectorAgent.IsDoingPassiveAttack)
				{
					highlight.HighlightType = this.GetHighlightTypeWithId("hlid_couched_lance_against_mounted_opponent");
					flag2 = true;
				}
				if (this._isFirstImpact && affectorAgent.Formation != null && affectorAgent.Formation.IsCavalry() && affectorAgent.Formation.GetReadonlyMovementOrderReference() == MovementOrder.MovementOrderCharge && this.CanSaveHighlight(this.GetHighlightTypeWithId("hlid_cavalry_charge_first_impact"), affectedAgent.Position))
				{
					this._cavalryChargeHitTimes.RemoveAll((float ht) => ht + 3f < Mission.Current.CurrentTime);
					this._cavalryChargeHitTimes.Add(Mission.Current.CurrentTime);
					if (this._cavalryChargeHitTimes.Count >= 5)
					{
						highlight.HighlightType = this.GetHighlightTypeWithId("hlid_cavalry_charge_first_impact");
						highlight.Start = this._cavalryChargeHitTimes[0];
						highlight.End = this._cavalryChargeHitTimes[this._cavalryChargeHitTimes.Count - 1];
						flag2 = true;
						this._isFirstImpact = false;
						this._cavalryChargeHitTimes.Clear();
					}
				}
				if (flag2)
				{
					this.SaveHighlight(highlight, affectedAgent.Position);
				}
			}
		}

		public override void OnMissionTick(float dt)
		{
			if (this._isArcherSalvoHappening && this._archerSalvoKillTimes[0] + 4f < Mission.Current.CurrentTime)
			{
				HighlightsController.Highlight highlight;
				highlight.HighlightType = this.GetHighlightTypeWithId("hlid_archer_salvo_kills");
				highlight.Start = this._archerSalvoKillTimes[0];
				highlight.End = this._archerSalvoKillTimes[this._archerSalvoKillTimes.Count - 1];
				this.SaveHighlight(highlight);
				this._isArcherSalvoHappening = false;
				this._archerSalvoKillTimes.Clear();
			}
			if (this._isKillingSpreeHappening && this._playerKillTimes[0] + 10f < Mission.Current.CurrentTime)
			{
				HighlightsController.Highlight highlight2;
				highlight2.HighlightType = this.GetHighlightTypeWithId("hlid_killing_spree");
				highlight2.Start = this._playerKillTimes[0];
				highlight2.End = this._playerKillTimes[this._playerKillTimes.Count - 1];
				this.SaveHighlight(highlight2);
				this._isKillingSpreeHappening = false;
				this._playerKillTimes.Clear();
			}
			this.TickHighlightsToBeSaved();
		}

		protected override void OnEndMission()
		{
			base.OnEndMission();
			foreach (string text in this._highlightGroupIds)
			{
				Highlights.CloseGroup(text, false);
			}
			this._highlightSaveQueue = null;
			this._lastSavedHighlightData = null;
			this._playerKillTimes = null;
			this._archerSalvoKillTimes = null;
			this._cavalryChargeHitTimes = null;
		}

		public static void AddHighlightType(HighlightsController.HighlightType highlightType)
		{
			if (!HighlightsController.HighlightTypes.Any((HighlightsController.HighlightType h) => h.Id == highlightType.Id))
			{
				if (HighlightsController.IsHighlightsInitialized)
				{
					Highlights.AddHighlight(highlightType.Id, highlightType.Description);
				}
				HighlightsController.HighlightTypes.Add(highlightType);
			}
		}

		public void SaveHighlight(HighlightsController.Highlight highlight)
		{
			this._highlightSaveQueue.Add(highlight);
		}

		public void SaveHighlight(HighlightsController.Highlight highlight, Vec3 position)
		{
			if (this.CanSaveHighlight(highlight.HighlightType, position))
			{
				this._highlightSaveQueue.Add(highlight);
			}
		}

		public bool CanSaveHighlight(HighlightsController.HighlightType highlightType, Vec3 position)
		{
			return highlightType.MaxHighlightDistance >= Mission.Current.Scene.LastFinalRenderCameraFrame.origin.Distance(position) && highlightType.MinVisibilityScore <= this.GetPlayerIsLookingAtPositionScore(position) && (!highlightType.IsVisibilityRequired || this.CanSeePosition(position));
		}

		public float GetPlayerIsLookingAtPositionScore(Vec3 position)
		{
			Vec3 vec = -Mission.Current.Scene.LastFinalRenderCameraFrame.rotation.u;
			Vec3 origin = Mission.Current.Scene.LastFinalRenderCameraFrame.origin;
			return MathF.Max(Vec3.DotProduct(vec.NormalizedCopy(), (position - origin).NormalizedCopy()), 0f);
		}

		public bool CanSeePosition(Vec3 position)
		{
			Vec3 origin = Mission.Current.Scene.LastFinalRenderCameraFrame.origin;
			float num;
			return !Mission.Current.Scene.RayCastForClosestEntityOrTerrain(origin, position, out num, 0.01f, BodyFlags.CameraCollisionRayCastExludeFlags) || MathF.Abs(position.Distance(origin) - num) < 0.1f;
		}

		public void ShowSummary()
		{
			if (this.IsAnyHighlightSaved)
			{
				Highlights.OpenSummary(this._savedHighlightGroups);
			}
		}

		private void TickHighlightsToBeSaved()
		{
			if (this._highlightSaveQueue != null)
			{
				if (this._lastSavedHighlightData != null && this._highlightSaveQueue.Count > 0)
				{
					float item = this._lastSavedHighlightData.Item1;
					float item2 = this._lastSavedHighlightData.Item2;
					float num = item2 - (item2 - item) * 0.5f;
					for (int i = 0; i < this._highlightSaveQueue.Count; i++)
					{
						float start = this._highlightSaveQueue[i].Start;
						HighlightsController.Highlight highlight = this._highlightSaveQueue[i];
						if (start - (float)highlight.HighlightType.StartDelta < num)
						{
							this._highlightSaveQueue.Remove(this._highlightSaveQueue[i]);
							i--;
						}
					}
				}
				if (this._highlightSaveQueue.Count > 0)
				{
					float start2 = this._highlightSaveQueue[0].Start;
					HighlightsController.Highlight highlight = this._highlightSaveQueue[0];
					float num2 = start2 + (float)(highlight.HighlightType.StartDelta / 1000);
					float end = this._highlightSaveQueue[0].End;
					highlight = this._highlightSaveQueue[0];
					float num3 = end + (float)(highlight.HighlightType.EndDelta / 1000);
					for (int j = 1; j < this._highlightSaveQueue.Count; j++)
					{
						float start3 = this._highlightSaveQueue[j].Start;
						highlight = this._highlightSaveQueue[j];
						float num4 = start3 + (float)(highlight.HighlightType.StartDelta / 1000);
						float end2 = this._highlightSaveQueue[j].End;
						highlight = this._highlightSaveQueue[j];
						float num5 = end2 + (float)(highlight.HighlightType.EndDelta / 1000);
						if (num4 < num2)
						{
							num2 = num4;
						}
						if (num5 > num3)
						{
							num3 = num5;
						}
					}
					highlight = this._highlightSaveQueue[0];
					string id = highlight.HighlightType.Id;
					highlight = this._highlightSaveQueue[0];
					this.SaveVideo(id, highlight.HighlightType.GroupId, (int)(num2 - Mission.Current.CurrentTime) * 1000, (int)(num3 - Mission.Current.CurrentTime) * 1000);
					this._lastSavedHighlightData = new Tuple<float, float>(num2, num3);
					this._highlightSaveQueue.Clear();
				}
			}
		}

		private bool _isKillingSpreeHappening;

		private List<float> _playerKillTimes;

		private const int MinKillingSpreeKills = 4;

		private const float MaxKillingSpreeDuration = 10f;

		private const float HighShotDifficultyThreshold = 7.5f;

		private bool _isArcherSalvoHappening;

		private List<float> _archerSalvoKillTimes;

		private const int MinArcherSalvoKills = 5;

		private const float MaxArcherSalvoDuration = 4f;

		private bool _isFirstImpact = true;

		private List<float> _cavalryChargeHitTimes;

		private const float CavalryChargeImpactTimeFrame = 3f;

		private const int MinCavalryChargeHits = 5;

		private Tuple<float, float> _lastSavedHighlightData;

		private List<HighlightsController.Highlight> _highlightSaveQueue;

		private const float IgnoreIfOverlapsLastVideoPercent = 0.5f;

		private List<string> _savedHighlightGroups;

		private List<string> _highlightGroupIds = new List<string> { "grpid_incidents", "grpid_achievements" };

		public struct HighlightType
		{
			public string Id { get; private set; }

			public string Description { get; private set; }

			public string GroupId { get; private set; }

			public int StartDelta { get; private set; }

			public int EndDelta { get; private set; }

			public float MinVisibilityScore { get; private set; }

			public float MaxHighlightDistance { get; private set; }

			public bool IsVisibilityRequired { get; private set; }

			public HighlightType(string id, string description, string groupId, int startDelta, int endDelta, float minVisibilityScore, float maxHighlightDistance, bool isVisibilityRequired)
			{
				this.Id = id;
				this.Description = description;
				this.GroupId = groupId;
				this.StartDelta = startDelta;
				this.EndDelta = endDelta;
				this.MinVisibilityScore = minVisibilityScore;
				this.MaxHighlightDistance = maxHighlightDistance;
				this.IsVisibilityRequired = isVisibilityRequired;
			}
		}

		public struct Highlight
		{
			public HighlightsController.HighlightType HighlightType;

			public float Start;

			public float End;
		}
	}
}
