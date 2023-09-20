using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200038F RID: 911
	public static class SkinVoiceManager
	{
		// Token: 0x060031E5 RID: 12773 RVA: 0x000CF504 File Offset: 0x000CD704
		public static int GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassName(string className)
		{
			return MBAPI.IMBVoiceManager.GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassName(className);
		}

		// Token: 0x060031E6 RID: 12774 RVA: 0x000CF511 File Offset: 0x000CD711
		public static void GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassName(string className, int[] definitionIndices)
		{
			MBAPI.IMBVoiceManager.GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassName(className, definitionIndices);
		}

		// Token: 0x02000697 RID: 1687
		public enum CombatVoiceNetworkPredictionType
		{
			// Token: 0x04002199 RID: 8601
			Prediction,
			// Token: 0x0400219A RID: 8602
			OwnerPrediction,
			// Token: 0x0400219B RID: 8603
			NoPrediction
		}

		// Token: 0x02000698 RID: 1688
		public struct SkinVoiceType
		{
			// Token: 0x170009E3 RID: 2531
			// (get) Token: 0x06003EE1 RID: 16097 RVA: 0x000F60FA File Offset: 0x000F42FA
			// (set) Token: 0x06003EE2 RID: 16098 RVA: 0x000F6102 File Offset: 0x000F4302
			public string TypeID { get; private set; }

			// Token: 0x170009E4 RID: 2532
			// (get) Token: 0x06003EE3 RID: 16099 RVA: 0x000F610B File Offset: 0x000F430B
			// (set) Token: 0x06003EE4 RID: 16100 RVA: 0x000F6113 File Offset: 0x000F4313
			public int Index { get; private set; }

			// Token: 0x06003EE5 RID: 16101 RVA: 0x000F611C File Offset: 0x000F431C
			public SkinVoiceType(string typeID)
			{
				this.TypeID = typeID;
				this.Index = MBAPI.IMBVoiceManager.GetVoiceTypeIndex(typeID);
			}

			// Token: 0x06003EE6 RID: 16102 RVA: 0x000F6136 File Offset: 0x000F4336
			public TextObject GetName()
			{
				return GameTexts.FindText("str_taunt_name", this.TypeID);
			}
		}

		// Token: 0x02000699 RID: 1689
		public static class VoiceType
		{
			// Token: 0x0400219E RID: 8606
			public static readonly SkinVoiceManager.SkinVoiceType Grunt = new SkinVoiceManager.SkinVoiceType("Grunt");

			// Token: 0x0400219F RID: 8607
			public static readonly SkinVoiceManager.SkinVoiceType Jump = new SkinVoiceManager.SkinVoiceType("Jump");

			// Token: 0x040021A0 RID: 8608
			public static readonly SkinVoiceManager.SkinVoiceType Yell = new SkinVoiceManager.SkinVoiceType("Yell");

			// Token: 0x040021A1 RID: 8609
			public static readonly SkinVoiceManager.SkinVoiceType Pain = new SkinVoiceManager.SkinVoiceType("Pain");

			// Token: 0x040021A2 RID: 8610
			public static readonly SkinVoiceManager.SkinVoiceType Death = new SkinVoiceManager.SkinVoiceType("Death");

			// Token: 0x040021A3 RID: 8611
			public static readonly SkinVoiceManager.SkinVoiceType Stun = new SkinVoiceManager.SkinVoiceType("Stun");

			// Token: 0x040021A4 RID: 8612
			public static readonly SkinVoiceManager.SkinVoiceType Fear = new SkinVoiceManager.SkinVoiceType("Fear");

			// Token: 0x040021A5 RID: 8613
			public static readonly SkinVoiceManager.SkinVoiceType Climb = new SkinVoiceManager.SkinVoiceType("Climb");

			// Token: 0x040021A6 RID: 8614
			public static readonly SkinVoiceManager.SkinVoiceType Focus = new SkinVoiceManager.SkinVoiceType("Focus");

			// Token: 0x040021A7 RID: 8615
			public static readonly SkinVoiceManager.SkinVoiceType Debacle = new SkinVoiceManager.SkinVoiceType("Debacle");

			// Token: 0x040021A8 RID: 8616
			public static readonly SkinVoiceManager.SkinVoiceType Victory = new SkinVoiceManager.SkinVoiceType("Victory");

			// Token: 0x040021A9 RID: 8617
			public static readonly SkinVoiceManager.SkinVoiceType HorseStop = new SkinVoiceManager.SkinVoiceType("HorseStop");

			// Token: 0x040021AA RID: 8618
			public static readonly SkinVoiceManager.SkinVoiceType HorseRally = new SkinVoiceManager.SkinVoiceType("HorseRally");

			// Token: 0x040021AB RID: 8619
			public static readonly SkinVoiceManager.SkinVoiceType Infantry = new SkinVoiceManager.SkinVoiceType("Infantry");

			// Token: 0x040021AC RID: 8620
			public static readonly SkinVoiceManager.SkinVoiceType Cavalry = new SkinVoiceManager.SkinVoiceType("Cavalry");

			// Token: 0x040021AD RID: 8621
			public static readonly SkinVoiceManager.SkinVoiceType Archers = new SkinVoiceManager.SkinVoiceType("Archers");

			// Token: 0x040021AE RID: 8622
			public static readonly SkinVoiceManager.SkinVoiceType HorseArchers = new SkinVoiceManager.SkinVoiceType("HorseArchers");

			// Token: 0x040021AF RID: 8623
			public static readonly SkinVoiceManager.SkinVoiceType Everyone = new SkinVoiceManager.SkinVoiceType("Everyone");

			// Token: 0x040021B0 RID: 8624
			public static readonly SkinVoiceManager.SkinVoiceType MixedFormation = new SkinVoiceManager.SkinVoiceType("Mixed");

			// Token: 0x040021B1 RID: 8625
			public static readonly SkinVoiceManager.SkinVoiceType Move = new SkinVoiceManager.SkinVoiceType("Move");

			// Token: 0x040021B2 RID: 8626
			public static readonly SkinVoiceManager.SkinVoiceType Follow = new SkinVoiceManager.SkinVoiceType("Follow");

			// Token: 0x040021B3 RID: 8627
			public static readonly SkinVoiceManager.SkinVoiceType Charge = new SkinVoiceManager.SkinVoiceType("Charge");

			// Token: 0x040021B4 RID: 8628
			public static readonly SkinVoiceManager.SkinVoiceType Advance = new SkinVoiceManager.SkinVoiceType("Advance");

			// Token: 0x040021B5 RID: 8629
			public static readonly SkinVoiceManager.SkinVoiceType FallBack = new SkinVoiceManager.SkinVoiceType("FallBack");

			// Token: 0x040021B6 RID: 8630
			public static readonly SkinVoiceManager.SkinVoiceType Stop = new SkinVoiceManager.SkinVoiceType("Stop");

			// Token: 0x040021B7 RID: 8631
			public static readonly SkinVoiceManager.SkinVoiceType Retreat = new SkinVoiceManager.SkinVoiceType("Retreat");

			// Token: 0x040021B8 RID: 8632
			public static readonly SkinVoiceManager.SkinVoiceType Mount = new SkinVoiceManager.SkinVoiceType("Mount");

			// Token: 0x040021B9 RID: 8633
			public static readonly SkinVoiceManager.SkinVoiceType Dismount = new SkinVoiceManager.SkinVoiceType("Dismount");

			// Token: 0x040021BA RID: 8634
			public static readonly SkinVoiceManager.SkinVoiceType FireAtWill = new SkinVoiceManager.SkinVoiceType("FireAtWill");

			// Token: 0x040021BB RID: 8635
			public static readonly SkinVoiceManager.SkinVoiceType HoldFire = new SkinVoiceManager.SkinVoiceType("HoldFire");

			// Token: 0x040021BC RID: 8636
			public static readonly SkinVoiceManager.SkinVoiceType PickSpears = new SkinVoiceManager.SkinVoiceType("PickSpears");

			// Token: 0x040021BD RID: 8637
			public static readonly SkinVoiceManager.SkinVoiceType PickDefault = new SkinVoiceManager.SkinVoiceType("PickDefault");

			// Token: 0x040021BE RID: 8638
			public static readonly SkinVoiceManager.SkinVoiceType FaceEnemy = new SkinVoiceManager.SkinVoiceType("FaceEnemy");

			// Token: 0x040021BF RID: 8639
			public static readonly SkinVoiceManager.SkinVoiceType FaceDirection = new SkinVoiceManager.SkinVoiceType("FaceDirection");

			// Token: 0x040021C0 RID: 8640
			public static readonly SkinVoiceManager.SkinVoiceType UseSiegeWeapon = new SkinVoiceManager.SkinVoiceType("UseSiegeWeapon");

			// Token: 0x040021C1 RID: 8641
			public static readonly SkinVoiceManager.SkinVoiceType UseLadders = new SkinVoiceManager.SkinVoiceType("UseLadders");

			// Token: 0x040021C2 RID: 8642
			public static readonly SkinVoiceManager.SkinVoiceType AttackGate = new SkinVoiceManager.SkinVoiceType("AttackGate");

			// Token: 0x040021C3 RID: 8643
			public static readonly SkinVoiceManager.SkinVoiceType CommandDelegate = new SkinVoiceManager.SkinVoiceType("CommandDelegate");

			// Token: 0x040021C4 RID: 8644
			public static readonly SkinVoiceManager.SkinVoiceType CommandUndelegate = new SkinVoiceManager.SkinVoiceType("CommandUndelegate");

			// Token: 0x040021C5 RID: 8645
			public static readonly SkinVoiceManager.SkinVoiceType FormLine = new SkinVoiceManager.SkinVoiceType("FormLine");

			// Token: 0x040021C6 RID: 8646
			public static readonly SkinVoiceManager.SkinVoiceType FormShieldWall = new SkinVoiceManager.SkinVoiceType("FormShieldWall");

			// Token: 0x040021C7 RID: 8647
			public static readonly SkinVoiceManager.SkinVoiceType FormLoose = new SkinVoiceManager.SkinVoiceType("FormLoose");

			// Token: 0x040021C8 RID: 8648
			public static readonly SkinVoiceManager.SkinVoiceType FormCircle = new SkinVoiceManager.SkinVoiceType("FormCircle");

			// Token: 0x040021C9 RID: 8649
			public static readonly SkinVoiceManager.SkinVoiceType FormSquare = new SkinVoiceManager.SkinVoiceType("FormSquare");

			// Token: 0x040021CA RID: 8650
			public static readonly SkinVoiceManager.SkinVoiceType FormSkein = new SkinVoiceManager.SkinVoiceType("FormSkein");

			// Token: 0x040021CB RID: 8651
			public static readonly SkinVoiceManager.SkinVoiceType FormColumn = new SkinVoiceManager.SkinVoiceType("FormColumn");

			// Token: 0x040021CC RID: 8652
			public static readonly SkinVoiceManager.SkinVoiceType FormScatter = new SkinVoiceManager.SkinVoiceType("FormScatter");

			// Token: 0x040021CD RID: 8653
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

			// Token: 0x040021CE RID: 8654
			public static readonly SkinVoiceManager.SkinVoiceType MpDefend = SkinVoiceManager.VoiceType.MpBarks[0];

			// Token: 0x040021CF RID: 8655
			public static readonly SkinVoiceManager.SkinVoiceType MpAttack = SkinVoiceManager.VoiceType.MpBarks[1];

			// Token: 0x040021D0 RID: 8656
			public static readonly SkinVoiceManager.SkinVoiceType MpHelp = SkinVoiceManager.VoiceType.MpBarks[2];

			// Token: 0x040021D1 RID: 8657
			public static readonly SkinVoiceManager.SkinVoiceType MpSpot = SkinVoiceManager.VoiceType.MpBarks[3];

			// Token: 0x040021D2 RID: 8658
			public static readonly SkinVoiceManager.SkinVoiceType MpThanks = SkinVoiceManager.VoiceType.MpBarks[4];

			// Token: 0x040021D3 RID: 8659
			public static readonly SkinVoiceManager.SkinVoiceType MpSorry = SkinVoiceManager.VoiceType.MpBarks[5];

			// Token: 0x040021D4 RID: 8660
			public static readonly SkinVoiceManager.SkinVoiceType MpAffirmative = SkinVoiceManager.VoiceType.MpBarks[6];

			// Token: 0x040021D5 RID: 8661
			public static readonly SkinVoiceManager.SkinVoiceType MpNegative = SkinVoiceManager.VoiceType.MpBarks[7];

			// Token: 0x040021D6 RID: 8662
			public static readonly SkinVoiceManager.SkinVoiceType MpRegroup = SkinVoiceManager.VoiceType.MpBarks[8];

			// Token: 0x040021D7 RID: 8663
			public static readonly SkinVoiceManager.SkinVoiceType Idle = new SkinVoiceManager.SkinVoiceType("Idle");

			// Token: 0x040021D8 RID: 8664
			public static readonly SkinVoiceManager.SkinVoiceType Neigh = new SkinVoiceManager.SkinVoiceType("Neigh");

			// Token: 0x040021D9 RID: 8665
			public static readonly SkinVoiceManager.SkinVoiceType Collide = new SkinVoiceManager.SkinVoiceType("Collide");
		}
	}
}
