using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.Network.Messages
{
	// Token: 0x020003B6 RID: 950
	public abstract class GameNetworkMessage
	{
		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x06003343 RID: 13123 RVA: 0x000D4B62 File Offset: 0x000D2D62
		// (set) Token: 0x06003344 RID: 13124 RVA: 0x000D4B6A File Offset: 0x000D2D6A
		public int MessageId { get; set; }

		// Token: 0x06003345 RID: 13125 RVA: 0x000D4B74 File Offset: 0x000D2D74
		internal void Write()
		{
			DebugNetworkEventStatistics.StartEvent(base.GetType().Name, this.MessageId);
			GameNetworkMessage.WriteIntToPacket(this.MessageId, GameNetwork.IsClientOrReplay ? CompressionBasic.NetworkComponentEventTypeFromClientCompressionInfo : CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo);
			this.OnWrite();
			GameNetworkMessage.WriteIntToPacket(5, GameNetworkMessage.TestValueCompressionInfo);
			DebugNetworkEventStatistics.EndEvent();
		}

		// Token: 0x06003346 RID: 13126
		protected abstract void OnWrite();

		// Token: 0x06003347 RID: 13127 RVA: 0x000D4BCC File Offset: 0x000D2DCC
		internal bool Read()
		{
			bool flag = this.OnRead();
			bool flag2 = true;
			if (GameNetworkMessage.ReadIntFromPacket(GameNetworkMessage.TestValueCompressionInfo, ref flag2) != 5)
			{
				throw new MBNetworkBitException(base.GetType().Name);
			}
			return flag;
		}

		// Token: 0x06003348 RID: 13128
		protected abstract bool OnRead();

		// Token: 0x06003349 RID: 13129 RVA: 0x000D4C01 File Offset: 0x000D2E01
		internal MultiplayerMessageFilter GetLogFilter()
		{
			return this.OnGetLogFilter();
		}

		// Token: 0x0600334A RID: 13130
		protected abstract MultiplayerMessageFilter OnGetLogFilter();

		// Token: 0x0600334B RID: 13131 RVA: 0x000D4C09 File Offset: 0x000D2E09
		internal string GetLogFormat()
		{
			return this.OnGetLogFormat();
		}

		// Token: 0x0600334C RID: 13132
		protected abstract string OnGetLogFormat();

		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x0600334D RID: 13133 RVA: 0x000D4C11 File Offset: 0x000D2E11
		public static bool IsClientMissionOver
		{
			get
			{
				return GameNetwork.IsClient && !NetworkMain.GameClient.IsInGame;
			}
		}

		// Token: 0x0600334E RID: 13134 RVA: 0x000D4C2C File Offset: 0x000D2E2C
		public static bool ReadBoolFromPacket(ref bool bufferReadValid)
		{
			CompressionInfo.Integer integer = new CompressionInfo.Integer(0, 1);
			int num = 0;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadIntFromPacket(ref integer, out num);
			return num != 0;
		}

		// Token: 0x0600334F RID: 13135 RVA: 0x000D4C60 File Offset: 0x000D2E60
		public static void WriteBoolToPacket(bool value)
		{
			CompressionInfo.Integer integer = new CompressionInfo.Integer(0, 1);
			MBAPI.IMBNetwork.WriteIntToPacket(value ? 1 : 0, ref integer);
			DebugNetworkEventStatistics.AddDataToStatistic(integer.GetNumBits());
		}

		// Token: 0x06003350 RID: 13136 RVA: 0x000D4C98 File Offset: 0x000D2E98
		public static int ReadIntFromPacket(CompressionInfo.Integer compressionInfo, ref bool bufferReadValid)
		{
			int num = 0;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadIntFromPacket(ref compressionInfo, out num);
			return num;
		}

		// Token: 0x06003351 RID: 13137 RVA: 0x000D4CBF File Offset: 0x000D2EBF
		public static void WriteIntToPacket(int value, CompressionInfo.Integer compressionInfo)
		{
			MBAPI.IMBNetwork.WriteIntToPacket(value, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		// Token: 0x06003352 RID: 13138 RVA: 0x000D4CDC File Offset: 0x000D2EDC
		public static uint ReadUintFromPacket(CompressionInfo.UnsignedInteger compressionInfo, ref bool bufferReadValid)
		{
			uint num = 0U;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadUintFromPacket(ref compressionInfo, out num);
			return num;
		}

		// Token: 0x06003353 RID: 13139 RVA: 0x000D4D03 File Offset: 0x000D2F03
		public static void WriteUintToPacket(uint value, CompressionInfo.UnsignedInteger compressionInfo)
		{
			MBAPI.IMBNetwork.WriteUintToPacket(value, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		// Token: 0x06003354 RID: 13140 RVA: 0x000D4D20 File Offset: 0x000D2F20
		public static long ReadLongFromPacket(CompressionInfo.LongInteger compressionInfo, ref bool bufferReadValid)
		{
			long num = 0L;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadLongFromPacket(ref compressionInfo, out num);
			return num;
		}

		// Token: 0x06003355 RID: 13141 RVA: 0x000D4D48 File Offset: 0x000D2F48
		public static void WriteLongToPacket(long value, CompressionInfo.LongInteger compressionInfo)
		{
			MBAPI.IMBNetwork.WriteLongToPacket(value, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		// Token: 0x06003356 RID: 13142 RVA: 0x000D4D64 File Offset: 0x000D2F64
		public static ulong ReadUlongFromPacket(CompressionInfo.UnsignedLongInteger compressionInfo, ref bool bufferReadValid)
		{
			ulong num = 0UL;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadUlongFromPacket(ref compressionInfo, out num);
			return num;
		}

		// Token: 0x06003357 RID: 13143 RVA: 0x000D4D8C File Offset: 0x000D2F8C
		public static void WriteUlongToPacket(ulong value, CompressionInfo.UnsignedLongInteger compressionInfo)
		{
			MBAPI.IMBNetwork.WriteUlongToPacket(value, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		// Token: 0x06003358 RID: 13144 RVA: 0x000D4DA8 File Offset: 0x000D2FA8
		public static float ReadFloatFromPacket(CompressionInfo.Float compressionInfo, ref bool bufferReadValid)
		{
			float num = 0f;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadFloatFromPacket(ref compressionInfo, out num);
			return num;
		}

		// Token: 0x06003359 RID: 13145 RVA: 0x000D4DD3 File Offset: 0x000D2FD3
		public static void WriteFloatToPacket(float value, CompressionInfo.Float compressionInfo)
		{
			MBAPI.IMBNetwork.WriteFloatToPacket(value, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		// Token: 0x0600335A RID: 13146 RVA: 0x000D4DF0 File Offset: 0x000D2FF0
		public static string ReadStringFromPacket(ref bool bufferReadValid)
		{
			byte[] array = new byte[1024];
			int num = GameNetworkMessage.ReadByteArrayFromPacket(array, 0, 1024, ref bufferReadValid);
			return GameNetworkMessage.StringEncoding.GetString(array, 0, num);
		}

		// Token: 0x0600335B RID: 13147 RVA: 0x000D4E24 File Offset: 0x000D3024
		public static void WriteStringToPacket(string value)
		{
			byte[] array = (string.IsNullOrEmpty(value) ? new byte[0] : GameNetworkMessage.StringEncoding.GetBytes(value));
			GameNetworkMessage.WriteByteArrayToPacket(array, 0, array.Length);
		}

		// Token: 0x0600335C RID: 13148 RVA: 0x000D4E58 File Offset: 0x000D3058
		public static void WriteBannerCodeToPacket(string bannerCode)
		{
			List<BannerData> bannerDataFromBannerCode = Banner.GetBannerDataFromBannerCode(bannerCode);
			GameNetworkMessage.WriteIntToPacket(bannerDataFromBannerCode.Count, CompressionBasic.BannerDataCountCompressionInfo);
			for (int i = 0; i < bannerDataFromBannerCode.Count; i++)
			{
				BannerData bannerData = bannerDataFromBannerCode[i];
				GameNetworkMessage.WriteIntToPacket(bannerData.MeshId, CompressionBasic.BannerDataMeshIdCompressionInfo);
				GameNetworkMessage.WriteIntToPacket(bannerData.ColorId, CompressionBasic.BannerDataColorIndexCompressionInfo);
				GameNetworkMessage.WriteIntToPacket(bannerData.ColorId2, CompressionBasic.BannerDataColorIndexCompressionInfo);
				GameNetworkMessage.WriteIntToPacket((int)bannerData.Size.X, CompressionBasic.BannerDataSizeCompressionInfo);
				GameNetworkMessage.WriteIntToPacket((int)bannerData.Size.Y, CompressionBasic.BannerDataSizeCompressionInfo);
				GameNetworkMessage.WriteIntToPacket((int)bannerData.Position.X, CompressionBasic.BannerDataSizeCompressionInfo);
				GameNetworkMessage.WriteIntToPacket((int)bannerData.Position.Y, CompressionBasic.BannerDataSizeCompressionInfo);
				GameNetworkMessage.WriteBoolToPacket(bannerData.DrawStroke);
				GameNetworkMessage.WriteBoolToPacket(bannerData.Mirror);
				GameNetworkMessage.WriteIntToPacket((int)bannerData.Rotation, CompressionBasic.BannerDataRotationCompressionInfo);
			}
		}

		// Token: 0x0600335D RID: 13149 RVA: 0x000D4F54 File Offset: 0x000D3154
		public static string ReadBannerCodeFromPacket(ref bool bufferReadValid)
		{
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.BannerDataCountCompressionInfo, ref bufferReadValid);
			MBList<BannerData> mblist = new MBList<BannerData>(num);
			for (int i = 0; i < num; i++)
			{
				BannerData bannerData = new BannerData(GameNetworkMessage.ReadIntFromPacket(CompressionBasic.BannerDataMeshIdCompressionInfo, ref bufferReadValid), GameNetworkMessage.ReadIntFromPacket(CompressionBasic.BannerDataColorIndexCompressionInfo, ref bufferReadValid), GameNetworkMessage.ReadIntFromPacket(CompressionBasic.BannerDataColorIndexCompressionInfo, ref bufferReadValid), new Vec2((float)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.BannerDataSizeCompressionInfo, ref bufferReadValid), (float)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.BannerDataSizeCompressionInfo, ref bufferReadValid)), new Vec2((float)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.BannerDataSizeCompressionInfo, ref bufferReadValid), (float)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.BannerDataSizeCompressionInfo, ref bufferReadValid)), GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid), GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid), (float)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.BannerDataRotationCompressionInfo, ref bufferReadValid) * 0.00278f);
				mblist.Add(bannerData);
			}
			return Banner.GetBannerCodeFromBannerDataList(mblist);
		}

		// Token: 0x0600335E RID: 13150 RVA: 0x000D5012 File Offset: 0x000D3212
		public static int ReadByteArrayFromPacket(byte[] buffer, int offset, int bufferCapacity, ref bool bufferReadValid)
		{
			return MBAPI.IMBNetwork.ReadByteArrayFromPacket(buffer, offset, bufferCapacity, ref bufferReadValid);
		}

		// Token: 0x0600335F RID: 13151 RVA: 0x000D5022 File Offset: 0x000D3222
		public static void WriteByteArrayToPacket(byte[] value, int offset, int size)
		{
			MBAPI.IMBNetwork.WriteByteArrayToPacket(value, offset, size);
			DebugNetworkEventStatistics.AddDataToStatistic(MathF.Min(size, 1024) + 10);
		}

		// Token: 0x06003360 RID: 13152 RVA: 0x000D5044 File Offset: 0x000D3244
		public static MBActionSet ReadActionSetReferenceFromPacket(CompressionInfo.Integer compressionInfo, ref bool bufferReadValid)
		{
			if (bufferReadValid)
			{
				int num;
				bufferReadValid = MBAPI.IMBNetwork.ReadIntFromPacket(ref compressionInfo, out num);
				return new MBActionSet(num);
			}
			return MBActionSet.InvalidActionSet;
		}

		// Token: 0x06003361 RID: 13153 RVA: 0x000D5071 File Offset: 0x000D3271
		public static void WriteActionSetReferenceToPacket(MBActionSet actionSet, CompressionInfo.Integer compressionInfo)
		{
			MBAPI.IMBNetwork.WriteIntToPacket(actionSet.Index, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		// Token: 0x06003362 RID: 13154 RVA: 0x000D5094 File Offset: 0x000D3294
		public static int ReadAgentIndexFromPacket(CompressionInfo.Integer compressionInfo, ref bool bufferReadValid)
		{
			int num = -1;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadIntFromPacket(ref compressionInfo, out num);
			return num;
		}

		// Token: 0x06003363 RID: 13155 RVA: 0x000D50BC File Offset: 0x000D32BC
		public static Agent ReadAgentReferenceFromPacket(ref bool bufferReadValid, bool canBeNull = false)
		{
			int num = GameNetworkMessage.ReadAgentIndexFromPacket(CompressionMission.AgentCompressionInfo, ref bufferReadValid);
			if (!bufferReadValid || GameNetworkMessage.IsClientMissionOver)
			{
				return null;
			}
			Agent agent = Mission.Current.FindAgentWithIndex(num);
			if (!canBeNull && agent == null && num >= 0)
			{
				Debug.Print("Agent with index: " + num + " could not be found while reading reference from packet.", 0, Debug.DebugColor.White, 17592186044416UL);
				throw new MBNotFoundException("Agent with index: " + num + " could not be found while reading reference from packet.");
			}
			return agent;
		}

		// Token: 0x06003364 RID: 13156 RVA: 0x000D513C File Offset: 0x000D333C
		public static void WriteAgentReferenceToPacket(Agent value)
		{
			CompressionInfo.Integer agentCompressionInfo = CompressionMission.AgentCompressionInfo;
			MBAPI.IMBNetwork.WriteIntToPacket((value != null) ? value.Index : (-1), ref agentCompressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(agentCompressionInfo.GetNumBits());
		}

		// Token: 0x06003365 RID: 13157 RVA: 0x000D5174 File Offset: 0x000D3374
		public static MBObjectBase ReadObjectReferenceFromPacket(MBObjectManager objectManager, CompressionInfo.UnsignedInteger compressionInfo, ref bool bufferReadValid)
		{
			uint num = GameNetworkMessage.ReadUintFromPacket(compressionInfo, ref bufferReadValid);
			if (bufferReadValid && num > 0U)
			{
				MBGUID mbguid = new MBGUID(num);
				return objectManager.GetObject(mbguid);
			}
			return null;
		}

		// Token: 0x06003366 RID: 13158 RVA: 0x000D51A4 File Offset: 0x000D33A4
		public static void WriteObjectReferenceToPacket(MBObjectBase value, CompressionInfo.UnsignedInteger compressionInfo)
		{
			MBAPI.IMBNetwork.WriteUintToPacket((value != null) ? value.Id.InternalValue : 0U, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		// Token: 0x06003367 RID: 13159 RVA: 0x000D51E0 File Offset: 0x000D33E0
		public static VirtualPlayer ReadVirtualPlayerReferenceToPacket(ref bool bufferReadValid, bool canReturnNull = false)
		{
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref bufferReadValid);
			bool flag = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
			if ((num >= 0 && !GameNetworkMessage.IsClientMissionOver) & bufferReadValid)
			{
				VirtualPlayer virtualPlayer;
				if (!flag)
				{
					virtualPlayer = MBNetwork.VirtualPlayers[num];
				}
				else
				{
					virtualPlayer = MBNetwork.DisconnectedNetworkPeers[num].VirtualPlayer;
				}
				return virtualPlayer;
			}
			return null;
		}

		// Token: 0x06003368 RID: 13160 RVA: 0x000D5235 File Offset: 0x000D3435
		public static NetworkCommunicator ReadNetworkPeerReferenceFromPacket(ref bool bufferReadValid, bool canReturnNull = false)
		{
			VirtualPlayer virtualPlayer = GameNetworkMessage.ReadVirtualPlayerReferenceToPacket(ref bufferReadValid, canReturnNull);
			return ((virtualPlayer != null) ? virtualPlayer.Communicator : null) as NetworkCommunicator;
		}

		// Token: 0x06003369 RID: 13161 RVA: 0x000D5250 File Offset: 0x000D3450
		public static void WriteVirtualPlayerReferenceToPacket(VirtualPlayer virtualPlayer)
		{
			bool flag = false;
			int num = ((virtualPlayer != null) ? virtualPlayer.Index : (-1));
			if (num >= 0 && MBNetwork.VirtualPlayers[num] != virtualPlayer)
			{
				for (int i = 0; i < MBNetwork.DisconnectedNetworkPeers.Count; i++)
				{
					if (MBNetwork.DisconnectedNetworkPeers[i].VirtualPlayer == virtualPlayer)
					{
						num = i;
						flag = true;
						break;
					}
				}
			}
			MBAPI.IMBNetwork.WriteIntToPacket(num, ref CompressionBasic.PlayerCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(flag);
			DebugNetworkEventStatistics.AddDataToStatistic(CompressionBasic.PlayerCompressionInfo.GetNumBits());
		}

		// Token: 0x0600336A RID: 13162 RVA: 0x000D52CD File Offset: 0x000D34CD
		public static void WriteNetworkPeerReferenceToPacket(NetworkCommunicator networkCommunicator)
		{
			GameNetworkMessage.WriteVirtualPlayerReferenceToPacket((networkCommunicator != null) ? networkCommunicator.VirtualPlayer : null);
		}

		// Token: 0x0600336B RID: 13163 RVA: 0x000D52E0 File Offset: 0x000D34E0
		public static MBTeam ReadMBTeamReferenceFromPacket(CompressionInfo.Integer compressionInfo, ref bool bufferReadValid)
		{
			int num = GameNetworkMessage.ReadIntFromPacket(compressionInfo, ref bufferReadValid);
			if (!GameNetworkMessage.IsClientMissionOver & bufferReadValid)
			{
				return new MBTeam(Mission.Current, num);
			}
			return MBTeam.InvalidTeam;
		}

		// Token: 0x0600336C RID: 13164 RVA: 0x000D5314 File Offset: 0x000D3514
		public static Team ReadTeamReferenceFromPacket(ref bool bufferReadValid)
		{
			MBTeam mbteam = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref bufferReadValid);
			if (mbteam.IsValid & bufferReadValid)
			{
				return Mission.Current.Teams.Find(mbteam);
			}
			return Team.Invalid;
		}

		// Token: 0x0600336D RID: 13165 RVA: 0x000D534F File Offset: 0x000D354F
		public static void WriteMBTeamReferenceToPacket(MBTeam value, CompressionInfo.Integer compressionInfo)
		{
			MBAPI.IMBNetwork.WriteIntToPacket(value.Index, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		// Token: 0x0600336E RID: 13166 RVA: 0x000D536F File Offset: 0x000D356F
		public static void WriteTeamReferenceToPacket(Team value)
		{
			GameNetworkMessage.WriteMBTeamReferenceToPacket((value == null) ? MBTeam.InvalidTeam : value.MBTeam, CompressionMission.TeamCompressionInfo);
		}

		// Token: 0x0600336F RID: 13167 RVA: 0x000D538C File Offset: 0x000D358C
		public static SynchedMissionObject ReadSynchedMissionObjectFromPacket(ref bool bufferReadValid)
		{
			MissionObject missionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
			if (bufferReadValid && missionObject != null)
			{
				SynchedMissionObject synchedMissionObject = (SynchedMissionObject)missionObject;
				bufferReadValid = synchedMissionObject.ReadFromNetwork();
				return synchedMissionObject;
			}
			return null;
		}

		// Token: 0x06003370 RID: 13168 RVA: 0x000D53BC File Offset: 0x000D35BC
		public static MissionObject ReadMissionObjectReferenceFromPacket(ref bool bufferReadValid)
		{
			MissionObjectId missionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref bufferReadValid);
			if (!bufferReadValid || missionObjectId.Id == -1 || GameNetworkMessage.IsClientMissionOver)
			{
				if (missionObjectId.Id != -1)
				{
					MBDebug.Print(string.Concat(new object[]
					{
						"Reading null MissionObject because IsClientMissionOver: ",
						GameNetworkMessage.IsClientMissionOver.ToString(),
						" valid read: ",
						bufferReadValid.ToString(),
						" MissionObject ID: ",
						missionObjectId.Id,
						" runtime: ",
						missionObjectId.CreatedAtRuntime.ToString()
					}), 0, Debug.DebugColor.White, 17592186044416UL);
				}
				return null;
			}
			MissionObject missionObject = Mission.Current.MissionObjects.FirstOrDefault((MissionObject mo) => mo.Id == missionObjectId);
			if (missionObject == null)
			{
				MBDebug.Print(string.Concat(new object[]
				{
					"MissionObject with ID: ",
					missionObjectId.Id,
					" runtime: ",
					missionObjectId.CreatedAtRuntime.ToString(),
					" could not be found."
				}), 0, Debug.DebugColor.White, 17592186044416UL);
			}
			return missionObject;
		}

		// Token: 0x06003371 RID: 13169 RVA: 0x000D5504 File Offset: 0x000D3704
		public static MissionObjectId ReadMissionObjectIdFromPacket(ref bool bufferReadValid)
		{
			bool flag = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
			return new MissionObjectId(GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref bufferReadValid), flag);
		}

		// Token: 0x06003372 RID: 13170 RVA: 0x000D5529 File Offset: 0x000D3729
		public void WriteSynchedMissionObjectToPacket(SynchedMissionObject value)
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(value);
			if (value != null)
			{
				value.WriteToNetwork();
			}
		}

		// Token: 0x06003373 RID: 13171 RVA: 0x000D553A File Offset: 0x000D373A
		public static void WriteMissionObjectReferenceToPacket(MissionObject value)
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket((value != null) ? value.Id : new MissionObjectId(-1, false));
		}

		// Token: 0x06003374 RID: 13172 RVA: 0x000D5553 File Offset: 0x000D3753
		public static void WriteMissionObjectIdToPacket(MissionObjectId value)
		{
			GameNetworkMessage.WriteBoolToPacket(value.CreatedAtRuntime);
			GameNetworkMessage.WriteIntToPacket(value.Id, CompressionBasic.MissionObjectIDCompressionInfo);
		}

		// Token: 0x06003375 RID: 13173 RVA: 0x000D5570 File Offset: 0x000D3770
		public static Vec3 ReadVec3FromPacket(CompressionInfo.Float compressionInfo, ref bool bufferReadValid)
		{
			float num = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
			float num2 = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
			float num3 = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
			return new Vec3(num, num2, num3, -1f);
		}

		// Token: 0x06003376 RID: 13174 RVA: 0x000D55A0 File Offset: 0x000D37A0
		public static void WriteVec3ToPacket(Vec3 value, CompressionInfo.Float compressionInfo)
		{
			GameNetworkMessage.WriteFloatToPacket(value.x, compressionInfo);
			GameNetworkMessage.WriteFloatToPacket(value.y, compressionInfo);
			GameNetworkMessage.WriteFloatToPacket(value.z, compressionInfo);
		}

		// Token: 0x06003377 RID: 13175 RVA: 0x000D55C8 File Offset: 0x000D37C8
		public static Vec2 ReadVec2FromPacket(CompressionInfo.Float compressionInfo, ref bool bufferReadValid)
		{
			float num = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
			float num2 = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
			return new Vec2(num, num2);
		}

		// Token: 0x06003378 RID: 13176 RVA: 0x000D55EA File Offset: 0x000D37EA
		public static void WriteVec2ToPacket(Vec2 value, CompressionInfo.Float compressionInfo)
		{
			GameNetworkMessage.WriteFloatToPacket(value.x, compressionInfo);
			GameNetworkMessage.WriteFloatToPacket(value.y, compressionInfo);
		}

		// Token: 0x06003379 RID: 13177 RVA: 0x000D5604 File Offset: 0x000D3804
		public static Mat3 ReadRotationMatrixFromPacket(ref bool bufferReadValid)
		{
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
			Vec3 vec2 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
			Vec3 vec3 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
			return new Mat3(vec, vec2, vec3);
		}

		// Token: 0x0600337A RID: 13178 RVA: 0x000D563B File Offset: 0x000D383B
		public static void WriteRotationMatrixToPacket(Mat3 value)
		{
			GameNetworkMessage.WriteVec3ToPacket(value.s, CompressionBasic.UnitVectorCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(value.f, CompressionBasic.UnitVectorCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(value.u, CompressionBasic.UnitVectorCompressionInfo);
		}

		// Token: 0x0600337B RID: 13179 RVA: 0x000D5670 File Offset: 0x000D3870
		public static MatrixFrame ReadMatrixFrameFromPacket(ref bool bufferReadValid)
		{
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
			Vec3 vec2 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ScaleCompressionInfo, ref bufferReadValid);
			Mat3 mat = GameNetworkMessage.ReadRotationMatrixFromPacket(ref bufferReadValid);
			MatrixFrame matrixFrame = new MatrixFrame(mat, vec);
			matrixFrame.Scale(vec2);
			return matrixFrame;
		}

		// Token: 0x0600337C RID: 13180 RVA: 0x000D56B0 File Offset: 0x000D38B0
		public static void WriteMatrixFrameToPacket(MatrixFrame frame)
		{
			Vec3 scaleVector = frame.rotation.GetScaleVector();
			MatrixFrame matrixFrame = frame;
			matrixFrame.Scale(new Vec3(1f / scaleVector.x, 1f / scaleVector.y, 1f / scaleVector.z, -1f));
			GameNetworkMessage.WriteVec3ToPacket(matrixFrame.origin, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(scaleVector, CompressionBasic.ScaleCompressionInfo);
			GameNetworkMessage.WriteRotationMatrixToPacket(matrixFrame.rotation);
		}

		// Token: 0x0600337D RID: 13181 RVA: 0x000D5728 File Offset: 0x000D3928
		public static MatrixFrame ReadNonUniformTransformFromPacket(CompressionInfo.Float positionCompressionInfo, CompressionInfo.Float quaternionCompressionInfo, ref bool bufferReadValid)
		{
			MatrixFrame matrixFrame = GameNetworkMessage.ReadUnitTransformFromPacket(positionCompressionInfo, quaternionCompressionInfo, ref bufferReadValid);
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ScaleCompressionInfo, ref bufferReadValid);
			matrixFrame.rotation.ApplyScaleLocal(vec);
			return matrixFrame;
		}

		// Token: 0x0600337E RID: 13182 RVA: 0x000D5758 File Offset: 0x000D3958
		public static void WriteNonUniformTransformToPacket(MatrixFrame frame, CompressionInfo.Float positionCompressionInfo, CompressionInfo.Float quaternionCompressionInfo)
		{
			MatrixFrame matrixFrame = frame;
			Vec3 vec = matrixFrame.rotation.MakeUnit();
			GameNetworkMessage.WriteUnitTransformToPacket(matrixFrame, positionCompressionInfo, quaternionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(vec, CompressionBasic.ScaleCompressionInfo);
		}

		// Token: 0x0600337F RID: 13183 RVA: 0x000D5788 File Offset: 0x000D3988
		public static MatrixFrame ReadTransformFromPacket(CompressionInfo.Float positionCompressionInfo, CompressionInfo.Float quaternionCompressionInfo, ref bool bufferReadValid)
		{
			MatrixFrame matrixFrame = GameNetworkMessage.ReadUnitTransformFromPacket(positionCompressionInfo, quaternionCompressionInfo, ref bufferReadValid);
			if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
			{
				float num = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.ScaleCompressionInfo, ref bufferReadValid);
				matrixFrame.rotation.ApplyScaleLocal(num);
			}
			return matrixFrame;
		}

		// Token: 0x06003380 RID: 13184 RVA: 0x000D57C0 File Offset: 0x000D39C0
		public static void WriteTransformToPacket(MatrixFrame frame, CompressionInfo.Float positionCompressionInfo, CompressionInfo.Float quaternionCompressionInfo)
		{
			MatrixFrame matrixFrame = frame;
			Vec3 vec = matrixFrame.rotation.MakeUnit();
			GameNetworkMessage.WriteUnitTransformToPacket(matrixFrame, positionCompressionInfo, quaternionCompressionInfo);
			bool flag = !vec.x.ApproximatelyEqualsTo(1f, CompressionBasic.ScaleCompressionInfo.GetPrecision());
			GameNetworkMessage.WriteBoolToPacket(flag);
			if (flag)
			{
				GameNetworkMessage.WriteFloatToPacket(vec.x, CompressionBasic.ScaleCompressionInfo);
			}
		}

		// Token: 0x06003381 RID: 13185 RVA: 0x000D581C File Offset: 0x000D3A1C
		public static MatrixFrame ReadUnitTransformFromPacket(CompressionInfo.Float positionCompressionInfo, CompressionInfo.Float quaternionCompressionInfo, ref bool bufferReadValid)
		{
			return new MatrixFrame
			{
				origin = GameNetworkMessage.ReadVec3FromPacket(positionCompressionInfo, ref bufferReadValid),
				rotation = GameNetworkMessage.ReadQuaternionFromPacket(quaternionCompressionInfo, ref bufferReadValid).ToMat3
			};
		}

		// Token: 0x06003382 RID: 13186 RVA: 0x000D5856 File Offset: 0x000D3A56
		public static void WriteUnitTransformToPacket(MatrixFrame frame, CompressionInfo.Float positionCompressionInfo, CompressionInfo.Float quaternionCompressionInfo)
		{
			GameNetworkMessage.WriteVec3ToPacket(frame.origin, positionCompressionInfo);
			GameNetworkMessage.WriteQuaternionToPacket(frame.rotation.ToQuaternion(), quaternionCompressionInfo);
		}

		// Token: 0x06003383 RID: 13187 RVA: 0x000D5878 File Offset: 0x000D3A78
		public static Quaternion ReadQuaternionFromPacket(CompressionInfo.Float compressionInfo, ref bool bufferReadValid)
		{
			Quaternion quaternion = default(Quaternion);
			float num = 0f;
			int num2 = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.OmittedQuaternionComponentIndexCompressionInfo, ref bufferReadValid);
			for (int i = 0; i < 4; i++)
			{
				if (i != num2)
				{
					quaternion[i] = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
					num += quaternion[i] * quaternion[i];
				}
			}
			quaternion[num2] = MathF.Sqrt(1f - num);
			quaternion.SafeNormalize();
			return quaternion;
		}

		// Token: 0x06003384 RID: 13188 RVA: 0x000D58F0 File Offset: 0x000D3AF0
		public static void WriteQuaternionToPacket(Quaternion q, CompressionInfo.Float compressionInfo)
		{
			int num = -1;
			float num2 = 0f;
			Quaternion quaternion = q;
			quaternion.SafeNormalize();
			for (int i = 0; i < 4; i++)
			{
				float num3 = MathF.Abs(quaternion[i]);
				if (num3 > num2)
				{
					num2 = num3;
					num = i;
				}
			}
			if (quaternion[num] < 0f)
			{
				quaternion.Flip();
			}
			GameNetworkMessage.WriteIntToPacket(num, CompressionBasic.OmittedQuaternionComponentIndexCompressionInfo);
			for (int j = 0; j < 4; j++)
			{
				if (j != num)
				{
					GameNetworkMessage.WriteFloatToPacket(quaternion[j], compressionInfo);
				}
			}
		}

		// Token: 0x06003385 RID: 13189 RVA: 0x000D597C File Offset: 0x000D3B7C
		public static void WriteBodyPropertiesToPacket(BodyProperties bodyProperties)
		{
			GameNetworkMessage.WriteFloatToPacket(bodyProperties.Age, CompressionBasic.AgentAgeCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(bodyProperties.Weight, CompressionBasic.FaceKeyDataCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(bodyProperties.Build, CompressionBasic.FaceKeyDataCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart1, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart2, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart3, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart4, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart5, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart6, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart7, CompressionBasic.DebugULongNonCompressionInfo);
			GameNetworkMessage.WriteUlongToPacket(bodyProperties.KeyPart8, CompressionBasic.DebugULongNonCompressionInfo);
		}

		// Token: 0x06003386 RID: 13190 RVA: 0x000D5A44 File Offset: 0x000D3C44
		public static BodyProperties ReadBodyPropertiesFromPacket(ref bool bufferReadValid)
		{
			float num = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AgentAgeCompressionInfo, ref bufferReadValid);
			float num2 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.FaceKeyDataCompressionInfo, ref bufferReadValid);
			float num3 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.FaceKeyDataCompressionInfo, ref bufferReadValid);
			ulong num4 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
			ulong num5 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
			ulong num6 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
			ulong num7 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
			ulong num8 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
			ulong num9 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
			ulong num10 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
			ulong num11 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
			if (bufferReadValid)
			{
				return new BodyProperties(new DynamicBodyProperties(num, num2, num3), new StaticBodyProperties(num4, num5, num6, num7, num8, num9, num10, num11));
			}
			return default(BodyProperties);
		}

		// Token: 0x040015DC RID: 5596
		private static readonly Encoding StringEncoding = new UTF8Encoding();

		// Token: 0x040015DD RID: 5597
		private static readonly CompressionInfo.Integer TestValueCompressionInfo = new CompressionInfo.Integer(0, 3);

		// Token: 0x040015DE RID: 5598
		private const int ConstTestValue = 5;

		// Token: 0x020006BD RID: 1725
		// (Invoke) Token: 0x06003FD0 RID: 16336
		public delegate bool ClientMessageHandlerDelegate<T>(NetworkCommunicator peer, T message) where T : GameNetworkMessage;

		// Token: 0x020006BE RID: 1726
		// (Invoke) Token: 0x06003FD4 RID: 16340
		public delegate void ServerMessageHandlerDelegate<T>(T message) where T : GameNetworkMessage;
	}
}
