using System;
using System.Runtime.InteropServices;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public struct MissionInitializerRecord : ISerializableObject
	{
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

		public int TerrainType;

		public float DamageToPlayerMultiplier;

		public float DamageToFriendsMultiplier;

		public float DamageFromPlayerToFriendsMultiplier;

		public float TimeOfDay;

		[MarshalAs(UnmanagedType.I1)]
		public bool NeedsRandomTerrain;

		public int RandomTerrainSeed;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string SceneName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string SceneLevels;

		[MarshalAs(UnmanagedType.I1)]
		public bool PlayingInCampaignMode;

		[MarshalAs(UnmanagedType.I1)]
		public bool EnableSceneRecording;

		public int SceneUpgradeLevel;

		[MarshalAs(UnmanagedType.I1)]
		public bool SceneHasMapPatch;

		public Vec2 PatchCoordinates;

		public Vec2 PatchEncounterDir;

		[MarshalAs(UnmanagedType.I1)]
		public bool DoNotUseLoadingScreen;

		[MarshalAs(UnmanagedType.I1)]
		public bool DisableDynamicPointlightShadows;

		public int AtlasGroup;

		public AtmosphereInfo AtmosphereOnCampaign;
	}
}
