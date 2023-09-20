using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	// Token: 0x02000050 RID: 80
	public class MissionMainAgentInteractionComponent
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600036A RID: 874 RVA: 0x0001E1BC File Offset: 0x0001C3BC
		// (remove) Token: 0x0600036B RID: 875 RVA: 0x0001E1F4 File Offset: 0x0001C3F4
		public event MissionMainAgentInteractionComponent.MissionFocusGainedEventDelegate OnFocusGained;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600036C RID: 876 RVA: 0x0001E22C File Offset: 0x0001C42C
		// (remove) Token: 0x0600036D RID: 877 RVA: 0x0001E264 File Offset: 0x0001C464
		public event MissionMainAgentInteractionComponent.MissionFocusLostEventDelegate OnFocusLost;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600036E RID: 878 RVA: 0x0001E29C File Offset: 0x0001C49C
		// (remove) Token: 0x0600036F RID: 879 RVA: 0x0001E2D4 File Offset: 0x0001C4D4
		public event MissionMainAgentInteractionComponent.MissionFocusHealthChangeDelegate OnFocusHealthChanged;

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000370 RID: 880 RVA: 0x0001E309 File Offset: 0x0001C509
		// (set) Token: 0x06000371 RID: 881 RVA: 0x0001E311 File Offset: 0x0001C511
		public IFocusable CurrentFocusedObject { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000372 RID: 882 RVA: 0x0001E31A File Offset: 0x0001C51A
		// (set) Token: 0x06000373 RID: 883 RVA: 0x0001E322 File Offset: 0x0001C522
		public IFocusable CurrentFocusedMachine { get; private set; }

		// Token: 0x06000374 RID: 884 RVA: 0x0001E32C File Offset: 0x0001C52C
		public void SetCurrentFocusedObject(IFocusable focusedObject, IFocusable focusedMachine, bool isInteractable)
		{
			if (this.CurrentFocusedObject != null && (this.CurrentFocusedObject != focusedObject || (this._currentInteractableObject != null && !isInteractable) || (this._currentInteractableObject == null && isInteractable)))
			{
				this.FocusLost(this.CurrentFocusedObject, this.CurrentFocusedMachine);
				this._currentInteractableObject = null;
				this.CurrentFocusedObject = null;
				this.CurrentFocusedMachine = null;
			}
			if (this.CurrentFocusedObject == null && focusedObject != null)
			{
				if (focusedObject != this.CurrentFocusedObject)
				{
					this.FocusGained(focusedObject, focusedMachine, isInteractable);
				}
				if (isInteractable)
				{
					this._currentInteractableObject = focusedObject;
				}
				this.CurrentFocusedObject = focusedObject;
				this.CurrentFocusedMachine = focusedMachine;
			}
		}

		// Token: 0x06000375 RID: 885 RVA: 0x0001E3BE File Offset: 0x0001C5BE
		public void ClearFocus()
		{
			if (this.CurrentFocusedObject != null)
			{
				this.FocusLost(this.CurrentFocusedObject, this.CurrentFocusedMachine);
			}
			this._currentInteractableObject = null;
			this.CurrentFocusedObject = null;
			this.CurrentFocusedMachine = null;
		}

		// Token: 0x06000376 RID: 886 RVA: 0x0001E3EF File Offset: 0x0001C5EF
		public void OnClearScene()
		{
			this.ClearFocus();
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000377 RID: 887 RVA: 0x0001E3F7 File Offset: 0x0001C5F7
		private Mission CurrentMission
		{
			get
			{
				return this._mainAgentController.Mission;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000378 RID: 888 RVA: 0x0001E404 File Offset: 0x0001C604
		private MissionScreen CurrentMissionScreen
		{
			get
			{
				return this._mainAgentController.MissionScreen;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000379 RID: 889 RVA: 0x0001E411 File Offset: 0x0001C611
		private Scene CurrentMissionScene
		{
			get
			{
				return this._mainAgentController.Mission.Scene;
			}
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0001E423 File Offset: 0x0001C623
		public MissionMainAgentInteractionComponent(MissionMainAgentController mainAgentController)
		{
			this._mainAgentController = mainAgentController;
		}

		// Token: 0x0600037B RID: 891 RVA: 0x0001E434 File Offset: 0x0001C634
		private static float GetCollisionDistanceSquaredOfIntersectionFromMainAgentEye(Vec3 rayStartPoint, Vec3 rayDirection, float rayLength)
		{
			float num = rayLength * rayLength;
			Vec3 vec = rayStartPoint + rayDirection * rayLength;
			Vec3 position = Agent.Main.Position;
			float eyeGlobalHeight = Agent.Main.GetEyeGlobalHeight();
			Vec3 vec2;
			vec2..ctor(position.x, position.y, position.z + eyeGlobalHeight, -1f);
			float num2 = vec.z - vec2.z;
			if (num2 < 0f)
			{
				num2 = MBMath.ClampFloat(-num2, 0f, (Agent.Main.HasMount ? (eyeGlobalHeight - Agent.Main.MountAgent.GetEyeGlobalHeight()) : eyeGlobalHeight) * 0.75f);
				vec2.z -= num2;
				num = vec2.DistanceSquared(vec);
			}
			return num;
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0001E4F0 File Offset: 0x0001C6F0
		private void FocusGained(IFocusable focusedObject, IFocusable focusedMachine, bool isInteractable)
		{
			focusedObject.OnFocusGain(Agent.Main);
			if (focusedMachine != null)
			{
				focusedMachine.OnFocusGain(Agent.Main);
			}
			foreach (MissionBehavior missionBehavior in this.CurrentMission.MissionBehaviors)
			{
				missionBehavior.OnFocusGained(Agent.Main, focusedObject, isInteractable);
			}
			MissionMainAgentInteractionComponent.MissionFocusGainedEventDelegate onFocusGained = this.OnFocusGained;
			if (onFocusGained == null)
			{
				return;
			}
			onFocusGained(Agent.Main, this.CurrentFocusedObject, isInteractable);
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0001E584 File Offset: 0x0001C784
		private void FocusLost(IFocusable focusedObject, IFocusable focusedMachine)
		{
			focusedObject.OnFocusLose(Agent.Main);
			if (focusedMachine != null)
			{
				focusedMachine.OnFocusLose(Agent.Main);
			}
			foreach (MissionBehavior missionBehavior in this.CurrentMission.MissionBehaviors)
			{
				missionBehavior.OnFocusLost(Agent.Main, focusedObject);
			}
			MissionMainAgentInteractionComponent.MissionFocusLostEventDelegate onFocusLost = this.OnFocusLost;
			if (onFocusLost == null)
			{
				return;
			}
			onFocusLost(Agent.Main, this.CurrentFocusedObject);
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0001E614 File Offset: 0x0001C814
		public void FocusTick()
		{
			IFocusable focusable = null;
			UsableMachine usableMachine = null;
			bool flag = true;
			bool flag2 = true;
			if (Mission.Current.Mode == 1 || Mission.Current.Mode == 9)
			{
				if (this.CurrentFocusedObject != null && Mission.Current.Mode != 1)
				{
					this.ClearFocus();
				}
				return;
			}
			Agent main = Agent.Main;
			if (!this.CurrentMissionScreen.SceneLayer.Input.IsGameKeyDown(25) && main != null && main.IsOnLand())
			{
				float num = 10f;
				Vec3 direction = this.CurrentMissionScreen.CombatCamera.Direction;
				Vec3 vec = direction;
				Vec3 position = this.CurrentMissionScreen.CombatCamera.Position;
				Vec3 position2 = main.Position;
				float num2 = new Vec3(position.x, position.y, 0f, -1f).Distance(new Vec3(position2.x, position2.y, 0f, -1f));
				Vec3 vec2 = position * (1f - num2) + (position + direction) * num2;
				float num3;
				if (this.CurrentMissionScene.RayCastForClosestEntityOrTerrain(vec2, vec2 + vec * num, ref num3, 0.01f, 16727871))
				{
					num = num3;
				}
				float num4 = float.MaxValue;
				Agent agent = this.CurrentMission.RayCastForClosestAgent(vec2, vec2 + vec * (num + 0.01f), ref num3, main.Index, 0.3f);
				if (agent != null && (!agent.IsMount || (agent.RiderAgent == null && main.MountAgent == null && main.CanReachAgent(agent))))
				{
					num4 = num3;
					focusable = agent;
					if (!main.CanInteractWithAgent(agent, this.CurrentMissionScreen.CameraElevation))
					{
						flag2 = false;
					}
				}
				float num5 = 3f;
				num += 0.1f;
				GameEntity gameEntity;
				if (!this.CurrentMissionScene.RayCastForClosestEntityOrTerrain(vec2, vec2 + vec * num, ref num3, ref gameEntity, 0.2f, 79617) || !(gameEntity != null) || num3 >= num4)
				{
					if (!this.CurrentMissionScene.RayCastForClosestEntityOrTerrain(vec2, vec2 + vec * num, ref num3, ref gameEntity, 0.2f * num5, 79617) || !(gameEntity != null) || num3 >= num4)
					{
						goto IL_381;
					}
				}
				for (;;)
				{
					if (gameEntity.GetScriptComponents().Any((ScriptComponentBehavior sc) => sc is IFocusable) || !(gameEntity.Parent != null))
					{
						break;
					}
					gameEntity = gameEntity.Parent;
				}
				usableMachine = gameEntity.GetFirstScriptOfType<UsableMachine>();
				if (usableMachine != null && !usableMachine.IsDisabled)
				{
					GameEntity validStandingPointForAgent = usableMachine.GetValidStandingPointForAgent(main);
					if (validStandingPointForAgent != null)
					{
						gameEntity = validStandingPointForAgent;
					}
				}
				flag = false;
				UsableMissionObject firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMissionObject>();
				if (firstScriptOfType is SpawnedItemEntity)
				{
					if (this.CurrentMission.IsMainAgentItemInteractionEnabled && main.CanReachObject(firstScriptOfType, MissionMainAgentInteractionComponent.GetCollisionDistanceSquaredOfIntersectionFromMainAgentEye(vec2, vec, num3)))
					{
						focusable = firstScriptOfType;
						if (main.CanUseObject(firstScriptOfType))
						{
							flag = true;
						}
					}
				}
				else if (firstScriptOfType != null)
				{
					focusable = firstScriptOfType;
					if (this.CurrentMission.IsMainAgentObjectInteractionEnabled && !main.IsUsingGameObject && main.IsOnLand() && main.ObjectHasVacantPosition(firstScriptOfType))
					{
						flag = true;
					}
				}
				else if (usableMachine != null)
				{
					focusable = usableMachine;
				}
				else
				{
					IFocusable focusable2 = gameEntity.GetScriptComponents().FirstOrDefault((ScriptComponentBehavior sc) => sc is IFocusable) as IFocusable;
					if (focusable2 != null)
					{
						focusable = focusable2;
					}
				}
				IL_381:
				if ((focusable == null || !flag) && main.MountAgent != null && main.CanInteractWithAgent(main.MountAgent, this.CurrentMissionScreen.CameraElevation))
				{
					focusable = main.MountAgent;
					flag = true;
				}
			}
			if (focusable == null)
			{
				this.ClearFocus();
				return;
			}
			bool flag3 = ((focusable is Agent) ? flag2 : flag);
			this.SetCurrentFocusedObject(focusable, usableMachine, flag3);
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0001EA14 File Offset: 0x0001CC14
		public void FocusStateCheckTick()
		{
			if (this.CurrentMissionScreen.SceneLayer.Input.IsGameKeyPressed(13) && this.CurrentMission.IsMainAgentObjectInteractionEnabled && !this.CurrentMission.IsOrderMenuOpen)
			{
				Agent main = Agent.Main;
				UsableMissionObject usableMissionObject;
				if ((usableMissionObject = this._currentInteractableObject as UsableMissionObject) != null)
				{
					if (!main.IsUsingGameObject && main.IsOnLand() && !(usableMissionObject is SpawnedItemEntity) && main.ObjectHasVacantPosition(usableMissionObject))
					{
						main.HandleStartUsingAction(usableMissionObject, -1);
						return;
					}
				}
				else
				{
					Agent agent = this._currentInteractableObject as Agent;
					if (main.IsOnLand() && agent != null)
					{
						agent.OnUse(main);
						return;
					}
					StandingPoint standingPoint;
					if (main.IsUsingGameObject && !(main.CurrentlyUsedGameObject is SpawnedItemEntity) && (agent == null || (standingPoint = main.CurrentlyUsedGameObject as StandingPoint) == null || !standingPoint.PlayerStopsUsingWhenInteractsWithOther))
					{
						main.HandleStopUsingAction();
						this.ClearFocus();
					}
				}
			}
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0001EAF8 File Offset: 0x0001CCF8
		public void FocusedItemHealthTick()
		{
			UsableMissionObject usableMissionObject;
			UsableMachine usableMachine;
			DestructableComponent destructableComponent;
			if ((usableMissionObject = this.CurrentFocusedObject as UsableMissionObject) != null)
			{
				GameEntity gameEntity = usableMissionObject.GameEntity;
				while (gameEntity != null && !gameEntity.HasScriptOfType<UsableMachine>())
				{
					gameEntity = gameEntity.Parent;
				}
				if (gameEntity != null)
				{
					UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
					if (((firstScriptOfType != null) ? firstScriptOfType.DestructionComponent : null) != null)
					{
						MissionMainAgentInteractionComponent.MissionFocusHealthChangeDelegate onFocusHealthChanged = this.OnFocusHealthChanged;
						if (onFocusHealthChanged == null)
						{
							return;
						}
						onFocusHealthChanged(this.CurrentFocusedObject, firstScriptOfType.DestructionComponent.HitPoint / firstScriptOfType.DestructionComponent.MaxHitPoint, true);
						return;
					}
				}
			}
			else if ((usableMachine = this.CurrentFocusedObject as UsableMachine) != null)
			{
				if (usableMachine.DestructionComponent != null)
				{
					MissionMainAgentInteractionComponent.MissionFocusHealthChangeDelegate onFocusHealthChanged2 = this.OnFocusHealthChanged;
					if (onFocusHealthChanged2 == null)
					{
						return;
					}
					onFocusHealthChanged2(this.CurrentFocusedObject, usableMachine.DestructionComponent.HitPoint / usableMachine.DestructionComponent.MaxHitPoint, true);
					return;
				}
			}
			else if ((destructableComponent = this.CurrentFocusedObject as DestructableComponent) != null)
			{
				MissionMainAgentInteractionComponent.MissionFocusHealthChangeDelegate onFocusHealthChanged3 = this.OnFocusHealthChanged;
				if (onFocusHealthChanged3 == null)
				{
					return;
				}
				onFocusHealthChanged3(this.CurrentFocusedObject, destructableComponent.HitPoint / destructableComponent.MaxHitPoint, true);
			}
		}

		// Token: 0x04000254 RID: 596
		private IFocusable _currentInteractableObject;

		// Token: 0x04000257 RID: 599
		private readonly MissionMainAgentController _mainAgentController;

		// Token: 0x020000BC RID: 188
		// (Invoke) Token: 0x0600055E RID: 1374
		public delegate void MissionFocusGainedEventDelegate(Agent agent, IFocusable focusableObject, bool isInteractable);

		// Token: 0x020000BD RID: 189
		// (Invoke) Token: 0x06000562 RID: 1378
		public delegate void MissionFocusLostEventDelegate(Agent agent, IFocusable focusableObject);

		// Token: 0x020000BE RID: 190
		// (Invoke) Token: 0x06000566 RID: 1382
		public delegate void MissionFocusHealthChangeDelegate(IFocusable focusable, float healthPercentage, bool hideHealthbarWhenFull);
	}
}
