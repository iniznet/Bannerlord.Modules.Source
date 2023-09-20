using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200026C RID: 620
	public class HighlightsController : MissionLogic
	{
		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x06002111 RID: 8465 RVA: 0x000775FF File Offset: 0x000757FF
		// (set) Token: 0x06002112 RID: 8466 RVA: 0x00077606 File Offset: 0x00075806
		private protected static List<HighlightsController.HighlightType> HighlightTypes { protected get; private set; }

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x06002113 RID: 8467 RVA: 0x0007760E File Offset: 0x0007580E
		// (set) Token: 0x06002114 RID: 8468 RVA: 0x00077615 File Offset: 0x00075815
		public static bool IsHighlightsInitialized { get; private set; }

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x06002115 RID: 8469 RVA: 0x0007761D File Offset: 0x0007581D
		public bool IsAnyHighlightSaved
		{
			get
			{
				return this._savedHighlightGroups.Count > 0;
			}
		}

		// Token: 0x06002116 RID: 8470 RVA: 0x00077630 File Offset: 0x00075830
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

		// Token: 0x06002117 RID: 8471 RVA: 0x00077690 File Offset: 0x00075890
		public HighlightsController.HighlightType GetHighlightTypeWithId(string highlightId)
		{
			return HighlightsController.HighlightTypes.First((HighlightsController.HighlightType h) => h.Id == highlightId);
		}

		// Token: 0x06002118 RID: 8472 RVA: 0x000776C0 File Offset: 0x000758C0
		private void SaveVideo(string highlightID, string groupID, int startDelta, int endDelta)
		{
			Highlights.SaveVideo(highlightID, groupID, startDelta, endDelta);
			if (!this._savedHighlightGroups.Contains(groupID))
			{
				this._savedHighlightGroups.Add(groupID);
			}
		}

		// Token: 0x06002119 RID: 8473 RVA: 0x000776E8 File Offset: 0x000758E8
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

		// Token: 0x0600211A RID: 8474 RVA: 0x00077954 File Offset: 0x00075B54
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

		// Token: 0x0600211B RID: 8475 RVA: 0x00077C9C File Offset: 0x00075E9C
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

		// Token: 0x0600211C RID: 8476 RVA: 0x00077E70 File Offset: 0x00076070
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

		// Token: 0x0600211D RID: 8477 RVA: 0x00077F8C File Offset: 0x0007618C
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

		// Token: 0x0600211E RID: 8478 RVA: 0x00078008 File Offset: 0x00076208
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

		// Token: 0x0600211F RID: 8479 RVA: 0x0007806C File Offset: 0x0007626C
		public void SaveHighlight(HighlightsController.Highlight highlight)
		{
			this._highlightSaveQueue.Add(highlight);
		}

		// Token: 0x06002120 RID: 8480 RVA: 0x0007807A File Offset: 0x0007627A
		public void SaveHighlight(HighlightsController.Highlight highlight, Vec3 position)
		{
			if (this.CanSaveHighlight(highlight.HighlightType, position))
			{
				this._highlightSaveQueue.Add(highlight);
			}
		}

		// Token: 0x06002121 RID: 8481 RVA: 0x00078098 File Offset: 0x00076298
		public bool CanSaveHighlight(HighlightsController.HighlightType highlightType, Vec3 position)
		{
			return highlightType.MaxHighlightDistance >= Mission.Current.Scene.LastFinalRenderCameraFrame.origin.Distance(position) && highlightType.MinVisibilityScore <= this.GetPlayerIsLookingAtPositionScore(position) && (!highlightType.IsVisibilityRequired || this.CanSeePosition(position));
		}

		// Token: 0x06002122 RID: 8482 RVA: 0x000780F0 File Offset: 0x000762F0
		public float GetPlayerIsLookingAtPositionScore(Vec3 position)
		{
			Vec3 vec = -Mission.Current.Scene.LastFinalRenderCameraFrame.rotation.u;
			Vec3 origin = Mission.Current.Scene.LastFinalRenderCameraFrame.origin;
			return MathF.Max(Vec3.DotProduct(vec.NormalizedCopy(), (position - origin).NormalizedCopy()), 0f);
		}

		// Token: 0x06002123 RID: 8483 RVA: 0x00078158 File Offset: 0x00076358
		public bool CanSeePosition(Vec3 position)
		{
			Vec3 origin = Mission.Current.Scene.LastFinalRenderCameraFrame.origin;
			float num;
			return !Mission.Current.Scene.RayCastForClosestEntityOrTerrain(origin, position, out num, 0.01f, BodyFlags.CameraCollisionRayCastExludeFlags) || MathF.Abs(position.Distance(origin) - num) < 0.1f;
		}

		// Token: 0x06002124 RID: 8484 RVA: 0x000781B1 File Offset: 0x000763B1
		public void ShowSummary()
		{
			if (this.IsAnyHighlightSaved)
			{
				Highlights.OpenSummary(this._savedHighlightGroups);
			}
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x000781C8 File Offset: 0x000763C8
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

		// Token: 0x04000C33 RID: 3123
		private bool _isKillingSpreeHappening;

		// Token: 0x04000C34 RID: 3124
		private List<float> _playerKillTimes;

		// Token: 0x04000C35 RID: 3125
		private const int MinKillingSpreeKills = 4;

		// Token: 0x04000C36 RID: 3126
		private const float MaxKillingSpreeDuration = 10f;

		// Token: 0x04000C37 RID: 3127
		private const float HighShotDifficultyThreshold = 7.5f;

		// Token: 0x04000C38 RID: 3128
		private bool _isArcherSalvoHappening;

		// Token: 0x04000C39 RID: 3129
		private List<float> _archerSalvoKillTimes;

		// Token: 0x04000C3A RID: 3130
		private const int MinArcherSalvoKills = 5;

		// Token: 0x04000C3B RID: 3131
		private const float MaxArcherSalvoDuration = 4f;

		// Token: 0x04000C3C RID: 3132
		private bool _isFirstImpact = true;

		// Token: 0x04000C3D RID: 3133
		private List<float> _cavalryChargeHitTimes;

		// Token: 0x04000C3E RID: 3134
		private const float CavalryChargeImpactTimeFrame = 3f;

		// Token: 0x04000C3F RID: 3135
		private const int MinCavalryChargeHits = 5;

		// Token: 0x04000C40 RID: 3136
		private Tuple<float, float> _lastSavedHighlightData;

		// Token: 0x04000C41 RID: 3137
		private List<HighlightsController.Highlight> _highlightSaveQueue;

		// Token: 0x04000C42 RID: 3138
		private const float IgnoreIfOverlapsLastVideoPercent = 0.5f;

		// Token: 0x04000C43 RID: 3139
		private List<string> _savedHighlightGroups;

		// Token: 0x04000C44 RID: 3140
		private List<string> _highlightGroupIds = new List<string> { "grpid_incidents", "grpid_achievements" };

		// Token: 0x02000575 RID: 1397
		public struct HighlightType
		{
			// Token: 0x17000986 RID: 2438
			// (get) Token: 0x06003AAE RID: 15022 RVA: 0x000ECCA3 File Offset: 0x000EAEA3
			// (set) Token: 0x06003AAF RID: 15023 RVA: 0x000ECCAB File Offset: 0x000EAEAB
			public string Id { get; private set; }

			// Token: 0x17000987 RID: 2439
			// (get) Token: 0x06003AB0 RID: 15024 RVA: 0x000ECCB4 File Offset: 0x000EAEB4
			// (set) Token: 0x06003AB1 RID: 15025 RVA: 0x000ECCBC File Offset: 0x000EAEBC
			public string Description { get; private set; }

			// Token: 0x17000988 RID: 2440
			// (get) Token: 0x06003AB2 RID: 15026 RVA: 0x000ECCC5 File Offset: 0x000EAEC5
			// (set) Token: 0x06003AB3 RID: 15027 RVA: 0x000ECCCD File Offset: 0x000EAECD
			public string GroupId { get; private set; }

			// Token: 0x17000989 RID: 2441
			// (get) Token: 0x06003AB4 RID: 15028 RVA: 0x000ECCD6 File Offset: 0x000EAED6
			// (set) Token: 0x06003AB5 RID: 15029 RVA: 0x000ECCDE File Offset: 0x000EAEDE
			public int StartDelta { get; private set; }

			// Token: 0x1700098A RID: 2442
			// (get) Token: 0x06003AB6 RID: 15030 RVA: 0x000ECCE7 File Offset: 0x000EAEE7
			// (set) Token: 0x06003AB7 RID: 15031 RVA: 0x000ECCEF File Offset: 0x000EAEEF
			public int EndDelta { get; private set; }

			// Token: 0x1700098B RID: 2443
			// (get) Token: 0x06003AB8 RID: 15032 RVA: 0x000ECCF8 File Offset: 0x000EAEF8
			// (set) Token: 0x06003AB9 RID: 15033 RVA: 0x000ECD00 File Offset: 0x000EAF00
			public float MinVisibilityScore { get; private set; }

			// Token: 0x1700098C RID: 2444
			// (get) Token: 0x06003ABA RID: 15034 RVA: 0x000ECD09 File Offset: 0x000EAF09
			// (set) Token: 0x06003ABB RID: 15035 RVA: 0x000ECD11 File Offset: 0x000EAF11
			public float MaxHighlightDistance { get; private set; }

			// Token: 0x1700098D RID: 2445
			// (get) Token: 0x06003ABC RID: 15036 RVA: 0x000ECD1A File Offset: 0x000EAF1A
			// (set) Token: 0x06003ABD RID: 15037 RVA: 0x000ECD22 File Offset: 0x000EAF22
			public bool IsVisibilityRequired { get; private set; }

			// Token: 0x06003ABE RID: 15038 RVA: 0x000ECD2B File Offset: 0x000EAF2B
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

		// Token: 0x02000576 RID: 1398
		public struct Highlight
		{
			// Token: 0x04001D31 RID: 7473
			public HighlightsController.HighlightType HighlightType;

			// Token: 0x04001D32 RID: 7474
			public float Start;

			// Token: 0x04001D33 RID: 7475
			public float End;
		}
	}
}
