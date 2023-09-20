using System;
using TaleWorlds.Engine.Options;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class SyncRelevantGameOptionsToServer : GameNetworkMessage
	{
		public bool SendMeBloodEvents { get; private set; }

		public bool SendMeSoundEvents { get; private set; }

		public SyncRelevantGameOptionsToServer()
		{
			this.SendMeBloodEvents = true;
			this.SendMeSoundEvents = true;
		}

		public void InitializeOptions()
		{
			this.SendMeBloodEvents = BannerlordConfig.ShowBlood;
			this.SendMeSoundEvents = NativeOptions.GetConfig(NativeOptions.NativeOptionsType.SoundVolume) > 0.01f && NativeOptions.GetConfig(NativeOptions.NativeOptionsType.MasterVolume) > 0.01f;
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.SendMeBloodEvents = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.SendMeSoundEvents = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteBoolToPacket(this.SendMeBloodEvents);
			GameNetworkMessage.WriteBoolToPacket(this.SendMeSoundEvents);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.General;
		}

		protected override string OnGetLogFormat()
		{
			return "SyncRelevantGameOptionsToServer";
		}
	}
}
