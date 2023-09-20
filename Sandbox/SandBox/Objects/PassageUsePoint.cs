using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects
{
	// Token: 0x02000024 RID: 36
	public class PassageUsePoint : StandingPoint
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000BE38 File Offset: 0x0000A038
		public MBReadOnlyList<Agent> MovingAgents
		{
			get
			{
				return this._movingAgents;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000BE40 File Offset: 0x0000A040
		public override Agent MovingAgent
		{
			get
			{
				if (this._movingAgents.Count <= 0)
				{
					return null;
				}
				return this._movingAgents[0];
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000BE5E File Offset: 0x0000A05E
		public PassageUsePoint()
		{
			base.IsInstantUse = true;
			this._movingAgents = new MBList<Agent>();
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000BE83 File Offset: 0x0000A083
		public Location ToLocation
		{
			get
			{
				if (!this._initialized)
				{
					this.InitializeLocation();
				}
				return this._toLocation;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x0000BE99 File Offset: 0x0000A099
		public override bool HasAIMovingTo
		{
			get
			{
				return this._movingAgents.Count > 0;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000BEA9 File Offset: 0x0000A0A9
		public override FocusableObjectType FocusableObjectType
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000BEAC File Offset: 0x0000A0AC
		public override bool IsDisabledForAgent(Agent agent)
		{
			return agent.MountAgent != null || base.IsDeactivated || this.ToLocation == null || (agent.IsAIControlled && !this.ToLocation.CanAIEnter(CampaignMission.Current.Location.GetLocationCharacter(agent.Origin)));
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000BF00 File Offset: 0x0000A100
		public override void AfterMissionStart()
		{
			this.DescriptionMessage = GameTexts.FindText("str_ui_door", null);
			this.ActionMessage = GameTexts.FindText("str_ui_default_door", null);
			if (this.ToLocation != null)
			{
				this.ActionMessage = GameTexts.FindText("str_key_action", null);
				this.ActionMessage.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
				this.ActionMessage.SetTextVariable("ACTION", (this.ToLocation == null) ? GameTexts.FindText("str_ui_default_door", null) : this.ToLocation.DoorName);
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x0000BF9B File Offset: 0x0000A19B
		public override bool DisableCombatActionsOnUse
		{
			get
			{
				return !base.IsInstantUse;
			}
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000BFA6 File Offset: 0x0000A1A6
		protected override void OnInit()
		{
			base.OnInit();
			this.LockUserPositions = true;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000BFB8 File Offset: 0x0000A1B8
		public override void OnUse(Agent userAgent)
		{
			if (Campaign.Current.GameMode == 1 || userAgent.IsAIControlled)
			{
				base.OnUse(userAgent);
				bool flag = false;
				if (this.ToLocation != null)
				{
					if (base.UserAgent.IsMainAgent)
					{
						if (!this.ToLocation.CanPlayerEnter())
						{
							InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=ILnr9eCQ}Door is locked!", null).ToString()));
						}
						else
						{
							flag = true;
							Campaign.Current.GameMenuManager.NextLocation = this.ToLocation;
							Campaign.Current.GameMenuManager.PreviousLocation = CampaignMission.Current.Location;
							Mission.Current.EndMission();
						}
					}
					else if (base.UserAgent.IsAIControlled)
					{
						LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(base.UserAgent.Origin);
						if (!this.ToLocation.CanAIEnter(locationCharacter))
						{
							MBDebug.ShowWarning("AI should not try to use passage ");
						}
						else
						{
							flag = true;
							LocationComplex.Current.ChangeLocation(locationCharacter, CampaignMission.Current.Location, this.ToLocation);
							base.UserAgent.FadeOut(false, true);
						}
					}
					if (flag)
					{
						Mission.Current.MakeSound(MiscSoundContainer.SoundCodeMovementFoleyDoorOpen, base.GameEntity.GetGlobalFrame().origin, true, false, -1, -1);
					}
				}
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000C0F8 File Offset: 0x0000A2F8
		public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
			if (this.LockUserFrames || this.LockUserPositions)
			{
				userAgent.ClearTargetFrame();
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000C11C File Offset: 0x0000A31C
		public override bool IsUsableByAgent(Agent userAgent)
		{
			bool flag = true;
			if (userAgent.IsAIControlled && (this.InteractionEntity.GetGlobalFrame().origin.AsVec2 - userAgent.Position.AsVec2).LengthSquared > 0.25f)
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000C170 File Offset: 0x0000A370
		private void InitializeLocation()
		{
			if (string.IsNullOrEmpty(this.ToLocationId))
			{
				this._toLocation = null;
				this._initialized = true;
				return;
			}
			if (Mission.Current != null && Campaign.Current != null)
			{
				if (PlayerEncounter.LocationEncounter != null && CampaignMission.Current.Location != null)
				{
					this._toLocation = CampaignMission.Current.Location.GetPassageToLocation(this.ToLocationId);
				}
				this._initialized = true;
			}
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000C1DC File Offset: 0x0000A3DC
		public override int GetMovingAgentCount()
		{
			return this._movingAgents.Count;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000C1E9 File Offset: 0x0000A3E9
		public override Agent GetMovingAgentWithIndex(int index)
		{
			return this._movingAgents[index];
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000C1F7 File Offset: 0x0000A3F7
		public override void AddMovingAgent(Agent movingAgent)
		{
			this._movingAgents.Add(movingAgent);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000C205 File Offset: 0x0000A405
		public override void RemoveMovingAgent(Agent movingAgent)
		{
			this._movingAgents.Remove(movingAgent);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000C214 File Offset: 0x0000A414
		public override bool IsAIMovingTo(Agent agent)
		{
			return this._movingAgents.Contains(agent);
		}

		// Token: 0x040000AE RID: 174
		public string ToLocationId = "";

		// Token: 0x040000AF RID: 175
		private bool _initialized;

		// Token: 0x040000B0 RID: 176
		private readonly MBList<Agent> _movingAgents;

		// Token: 0x040000B1 RID: 177
		private Location _toLocation;

		// Token: 0x040000B2 RID: 178
		private const float InteractionDistanceForAI = 0.5f;
	}
}
