using System;

namespace TaleWorlds.MountAndBlade
{
	[Flags]
	public enum MultiplayerMessageFilter : ulong
	{
		None = 0UL,
		Peers = 1UL,
		Messaging = 2UL,
		Items = 4UL,
		General = 8UL,
		Equipment = 16UL,
		EquipmentDetailed = 32UL,
		Formations = 64UL,
		Agents = 128UL,
		AgentsDetailed = 256UL,
		Mission = 512UL,
		MissionDetailed = 1024UL,
		AgentAnimations = 2048UL,
		SiegeWeapons = 4096UL,
		MissionObjects = 8192UL,
		MissionObjectsDetailed = 16384UL,
		SiegeWeaponsDetailed = 32768UL,
		Orders = 65536UL,
		GameMode = 131072UL,
		Administration = 262144UL,
		Particles = 524288UL,
		RPC = 1048576UL,
		All = 4294967295UL,
		LightLogging = 139913UL,
		NormalLogging = 1979037UL,
		AllWithoutDetails = 2044639UL
	}
}
