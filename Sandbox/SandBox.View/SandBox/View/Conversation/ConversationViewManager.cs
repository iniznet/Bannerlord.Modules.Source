using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.MountAndBlade.View.Screens;

namespace SandBox.View.Conversation
{
	// Token: 0x0200005A RID: 90
	public class ConversationViewManager
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060003E9 RID: 1001 RVA: 0x00021FBF File Offset: 0x000201BF
		public static ConversationViewManager Instance
		{
			get
			{
				return SandBoxViewSubModule.ConversationViewManager;
			}
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x00021FC8 File Offset: 0x000201C8
		public ConversationViewManager()
		{
			this.FillEventHandlers();
			Campaign.Current.ConversationManager.ConditionRunned += this.OnCondition;
			Campaign.Current.ConversationManager.ConsequenceRunned += this.OnConsequence;
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00022018 File Offset: 0x00020218
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

		// Token: 0x060003EC RID: 1004 RVA: 0x00022074 File Offset: 0x00020274
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

		// Token: 0x060003ED RID: 1005 RVA: 0x000221B8 File Offset: 0x000203B8
		private void OnConsequence(ConversationSentence sentence)
		{
			ConversationViewEventHandlerDelegate conversationViewEventHandlerDelegate;
			if (this._consequenceEventHandlers.TryGetValue(sentence.Id, out conversationViewEventHandlerDelegate))
			{
				conversationViewEventHandlerDelegate();
			}
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x000221E0 File Offset: 0x000203E0
		private void OnCondition(ConversationSentence sentence)
		{
			ConversationViewEventHandlerDelegate conversationViewEventHandlerDelegate;
			if (this._conditionEventHandlers.TryGetValue(sentence.Id, out conversationViewEventHandlerDelegate))
			{
				conversationViewEventHandlerDelegate();
			}
		}

		// Token: 0x04000218 RID: 536
		private Dictionary<string, ConversationViewEventHandlerDelegate> _conditionEventHandlers;

		// Token: 0x04000219 RID: 537
		private Dictionary<string, ConversationViewEventHandlerDelegate> _consequenceEventHandlers;
	}
}
