using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class FlagRaisingStatus : GameNetworkMessage
	{
		public float Progress { get; private set; }

		public CaptureTheFlagFlagDirection Direction { get; private set; }

		public float Speed { get; private set; }

		public FlagRaisingStatus()
		{
		}

		public FlagRaisingStatus(float currProgress, CaptureTheFlagFlagDirection direction, float speed)
		{
			this.Progress = currProgress;
			this.Direction = direction;
			this.Speed = speed;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Progress = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagClassicProgressCompressionInfo, ref flag);
			this.Direction = (CaptureTheFlagFlagDirection)GameNetworkMessage.ReadIntFromPacket(CompressionMission.FlagDirectionEnumCompressionInfo, ref flag);
			if (flag && this.Direction != CaptureTheFlagFlagDirection.None && this.Direction != CaptureTheFlagFlagDirection.Static)
			{
				this.Speed = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagSpeedCompressionInfo, ref flag);
			}
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteFloatToPacket(this.Progress, CompressionMission.FlagClassicProgressCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.Direction, CompressionMission.FlagDirectionEnumCompressionInfo);
			if (this.Direction != CaptureTheFlagFlagDirection.None && this.Direction != CaptureTheFlagFlagDirection.Static)
			{
				GameNetworkMessage.WriteFloatToPacket(this.Speed, CompressionMission.FlagSpeedCompressionInfo);
			}
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Updating flag movement: Progress: ", this.Progress, ", Direction: ", this.Direction, ", Speed: ", this.Speed });
		}
	}
}
