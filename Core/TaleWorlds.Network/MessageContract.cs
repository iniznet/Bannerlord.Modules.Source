using System;
using System.Collections.Generic;

namespace TaleWorlds.Network
{
	public abstract class MessageContract
	{
		private static Dictionary<Type, byte> MessageContracts { get; set; } = new Dictionary<Type, byte>();

		private static Dictionary<Type, MessageContractCreator> MessageContractCreators { get; set; } = new Dictionary<Type, MessageContractCreator>();

		public byte MessageId
		{
			get
			{
				return MessageContract.MessageContracts[this._myType];
			}
		}

		internal static byte GetContractId(Type type)
		{
			MessageContract.InitializeMessageContract(type);
			return MessageContract.MessageContracts[type];
		}

		internal static MessageContractCreator GetContractCreator(Type type)
		{
			MessageContract.InitializeMessageContract(type);
			return MessageContract.MessageContractCreators[type];
		}

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

		protected MessageContract()
		{
			this._myType = base.GetType();
			MessageContract.InitializeMessageContract(this._myType);
		}

		public static MessageContract CreateMessageContract(Type messageContractType)
		{
			MessageContract.InitializeMessageContract(messageContractType);
			return MessageContract.MessageContractCreators[messageContractType].Invoke();
		}

		public abstract void SerializeToNetworkMessage(INetworkMessageWriter networkMessage);

		public abstract void DeserializeFromNetworkMessage(INetworkMessageReader networkMessage);

		private Type _myType;
	}
}
