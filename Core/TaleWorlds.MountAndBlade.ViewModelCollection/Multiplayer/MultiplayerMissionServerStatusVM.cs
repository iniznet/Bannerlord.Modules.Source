using System;
using NetworkMessages.FromServer;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	public class MultiplayerMissionServerStatusVM : ViewModel
	{
		public void UpdatePacketLossRatio(float v)
		{
			if (v >= 0.02f)
			{
				this.PacketLossState = 2;
				return;
			}
			if (v >= 0.01f)
			{
				this.PacketLossState = 1;
				return;
			}
			this.PacketLossState = 0;
		}

		public void UpdatePeerPing(double averagePingInMilliseconds)
		{
			if (averagePingInMilliseconds >= 110.0)
			{
				this.PingState = 2;
				return;
			}
			if (averagePingInMilliseconds >= 90.0)
			{
				this.PingState = 1;
				return;
			}
			this.PingState = 0;
		}

		public void UpdateServerPerformanceState(ServerPerformanceState serverPerformanceState)
		{
			switch (serverPerformanceState)
			{
			default:
				this.ServerPerformanceState = 0;
				return;
			case NetworkMessages.FromServer.ServerPerformanceState.Medium:
				this.ServerPerformanceState = 1;
				return;
			case NetworkMessages.FromServer.ServerPerformanceState.Low:
			case NetworkMessages.FromServer.ServerPerformanceState.Count:
				this.ServerPerformanceState = 2;
				return;
			}
		}

		public void ResetStates()
		{
			this.PacketLossState = 0;
			this.PingState = 0;
			this.ServerPerformanceState = 0;
		}

		[DataSourceProperty]
		public int PacketLossState
		{
			get
			{
				return this._packetLossState;
			}
			set
			{
				if (value != this._packetLossState)
				{
					this._packetLossState = value;
					base.OnPropertyChangedWithValue(value, "PacketLossState");
				}
			}
		}

		[DataSourceProperty]
		public int PingState
		{
			get
			{
				return this._pingState;
			}
			set
			{
				if (value != this._pingState)
				{
					this._pingState = value;
					base.OnPropertyChangedWithValue(value, "PingState");
				}
			}
		}

		[DataSourceProperty]
		public int ServerPerformanceState
		{
			get
			{
				return this._serverPerformanceState;
			}
			set
			{
				if (value != this._serverPerformanceState)
				{
					this._serverPerformanceState = value;
					base.OnPropertyChangedWithValue(value, "ServerPerformanceState");
				}
			}
		}

		private int _packetLossState;

		private int _pingState;

		private int _serverPerformanceState;

		private enum StatusTypes
		{
			Good,
			Average,
			Poor
		}
	}
}
