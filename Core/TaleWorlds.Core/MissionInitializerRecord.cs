using System;
using System.Runtime.InteropServices;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x020000AE RID: 174
	public struct MissionInitializerRecord : ISerializableObject
	{
		// Token: 0x0600086E RID: 2158 RVA: 0x0001CA48 File Offset: 0x0001AC48
		public MissionInitializerRecord(string name)
		{
			this.SceneName = name;
			this.TerrainType = -1;
			this.DamageToPlayerMultiplier = 1f;
			this.DamageToFriendsMultiplier = 1f;
			this.DamageFromPlayerToFriendsMultiplier = 1f;
			this.SceneLevels = "";
			this.TimeOfDay = 6f;
			this.NeedsRandomTerrain = false;
			this.RandomTerrainSeed = 0;
			this.PlayingInCampaignMode = false;
			this.EnableSceneRecording = false;
			this.SceneUpgradeLevel = 0;
			this.SceneHasMapPatch = false;
			this.PatchCoordinates = Vec2.Zero;
			this.PatchEncounterDir = Vec2.Zero;
			this.DisableDynamicPointlightShadows = false;
			this.DoNotUseLoadingScreen = false;
			this.AtlasGroup = 0;
			this.AtmosphereOnCampaign = null;
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x0001CAF8 File Offset: 0x0001ACF8
		void ISerializableObject.DeserializeFrom(IReader reader)
		{
			this.SceneName = reader.ReadString();
			this.SceneLevels = reader.ReadString();
			this.TimeOfDay = reader.ReadFloat();
			this.NeedsRandomTerrain = reader.ReadBool();
			this.RandomTerrainSeed = reader.ReadInt();
			this.EnableSceneRecording = reader.ReadBool();
			this.SceneUpgradeLevel = reader.ReadInt();
			this.PlayingInCampaignMode = reader.ReadBool();
			this.DisableDynamicPointlightShadows = reader.ReadBool();
			this.DoNotUseLoadingScreen = reader.ReadBool();
			if (reader.ReadBool())
			{
				this.AtmosphereOnCampaign = new AtmosphereInfo();
				this.AtmosphereOnCampaign.DeserializeFrom(reader);
				return;
			}
			this.AtmosphereOnCampaign = null;
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x0001CBA4 File Offset: 0x0001ADA4
		void ISerializableObject.SerializeTo(IWriter writer)
		{
			writer.WriteString(this.SceneName);
			writer.WriteString(this.SceneLevels);
			writer.WriteFloat(this.TimeOfDay);
			writer.WriteBool(this.NeedsRandomTerrain);
			writer.WriteInt(this.RandomTerrainSeed);
			writer.WriteBool(this.EnableSceneRecording);
			writer.WriteInt(this.SceneUpgradeLevel);
			writer.WriteBool(this.PlayingInCampaignMode);
			writer.WriteBool(this.DisableDynamicPointlightShadows);
			writer.WriteBool(this.DoNotUseLoadingScreen);
			writer.WriteInt(this.AtlasGroup);
			writer.WriteBool(this.AtmosphereOnCampaign != null);
			AtmosphereInfo atmosphereOnCampaign = this.AtmosphereOnCampaign;
			if (atmosphereOnCampaign == null)
			{
				return;
			}
			atmosphereOnCampaign.SerializeTo(writer);
		}

		// Token: 0x040004C0 RID: 1216
		public int TerrainType;

		// Token: 0x040004C1 RID: 1217
		public float DamageToPlayerMultiplier;

		// Token: 0x040004C2 RID: 1218
		public float DamageToFriendsMultiplier;

		// Token: 0x040004C3 RID: 1219
		public float DamageFromPlayerToFriendsMultiplier;

		// Token: 0x040004C4 RID: 1220
		public float TimeOfDay;

		// Token: 0x040004C5 RID: 1221
		[MarshalAs(UnmanagedType.I1)]
		public bool NeedsRandomTerrain;

		// Token: 0x040004C6 RID: 1222
		public int RandomTerrainSeed;

		// Token: 0x040004C7 RID: 1223
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string SceneName;

		// Token: 0x040004C8 RID: 1224
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string SceneLevels;

		// Token: 0x040004C9 RID: 1225
		[MarshalAs(UnmanagedType.I1)]
		public bool PlayingInCampaignMode;

		// Token: 0x040004CA RID: 1226
		[MarshalAs(UnmanagedType.I1)]
		public bool EnableSceneRecording;

		// Token: 0x040004CB RID: 1227
		public int SceneUpgradeLevel;

		// Token: 0x040004CC RID: 1228
		[MarshalAs(UnmanagedType.I1)]
		public bool SceneHasMapPatch;

		// Token: 0x040004CD RID: 1229
		public Vec2 PatchCoordinates;

		// Token: 0x040004CE RID: 1230
		public Vec2 PatchEncounterDir;

		// Token: 0x040004CF RID: 1231
		[MarshalAs(UnmanagedType.I1)]
		public bool DoNotUseLoadingScreen;

		// Token: 0x040004D0 RID: 1232
		[MarshalAs(UnmanagedType.I1)]
		public bool DisableDynamicPointlightShadows;

		// Token: 0x040004D1 RID: 1233
		public int AtlasGroup;

		// Token: 0x040004D2 RID: 1234
		public AtmosphereInfo AtmosphereOnCampaign;
	}
}
