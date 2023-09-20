using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View.Screens;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	public abstract class MissionView : MissionBehavior
	{
		public MissionScreen MissionScreen
		{
			get
			{
				return this._missionScreen;
			}
			internal set
			{
				if (this._missionScreen == null && value != null)
				{
					value.RegisterView(this);
				}
				if (this._missionScreen != null && value == null)
				{
					this._missionScreen.UnregisterView(this);
				}
				this._missionScreen = value;
			}
		}

		public IInputContext Input
		{
			get
			{
				return this.MissionScreen.SceneLayer.Input;
			}
		}

		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return 1;
			}
		}

		public virtual void OnMissionScreenTick(float dt)
		{
		}

		public virtual bool OnEscape()
		{
			return false;
		}

		public virtual bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return true;
		}

		public virtual void OnFocusChangeOnGameWindow(bool focusGained)
		{
		}

		public virtual void OnSceneRenderingStarted()
		{
		}

		public virtual void OnMissionScreenInitialize()
		{
		}

		public virtual void OnMissionScreenFinalize()
		{
		}

		public virtual void OnMissionScreenActivate()
		{
		}

		public virtual void OnMissionScreenDeactivate()
		{
		}

		public virtual bool UpdateOverridenCamera(float dt)
		{
			return false;
		}

		public virtual bool IsReady()
		{
			return true;
		}

		public virtual void OnPhotoModeActivated()
		{
		}

		public virtual void OnPhotoModeDeactivated()
		{
		}

		public virtual void OnConversationBegin()
		{
		}

		public virtual void OnConversationEnd()
		{
		}

		public virtual void OnInitialDeploymentPlanMadeForSide(BattleSideEnum side, bool isFirstPlan)
		{
		}

		public sealed override void OnEndMissionInternal()
		{
			this.OnEndMission();
			this.MissionScreen = null;
		}

		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this.MissionScreen = null;
		}

		public int ViewOrderPriority;

		private MissionScreen _missionScreen;
	}
}
