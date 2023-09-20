using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.MountAndBlade.View.Screens;

namespace SandBox.View.Conversation
{
	public class ConversationViewManager
	{
		public static ConversationViewManager Instance
		{
			get
			{
				return SandBoxViewSubModule.ConversationViewManager;
			}
		}

		public ConversationViewManager()
		{
			this.FillEventHandlers();
			Campaign.Current.ConversationManager.ConditionRunned += this.OnCondition;
			Campaign.Current.ConversationManager.ConsequenceRunned += this.OnConsequence;
		}

		private void FillEventHandlers()
		{
			this._conditionEventHandlers = new Dictionary<string, ConversationViewEventHandlerDelegate>();
			this._consequenceEventHandlers = new Dictionary<string, ConversationViewEventHandlerDelegate>();
			Assembly assembly = typeof(ConversationViewEventHandlerDelegate).Assembly;
			this.FillEventHandlersWith(assembly);
			foreach (Assembly assembly2 in GameStateScreenManager.GetViewAssemblies())
			{
				this.FillEventHandlersWith(assembly2);
			}
		}

		private void FillEventHandlersWith(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(ConversationViewEventHandler), false);
					if (customAttributes != null && customAttributes.Length != 0)
					{
						foreach (ConversationViewEventHandler conversationViewEventHandler in customAttributes)
						{
							ConversationViewEventHandlerDelegate conversationViewEventHandlerDelegate = Delegate.CreateDelegate(typeof(ConversationViewEventHandlerDelegate), methodInfo) as ConversationViewEventHandlerDelegate;
							if (conversationViewEventHandler.Type == ConversationViewEventHandler.EventType.OnCondition)
							{
								if (!this._conditionEventHandlers.ContainsKey(conversationViewEventHandler.Id))
								{
									this._conditionEventHandlers.Add(conversationViewEventHandler.Id, conversationViewEventHandlerDelegate);
								}
								else
								{
									this._conditionEventHandlers[conversationViewEventHandler.Id] = conversationViewEventHandlerDelegate;
								}
							}
							else if (conversationViewEventHandler.Type == ConversationViewEventHandler.EventType.OnConsequence)
							{
								if (!this._consequenceEventHandlers.ContainsKey(conversationViewEventHandler.Id))
								{
									this._consequenceEventHandlers.Add(conversationViewEventHandler.Id, conversationViewEventHandlerDelegate);
								}
								else
								{
									this._consequenceEventHandlers[conversationViewEventHandler.Id] = conversationViewEventHandlerDelegate;
								}
							}
						}
					}
				}
			}
		}

		private void OnConsequence(ConversationSentence sentence)
		{
			ConversationViewEventHandlerDelegate conversationViewEventHandlerDelegate;
			if (this._consequenceEventHandlers.TryGetValue(sentence.Id, out conversationViewEventHandlerDelegate))
			{
				conversationViewEventHandlerDelegate();
			}
		}

		private void OnCondition(ConversationSentence sentence)
		{
			ConversationViewEventHandlerDelegate conversationViewEventHandlerDelegate;
			if (this._conditionEventHandlers.TryGetValue(sentence.Id, out conversationViewEventHandlerDelegate))
			{
				conversationViewEventHandlerDelegate();
			}
		}

		private Dictionary<string, ConversationViewEventHandlerDelegate> _conditionEventHandlers;

		private Dictionary<string, ConversationViewEventHandlerDelegate> _consequenceEventHandlers;
	}
}
