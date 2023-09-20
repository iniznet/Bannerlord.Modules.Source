using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class UnselectSiegeWeapon : GameNetworkMessage
	{
		public SiegeWeapon SiegeWeapon { get; private set; }

		public UnselectSiegeWeapon(SiegeWeapon siegeWeapon)
		{
			this.SiegeWeapon = siegeWeapon;
		}

		public UnselectSiegeWeapon()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as SiegeWeapon;
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SiegeWeapon);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Deselect SiegeWeapon with ID: ",
				this.SiegeWeapon.Id,
				" and with name: ",
				this.SiegeWeapon.GameEntity.Name
			});
		}
	}
}
