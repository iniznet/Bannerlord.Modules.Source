using System;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AddMissionObjectBodyFlags : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public BodyFlags BodyFlags { get; private set; }

		public bool ApplyToChildren { get; private set; }

		public AddMissionObjectBodyFlags(MissionObject missionObject, BodyFlags bodyFlags, bool applyToChildren)
		{
			this.MissionObject = missionObject;
			this.BodyFlags = bodyFlags;
			this.ApplyToChildren = applyToChildren;
		}

		public AddMissionObjectBodyFlags()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.BodyFlags = (BodyFlags)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.FlagsCompressionInfo, ref flag);
			this.ApplyToChildren = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteIntToPacket((int)this.BodyFlags, CompressionBasic.FlagsCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.ApplyToChildren);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Add bodyflags: ",
				this.BodyFlags,
				" to MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name,
				this.ApplyToChildren ? "" : " and to all of its children."
			});
		}
	}
}
