﻿using System;

namespace TaleWorlds.ServiceDiscovery.Client
{
	[Serializable]
	public class ServiceResolvedAddress
	{
		public string Address { get; private set; }

		public int Port { get; private set; }

		public bool IsSecure { get; private set; }

		public string[] Tags { get; private set; }

		public ServiceResolvedAddress(string address, int port, bool isSecure, string[] tags)
		{
			this.Address = address;
			this.Port = port;
			this.IsSecure = isSecure;
			this.Tags = tags;
		}
	}
}
