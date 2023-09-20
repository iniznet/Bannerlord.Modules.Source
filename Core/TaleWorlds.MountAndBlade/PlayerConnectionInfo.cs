using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200032B RID: 811
	public class PlayerConnectionInfo
	{
		// Token: 0x06002BE8 RID: 11240 RVA: 0x000AA7E0 File Offset: 0x000A89E0
		public PlayerConnectionInfo(PlayerId playerID)
		{
			this.PlayerID = playerID;
			this._parameters = new Dictionary<string, object>();
		}

		// Token: 0x06002BE9 RID: 11241 RVA: 0x000AA7FA File Offset: 0x000A89FA
		public void AddParameter(string name, object parameter)
		{
			if (!this._parameters.ContainsKey(name))
			{
				this._parameters.Add(name, parameter);
			}
		}

		// Token: 0x06002BEA RID: 11242 RVA: 0x000AA818 File Offset: 0x000A8A18
		public T GetParameter<T>(string name) where T : class
		{
			if (this._parameters.ContainsKey(name))
			{
				return this._parameters[name] as T;
			}
			return default(T);
		}

		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x06002BEB RID: 11243 RVA: 0x000AA853 File Offset: 0x000A8A53
		// (set) Token: 0x06002BEC RID: 11244 RVA: 0x000AA85B File Offset: 0x000A8A5B
		public int SessionKey { get; set; }

		// Token: 0x170007E5 RID: 2021
		// (get) Token: 0x06002BED RID: 11245 RVA: 0x000AA864 File Offset: 0x000A8A64
		// (set) Token: 0x06002BEE RID: 11246 RVA: 0x000AA86C File Offset: 0x000A8A6C
		public string Name { get; set; }

		// Token: 0x170007E6 RID: 2022
		// (get) Token: 0x06002BEF RID: 11247 RVA: 0x000AA875 File Offset: 0x000A8A75
		// (set) Token: 0x06002BF0 RID: 11248 RVA: 0x000AA87D File Offset: 0x000A8A7D
		public NetworkCommunicator NetworkPeer { get; set; }

		// Token: 0x0400109A RID: 4250
		private Dictionary<string, object> _parameters;

		// Token: 0x0400109E RID: 4254
		public readonly PlayerId PlayerID;
	}
}
