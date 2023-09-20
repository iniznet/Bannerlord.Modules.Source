using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
	public class MessageContractHandlerManager
	{
		private Dictionary<byte, MessageContractHandler> MessageHandlers { get; set; }

		private Dictionary<byte, Type> MessageContractTypes { get; set; }

		public MessageContractHandlerManager()
		{
			this.MessageHandlers = new Dictionary<byte, MessageContractHandler>();
			this.MessageContractTypes = new Dictionary<byte, Type>();
		}

		public void AddMessageHandler<T>(MessageContractHandlerDelegate<T> handler) where T : MessageContract
		{
			MessageContractHandler<T> messageContractHandler = new MessageContractHandler<T>(handler);
			Type typeFromHandle = typeof(T);
			byte contractId = MessageContract.GetContractId(typeFromHandle);
			this.MessageContractTypes.Add(contractId, typeFromHandle);
			this.MessageHandlers.Add(contractId, messageContractHandler);
		}

		public void HandleMessage(MessageContract messageContract)
		{
			this.MessageHandlers[messageContract.MessageId].Invoke(messageContract);
		}

		public void HandleNetworkMessage(NetworkMessage networkMessage)
		{
			byte b = networkMessage.ReadByte();
			Type type = this.MessageContractTypes[b];
			MessageContract messageContract = MessageContract.CreateMessageContract(type);
			Debug.Print(string.Concat(new object[] { "Message with id: ", b, " / contract:", type, "received and processing..." }), 0, Debug.DebugColor.White, 17592186044416UL);
			messageContract.DeserializeFromNetworkMessage(networkMessage);
			this.HandleMessage(messageContract);
		}

		internal Type GetMessageContractType(byte id)
		{
			return this.MessageContractTypes[id];
		}

		public bool ContainsMessageHandler(byte id)
		{
			return this.MessageContractTypes.ContainsKey(id);
		}
	}
}
