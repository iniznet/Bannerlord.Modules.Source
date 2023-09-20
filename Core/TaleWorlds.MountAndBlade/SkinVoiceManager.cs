using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public static class SkinVoiceManager
	{
		public static int GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassName(string className)
		{
			return MBAPI.IMBVoiceManager.GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassName(className);
		}

		public static void GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassName(string className, int[] definitionIndices)
		{
			MBAPI.IMBVoiceManager.GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassName(className, definitionIndices);
		}

		public enum CombatVoiceNetworkPredictionType
		{
			Prediction,
			OwnerPrediction,
			NoPrediction
		}

		public struct SkinVoiceType
		{
			public string TypeID { get; private set; }

			public int Index { get; private set; }

			public SkinVoiceType(string typeID)
			{
				this.TypeID = typeID;
				this.Index = MBAPI.IMBVoiceManager.GetVoiceTypeIndex(typeID);
			}

			public TextObject GetName()
			{
				return GameTexts.FindText("str_taunt_name", this.TypeID);
			}
		}

		public static class VoiceType
		{
			public static readonly SkinVoiceManager.SkinVoiceType Grunt = new SkinVoiceManager.SkinVoiceType("Grunt");

			public static readonly SkinVoiceManager.SkinVoiceType Jump = new SkinVoiceManager.SkinVoiceType("Jump");

			public static readonly SkinVoiceManager.SkinVoiceType Yell = new SkinVoiceManager.SkinVoiceType("Yell");

			public static readonly SkinVoiceManager.SkinVoiceType Pain = new SkinVoiceManager.SkinVoiceType("Pain");

			public static readonly SkinVoiceManager.SkinVoiceType Death = new SkinVoiceManager.SkinVoiceType("Death");

			public static readonly SkinVoiceManager.SkinVoiceType Stun = new SkinVoiceManager.SkinVoiceType("Stun");

			public static readonly SkinVoiceManager.SkinVoiceType Fear = new SkinVoiceManager.SkinVoiceType("Fear");

			public static readonly SkinVoiceManager.SkinVoiceType Climb = new SkinVoiceManager.SkinVoiceType("Climb");

			public static readonly SkinVoiceManager.SkinVoiceType Focus = new SkinVoiceManager.SkinVoiceType("Focus");

			public static readonly SkinVoiceManager.SkinVoiceType Debacle = new SkinVoiceManager.SkinVoiceType("Debacle");

			public static readonly SkinVoiceManager.SkinVoiceType Victory = new SkinVoiceManager.SkinVoiceType("Victory");

			public static readonly SkinVoiceManager.SkinVoiceType HorseStop = new SkinVoiceManager.SkinVoiceType("HorseStop");

			public static readonly SkinVoiceManager.SkinVoiceType HorseRally = new SkinVoiceManager.SkinVoiceType("HorseRally");

			public static readonly SkinVoiceManager.SkinVoiceType Infantry = new SkinVoiceManager.SkinVoiceType("Infantry");

			public static readonly SkinVoiceManager.SkinVoiceType Cavalry = new SkinVoiceManager.SkinVoiceType("Cavalry");

			public static readonly SkinVoiceManager.SkinVoiceType Archers = new SkinVoiceManager.SkinVoiceType("Archers");

			public static readonly SkinVoiceManager.SkinVoiceType HorseArchers = new SkinVoiceManager.SkinVoiceType("HorseArchers");

			public static readonly SkinVoiceManager.SkinVoiceType Everyone = new SkinVoiceManager.SkinVoiceType("Everyone");

			public static readonly SkinVoiceManager.SkinVoiceType MixedFormation = new SkinVoiceManager.SkinVoiceType("Mixed");

			public static readonly SkinVoiceManager.SkinVoiceType Move = new SkinVoiceManager.SkinVoiceType("Move");

			public static readonly SkinVoiceManager.SkinVoiceType Follow = new SkinVoiceManager.SkinVoiceType("Follow");

			public static readonly SkinVoiceManager.SkinVoiceType Charge = new SkinVoiceManager.SkinVoiceType("Charge");

			public static readonly SkinVoiceManager.SkinVoiceType Advance = new SkinVoiceManager.SkinVoiceType("Advance");

			public static readonly SkinVoiceManager.SkinVoiceType FallBack = new SkinVoiceManager.SkinVoiceType("FallBack");

			public static readonly SkinVoiceManager.SkinVoiceType Stop = new SkinVoiceManager.SkinVoiceType("Stop");

			public static readonly SkinVoiceManager.SkinVoiceType Retreat = new SkinVoiceManager.SkinVoiceType("Retreat");

			public static readonly SkinVoiceManager.SkinVoiceType Mount = new SkinVoiceManager.SkinVoiceType("Mount");

			public static readonly SkinVoiceManager.SkinVoiceType Dismount = new SkinVoiceManager.SkinVoiceType("Dismount");

			public static readonly SkinVoiceManager.SkinVoiceType FireAtWill = new SkinVoiceManager.SkinVoiceType("FireAtWill");

			public static readonly SkinVoiceManager.SkinVoiceType HoldFire = new SkinVoiceManager.SkinVoiceType("HoldFire");

			public static readonly SkinVoiceManager.SkinVoiceType PickSpears = new SkinVoiceManager.SkinVoiceType("PickSpears");

			public static readonly SkinVoiceManager.SkinVoiceType PickDefault = new SkinVoiceManager.SkinVoiceType("PickDefault");

			public static readonly SkinVoiceManager.SkinVoiceType FaceEnemy = new SkinVoiceManager.SkinVoiceType("FaceEnemy");

			public static readonly SkinVoiceManager.SkinVoiceType FaceDirection = new SkinVoiceManager.SkinVoiceType("FaceDirection");

			public static readonly SkinVoiceManager.SkinVoiceType UseSiegeWeapon = new SkinVoiceManager.SkinVoiceType("UseSiegeWeapon");

			public static readonly SkinVoiceManager.SkinVoiceType UseLadders = new SkinVoiceManager.SkinVoiceType("UseLadders");

			public static readonly SkinVoiceManager.SkinVoiceType AttackGate = new SkinVoiceManager.SkinVoiceType("AttackGate");

			public static readonly SkinVoiceManager.SkinVoiceType CommandDelegate = new SkinVoiceManager.SkinVoiceType("CommandDelegate");

			public static readonly SkinVoiceManager.SkinVoiceType CommandUndelegate = new SkinVoiceManager.SkinVoiceType("CommandUndelegate");

			public static readonly SkinVoiceManager.SkinVoiceType FormLine = new SkinVoiceManager.SkinVoiceType("FormLine");

			public static readonly SkinVoiceManager.SkinVoiceType FormShieldWall = new SkinVoiceManager.SkinVoiceType("FormShieldWall");

			public static readonly SkinVoiceManager.SkinVoiceType FormLoose = new SkinVoiceManager.SkinVoiceType("FormLoose");

			public static readonly SkinVoiceManager.SkinVoiceType FormCircle = new SkinVoiceManager.SkinVoiceType("FormCircle");

			public static readonly SkinVoiceManager.SkinVoiceType FormSquare = new SkinVoiceManager.SkinVoiceType("FormSquare");

			public static readonly SkinVoiceManager.SkinVoiceType FormSkein = new SkinVoiceManager.SkinVoiceType("FormSkein");

			public static readonly SkinVoiceManager.SkinVoiceType FormColumn = new SkinVoiceManager.SkinVoiceType("FormColumn");

			public static readonly SkinVoiceManager.SkinVoiceType FormScatter = new SkinVoiceManager.SkinVoiceType("FormScatter");

			public static readonly SkinVoiceManager.SkinVoiceType[] MpBarks = new SkinVoiceManager.SkinVoiceType[]
			{
				new SkinVoiceManager.SkinVoiceType("MpDefend"),
				new SkinVoiceManager.SkinVoiceType("MpAttack"),
				new SkinVoiceManager.SkinVoiceType("MpHelp"),
				new SkinVoiceManager.SkinVoiceType("MpSpot"),
				new SkinVoiceManager.SkinVoiceType("MpThanks"),
				new SkinVoiceManager.SkinVoiceType("MpSorry"),
				new SkinVoiceManager.SkinVoiceType("MpAffirmative"),
				new SkinVoiceManager.SkinVoiceType("MpNegative"),
				new SkinVoiceManager.SkinVoiceType("MpRegroup")
			};

			public static readonly SkinVoiceManager.SkinVoiceType MpDefend = SkinVoiceManager.VoiceType.MpBarks[0];

			public static readonly SkinVoiceManager.SkinVoiceType MpAttack = SkinVoiceManager.VoiceType.MpBarks[1];

			public static readonly SkinVoiceManager.SkinVoiceType MpHelp = SkinVoiceManager.VoiceType.MpBarks[2];

			public static readonly SkinVoiceManager.SkinVoiceType MpSpot = SkinVoiceManager.VoiceType.MpBarks[3];

			public static readonly SkinVoiceManager.SkinVoiceType MpThanks = SkinVoiceManager.VoiceType.MpBarks[4];

			public static readonly SkinVoiceManager.SkinVoiceType MpSorry = SkinVoiceManager.VoiceType.MpBarks[5];

			public static readonly SkinVoiceManager.SkinVoiceType MpAffirmative = SkinVoiceManager.VoiceType.MpBarks[6];

			public static readonly SkinVoiceManager.SkinVoiceType MpNegative = SkinVoiceManager.VoiceType.MpBarks[7];

			public static readonly SkinVoiceManager.SkinVoiceType MpRegroup = SkinVoiceManager.VoiceType.MpBarks[8];

			public static readonly SkinVoiceManager.SkinVoiceType Idle = new SkinVoiceManager.SkinVoiceType("Idle");

			public static readonly SkinVoiceManager.SkinVoiceType Neigh = new SkinVoiceManager.SkinVoiceType("Neigh");

			public static readonly SkinVoiceManager.SkinVoiceType Collide = new SkinVoiceManager.SkinVoiceType("Collide");
		}
	}
}
