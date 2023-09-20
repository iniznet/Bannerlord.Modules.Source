using System;
using NetworkMessages.FromServer;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000044 RID: 68
	public class MultiplayerMissionServerStatusVM : ViewModel
	{
		// Token: 0x060005C5 RID: 1477 RVA: 0x00018903 File Offset: 0x00016B03
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

		// Token: 0x060005C6 RID: 1478 RVA: 0x0001892C File Offset: 0x00016B2C
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

		// Token: 0x060005C7 RID: 1479 RVA: 0x0001895D File Offset: 0x00016B5D
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

		// Token: 0x060005C8 RID: 1480 RVA: 0x0001898C File Offset: 0x00016B8C
		public void ResetStates()
		{
			this.PacketLossState = 0;
			this.PingState = 0;
			this.ServerPerformanceState = 0;
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x000189A3 File Offset: 0x00016BA3
		// (set) Token: 0x060005CA RID: 1482 RVA: 0x000189AB File Offset: 0x00016BAB
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

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x060005CB RID: 1483 RVA: 0x000189C9 File Offset: 0x00016BC9
		// (set) Token: 0x060005CC RID: 1484 RVA: 0x000189D1 File Offset: 0x00016BD1
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

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x060005CD RID: 1485 RVA: 0x000189EF File Offset: 0x00016BEF
		// (set) Token: 0x060005CE RID: 1486 RVA: 0x000189F7 File Offset: 0x00016BF7
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

		// Token: 0x040002F2 RID: 754
		private int _packetLossState;

		// Token: 0x040002F3 RID: 755
		private int _pingState;

		// Token: 0x040002F4 RID: 756
		private int _serverPerformanceState;

		// Token: 0x02000168 RID: 360
		private enum StatusTypes
		{
			// Token: 0x04000C89 RID: 3209
			Good,
			// Token: 0x04000C8A RID: 3210
			Average,
			// Token: 0x04000C8B RID: 3211
			Poor
		}
	}
}
