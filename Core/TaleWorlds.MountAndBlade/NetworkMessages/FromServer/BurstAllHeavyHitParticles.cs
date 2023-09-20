using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class BurstAllHeavyHitParticles : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public BurstAllHeavyHitParticles(MissionObject missionObject)
		{
			this.MissionObject = missionObject;
		}

		public BurstAllHeavyHitParticles()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Bursting all heavy-hit particles for the DestructableComponent of MissionObject with Id: ",
				this.MissionObject.Id,
				" and name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
