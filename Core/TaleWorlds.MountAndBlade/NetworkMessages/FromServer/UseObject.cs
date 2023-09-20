using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000BE RID: 190
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class UseObject : GameNetworkMessage
	{
		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x060007DE RID: 2014 RVA: 0x0000E337 File Offset: 0x0000C537
		// (set) Token: 0x060007DF RID: 2015 RVA: 0x0000E33F File Offset: 0x0000C53F
		public Agent Agent { get; private set; }

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x060007E0 RID: 2016 RVA: 0x0000E348 File Offset: 0x0000C548
		// (set) Token: 0x060007E1 RID: 2017 RVA: 0x0000E350 File Offset: 0x0000C550
		public UsableMissionObject UsableGameObject { get; private set; }

		// Token: 0x060007E2 RID: 2018 RVA: 0x0000E359 File Offset: 0x0000C559
		public UseObject(Agent agent, UsableMissionObject usableGameObject)
		{
			this.Agent = agent;
			this.UsableGameObject = usableGameObject;
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x0000E36F File Offset: 0x0000C56F
		public UseObject()
		{
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x0000E378 File Offset: 0x0000C578
		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMissionObject;
			return flag;
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x0000E3A8 File Offset: 0x0000C5A8
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableGameObject);
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x0000E3C0 File Offset: 0x0000C5C0
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Agents | MultiplayerMessageFilter.MissionObjects;
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x0000E3C8 File Offset: 0x0000C5C8
		protected override string OnGetLogFormat()
		{
			string text = "Use UsableMissionObject with ID: ";
			if (this.UsableGameObject != null)
			{
				text += this.UsableGameObject.Id;
				text += "and name: ";
				if (this.UsableGameObject.GameEntity != null)
				{
					text += this.UsableGameObject.GameEntity.Name;
				}
				else
				{
					text += "log";
				}
			}
			else
			{
				text += "null";
			}
			text += " by Agent with name: ";
			if (this.Agent != null)
			{
				text = string.Concat(new object[]
				{
					text,
					this.Agent.Name,
					" and agent-index: ",
					this.Agent.Index
				});
			}
			else
			{
				text += "null";
			}
			return text;
		}
	}
}
