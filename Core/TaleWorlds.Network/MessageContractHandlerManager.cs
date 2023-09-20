using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
	// Token: 0x0200001A RID: 26
	public class MessageContractHandlerManager
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000321F File Offset: 0x0000141F
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00003227 File Offset: 0x00001427
		private Dictionary<byte, MessageContractHandler> MessageHandlers { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00003230 File Offset: 0x00001430
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00003238 File Offset: 0x00001438
		private Dictionary<byte, Type> MessageContractTypes { get; set; }

		// Token: 0x06000092 RID: 146 RVA: 0x00003241 File Offset: 0x00001441
		public MessageContractHandlerManager()
		{
			this.MessageHandlers = new Dictionary<byte, MessageContractHandler>();
			this.MessageContractTypes = new Dictionary<byte, Type>();
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00003260 File Offset: 0x00001460
		public void AddMessageHandler<T>(MessageContractHandlerDelegate<T> handler) where T : MessageContract
		{
			MessageContractHandler<T> messageContractHandler = new MessageContractHandler<T>(handler);
			Type typeFromHandle = typeof(T);
			byte contractId = MessageContract.GetContractId(typeFromHandle);
			this.MessageContractTypes.Add(contractId, typeFromHandle);
			this.MessageHandlers.Add(contractId, messageContractHandler);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000032A0 File Offset: 0x000014A0
		public void HandleMessage(MessageContract messageContract)
		{
			this.MessageHandlers[messageContract.MessageId].Invoke(messageContract);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000032BC File Offset: 0x000014BC
		public void HandleNetworkMessage(NetworkMessage networkMessage)
		{
			byte b = networkMessage.ReadByte();
			Type type = this.MessageContractTypes[b];
			MessageContract messageContract = MessageContract.CreateMessageContract(type);
			Debug.Print(string.Concat(new object[] { "Message with id: ", b, " / contract:", type, "received and processing..." }), 0, Debug.DebugColor.White, 17592186044416UL);
			messageContract.DeserializeFromNetworkMessage(networkMessage);
			this.HandleMessage(messageContract);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003333 File Offset: 0x00001533
		internal Type GetMessageContractType(byte id)
		{
			return this.MessageContractTypes[id];
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003341 File Offset: 0x00001541
		public bool ContainsMessageHandler(byte id)
		{
			return this.MessageContractTypes.ContainsKey(id);
		}
	}
}
