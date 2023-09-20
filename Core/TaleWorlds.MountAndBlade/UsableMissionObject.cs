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
	// Token: 0x02000383 RID: 899
	public abstract class UsableMissionObject : SynchedMissionObject, IFocusable, IUsable, IVisible
	{
		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x06003124 RID: 12580 RVA: 0x000CC047 File Offset: 0x000CA247
		public virtual FocusableObjectType FocusableObjectType
		{
			get
			{
				return FocusableObjectType.Item;
			}
		}

		// Token: 0x06003125 RID: 12581 RVA: 0x000CC04A File Offset: 0x000CA24A
		public virtual void OnUserConversationStart()
		{
		}

		// Token: 0x06003126 RID: 12582 RVA: 0x000CC04C File Offset: 0x000CA24C
		public virtual void OnUserConversationEnd()
		{
		}

		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x06003127 RID: 12583 RVA: 0x000CC04E File Offset: 0x000CA24E
		// (set) Token: 0x06003128 RID: 12584 RVA: 0x000CC056 File Offset: 0x000CA256
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

		// Token: 0x170008CE RID: 2254
		// (get) Token: 0x06003129 RID: 12585 RVA: 0x000CC080 File Offset: 0x000CA280
		// (set) Token: 0x0600312A RID: 12586 RVA: 0x000CC088 File Offset: 0x000CA288
		public Agent PreviousUserAgent { get; private set; }

		// Token: 0x170008CF RID: 2255
		// (get) Token: 0x0600312B RID: 12587 RVA: 0x000CC091 File Offset: 0x000CA291
		// (set) Token: 0x0600312C RID: 12588 RVA: 0x000CC099 File Offset: 0x000CA299
		public GameEntityWithWorldPosition GameEntityWithWorldPosition { get; private set; }

		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x0600312D RID: 12589 RVA: 0x000CC0A2 File Offset: 0x000CA2A2
		// (set) Token: 0x0600312E RID: 12590 RVA: 0x000CC0AA File Offset: 0x000CA2AA
		public virtual Agent MovingAgent { get; private set; }

		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x0600312F RID: 12591 RVA: 0x000CC0B3 File Offset: 0x000CA2B3
		// (set) Token: 0x06003130 RID: 12592 RVA: 0x000CC0BB File Offset: 0x000CA2BB
		public List<Agent> DefendingAgents { get; private set; }

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x06003131 RID: 12593 RVA: 0x000CC0C4 File Offset: 0x000CA2C4
		public bool HasDefendingAgent
		{
			get
			{
				return this.DefendingAgents != null && this.GetDefendingAgentCount() > 0;
			}
		}

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06003132 RID: 12594 RVA: 0x000CC0D9 File Offset: 0x000CA2D9
		// (set) Token: 0x06003133 RID: 12595 RVA: 0x000CC0E1 File Offset: 0x000CA2E1
		public bool IsInstantUse { get; protected set; }

		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x06003134 RID: 12596 RVA: 0x000CC0EA File Offset: 0x000CA2EA
		// (set) Token: 0x06003135 RID: 12597 RVA: 0x000CC0F4 File Offset: 0x000CA2F4
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

		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x06003136 RID: 12598 RVA: 0x000CC16D File Offset: 0x000CA36D
		// (set) Token: 0x06003137 RID: 12599 RVA: 0x000CC178 File Offset: 0x000CA378
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

		// Token: 0x06003138 RID: 12600 RVA: 0x000CC1C6 File Offset: 0x000CA3C6
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

		// Token: 0x06003139 RID: 12601 RVA: 0x000CC1F7 File Offset: 0x000CA3F7
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

		// Token: 0x0600313A RID: 12602 RVA: 0x000CC228 File Offset: 0x000CA428
		public virtual bool IsDisabledForAgent(Agent agent)
		{
			return this.IsDeactivated || agent.MountAgent != null || (this.IsDisabledForPlayers && !agent.IsAIControlled) || !agent.IsOnLand();
		}

		// Token: 0x0600313B RID: 12603 RVA: 0x000CC255 File Offset: 0x000CA455
		protected UsableMissionObject(bool isInstantUse = false)
		{
			this._components = new List<UsableMissionObjectComponent>();
			this.IsInstantUse = isInstantUse;
			this.GameEntityWithWorldPosition = null;
			this._needsSingleThreadTickOnce = false;
		}

		// Token: 0x0600313C RID: 12604 RVA: 0x000CC293 File Offset: 0x000CA493
		public void AddComponent(UsableMissionObjectComponent component)
		{
			this._components.Add(component);
			component.OnAdded(base.Scene);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x0600313D RID: 12605 RVA: 0x000CC2B9 File Offset: 0x000CA4B9
		public void RemoveComponent(UsableMissionObjectComponent component)
		{
			component.OnRemoved();
			this._components.Remove(component);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x0600313E RID: 12606 RVA: 0x000CC2DA File Offset: 0x000CA4DA
		public T GetComponent<T>() where T : UsableMissionObjectComponent
		{
			return this._components.Find((UsableMissionObjectComponent c) => c is T) as T;
		}

		// Token: 0x0600313F RID: 12607 RVA: 0x000CC310 File Offset: 0x000CA510
		private void CollectChildEntities()
		{
			this.CollectChildEntitiesAux(base.GameEntity);
		}

		// Token: 0x06003140 RID: 12608 RVA: 0x000CC320 File Offset: 0x000CA520
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

		// Token: 0x06003141 RID: 12609 RVA: 0x000CC384 File Offset: 0x000CA584
		public void RefreshGameEntityWithWorldPosition()
		{
			this.GameEntityWithWorldPosition = new GameEntityWithWorldPosition(base.GameEntity);
		}

		// Token: 0x06003142 RID: 12610 RVA: 0x000CC397 File Offset: 0x000CA597
		protected virtual void CollectChildEntity(GameEntity childEntity)
		{
		}

		// Token: 0x06003143 RID: 12611 RVA: 0x000CC399 File Offset: 0x000CA599
		protected virtual bool VerifyChildEntities(ref string errorMessage)
		{
			return true;
		}

		// Token: 0x06003144 RID: 12612 RVA: 0x000CC39C File Offset: 0x000CA59C
		protected internal override void OnInit()
		{
			base.OnInit();
			this.CollectChildEntities();
			this.LockUserFrames = !this.IsInstantUse;
			this.RefreshGameEntityWithWorldPosition();
		}

		// Token: 0x06003145 RID: 12613 RVA: 0x000CC3BF File Offset: 0x000CA5BF
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.CollectChildEntities();
		}

		// Token: 0x06003146 RID: 12614 RVA: 0x000CC3D0 File Offset: 0x000CA5D0
		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnMissionReset();
			}
		}

		// Token: 0x06003147 RID: 12615 RVA: 0x000CC428 File Offset: 0x000CA628
		public virtual void OnFocusGain(Agent userAgent)
		{
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnFocusGain(userAgent);
			}
		}

		// Token: 0x06003148 RID: 12616 RVA: 0x000CC47C File Offset: 0x000CA67C
		public virtual void OnFocusLose(Agent userAgent)
		{
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnFocusLose(userAgent);
			}
		}

		// Token: 0x06003149 RID: 12617 RVA: 0x000CC4D0 File Offset: 0x000CA6D0
		public virtual TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			return TextObject.Empty;
		}

		// Token: 0x0600314A RID: 12618 RVA: 0x000CC4D7 File Offset: 0x000CA6D7
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

		// Token: 0x0600314B RID: 12619 RVA: 0x000CC4FC File Offset: 0x000CA6FC
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

		// Token: 0x0600314C RID: 12620 RVA: 0x000CC644 File Offset: 0x000CA844
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

		// Token: 0x0600314D RID: 12621 RVA: 0x000CC678 File Offset: 0x000CA878
		public virtual void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnUseStopped(userAgent, isSuccessful);
			}
			this.UserAgent = null;
		}

		// Token: 0x0600314E RID: 12622 RVA: 0x000CC6D4 File Offset: 0x000CA8D4
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

		// Token: 0x0600314F RID: 12623 RVA: 0x000CC705 File Offset: 0x000CA905
		public virtual int GetMovingAgentCount()
		{
			if (this.MovingAgent == null)
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06003150 RID: 12624 RVA: 0x000CC712 File Offset: 0x000CA912
		public virtual Agent GetMovingAgentWithIndex(int index)
		{
			return this.MovingAgent;
		}

		// Token: 0x06003151 RID: 12625 RVA: 0x000CC71A File Offset: 0x000CA91A
		public virtual void RemoveMovingAgent(Agent movingAgent)
		{
			this.MovingAgent = null;
		}

		// Token: 0x06003152 RID: 12626 RVA: 0x000CC723 File Offset: 0x000CA923
		public virtual void AddMovingAgent(Agent movingAgent)
		{
			this.MovingAgent = movingAgent;
		}

		// Token: 0x06003153 RID: 12627 RVA: 0x000CC72C File Offset: 0x000CA92C
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

		// Token: 0x06003154 RID: 12628 RVA: 0x000CC75E File Offset: 0x000CA95E
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

		// Token: 0x06003155 RID: 12629 RVA: 0x000CC78F File Offset: 0x000CA98F
		public void InitializeDefendingAgents()
		{
			if (this.DefendingAgents == null)
			{
				this.DefendingAgents = new List<Agent>();
			}
		}

		// Token: 0x06003156 RID: 12630 RVA: 0x000CC7A4 File Offset: 0x000CA9A4
		public int GetDefendingAgentCount()
		{
			return this.DefendingAgents.Count;
		}

		// Token: 0x06003157 RID: 12631 RVA: 0x000CC7B1 File Offset: 0x000CA9B1
		public void AddDefendingAgent(Agent agent)
		{
			this.DefendingAgents.Add(agent);
		}

		// Token: 0x06003158 RID: 12632 RVA: 0x000CC7BF File Offset: 0x000CA9BF
		public void RemoveDefendingAgent(Agent agent)
		{
			this.DefendingAgents.Remove(agent);
		}

		// Token: 0x06003159 RID: 12633 RVA: 0x000CC7CE File Offset: 0x000CA9CE
		public void RemoveDefendingAgentAtIndex(int index)
		{
			this.DefendingAgents.RemoveAt(index);
		}

		// Token: 0x0600315A RID: 12634 RVA: 0x000CC7DC File Offset: 0x000CA9DC
		public bool IsAgentDefending(Agent agent)
		{
			return this.DefendingAgents.Contains(agent);
		}

		// Token: 0x0600315B RID: 12635 RVA: 0x000CC7EA File Offset: 0x000CA9EA
		private void StopAllDefenderAgents()
		{
			this.RemoveAllDefenderAgents();
		}

		// Token: 0x0600315C RID: 12636 RVA: 0x000CC7F4 File Offset: 0x000CA9F4
		private void RemoveAllDefenderAgents()
		{
			for (int i = this.GetDefendingAgentCount() - 1; i >= 0; i--)
			{
				this.RemoveDefendingAgentAtIndex(i);
			}
		}

		// Token: 0x0600315D RID: 12637 RVA: 0x000CC81B File Offset: 0x000CAA1B
		public virtual void SimulateTick(float dt)
		{
		}

		// Token: 0x0600315E RID: 12638 RVA: 0x000CC820 File Offset: 0x000CAA20
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

		// Token: 0x0600315F RID: 12639 RVA: 0x000CC8B4 File Offset: 0x000CAAB4
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

		// Token: 0x06003160 RID: 12640 RVA: 0x000CC8EC File Offset: 0x000CAAEC
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

		// Token: 0x06003161 RID: 12641 RVA: 0x000CCA28 File Offset: 0x000CAC28
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnEditorTick(dt);
			}
		}

		// Token: 0x06003162 RID: 12642 RVA: 0x000CCA80 File Offset: 0x000CAC80
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

		// Token: 0x06003163 RID: 12643 RVA: 0x000CCAE8 File Offset: 0x000CACE8
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				usableMissionObjectComponent.OnRemoved();
			}
		}

		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x06003164 RID: 12644 RVA: 0x000CCB40 File Offset: 0x000CAD40
		public virtual GameEntity InteractionEntity
		{
			get
			{
				return base.GameEntity;
			}
		}

		// Token: 0x06003165 RID: 12645 RVA: 0x000CCB48 File Offset: 0x000CAD48
		public virtual WorldFrame GetUserFrameForAgent(Agent agent)
		{
			return this.GameEntityWithWorldPosition.WorldFrame;
		}

		// Token: 0x06003166 RID: 12646 RVA: 0x000CCB58 File Offset: 0x000CAD58
		public override string ToString()
		{
			string text = base.GetType() + " with Components:";
			foreach (UsableMissionObjectComponent usableMissionObjectComponent in this._components)
			{
				text = string.Concat(new object[] { text, "[", usableMissionObjectComponent, "]" });
			}
			return text;
		}

		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x06003167 RID: 12647 RVA: 0x000CCBDC File Offset: 0x000CADDC
		public bool HasAIUser
		{
			get
			{
				return this.HasUser && this.UserAgent.IsAIControlled;
			}
		}

		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x06003168 RID: 12648 RVA: 0x000CCBF3 File Offset: 0x000CADF3
		public bool HasUser
		{
			get
			{
				return this.UserAgent != null;
			}
		}

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x06003169 RID: 12649 RVA: 0x000CCBFE File Offset: 0x000CADFE
		public virtual bool HasAIMovingTo
		{
			get
			{
				return this.MovingAgent != null;
			}
		}

		// Token: 0x0600316A RID: 12650 RVA: 0x000CCC09 File Offset: 0x000CAE09
		public virtual bool IsAIMovingTo(Agent agent)
		{
			return this.MovingAgent == agent;
		}

		// Token: 0x0600316B RID: 12651 RVA: 0x000CCC14 File Offset: 0x000CAE14
		public virtual bool HasUserPositionsChanged(Agent agent)
		{
			return (this.LockUserFrames || this.LockUserPositions) && base.GameEntity.GetHasFrameChanged();
		}

		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x0600316C RID: 12652 RVA: 0x000CCC33 File Offset: 0x000CAE33
		public virtual bool DisableCombatActionsOnUse
		{
			get
			{
				return !this.IsInstantUse;
			}
		}

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x0600316D RID: 12653 RVA: 0x000CCC3E File Offset: 0x000CAE3E
		// (set) Token: 0x0600316E RID: 12654 RVA: 0x000CCC46 File Offset: 0x000CAE46
		protected internal virtual bool LockUserFrames { get; set; }

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x0600316F RID: 12655 RVA: 0x000CCC4F File Offset: 0x000CAE4F
		// (set) Token: 0x06003170 RID: 12656 RVA: 0x000CCC57 File Offset: 0x000CAE57
		protected internal virtual bool LockUserPositions { get; set; }

		// Token: 0x06003171 RID: 12657 RVA: 0x000CCC60 File Offset: 0x000CAE60
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

		// Token: 0x06003172 RID: 12658 RVA: 0x000CCCA0 File Offset: 0x000CAEA0
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

		// Token: 0x06003173 RID: 12659 RVA: 0x000CCCF4 File Offset: 0x000CAEF4
		public virtual bool IsUsableByAgent(Agent userAgent)
		{
			return true;
		}

		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x06003174 RID: 12660 RVA: 0x000CCCF7 File Offset: 0x000CAEF7
		// (set) Token: 0x06003175 RID: 12661 RVA: 0x000CCD04 File Offset: 0x000CAF04
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
						Debug.FailedAssert("Unexpected component in UsableMissionObject", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Usables\\UsableMissionObject.cs", "IsVisible", 751);
						((IVisible)usableMissionObjectComponent).IsVisible = value;
					}
				}
			}
		}

		// Token: 0x06003176 RID: 12662 RVA: 0x000CCD8C File Offset: 0x000CAF8C
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

		// Token: 0x06003177 RID: 12663
		public abstract string GetDescriptionText(GameEntity gameEntity = null);

		// Token: 0x0400149D RID: 5277
		private Agent _userAgent;

		// Token: 0x040014A2 RID: 5282
		private readonly List<UsableMissionObjectComponent> _components;

		// Token: 0x040014A3 RID: 5283
		[EditableScriptComponentVariable(false)]
		public TextObject DescriptionMessage = TextObject.Empty;

		// Token: 0x040014A4 RID: 5284
		[EditableScriptComponentVariable(false)]
		public TextObject ActionMessage = TextObject.Empty;

		// Token: 0x040014A5 RID: 5285
		private bool _needsSingleThreadTickOnce;

		// Token: 0x040014A7 RID: 5287
		private bool _isDeactivated;

		// Token: 0x040014A8 RID: 5288
		private bool _isDisabledForPlayers;
	}
}
