using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200003D RID: 61
	[ApplicationInterfaceBase]
	internal interface ISoundManager
	{
		// Token: 0x06000551 RID: 1361
		[EngineMethod("set_listener_frame", false)]
		void SetListenerFrame(ref MatrixFrame frame, ref Vec3 attenuationPosition);

		// Token: 0x06000552 RID: 1362
		[EngineMethod("get_listener_frame", false)]
		void GetListenerFrame(out MatrixFrame result);

		// Token: 0x06000553 RID: 1363
		[EngineMethod("get_attenuation_position", false)]
		void GetAttenuationPosition(out Vec3 result);

		// Token: 0x06000554 RID: 1364
		[EngineMethod("reset", false)]
		void Reset();

		// Token: 0x06000555 RID: 1365
		[EngineMethod("start_one_shot_event_with_param", false)]
		bool StartOneShotEventWithParam(string eventFullName, Vec3 position, string paramName, float paramValue);

		// Token: 0x06000556 RID: 1366
		[EngineMethod("start_one_shot_event", false)]
		bool StartOneShotEvent(string eventFullName, Vec3 position);

		// Token: 0x06000557 RID: 1367
		[EngineMethod("set_state", false)]
		void SetState(string stateGroup, string state);

		// Token: 0x06000558 RID: 1368
		[EngineMethod("load_event_file_aux", false)]
		void LoadEventFileAux(string soundBankName, bool decompressSamples);

		// Token: 0x06000559 RID: 1369
		[EngineMethod("set_global_parameter", false)]
		void SetGlobalParameter(string parameterName, float value);

		// Token: 0x0600055A RID: 1370
		[EngineMethod("add_sound_client_with_id", false)]
		void AddSoundClientWithId(ulong client_id);

		// Token: 0x0600055B RID: 1371
		[EngineMethod("delete_sound_client_with_id", false)]
		void DeleteSoundClientWithId(ulong client_id);

		// Token: 0x0600055C RID: 1372
		[EngineMethod("get_global_index_of_event", false)]
		int GetGlobalIndexOfEvent(string eventFullName);

		// Token: 0x0600055D RID: 1373
		[EngineMethod("create_voice_event", false)]
		void CreateVoiceEvent();

		// Token: 0x0600055E RID: 1374
		[EngineMethod("destroy_voice_event", false)]
		void DestroyVoiceEvent(int id);

		// Token: 0x0600055F RID: 1375
		[EngineMethod("init_voice_play_event", false)]
		void InitializeVoicePlayEvent();

		// Token: 0x06000560 RID: 1376
		[EngineMethod("finalize_voice_play_event", false)]
		void FinalizeVoicePlayEvent();

		// Token: 0x06000561 RID: 1377
		[EngineMethod("start_voice_record", false)]
		void StartVoiceRecord();

		// Token: 0x06000562 RID: 1378
		[EngineMethod("stop_voice_record", false)]
		void StopVoiceRecord();

		// Token: 0x06000563 RID: 1379
		[EngineMethod("get_voice_data", false)]
		void GetVoiceData(byte[] voiceBuffer, int chunkSize, ref int readBytesLength);

		// Token: 0x06000564 RID: 1380
		[EngineMethod("update_voice_to_play", false)]
		void UpdateVoiceToPlay(byte[] voiceBuffer, int length, int index);

		// Token: 0x06000565 RID: 1381
		[EngineMethod("compress_voice_data", false)]
		void CompressData(ulong clientID, byte[] buffer, int length, byte[] compressedBuffer, ref int compressedBufferLength);

		// Token: 0x06000566 RID: 1382
		[EngineMethod("decompress_voice_data", false)]
		void DecompressData(ulong clientID, byte[] compressedBuffer, int compressedBufferLength, byte[] decompressedBuffer, ref int decompressedBufferLength);

		// Token: 0x06000567 RID: 1383
		[EngineMethod("remove_xbox_remote_user", false)]
		void RemoveXBOXRemoteUser(ulong XUID);

		// Token: 0x06000568 RID: 1384
		[EngineMethod("add_xbox_remote_user", false)]
		void AddXBOXRemoteUser(ulong XUID, ulong deviceID, bool canSendMicSound, bool canSendTextSound, bool canSendText, bool canReceiveSound, bool canReceiveText);

		// Token: 0x06000569 RID: 1385
		[EngineMethod("initialize_xbox_sound_manager", false)]
		void InitializeXBOXSoundManager();

		// Token: 0x0600056A RID: 1386
		[EngineMethod("apply_push_to_talk", false)]
		void ApplyPushToTalk(bool pushed);

		// Token: 0x0600056B RID: 1387
		[EngineMethod("clear_xbox_sound_manager", false)]
		void ClearXBOXSoundManager();

		// Token: 0x0600056C RID: 1388
		[EngineMethod("update_xbox_local_user", false)]
		void UpdateXBOXLocalUser();

		// Token: 0x0600056D RID: 1389
		[EngineMethod("update_xbox_chat_communication_flags", false)]
		void UpdateXBOXChatCommunicationFlags(ulong XUID, bool canSendMicSound, bool canSendTextSound, bool canSendText, bool canReceiveSound, bool canReceiveText);

		// Token: 0x0600056E RID: 1390
		[EngineMethod("process_data_to_be_received", false)]
		void ProcessDataToBeReceived(ulong senderDeviceID, byte[] data, uint dataSize);

		// Token: 0x0600056F RID: 1391
		[EngineMethod("process_data_to_be_sent", false)]
		void ProcessDataToBeSent(ref int numData);

		// Token: 0x06000570 RID: 1392
		[EngineMethod("handle_state_changes", false)]
		void HandleStateChanges();

		// Token: 0x06000571 RID: 1393
		[EngineMethod("get_size_of_data_to_be_sent_at", false)]
		void GetSizeOfDataToBeSentAt(int index, ref uint byte_count, ref uint numReceivers);

		// Token: 0x06000572 RID: 1394
		[EngineMethod("get_data_to_be_sent_at", false)]
		bool GetDataToBeSentAt(int index, byte[] buffer, ulong[] receivers, ref bool transportGuaranteed);

		// Token: 0x06000573 RID: 1395
		[EngineMethod("clear_data_to_be_sent", false)]
		void ClearDataToBeSent();
	}
}
