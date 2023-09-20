﻿using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	public class QuestTaskBase
	{
		internal static void AutoGeneratedStaticCollectObjectsQuestTaskBase(object o, List<object> collectedObjects)
		{
			((QuestTaskBase)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		internal static object AutoGeneratedGetMemberValueIsLogged(object o)
		{
			return ((QuestTaskBase)o).IsLogged;
		}

		internal static object AutoGeneratedGetMemberValueIsActive(object o)
		{
			return ((QuestTaskBase)o).IsActive;
		}

		[SaveableProperty(1)]
		public bool IsLogged { get; set; }

		[SaveableProperty(2)]
		public bool IsActive { get; private set; }

		public QuestTaskBase(DialogFlow dialogFlow = null, Action onSucceedAction = null, Action onFailedAction = null, Action onCanceledAction = null)
		{
			this.IsLogged = true;
			this.IsActive = true;
			this._onSucceededAction = onSucceedAction;
			this._onFailedAction = onFailedAction;
			this._onCanceledAction = onCanceledAction;
			this._dialogFlow = dialogFlow;
			if (this._dialogFlow != null)
			{
				Campaign.Current.ConversationManager.AddDialogFlow(this._dialogFlow, this);
			}
		}

		public void Finish(QuestTaskBase.FinishStates finishState)
		{
			this.IsActive = false;
			if (this._dialogFlow != null)
			{
				Campaign.Current.ConversationManager.RemoveRelatedLines(this);
				this._dialogFlow = null;
			}
			switch (finishState)
			{
			case QuestTaskBase.FinishStates.Success:
				if (this._onSucceededAction != null)
				{
					this._onSucceededAction();
				}
				break;
			case QuestTaskBase.FinishStates.Fail:
				if (this._onFailedAction != null)
				{
					this._onFailedAction();
				}
				break;
			case QuestTaskBase.FinishStates.Cancel:
				if (this._onCanceledAction != null)
				{
					this._onCanceledAction();
				}
				break;
			}
			this._onSucceededAction = null;
			this._onFailedAction = null;
			this._onCanceledAction = null;
			CampaignEventDispatcher.Instance.RemoveListeners(this);
			this.OnFinished();
		}

		public void AddTaskDialogs()
		{
			if (this._dialogFlow != null)
			{
				Campaign.Current.ConversationManager.AddDialogFlow(this._dialogFlow, this);
			}
		}

		protected virtual void OnFinished()
		{
		}

		public virtual void SetReferences()
		{
		}

		public void AddTaskDialogOnGameLoaded(DialogFlow dialogFlow)
		{
			if (this.IsActive)
			{
				this._dialogFlow = dialogFlow;
			}
		}

		public void AddTaskBehaviorsOnGameLoad(Action onSucceededAction = null, Action onFailedAction = null, Action onCanceledAction = null)
		{
			this._onSucceededAction = onSucceededAction;
			this._onFailedAction = onFailedAction;
			this._onCanceledAction = onCanceledAction;
		}

		protected Dictionary<EventDelegateType, TriggerDelegateType> EvenTriggerDelegates;

		private DialogFlow _dialogFlow;

		private Action _onSucceededAction;

		private Action _onFailedAction;

		private Action _onCanceledAction;

		public enum FinishStates
		{
			Success,
			Fail,
			Cancel
		}
	}
}