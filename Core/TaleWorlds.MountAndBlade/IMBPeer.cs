using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000198 RID: 408
	[ScriptingInterfaceBase]
	internal interface IMBPeer
	{
		// Token: 0x060016B9 RID: 5817
		[EngineMethod("set_user_data", false)]
		void SetUserData(int index, MBNetworkPeer data);

		// Token: 0x060016BA RID: 5818
		[EngineMethod("set_controlled_agent", false)]
		void SetControlledAgent(int index, UIntPtr missionPointer, int agentIndex);

		// Token: 0x060016BB RID: 5819
		[EngineMethod("set_team", false)]
		void SetTeam(int index, int teamIndex);

		// Token: 0x060016BC RID: 5820
		[EngineMethod("is_active", false)]
		bool IsActive(int index);

		// Token: 0x060016BD RID: 5821
		[EngineMethod("set_is_synchronized", false)]
		void SetIsSynchronized(int index, bool value);

		// Token: 0x060016BE RID: 5822
		[EngineMethod("get_is_synchronized", false)]
		bool GetIsSynchronized(int index);

		// Token: 0x060016BF RID: 5823
		[EngineMethod("send_existing_objects", false)]
		void SendExistingObjects(int index, UIntPtr missionPointer);

		// Token: 0x060016C0 RID: 5824
		[EngineMethod("begin_module_event", false)]
		void BeginModuleEvent(int index, bool isReliable);

		// Token: 0x060016C1 RID: 5825
		[EngineMethod("end_module_event", false)]
		void EndModuleEvent(bool isReliable);

		// Token: 0x060016C2 RID: 5826
		[EngineMethod("get_average_ping_in_milliseconds", false)]
		double GetAveragePingInMilliseconds(int index);

		// Token: 0x060016C3 RID: 5827
		[EngineMethod("get_average_loss_percent", false)]
		double GetAverageLossPercent(int index);

		// Token: 0x060016C4 RID: 5828
		[EngineMethod("set_relevant_game_options", false)]
		void SetRelevantGameOptions(int index, bool sendMeBloodEvents, bool sendMeSoundEvents);

		// Token: 0x060016C5 RID: 5829
		[EngineMethod("get_reversed_host", false)]
		uint GetReversedHost(int index);

		// Token: 0x060016C6 RID: 5830
		[EngineMethod("get_host", false)]
		uint GetHost(int index);

		// Token: 0x060016C7 RID: 5831
		[EngineMethod("get_port", false)]
		ushort GetPort(int index);
	}
}
