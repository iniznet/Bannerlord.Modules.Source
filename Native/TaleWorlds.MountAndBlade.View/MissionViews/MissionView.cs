using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View.Screens;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	// Token: 0x02000052 RID: 82
	public abstract class MissionView : MissionBehavior
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000382 RID: 898 RVA: 0x0001EC0C File Offset: 0x0001CE0C
		// (set) Token: 0x06000383 RID: 899 RVA: 0x0001EC14 File Offset: 0x0001CE14
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

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000384 RID: 900 RVA: 0x0001EC46 File Offset: 0x0001CE46
		public IInputContext Input
		{
			get
			{
				return this.MissionScreen.SceneLayer.Input;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000385 RID: 901 RVA: 0x0001EC58 File Offset: 0x0001CE58
		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0001EC5B File Offset: 0x0001CE5B
		public virtual void OnMissionScreenTick(float dt)
		{
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0001EC5D File Offset: 0x0001CE5D
		public virtual bool OnEscape()
		{
			return false;
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0001EC60 File Offset: 0x0001CE60
		public virtual bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return true;
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0001EC63 File Offset: 0x0001CE63
		public virtual void OnFocusChangeOnGameWindow(bool focusGained)
		{
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0001EC65 File Offset: 0x0001CE65
		public virtual void OnSceneRenderingStarted()
		{
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0001EC67 File Offset: 0x0001CE67
		public virtual void OnMissionScreenInitialize()
		{
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0001EC69 File Offset: 0x0001CE69
		public virtual void OnMissionScreenFinalize()
		{
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0001EC6B File Offset: 0x0001CE6B
		public virtual void OnMissionScreenActivate()
		{
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0001EC6D File Offset: 0x0001CE6D
		public virtual void OnMissionScreenDeactivate()
		{
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0001EC6F File Offset: 0x0001CE6F
		public virtual bool UpdateOverridenCamera(float dt)
		{
			return false;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0001EC72 File Offset: 0x0001CE72
		public virtual bool IsReady()
		{
			return true;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0001EC75 File Offset: 0x0001CE75
		public virtual void OnPhotoModeActivated()
		{
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0001EC77 File Offset: 0x0001CE77
		public virtual void OnPhotoModeDeactivated()
		{
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0001EC79 File Offset: 0x0001CE79
		public virtual void OnConversationBegin()
		{
		}

		// Token: 0x06000394 RID: 916 RVA: 0x0001EC7B File Offset: 0x0001CE7B
		public virtual void OnConversationEnd()
		{
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0001EC7D File Offset: 0x0001CE7D
		public virtual void OnInitialDeploymentPlanMadeForSide(BattleSideEnum side, bool isFirstPlan)
		{
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0001EC7F File Offset: 0x0001CE7F
		public sealed override void OnEndMissionInternal()
		{
			this.OnEndMission();
			this.MissionScreen = null;
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0001EC8E File Offset: 0x0001CE8E
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this.MissionScreen = null;
		}

		// Token: 0x04000258 RID: 600
		public int ViewOrderPriority;

		// Token: 0x04000259 RID: 601
		private MissionScreen _missionScreen;
	}
}
