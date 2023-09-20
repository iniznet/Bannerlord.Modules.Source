using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	public class MissionMainAgentInteractionComponent
	{
		public event MissionMainAgentInteractionComponent.MissionFocusGainedEventDelegate OnFocusGained;

		public event MissionMainAgentInteractionComponent.MissionFocusLostEventDelegate OnFocusLost;

		public event MissionMainAgentInteractionComponent.MissionFocusHealthChangeDelegate OnFocusHealthChanged;

		public IFocusable CurrentFocusedObject { get; private set; }

		public IFocusable CurrentFocusedMachine { get; private set; }

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

		public void OnClearScene()
		{
			this.ClearFocus();
		}

		private Mission CurrentMission
		{
			get
			{
				return this._mainAgentController.Mission;
			}
		}

		private MissionScreen CurrentMissionScreen
		{
			get
			{
				return this._mainAgentController.MissionScreen;
			}
		}

		private Scene CurrentMissionScene
		{
			get
			{
				return this._mainAgentController.Mission.Scene;
			}
		}

		public MissionMainAgentInteractionComponent(MissionMainAgentController mainAgentController)
		{
			this._mainAgentController = mainAgentController;
		}

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

		private IFocusable _currentInteractableObject;

		private readonly MissionMainAgentController _mainAgentController;

		public delegate void MissionFocusGainedEventDelegate(Agent agent, IFocusable focusableObject, bool isInteractable);

		public delegate void MissionFocusLostEventDelegate(Agent agent, IFocusable focusableObject);

		public delegate void MissionFocusHealthChangeDelegate(IFocusable focusable, float healthPercentage, bool hideHealthbarWhenFull);
	}
}
