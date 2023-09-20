using System;
using System.Collections.Generic;

namespace TaleWorlds.Network
{
	// Token: 0x02000017 RID: 23
	public abstract class MessageContract
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600007D RID: 125 RVA: 0x0000309A File Offset: 0x0000129A
		// (set) Token: 0x0600007E RID: 126 RVA: 0x000030A1 File Offset: 0x000012A1
		private static Dictionary<Type, byte> MessageContracts { get; set; } = new Dictionary<Type, byte>();

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000030A9 File Offset: 0x000012A9
		// (set) Token: 0x06000080 RID: 128 RVA: 0x000030B0 File Offset: 0x000012B0
		private static Dictionary<Type, MessageContractCreator> MessageContractCreators { get; set; } = new Dictionary<Type, MessageContractCreator>();

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000081 RID: 129 RVA: 0x000030B8 File Offset: 0x000012B8
		public byte MessageId
		{
			get
			{
				return MessageContract.MessageContracts[this._myType];
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000030E0 File Offset: 0x000012E0
		internal static byte GetContractId(Type type)
		{
			MessageContract.InitializeMessageContract(type);
			return MessageContract.MessageContracts[type];
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000030F3 File Offset: 0x000012F3
		internal static MessageContractCreator GetContractCreator(Type type)
		{
			MessageContract.InitializeMessageContract(type);
			return MessageContract.MessageContractCreators[type];
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003108 File Offset: 0x00001308
		private static void InitializeMessageContract(Type type)
		{
			if (!MessageContract.MessageContracts.ContainsKey(type))
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(MessageId), true);
				if (customAttributes.Length == 1)
				{
					MessageId messageId = customAttributes[0] as MessageId;
					Dictionary<Type, byte> messageContracts = MessageContract.MessageContracts;
					lock (messageContracts)
					{
						if (!MessageContract.MessageContracts.ContainsKey(type))
						{
							MessageContract.MessageContracts.Add(type, messageId.Id);
							Type typeFromHandle = typeof(MessageContractCreator<>);
							Type[] array = new Type[] { type };
							MessageContractCreator messageContractCreator = Activator.CreateInstance(typeFromHandle.MakeGenericType(array)) as MessageContractCreator;
							MessageContract.MessageContractCreators.Add(type, messageContractCreator);
						}
					}
				}
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000031CC File Offset: 0x000013CC
		protected MessageContract()
		{
			this._myType = base.GetType();
			MessageContract.InitializeMessageContract(this._myType);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000031EB File Offset: 0x000013EB
		public static MessageContract CreateMessageContract(Type messageContractType)
		{
			MessageContract.InitializeMessageContract(messageContractType);
			return MessageContract.MessageContractCreators[messageContractType].Invoke();
		}

		// Token: 0x06000088 RID: 136
		public abstract void SerializeToNetworkMessage(INetworkMessageWriter networkMessage);

		// Token: 0x06000089 RID: 137
		public abstract void DeserializeFromNetworkMessage(INetworkMessageReader networkMessage);

		// Token: 0x04000037 RID: 55
		private Type _myType;
	}
}
