using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public abstract class UsableMissionObject : SynchedMissionObject, IFocusable, IUsable, IVisible
	{
		public virtual FocusableObjectType FocusableObjectType
		{
			get
			{
				return FocusableObjectType.Item;
			}
		}

		public virtual void OnUserConversationStart()
		{
		}

		public virtual void OnUserConversationEnd()
		{
		}

		public Agent UserAgent
		{
			get
			{
				return this._userAgent;
			}
			private set
			{
				if (this._userAgent != value)
				{
					this.PreviousUserAgent = this._userAgent;
					this._userAgent = value;
					base.SetScriptComponentToTickMT(this.GetTickRequirement());
				}
			}
		}

		public Agent PreviousUserAgent { get; private set; }

		public GameEntityWithWorldPosition GameEntityWithWorldPosition { get; private set; }

		public virtual Agent MovingAgent { get; private set; }

		public List<Agent> DefendingAgents { get; private set; }

		public bool HasDefendingAgent
		{
			get
			{
				return this.DefendingAgents != null && this.GetDefendingAgentCount() > 0;
			}
		}

		public bool IsInstantUse { get; protected set; }

		public bool IsDeactivated
		{
			get
			{
				return this._isDeactivated;
			}
			set
			{
				if (value != this._isDeactivated)
				{
					this._isDeactivated = value;
					if (this._isDeactivated && !GameNetwork.IsClientOrReplay)
					{
						Agent userAgent = this.UserAgent;
						if (userAgent != null)
						{
							userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
						}
						bool flag = false;
						while (this.HasAIMovingTo)
						{
							this.MovingAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
							flag = true;
						}
						if (this.HasDefendingAgent)
						{
							this.StopAllDefenderAgents();
							flag = true;
						}
						if (flag)
						{
							base.SetScriptComponentToTick(this.GetTickRequirement());
						}
					}
				}
			}
		}

		public bool IsDisabledForPlayers
		{
			get
			{
				return this._isDisabledForPlayers;
			}
			set
			{
				if (value != this._isDisabledForPlayers)
				{
					this._isDisabledForPlayers = value;
					if (this._isDisabledForPlayers && !GameNetwork.IsClientOrReplay && this.UserAgent != null && !this.UserAgent.IsAIControlled)
					{
						this.UserAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
				}
			}
		}

		public void SetIsDeactivatedSynched(bool value)
		{
			if (this.IsDeactivated != value)
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetUsableMissionObjectIsDeactivated(this, value));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
				this.IsDeactivated = value;
			}
		}

		public void SetIsDisabledForPlayersSynched(bool value)
		{
			if (this.IsDisabledForPlayers != value)
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetUsableMissionObjectIsDisabledForPlayers(this, value));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
				this.IsDisabledForPlayers = value;
			}
		}

		public virtual bool IsDisabledForAgent(Agent agent)
		{
			return this.IsDeactivated || agent.MountAgent != null || (this.IsDisabledForPlayers && !agent.IsAIControlled) || !agent.IsOnLand();
		}

		protected UsableMissionObject(bool isInstantUse = false)
		{
			this._components = new List<UsableMissionObjectComponent>();
			this.IsInstantUse = isInstantUse;
			this.GameEntityWithWorldPosition = null;
			this._needsSingleThreadTickOnce = false;
		}

		public void AddComponent(UsableMissionObjectComponent component)
		{
			this._components.Add(component);
			component.OnAdded(base.Scene);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public void RemoveComponent(UsableMissionObjectComponent component)
		{
			component.OnRemoved();
			this._components.Remove(component);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public T GetComponent<T>() where T : UsableMissionObjectComponent
		{
			return this._components.Find((UsableMissionObjectComponent c) => c is T) as T;
		}

		private void CollectChildEntities()
		{
			this.CollectChildEntitiesAux(base.GameEntity);
		}

		private void CollectChildEntitiesAux(GameEntity entity)
		{
			foreach (GameEntity gameEntity in entity.GetChildren())
			{
				this.CollectChildEntity(gameEntity);
				if (gameEntity.GetScriptComponents().IsEmpty<ScriptComponentBehavior>())
				{
					this.CollectChildEntitiesAux(gameEntity);
				}
			}
		}

		public void RefreshGameEntityWithWorldPosition()
		{
			this.GameEntityWithWorldPosition = new GameEntityWithWorldPosition(base.GameEntity);
		}

		protected virtual void CollectChildEntity(GameEntity childEntity)
		{
		}

		protected virtual bool VerifyChildEntities(ref string errorMessage)
		{
			return true;
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this.CollectChildEntities();
			this.LockUserFrames = !this.IsInstantUse;
			this.RefreshGameEntityWithWorldPosition();
		}

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.CollectChildEntities();
		}

		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnMissionReset();
			}
		}

		public virtual void OnFocusGain(Agent userAgent)
		{
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnFocusGain(userAgent);
			}
		}

		public virtual void OnFocusLose(Agent userAgent)
		{
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnFocusLose(userAgent);
			}
		}

		public virtual TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			return TextObject.Empty;
		}

		public virtual void SetUserForClient(Agent userAgent)
		{
			Agent userAgent2 = this.UserAgent;
			if (userAgent2 != null)
			{
				userAgent2.SetUsedGameObjectForClient(null);
			}
			this.UserAgent = userAgent;
			if (userAgent != null)
			{
				userAgent.SetUsedGameObjectForClient(this);
			}
		}

		public virtual void OnUse(Agent userAgent)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				if (!userAgent.IsAIControlled && this.HasAIUser)
				{
					this.UserAgent.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
				}
				if (this.IsAIMovingTo(userAgent))
				{
					Formation formation = userAgent.Formation;
					if (formation != null)
					{
						formation.Team.DetachmentManager.RemoveAgentAsMovingToDetachment(userAgent);
					}
					this.RemoveMovingAgent(userAgent);
					base.SetScriptComponentToTick(this.GetTickRequirement());
				}
				while (this.HasAIMovingTo && !this.IsInstantUse)
				{
					this.MovingAgent.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
				}
				foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
				{
					usableMissionObjectComponent.OnUse(userAgent);
				}
				this.UserAgent = userAgent;
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new UseObject(userAgent, this));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
					return;
				}
			}
			else
			{
				if (this.LockUserFrames)
				{
					WorldFrame userFrameForAgent = this.GetUserFrameForAgent(userAgent);
					userAgent.SetTargetPositionAndDirection(userFrameForAgent.Origin.AsVec2, userFrameForAgent.Rotation.f);
					return;
				}
				if (this.LockUserPositions)
				{
					userAgent.SetTargetPosition(this.GetUserFrameForAgent(userAgent).Origin.AsVec2);
				}
			}
		}

		public virtual void OnAIMoveToUse(Agent userAgent, IDetachment detachment)
		{
			this.AddMovingAgent(userAgent);
			Formation formation = userAgent.Formation;
			if (formation != null)
			{
				formation.Team.DetachmentManager.AddAgentAsMovingToDetachment(userAgent, detachment);
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public virtual void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnUseStopped(userAgent, isSuccessful);
			}
			this.UserAgent = null;
		}

		public virtual void OnMoveToStopped(Agent movingAgent)
		{
			Formation formation = movingAgent.Formation;
			if (formation != null)
			{
				formation.Team.DetachmentManager.RemoveAgentAsMovingToDetachment(movingAgent);
			}
			this.RemoveMovingAgent(movingAgent);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public virtual int GetMovingAgentCount()
		{
			if (this.MovingAgent == null)
			{
				return 0;
			}
			return 1;
		}

		public virtual Agent GetMovingAgentWithIndex(int index)
		{
			return this.MovingAgent;
		}

		public virtual void RemoveMovingAgent(Agent movingAgent)
		{
			this.MovingAgent = null;
		}

		public virtual void AddMovingAgent(Agent movingAgent)
		{
			this.MovingAgent = movingAgent;
		}

		public void OnAIDefendBegin(Agent agent, IDetachment detachment)
		{
			this.AddDefendingAgent(agent);
			Formation formation = agent.Formation;
			if (formation != null)
			{
				formation.Team.DetachmentManager.AddAgentAsDefendingToDetachment(agent, detachment);
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public void OnAIDefendEnd(Agent agent)
		{
			Formation formation = agent.Formation;
			if (formation != null)
			{
				formation.Team.DetachmentManager.RemoveAgentAsDefendingToDetachment(agent);
			}
			this.RemoveDefendingAgent(agent);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public void InitializeDefendingAgents()
		{
			if (this.DefendingAgents == null)
			{
				this.DefendingAgents = new List<Agent>();
			}
		}

		public int GetDefendingAgentCount()
		{
			return this.DefendingAgents.Count;
		}

		public void AddDefendingAgent(Agent agent)
		{
			this.DefendingAgents.Add(agent);
		}

		public void RemoveDefendingAgent(Agent agent)
		{
			this.DefendingAgents.Remove(agent);
		}

		public void RemoveDefendingAgentAtIndex(int index)
		{
			this.DefendingAgents.RemoveAt(index);
		}

		public bool IsAgentDefending(Agent agent)
		{
			return this.DefendingAgents.Contains(agent);
		}

		private void StopAllDefenderAgents()
		{
			this.RemoveAllDefenderAgents();
		}

		private void RemoveAllDefenderAgents()
		{
			for (int i = this.GetDefendingAgentCount() - 1; i >= 0; i--)
			{
				this.RemoveDefendingAgentAtIndex(i);
			}
		}

		public virtual void SimulateTick(float dt)
		{
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (this.HasUser || this.HasAIMovingTo)
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel2;
			}
			if (this.HasDefendingAgent)
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick;
			}
			using (List<UsableMissionObjectComponent>.Enumerator enumerator = this._components.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsOnTickRequired())
					{
						return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick;
					}
				}
			}
			return base.GetTickRequirement();
		}

		protected internal override void OnTickParallel2(float dt)
		{
			for (int i = this.GetMovingAgentCount() - 1; i >= 0; i--)
			{
				if (!this.GetMovingAgentWithIndex(i).IsActive())
				{
					this._needsSingleThreadTickOnce = true;
				}
			}
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnTick(dt);
			}
			if (this.HasUser && this.HasUserPositionsChanged(this.UserAgent))
			{
				if (this.LockUserFrames)
				{
					WorldFrame userFrameForAgent = this.GetUserFrameForAgent(this.UserAgent);
					this.UserAgent.SetTargetPositionAndDirection(userFrameForAgent.Origin.AsVec2, userFrameForAgent.Rotation.f);
				}
				else if (this.LockUserPositions)
				{
					this.UserAgent.SetTargetPosition(this.GetUserFrameForAgent(this.UserAgent).Origin.AsVec2);
				}
			}
			if (this._needsSingleThreadTickOnce)
			{
				this._needsSingleThreadTickOnce = false;
				for (int i = this.GetMovingAgentCount() - 1; i >= 0; i--)
				{
					Agent movingAgentWithIndex = this.GetMovingAgentWithIndex(i);
					if (!movingAgentWithIndex.IsActive())
					{
						Formation formation = movingAgentWithIndex.Formation;
						if (formation != null)
						{
							formation.Team.DetachmentManager.RemoveAgentAsMovingToDetachment(movingAgentWithIndex);
						}
						this.RemoveMovingAgent(movingAgentWithIndex);
						base.SetScriptComponentToTick(this.GetTickRequirement());
					}
				}
			}
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnEditorTick(dt);
			}
		}

		protected internal override void OnEditorValidate()
		{
			base.OnEditorValidate();
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnEditorValidate();
			}
			string text = null;
			if (!this.VerifyChildEntities(ref text))
			{
				MBDebug.ShowWarning(text);
			}
		}

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnRemoved();
			}
		}

		public virtual GameEntity InteractionEntity
		{
			get
			{
				return base.GameEntity;
			}
		}

		public virtual WorldFrame GetUserFrameForAgent(Agent agent)
		{
			return this.GameEntityWithWorldPosition.WorldFrame;
		}

		public override string ToString()
		{
			string text = base.GetType() + " with Components:";
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				text = string.Concat(new object[] { text, "[", usableMissionObjectComponent, "]" });
			}
			return text;
		}

		public bool HasAIUser
		{
			get
			{
				return this.HasUser && this.UserAgent.IsAIControlled;
			}
		}

		public bool HasUser
		{
			get
			{
				return this.UserAgent != null;
			}
		}

		public virtual bool HasAIMovingTo
		{
			get
			{
				return this.MovingAgent != null;
			}
		}

		public virtual bool IsAIMovingTo(Agent agent)
		{
			return this.MovingAgent == agent;
		}

		public virtual bool HasUserPositionsChanged(Agent agent)
		{
			return (this.LockUserFrames || this.LockUserPositions) && base.GameEntity.GetHasFrameChanged();
		}

		public virtual bool DisableCombatActionsOnUse
		{
			get
			{
				return !this.IsInstantUse;
			}
		}

		protected internal virtual bool LockUserFrames { get; set; }

		protected internal virtual bool LockUserPositions { get; set; }

		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteBoolToPacket(this.IsDeactivated);
			GameNetworkMessage.WriteBoolToPacket(this.IsDisabledForPlayers);
			GameNetworkMessage.WriteBoolToPacket(this.UserAgent != null);
			if (this.UserAgent != null)
			{
				GameNetworkMessage.WriteAgentReferenceToPacket(this.UserAgent);
			}
		}

		public override bool ReadFromNetwork()
		{
			bool flag = base.ReadFromNetwork();
			bool flag2 = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			bool flag3 = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			Agent agent = null;
			if (GameNetworkMessage.ReadBoolFromPacket(ref flag))
			{
				agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			}
			if (flag)
			{
				this.IsDeactivated = flag2;
				this.IsDisabledForPlayers = flag3;
				if (agent != null)
				{
					this.SetUserForClient(agent);
				}
			}
			return flag;
		}

		public virtual bool IsUsableByAgent(Agent userAgent)
		{
			return true;
		}

		public bool IsVisible
		{
			get
			{
				return base.GameEntity.IsVisibleIncludeParents();
			}
			set
			{
				base.GameEntity.SetVisibilityExcludeParents(value);
				foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
				{
					if (usableMissionObjectComponent is IVisible)
					{
						Debug.FailedAssert("Unexpected component in UsableMissionObject", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Usables\\UsableMissionObject.cs", "IsVisible", 763);
						((IVisible)usableMissionObjectComponent).IsVisible = value;
					}
				}
			}
		}

		public override void OnEndMission()
		{
			this.UserAgent = null;
			for (int i = this.GetMovingAgentCount() - 1; i >= 0; i--)
			{
				this.RemoveMovingAgent(this.GetMovingAgentWithIndex(i));
			}
			if (this.HasDefendingAgent)
			{
				this.RemoveAllDefenderAgents();
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public abstract string GetDescriptionText(GameEntity gameEntity = null);

		private Agent _userAgent;

		private readonly List<UsableMissionObjectComponent> _components;

		[EditableScriptComponentVariable(false)]
		public TextObject DescriptionMessage = TextObject.Empty;

		[EditableScriptComponentVariable(false)]
		public TextObject ActionMessage = TextObject.Empty;

		private bool _needsSingleThreadTickOnce;

		private bool _isDeactivated;

		private bool _isDisabledForPlayers;
	}
}
