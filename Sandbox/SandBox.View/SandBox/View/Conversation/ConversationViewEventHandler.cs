using System;

namespace SandBox.View.Conversation
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ConversationViewEventHandler : Attribute
	{
		public string Id { get; }

		public ConversationViewEventHandler.EventType Type { get; }

		public ConversationViewEventHandler(string id, ConversationViewEventHandler.EventType type)
		{
			this.Id = id;
			this.Type = type;
		}

		public enum EventType
		{
			OnCondition,
			OnConsequence
		}
	}
}
