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
	public class PassageUsePoint : StandingPoint
	{
		public MBReadOnlyList<Agent> MovingAgents
		{
			get
			{
				return this._movingAgents;
			}
		}

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

		public PassageUsePoint()
		{
			base.IsInstantUse = true;
			this._movingAgents = new MBList<Agent>();
		}

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

		public override bool HasAIMovingTo
		{
			get
			{
				return this._movingAgents.Count > 0;
			}
		}

		public override FocusableObjectType FocusableObjectType
		{
			get
			{
				return 4;
			}
		}

		public override bool IsDisabledForAgent(Agent agent)
		{
			return agent.MountAgent != null || base.IsDeactivated || this.ToLocation == null || (agent.IsAIControlled && !this.ToLocation.CanAIEnter(CampaignMission.Current.Location.GetLocationCharacter(agent.Origin)));
		}

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

		public override bool DisableCombatActionsOnUse
		{
			get
			{
				return !base.IsInstantUse;
			}
		}

		protected override void OnInit()
		{
			base.OnInit();
			this.LockUserPositions = true;
		}

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

		public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
			if (this.LockUserFrames || this.LockUserPositions)
			{
				userAgent.ClearTargetFrame();
			}
		}

		public override bool IsUsableByAgent(Agent userAgent)
		{
			bool flag = true;
			if (userAgent.IsAIControlled && (this.InteractionEntity.GetGlobalFrame().origin.AsVec2 - userAgent.Position.AsVec2).LengthSquared > 0.25f)
			{
				flag = false;
			}
			return flag;
		}

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

		public override int GetMovingAgentCount()
		{
			return this._movingAgents.Count;
		}

		public override Agent GetMovingAgentWithIndex(int index)
		{
			return this._movingAgents[index];
		}

		public override void AddMovingAgent(Agent movingAgent)
		{
			this._movingAgents.Add(movingAgent);
		}

		public override void RemoveMovingAgent(Agent movingAgent)
		{
			this._movingAgents.Remove(movingAgent);
		}

		public override bool IsAIMovingTo(Agent agent)
		{
			return this._movingAgents.Contains(agent);
		}

		public string ToLocationId = "";

		private bool _initialized;

		private readonly MBList<Agent> _movingAgents;

		private Location _toLocation;

		private const float InteractionDistanceForAI = 0.5f;
	}
}
