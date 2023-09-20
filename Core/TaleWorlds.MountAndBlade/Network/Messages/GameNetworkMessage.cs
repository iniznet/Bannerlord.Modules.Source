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
	public abstract class GameNetworkMessage
	{
		public int MessageId { get; set; }

		internal void Write()
		{
			DebugNetworkEventStatistics.StartEvent(base.GetType().Name, this.MessageId);
			GameNetworkMessage.WriteIntToPacket(this.MessageId, GameNetwork.IsClientOrReplay ? CompressionBasic.NetworkComponentEventTypeFromClientCompressionInfo : CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo);
			this.OnWrite();
			GameNetworkMessage.WriteIntToPacket(5, GameNetworkMessage.TestValueCompressionInfo);
			DebugNetworkEventStatistics.EndEvent();
		}

		protected abstract void OnWrite();

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

		protected abstract bool OnRead();

		internal MultiplayerMessageFilter GetLogFilter()
		{
			return this.OnGetLogFilter();
		}

		protected abstract MultiplayerMessageFilter OnGetLogFilter();

		internal string GetLogFormat()
		{
			return this.OnGetLogFormat();
		}

		protected abstract string OnGetLogFormat();

		public static bool IsClientMissionOver
		{
			get
			{
				return GameNetwork.IsClient && !NetworkMain.GameClient.IsInGame;
			}
		}

		public static bool ReadBoolFromPacket(ref bool bufferReadValid)
		{
			CompressionInfo.Integer integer = new CompressionInfo.Integer(0, 1);
			int num = 0;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadIntFromPacket(ref integer, out num);
			return num != 0;
		}

		public static void WriteBoolToPacket(bool value)
		{
			CompressionInfo.Integer integer = new CompressionInfo.Integer(0, 1);
			MBAPI.IMBNetwork.WriteIntToPacket(value ? 1 : 0, ref integer);
			DebugNetworkEventStatistics.AddDataToStatistic(integer.GetNumBits());
		}

		public static int ReadIntFromPacket(CompressionInfo.Integer compressionInfo, ref bool bufferReadValid)
		{
			int num = 0;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadIntFromPacket(ref compressionInfo, out num);
			return num;
		}

		public static void WriteIntToPacket(int value, CompressionInfo.Integer compressionInfo)
		{
			MBAPI.IMBNetwork.WriteIntToPacket(value, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		public static uint ReadUintFromPacket(CompressionInfo.UnsignedInteger compressionInfo, ref bool bufferReadValid)
		{
			uint num = 0U;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadUintFromPacket(ref compressionInfo, out num);
			return num;
		}

		public static void WriteUintToPacket(uint value, CompressionInfo.UnsignedInteger compressionInfo)
		{
			MBAPI.IMBNetwork.WriteUintToPacket(value, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		public static long ReadLongFromPacket(CompressionInfo.LongInteger compressionInfo, ref bool bufferReadValid)
		{
			long num = 0L;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadLongFromPacket(ref compressionInfo, out num);
			return num;
		}

		public static void WriteLongToPacket(long value, CompressionInfo.LongInteger compressionInfo)
		{
			MBAPI.IMBNetwork.WriteLongToPacket(value, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		public static ulong ReadUlongFromPacket(CompressionInfo.UnsignedLongInteger compressionInfo, ref bool bufferReadValid)
		{
			ulong num = 0UL;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadUlongFromPacket(ref compressionInfo, out num);
			return num;
		}

		public static void WriteUlongToPacket(ulong value, CompressionInfo.UnsignedLongInteger compressionInfo)
		{
			MBAPI.IMBNetwork.WriteUlongToPacket(value, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		public static float ReadFloatFromPacket(CompressionInfo.Float compressionInfo, ref bool bufferReadValid)
		{
			float num = 0f;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadFloatFromPacket(ref compressionInfo, out num);
			return num;
		}

		public static void WriteFloatToPacket(float value, CompressionInfo.Float compressionInfo)
		{
			MBAPI.IMBNetwork.WriteFloatToPacket(value, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		public static string ReadStringFromPacket(ref bool bufferReadValid)
		{
			byte[] array = new byte[1024];
			int num = GameNetworkMessage.ReadByteArrayFromPacket(array, 0, 1024, ref bufferReadValid);
			return GameNetworkMessage.StringEncoding.GetString(array, 0, num);
		}

		public static void WriteStringToPacket(string value)
		{
			byte[] array = (string.IsNullOrEmpty(value) ? new byte[0] : GameNetworkMessage.StringEncoding.GetBytes(value));
			GameNetworkMessage.WriteByteArrayToPacket(array, 0, array.Length);
		}

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

		public static int ReadByteArrayFromPacket(byte[] buffer, int offset, int bufferCapacity, ref bool bufferReadValid)
		{
			return MBAPI.IMBNetwork.ReadByteArrayFromPacket(buffer, offset, bufferCapacity, ref bufferReadValid);
		}

		public static void WriteByteArrayToPacket(byte[] value, int offset, int size)
		{
			MBAPI.IMBNetwork.WriteByteArrayToPacket(value, offset, size);
			DebugNetworkEventStatistics.AddDataToStatistic(MathF.Min(size, 1024) + 10);
		}

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

		public static void WriteActionSetReferenceToPacket(MBActionSet actionSet, CompressionInfo.Integer compressionInfo)
		{
			MBAPI.IMBNetwork.WriteIntToPacket(actionSet.Index, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		public static int ReadAgentIndexFromPacket(CompressionInfo.Integer compressionInfo, ref bool bufferReadValid)
		{
			int num = -1;
			bufferReadValid = bufferReadValid && MBAPI.IMBNetwork.ReadIntFromPacket(ref compressionInfo, out num);
			return num;
		}

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

		public static void WriteAgentReferenceToPacket(Agent value)
		{
			CompressionInfo.Integer agentCompressionInfo = CompressionMission.AgentCompressionInfo;
			MBAPI.IMBNetwork.WriteIntToPacket((value != null) ? value.Index : (-1), ref agentCompressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(agentCompressionInfo.GetNumBits());
		}

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

		public static void WriteObjectReferenceToPacket(MBObjectBase value, CompressionInfo.UnsignedInteger compressionInfo)
		{
			MBAPI.IMBNetwork.WriteUintToPacket((value != null) ? value.Id.InternalValue : 0U, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

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

		public static NetworkCommunicator ReadNetworkPeerReferenceFromPacket(ref bool bufferReadValid, bool canReturnNull = false)
		{
			VirtualPlayer virtualPlayer = GameNetworkMessage.ReadVirtualPlayerReferenceToPacket(ref bufferReadValid, canReturnNull);
			return ((virtualPlayer != null) ? virtualPlayer.Communicator : null) as NetworkCommunicator;
		}

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

		public static void WriteNetworkPeerReferenceToPacket(NetworkCommunicator networkCommunicator)
		{
			GameNetworkMessage.WriteVirtualPlayerReferenceToPacket((networkCommunicator != null) ? networkCommunicator.VirtualPlayer : null);
		}

		public static MBTeam ReadMBTeamReferenceFromPacket(CompressionInfo.Integer compressionInfo, ref bool bufferReadValid)
		{
			int num = GameNetworkMessage.ReadIntFromPacket(compressionInfo, ref bufferReadValid);
			if (!GameNetworkMessage.IsClientMissionOver & bufferReadValid)
			{
				return new MBTeam(Mission.Current, num);
			}
			return MBTeam.InvalidTeam;
		}

		public static Team ReadTeamReferenceFromPacket(ref bool bufferReadValid)
		{
			MBTeam mbteam = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref bufferReadValid);
			if (mbteam.IsValid & bufferReadValid)
			{
				return Mission.Current.Teams.Find(mbteam);
			}
			return Team.Invalid;
		}

		public static void WriteMBTeamReferenceToPacket(MBTeam value, CompressionInfo.Integer compressionInfo)
		{
			MBAPI.IMBNetwork.WriteIntToPacket(value.Index, ref compressionInfo);
			DebugNetworkEventStatistics.AddDataToStatistic(compressionInfo.GetNumBits());
		}

		public static void WriteTeamReferenceToPacket(Team value)
		{
			GameNetworkMessage.WriteMBTeamReferenceToPacket((value == null) ? MBTeam.InvalidTeam : value.MBTeam, CompressionMission.TeamCompressionInfo);
		}

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

		public static MissionObjectId ReadMissionObjectIdFromPacket(ref bool bufferReadValid)
		{
			bool flag = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
			return new MissionObjectId(GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref bufferReadValid), flag);
		}

		public void WriteSynchedMissionObjectToPacket(SynchedMissionObject value)
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(value);
			if (value != null)
			{
				value.WriteToNetwork();
			}
		}

		public static void WriteMissionObjectReferenceToPacket(MissionObject value)
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket((value != null) ? value.Id : new MissionObjectId(-1, false));
		}

		public static void WriteMissionObjectIdToPacket(MissionObjectId value)
		{
			GameNetworkMessage.WriteBoolToPacket(value.CreatedAtRuntime);
			GameNetworkMessage.WriteIntToPacket(value.Id, CompressionBasic.MissionObjectIDCompressionInfo);
		}

		public static Vec3 ReadVec3FromPacket(CompressionInfo.Float compressionInfo, ref bool bufferReadValid)
		{
			float num = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
			float num2 = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
			float num3 = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
			return new Vec3(num, num2, num3, -1f);
		}

		public static void WriteVec3ToPacket(Vec3 value, CompressionInfo.Float compressionInfo)
		{
			GameNetworkMessage.WriteFloatToPacket(value.x, compressionInfo);
			GameNetworkMessage.WriteFloatToPacket(value.y, compressionInfo);
			GameNetworkMessage.WriteFloatToPacket(value.z, compressionInfo);
		}

		public static Vec2 ReadVec2FromPacket(CompressionInfo.Float compressionInfo, ref bool bufferReadValid)
		{
			float num = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
			float num2 = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
			return new Vec2(num, num2);
		}

		public static void WriteVec2ToPacket(Vec2 value, CompressionInfo.Float compressionInfo)
		{
			GameNetworkMessage.WriteFloatToPacket(value.x, compressionInfo);
			GameNetworkMessage.WriteFloatToPacket(value.y, compressionInfo);
		}

		public static Mat3 ReadRotationMatrixFromPacket(ref bool bufferReadValid)
		{
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
			Vec3 vec2 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
			Vec3 vec3 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
			return new Mat3(vec, vec2, vec3);
		}

		public static void WriteRotationMatrixToPacket(Mat3 value)
		{
			GameNetworkMessage.WriteVec3ToPacket(value.s, CompressionBasic.UnitVectorCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(value.f, CompressionBasic.UnitVectorCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(value.u, CompressionBasic.UnitVectorCompressionInfo);
		}

		public static MatrixFrame ReadMatrixFrameFromPacket(ref bool bufferReadValid)
		{
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
			Vec3 vec2 = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ScaleCompressionInfo, ref bufferReadValid);
			Mat3 mat = GameNetworkMessage.ReadRotationMatrixFromPacket(ref bufferReadValid);
			MatrixFrame matrixFrame = new MatrixFrame(mat, vec);
			matrixFrame.Scale(vec2);
			return matrixFrame;
		}

		public static void WriteMatrixFrameToPacket(MatrixFrame frame)
		{
			Vec3 scaleVector = frame.rotation.GetScaleVector();
			MatrixFrame matrixFrame = frame;
			matrixFrame.Scale(new Vec3(1f / scaleVector.x, 1f / scaleVector.y, 1f / scaleVector.z, -1f));
			GameNetworkMessage.WriteVec3ToPacket(matrixFrame.origin, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(scaleVector, CompressionBasic.ScaleCompressionInfo);
			GameNetworkMessage.WriteRotationMatrixToPacket(matrixFrame.rotation);
		}

		public static MatrixFrame ReadNonUniformTransformFromPacket(CompressionInfo.Float positionCompressionInfo, CompressionInfo.Float quaternionCompressionInfo, ref bool bufferReadValid)
		{
			MatrixFrame matrixFrame = GameNetworkMessage.ReadUnitTransformFromPacket(positionCompressionInfo, quaternionCompressionInfo, ref bufferReadValid);
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.ScaleCompressionInfo, ref bufferReadValid);
			matrixFrame.rotation.ApplyScaleLocal(vec);
			return matrixFrame;
		}

		public static void WriteNonUniformTransformToPacket(MatrixFrame frame, CompressionInfo.Float positionCompressionInfo, CompressionInfo.Float quaternionCompressionInfo)
		{
			MatrixFrame matrixFrame = frame;
			Vec3 vec = matrixFrame.rotation.MakeUnit();
			GameNetworkMessage.WriteUnitTransformToPacket(matrixFrame, positionCompressionInfo, quaternionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(vec, CompressionBasic.ScaleCompressionInfo);
		}

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

		public static MatrixFrame ReadUnitTransformFromPacket(CompressionInfo.Float positionCompressionInfo, CompressionInfo.Float quaternionCompressionInfo, ref bool bufferReadValid)
		{
			return new MatrixFrame
			{
				origin = GameNetworkMessage.ReadVec3FromPacket(positionCompressionInfo, ref bufferReadValid),
				rotation = GameNetworkMessage.ReadQuaternionFromPacket(quaternionCompressionInfo, ref bufferReadValid).ToMat3
			};
		}

		public static void WriteUnitTransformToPacket(MatrixFrame frame, CompressionInfo.Float positionCompressionInfo, CompressionInfo.Float quaternionCompressionInfo)
		{
			GameNetworkMessage.WriteVec3ToPacket(frame.origin, positionCompressionInfo);
			GameNetworkMessage.WriteQuaternionToPacket(frame.rotation.ToQuaternion(), quaternionCompressionInfo);
		}

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

		private static readonly Encoding StringEncoding = new UTF8Encoding();

		private static readonly CompressionInfo.Integer TestValueCompressionInfo = new CompressionInfo.Integer(0, 3);

		private const int ConstTestValue = 5;

		public delegate bool ClientMessageHandlerDelegate<T>(NetworkCommunicator peer, T message) where T : GameNetworkMessage;

		public delegate void ServerMessageHandlerDelegate<T>(T message) where T : GameNetworkMessage;
	}
}
