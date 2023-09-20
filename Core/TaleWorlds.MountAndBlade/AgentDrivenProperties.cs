using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000EE RID: 238
	public class AgentDrivenProperties
	{
		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06000B34 RID: 2868 RVA: 0x00015A0F File Offset: 0x00013C0F
		internal float[] Values
		{
			get
			{
				return this._statValues;
			}
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x00015A17 File Offset: 0x00013C17
		public AgentDrivenProperties()
		{
			this._statValues = new float[84];
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x00015A2C File Offset: 0x00013C2C
		public float GetStat(DrivenProperty propertyEnum)
		{
			return this._statValues[(int)propertyEnum];
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x00015A36 File Offset: 0x00013C36
		public void SetStat(DrivenProperty propertyEnum, float value)
		{
			this._statValues[(int)propertyEnum] = value;
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06000B38 RID: 2872 RVA: 0x00015A41 File Offset: 0x00013C41
		// (set) Token: 0x06000B39 RID: 2873 RVA: 0x00015A4B File Offset: 0x00013C4B
		public float SwingSpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.SwingSpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.SwingSpeedMultiplier, value);
			}
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06000B3A RID: 2874 RVA: 0x00015A56 File Offset: 0x00013C56
		// (set) Token: 0x06000B3B RID: 2875 RVA: 0x00015A60 File Offset: 0x00013C60
		public float ThrustOrRangedReadySpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.ThrustOrRangedReadySpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.ThrustOrRangedReadySpeedMultiplier, value);
			}
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06000B3C RID: 2876 RVA: 0x00015A6B File Offset: 0x00013C6B
		// (set) Token: 0x06000B3D RID: 2877 RVA: 0x00015A75 File Offset: 0x00013C75
		public float HandlingMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.HandlingMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.HandlingMultiplier, value);
			}
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06000B3E RID: 2878 RVA: 0x00015A80 File Offset: 0x00013C80
		// (set) Token: 0x06000B3F RID: 2879 RVA: 0x00015A8A File Offset: 0x00013C8A
		public float ReloadSpeed
		{
			get
			{
				return this.GetStat(DrivenProperty.ReloadSpeed);
			}
			set
			{
				this.SetStat(DrivenProperty.ReloadSpeed, value);
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06000B40 RID: 2880 RVA: 0x00015A95 File Offset: 0x00013C95
		// (set) Token: 0x06000B41 RID: 2881 RVA: 0x00015A9F File Offset: 0x00013C9F
		public float MissileSpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.MissileSpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.MissileSpeedMultiplier, value);
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06000B42 RID: 2882 RVA: 0x00015AAA File Offset: 0x00013CAA
		// (set) Token: 0x06000B43 RID: 2883 RVA: 0x00015AB4 File Offset: 0x00013CB4
		public float WeaponInaccuracy
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponInaccuracy);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponInaccuracy, value);
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06000B44 RID: 2884 RVA: 0x00015ABF File Offset: 0x00013CBF
		// (set) Token: 0x06000B45 RID: 2885 RVA: 0x00015AC9 File Offset: 0x00013CC9
		public float WeaponMaxMovementAccuracyPenalty
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponWorstMobileAccuracyPenalty);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponWorstMobileAccuracyPenalty, value);
			}
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06000B46 RID: 2886 RVA: 0x00015AD4 File Offset: 0x00013CD4
		// (set) Token: 0x06000B47 RID: 2887 RVA: 0x00015ADE File Offset: 0x00013CDE
		public float WeaponMaxUnsteadyAccuracyPenalty
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponWorstUnsteadyAccuracyPenalty);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponWorstUnsteadyAccuracyPenalty, value);
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06000B48 RID: 2888 RVA: 0x00015AE9 File Offset: 0x00013CE9
		// (set) Token: 0x06000B49 RID: 2889 RVA: 0x00015AF3 File Offset: 0x00013CF3
		public float WeaponBestAccuracyWaitTime
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponBestAccuracyWaitTime);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponBestAccuracyWaitTime, value);
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06000B4A RID: 2890 RVA: 0x00015AFE File Offset: 0x00013CFE
		// (set) Token: 0x06000B4B RID: 2891 RVA: 0x00015B08 File Offset: 0x00013D08
		public float WeaponUnsteadyBeginTime
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponUnsteadyBeginTime);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponUnsteadyBeginTime, value);
			}
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06000B4C RID: 2892 RVA: 0x00015B13 File Offset: 0x00013D13
		// (set) Token: 0x06000B4D RID: 2893 RVA: 0x00015B1D File Offset: 0x00013D1D
		public float WeaponUnsteadyEndTime
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponUnsteadyEndTime);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponUnsteadyEndTime, value);
			}
		}

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06000B4E RID: 2894 RVA: 0x00015B28 File Offset: 0x00013D28
		// (set) Token: 0x06000B4F RID: 2895 RVA: 0x00015B32 File Offset: 0x00013D32
		public float WeaponRotationalAccuracyPenaltyInRadians
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponRotationalAccuracyPenaltyInRadians);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponRotationalAccuracyPenaltyInRadians, value);
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06000B50 RID: 2896 RVA: 0x00015B3D File Offset: 0x00013D3D
		// (set) Token: 0x06000B51 RID: 2897 RVA: 0x00015B47 File Offset: 0x00013D47
		public float ArmorEncumbrance
		{
			get
			{
				return this.GetStat(DrivenProperty.ArmorEncumbrance);
			}
			set
			{
				this.SetStat(DrivenProperty.ArmorEncumbrance, value);
			}
		}

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06000B52 RID: 2898 RVA: 0x00015B52 File Offset: 0x00013D52
		// (set) Token: 0x06000B53 RID: 2899 RVA: 0x00015B5C File Offset: 0x00013D5C
		public float WeaponsEncumbrance
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponsEncumbrance);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponsEncumbrance, value);
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06000B54 RID: 2900 RVA: 0x00015B67 File Offset: 0x00013D67
		// (set) Token: 0x06000B55 RID: 2901 RVA: 0x00015B71 File Offset: 0x00013D71
		public float ArmorHead
		{
			get
			{
				return this.GetStat(DrivenProperty.ArmorHead);
			}
			set
			{
				this.SetStat(DrivenProperty.ArmorHead, value);
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06000B56 RID: 2902 RVA: 0x00015B7C File Offset: 0x00013D7C
		// (set) Token: 0x06000B57 RID: 2903 RVA: 0x00015B86 File Offset: 0x00013D86
		public float ArmorTorso
		{
			get
			{
				return this.GetStat(DrivenProperty.ArmorTorso);
			}
			set
			{
				this.SetStat(DrivenProperty.ArmorTorso, value);
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06000B58 RID: 2904 RVA: 0x00015B91 File Offset: 0x00013D91
		// (set) Token: 0x06000B59 RID: 2905 RVA: 0x00015B9B File Offset: 0x00013D9B
		public float ArmorLegs
		{
			get
			{
				return this.GetStat(DrivenProperty.ArmorLegs);
			}
			set
			{
				this.SetStat(DrivenProperty.ArmorLegs, value);
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06000B5A RID: 2906 RVA: 0x00015BA6 File Offset: 0x00013DA6
		// (set) Token: 0x06000B5B RID: 2907 RVA: 0x00015BB0 File Offset: 0x00013DB0
		public float ArmorArms
		{
			get
			{
				return this.GetStat(DrivenProperty.ArmorArms);
			}
			set
			{
				this.SetStat(DrivenProperty.ArmorArms, value);
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06000B5C RID: 2908 RVA: 0x00015BBB File Offset: 0x00013DBB
		// (set) Token: 0x06000B5D RID: 2909 RVA: 0x00015BC5 File Offset: 0x00013DC5
		public float AttributeRiding
		{
			get
			{
				return this.GetStat(DrivenProperty.AttributeRiding);
			}
			set
			{
				this.SetStat(DrivenProperty.AttributeRiding, value);
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06000B5E RID: 2910 RVA: 0x00015BD0 File Offset: 0x00013DD0
		// (set) Token: 0x06000B5F RID: 2911 RVA: 0x00015BDA File Offset: 0x00013DDA
		public float AttributeShield
		{
			get
			{
				return this.GetStat(DrivenProperty.AttributeShield);
			}
			set
			{
				this.SetStat(DrivenProperty.AttributeShield, value);
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06000B60 RID: 2912 RVA: 0x00015BE5 File Offset: 0x00013DE5
		// (set) Token: 0x06000B61 RID: 2913 RVA: 0x00015BEF File Offset: 0x00013DEF
		public float AttributeShieldMissileCollisionBodySizeAdder
		{
			get
			{
				return this.GetStat(DrivenProperty.AttributeShieldMissileCollisionBodySizeAdder);
			}
			set
			{
				this.SetStat(DrivenProperty.AttributeShieldMissileCollisionBodySizeAdder, value);
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06000B62 RID: 2914 RVA: 0x00015BFA File Offset: 0x00013DFA
		// (set) Token: 0x06000B63 RID: 2915 RVA: 0x00015C04 File Offset: 0x00013E04
		public float ShieldBashStunDurationMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.ShieldBashStunDurationMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.ShieldBashStunDurationMultiplier, value);
			}
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06000B64 RID: 2916 RVA: 0x00015C0F File Offset: 0x00013E0F
		// (set) Token: 0x06000B65 RID: 2917 RVA: 0x00015C19 File Offset: 0x00013E19
		public float KickStunDurationMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.KickStunDurationMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.KickStunDurationMultiplier, value);
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06000B66 RID: 2918 RVA: 0x00015C24 File Offset: 0x00013E24
		// (set) Token: 0x06000B67 RID: 2919 RVA: 0x00015C2E File Offset: 0x00013E2E
		public float ReloadMovementPenaltyFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.ReloadMovementPenaltyFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.ReloadMovementPenaltyFactor, value);
			}
		}

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06000B68 RID: 2920 RVA: 0x00015C39 File Offset: 0x00013E39
		// (set) Token: 0x06000B69 RID: 2921 RVA: 0x00015C43 File Offset: 0x00013E43
		public float TopSpeedReachDuration
		{
			get
			{
				return this.GetStat(DrivenProperty.TopSpeedReachDuration);
			}
			set
			{
				this.SetStat(DrivenProperty.TopSpeedReachDuration, value);
			}
		}

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06000B6A RID: 2922 RVA: 0x00015C4E File Offset: 0x00013E4E
		// (set) Token: 0x06000B6B RID: 2923 RVA: 0x00015C58 File Offset: 0x00013E58
		public float MaxSpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.MaxSpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.MaxSpeedMultiplier, value);
			}
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06000B6C RID: 2924 RVA: 0x00015C63 File Offset: 0x00013E63
		// (set) Token: 0x06000B6D RID: 2925 RVA: 0x00015C6D File Offset: 0x00013E6D
		public float CombatMaxSpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.CombatMaxSpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.CombatMaxSpeedMultiplier, value);
			}
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06000B6E RID: 2926 RVA: 0x00015C78 File Offset: 0x00013E78
		// (set) Token: 0x06000B6F RID: 2927 RVA: 0x00015C82 File Offset: 0x00013E82
		public float AttributeHorseArchery
		{
			get
			{
				return this.GetStat(DrivenProperty.AttributeHorseArchery);
			}
			set
			{
				this.SetStat(DrivenProperty.AttributeHorseArchery, value);
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000B70 RID: 2928 RVA: 0x00015C8D File Offset: 0x00013E8D
		// (set) Token: 0x06000B71 RID: 2929 RVA: 0x00015C97 File Offset: 0x00013E97
		public float AttributeCourage
		{
			get
			{
				return this.GetStat(DrivenProperty.AttributeCourage);
			}
			set
			{
				this.SetStat(DrivenProperty.AttributeCourage, value);
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000B72 RID: 2930 RVA: 0x00015CA2 File Offset: 0x00013EA2
		// (set) Token: 0x06000B73 RID: 2931 RVA: 0x00015CAC File Offset: 0x00013EAC
		public float MountManeuver
		{
			get
			{
				return this.GetStat(DrivenProperty.MountManeuver);
			}
			set
			{
				this.SetStat(DrivenProperty.MountManeuver, value);
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06000B74 RID: 2932 RVA: 0x00015CB7 File Offset: 0x00013EB7
		// (set) Token: 0x06000B75 RID: 2933 RVA: 0x00015CC1 File Offset: 0x00013EC1
		public float MountSpeed
		{
			get
			{
				return this.GetStat(DrivenProperty.MountSpeed);
			}
			set
			{
				this.SetStat(DrivenProperty.MountSpeed, value);
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x00015CCC File Offset: 0x00013ECC
		// (set) Token: 0x06000B77 RID: 2935 RVA: 0x00015CD6 File Offset: 0x00013ED6
		public float MountDashAccelerationMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.MountDashAccelerationMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.MountDashAccelerationMultiplier, value);
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06000B78 RID: 2936 RVA: 0x00015CE1 File Offset: 0x00013EE1
		// (set) Token: 0x06000B79 RID: 2937 RVA: 0x00015CEB File Offset: 0x00013EEB
		public float MountChargeDamage
		{
			get
			{
				return this.GetStat(DrivenProperty.MountChargeDamage);
			}
			set
			{
				this.SetStat(DrivenProperty.MountChargeDamage, value);
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06000B7A RID: 2938 RVA: 0x00015CF6 File Offset: 0x00013EF6
		// (set) Token: 0x06000B7B RID: 2939 RVA: 0x00015D00 File Offset: 0x00013F00
		public float MountDifficulty
		{
			get
			{
				return this.GetStat(DrivenProperty.MountDifficulty);
			}
			set
			{
				this.SetStat(DrivenProperty.MountDifficulty, value);
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06000B7C RID: 2940 RVA: 0x00015D0B File Offset: 0x00013F0B
		// (set) Token: 0x06000B7D RID: 2941 RVA: 0x00015D15 File Offset: 0x00013F15
		public float BipedalRangedReadySpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.BipedalRangedReadySpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.BipedalRangedReadySpeedMultiplier, value);
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000B7E RID: 2942 RVA: 0x00015D20 File Offset: 0x00013F20
		// (set) Token: 0x06000B7F RID: 2943 RVA: 0x00015D2A File Offset: 0x00013F2A
		public float BipedalRangedReloadSpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.BipedalRangedReloadSpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.BipedalRangedReloadSpeedMultiplier, value);
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000B80 RID: 2944 RVA: 0x00015D35 File Offset: 0x00013F35
		// (set) Token: 0x06000B81 RID: 2945 RVA: 0x00015D3E File Offset: 0x00013F3E
		public float AiRangedHorsebackMissileRange
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRangedHorsebackMissileRange);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRangedHorsebackMissileRange, value);
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06000B82 RID: 2946 RVA: 0x00015D48 File Offset: 0x00013F48
		// (set) Token: 0x06000B83 RID: 2947 RVA: 0x00015D51 File Offset: 0x00013F51
		public float AiFacingMissileWatch
		{
			get
			{
				return this.GetStat(DrivenProperty.AiFacingMissileWatch);
			}
			set
			{
				this.SetStat(DrivenProperty.AiFacingMissileWatch, value);
			}
		}

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06000B84 RID: 2948 RVA: 0x00015D5B File Offset: 0x00013F5B
		// (set) Token: 0x06000B85 RID: 2949 RVA: 0x00015D64 File Offset: 0x00013F64
		public float AiFlyingMissileCheckRadius
		{
			get
			{
				return this.GetStat(DrivenProperty.AiFlyingMissileCheckRadius);
			}
			set
			{
				this.SetStat(DrivenProperty.AiFlyingMissileCheckRadius, value);
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06000B86 RID: 2950 RVA: 0x00015D6E File Offset: 0x00013F6E
		// (set) Token: 0x06000B87 RID: 2951 RVA: 0x00015D77 File Offset: 0x00013F77
		public float AiShootFreq
		{
			get
			{
				return this.GetStat(DrivenProperty.AiShootFreq);
			}
			set
			{
				this.SetStat(DrivenProperty.AiShootFreq, value);
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06000B88 RID: 2952 RVA: 0x00015D81 File Offset: 0x00013F81
		// (set) Token: 0x06000B89 RID: 2953 RVA: 0x00015D8A File Offset: 0x00013F8A
		public float AiWaitBeforeShootFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiWaitBeforeShootFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiWaitBeforeShootFactor, value);
			}
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06000B8A RID: 2954 RVA: 0x00015D94 File Offset: 0x00013F94
		// (set) Token: 0x06000B8B RID: 2955 RVA: 0x00015D9D File Offset: 0x00013F9D
		public float AIBlockOnDecideAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIBlockOnDecideAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIBlockOnDecideAbility, value);
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06000B8C RID: 2956 RVA: 0x00015DA7 File Offset: 0x00013FA7
		// (set) Token: 0x06000B8D RID: 2957 RVA: 0x00015DB0 File Offset: 0x00013FB0
		public float AIParryOnDecideAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIParryOnDecideAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIParryOnDecideAbility, value);
			}
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000B8E RID: 2958 RVA: 0x00015DBA File Offset: 0x00013FBA
		// (set) Token: 0x06000B8F RID: 2959 RVA: 0x00015DC3 File Offset: 0x00013FC3
		public float AiTryChamberAttackOnDecide
		{
			get
			{
				return this.GetStat(DrivenProperty.AiTryChamberAttackOnDecide);
			}
			set
			{
				this.SetStat(DrivenProperty.AiTryChamberAttackOnDecide, value);
			}
		}

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000B90 RID: 2960 RVA: 0x00015DCD File Offset: 0x00013FCD
		// (set) Token: 0x06000B91 RID: 2961 RVA: 0x00015DD6 File Offset: 0x00013FD6
		public float AIAttackOnParryChance
		{
			get
			{
				return this.GetStat(DrivenProperty.AIAttackOnParryChance);
			}
			set
			{
				this.SetStat(DrivenProperty.AIAttackOnParryChance, value);
			}
		}

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000B92 RID: 2962 RVA: 0x00015DE0 File Offset: 0x00013FE0
		// (set) Token: 0x06000B93 RID: 2963 RVA: 0x00015DEA File Offset: 0x00013FEA
		public float AiAttackOnParryTiming
		{
			get
			{
				return this.GetStat(DrivenProperty.AiAttackOnParryTiming);
			}
			set
			{
				this.SetStat(DrivenProperty.AiAttackOnParryTiming, value);
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06000B94 RID: 2964 RVA: 0x00015DF5 File Offset: 0x00013FF5
		// (set) Token: 0x06000B95 RID: 2965 RVA: 0x00015DFF File Offset: 0x00013FFF
		public float AIDecideOnAttackChance
		{
			get
			{
				return this.GetStat(DrivenProperty.AIDecideOnAttackChance);
			}
			set
			{
				this.SetStat(DrivenProperty.AIDecideOnAttackChance, value);
			}
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06000B96 RID: 2966 RVA: 0x00015E0A File Offset: 0x0001400A
		// (set) Token: 0x06000B97 RID: 2967 RVA: 0x00015E14 File Offset: 0x00014014
		public float AIParryOnAttackAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIParryOnAttackAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIParryOnAttackAbility, value);
			}
		}

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06000B98 RID: 2968 RVA: 0x00015E1F File Offset: 0x0001401F
		// (set) Token: 0x06000B99 RID: 2969 RVA: 0x00015E29 File Offset: 0x00014029
		public float AiKick
		{
			get
			{
				return this.GetStat(DrivenProperty.AiKick);
			}
			set
			{
				this.SetStat(DrivenProperty.AiKick, value);
			}
		}

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000B9A RID: 2970 RVA: 0x00015E34 File Offset: 0x00014034
		// (set) Token: 0x06000B9B RID: 2971 RVA: 0x00015E3E File Offset: 0x0001403E
		public float AiAttackCalculationMaxTimeFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiAttackCalculationMaxTimeFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiAttackCalculationMaxTimeFactor, value);
			}
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000B9C RID: 2972 RVA: 0x00015E49 File Offset: 0x00014049
		// (set) Token: 0x06000B9D RID: 2973 RVA: 0x00015E53 File Offset: 0x00014053
		public float AiDecideOnAttackWhenReceiveHitTiming
		{
			get
			{
				return this.GetStat(DrivenProperty.AiDecideOnAttackWhenReceiveHitTiming);
			}
			set
			{
				this.SetStat(DrivenProperty.AiDecideOnAttackWhenReceiveHitTiming, value);
			}
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000B9E RID: 2974 RVA: 0x00015E5E File Offset: 0x0001405E
		// (set) Token: 0x06000B9F RID: 2975 RVA: 0x00015E68 File Offset: 0x00014068
		public float AiDecideOnAttackContinueAction
		{
			get
			{
				return this.GetStat(DrivenProperty.AiDecideOnAttackContinueAction);
			}
			set
			{
				this.SetStat(DrivenProperty.AiDecideOnAttackContinueAction, value);
			}
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000BA0 RID: 2976 RVA: 0x00015E73 File Offset: 0x00014073
		// (set) Token: 0x06000BA1 RID: 2977 RVA: 0x00015E7D File Offset: 0x0001407D
		public float AiDecideOnAttackingContinue
		{
			get
			{
				return this.GetStat(DrivenProperty.AiDecideOnAttackingContinue);
			}
			set
			{
				this.SetStat(DrivenProperty.AiDecideOnAttackingContinue, value);
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06000BA2 RID: 2978 RVA: 0x00015E88 File Offset: 0x00014088
		// (set) Token: 0x06000BA3 RID: 2979 RVA: 0x00015E92 File Offset: 0x00014092
		public float AIParryOnAttackingContinueAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIParryOnAttackingContinueAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIParryOnAttackingContinueAbility, value);
			}
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06000BA4 RID: 2980 RVA: 0x00015E9D File Offset: 0x0001409D
		// (set) Token: 0x06000BA5 RID: 2981 RVA: 0x00015EA7 File Offset: 0x000140A7
		public float AIDecideOnRealizeEnemyBlockingAttackAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIDecideOnRealizeEnemyBlockingAttackAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIDecideOnRealizeEnemyBlockingAttackAbility, value);
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06000BA6 RID: 2982 RVA: 0x00015EB2 File Offset: 0x000140B2
		// (set) Token: 0x06000BA7 RID: 2983 RVA: 0x00015EBC File Offset: 0x000140BC
		public float AIRealizeBlockingFromIncorrectSideAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIRealizeBlockingFromIncorrectSideAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIRealizeBlockingFromIncorrectSideAbility, value);
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000BA8 RID: 2984 RVA: 0x00015EC7 File Offset: 0x000140C7
		// (set) Token: 0x06000BA9 RID: 2985 RVA: 0x00015ED1 File Offset: 0x000140D1
		public float AiAttackingShieldDefenseChance
		{
			get
			{
				return this.GetStat(DrivenProperty.AiAttackingShieldDefenseChance);
			}
			set
			{
				this.SetStat(DrivenProperty.AiAttackingShieldDefenseChance, value);
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000BAA RID: 2986 RVA: 0x00015EDC File Offset: 0x000140DC
		// (set) Token: 0x06000BAB RID: 2987 RVA: 0x00015EE6 File Offset: 0x000140E6
		public float AiAttackingShieldDefenseTimer
		{
			get
			{
				return this.GetStat(DrivenProperty.AiAttackingShieldDefenseTimer);
			}
			set
			{
				this.SetStat(DrivenProperty.AiAttackingShieldDefenseTimer, value);
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06000BAC RID: 2988 RVA: 0x00015EF1 File Offset: 0x000140F1
		// (set) Token: 0x06000BAD RID: 2989 RVA: 0x00015EFB File Offset: 0x000140FB
		public float AiCheckMovementIntervalFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiCheckMovementIntervalFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiCheckMovementIntervalFactor, value);
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06000BAE RID: 2990 RVA: 0x00015F06 File Offset: 0x00014106
		// (set) Token: 0x06000BAF RID: 2991 RVA: 0x00015F10 File Offset: 0x00014110
		public float AiMovementDelayFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiMovementDelayFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiMovementDelayFactor, value);
			}
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06000BB0 RID: 2992 RVA: 0x00015F1B File Offset: 0x0001411B
		// (set) Token: 0x06000BB1 RID: 2993 RVA: 0x00015F25 File Offset: 0x00014125
		public float AiParryDecisionChangeValue
		{
			get
			{
				return this.GetStat(DrivenProperty.AiParryDecisionChangeValue);
			}
			set
			{
				this.SetStat(DrivenProperty.AiParryDecisionChangeValue, value);
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06000BB2 RID: 2994 RVA: 0x00015F30 File Offset: 0x00014130
		// (set) Token: 0x06000BB3 RID: 2995 RVA: 0x00015F3A File Offset: 0x0001413A
		public float AiDefendWithShieldDecisionChanceValue
		{
			get
			{
				return this.GetStat(DrivenProperty.AiDefendWithShieldDecisionChanceValue);
			}
			set
			{
				this.SetStat(DrivenProperty.AiDefendWithShieldDecisionChanceValue, value);
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000BB4 RID: 2996 RVA: 0x00015F45 File Offset: 0x00014145
		// (set) Token: 0x06000BB5 RID: 2997 RVA: 0x00015F4F File Offset: 0x0001414F
		public float AiMoveEnemySideTimeValue
		{
			get
			{
				return this.GetStat(DrivenProperty.AiMoveEnemySideTimeValue);
			}
			set
			{
				this.SetStat(DrivenProperty.AiMoveEnemySideTimeValue, value);
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06000BB6 RID: 2998 RVA: 0x00015F5A File Offset: 0x0001415A
		// (set) Token: 0x06000BB7 RID: 2999 RVA: 0x00015F64 File Offset: 0x00014164
		public float AiMinimumDistanceToContinueFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiMinimumDistanceToContinueFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiMinimumDistanceToContinueFactor, value);
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06000BB8 RID: 3000 RVA: 0x00015F6F File Offset: 0x0001416F
		// (set) Token: 0x06000BB9 RID: 3001 RVA: 0x00015F79 File Offset: 0x00014179
		public float AiHearingDistanceFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiHearingDistanceFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiHearingDistanceFactor, value);
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06000BBA RID: 3002 RVA: 0x00015F84 File Offset: 0x00014184
		// (set) Token: 0x06000BBB RID: 3003 RVA: 0x00015F8E File Offset: 0x0001418E
		public float AiChargeHorsebackTargetDistFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiChargeHorsebackTargetDistFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiChargeHorsebackTargetDistFactor, value);
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000BBC RID: 3004 RVA: 0x00015F99 File Offset: 0x00014199
		// (set) Token: 0x06000BBD RID: 3005 RVA: 0x00015FA3 File Offset: 0x000141A3
		public float AiRangerLeadErrorMin
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRangerLeadErrorMin);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRangerLeadErrorMin, value);
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06000BBE RID: 3006 RVA: 0x00015FAE File Offset: 0x000141AE
		// (set) Token: 0x06000BBF RID: 3007 RVA: 0x00015FB8 File Offset: 0x000141B8
		public float AiRangerLeadErrorMax
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRangerLeadErrorMax);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRangerLeadErrorMax, value);
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06000BC0 RID: 3008 RVA: 0x00015FC3 File Offset: 0x000141C3
		// (set) Token: 0x06000BC1 RID: 3009 RVA: 0x00015FCD File Offset: 0x000141CD
		public float AiRangerVerticalErrorMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRangerVerticalErrorMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRangerVerticalErrorMultiplier, value);
			}
		}

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06000BC2 RID: 3010 RVA: 0x00015FD8 File Offset: 0x000141D8
		// (set) Token: 0x06000BC3 RID: 3011 RVA: 0x00015FE2 File Offset: 0x000141E2
		public float AiRangerHorizontalErrorMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRangerHorizontalErrorMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRangerHorizontalErrorMultiplier, value);
			}
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06000BC4 RID: 3012 RVA: 0x00015FED File Offset: 0x000141ED
		// (set) Token: 0x06000BC5 RID: 3013 RVA: 0x00015FF7 File Offset: 0x000141F7
		public float AIAttackOnDecideChance
		{
			get
			{
				return this.GetStat(DrivenProperty.AIAttackOnDecideChance);
			}
			set
			{
				this.SetStat(DrivenProperty.AIAttackOnDecideChance, value);
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06000BC6 RID: 3014 RVA: 0x00016002 File Offset: 0x00014202
		// (set) Token: 0x06000BC7 RID: 3015 RVA: 0x0001600C File Offset: 0x0001420C
		public float AiRaiseShieldDelayTimeBase
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRaiseShieldDelayTimeBase);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRaiseShieldDelayTimeBase, value);
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06000BC8 RID: 3016 RVA: 0x00016017 File Offset: 0x00014217
		// (set) Token: 0x06000BC9 RID: 3017 RVA: 0x00016021 File Offset: 0x00014221
		public float AiUseShieldAgainstEnemyMissileProbability
		{
			get
			{
				return this.GetStat(DrivenProperty.AiUseShieldAgainstEnemyMissileProbability);
			}
			set
			{
				this.SetStat(DrivenProperty.AiUseShieldAgainstEnemyMissileProbability, value);
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06000BCA RID: 3018 RVA: 0x0001602C File Offset: 0x0001422C
		// (set) Token: 0x06000BCB RID: 3019 RVA: 0x0001603B File Offset: 0x0001423B
		public int AiSpeciesIndex
		{
			get
			{
				return MathF.Round(this.GetStat(DrivenProperty.AiSpeciesIndex));
			}
			set
			{
				this.SetStat(DrivenProperty.AiSpeciesIndex, (float)value);
			}
		}

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06000BCC RID: 3020 RVA: 0x00016047 File Offset: 0x00014247
		// (set) Token: 0x06000BCD RID: 3021 RVA: 0x00016051 File Offset: 0x00014251
		public float AiRandomizedDefendDirectionChance
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRandomizedDefendDirectionChance);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRandomizedDefendDirectionChance, value);
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06000BCE RID: 3022 RVA: 0x0001605C File Offset: 0x0001425C
		// (set) Token: 0x06000BCF RID: 3023 RVA: 0x00016066 File Offset: 0x00014266
		public float AiShooterError
		{
			get
			{
				return this.GetStat(DrivenProperty.AiShooterError);
			}
			set
			{
				this.SetStat(DrivenProperty.AiShooterError, value);
			}
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06000BD0 RID: 3024 RVA: 0x00016071 File Offset: 0x00014271
		// (set) Token: 0x06000BD1 RID: 3025 RVA: 0x0001607B File Offset: 0x0001427B
		public float AISetNoAttackTimerAfterBeingHitAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AISetNoAttackTimerAfterBeingHitAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AISetNoAttackTimerAfterBeingHitAbility, value);
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06000BD2 RID: 3026 RVA: 0x00016086 File Offset: 0x00014286
		// (set) Token: 0x06000BD3 RID: 3027 RVA: 0x00016090 File Offset: 0x00014290
		public float AISetNoAttackTimerAfterBeingParriedAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AISetNoAttackTimerAfterBeingParriedAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AISetNoAttackTimerAfterBeingParriedAbility, value);
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06000BD4 RID: 3028 RVA: 0x0001609B File Offset: 0x0001429B
		// (set) Token: 0x06000BD5 RID: 3029 RVA: 0x000160A5 File Offset: 0x000142A5
		public float AISetNoDefendTimerAfterHittingAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AISetNoDefendTimerAfterHittingAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AISetNoDefendTimerAfterHittingAbility, value);
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06000BD6 RID: 3030 RVA: 0x000160B0 File Offset: 0x000142B0
		// (set) Token: 0x06000BD7 RID: 3031 RVA: 0x000160BA File Offset: 0x000142BA
		public float AISetNoDefendTimerAfterParryingAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AISetNoDefendTimerAfterParryingAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AISetNoDefendTimerAfterParryingAbility, value);
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06000BD8 RID: 3032 RVA: 0x000160C5 File Offset: 0x000142C5
		// (set) Token: 0x06000BD9 RID: 3033 RVA: 0x000160CF File Offset: 0x000142CF
		public float AIEstimateStunDurationPrecision
		{
			get
			{
				return this.GetStat(DrivenProperty.AIEstimateStunDurationPrecision);
			}
			set
			{
				this.SetStat(DrivenProperty.AIEstimateStunDurationPrecision, value);
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06000BDA RID: 3034 RVA: 0x000160DA File Offset: 0x000142DA
		// (set) Token: 0x06000BDB RID: 3035 RVA: 0x000160E4 File Offset: 0x000142E4
		public float AIHoldingReadyMaxDuration
		{
			get
			{
				return this.GetStat(DrivenProperty.AIHoldingReadyMaxDuration);
			}
			set
			{
				this.SetStat(DrivenProperty.AIHoldingReadyMaxDuration, value);
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06000BDC RID: 3036 RVA: 0x000160EF File Offset: 0x000142EF
		// (set) Token: 0x06000BDD RID: 3037 RVA: 0x000160F9 File Offset: 0x000142F9
		public float AIHoldingReadyVariationPercentage
		{
			get
			{
				return this.GetStat(DrivenProperty.AIHoldingReadyVariationPercentage);
			}
			set
			{
				this.SetStat(DrivenProperty.AIHoldingReadyVariationPercentage, value);
			}
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x00016104 File Offset: 0x00014304
		internal float[] InitializeDrivenProperties(Agent agent, Equipment spawnEquipment, AgentBuildData agentBuildData)
		{
			MissionGameModels.Current.AgentStatCalculateModel.InitializeAgentStats(agent, spawnEquipment, this, agentBuildData);
			return this._statValues;
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x0001611F File Offset: 0x0001431F
		internal float[] UpdateDrivenProperties(Agent agent)
		{
			MissionGameModels.Current.AgentStatCalculateModel.UpdateAgentStats(agent, this);
			return this._statValues;
		}

		// Token: 0x0400028C RID: 652
		private readonly float[] _statValues;
	}
}
