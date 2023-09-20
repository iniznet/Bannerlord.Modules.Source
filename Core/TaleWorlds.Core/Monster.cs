using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x020000B2 RID: 178
	public sealed class Monster : MBObjectBase
	{
		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000881 RID: 2177 RVA: 0x0001CCD9 File Offset: 0x0001AED9
		// (set) Token: 0x06000882 RID: 2178 RVA: 0x0001CCE1 File Offset: 0x0001AEE1
		public string BaseMonster { get; private set; }

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000883 RID: 2179 RVA: 0x0001CCEA File Offset: 0x0001AEEA
		// (set) Token: 0x06000884 RID: 2180 RVA: 0x0001CCF2 File Offset: 0x0001AEF2
		public float BodyCapsuleRadius { get; private set; }

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06000885 RID: 2181 RVA: 0x0001CCFB File Offset: 0x0001AEFB
		// (set) Token: 0x06000886 RID: 2182 RVA: 0x0001CD03 File Offset: 0x0001AF03
		public Vec3 BodyCapsulePoint1 { get; private set; }

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06000887 RID: 2183 RVA: 0x0001CD0C File Offset: 0x0001AF0C
		// (set) Token: 0x06000888 RID: 2184 RVA: 0x0001CD14 File Offset: 0x0001AF14
		public Vec3 BodyCapsulePoint2 { get; private set; }

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06000889 RID: 2185 RVA: 0x0001CD1D File Offset: 0x0001AF1D
		// (set) Token: 0x0600088A RID: 2186 RVA: 0x0001CD25 File Offset: 0x0001AF25
		public float CrouchedBodyCapsuleRadius { get; private set; }

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x0600088B RID: 2187 RVA: 0x0001CD2E File Offset: 0x0001AF2E
		// (set) Token: 0x0600088C RID: 2188 RVA: 0x0001CD36 File Offset: 0x0001AF36
		public Vec3 CrouchedBodyCapsulePoint1 { get; private set; }

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x0600088D RID: 2189 RVA: 0x0001CD3F File Offset: 0x0001AF3F
		// (set) Token: 0x0600088E RID: 2190 RVA: 0x0001CD47 File Offset: 0x0001AF47
		public Vec3 CrouchedBodyCapsulePoint2 { get; private set; }

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x0600088F RID: 2191 RVA: 0x0001CD50 File Offset: 0x0001AF50
		// (set) Token: 0x06000890 RID: 2192 RVA: 0x0001CD58 File Offset: 0x0001AF58
		public AgentFlag Flags { get; private set; }

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000891 RID: 2193 RVA: 0x0001CD61 File Offset: 0x0001AF61
		// (set) Token: 0x06000892 RID: 2194 RVA: 0x0001CD69 File Offset: 0x0001AF69
		public int Weight { get; private set; }

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000893 RID: 2195 RVA: 0x0001CD72 File Offset: 0x0001AF72
		// (set) Token: 0x06000894 RID: 2196 RVA: 0x0001CD7A File Offset: 0x0001AF7A
		public int HitPoints { get; private set; }

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000895 RID: 2197 RVA: 0x0001CD83 File Offset: 0x0001AF83
		// (set) Token: 0x06000896 RID: 2198 RVA: 0x0001CD8B File Offset: 0x0001AF8B
		public string ActionSetCode { get; private set; }

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06000897 RID: 2199 RVA: 0x0001CD94 File Offset: 0x0001AF94
		// (set) Token: 0x06000898 RID: 2200 RVA: 0x0001CD9C File Offset: 0x0001AF9C
		public string FemaleActionSetCode { get; private set; }

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06000899 RID: 2201 RVA: 0x0001CDA5 File Offset: 0x0001AFA5
		// (set) Token: 0x0600089A RID: 2202 RVA: 0x0001CDAD File Offset: 0x0001AFAD
		public int NumPaces { get; private set; }

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x0600089B RID: 2203 RVA: 0x0001CDB6 File Offset: 0x0001AFB6
		// (set) Token: 0x0600089C RID: 2204 RVA: 0x0001CDBE File Offset: 0x0001AFBE
		public string MonsterUsage { get; private set; }

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x0600089D RID: 2205 RVA: 0x0001CDC7 File Offset: 0x0001AFC7
		// (set) Token: 0x0600089E RID: 2206 RVA: 0x0001CDCF File Offset: 0x0001AFCF
		public float WalkingSpeedLimit { get; private set; }

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x0600089F RID: 2207 RVA: 0x0001CDD8 File Offset: 0x0001AFD8
		// (set) Token: 0x060008A0 RID: 2208 RVA: 0x0001CDE0 File Offset: 0x0001AFE0
		public float CrouchWalkingSpeedLimit { get; private set; }

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x060008A1 RID: 2209 RVA: 0x0001CDE9 File Offset: 0x0001AFE9
		// (set) Token: 0x060008A2 RID: 2210 RVA: 0x0001CDF1 File Offset: 0x0001AFF1
		public float JumpAcceleration { get; private set; }

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x060008A3 RID: 2211 RVA: 0x0001CDFA File Offset: 0x0001AFFA
		// (set) Token: 0x060008A4 RID: 2212 RVA: 0x0001CE02 File Offset: 0x0001B002
		public float AbsorbedDamageRatio { get; private set; }

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x060008A5 RID: 2213 RVA: 0x0001CE0B File Offset: 0x0001B00B
		// (set) Token: 0x060008A6 RID: 2214 RVA: 0x0001CE13 File Offset: 0x0001B013
		public string SoundAndCollisionInfoClassName { get; private set; }

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x060008A7 RID: 2215 RVA: 0x0001CE1C File Offset: 0x0001B01C
		// (set) Token: 0x060008A8 RID: 2216 RVA: 0x0001CE24 File Offset: 0x0001B024
		public float RiderCameraHeightAdder { get; private set; }

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x0001CE2D File Offset: 0x0001B02D
		// (set) Token: 0x060008AA RID: 2218 RVA: 0x0001CE35 File Offset: 0x0001B035
		public float RiderBodyCapsuleHeightAdder { get; private set; }

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x060008AB RID: 2219 RVA: 0x0001CE3E File Offset: 0x0001B03E
		// (set) Token: 0x060008AC RID: 2220 RVA: 0x0001CE46 File Offset: 0x0001B046
		public float RiderBodyCapsuleForwardAdder { get; private set; }

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x060008AD RID: 2221 RVA: 0x0001CE4F File Offset: 0x0001B04F
		// (set) Token: 0x060008AE RID: 2222 RVA: 0x0001CE57 File Offset: 0x0001B057
		public float StandingEyeHeight { get; private set; }

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x060008AF RID: 2223 RVA: 0x0001CE60 File Offset: 0x0001B060
		// (set) Token: 0x060008B0 RID: 2224 RVA: 0x0001CE68 File Offset: 0x0001B068
		public float CrouchEyeHeight { get; private set; }

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x060008B1 RID: 2225 RVA: 0x0001CE71 File Offset: 0x0001B071
		// (set) Token: 0x060008B2 RID: 2226 RVA: 0x0001CE79 File Offset: 0x0001B079
		public float MountedEyeHeight { get; private set; }

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x060008B3 RID: 2227 RVA: 0x0001CE82 File Offset: 0x0001B082
		// (set) Token: 0x060008B4 RID: 2228 RVA: 0x0001CE8A File Offset: 0x0001B08A
		public float RiderEyeHeightAdder { get; private set; }

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x060008B5 RID: 2229 RVA: 0x0001CE93 File Offset: 0x0001B093
		// (set) Token: 0x060008B6 RID: 2230 RVA: 0x0001CE9B File Offset: 0x0001B09B
		public Vec3 EyeOffsetWrtHead { get; private set; }

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x060008B7 RID: 2231 RVA: 0x0001CEA4 File Offset: 0x0001B0A4
		// (set) Token: 0x060008B8 RID: 2232 RVA: 0x0001CEAC File Offset: 0x0001B0AC
		public Vec3 FirstPersonCameraOffsetWrtHead { get; private set; }

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x060008B9 RID: 2233 RVA: 0x0001CEB5 File Offset: 0x0001B0B5
		// (set) Token: 0x060008BA RID: 2234 RVA: 0x0001CEBD File Offset: 0x0001B0BD
		public float ArmLength { get; private set; }

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x060008BB RID: 2235 RVA: 0x0001CEC6 File Offset: 0x0001B0C6
		// (set) Token: 0x060008BC RID: 2236 RVA: 0x0001CECE File Offset: 0x0001B0CE
		public float ArmWeight { get; private set; }

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x060008BD RID: 2237 RVA: 0x0001CED7 File Offset: 0x0001B0D7
		// (set) Token: 0x060008BE RID: 2238 RVA: 0x0001CEDF File Offset: 0x0001B0DF
		public float JumpSpeedLimit { get; private set; }

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x060008BF RID: 2239 RVA: 0x0001CEE8 File Offset: 0x0001B0E8
		// (set) Token: 0x060008C0 RID: 2240 RVA: 0x0001CEF0 File Offset: 0x0001B0F0
		public float RelativeSpeedLimitForCharge { get; private set; }

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x060008C1 RID: 2241 RVA: 0x0001CEF9 File Offset: 0x0001B0F9
		// (set) Token: 0x060008C2 RID: 2242 RVA: 0x0001CF01 File Offset: 0x0001B101
		public int FamilyType { get; private set; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x060008C3 RID: 2243 RVA: 0x0001CF0A File Offset: 0x0001B10A
		// (set) Token: 0x060008C4 RID: 2244 RVA: 0x0001CF12 File Offset: 0x0001B112
		public sbyte[] IndicesOfRagdollBonesToCheckForCorpses { get; private set; }

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x060008C5 RID: 2245 RVA: 0x0001CF1B File Offset: 0x0001B11B
		// (set) Token: 0x060008C6 RID: 2246 RVA: 0x0001CF23 File Offset: 0x0001B123
		public sbyte[] RagdollFallSoundBoneIndices { get; private set; }

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x060008C7 RID: 2247 RVA: 0x0001CF2C File Offset: 0x0001B12C
		// (set) Token: 0x060008C8 RID: 2248 RVA: 0x0001CF34 File Offset: 0x0001B134
		public sbyte HeadLookDirectionBoneIndex { get; private set; }

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x060008C9 RID: 2249 RVA: 0x0001CF3D File Offset: 0x0001B13D
		// (set) Token: 0x060008CA RID: 2250 RVA: 0x0001CF45 File Offset: 0x0001B145
		public sbyte SpineLowerBoneIndex { get; private set; }

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x060008CB RID: 2251 RVA: 0x0001CF4E File Offset: 0x0001B14E
		// (set) Token: 0x060008CC RID: 2252 RVA: 0x0001CF56 File Offset: 0x0001B156
		public sbyte SpineUpperBoneIndex { get; private set; }

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x060008CD RID: 2253 RVA: 0x0001CF5F File Offset: 0x0001B15F
		// (set) Token: 0x060008CE RID: 2254 RVA: 0x0001CF67 File Offset: 0x0001B167
		public sbyte ThoraxLookDirectionBoneIndex { get; private set; }

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x060008CF RID: 2255 RVA: 0x0001CF70 File Offset: 0x0001B170
		// (set) Token: 0x060008D0 RID: 2256 RVA: 0x0001CF78 File Offset: 0x0001B178
		public sbyte NeckRootBoneIndex { get; private set; }

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x060008D1 RID: 2257 RVA: 0x0001CF81 File Offset: 0x0001B181
		// (set) Token: 0x060008D2 RID: 2258 RVA: 0x0001CF89 File Offset: 0x0001B189
		public sbyte PelvisBoneIndex { get; private set; }

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x060008D3 RID: 2259 RVA: 0x0001CF92 File Offset: 0x0001B192
		// (set) Token: 0x060008D4 RID: 2260 RVA: 0x0001CF9A File Offset: 0x0001B19A
		public sbyte RightUpperArmBoneIndex { get; private set; }

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x060008D5 RID: 2261 RVA: 0x0001CFA3 File Offset: 0x0001B1A3
		// (set) Token: 0x060008D6 RID: 2262 RVA: 0x0001CFAB File Offset: 0x0001B1AB
		public sbyte LeftUpperArmBoneIndex { get; private set; }

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x060008D7 RID: 2263 RVA: 0x0001CFB4 File Offset: 0x0001B1B4
		// (set) Token: 0x060008D8 RID: 2264 RVA: 0x0001CFBC File Offset: 0x0001B1BC
		public sbyte FallBlowDamageBoneIndex { get; private set; }

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x060008D9 RID: 2265 RVA: 0x0001CFC5 File Offset: 0x0001B1C5
		// (set) Token: 0x060008DA RID: 2266 RVA: 0x0001CFCD File Offset: 0x0001B1CD
		public sbyte TerrainDecalBone0Index { get; private set; }

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x060008DB RID: 2267 RVA: 0x0001CFD6 File Offset: 0x0001B1D6
		// (set) Token: 0x060008DC RID: 2268 RVA: 0x0001CFDE File Offset: 0x0001B1DE
		public sbyte TerrainDecalBone1Index { get; private set; }

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x060008DD RID: 2269 RVA: 0x0001CFE7 File Offset: 0x0001B1E7
		// (set) Token: 0x060008DE RID: 2270 RVA: 0x0001CFEF File Offset: 0x0001B1EF
		public sbyte[] RagdollStationaryCheckBoneIndices { get; private set; }

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x060008DF RID: 2271 RVA: 0x0001CFF8 File Offset: 0x0001B1F8
		// (set) Token: 0x060008E0 RID: 2272 RVA: 0x0001D000 File Offset: 0x0001B200
		public sbyte[] MoveAdderBoneIndices { get; private set; }

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x060008E1 RID: 2273 RVA: 0x0001D009 File Offset: 0x0001B209
		// (set) Token: 0x060008E2 RID: 2274 RVA: 0x0001D011 File Offset: 0x0001B211
		public sbyte[] SplashDecalBoneIndices { get; private set; }

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x060008E3 RID: 2275 RVA: 0x0001D01A File Offset: 0x0001B21A
		// (set) Token: 0x060008E4 RID: 2276 RVA: 0x0001D022 File Offset: 0x0001B222
		public sbyte[] BloodBurstBoneIndices { get; private set; }

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x060008E5 RID: 2277 RVA: 0x0001D02B File Offset: 0x0001B22B
		// (set) Token: 0x060008E6 RID: 2278 RVA: 0x0001D033 File Offset: 0x0001B233
		public sbyte MainHandBoneIndex { get; private set; }

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x060008E7 RID: 2279 RVA: 0x0001D03C File Offset: 0x0001B23C
		// (set) Token: 0x060008E8 RID: 2280 RVA: 0x0001D044 File Offset: 0x0001B244
		public sbyte OffHandBoneIndex { get; private set; }

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x060008E9 RID: 2281 RVA: 0x0001D04D File Offset: 0x0001B24D
		// (set) Token: 0x060008EA RID: 2282 RVA: 0x0001D055 File Offset: 0x0001B255
		public sbyte MainHandItemBoneIndex { get; private set; }

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x060008EB RID: 2283 RVA: 0x0001D05E File Offset: 0x0001B25E
		// (set) Token: 0x060008EC RID: 2284 RVA: 0x0001D066 File Offset: 0x0001B266
		public sbyte OffHandItemBoneIndex { get; private set; }

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x060008ED RID: 2285 RVA: 0x0001D06F File Offset: 0x0001B26F
		// (set) Token: 0x060008EE RID: 2286 RVA: 0x0001D077 File Offset: 0x0001B277
		public sbyte MainHandItemSecondaryBoneIndex { get; private set; }

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060008EF RID: 2287 RVA: 0x0001D080 File Offset: 0x0001B280
		// (set) Token: 0x060008F0 RID: 2288 RVA: 0x0001D088 File Offset: 0x0001B288
		public sbyte OffHandItemSecondaryBoneIndex { get; private set; }

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060008F1 RID: 2289 RVA: 0x0001D091 File Offset: 0x0001B291
		// (set) Token: 0x060008F2 RID: 2290 RVA: 0x0001D099 File Offset: 0x0001B299
		public sbyte OffHandShoulderBoneIndex { get; private set; }

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x060008F3 RID: 2291 RVA: 0x0001D0A2 File Offset: 0x0001B2A2
		// (set) Token: 0x060008F4 RID: 2292 RVA: 0x0001D0AA File Offset: 0x0001B2AA
		public sbyte HandNumBonesForIk { get; private set; }

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x060008F5 RID: 2293 RVA: 0x0001D0B3 File Offset: 0x0001B2B3
		// (set) Token: 0x060008F6 RID: 2294 RVA: 0x0001D0BB File Offset: 0x0001B2BB
		public sbyte PrimaryFootBoneIndex { get; private set; }

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x060008F7 RID: 2295 RVA: 0x0001D0C4 File Offset: 0x0001B2C4
		// (set) Token: 0x060008F8 RID: 2296 RVA: 0x0001D0CC File Offset: 0x0001B2CC
		public sbyte SecondaryFootBoneIndex { get; private set; }

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x060008F9 RID: 2297 RVA: 0x0001D0D5 File Offset: 0x0001B2D5
		// (set) Token: 0x060008FA RID: 2298 RVA: 0x0001D0DD File Offset: 0x0001B2DD
		public sbyte RightFootIkEndEffectorBoneIndex { get; private set; }

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x060008FB RID: 2299 RVA: 0x0001D0E6 File Offset: 0x0001B2E6
		// (set) Token: 0x060008FC RID: 2300 RVA: 0x0001D0EE File Offset: 0x0001B2EE
		public sbyte LeftFootIkEndEffectorBoneIndex { get; private set; }

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x060008FD RID: 2301 RVA: 0x0001D0F7 File Offset: 0x0001B2F7
		// (set) Token: 0x060008FE RID: 2302 RVA: 0x0001D0FF File Offset: 0x0001B2FF
		public sbyte RightFootIkTipBoneIndex { get; private set; }

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x060008FF RID: 2303 RVA: 0x0001D108 File Offset: 0x0001B308
		// (set) Token: 0x06000900 RID: 2304 RVA: 0x0001D110 File Offset: 0x0001B310
		public sbyte LeftFootIkTipBoneIndex { get; private set; }

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06000901 RID: 2305 RVA: 0x0001D119 File Offset: 0x0001B319
		// (set) Token: 0x06000902 RID: 2306 RVA: 0x0001D121 File Offset: 0x0001B321
		public sbyte FootNumBonesForIk { get; private set; }

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06000903 RID: 2307 RVA: 0x0001D12A File Offset: 0x0001B32A
		// (set) Token: 0x06000904 RID: 2308 RVA: 0x0001D132 File Offset: 0x0001B332
		public Vec3 ReinHandleLeftLocalPosition { get; private set; }

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06000905 RID: 2309 RVA: 0x0001D13B File Offset: 0x0001B33B
		// (set) Token: 0x06000906 RID: 2310 RVA: 0x0001D143 File Offset: 0x0001B343
		public Vec3 ReinHandleRightLocalPosition { get; private set; }

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06000907 RID: 2311 RVA: 0x0001D14C File Offset: 0x0001B34C
		// (set) Token: 0x06000908 RID: 2312 RVA: 0x0001D154 File Offset: 0x0001B354
		public string ReinSkeleton { get; private set; }

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06000909 RID: 2313 RVA: 0x0001D15D File Offset: 0x0001B35D
		// (set) Token: 0x0600090A RID: 2314 RVA: 0x0001D165 File Offset: 0x0001B365
		public string ReinCollisionBody { get; private set; }

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x0600090B RID: 2315 RVA: 0x0001D16E File Offset: 0x0001B36E
		// (set) Token: 0x0600090C RID: 2316 RVA: 0x0001D176 File Offset: 0x0001B376
		public sbyte FrontBoneToDetectGroundSlopeIndex { get; private set; }

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x0600090D RID: 2317 RVA: 0x0001D17F File Offset: 0x0001B37F
		// (set) Token: 0x0600090E RID: 2318 RVA: 0x0001D187 File Offset: 0x0001B387
		public sbyte BackBoneToDetectGroundSlopeIndex { get; private set; }

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x0600090F RID: 2319 RVA: 0x0001D190 File Offset: 0x0001B390
		// (set) Token: 0x06000910 RID: 2320 RVA: 0x0001D198 File Offset: 0x0001B398
		public sbyte[] BoneIndicesToModifyOnSlopingGround { get; private set; }

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06000911 RID: 2321 RVA: 0x0001D1A1 File Offset: 0x0001B3A1
		// (set) Token: 0x06000912 RID: 2322 RVA: 0x0001D1A9 File Offset: 0x0001B3A9
		public sbyte BodyRotationReferenceBoneIndex { get; private set; }

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06000913 RID: 2323 RVA: 0x0001D1B2 File Offset: 0x0001B3B2
		// (set) Token: 0x06000914 RID: 2324 RVA: 0x0001D1BA File Offset: 0x0001B3BA
		public sbyte RiderSitBoneIndex { get; private set; }

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06000915 RID: 2325 RVA: 0x0001D1C3 File Offset: 0x0001B3C3
		// (set) Token: 0x06000916 RID: 2326 RVA: 0x0001D1CB File Offset: 0x0001B3CB
		public sbyte ReinHandleBoneIndex { get; private set; }

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06000917 RID: 2327 RVA: 0x0001D1D4 File Offset: 0x0001B3D4
		// (set) Token: 0x06000918 RID: 2328 RVA: 0x0001D1DC File Offset: 0x0001B3DC
		public sbyte ReinCollision1BoneIndex { get; private set; }

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06000919 RID: 2329 RVA: 0x0001D1E5 File Offset: 0x0001B3E5
		// (set) Token: 0x0600091A RID: 2330 RVA: 0x0001D1ED File Offset: 0x0001B3ED
		public sbyte ReinCollision2BoneIndex { get; private set; }

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x0600091B RID: 2331 RVA: 0x0001D1F6 File Offset: 0x0001B3F6
		// (set) Token: 0x0600091C RID: 2332 RVA: 0x0001D1FE File Offset: 0x0001B3FE
		public sbyte ReinHeadBoneIndex { get; private set; }

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x0600091D RID: 2333 RVA: 0x0001D207 File Offset: 0x0001B407
		// (set) Token: 0x0600091E RID: 2334 RVA: 0x0001D20F File Offset: 0x0001B40F
		public sbyte ReinHeadRightAttachmentBoneIndex { get; private set; }

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x0600091F RID: 2335 RVA: 0x0001D218 File Offset: 0x0001B418
		// (set) Token: 0x06000920 RID: 2336 RVA: 0x0001D220 File Offset: 0x0001B420
		public sbyte ReinHeadLeftAttachmentBoneIndex { get; private set; }

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06000921 RID: 2337 RVA: 0x0001D229 File Offset: 0x0001B429
		// (set) Token: 0x06000922 RID: 2338 RVA: 0x0001D231 File Offset: 0x0001B431
		public sbyte ReinRightHandBoneIndex { get; private set; }

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06000923 RID: 2339 RVA: 0x0001D23A File Offset: 0x0001B43A
		// (set) Token: 0x06000924 RID: 2340 RVA: 0x0001D242 File Offset: 0x0001B442
		public sbyte ReinLeftHandBoneIndex { get; private set; }

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06000925 RID: 2341 RVA: 0x0001D24C File Offset: 0x0001B44C
		[CachedData]
		public IMonsterMissionData MonsterMissionData
		{
			get
			{
				IMonsterMissionData monsterMissionData;
				if ((monsterMissionData = this._monsterMissionData) == null)
				{
					monsterMissionData = (this._monsterMissionData = Game.Current.MonsterMissionDataCreator.CreateMonsterMissionData(this));
				}
				return monsterMissionData;
			}
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x0001D27C File Offset: 0x0001B47C
		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			bool flag = false;
			XmlAttribute xmlAttribute = node.Attributes["base_monster"];
			List<sbyte> list;
			List<sbyte> list2;
			List<sbyte> list3;
			List<sbyte> list4;
			List<sbyte> list5;
			List<sbyte> list6;
			List<sbyte> list7;
			if (xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value))
			{
				flag = true;
				this.BaseMonster = xmlAttribute.Value;
				Monster @object = objectManager.GetObject<Monster>(this.BaseMonster);
				if (!string.IsNullOrEmpty(@object.BaseMonster))
				{
					this.BaseMonster = @object.BaseMonster;
				}
				this.BodyCapsuleRadius = @object.BodyCapsuleRadius;
				this.BodyCapsulePoint1 = @object.BodyCapsulePoint1;
				this.BodyCapsulePoint2 = @object.BodyCapsulePoint2;
				this.CrouchedBodyCapsuleRadius = @object.CrouchedBodyCapsuleRadius;
				this.CrouchedBodyCapsulePoint1 = @object.CrouchedBodyCapsulePoint1;
				this.CrouchedBodyCapsulePoint2 = @object.CrouchedBodyCapsulePoint2;
				this.Flags = @object.Flags;
				this.Weight = @object.Weight;
				this.HitPoints = @object.HitPoints;
				this.ActionSetCode = @object.ActionSetCode;
				this.FemaleActionSetCode = @object.FemaleActionSetCode;
				this.MonsterUsage = @object.MonsterUsage;
				this.NumPaces = @object.NumPaces;
				this.WalkingSpeedLimit = @object.WalkingSpeedLimit;
				this.CrouchWalkingSpeedLimit = @object.CrouchWalkingSpeedLimit;
				this.JumpAcceleration = @object.JumpAcceleration;
				this.AbsorbedDamageRatio = @object.AbsorbedDamageRatio;
				this.SoundAndCollisionInfoClassName = @object.SoundAndCollisionInfoClassName;
				this.RiderCameraHeightAdder = @object.RiderCameraHeightAdder;
				this.RiderBodyCapsuleHeightAdder = @object.RiderBodyCapsuleHeightAdder;
				this.RiderBodyCapsuleForwardAdder = @object.RiderBodyCapsuleForwardAdder;
				this.StandingEyeHeight = @object.StandingEyeHeight;
				this.CrouchEyeHeight = @object.CrouchEyeHeight;
				this.MountedEyeHeight = @object.MountedEyeHeight;
				this.RiderEyeHeightAdder = @object.RiderEyeHeightAdder;
				this.EyeOffsetWrtHead = @object.EyeOffsetWrtHead;
				this.FirstPersonCameraOffsetWrtHead = @object.FirstPersonCameraOffsetWrtHead;
				this.ArmLength = @object.ArmLength;
				this.ArmWeight = @object.ArmWeight;
				this.JumpSpeedLimit = @object.JumpSpeedLimit;
				this.RelativeSpeedLimitForCharge = @object.RelativeSpeedLimitForCharge;
				this.FamilyType = @object.FamilyType;
				list = new List<sbyte>(@object.IndicesOfRagdollBonesToCheckForCorpses);
				list2 = new List<sbyte>(@object.RagdollFallSoundBoneIndices);
				this.HeadLookDirectionBoneIndex = @object.HeadLookDirectionBoneIndex;
				this.SpineLowerBoneIndex = @object.SpineLowerBoneIndex;
				this.SpineUpperBoneIndex = @object.SpineUpperBoneIndex;
				this.ThoraxLookDirectionBoneIndex = @object.ThoraxLookDirectionBoneIndex;
				this.NeckRootBoneIndex = @object.NeckRootBoneIndex;
				this.PelvisBoneIndex = @object.PelvisBoneIndex;
				this.RightUpperArmBoneIndex = @object.RightUpperArmBoneIndex;
				this.LeftUpperArmBoneIndex = @object.LeftUpperArmBoneIndex;
				this.FallBlowDamageBoneIndex = @object.FallBlowDamageBoneIndex;
				this.TerrainDecalBone0Index = @object.TerrainDecalBone0Index;
				this.TerrainDecalBone1Index = @object.TerrainDecalBone1Index;
				list3 = new List<sbyte>(@object.RagdollStationaryCheckBoneIndices);
				list4 = new List<sbyte>(@object.MoveAdderBoneIndices);
				list5 = new List<sbyte>(@object.SplashDecalBoneIndices);
				list6 = new List<sbyte>(@object.BloodBurstBoneIndices);
				this.MainHandBoneIndex = @object.MainHandBoneIndex;
				this.OffHandBoneIndex = @object.OffHandBoneIndex;
				this.MainHandItemBoneIndex = @object.MainHandItemBoneIndex;
				this.OffHandItemBoneIndex = @object.OffHandItemBoneIndex;
				this.MainHandItemSecondaryBoneIndex = @object.MainHandItemSecondaryBoneIndex;
				this.OffHandItemSecondaryBoneIndex = @object.OffHandItemSecondaryBoneIndex;
				this.OffHandShoulderBoneIndex = @object.OffHandShoulderBoneIndex;
				this.HandNumBonesForIk = @object.HandNumBonesForIk;
				this.PrimaryFootBoneIndex = @object.PrimaryFootBoneIndex;
				this.SecondaryFootBoneIndex = @object.SecondaryFootBoneIndex;
				this.RightFootIkEndEffectorBoneIndex = @object.RightFootIkEndEffectorBoneIndex;
				this.LeftFootIkEndEffectorBoneIndex = @object.LeftFootIkEndEffectorBoneIndex;
				this.RightFootIkTipBoneIndex = @object.RightFootIkTipBoneIndex;
				this.LeftFootIkTipBoneIndex = @object.LeftFootIkTipBoneIndex;
				this.FootNumBonesForIk = @object.FootNumBonesForIk;
				this.ReinHandleLeftLocalPosition = @object.ReinHandleLeftLocalPosition;
				this.ReinHandleRightLocalPosition = @object.ReinHandleRightLocalPosition;
				this.ReinSkeleton = @object.ReinSkeleton;
				this.ReinCollisionBody = @object.ReinCollisionBody;
				this.FrontBoneToDetectGroundSlopeIndex = @object.FrontBoneToDetectGroundSlopeIndex;
				this.BackBoneToDetectGroundSlopeIndex = @object.BackBoneToDetectGroundSlopeIndex;
				list7 = new List<sbyte>(@object.BoneIndicesToModifyOnSlopingGround);
				this.BodyRotationReferenceBoneIndex = @object.BodyRotationReferenceBoneIndex;
				this.RiderSitBoneIndex = @object.RiderSitBoneIndex;
				this.ReinHandleBoneIndex = @object.ReinHandleBoneIndex;
				this.ReinCollision1BoneIndex = @object.ReinCollision1BoneIndex;
				this.ReinCollision2BoneIndex = @object.ReinCollision2BoneIndex;
				this.ReinHeadBoneIndex = @object.ReinHeadBoneIndex;
				this.ReinHeadRightAttachmentBoneIndex = @object.ReinHeadRightAttachmentBoneIndex;
				this.ReinHeadLeftAttachmentBoneIndex = @object.ReinHeadLeftAttachmentBoneIndex;
				this.ReinRightHandBoneIndex = @object.ReinRightHandBoneIndex;
				this.ReinLeftHandBoneIndex = @object.ReinLeftHandBoneIndex;
			}
			else
			{
				list = new List<sbyte>(12);
				list2 = new List<sbyte>(4);
				list3 = new List<sbyte>(8);
				list4 = new List<sbyte>(8);
				list5 = new List<sbyte>(8);
				list6 = new List<sbyte>(8);
				list7 = new List<sbyte>(8);
			}
			XmlAttribute xmlAttribute2 = node.Attributes["action_set"];
			if (xmlAttribute2 != null && !string.IsNullOrEmpty(xmlAttribute2.Value))
			{
				this.ActionSetCode = xmlAttribute2.Value;
			}
			XmlAttribute xmlAttribute3 = node.Attributes["female_action_set"];
			if (xmlAttribute3 != null && !string.IsNullOrEmpty(xmlAttribute3.Value))
			{
				this.FemaleActionSetCode = xmlAttribute3.Value;
			}
			XmlAttribute xmlAttribute4 = node.Attributes["monster_usage"];
			if (xmlAttribute4 != null && !string.IsNullOrEmpty(xmlAttribute4.Value))
			{
				this.MonsterUsage = xmlAttribute4.Value;
			}
			else if (!flag)
			{
				this.MonsterUsage = "";
			}
			if (!flag)
			{
				this.Weight = 1;
			}
			XmlAttribute xmlAttribute5 = node.Attributes["weight"];
			int num;
			if (xmlAttribute5 != null && !string.IsNullOrEmpty(xmlAttribute5.Value) && int.TryParse(xmlAttribute5.Value, out num))
			{
				this.Weight = num;
			}
			if (!flag)
			{
				this.HitPoints = 1;
			}
			XmlAttribute xmlAttribute6 = node.Attributes["hit_points"];
			int num2;
			if (xmlAttribute6 != null && !string.IsNullOrEmpty(xmlAttribute6.Value) && int.TryParse(xmlAttribute6.Value, out num2))
			{
				this.HitPoints = num2;
			}
			XmlAttribute xmlAttribute7 = node.Attributes["num_paces"];
			int num3;
			if (xmlAttribute7 != null && !string.IsNullOrEmpty(xmlAttribute7.Value) && int.TryParse(xmlAttribute7.Value, out num3))
			{
				this.NumPaces = num3;
			}
			XmlAttribute xmlAttribute8 = node.Attributes["walking_speed_limit"];
			float num4;
			if (xmlAttribute8 != null && !string.IsNullOrEmpty(xmlAttribute8.Value) && float.TryParse(xmlAttribute8.Value, out num4))
			{
				this.WalkingSpeedLimit = num4;
			}
			XmlAttribute xmlAttribute9 = node.Attributes["crouch_walking_speed_limit"];
			if (xmlAttribute9 != null && !string.IsNullOrEmpty(xmlAttribute9.Value))
			{
				float num5;
				if (float.TryParse(xmlAttribute9.Value, out num5))
				{
					this.CrouchWalkingSpeedLimit = num5;
				}
			}
			else if (!flag)
			{
				this.CrouchWalkingSpeedLimit = this.WalkingSpeedLimit;
			}
			XmlAttribute xmlAttribute10 = node.Attributes["jump_acceleration"];
			float num6;
			if (xmlAttribute10 != null && !string.IsNullOrEmpty(xmlAttribute10.Value) && float.TryParse(xmlAttribute10.Value, out num6))
			{
				this.JumpAcceleration = num6;
			}
			XmlAttribute xmlAttribute11 = node.Attributes["absorbed_damage_ratio"];
			if (xmlAttribute11 != null && !string.IsNullOrEmpty(xmlAttribute11.Value))
			{
				float num7;
				if (float.TryParse(xmlAttribute11.Value, out num7))
				{
					if (num7 < 0f)
					{
						num7 = 0f;
					}
					this.AbsorbedDamageRatio = num7;
				}
			}
			else if (!flag)
			{
				this.AbsorbedDamageRatio = 1f;
			}
			XmlAttribute xmlAttribute12 = node.Attributes["sound_and_collision_info_class"];
			if (xmlAttribute12 != null && !string.IsNullOrEmpty(xmlAttribute12.Value))
			{
				this.SoundAndCollisionInfoClassName = xmlAttribute12.Value;
			}
			XmlAttribute xmlAttribute13 = node.Attributes["rider_camera_height_adder"];
			float num8;
			if (xmlAttribute13 != null && !string.IsNullOrEmpty(xmlAttribute13.Value) && float.TryParse(xmlAttribute13.Value, out num8))
			{
				this.RiderCameraHeightAdder = num8;
			}
			XmlAttribute xmlAttribute14 = node.Attributes["rider_body_capsule_height_adder"];
			float num9;
			if (xmlAttribute14 != null && !string.IsNullOrEmpty(xmlAttribute14.Value) && float.TryParse(xmlAttribute14.Value, out num9))
			{
				this.RiderBodyCapsuleHeightAdder = num9;
			}
			XmlAttribute xmlAttribute15 = node.Attributes["rider_body_capsule_forward_adder"];
			float num10;
			if (xmlAttribute15 != null && !string.IsNullOrEmpty(xmlAttribute15.Value) && float.TryParse(xmlAttribute15.Value, out num10))
			{
				this.RiderBodyCapsuleForwardAdder = num10;
			}
			XmlAttribute xmlAttribute16 = node.Attributes["preliminary_collision_capsule_radius_multiplier"];
			if (!flag && xmlAttribute16 != null && !string.IsNullOrEmpty(xmlAttribute16.Value))
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Monster.cs", "Deserialize", 425);
			}
			XmlAttribute xmlAttribute17 = node.Attributes["rider_preliminary_collision_capsule_height_multiplier"];
			if (!flag && xmlAttribute17 != null && !string.IsNullOrEmpty(xmlAttribute17.Value))
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Monster.cs", "Deserialize", 434);
			}
			XmlAttribute xmlAttribute18 = node.Attributes["rider_preliminary_collision_capsule_height_adder"];
			if (!flag && xmlAttribute18 != null && !string.IsNullOrEmpty(xmlAttribute18.Value))
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Monster.cs", "Deserialize", 443);
			}
			XmlAttribute xmlAttribute19 = node.Attributes["standing_eye_height"];
			float num11;
			if (xmlAttribute19 != null && !string.IsNullOrEmpty(xmlAttribute19.Value) && float.TryParse(xmlAttribute19.Value, out num11))
			{
				this.StandingEyeHeight = num11;
			}
			XmlAttribute xmlAttribute20 = node.Attributes["crouch_eye_height"];
			float num12;
			if (xmlAttribute20 != null && !string.IsNullOrEmpty(xmlAttribute20.Value) && float.TryParse(xmlAttribute20.Value, out num12))
			{
				this.CrouchEyeHeight = num12;
			}
			XmlAttribute xmlAttribute21 = node.Attributes["mounted_eye_height"];
			float num13;
			if (xmlAttribute21 != null && !string.IsNullOrEmpty(xmlAttribute21.Value) && float.TryParse(xmlAttribute21.Value, out num13))
			{
				this.MountedEyeHeight = num13;
			}
			XmlAttribute xmlAttribute22 = node.Attributes["rider_eye_height_adder"];
			float num14;
			if (xmlAttribute22 != null && !string.IsNullOrEmpty(xmlAttribute22.Value) && float.TryParse(xmlAttribute22.Value, out num14))
			{
				this.RiderEyeHeightAdder = num14;
			}
			if (!flag)
			{
				this.EyeOffsetWrtHead = new Vec3(0.01f, 0.01f, 0.01f, -1f);
			}
			XmlAttribute xmlAttribute23 = node.Attributes["eye_offset_wrt_head"];
			Vec3 vec;
			if (xmlAttribute23 != null && !string.IsNullOrEmpty(xmlAttribute23.Value) && Monster.ReadVec3(xmlAttribute23.Value, out vec))
			{
				this.EyeOffsetWrtHead = vec;
			}
			if (!flag)
			{
				this.FirstPersonCameraOffsetWrtHead = new Vec3(0.01f, 0.01f, 0.01f, -1f);
			}
			XmlAttribute xmlAttribute24 = node.Attributes["first_person_camera_offset_wrt_head"];
			Vec3 vec2;
			if (xmlAttribute24 != null && !string.IsNullOrEmpty(xmlAttribute24.Value) && Monster.ReadVec3(xmlAttribute24.Value, out vec2))
			{
				this.FirstPersonCameraOffsetWrtHead = vec2;
			}
			XmlAttribute xmlAttribute25 = node.Attributes["arm_length"];
			float num15;
			if (xmlAttribute25 != null && !string.IsNullOrEmpty(xmlAttribute25.Value) && float.TryParse(xmlAttribute25.Value, out num15))
			{
				this.ArmLength = num15;
			}
			XmlAttribute xmlAttribute26 = node.Attributes["arm_weight"];
			float num16;
			if (xmlAttribute26 != null && !string.IsNullOrEmpty(xmlAttribute26.Value) && float.TryParse(xmlAttribute26.Value, out num16))
			{
				this.ArmWeight = num16;
			}
			XmlAttribute xmlAttribute27 = node.Attributes["jump_speed_limit"];
			float num17;
			if (xmlAttribute27 != null && !string.IsNullOrEmpty(xmlAttribute27.Value) && float.TryParse(xmlAttribute27.Value, out num17))
			{
				this.JumpSpeedLimit = num17;
			}
			if (!flag)
			{
				this.RelativeSpeedLimitForCharge = float.MaxValue;
			}
			XmlAttribute xmlAttribute28 = node.Attributes["relative_speed_limit_for_charge"];
			float num18;
			if (xmlAttribute28 != null && !string.IsNullOrEmpty(xmlAttribute28.Value) && float.TryParse(xmlAttribute28.Value, out num18))
			{
				this.RelativeSpeedLimitForCharge = num18;
			}
			XmlAttribute xmlAttribute29 = node.Attributes["family_type"];
			int num19;
			if (xmlAttribute29 != null && !string.IsNullOrEmpty(xmlAttribute29.Value) && int.TryParse(xmlAttribute29.Value, out num19))
			{
				this.FamilyType = num19;
			}
			sbyte b = -1;
			this.DeserializeBoneIndexArray(list, node, flag, "ragdoll_bone_to_check_for_corpses_", b, false);
			this.DeserializeBoneIndexArray(list2, node, flag, "ragdoll_fall_sound_bone_", b, false);
			this.HeadLookDirectionBoneIndex = this.DeserializeBoneIndex(node, "head_look_direction_bone", flag ? this.HeadLookDirectionBoneIndex : b, b, true);
			this.SpineLowerBoneIndex = this.DeserializeBoneIndex(node, "spine_lower_bone", flag ? this.SpineLowerBoneIndex : b, b, false);
			this.SpineUpperBoneIndex = this.DeserializeBoneIndex(node, "spine_upper_bone", flag ? this.SpineUpperBoneIndex : b, b, false);
			this.ThoraxLookDirectionBoneIndex = this.DeserializeBoneIndex(node, "thorax_look_direction_bone", flag ? this.ThoraxLookDirectionBoneIndex : b, b, true);
			this.NeckRootBoneIndex = this.DeserializeBoneIndex(node, "neck_root_bone", flag ? this.NeckRootBoneIndex : b, b, true);
			this.PelvisBoneIndex = this.DeserializeBoneIndex(node, "pelvis_bone", flag ? this.PelvisBoneIndex : b, b, false);
			this.RightUpperArmBoneIndex = this.DeserializeBoneIndex(node, "right_upper_arm_bone", flag ? this.RightUpperArmBoneIndex : b, b, false);
			this.LeftUpperArmBoneIndex = this.DeserializeBoneIndex(node, "left_upper_arm_bone", flag ? this.LeftUpperArmBoneIndex : b, b, false);
			this.FallBlowDamageBoneIndex = this.DeserializeBoneIndex(node, "fall_blow_damage_bone", flag ? this.FallBlowDamageBoneIndex : b, b, false);
			this.TerrainDecalBone0Index = this.DeserializeBoneIndex(node, "terrain_decal_bone_0", flag ? this.TerrainDecalBone0Index : b, b, false);
			this.TerrainDecalBone1Index = this.DeserializeBoneIndex(node, "terrain_decal_bone_1", flag ? this.TerrainDecalBone1Index : b, b, false);
			this.DeserializeBoneIndexArray(list3, node, flag, "ragdoll_stationary_check_bone_", b, false);
			this.DeserializeBoneIndexArray(list4, node, flag, "move_adder_bone_", b, false);
			this.DeserializeBoneIndexArray(list5, node, flag, "splash_decal_bone_", b, false);
			this.DeserializeBoneIndexArray(list6, node, flag, "blood_burst_bone_", b, false);
			this.MainHandBoneIndex = this.DeserializeBoneIndex(node, "main_hand_bone", flag ? this.MainHandBoneIndex : b, b, true);
			this.OffHandBoneIndex = this.DeserializeBoneIndex(node, "off_hand_bone", flag ? this.OffHandBoneIndex : b, b, true);
			this.MainHandItemBoneIndex = this.DeserializeBoneIndex(node, "main_hand_item_bone", flag ? this.MainHandItemBoneIndex : b, b, true);
			this.OffHandItemBoneIndex = this.DeserializeBoneIndex(node, "off_hand_item_bone", flag ? this.OffHandItemBoneIndex : b, b, true);
			this.MainHandItemSecondaryBoneIndex = this.DeserializeBoneIndex(node, "main_hand_item_secondary_bone", flag ? this.MainHandItemSecondaryBoneIndex : b, b, false);
			this.OffHandItemSecondaryBoneIndex = this.DeserializeBoneIndex(node, "off_hand_item_secondary_bone", flag ? this.OffHandItemSecondaryBoneIndex : b, b, false);
			this.OffHandShoulderBoneIndex = this.DeserializeBoneIndex(node, "off_hand_shoulder_bone", flag ? this.OffHandShoulderBoneIndex : b, b, false);
			XmlAttribute xmlAttribute30 = node.Attributes["hand_num_bones_for_ik"];
			this.HandNumBonesForIk = ((xmlAttribute30 != null) ? sbyte.Parse(xmlAttribute30.Value) : (flag ? this.HandNumBonesForIk : 0));
			this.PrimaryFootBoneIndex = this.DeserializeBoneIndex(node, "primary_foot_bone", flag ? this.PrimaryFootBoneIndex : b, b, false);
			this.SecondaryFootBoneIndex = this.DeserializeBoneIndex(node, "secondary_foot_bone", flag ? this.SecondaryFootBoneIndex : b, b, false);
			this.RightFootIkEndEffectorBoneIndex = this.DeserializeBoneIndex(node, "right_foot_ik_end_effector_bone", flag ? this.RightFootIkEndEffectorBoneIndex : b, b, true);
			this.LeftFootIkEndEffectorBoneIndex = this.DeserializeBoneIndex(node, "left_foot_ik_end_effector_bone", flag ? this.LeftFootIkEndEffectorBoneIndex : b, b, true);
			this.RightFootIkTipBoneIndex = this.DeserializeBoneIndex(node, "right_foot_ik_tip_bone", flag ? this.RightFootIkTipBoneIndex : b, b, true);
			this.LeftFootIkTipBoneIndex = this.DeserializeBoneIndex(node, "left_foot_ik_tip_bone", flag ? this.LeftFootIkTipBoneIndex : b, b, true);
			XmlAttribute xmlAttribute31 = node.Attributes["foot_num_bones_for_ik"];
			this.FootNumBonesForIk = ((xmlAttribute31 != null) ? sbyte.Parse(xmlAttribute31.Value) : (flag ? this.FootNumBonesForIk : 0));
			XmlNode xmlNode = node.Attributes["rein_handle_left_local_pos"];
			Vec3 vec3;
			if (xmlNode != null && Monster.ReadVec3(xmlNode.Value, out vec3))
			{
				this.ReinHandleLeftLocalPosition = vec3;
			}
			XmlNode xmlNode2 = node.Attributes["rein_handle_right_local_pos"];
			Vec3 vec4;
			if (xmlNode2 != null && Monster.ReadVec3(xmlNode2.Value, out vec4))
			{
				this.ReinHandleRightLocalPosition = vec4;
			}
			XmlAttribute xmlAttribute32 = node.Attributes["rein_skeleton"];
			this.ReinSkeleton = ((xmlAttribute32 != null) ? xmlAttribute32.Value : this.ReinSkeleton);
			XmlAttribute xmlAttribute33 = node.Attributes["rein_collision_body"];
			this.ReinCollisionBody = ((xmlAttribute33 != null) ? xmlAttribute33.Value : this.ReinCollisionBody);
			this.DeserializeBoneIndexArray(list7, node, flag, "bones_to_modify_on_sloping_ground_", b, true);
			XmlAttribute xmlAttribute34 = node.Attributes["front_bone_to_detect_ground_slope_index"];
			this.FrontBoneToDetectGroundSlopeIndex = ((xmlAttribute34 != null) ? sbyte.Parse(xmlAttribute34.Value) : (flag ? this.FrontBoneToDetectGroundSlopeIndex : -1));
			XmlAttribute xmlAttribute35 = node.Attributes["back_bone_to_detect_ground_slope_index"];
			this.BackBoneToDetectGroundSlopeIndex = ((xmlAttribute35 != null) ? sbyte.Parse(xmlAttribute35.Value) : (flag ? this.BackBoneToDetectGroundSlopeIndex : -1));
			this.BodyRotationReferenceBoneIndex = this.DeserializeBoneIndex(node, "body_rotation_reference_bone", flag ? this.BodyRotationReferenceBoneIndex : b, b, true);
			this.RiderSitBoneIndex = this.DeserializeBoneIndex(node, "rider_sit_bone", flag ? this.RiderSitBoneIndex : b, b, false);
			this.ReinHandleBoneIndex = this.DeserializeBoneIndex(node, "rein_handle_bone", flag ? this.ReinHandleBoneIndex : b, b, false);
			this.ReinCollision1BoneIndex = this.DeserializeBoneIndex(node, "rein_collision_1_bone", flag ? this.ReinCollision1BoneIndex : b, b, false);
			this.ReinCollision2BoneIndex = this.DeserializeBoneIndex(node, "rein_collision_2_bone", flag ? this.ReinCollision2BoneIndex : b, b, false);
			this.ReinHeadBoneIndex = this.DeserializeBoneIndex(node, "rein_head_bone", flag ? this.ReinHeadBoneIndex : b, b, false);
			this.ReinHeadRightAttachmentBoneIndex = this.DeserializeBoneIndex(node, "rein_head_right_attachment_bone", flag ? this.ReinHeadRightAttachmentBoneIndex : b, b, false);
			this.ReinHeadLeftAttachmentBoneIndex = this.DeserializeBoneIndex(node, "rein_head_left_attachment_bone", flag ? this.ReinHeadLeftAttachmentBoneIndex : b, b, false);
			this.ReinRightHandBoneIndex = this.DeserializeBoneIndex(node, "rein_right_hand_bone", flag ? this.ReinRightHandBoneIndex : b, b, false);
			this.ReinLeftHandBoneIndex = this.DeserializeBoneIndex(node, "rein_left_hand_bone", flag ? this.ReinLeftHandBoneIndex : b, b, false);
			this.IndicesOfRagdollBonesToCheckForCorpses = list.ToArray();
			this.RagdollFallSoundBoneIndices = list2.ToArray();
			this.RagdollStationaryCheckBoneIndices = list3.ToArray();
			this.MoveAdderBoneIndices = list4.ToArray();
			this.SplashDecalBoneIndices = list5.ToArray();
			this.BloodBurstBoneIndices = list6.ToArray();
			this.BoneIndicesToModifyOnSlopingGround = list7.ToArray();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode3 = (XmlNode)obj;
				if (xmlNode3.Name == "Flags")
				{
					this.Flags = AgentFlag.None;
					using (IEnumerator enumerator2 = Enum.GetValues(typeof(AgentFlag)).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							AgentFlag agentFlag = (AgentFlag)obj2;
							XmlAttribute xmlAttribute36 = xmlNode3.Attributes[agentFlag.ToString()];
							if (xmlAttribute36 != null && !xmlAttribute36.Value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
							{
								this.Flags |= agentFlag;
							}
						}
						continue;
					}
				}
				if (xmlNode3.Name == "Capsules")
				{
					foreach (object obj3 in xmlNode3.ChildNodes)
					{
						XmlNode xmlNode4 = (XmlNode)obj3;
						if (xmlNode4.Attributes != null && (xmlNode4.Name == "preliminary_collision_capsule" || xmlNode4.Name == "body_capsule" || xmlNode4.Name == "crouched_body_capsule"))
						{
							bool flag2 = true;
							Vec3 vec5 = new Vec3(0f, 0f, 0.01f, -1f);
							Vec3 vec6 = Vec3.Zero;
							float num20 = 0.01f;
							if (xmlNode4.Attributes["pos1"] != null)
							{
								Vec3 vec7;
								flag2 = Monster.ReadVec3(xmlNode4.Attributes["pos1"].Value, out vec7) && flag2;
								if (flag2)
								{
									vec5 = vec7;
								}
							}
							if (xmlNode4.Attributes["pos2"] != null)
							{
								Vec3 vec8;
								flag2 = Monster.ReadVec3(xmlNode4.Attributes["pos2"].Value, out vec8) && flag2;
								if (flag2)
								{
									vec6 = vec8;
								}
							}
							if (xmlNode4.Attributes["radius"] != null)
							{
								string text = xmlNode4.Attributes["radius"].Value;
								text = text.Trim();
								flag2 = flag2 && float.TryParse(text, out num20);
							}
							if (flag2)
							{
								if (xmlNode4.Name.StartsWith("p"))
								{
									Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Monster.cs", "Deserialize", 705);
								}
								else if (xmlNode4.Name.StartsWith("c"))
								{
									this.CrouchedBodyCapsuleRadius = num20;
									this.CrouchedBodyCapsulePoint1 = vec5;
									this.CrouchedBodyCapsulePoint2 = vec6;
								}
								else
								{
									this.BodyCapsuleRadius = num20;
									this.BodyCapsulePoint1 = vec5;
									this.BodyCapsulePoint2 = vec6;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x0001E8BC File Offset: 0x0001CABC
		private sbyte DeserializeBoneIndex(XmlNode node, string attributeName, sbyte baseValue, sbyte invalidBoneIndex, bool validateHasParentBone)
		{
			XmlAttribute xmlAttribute = node.Attributes[attributeName];
			sbyte b = ((Monster.GetBoneIndexWithId != null && xmlAttribute != null) ? Monster.GetBoneIndexWithId(this.ActionSetCode, xmlAttribute.Value) : baseValue);
			if (validateHasParentBone && b != invalidBoneIndex)
			{
				Func<string, sbyte, bool> getBoneHasParentBone = Monster.GetBoneHasParentBone;
			}
			return b;
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x0001E90C File Offset: 0x0001CB0C
		private void DeserializeBoneIndexArray(List<sbyte> boneIndices, XmlNode node, bool hasBaseMonster, string attributeNamePrefix, sbyte invalidBoneIndex, bool validateHasParentBone)
		{
			int num = 0;
			for (;;)
			{
				bool flag = hasBaseMonster && num < boneIndices.Count;
				sbyte b = this.DeserializeBoneIndex(node, attributeNamePrefix + num, flag ? boneIndices[num] : invalidBoneIndex, invalidBoneIndex, validateHasParentBone);
				if (b == invalidBoneIndex)
				{
					break;
				}
				if (flag)
				{
					boneIndices[num] = b;
				}
				else
				{
					boneIndices.Add(b);
				}
				num++;
			}
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x0001E974 File Offset: 0x0001CB74
		private static bool ReadVec3(string str, out Vec3 v)
		{
			str = str.Trim();
			string[] array = str.Split(",".ToCharArray());
			v = new Vec3(0f, 0f, 0f, -1f);
			return float.TryParse(array[0], out v.x) && float.TryParse(array[1], out v.y) && float.TryParse(array[2], out v.z);
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x0001E9EC File Offset: 0x0001CBEC
		public sbyte GetBoneToAttachForItemFlags(ItemFlags itemFlags)
		{
			ItemFlags itemFlags2 = itemFlags & ItemFlags.AttachmentMask;
			if (itemFlags2 <= (ItemFlags)0U)
			{
				return this.MainHandItemBoneIndex;
			}
			if (itemFlags2 == ItemFlags.ForceAttachOffHandPrimaryItemBone)
			{
				return this.OffHandItemBoneIndex;
			}
			if (itemFlags2 != ItemFlags.ForceAttachOffHandSecondaryItemBone)
			{
				return this.MainHandItemBoneIndex;
			}
			return this.OffHandItemSecondaryBoneIndex;
		}

		// Token: 0x040004F6 RID: 1270
		public static Func<string, string, sbyte> GetBoneIndexWithId;

		// Token: 0x040004F7 RID: 1271
		public static Func<string, sbyte, bool> GetBoneHasParentBone;

		// Token: 0x0400054A RID: 1354
		[CachedData]
		private IMonsterMissionData _monsterMissionData;
	}
}
