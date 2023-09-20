using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BurstMissionObjectParticles : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public bool DoChildren { get; private set; }

		public BurstMissionObjectParticles(MissionObject missionObject, bool doChildren)
		{
			this.MissionObject = missionObject;
			this.DoChildren = doChildren;
		}

		public BurstMissionObjectParticles()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.DoChildren = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteBoolToPacket(this.DoChildren);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.Particles;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Burst MissionObject particles on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
