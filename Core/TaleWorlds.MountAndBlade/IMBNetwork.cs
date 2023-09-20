using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000197 RID: 407
	[ScriptingInterfaceBase]
	internal interface IMBNetwork
	{
		// Token: 0x06001690 RID: 5776
		[EngineMethod("get_multiplayer_disabled", false)]
		bool GetMultiplayerDisabled();

		// Token: 0x06001691 RID: 5777
		[EngineMethod("is_dedicated_server", false)]
		bool IsDedicatedServer();

		// Token: 0x06001692 RID: 5778
		[EngineMethod("initialize_server_side", false)]
		void InitializeServerSide(int port);

		// Token: 0x06001693 RID: 5779
		[EngineMethod("initialize_client_side", false)]
		void InitializeClientSide(string serverAddress, int port, int sessionKey, int playerIndex);

		// Token: 0x06001694 RID: 5780
		[EngineMethod("terminate_server_side", false)]
		void TerminateServerSide();

		// Token: 0x06001695 RID: 5781
		[EngineMethod("terminate_client_side", false)]
		void TerminateClientSide();

		// Token: 0x06001696 RID: 5782
		[EngineMethod("server_ping", false)]
		void ServerPing(string serverAddress, int port);

		// Token: 0x06001697 RID: 5783
		[EngineMethod("add_peer_to_disconnect", false)]
		void AddPeerToDisconnect(int peer);

		// Token: 0x06001698 RID: 5784
		[EngineMethod("prepare_new_udp_session", false)]
		void PrepareNewUdpSession(int player, int sessionKey);

		// Token: 0x06001699 RID: 5785
		[EngineMethod("can_add_new_players_on_server", false)]
		bool CanAddNewPlayersOnServer(int numPlayers);

		// Token: 0x0600169A RID: 5786
		[EngineMethod("add_new_player_on_server", false)]
		int AddNewPlayerOnServer(bool serverPlayer);

		// Token: 0x0600169B RID: 5787
		[EngineMethod("add_new_bot_on_server", false)]
		int AddNewBotOnServer();

		// Token: 0x0600169C RID: 5788
		[EngineMethod("remove_bot_on_server", false)]
		void RemoveBotOnServer(int botPlayerIndex);

		// Token: 0x0600169D RID: 5789
		[EngineMethod("reset_mission_data", false)]
		void ResetMissionData();

		// Token: 0x0600169E RID: 5790
		[EngineMethod("begin_broadcast_module_event", false)]
		void BeginBroadcastModuleEvent();

		// Token: 0x0600169F RID: 5791
		[EngineMethod("end_broadcast_module_event", false)]
		void EndBroadcastModuleEvent(int broadcastFlags, int targetPlayer, bool isReliable);

		// Token: 0x060016A0 RID: 5792
		[EngineMethod("elapsed_time_since_last_udp_packet_arrived", false)]
		double ElapsedTimeSinceLastUdpPacketArrived();

		// Token: 0x060016A1 RID: 5793
		[EngineMethod("begin_module_event_as_client", false)]
		void BeginModuleEventAsClient(bool isReliable);

		// Token: 0x060016A2 RID: 5794
		[EngineMethod("end_module_event_as_client", false)]
		void EndModuleEventAsClient(bool isReliable);

		// Token: 0x060016A3 RID: 5795
		[EngineMethod("read_int_from_packet", false)]
		bool ReadIntFromPacket(ref CompressionInfo.Integer compressionInfo, out int output);

		// Token: 0x060016A4 RID: 5796
		[EngineMethod("read_uint_from_packet", false)]
		bool ReadUintFromPacket(ref CompressionInfo.UnsignedInteger compressionInfo, out uint output);

		// Token: 0x060016A5 RID: 5797
		[EngineMethod("read_long_from_packet", false)]
		bool ReadLongFromPacket(ref CompressionInfo.LongInteger compressionInfo, out long output);

		// Token: 0x060016A6 RID: 5798
		[EngineMethod("read_ulong_from_packet", false)]
		bool ReadUlongFromPacket(ref CompressionInfo.UnsignedLongInteger compressionInfo, out ulong output);

		// Token: 0x060016A7 RID: 5799
		[EngineMethod("read_float_from_packet", false)]
		bool ReadFloatFromPacket(ref CompressionInfo.Float compressionInfo, out float output);

		// Token: 0x060016A8 RID: 5800
		[EngineMethod("read_string_from_packet", false)]
		string ReadStringFromPacket(ref bool bufferReadValid);

		// Token: 0x060016A9 RID: 5801
		[EngineMethod("write_int_to_packet", false)]
		void WriteIntToPacket(int value, ref CompressionInfo.Integer compressionInfo);

		// Token: 0x060016AA RID: 5802
		[EngineMethod("write_uint_to_packet", false)]
		void WriteUintToPacket(uint value, ref CompressionInfo.UnsignedInteger compressionInfo);

		// Token: 0x060016AB RID: 5803
		[EngineMethod("write_long_to_packet", false)]
		void WriteLongToPacket(long value, ref CompressionInfo.LongInteger compressionInfo);

		// Token: 0x060016AC RID: 5804
		[EngineMethod("write_ulong_to_packet", false)]
		void WriteUlongToPacket(ulong value, ref CompressionInfo.UnsignedLongInteger compressionInfo);

		// Token: 0x060016AD RID: 5805
		[EngineMethod("write_float_to_packet", false)]
		void WriteFloatToPacket(float value, ref CompressionInfo.Float compressionInfo);

		// Token: 0x060016AE RID: 5806
		[EngineMethod("write_string_to_packet", false)]
		void WriteStringToPacket(string value);

		// Token: 0x060016AF RID: 5807
		[EngineMethod("read_byte_array_from_packet", false)]
		int ReadByteArrayFromPacket(byte[] buffer, int offset, int bufferCapacity, ref bool bufferReadValid);

		// Token: 0x060016B0 RID: 5808
		[EngineMethod("write_byte_array_to_packet", false)]
		void WriteByteArrayToPacket(byte[] value, int offset, int size);

		// Token: 0x060016B1 RID: 5809
		[EngineMethod("increase_total_upload_limit", false)]
		void IncreaseTotalUploadLimit(int value);

		// Token: 0x060016B2 RID: 5810
		[EngineMethod("reset_debug_variables", false)]
		void ResetDebugVariables();

		// Token: 0x060016B3 RID: 5811
		[EngineMethod("print_debug_stats", false)]
		void PrintDebugStats();

		// Token: 0x060016B4 RID: 5812
		[EngineMethod("get_average_packet_loss_ratio", false)]
		float GetAveragePacketLossRatio();

		// Token: 0x060016B5 RID: 5813
		[EngineMethod("get_debug_uploads_in_bits", false)]
		void GetDebugUploadsInBits(ref GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct, ref GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct);

		// Token: 0x060016B6 RID: 5814
		[EngineMethod("reset_debug_uploads", false)]
		void ResetDebugUploads();

		// Token: 0x060016B7 RID: 5815
		[EngineMethod("print_replication_table_statistics", false)]
		void PrintReplicationTableStatistics();

		// Token: 0x060016B8 RID: 5816
		[EngineMethod("clear_replication_table_statistics", false)]
		void ClearReplicationTableStatistics();
	}
}
