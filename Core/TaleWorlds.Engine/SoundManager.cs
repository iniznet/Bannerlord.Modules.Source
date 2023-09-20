using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000089 RID: 137
	public static class SoundManager
	{
		// Token: 0x06000A64 RID: 2660 RVA: 0x0000B64B File Offset: 0x0000984B
		public static void SetListenerFrame(MatrixFrame frame)
		{
			EngineApplicationInterface.ISoundManager.SetListenerFrame(ref frame, ref frame.origin);
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x0000B660 File Offset: 0x00009860
		public static void SetListenerFrame(MatrixFrame frame, Vec3 attenuationPosition)
		{
			EngineApplicationInterface.ISoundManager.SetListenerFrame(ref frame, ref attenuationPosition);
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x0000B670 File Offset: 0x00009870
		public static MatrixFrame GetListenerFrame()
		{
			MatrixFrame matrixFrame;
			EngineApplicationInterface.ISoundManager.GetListenerFrame(out matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x0000B68C File Offset: 0x0000988C
		public static Vec3 GetAttenuationPosition()
		{
			Vec3 vec;
			EngineApplicationInterface.ISoundManager.GetAttenuationPosition(out vec);
			return vec;
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x0000B6A6 File Offset: 0x000098A6
		public static void Reset()
		{
			EngineApplicationInterface.ISoundManager.Reset();
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x0000B6B2 File Offset: 0x000098B2
		public static bool StartOneShotEvent(string eventFullName, in Vec3 position, string paramName, float paramValue)
		{
			return EngineApplicationInterface.ISoundManager.StartOneShotEventWithParam(eventFullName, position, paramName, paramValue);
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x0000B6C7 File Offset: 0x000098C7
		public static bool StartOneShotEvent(string eventFullName, in Vec3 position)
		{
			return EngineApplicationInterface.ISoundManager.StartOneShotEvent(eventFullName, position);
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x0000B6DA File Offset: 0x000098DA
		public static void SetState(string stateGroup, string state)
		{
			EngineApplicationInterface.ISoundManager.SetState(stateGroup, state);
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x0000B6E8 File Offset: 0x000098E8
		public static SoundEvent CreateEvent(string eventFullName, Scene scene)
		{
			return SoundEvent.CreateEventFromString(eventFullName, scene);
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x0000B6F1 File Offset: 0x000098F1
		public static void LoadEventFileAux(string soundBank, bool decompressSamples)
		{
			if (!SoundManager._loaded)
			{
				EngineApplicationInterface.ISoundManager.LoadEventFileAux(soundBank, decompressSamples);
				SoundManager._loaded = true;
			}
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x0000B70C File Offset: 0x0000990C
		public static void AddSoundClientWithId(ulong clientId)
		{
			EngineApplicationInterface.ISoundManager.AddSoundClientWithId(clientId);
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x0000B719 File Offset: 0x00009919
		public static void DeleteSoundClientWithId(ulong clientId)
		{
			EngineApplicationInterface.ISoundManager.DeleteSoundClientWithId(clientId);
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x0000B726 File Offset: 0x00009926
		public static void SetGlobalParameter(string parameterName, float value)
		{
			EngineApplicationInterface.ISoundManager.SetGlobalParameter(parameterName, value);
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x0000B734 File Offset: 0x00009934
		public static int GetEventGlobalIndex(string eventFullName)
		{
			if (string.IsNullOrEmpty(eventFullName))
			{
				return -1;
			}
			return EngineApplicationInterface.ISoundManager.GetGlobalIndexOfEvent(eventFullName);
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x0000B74B File Offset: 0x0000994B
		public static void InitializeVoicePlayEvent()
		{
			EngineApplicationInterface.ISoundManager.InitializeVoicePlayEvent();
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x0000B757 File Offset: 0x00009957
		public static void CreateVoiceEvent()
		{
			EngineApplicationInterface.ISoundManager.CreateVoiceEvent();
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x0000B763 File Offset: 0x00009963
		public static void DestroyVoiceEvent(int id)
		{
			EngineApplicationInterface.ISoundManager.DestroyVoiceEvent(id);
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x0000B770 File Offset: 0x00009970
		public static void FinalizeVoicePlayEvent()
		{
			EngineApplicationInterface.ISoundManager.FinalizeVoicePlayEvent();
		}

		// Token: 0x06000A76 RID: 2678 RVA: 0x0000B77C File Offset: 0x0000997C
		public static void StartVoiceRecording()
		{
			EngineApplicationInterface.ISoundManager.StartVoiceRecord();
		}

		// Token: 0x06000A77 RID: 2679 RVA: 0x0000B788 File Offset: 0x00009988
		public static void StopVoiceRecording()
		{
			EngineApplicationInterface.ISoundManager.StopVoiceRecord();
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x0000B794 File Offset: 0x00009994
		public static void GetVoiceData(byte[] voiceBuffer, int chunkSize, out int readBytesLength)
		{
			readBytesLength = 0;
			EngineApplicationInterface.ISoundManager.GetVoiceData(voiceBuffer, chunkSize, ref readBytesLength);
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x0000B7A6 File Offset: 0x000099A6
		public static void UpdateVoiceToPlay(byte[] voiceBuffer, int length, int index)
		{
			EngineApplicationInterface.ISoundManager.UpdateVoiceToPlay(voiceBuffer, length, index);
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x0000B7B5 File Offset: 0x000099B5
		public static void AddXBOXRemoteUser(ulong XUID, ulong deviceID, bool canSendMicSound, bool canSendTextSound, bool canSendText, bool canReceiveSound, bool canReceiveText)
		{
			EngineApplicationInterface.ISoundManager.AddXBOXRemoteUser(XUID, deviceID, canSendMicSound, canSendTextSound, canSendText, canReceiveSound, canReceiveText);
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x0000B7CB File Offset: 0x000099CB
		public static void InitializeXBOXSoundManager()
		{
			EngineApplicationInterface.ISoundManager.InitializeXBOXSoundManager();
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x0000B7D7 File Offset: 0x000099D7
		public static void ApplyPushToTalk(bool pushed)
		{
			EngineApplicationInterface.ISoundManager.ApplyPushToTalk(pushed);
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x0000B7E4 File Offset: 0x000099E4
		public static void ClearXBOXSoundManager()
		{
			EngineApplicationInterface.ISoundManager.ClearXBOXSoundManager();
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x0000B7F0 File Offset: 0x000099F0
		public static void UpdateXBOXLocalUser()
		{
			EngineApplicationInterface.ISoundManager.UpdateXBOXLocalUser();
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x0000B7FC File Offset: 0x000099FC
		public static void UpdateXBOXChatCommunicationFlags(ulong XUID, bool canSendMicSound, bool canSendTextSound, bool canSendText, bool canReceiveSound, bool canReceiveText)
		{
			EngineApplicationInterface.ISoundManager.UpdateXBOXChatCommunicationFlags(XUID, canSendMicSound, canSendTextSound, canSendText, canReceiveSound, canReceiveText);
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x0000B810 File Offset: 0x00009A10
		public static void RemoveXBOXRemoteUser(ulong XUID)
		{
			EngineApplicationInterface.ISoundManager.RemoveXBOXRemoteUser(XUID);
		}

		// Token: 0x06000A81 RID: 2689 RVA: 0x0000B81D File Offset: 0x00009A1D
		public static void ProcessDataToBeReceived(ulong senderDeviceID, byte[] data, uint dataSize)
		{
			EngineApplicationInterface.ISoundManager.ProcessDataToBeReceived(senderDeviceID, data, dataSize);
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x0000B82C File Offset: 0x00009A2C
		public static void ProcessDataToBeSent(ref int numData)
		{
			EngineApplicationInterface.ISoundManager.ProcessDataToBeSent(ref numData);
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x0000B839 File Offset: 0x00009A39
		public static void HandleStateChanges()
		{
			EngineApplicationInterface.ISoundManager.HandleStateChanges();
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x0000B845 File Offset: 0x00009A45
		public static void GetSizeOfDataToBeSentAt(int index, ref uint byteCount, ref uint numReceivers)
		{
			EngineApplicationInterface.ISoundManager.GetSizeOfDataToBeSentAt(index, ref byteCount, ref numReceivers);
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x0000B854 File Offset: 0x00009A54
		public static bool GetDataToBeSentAt(int index, byte[] buffer, ulong[] receivers, ref bool transportGuaranteed)
		{
			return EngineApplicationInterface.ISoundManager.GetDataToBeSentAt(index, buffer, receivers, ref transportGuaranteed);
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x0000B864 File Offset: 0x00009A64
		public static void ClearDataToBeSent()
		{
			EngineApplicationInterface.ISoundManager.ClearDataToBeSent();
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x0000B870 File Offset: 0x00009A70
		public static void CompressData(int clientID, byte[] buffer, int length, byte[] compressedBuffer, out int compressedBufferLength)
		{
			compressedBufferLength = 0;
			EngineApplicationInterface.ISoundManager.CompressData((ulong)((long)clientID), buffer, length, compressedBuffer, ref compressedBufferLength);
		}

		// Token: 0x06000A88 RID: 2696 RVA: 0x0000B887 File Offset: 0x00009A87
		public static void DecompressData(int clientID, byte[] compressedBuffer, int compressedBufferLength, byte[] decompressedBuffer, out int decompressedBufferLength)
		{
			decompressedBufferLength = 0;
			EngineApplicationInterface.ISoundManager.DecompressData((ulong)((long)clientID), compressedBuffer, compressedBufferLength, decompressedBuffer, ref decompressedBufferLength);
		}

		// Token: 0x040001A9 RID: 425
		private static bool _loaded;
	}
}
