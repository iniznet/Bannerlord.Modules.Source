using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetStonePileAmmo : GameNetworkMessage
	{
		public StonePile StonePile { get; private set; }

		public int AmmoCount { get; private set; }

		public SetStonePileAmmo(StonePile stonePile, int ammoCount)
		{
			this.StonePile = stonePile;
			this.AmmoCount = ammoCount;
		}

		public SetStonePileAmmo()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.StonePile = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as StonePile;
			this.AmmoCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.StonePile);
			GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set ammo left to: ",
				this.AmmoCount,
				" on StonePile with ID: ",
				this.StonePile.Id,
				" and name: ",
				this.StonePile.GameEntity.Name
			});
		}
	}
}
