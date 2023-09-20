using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	public class PlayerConnectionInfo
	{
		public PlayerConnectionInfo(PlayerId playerID)
		{
			this.PlayerID = playerID;
			this._parameters = new Dictionary<string, object>();
		}

		public void AddParameter(string name, object parameter)
		{
			if (!this._parameters.ContainsKey(name))
			{
				this._parameters.Add(name, parameter);
			}
		}

		public T GetParameter<T>(string name) where T : class
		{
			if (this._parameters.ContainsKey(name))
			{
				return this._parameters[name] as T;
			}
			return default(T);
		}

		public int SessionKey { get; set; }

		public string Name { get; set; }

		public NetworkCommunicator NetworkPeer { get; set; }

		private Dictionary<string, object> _parameters;

		public readonly PlayerId PlayerID;
	}
}
