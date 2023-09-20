using System;
using System.Runtime.InteropServices;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public struct MissionInitializerRecord : ISerializableObject
	{
		public MissionInitializerRecord(string name)
		{
			this.TerrainType = -1;
			this.DamageToPlayerMultiplier = 1f;
			this.DamageToFriendsMultiplier = 1f;
			this.DamageFromPlayerToFriendsMultiplier = 1f;
			this.TimeOfDay = 6f;
			this.NeedsRandomTerrain = false;
			this.RandomTerrainSeed = 0;
			this.SceneName = name;
			this.SceneLevels = "";
			this.PlayingInCampaignMode = false;
			this.EnableSceneRecording = false;
			this.SceneUpgradeLevel = 0;
			this.SceneHasMapPatch = false;
			this.PatchCoordinates = Vec2.Zero;
			this.PatchEncounterDir = Vec2.Zero;
			this.DoNotUseLoadingScreen = false;
			this.DisableDynamicPointlightShadows = false;
			this.DecalAtlasGroup = 0;
			this.AtmosphereOnCampaign = AtmosphereInfo.GetInvalidAtmosphereInfo();
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
				this.AtmosphereOnCampaign = AtmosphereInfo.GetInvalidAtmosphereInfo();
				this.AtmosphereOnCampaign.DeserializeFrom(reader);
			}
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
			writer.WriteInt(this.DecalAtlasGroup);
			bool isValid = this.AtmosphereOnCampaign.IsValid;
			writer.WriteBool(isValid);
			if (isValid)
			{
				this.AtmosphereOnCampaign.SerializeTo(writer);
			}
		}

		public int TerrainType;

		public float DamageToPlayerMultiplier;

		public float DamageToFriendsMultiplier;

		public float DamageFromPlayerToFriendsMultiplier;

		public float TimeOfDay;

		[MarshalAs(UnmanagedType.U1)]
		public bool NeedsRandomTerrain;

		public int RandomTerrainSeed;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string SceneName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string SceneLevels;

		[MarshalAs(UnmanagedType.U1)]
		public bool PlayingInCampaignMode;

		[MarshalAs(UnmanagedType.U1)]
		public bool EnableSceneRecording;

		public int SceneUpgradeLevel;

		[MarshalAs(UnmanagedType.U1)]
		public bool SceneHasMapPatch;

		public Vec2 PatchCoordinates;

		public Vec2 PatchEncounterDir;

		[MarshalAs(UnmanagedType.U1)]
		public bool DoNotUseLoadingScreen;

		[MarshalAs(UnmanagedType.U1)]
		public bool DisableDynamicPointlightShadows;

		public int DecalAtlasGroup;

		public AtmosphereInfo AtmosphereOnCampaign;
	}
}
