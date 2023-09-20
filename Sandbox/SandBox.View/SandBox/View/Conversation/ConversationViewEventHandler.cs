using System;

namespace SandBox.View.Conversation
{
	// Token: 0x02000058 RID: 88
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class ConversationViewEventHandler : Attribute
	{
		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060003E2 RID: 994 RVA: 0x00021F99 File Offset: 0x00020199
		public string Id { get; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060003E3 RID: 995 RVA: 0x00021FA1 File Offset: 0x000201A1
		public ConversationViewEventHandler.EventType Type { get; }

		// Token: 0x060003E4 RID: 996 RVA: 0x00021FA9 File Offset: 0x000201A9
		public ConversationViewEventHandler(string id, ConversationViewEventHandler.EventType type)
		{
			this.Id = id;
			this.Type = type;
		}

		// Token: 0x02000095 RID: 149
		public enum EventType
		{
			// Token: 0x04000328 RID: 808
			OnCondition,
			// Token: 0x04000329 RID: 809
			OnConsequence
		}
	}
}
