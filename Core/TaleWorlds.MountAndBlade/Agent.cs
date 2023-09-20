using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000E9 RID: 233
	public sealed class Agent : DotNetObject, IAgent, IFocusable, IUsable, IFormationUnit, ITrackableBase
	{
		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000899 RID: 2201 RVA: 0x0000F418 File Offset: 0x0000D618
		public static Agent Main
		{
			get
			{
				Mission mission = Mission.Current;
				if (mission == null)
				{
					return null;
				}
				return mission.MainAgent;
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600089A RID: 2202 RVA: 0x0000F42C File Offset: 0x0000D62C
		// (remove) Token: 0x0600089B RID: 2203 RVA: 0x0000F464 File Offset: 0x0000D664
		public event Agent.OnAgentHealthChangedDelegate OnAgentHealthChanged;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600089C RID: 2204 RVA: 0x0000F49C File Offset: 0x0000D69C
		// (remove) Token: 0x0600089D RID: 2205 RVA: 0x0000F4D4 File Offset: 0x0000D6D4
		public event Agent.OnMountHealthChangedDelegate OnMountHealthChanged;

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x0600089E RID: 2206 RVA: 0x0000F509 File Offset: 0x0000D709
		public bool IsPlayerControlled
		{
			get
			{
				return this.IsMine || this.MissionPeer != null;
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x0600089F RID: 2207 RVA: 0x0000F51E File Offset: 0x0000D71E
		public bool IsMine
		{
			get
			{
				return this.Controller == Agent.ControllerType.Player;
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x060008A0 RID: 2208 RVA: 0x0000F529 File Offset: 0x0000D729
		public bool IsMainAgent
		{
			get
			{
				return this == Agent.Main;
			}
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x060008A1 RID: 2209 RVA: 0x0000F533 File Offset: 0x0000D733
		public bool IsHuman
		{
			get
			{
				return (this.GetAgentFlags() & AgentFlag.IsHumanoid) > AgentFlag.None;
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x060008A2 RID: 2210 RVA: 0x0000F544 File Offset: 0x0000D744
		public bool IsMount
		{
			get
			{
				return (this.GetAgentFlags() & AgentFlag.Mountable) > AgentFlag.None;
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x060008A3 RID: 2211 RVA: 0x0000F551 File Offset: 0x0000D751
		public bool IsAIControlled
		{
			get
			{
				return this.Controller == Agent.ControllerType.AI && !GameNetwork.IsClientOrReplay;
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x060008A4 RID: 2212 RVA: 0x0000F566 File Offset: 0x0000D766
		public bool IsPlayerTroop
		{
			get
			{
				return !GameNetwork.IsMultiplayer && this.Origin != null && this.Origin.Troop == Game.Current.PlayerTroop;
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060008A5 RID: 2213 RVA: 0x0000F590 File Offset: 0x0000D790
		public bool IsUsingGameObject
		{
			get
			{
				return this.CurrentlyUsedGameObject != null;
			}
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060008A6 RID: 2214 RVA: 0x0000F59B File Offset: 0x0000D79B
		public bool CanLeadFormationsRemotely
		{
			get
			{
				return this._canLeadFormationsRemotely;
			}
		}

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060008A7 RID: 2215 RVA: 0x0000F5A3 File Offset: 0x0000D7A3
		public bool IsDetachableFromFormation
		{
			get
			{
				return this._isDetachableFromFormation;
			}
		}

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060008A8 RID: 2216 RVA: 0x0000F5AB File Offset: 0x0000D7AB
		public float AgentScale
		{
			get
			{
				return MBAPI.IMBAgent.GetAgentScale(this.GetPtr());
			}
		}

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x060008A9 RID: 2217 RVA: 0x0000F5BD File Offset: 0x0000D7BD
		public bool CrouchMode
		{
			get
			{
				return MBAPI.IMBAgent.GetCrouchMode(this.GetPtr());
			}
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x060008AA RID: 2218 RVA: 0x0000F5CF File Offset: 0x0000D7CF
		public bool WalkMode
		{
			get
			{
				return MBAPI.IMBAgent.GetWalkMode(this.GetPtr());
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x060008AB RID: 2219 RVA: 0x0000F5E1 File Offset: 0x0000D7E1
		public Vec3 Position
		{
			get
			{
				return AgentHelper.GetAgentPosition(this.PositionPointer);
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x060008AC RID: 2220 RVA: 0x0000F5EE File Offset: 0x0000D7EE
		public Vec3 VisualPosition
		{
			get
			{
				return MBAPI.IMBAgent.GetVisualPosition(this.GetPtr());
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x060008AD RID: 2221 RVA: 0x0000F600 File Offset: 0x0000D800
		public Vec2 MovementVelocity
		{
			get
			{
				return MBAPI.IMBAgent.GetMovementVelocity(this.GetPtr());
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060008AE RID: 2222 RVA: 0x0000F612 File Offset: 0x0000D812
		public Vec3 AverageVelocity
		{
			get
			{
				return MBAPI.IMBAgent.GetAverageVelocity(this.GetPtr());
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060008AF RID: 2223 RVA: 0x0000F624 File Offset: 0x0000D824
		public float MaximumForwardUnlimitedSpeed
		{
			get
			{
				return MBAPI.IMBAgent.GetMaximumForwardUnlimitedSpeed(this.GetPtr());
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060008B0 RID: 2224 RVA: 0x0000F636 File Offset: 0x0000D836
		public float MovementDirectionAsAngle
		{
			get
			{
				return MBAPI.IMBAgent.GetMovementDirectionAsAngle(this.GetPtr());
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060008B1 RID: 2225 RVA: 0x0000F648 File Offset: 0x0000D848
		public bool IsLookRotationInSlowMotion
		{
			get
			{
				return MBAPI.IMBAgent.IsLookRotationInSlowMotion(this.GetPtr());
			}
		}

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060008B2 RID: 2226 RVA: 0x0000F65A File Offset: 0x0000D85A
		public Agent.AgentPropertiesModifiers PropertyModifiers
		{
			get
			{
				return this._propertyModifiers;
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060008B3 RID: 2227 RVA: 0x0000F662 File Offset: 0x0000D862
		public MBActionSet ActionSet
		{
			get
			{
				return new MBActionSet(MBAPI.IMBAgent.GetActionSetNo(this.GetPtr()));
			}
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060008B4 RID: 2228 RVA: 0x0000F679 File Offset: 0x0000D879
		public MBReadOnlyList<AgentComponent> Components
		{
			get
			{
				return this._components;
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060008B5 RID: 2229 RVA: 0x0000F681 File Offset: 0x0000D881
		public MBReadOnlyList<Agent.Hitter> HitterList
		{
			get
			{
				return this._hitterList;
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060008B6 RID: 2230 RVA: 0x0000F689 File Offset: 0x0000D889
		public Agent.GuardMode CurrentGuardMode
		{
			get
			{
				return MBAPI.IMBAgent.GetCurrentGuardMode(this.GetPtr());
			}
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060008B7 RID: 2231 RVA: 0x0000F69B File Offset: 0x0000D89B
		public Agent ImmediateEnemy
		{
			get
			{
				return MBAPI.IMBAgent.GetImmediateEnemy(this.GetPtr());
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060008B8 RID: 2232 RVA: 0x0000F6AD File Offset: 0x0000D8AD
		public bool IsDoingPassiveAttack
		{
			get
			{
				return MBAPI.IMBAgent.GetIsDoingPassiveAttack(this.GetPtr());
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x060008B9 RID: 2233 RVA: 0x0000F6BF File Offset: 0x0000D8BF
		public bool IsPassiveUsageConditionsAreMet
		{
			get
			{
				return MBAPI.IMBAgent.GetIsPassiveUsageConditionsAreMet(this.GetPtr());
			}
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x060008BA RID: 2234 RVA: 0x0000F6D1 File Offset: 0x0000D8D1
		public float CurrentAimingError
		{
			get
			{
				return MBAPI.IMBAgent.GetCurrentAimingError(this.GetPtr());
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060008BB RID: 2235 RVA: 0x0000F6E3 File Offset: 0x0000D8E3
		public float CurrentAimingTurbulance
		{
			get
			{
				return MBAPI.IMBAgent.GetCurrentAimingTurbulance(this.GetPtr());
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060008BC RID: 2236 RVA: 0x0000F6F5 File Offset: 0x0000D8F5
		public Agent.UsageDirection AttackDirection
		{
			get
			{
				return MBAPI.IMBAgent.GetAttackDirectionUsage(this.GetPtr());
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060008BD RID: 2237 RVA: 0x0000F707 File Offset: 0x0000D907
		public float WalkingSpeedLimitOfMountable
		{
			get
			{
				return MBAPI.IMBAgent.GetWalkSpeedLimitOfMountable(this.GetPtr());
			}
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060008BE RID: 2238 RVA: 0x0000F719 File Offset: 0x0000D919
		public Agent RiderAgent
		{
			get
			{
				return this.GetRiderAgentAux();
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060008BF RID: 2239 RVA: 0x0000F721 File Offset: 0x0000D921
		public bool HasMount
		{
			get
			{
				return this.MountAgent != null;
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060008C0 RID: 2240 RVA: 0x0000F72C File Offset: 0x0000D92C
		public bool CanLogCombatFor
		{
			get
			{
				return (this.RiderAgent != null && !this.RiderAgent.IsAIControlled) || (!this.IsMount && !this.IsAIControlled);
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060008C1 RID: 2241 RVA: 0x0000F758 File Offset: 0x0000D958
		public float MissileRangeAdjusted
		{
			get
			{
				return this.GetMissileRangeWithHeightDifference();
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x060008C2 RID: 2242 RVA: 0x0000F760 File Offset: 0x0000D960
		public float MaximumMissileRange
		{
			get
			{
				return this.GetMissileRange();
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x060008C3 RID: 2243 RVA: 0x0000F768 File Offset: 0x0000D968
		FocusableObjectType IFocusable.FocusableObjectType
		{
			get
			{
				if (!this.IsMount)
				{
					return FocusableObjectType.Agent;
				}
				return FocusableObjectType.Mount;
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x060008C4 RID: 2244 RVA: 0x0000F775 File Offset: 0x0000D975
		public string Name
		{
			get
			{
				if (this.MissionPeer == null)
				{
					return this._name.ToString();
				}
				return this.MissionPeer.Name;
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x060008C5 RID: 2245 RVA: 0x0000F796 File Offset: 0x0000D996
		public AgentMovementLockedState MovementLockedState
		{
			get
			{
				return this.GetMovementLockedState();
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x060008C6 RID: 2246 RVA: 0x0000F79E File Offset: 0x0000D99E
		public Monster Monster { get; }

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060008C7 RID: 2247 RVA: 0x0000F7A6 File Offset: 0x0000D9A6
		// (set) Token: 0x060008C8 RID: 2248 RVA: 0x0000F7AE File Offset: 0x0000D9AE
		public bool IsRunningAway { get; private set; }

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060008C9 RID: 2249 RVA: 0x0000F7B7 File Offset: 0x0000D9B7
		// (set) Token: 0x060008CA RID: 2250 RVA: 0x0000F7BF File Offset: 0x0000D9BF
		public BodyProperties BodyPropertiesValue { get; private set; }

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060008CB RID: 2251 RVA: 0x0000F7C8 File Offset: 0x0000D9C8
		// (set) Token: 0x060008CC RID: 2252 RVA: 0x0000F7D0 File Offset: 0x0000D9D0
		public CommonAIComponent CommonAIComponent { get; private set; }

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060008CD RID: 2253 RVA: 0x0000F7D9 File Offset: 0x0000D9D9
		// (set) Token: 0x060008CE RID: 2254 RVA: 0x0000F7E1 File Offset: 0x0000D9E1
		public HumanAIComponent HumanAIComponent { get; private set; }

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x060008CF RID: 2255 RVA: 0x0000F7EA File Offset: 0x0000D9EA
		// (set) Token: 0x060008D0 RID: 2256 RVA: 0x0000F7F2 File Offset: 0x0000D9F2
		public int BodyPropertiesSeed { get; internal set; }

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x060008D1 RID: 2257 RVA: 0x0000F7FB File Offset: 0x0000D9FB
		// (set) Token: 0x060008D2 RID: 2258 RVA: 0x0000F803 File Offset: 0x0000DA03
		public float LastRangedHitTime { get; private set; } = float.MinValue;

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x060008D3 RID: 2259 RVA: 0x0000F80C File Offset: 0x0000DA0C
		// (set) Token: 0x060008D4 RID: 2260 RVA: 0x0000F814 File Offset: 0x0000DA14
		public float LastMeleeHitTime { get; private set; } = float.MinValue;

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x060008D5 RID: 2261 RVA: 0x0000F81D File Offset: 0x0000DA1D
		// (set) Token: 0x060008D6 RID: 2262 RVA: 0x0000F825 File Offset: 0x0000DA25
		public float LastRangedAttackTime { get; private set; } = float.MinValue;

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x060008D7 RID: 2263 RVA: 0x0000F82E File Offset: 0x0000DA2E
		// (set) Token: 0x060008D8 RID: 2264 RVA: 0x0000F836 File Offset: 0x0000DA36
		public float LastMeleeAttackTime { get; private set; } = float.MinValue;

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x060008D9 RID: 2265 RVA: 0x0000F83F File Offset: 0x0000DA3F
		// (set) Token: 0x060008DA RID: 2266 RVA: 0x0000F847 File Offset: 0x0000DA47
		public bool IsFemale { get; set; }

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x060008DB RID: 2267 RVA: 0x0000F850 File Offset: 0x0000DA50
		public ItemObject Banner
		{
			get
			{
				MissionEquipment equipment = this.Equipment;
				if (equipment == null)
				{
					return null;
				}
				return equipment.GetBanner();
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x060008DC RID: 2268 RVA: 0x0000F863 File Offset: 0x0000DA63
		public ItemObject FormationBanner
		{
			get
			{
				return this._formationBanner;
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x060008DD RID: 2269 RVA: 0x0000F86C File Offset: 0x0000DA6C
		public MissionWeapon WieldedWeapon
		{
			get
			{
				EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
				if (wieldedItemIndex < EquipmentIndex.WeaponItemBeginSlot)
				{
					return MissionWeapon.Invalid;
				}
				return this.Equipment[wieldedItemIndex];
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x060008DE RID: 2270 RVA: 0x0000F897 File Offset: 0x0000DA97
		// (set) Token: 0x060008DF RID: 2271 RVA: 0x0000F89F File Offset: 0x0000DA9F
		public bool IsItemUseDisabled { get; set; }

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x060008E0 RID: 2272 RVA: 0x0000F8A8 File Offset: 0x0000DAA8
		// (set) Token: 0x060008E1 RID: 2273 RVA: 0x0000F8B0 File Offset: 0x0000DAB0
		public bool SyncHealthToAllClients { get; private set; }

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x060008E2 RID: 2274 RVA: 0x0000F8B9 File Offset: 0x0000DAB9
		// (set) Token: 0x060008E3 RID: 2275 RVA: 0x0000F8C1 File Offset: 0x0000DAC1
		public UsableMissionObject CurrentlyUsedGameObject { get; private set; }

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x060008E4 RID: 2276 RVA: 0x0000F8CA File Offset: 0x0000DACA
		public bool CombatActionsEnabled
		{
			get
			{
				return this.CurrentlyUsedGameObject == null || !this.CurrentlyUsedGameObject.DisableCombatActionsOnUse;
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x060008E5 RID: 2277 RVA: 0x0000F8E4 File Offset: 0x0000DAE4
		// (set) Token: 0x060008E6 RID: 2278 RVA: 0x0000F8EC File Offset: 0x0000DAEC
		public Mission Mission { get; private set; }

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x060008E7 RID: 2279 RVA: 0x0000F8F5 File Offset: 0x0000DAF5
		public bool IsHero
		{
			get
			{
				return this.Character != null && this.Character.IsHero;
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x060008E8 RID: 2280 RVA: 0x0000F90C File Offset: 0x0000DB0C
		public int Index { get; }

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x060008E9 RID: 2281 RVA: 0x0000F914 File Offset: 0x0000DB14
		// (set) Token: 0x060008EA RID: 2282 RVA: 0x0000F91C File Offset: 0x0000DB1C
		public MissionEquipment Equipment { get; private set; }

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x060008EB RID: 2283 RVA: 0x0000F925 File Offset: 0x0000DB25
		// (set) Token: 0x060008EC RID: 2284 RVA: 0x0000F92D File Offset: 0x0000DB2D
		public TextObject AgentRole { get; set; }

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x060008ED RID: 2285 RVA: 0x0000F936 File Offset: 0x0000DB36
		// (set) Token: 0x060008EE RID: 2286 RVA: 0x0000F93E File Offset: 0x0000DB3E
		public bool HasBeenBuilt { get; private set; }

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060008EF RID: 2287 RVA: 0x0000F947 File Offset: 0x0000DB47
		// (set) Token: 0x060008F0 RID: 2288 RVA: 0x0000F94F File Offset: 0x0000DB4F
		public Agent.MortalityState CurrentMortalityState { get; private set; }

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x060008F1 RID: 2289 RVA: 0x0000F958 File Offset: 0x0000DB58
		// (set) Token: 0x060008F2 RID: 2290 RVA: 0x0000F960 File Offset: 0x0000DB60
		public Equipment SpawnEquipment { get; private set; }

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x060008F3 RID: 2291 RVA: 0x0000F969 File Offset: 0x0000DB69
		// (set) Token: 0x060008F4 RID: 2292 RVA: 0x0000F971 File Offset: 0x0000DB71
		public FormationPositionPreference FormationPositionPreference { get; set; }

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x060008F5 RID: 2293 RVA: 0x0000F97A File Offset: 0x0000DB7A
		// (set) Token: 0x060008F6 RID: 2294 RVA: 0x0000F982 File Offset: 0x0000DB82
		public bool RandomizeColors { get; private set; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x060008F7 RID: 2295 RVA: 0x0000F98B File Offset: 0x0000DB8B
		// (set) Token: 0x060008F8 RID: 2296 RVA: 0x0000F993 File Offset: 0x0000DB93
		public float CharacterPowerCached { get; private set; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x060008F9 RID: 2297 RVA: 0x0000F99C File Offset: 0x0000DB9C
		// (set) Token: 0x060008FA RID: 2298 RVA: 0x0000F9A4 File Offset: 0x0000DBA4
		public float WalkSpeedCached { get; private set; }

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x060008FB RID: 2299 RVA: 0x0000F9AD File Offset: 0x0000DBAD
		// (set) Token: 0x060008FC RID: 2300 RVA: 0x0000F9B5 File Offset: 0x0000DBB5
		public float RunSpeedCached { get; private set; }

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x060008FD RID: 2301 RVA: 0x0000F9BE File Offset: 0x0000DBBE
		// (set) Token: 0x060008FE RID: 2302 RVA: 0x0000F9C6 File Offset: 0x0000DBC6
		public IAgentOriginBase Origin { get; set; }

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x060008FF RID: 2303 RVA: 0x0000F9CF File Offset: 0x0000DBCF
		// (set) Token: 0x06000900 RID: 2304 RVA: 0x0000F9D7 File Offset: 0x0000DBD7
		public Team Team { get; private set; }

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000901 RID: 2305 RVA: 0x0000F9E0 File Offset: 0x0000DBE0
		// (set) Token: 0x06000902 RID: 2306 RVA: 0x0000F9E8 File Offset: 0x0000DBE8
		public int KillCount { get; set; }

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000903 RID: 2307 RVA: 0x0000F9F1 File Offset: 0x0000DBF1
		// (set) Token: 0x06000904 RID: 2308 RVA: 0x0000F9F9 File Offset: 0x0000DBF9
		public AgentDrivenProperties AgentDrivenProperties { get; private set; }

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000905 RID: 2309 RVA: 0x0000FA02 File Offset: 0x0000DC02
		// (set) Token: 0x06000906 RID: 2310 RVA: 0x0000FA0A File Offset: 0x0000DC0A
		public float BaseHealthLimit { get; set; }

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06000907 RID: 2311 RVA: 0x0000FA13 File Offset: 0x0000DC13
		// (set) Token: 0x06000908 RID: 2312 RVA: 0x0000FA1B File Offset: 0x0000DC1B
		public string HorseCreationKey { get; private set; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000909 RID: 2313 RVA: 0x0000FA24 File Offset: 0x0000DC24
		// (set) Token: 0x0600090A RID: 2314 RVA: 0x0000FA2C File Offset: 0x0000DC2C
		public float HealthLimit { get; set; }

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x0600090B RID: 2315 RVA: 0x0000FA35 File Offset: 0x0000DC35
		public bool IsRangedCached
		{
			get
			{
				return this.Equipment.ContainsNonConsumableRangedWeaponWithAmmo();
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x0600090C RID: 2316 RVA: 0x0000FA42 File Offset: 0x0000DC42
		public bool HasMeleeWeaponCached
		{
			get
			{
				return this.Equipment.ContainsMeleeWeapon();
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x0600090D RID: 2317 RVA: 0x0000FA4F File Offset: 0x0000DC4F
		public bool HasShieldCached
		{
			get
			{
				return this.Equipment.ContainsShield();
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x0600090E RID: 2318 RVA: 0x0000FA5C File Offset: 0x0000DC5C
		public bool HasSpearCached
		{
			get
			{
				return this.Equipment.ContainsSpear();
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x0600090F RID: 2319 RVA: 0x0000FA69 File Offset: 0x0000DC69
		public bool HasThrownCached
		{
			get
			{
				return this.Equipment.ContainsThrownWeapon();
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000910 RID: 2320 RVA: 0x0000FA76 File Offset: 0x0000DC76
		// (set) Token: 0x06000911 RID: 2321 RVA: 0x0000FA88 File Offset: 0x0000DC88
		public Agent.AIStateFlag AIStateFlags
		{
			get
			{
				return MBAPI.IMBAgent.GetAIStateFlags(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetAIStateFlags(this.GetPtr(), value);
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000912 RID: 2322 RVA: 0x0000FA9C File Offset: 0x0000DC9C
		public MatrixFrame Frame
		{
			get
			{
				MatrixFrame matrixFrame = default(MatrixFrame);
				MBAPI.IMBAgent.GetRotationFrame(this.GetPtr(), ref matrixFrame);
				return matrixFrame;
			}
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000913 RID: 2323 RVA: 0x0000FAC4 File Offset: 0x0000DCC4
		// (set) Token: 0x06000914 RID: 2324 RVA: 0x0000FAD6 File Offset: 0x0000DCD6
		public Agent.MovementControlFlag MovementFlags
		{
			get
			{
				return (Agent.MovementControlFlag)MBAPI.IMBAgent.GetMovementFlags(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetMovementFlags(this.GetPtr(), value);
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000915 RID: 2325 RVA: 0x0000FAE9 File Offset: 0x0000DCE9
		// (set) Token: 0x06000916 RID: 2326 RVA: 0x0000FAFB File Offset: 0x0000DCFB
		public Vec2 MovementInputVector
		{
			get
			{
				return MBAPI.IMBAgent.GetMovementInputVector(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetMovementInputVector(this.GetPtr(), value);
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000917 RID: 2327 RVA: 0x0000FB10 File Offset: 0x0000DD10
		public CapsuleData CollisionCapsule
		{
			get
			{
				CapsuleData capsuleData = default(CapsuleData);
				MBAPI.IMBAgent.GetCollisionCapsule(this.GetPtr(), ref capsuleData);
				return capsuleData;
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000918 RID: 2328 RVA: 0x0000FB38 File Offset: 0x0000DD38
		public Vec3 CollisionCapsuleCenter
		{
			get
			{
				CapsuleData collisionCapsule = this.CollisionCapsule;
				return (collisionCapsule.GetBoxMax() + collisionCapsule.GetBoxMin()) * 0.5f;
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000919 RID: 2329 RVA: 0x0000FB6C File Offset: 0x0000DD6C
		public MBAgentVisuals AgentVisuals
		{
			get
			{
				MBAgentVisuals agentVisuals;
				if (!this._visualsWeakRef.TryGetTarget(out agentVisuals))
				{
					agentVisuals = MBAPI.IMBAgent.GetAgentVisuals(this.GetPtr());
					this._visualsWeakRef.SetTarget(agentVisuals);
				}
				return agentVisuals;
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x0600091A RID: 2330 RVA: 0x0000FBA6 File Offset: 0x0000DDA6
		// (set) Token: 0x0600091B RID: 2331 RVA: 0x0000FBB8 File Offset: 0x0000DDB8
		public bool HeadCameraMode
		{
			get
			{
				return MBAPI.IMBAgent.GetHeadCameraMode(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetHeadCameraMode(this.GetPtr(), value);
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x0600091C RID: 2332 RVA: 0x0000FBCB File Offset: 0x0000DDCB
		// (set) Token: 0x0600091D RID: 2333 RVA: 0x0000FBD3 File Offset: 0x0000DDD3
		public Agent MountAgent
		{
			get
			{
				return this.GetMountAgentAux();
			}
			private set
			{
				this.SetMountAgent(value);
				this.UpdateAgentStats();
			}
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x0600091E RID: 2334 RVA: 0x0000FBE2 File Offset: 0x0000DDE2
		// (set) Token: 0x0600091F RID: 2335 RVA: 0x0000FBEA File Offset: 0x0000DDEA
		public IDetachment Detachment
		{
			get
			{
				return this._detachment;
			}
			set
			{
				this._detachment = value;
				if (this._detachment != null)
				{
					Formation formation = this.Formation;
					if (formation == null)
					{
						return;
					}
					formation.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(this);
				}
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000920 RID: 2336 RVA: 0x0000FC16 File Offset: 0x0000DE16
		// (set) Token: 0x06000921 RID: 2337 RVA: 0x0000FC24 File Offset: 0x0000DE24
		public bool IsPaused
		{
			get
			{
				return this.AIStateFlags.HasAnyFlag(Agent.AIStateFlag.Paused);
			}
			set
			{
				if (value)
				{
					this.AIStateFlags |= Agent.AIStateFlag.Paused;
					return;
				}
				this.AIStateFlags &= ~Agent.AIStateFlag.Paused;
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000922 RID: 2338 RVA: 0x0000FC47 File Offset: 0x0000DE47
		public bool IsDetachedFromFormation
		{
			get
			{
				return this._detachment != null;
			}
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000923 RID: 2339 RVA: 0x0000FC54 File Offset: 0x0000DE54
		// (set) Token: 0x06000924 RID: 2340 RVA: 0x0000FC7C File Offset: 0x0000DE7C
		public Agent.WatchState CurrentWatchState
		{
			get
			{
				Agent.AIStateFlag aistateFlags = this.AIStateFlags;
				if ((aistateFlags & Agent.AIStateFlag.Alarmed) == Agent.AIStateFlag.Alarmed)
				{
					return Agent.WatchState.Alarmed;
				}
				if ((aistateFlags & Agent.AIStateFlag.Cautious) == Agent.AIStateFlag.Cautious)
				{
					return Agent.WatchState.Cautious;
				}
				return Agent.WatchState.Patrolling;
			}
			private set
			{
				Agent.AIStateFlag aistateFlag = this.AIStateFlags;
				switch (value)
				{
				case Agent.WatchState.Patrolling:
					aistateFlag &= ~(Agent.AIStateFlag.Cautious | Agent.AIStateFlag.Alarmed);
					break;
				case Agent.WatchState.Cautious:
					aistateFlag |= Agent.AIStateFlag.Cautious;
					aistateFlag &= ~Agent.AIStateFlag.Alarmed;
					break;
				case Agent.WatchState.Alarmed:
					aistateFlag |= Agent.AIStateFlag.Alarmed;
					aistateFlag &= ~Agent.AIStateFlag.Cautious;
					break;
				default:
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Agent.cs", "CurrentWatchState", 899);
					break;
				}
				this.AIStateFlags = aistateFlag;
			}
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000925 RID: 2341 RVA: 0x0000FCE1 File Offset: 0x0000DEE1
		// (set) Token: 0x06000926 RID: 2342 RVA: 0x0000FCE9 File Offset: 0x0000DEE9
		public float Defensiveness
		{
			get
			{
				return this._defensiveness;
			}
			set
			{
				if (MathF.Abs(value - this._defensiveness) > 0.0001f)
				{
					this._defensiveness = value;
					this.UpdateAgentProperties();
				}
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000927 RID: 2343 RVA: 0x0000FD0C File Offset: 0x0000DF0C
		// (set) Token: 0x06000928 RID: 2344 RVA: 0x0000FD14 File Offset: 0x0000DF14
		public Formation Formation
		{
			get
			{
				return this._formation;
			}
			set
			{
				if (this._formation != value)
				{
					if (GameNetwork.IsServer && this.HasBeenBuilt && this.Mission.GetMissionBehavior<MissionNetworkComponent>() != null)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new AgentSetFormation(this, (value != null) ? value.Index : (-1)));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
					this.SetNativeFormationNo((value != null) ? value.Index : (-1));
					IDetachment detachment = null;
					float num = 0f;
					if (this._formation != null)
					{
						if (this.IsDetachedFromFormation)
						{
							detachment = this.Detachment;
							num = this.DetachmentWeight;
						}
						this._formation.RemoveUnit(this);
					}
					this._formation = value;
					if (this._formation != null)
					{
						this._formation.AddUnit(this);
						if (detachment != null && this._formation.Detachments.IndexOf(detachment) >= 0 && detachment.IsStandingPointAvailableForAgent(this))
						{
							detachment.AddAgent(this, -1);
							this._formation.DetachUnit(this, detachment.IsLoose);
							this.Detachment = detachment;
							this.DetachmentWeight = num;
						}
					}
					this.UpdateCachedAndFormationValues(this._formation != null && this._formation.PostponeCostlyOperations, false);
				}
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06000929 RID: 2345 RVA: 0x0000FE30 File Offset: 0x0000E030
		IFormationUnit IFormationUnit.FollowedUnit
		{
			get
			{
				if (!this.IsActive())
				{
					return null;
				}
				if (this.IsAIControlled)
				{
					return this.GetFollowedUnit();
				}
				return null;
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x0600092A RID: 2346 RVA: 0x0000FE4C File Offset: 0x0000E04C
		public bool IsShieldUsageEncouraged
		{
			get
			{
				return this.Formation.FiringOrder.OrderEnum == FiringOrder.RangedWeaponUsageOrderEnum.HoldYourFire || !this.Equipment.HasAnyWeaponWithFlags(WeaponFlags.RangedWeapon | WeaponFlags.NotUsableWithOneHand);
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x0600092B RID: 2347 RVA: 0x0000FE74 File Offset: 0x0000E074
		public bool IsPlayerUnit
		{
			get
			{
				return this.IsPlayerControlled || this.IsPlayerTroop;
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x0600092C RID: 2348 RVA: 0x0000FE86 File Offset: 0x0000E086
		// (set) Token: 0x0600092D RID: 2349 RVA: 0x0000FE90 File Offset: 0x0000E090
		public Agent.ControllerType Controller
		{
			get
			{
				return this.GetController();
			}
			set
			{
				Agent.ControllerType controller = this.Controller;
				if (value != controller)
				{
					this.SetController(value);
					if (value == Agent.ControllerType.Player)
					{
						this.Mission.MainAgent = this;
						this.SetAgentFlags(this.GetAgentFlags() | AgentFlag.CanRide);
					}
					if (this.Formation != null)
					{
						this.Formation.OnAgentControllerChanged(this, controller);
					}
					if (value != Agent.ControllerType.AI && this.GetAgentFlags().HasAnyFlag(AgentFlag.IsHumanoid))
					{
						this.SetMaximumSpeedLimit(-1f, false);
						if (this.WalkMode)
						{
							this.EventControlFlags |= Agent.EventControlFlag.Run;
						}
					}
					foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
					{
						missionBehavior.OnAgentControllerChanged(this, controller);
					}
					if (GameNetwork.IsServer)
					{
						MissionPeer missionPeer = this.MissionPeer;
						NetworkCommunicator networkCommunicator = ((missionPeer != null) ? missionPeer.GetNetworkPeer() : null);
						if (networkCommunicator != null && !networkCommunicator.IsServerPeer)
						{
							GameNetwork.BeginModuleEventAsServer(networkCommunicator);
							GameNetwork.WriteMessage(new SetAgentIsPlayer(this, this.Controller != Agent.ControllerType.AI));
							GameNetwork.EndModuleEventAsServer();
						}
					}
				}
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x0600092E RID: 2350 RVA: 0x0000FFB8 File Offset: 0x0000E1B8
		public uint ClothingColor1
		{
			get
			{
				if (this._clothingColor1 != null)
				{
					return this._clothingColor1.Value;
				}
				if (this.Team != null)
				{
					return this.Team.Color;
				}
				Debug.FailedAssert("Clothing color is not set.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Agent.cs", "ClothingColor1", 1090);
				return uint.MaxValue;
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x0600092F RID: 2351 RVA: 0x0001000C File Offset: 0x0000E20C
		public uint ClothingColor2
		{
			get
			{
				if (this._clothingColor2 != null)
				{
					return this._clothingColor2.Value;
				}
				return this.ClothingColor1;
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06000930 RID: 2352 RVA: 0x00010030 File Offset: 0x0000E230
		public MatrixFrame LookFrame
		{
			get
			{
				return new MatrixFrame
				{
					origin = this.Position,
					rotation = this.LookRotation
				};
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06000931 RID: 2353 RVA: 0x00010060 File Offset: 0x0000E260
		// (set) Token: 0x06000932 RID: 2354 RVA: 0x00010072 File Offset: 0x0000E272
		public float LookDirectionAsAngle
		{
			get
			{
				return MBAPI.IMBAgent.GetLookDirectionAsAngle(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetLookDirectionAsAngle(this.GetPtr(), value);
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06000933 RID: 2355 RVA: 0x00010088 File Offset: 0x0000E288
		public Mat3 LookRotation
		{
			get
			{
				Mat3 mat;
				mat.f = this.LookDirection;
				mat.u = Vec3.Up;
				mat.s = Vec3.CrossProduct(mat.f, mat.u);
				mat.s.Normalize();
				mat.u = Vec3.CrossProduct(mat.s, mat.f);
				return mat;
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000934 RID: 2356 RVA: 0x000100EC File Offset: 0x0000E2EC
		// (set) Token: 0x06000935 RID: 2357 RVA: 0x000100FE File Offset: 0x0000E2FE
		public bool IsLookDirectionLocked
		{
			get
			{
				return MBAPI.IMBAgent.GetIsLookDirectionLocked(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetIsLookDirectionLocked(this.GetPtr(), value);
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06000936 RID: 2358 RVA: 0x00010114 File Offset: 0x0000E314
		public bool IsCheering
		{
			get
			{
				bool flag = false;
				ActionIndexValueCache currentActionValue = this.GetCurrentActionValue(1);
				int num = Agent.TauntCheerActions.Length;
				for (int i = 0; i < num; i++)
				{
					if (Agent.TauntCheerActions[i] == currentActionValue)
					{
						flag = true;
						break;
					}
				}
				return flag;
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06000937 RID: 2359 RVA: 0x00010153 File Offset: 0x0000E353
		public bool IsInBeingStruckAction
		{
			get
			{
				return MBMath.IsBetween((int)this.GetCurrentActionType(1), 47, 51) || MBMath.IsBetween((int)this.GetCurrentActionType(0), 47, 51);
			}
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06000938 RID: 2360 RVA: 0x00010179 File Offset: 0x0000E379
		// (set) Token: 0x06000939 RID: 2361 RVA: 0x00010184 File Offset: 0x0000E384
		public MissionPeer MissionPeer
		{
			get
			{
				return this._missionPeer;
			}
			set
			{
				if (this._missionPeer != value)
				{
					MissionPeer missionPeer = this._missionPeer;
					this._missionPeer = value;
					if (missionPeer != null && missionPeer.ControlledAgent == this)
					{
						missionPeer.ControlledAgent = null;
					}
					if (this._missionPeer != null && this._missionPeer.ControlledAgent != this)
					{
						this._missionPeer.ControlledAgent = this;
						if (GameNetwork.IsServerOrRecorder)
						{
							this.SyncHealthToClients();
							Agent.OnAgentHealthChangedDelegate onAgentHealthChanged = this.OnAgentHealthChanged;
							if (onAgentHealthChanged != null)
							{
								onAgentHealthChanged(this, this.Health, this.Health);
							}
						}
					}
					if (value != null)
					{
						this.Controller = (value.IsMine ? Agent.ControllerType.Player : Agent.ControllerType.None);
					}
					if (GameNetwork.IsServer && this.IsHuman && !this._isDeleted)
					{
						NetworkCommunicator networkCommunicator = ((value != null) ? value.GetNetworkPeer() : null);
						this.SetNetworkPeer(networkCommunicator);
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SetAgentPeer(this, networkCommunicator));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
				}
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x0600093A RID: 2362 RVA: 0x00010263 File Offset: 0x0000E463
		// (set) Token: 0x0600093B RID: 2363 RVA: 0x0001026C File Offset: 0x0000E46C
		public BasicCharacterObject Character
		{
			get
			{
				return this._character;
			}
			set
			{
				this._character = value;
				if (value != null)
				{
					this.Health = (float)this._character.HitPoints;
					this.BaseHealthLimit = (float)this._character.MaxHitPoints();
					this.HealthLimit = this.BaseHealthLimit;
					this.CharacterPowerCached = value.GetPower();
					this._name = value.Name;
					this.IsFemale = value.IsFemale;
				}
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x0600093C RID: 2364 RVA: 0x000102D7 File Offset: 0x0000E4D7
		IMissionTeam IAgent.Team
		{
			get
			{
				return this.Team;
			}
		}

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x0600093D RID: 2365 RVA: 0x000102DF File Offset: 0x0000E4DF
		IFormationArrangement IFormationUnit.Formation
		{
			get
			{
				return this._formation.Arrangement;
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x0600093E RID: 2366 RVA: 0x000102EC File Offset: 0x0000E4EC
		// (set) Token: 0x0600093F RID: 2367 RVA: 0x000102F4 File Offset: 0x0000E4F4
		int IFormationUnit.FormationFileIndex { get; set; }

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06000940 RID: 2368 RVA: 0x000102FD File Offset: 0x0000E4FD
		// (set) Token: 0x06000941 RID: 2369 RVA: 0x00010305 File Offset: 0x0000E505
		int IFormationUnit.FormationRankIndex { get; set; }

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06000942 RID: 2370 RVA: 0x0001030E File Offset: 0x0000E50E
		private UIntPtr Pointer
		{
			get
			{
				return this._pointer;
			}
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06000943 RID: 2371 RVA: 0x00010316 File Offset: 0x0000E516
		private UIntPtr FlagsPointer
		{
			get
			{
				return this._flagsPointer;
			}
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06000944 RID: 2372 RVA: 0x0001031E File Offset: 0x0000E51E
		private UIntPtr PositionPointer
		{
			get
			{
				return this._positionPointer;
			}
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x00010328 File Offset: 0x0000E528
		internal Agent(Mission mission, Mission.AgentCreationResult creationResult, Agent.CreationType creationType, Monster monster)
		{
			this.AgentRole = TextObject.Empty;
			this.Mission = mission;
			this.Index = creationResult.Index;
			this._pointer = creationResult.AgentPtr;
			this._positionPointer = creationResult.PositionPtr;
			this._flagsPointer = creationResult.FlagsPtr;
			this._indexPointer = creationResult.IndexPtr;
			this._statePointer = creationResult.StatePtr;
			this._lastHitInfo = default(Agent.AgentLastHitInfo);
			this._lastHitInfo.Initialize();
			MBAPI.IMBAgent.SetMonoObject(this.GetPtr(), this);
			this.Monster = monster;
			this.KillCount = 0;
			this.HasBeenBuilt = false;
			this._creationType = creationType;
			this._agentControllers = new List<AgentController>();
			this._components = new MBList<AgentComponent>();
			this._hitterList = new MBList<Agent.Hitter>();
			((IFormationUnit)this).FormationFileIndex = -1;
			((IFormationUnit)this).FormationRankIndex = -1;
			this._synchedBodyComponents = null;
			this._cachedAndFormationValuesUpdateTimer = new Timer(this.Mission.CurrentTime, 0.45f + MBRandom.RandomFloat * 0.1f, true);
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06000946 RID: 2374 RVA: 0x00010485 File Offset: 0x0000E685
		// (set) Token: 0x06000947 RID: 2375 RVA: 0x00010497 File Offset: 0x0000E697
		public Vec3 LookDirection
		{
			get
			{
				return MBAPI.IMBAgent.GetLookDirection(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetLookDirection(this.GetPtr(), value);
			}
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x000104AA File Offset: 0x0000E6AA
		bool IAgent.IsEnemyOf(IAgent agent)
		{
			return this.IsEnemyOf((Agent)agent);
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x000104B8 File Offset: 0x0000E6B8
		bool IAgent.IsFriendOf(IAgent agent)
		{
			return this.IsFriendOf((Agent)agent);
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x0600094A RID: 2378 RVA: 0x000104C6 File Offset: 0x0000E6C6
		// (set) Token: 0x0600094B RID: 2379 RVA: 0x000104D0 File Offset: 0x0000E6D0
		public float Health
		{
			get
			{
				return this._health;
			}
			set
			{
				float num = (float)(value.ApproximatelyEqualsTo(0f, 1E-05f) ? 0 : MathF.Ceiling(value));
				if (!this._health.ApproximatelyEqualsTo(num, 1E-05f))
				{
					float health = this._health;
					this._health = num;
					if (GameNetwork.IsServerOrRecorder)
					{
						this.SyncHealthToClients();
					}
					Agent.OnAgentHealthChangedDelegate onAgentHealthChanged = this.OnAgentHealthChanged;
					if (onAgentHealthChanged != null)
					{
						onAgentHealthChanged(this, health, this._health);
					}
					if (this.RiderAgent != null)
					{
						Agent.OnMountHealthChangedDelegate onMountHealthChanged = this.RiderAgent.OnMountHealthChanged;
						if (onMountHealthChanged == null)
						{
							return;
						}
						onMountHealthChanged(this.RiderAgent, this, health, this._health);
					}
				}
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x0600094C RID: 2380 RVA: 0x0001056C File Offset: 0x0000E76C
		// (set) Token: 0x0600094D RID: 2381 RVA: 0x00010588 File Offset: 0x0000E788
		public float Age
		{
			get
			{
				return this.BodyPropertiesValue.Age;
			}
			set
			{
				this.BodyPropertiesValue = new BodyProperties(new DynamicBodyProperties(value, this.BodyPropertiesValue.Weight, this.BodyPropertiesValue.Build), this.BodyPropertiesValue.StaticProperties);
				BodyProperties bodyPropertiesValue = this.BodyPropertiesValue;
				this.BodyPropertiesValue = bodyPropertiesValue;
			}
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x000105DE File Offset: 0x0000E7DE
		Vec3 ITrackableBase.GetPosition()
		{
			return this.Position;
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x0600094F RID: 2383 RVA: 0x000105E8 File Offset: 0x0000E7E8
		public Vec3 Velocity
		{
			get
			{
				Vec2 movementVelocity = MBAPI.IMBAgent.GetMovementVelocity(this.GetPtr());
				Vec3 vec = new Vec3(movementVelocity, 0f, -1f);
				return this.Frame.rotation.TransformToParent(vec);
			}
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x0001062C File Offset: 0x0000E82C
		TextObject ITrackableBase.GetName()
		{
			if (this.Character != null)
			{
				return new TextObject(this.Character.Name.ToString(), null);
			}
			return TextObject.Empty;
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x00010652 File Offset: 0x0000E852
		[MBCallback]
		internal void SetAgentAIPerformingRetreatBehavior(bool isAgentAIPerformingRetreatBehavior)
		{
			if (!GameNetwork.IsClientOrReplay && this.Mission != null)
			{
				this.IsRunningAway = isAgentAIPerformingRetreatBehavior;
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06000952 RID: 2386 RVA: 0x0001066A File Offset: 0x0000E86A
		// (set) Token: 0x06000953 RID: 2387 RVA: 0x0001067C File Offset: 0x0000E87C
		public Agent.EventControlFlag EventControlFlags
		{
			get
			{
				return (Agent.EventControlFlag)MBAPI.IMBAgent.GetEventControlFlags(this.GetPtr());
			}
			set
			{
				MBAPI.IMBAgent.SetEventControlFlags(this.GetPtr(), value);
			}
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x0001068F File Offset: 0x0000E88F
		[MBCallback]
		public float GetMissileRangeWithHeightDifferenceAux(float targetZ)
		{
			return MBAPI.IMBAgent.GetMissileRangeWithHeightDifference(this.GetPtr(), targetZ);
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x000106A2 File Offset: 0x0000E8A2
		[MBCallback]
		internal int GetFormationUnitSpacing()
		{
			return this.Formation.UnitSpacing;
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x000106AF File Offset: 0x0000E8AF
		[MBCallback]
		public string GetSoundAndCollisionInfoClassName()
		{
			return this.Monster.SoundAndCollisionInfoClassName;
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x000106BC File Offset: 0x0000E8BC
		[MBCallback]
		internal bool IsInSameFormationWith(Agent otherAgent)
		{
			Formation formation = otherAgent.Formation;
			return this.Formation != null && formation != null && this.Formation == formation;
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x000106E6 File Offset: 0x0000E8E6
		[MBCallback]
		internal void OnWeaponSwitchingToAlternativeStart(EquipmentIndex slotIndex, int usageIndex)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new StartSwitchingWeaponUsageIndex(this, slotIndex, usageIndex, Agent.MovementFlagToDirection(this.MovementFlags)));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x00010714 File Offset: 0x0000E914
		[MBCallback]
		internal void OnWeaponReloadPhaseChange(EquipmentIndex slotIndex, short reloadPhase)
		{
			this.Equipment.SetReloadPhaseOfSlot(slotIndex, reloadPhase);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetWeaponReloadPhase(this, slotIndex, reloadPhase));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x00010744 File Offset: 0x0000E944
		[MBCallback]
		internal void OnWeaponAmmoReload(EquipmentIndex slotIndex, EquipmentIndex ammoSlotIndex, short totalAmmo)
		{
			if (this.Equipment[slotIndex].CurrentUsageItem.IsRangedWeapon)
			{
				this.Equipment.SetReloadedAmmoOfSlot(slotIndex, ammoSlotIndex, totalAmmo);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetWeaponAmmoData(this, slotIndex, ammoSlotIndex, totalAmmo));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
			this.UpdateAgentProperties();
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x000107A4 File Offset: 0x0000E9A4
		[MBCallback]
		internal void OnWeaponAmmoConsume(EquipmentIndex slotIndex, short totalAmmo)
		{
			if (this.Equipment[slotIndex].CurrentUsageItem.IsRangedWeapon)
			{
				this.Equipment.SetConsumedAmmoOfSlot(slotIndex, totalAmmo);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetWeaponAmmoData(this, slotIndex, EquipmentIndex.None, totalAmmo));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
			this.UpdateAgentProperties();
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x0600095C RID: 2396 RVA: 0x00010801 File Offset: 0x0000EA01
		// (set) Token: 0x0600095D RID: 2397 RVA: 0x0001080E File Offset: 0x0000EA0E
		public AgentState State
		{
			get
			{
				return AgentHelper.GetAgentState(this._statePointer);
			}
			set
			{
				if (this.State != value)
				{
					MBAPI.IMBAgent.SetStateFlags(this.GetPtr(), value);
				}
			}
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x0001082C File Offset: 0x0000EA2C
		[MBCallback]
		internal void OnShieldDamaged(EquipmentIndex slotIndex, int inflictedDamage)
		{
			int num = MathF.Max(0, (int)this.Equipment[slotIndex].HitPoints - inflictedDamage);
			this.ChangeWeaponHitPoints(slotIndex, (short)num);
			if (num == 0)
			{
				this.RemoveEquippedWeapon(slotIndex);
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x0001086C File Offset: 0x0000EA6C
		public MissionWeapon WieldedOffhandWeapon
		{
			get
			{
				EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
				if (wieldedItemIndex < EquipmentIndex.WeaponItemBeginSlot)
				{
					return MissionWeapon.Invalid;
				}
				return this.Equipment[wieldedItemIndex];
			}
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x00010898 File Offset: 0x0000EA98
		[MBCallback]
		internal void OnWeaponAmmoRemoved(EquipmentIndex slotIndex)
		{
			if (!this.Equipment[slotIndex].AmmoWeapon.IsEmpty)
			{
				this.Equipment.SetConsumedAmmoOfSlot(slotIndex, 0);
			}
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x000108D0 File Offset: 0x0000EAD0
		[MBCallback]
		internal void OnMount(Agent mount)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				if (mount.IsAIControlled && mount.IsRetreating(false))
				{
					mount.StopRetreatingMoraleComponent();
				}
				this.CheckToDropFlaggedItem();
			}
			if (this.HasBeenBuilt)
			{
				foreach (AgentComponent agentComponent in this._components)
				{
					agentComponent.OnMount(mount);
				}
				this.Mission.OnAgentMount(this);
			}
			this.UpdateAgentStats();
			Action onAgentMountedStateChanged = this.OnAgentMountedStateChanged;
			if (onAgentMountedStateChanged != null)
			{
				onAgentMountedStateChanged();
			}
			if (GameNetwork.IsServerOrRecorder)
			{
				mount.SyncHealthToClients();
			}
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x00010980 File Offset: 0x0000EB80
		[MBCallback]
		internal void OnDismount(Agent mount)
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				Formation formation = this.Formation;
				if (formation != null)
				{
					formation.OnAgentLostMount(this);
				}
				this.CheckToDropFlaggedItem();
			}
			foreach (AgentComponent agentComponent in this._components)
			{
				agentComponent.OnDismount(mount);
			}
			this.Mission.OnAgentDismount(this);
			if (this.IsActive())
			{
				this.UpdateAgentStats();
				Action onAgentMountedStateChanged = this.OnAgentMountedStateChanged;
				if (onAgentMountedStateChanged == null)
				{
					return;
				}
				onAgentMountedStateChanged();
			}
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x00010A1C File Offset: 0x0000EC1C
		[MBCallback]
		internal void OnAgentAlarmedStateChanged(Agent.AIStateFlag flag)
		{
			foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
			{
				missionBehavior.OnAgentAlarmedStateChanged(this, flag);
			}
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x00010A74 File Offset: 0x0000EC74
		[MBCallback]
		internal void OnRetreating()
		{
			if (!GameNetwork.IsClientOrReplay && this.Mission != null && !this.Mission.MissionEnded)
			{
				if (this.IsUsingGameObject)
				{
					this.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
				}
				foreach (AgentComponent agentComponent in this._components)
				{
					agentComponent.OnRetreating();
				}
			}
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x00010AF0 File Offset: 0x0000ECF0
		[MBCallback]
		internal void UpdateMountAgentCache(Agent newMountAgent)
		{
			this._cachedMountAgent = newMountAgent;
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x00010AF9 File Offset: 0x0000ECF9
		[MBCallback]
		internal void UpdateRiderAgentCache(Agent newRiderAgent)
		{
			this._cachedRiderAgent = newRiderAgent;
			if (newRiderAgent == null)
			{
				Mission.Current.AddMountWithoutRider(this);
				return;
			}
			Mission.Current.RemoveMountWithoutRider(this);
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x00010B1C File Offset: 0x0000ED1C
		[MBCallback]
		public void UpdateAgentStats()
		{
			if (this.IsActive())
			{
				this.UpdateAgentProperties();
			}
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x00010B2C File Offset: 0x0000ED2C
		[MBCallback]
		public float GetWeaponInaccuracy(EquipmentIndex weaponSlotIndex, int weaponUsageIndex)
		{
			WeaponComponentData weaponComponentDataForUsage = this.Equipment[weaponSlotIndex].GetWeaponComponentDataForUsage(weaponUsageIndex);
			return MissionGameModels.Current.AgentStatCalculateModel.GetWeaponInaccuracy(this, weaponComponentDataForUsage, MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(this.Character, this.Origin, this.Formation, weaponComponentDataForUsage.RelevantSkill));
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x00010B87 File Offset: 0x0000ED87
		[MBCallback]
		public float DebugGetHealth()
		{
			return this.Health;
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x00010B8F File Offset: 0x0000ED8F
		public void SetTargetPosition(Vec2 value)
		{
			MBAPI.IMBAgent.SetTargetPosition(this.GetPtr(), ref value);
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x00010BA3 File Offset: 0x0000EDA3
		public void SetGuardState(Agent guardedAgent, bool isGuarding)
		{
			if (isGuarding)
			{
				this.AIStateFlags |= Agent.AIStateFlag.Guard;
			}
			else
			{
				this.AIStateFlags &= ~Agent.AIStateFlag.Guard;
			}
			this.SetGuardedAgent(guardedAgent);
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x00010BCF File Offset: 0x0000EDCF
		public void SetCanLeadFormationsRemotely(bool value)
		{
			this._canLeadFormationsRemotely = value;
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x00010BD8 File Offset: 0x0000EDD8
		public void SetAveragePingInMilliseconds(double averagePingInMilliseconds)
		{
			MBAPI.IMBAgent.SetAveragePingInMilliseconds(this.GetPtr(), averagePingInMilliseconds);
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x00010BEB File Offset: 0x0000EDEB
		public void SetTargetPositionAndDirection(Vec2 targetPosition, Vec3 targetDirection)
		{
			MBAPI.IMBAgent.SetTargetPositionAndDirection(this.GetPtr(), ref targetPosition, ref targetDirection);
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x00010C01 File Offset: 0x0000EE01
		public void SetWatchState(Agent.WatchState watchState)
		{
			this.CurrentWatchState = watchState;
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x00010C0C File Offset: 0x0000EE0C
		[MBCallback]
		internal void OnWieldedItemIndexChange(bool isOffHand, bool isWieldedInstantly, bool isWieldedOnSpawn)
		{
			if (this.IsMainAgent)
			{
				Agent.OnMainAgentWieldedItemChangeDelegate onMainAgentWieldedItemChange = this.OnMainAgentWieldedItemChange;
				if (onMainAgentWieldedItemChange != null)
				{
					onMainAgentWieldedItemChange();
				}
			}
			Action onAgentWieldedItemChange = this.OnAgentWieldedItemChange;
			if (onAgentWieldedItemChange != null)
			{
				onAgentWieldedItemChange();
			}
			if (GameNetwork.IsServerOrRecorder)
			{
				int num = 0;
				EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
				if (wieldedItemIndex != EquipmentIndex.None)
				{
					num = this.Equipment[wieldedItemIndex].CurrentUsageIndex;
				}
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetWieldedItemIndex(this, isOffHand, isWieldedInstantly, isWieldedOnSpawn, this.GetWieldedItemIndex(isOffHand ? Agent.HandIndex.OffHand : Agent.HandIndex.MainHand), num));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.CheckEquipmentForCapeClothSimulationStateChange();
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x00010C99 File Offset: 0x0000EE99
		public void SetFormationBanner(ItemObject banner)
		{
			this._formationBanner = banner;
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x00010CA2 File Offset: 0x0000EEA2
		public void SetIsAIPaused(bool isPaused)
		{
			this.IsPaused = isPaused;
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x00010CAB File Offset: 0x0000EEAB
		public void ResetEnemyCaches()
		{
			MBAPI.IMBAgent.ResetEnemyCaches(this.GetPtr());
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x00010CC0 File Offset: 0x0000EEC0
		public void SetTargetPositionSynched(ref Vec2 targetPosition)
		{
			if (this.MovementLockedState == AgentMovementLockedState.None || this.GetTargetPosition() != targetPosition)
			{
				if (GameNetwork.IsClientOrReplay)
				{
					this._lastSynchedTargetPosition = targetPosition;
					this._checkIfTargetFrameIsChanged = true;
					return;
				}
				this.SetTargetPosition(targetPosition);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetAgentTargetPosition(this, ref targetPosition));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x00010D30 File Offset: 0x0000EF30
		public void SetTargetPositionAndDirectionSynched(ref Vec2 targetPosition, ref Vec3 targetDirection)
		{
			if (this.MovementLockedState == AgentMovementLockedState.None || this.GetTargetDirection() != targetDirection)
			{
				if (GameNetwork.IsClientOrReplay)
				{
					this._lastSynchedTargetDirection = targetDirection;
					this._checkIfTargetFrameIsChanged = true;
					return;
				}
				this.SetTargetPositionAndDirection(targetPosition, targetDirection);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetAgentTargetPositionAndDirection(this, ref targetPosition, ref targetDirection));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x00010DA6 File Offset: 0x0000EFA6
		public void SetBodyArmorMaterialType(ArmorComponent.ArmorMaterialTypes bodyArmorMaterialType)
		{
			MBAPI.IMBAgent.SetBodyArmorMaterialType(this.GetPtr(), bodyArmorMaterialType);
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x00010DB9 File Offset: 0x0000EFB9
		public void SetUsedGameObjectForClient(UsableMissionObject usedObject)
		{
			this.CurrentlyUsedGameObject = usedObject;
			usedObject.OnUse(this);
			this.Mission.OnObjectUsed(this, usedObject);
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x00010DD8 File Offset: 0x0000EFD8
		public void SetTeam(Team team, bool sync)
		{
			if (this.Team != team)
			{
				Team team2 = this.Team;
				Team team3 = this.Team;
				if (team3 != null)
				{
					team3.RemoveAgentFromTeam(this);
				}
				this.Team = team;
				Team team4 = this.Team;
				if (team4 != null)
				{
					team4.AddAgentToTeam(this);
				}
				this.SetTeamInternal((team != null) ? team.MBTeam : MBTeam.InvalidTeam);
				if (sync && GameNetwork.IsServer && this.Mission.HasMissionBehavior<MissionNetworkComponent>())
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new AgentSetTeam(this, team));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
				{
					missionBehavior.OnAgentTeamChanged(team2, team, this);
				}
			}
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x00010EB4 File Offset: 0x0000F0B4
		public void SetClothingColor1(uint color)
		{
			this._clothingColor1 = new uint?(color);
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x00010EC2 File Offset: 0x0000F0C2
		public void SetClothingColor2(uint color)
		{
			this._clothingColor2 = new uint?(color);
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x00010ED0 File Offset: 0x0000F0D0
		public void SetWieldedItemIndexAsClient(Agent.HandIndex handIndex, EquipmentIndex equipmentIndex, bool isWieldedInstantly, bool isWieldedOnSpawn, int mainHandCurrentUsageIndex)
		{
			MBAPI.IMBAgent.SetWieldedItemIndexAsClient(this.GetPtr(), (int)handIndex, (int)equipmentIndex, isWieldedInstantly, isWieldedOnSpawn, mainHandCurrentUsageIndex);
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x00010EE9 File Offset: 0x0000F0E9
		public void SetAsConversationAgent(bool set)
		{
			if (set)
			{
				this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.InConversation);
				this.DisableLookToPointOfInterest();
				return;
			}
			this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.InConversation);
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x00010F19 File Offset: 0x0000F119
		public void SetCrouchMode(bool set)
		{
			if (set)
			{
				this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.Crouch);
				return;
			}
			this.SetScriptedFlags(this.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.Crouch);
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x00010F43 File Offset: 0x0000F143
		public void SetWeaponAmountInSlot(EquipmentIndex equipmentSlot, short amount, bool enforcePrimaryItem)
		{
			MBAPI.IMBAgent.SetWeaponAmountInSlot(this.GetPtr(), (int)equipmentSlot, amount, enforcePrimaryItem);
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x00010F58 File Offset: 0x0000F158
		public void SetWeaponAmmoAsClient(EquipmentIndex equipmentIndex, EquipmentIndex ammoEquipmentIndex, short ammo)
		{
			MBAPI.IMBAgent.SetWeaponAmmoAsClient(this.GetPtr(), (int)equipmentIndex, (int)ammoEquipmentIndex, ammo);
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x00010F6D File Offset: 0x0000F16D
		public void SetWeaponReloadPhaseAsClient(EquipmentIndex equipmentIndex, short reloadState)
		{
			MBAPI.IMBAgent.SetWeaponReloadPhaseAsClient(this.GetPtr(), (int)equipmentIndex, reloadState);
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x00010F81 File Offset: 0x0000F181
		public void SetReloadAmmoInSlot(EquipmentIndex equipmentIndex, EquipmentIndex ammoSlotIndex, short reloadedAmmo)
		{
			MBAPI.IMBAgent.SetReloadAmmoInSlot(this.GetPtr(), (int)equipmentIndex, (int)ammoSlotIndex, reloadedAmmo);
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x00010F96 File Offset: 0x0000F196
		public void SetUsageIndexOfWeaponInSlotAsClient(EquipmentIndex slotIndex, int usageIndex)
		{
			MBAPI.IMBAgent.SetUsageIndexOfWeaponInSlotAsClient(this.GetPtr(), (int)slotIndex, usageIndex);
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x00010FAA File Offset: 0x0000F1AA
		public void SetRandomizeColors(bool shouldRandomize)
		{
			this.RandomizeColors = shouldRandomize;
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x00010FB3 File Offset: 0x0000F1B3
		public void SetAlwaysAttackInMelee(bool attack)
		{
			MBAPI.IMBAgent.SetAlwaysAttackInMelee(this.GetPtr(), attack);
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x00010FC6 File Offset: 0x0000F1C6
		[MBCallback]
		internal void OnRemoveWeapon(EquipmentIndex slotIndex)
		{
			this.RemoveEquippedWeapon(slotIndex);
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x00010FCF File Offset: 0x0000F1CF
		public void SetFormationFrameDisabled()
		{
			MBAPI.IMBAgent.SetFormationFrameDisabled(this.GetPtr());
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x00010FE1 File Offset: 0x0000F1E1
		public void SetFormationFrameEnabled(WorldPosition position, Vec2 direction, float formationDirectionEnforcingFactor)
		{
			MBAPI.IMBAgent.SetFormationFrameEnabled(this.GetPtr(), position, direction, formationDirectionEnforcingFactor);
			if (this.Mission.IsTeleportingAgents)
			{
				this.TeleportToPosition(position.GetGroundVec3());
			}
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x00011011 File Offset: 0x0000F211
		public void SetShouldCatchUpWithFormation(bool value)
		{
			MBAPI.IMBAgent.SetShouldCatchUpWithFormation(this.GetPtr(), value);
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x00011024 File Offset: 0x0000F224
		public void SetFormationIntegrityData(Vec2 position, Vec2 currentFormationDirection, Vec2 averageVelocityOfCloseAgents, float averageMaxUnlimitedSpeedOfCloseAgents, float deviationOfPositions)
		{
			MBAPI.IMBAgent.SetFormationIntegrityData(this.GetPtr(), position, currentFormationDirection, averageVelocityOfCloseAgents, averageMaxUnlimitedSpeedOfCloseAgents, deviationOfPositions);
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x00011040 File Offset: 0x0000F240
		public void SetGuardedAgent(Agent guardedAgent)
		{
			int num = ((guardedAgent != null) ? guardedAgent.Index : (-1));
			MBAPI.IMBAgent.SetGuardedAgentIndex(this.GetPtr(), num);
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x0001106B File Offset: 0x0000F26B
		[MBCallback]
		internal void OnWeaponUsageIndexChange(EquipmentIndex slotIndex, int usageIndex)
		{
			this.Equipment.SetUsageIndexOfSlot(slotIndex, usageIndex);
			this.UpdateAgentProperties();
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new WeaponUsageIndexChangeMessage(this, slotIndex, usageIndex));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x000110A1 File Offset: 0x0000F2A1
		public void SetCurrentActionProgress(int channelNo, float progress)
		{
			MBAPI.IMBAgent.SetCurrentActionProgress(this.GetPtr(), channelNo, progress);
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x000110B5 File Offset: 0x0000F2B5
		public void SetCurrentActionSpeed(int channelNo, float speed)
		{
			MBAPI.IMBAgent.SetCurrentActionSpeed(this.GetPtr(), channelNo, speed);
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x000110CC File Offset: 0x0000F2CC
		public bool SetActionChannel(int channelNo, ActionIndexCache actionIndexCache, bool ignorePriority = false, ulong additionalFlags = 0UL, float blendWithNextActionFactor = 0f, float actionSpeed = 1f, float blendInPeriod = -0.2f, float blendOutPeriodToNoAnim = 0.4f, float startProgress = 0f, bool useLinearSmoothing = false, float blendOutPeriod = -0.2f, int actionShift = 0, bool forceFaceMorphRestart = true)
		{
			int index = actionIndexCache.Index;
			return MBAPI.IMBAgent.SetActionChannel(this.GetPtr(), channelNo, index + actionShift, additionalFlags, ignorePriority, blendWithNextActionFactor, actionSpeed, blendInPeriod, blendOutPeriodToNoAnim, startProgress, useLinearSmoothing, blendOutPeriod, forceFaceMorphRestart);
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x00011108 File Offset: 0x0000F308
		public bool SetActionChannel(int channelNo, ActionIndexValueCache actionIndexCache, bool ignorePriority = false, ulong additionalFlags = 0UL, float blendWithNextActionFactor = 0f, float actionSpeed = 1f, float blendInPeriod = -0.2f, float blendOutPeriodToNoAnim = 0.4f, float startProgress = 0f, bool useLinearSmoothing = false, float blendOutPeriod = -0.2f, int actionShift = 0, bool forceFaceMorphRestart = true)
		{
			int index = actionIndexCache.Index;
			return MBAPI.IMBAgent.SetActionChannel(this.GetPtr(), channelNo, index + actionShift, additionalFlags, ignorePriority, blendWithNextActionFactor, actionSpeed, blendInPeriod, blendOutPeriodToNoAnim, startProgress, useLinearSmoothing, blendOutPeriod, forceFaceMorphRestart);
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x00011145 File Offset: 0x0000F345
		[MBCallback]
		internal void OnWeaponAmountChange(EquipmentIndex slotIndex, short amount)
		{
			this.Equipment.SetAmountOfSlot(slotIndex, amount, false);
			this.UpdateAgentProperties();
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetWeaponNetworkData(this, slotIndex, amount));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x0001117C File Offset: 0x0000F37C
		public void SetMinimumSpeed(float speed)
		{
			MBAPI.IMBAgent.SetMinimumSpeed(this.GetPtr(), speed);
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x0001118F File Offset: 0x0000F38F
		public void SetAttackState(int attackState)
		{
			MBAPI.IMBAgent.SetAttackState(this.GetPtr(), attackState);
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x000111A2 File Offset: 0x0000F3A2
		public void SetAIBehaviorParams(HumanAIComponent.AISimpleBehaviorKind behavior, float y1, float x2, float y2, float x3, float y3)
		{
			MBAPI.IMBAgent.SetAIBehaviorParams(this.GetPtr(), (int)behavior, y1, x2, y2, x3, y3);
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x000111BD File Offset: 0x0000F3BD
		public void SetAllBehaviorParams(HumanAIComponent.BehaviorValues[] behaviorParams)
		{
			MBAPI.IMBAgent.SetAllAIBehaviorParams(this.GetPtr(), behaviorParams);
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x000111D0 File Offset: 0x0000F3D0
		public void SetMovementDirection(in Vec2 direction)
		{
			MBAPI.IMBAgent.SetMovementDirection(this.GetPtr(), direction);
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x000111E3 File Offset: 0x0000F3E3
		public void SetScriptedFlags(Agent.AIScriptedFrameFlags flags)
		{
			MBAPI.IMBAgent.SetScriptedFlags(this.GetPtr(), (int)flags);
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x000111F6 File Offset: 0x0000F3F6
		public void GetDebugValues(float[] values, ref int valueCount)
		{
			MBAPI.IMBAgent.GetDebugValues(this.GetPtr(), values, ref valueCount);
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x0001120A File Offset: 0x0000F40A
		public void SetScriptedCombatFlags(Agent.AISpecialCombatModeFlags flags)
		{
			MBAPI.IMBAgent.SetScriptedCombatFlags(this.GetPtr(), (int)flags);
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x00011220 File Offset: 0x0000F420
		public void SetScriptedPositionAndDirection(ref WorldPosition scriptedPosition, float scriptedDirection, bool addHumanLikeDelay, Agent.AIScriptedFrameFlags additionalFlags = Agent.AIScriptedFrameFlags.None)
		{
			MBAPI.IMBAgent.SetScriptedPositionAndDirection(this.GetPtr(), ref scriptedPosition, scriptedDirection, addHumanLikeDelay, (int)additionalFlags);
			if (this.Mission.IsTeleportingAgents && scriptedPosition.AsVec2 != this.Position.AsVec2)
			{
				this.TeleportToPosition(scriptedPosition.GetGroundVec3());
			}
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x00011278 File Offset: 0x0000F478
		public void SetScriptedPosition(ref WorldPosition position, bool addHumanLikeDelay, Agent.AIScriptedFrameFlags additionalFlags = Agent.AIScriptedFrameFlags.None)
		{
			MBAPI.IMBAgent.SetScriptedPosition(this.GetPtr(), ref position, addHumanLikeDelay, (int)additionalFlags);
			if (this.Mission.IsTeleportingAgents && position.AsVec2 != this.Position.AsVec2)
			{
				this.TeleportToPosition(position.GetGroundVec3());
			}
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x000112CD File Offset: 0x0000F4CD
		public void SetScriptedTargetEntityAndPosition(GameEntity target, WorldPosition position, Agent.AISpecialCombatModeFlags additionalFlags = Agent.AISpecialCombatModeFlags.None, bool ignoreIfAlreadyAttacking = false)
		{
			MBAPI.IMBAgent.SetScriptedTargetEntity(this.GetPtr(), target.Pointer, ref position, (int)additionalFlags, ignoreIfAlreadyAttacking);
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x000112EA File Offset: 0x0000F4EA
		public void SetAgentExcludeStateForFaceGroupId(int faceGroupId, bool isExcluded)
		{
			MBAPI.IMBAgent.SetAgentExcludeStateForFaceGroupId(this.GetPtr(), faceGroupId, isExcluded);
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x000112FE File Offset: 0x0000F4FE
		public void SetLookAgent(Agent agent)
		{
			this._lookAgentCache = agent;
			MBAPI.IMBAgent.SetLookAgent(this.GetPtr(), (agent != null) ? agent.GetPtr() : UIntPtr.Zero);
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x00011327 File Offset: 0x0000F527
		public void SetInteractionAgent(Agent agent)
		{
			MBAPI.IMBAgent.SetInteractionAgent(this.GetPtr(), (agent != null) ? agent.GetPtr() : UIntPtr.Zero);
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00011349 File Offset: 0x0000F549
		public void SetLookToPointOfInterest(Vec3 point)
		{
			MBAPI.IMBAgent.SetLookToPointOfInterest(this.GetPtr(), point);
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x0001135C File Offset: 0x0000F55C
		public void SetAgentFlags(AgentFlag agentFlags)
		{
			MBAPI.IMBAgent.SetAgentFlags(this.GetPtr(), (uint)agentFlags);
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x0001136F File Offset: 0x0000F56F
		public void SetSelectedMountIndex(int mountIndex)
		{
			MBAPI.IMBAgent.SetSelectedMountIndex(this.GetPtr(), mountIndex);
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x00011382 File Offset: 0x0000F582
		public int GetSelectedMountIndex()
		{
			return MBAPI.IMBAgent.GetSelectedMountIndex(this.GetPtr());
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x00011394 File Offset: 0x0000F594
		public int GetRidingOrder()
		{
			return MBAPI.IMBAgent.GetRidingOrder(this.GetPtr());
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x000113A6 File Offset: 0x0000F5A6
		public void SetAgentFacialAnimation(Agent.FacialAnimChannel channel, string animationName, bool loop)
		{
			MBAPI.IMBAgent.SetAgentFacialAnimation(this.GetPtr(), (int)channel, animationName, loop);
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x000113BB File Offset: 0x0000F5BB
		public bool SetHandInverseKinematicsFrame(ref MatrixFrame leftGlobalFrame, ref MatrixFrame rightGlobalFrame)
		{
			return MBAPI.IMBAgent.SetHandInverseKinematicsFrame(this.GetPtr(), ref leftGlobalFrame, ref rightGlobalFrame);
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x000113CF File Offset: 0x0000F5CF
		public void SetNativeFormationNo(int formationNo)
		{
			MBAPI.IMBAgent.SetFormationNo(this.GetPtr(), formationNo);
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x000113E2 File Offset: 0x0000F5E2
		public void SetDirectionChangeTendency(float tendency)
		{
			MBAPI.IMBAgent.SetDirectionChangeTendency(this.GetPtr(), tendency);
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x000113F5 File Offset: 0x0000F5F5
		public void SetFiringOrder(int order)
		{
			MBAPI.IMBAgent.SetFiringOrder(this.GetPtr(), order);
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x00011408 File Offset: 0x0000F608
		public void SetSoundOcclusion(float value)
		{
			MBAPI.IMBAgent.SetSoundOcclusion(this.GetPtr(), value);
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x0001141B File Offset: 0x0000F61B
		public void SetRidingOrder(int order)
		{
			MBAPI.IMBAgent.SetRidingOrder(this.GetPtr(), order);
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x00011430 File Offset: 0x0000F630
		public float GetBattleImportance()
		{
			BasicCharacterObject character = this.Character;
			float num = ((character != null) ? character.GetBattlePower() : 1f);
			if (this.Team != null && this == this.Team.GeneralAgent)
			{
				num *= 2f;
			}
			else if (this.Formation != null && this == this.Formation.Captain)
			{
				num *= 1.2f;
			}
			return num;
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00011493 File Offset: 0x0000F693
		public void SetSynchedPrefabComponentVisibility(int componentIndex, bool visibility)
		{
			this._synchedBodyComponents[componentIndex].SetVisible(visibility);
			this.AgentVisuals.LazyUpdateAgentRendererData();
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetAgentPrefabComponentVisibility(this, componentIndex, visibility));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x000114D3 File Offset: 0x0000F6D3
		public void SetActionSet(ref AnimationSystemData animationSystemData)
		{
			MBAPI.IMBAgent.SetActionSet(this.GetPtr(), ref animationSystemData);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetAgentActionSet(this, animationSystemData));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x0001150C File Offset: 0x0000F70C
		public void SetColumnwiseFollowAgent(Agent followAgent, ref Vec2 followPosition)
		{
			if (!this.IsAIControlled)
			{
				return;
			}
			int num = ((followAgent != null) ? followAgent.Index : (-1));
			MBAPI.IMBAgent.SetColumnwiseFollowAgent(this.GetPtr(), num, ref followPosition);
			this.SetFollowedUnit(followAgent);
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x00011548 File Offset: 0x0000F748
		public void SetHandInverseKinematicsFrameForMissionObjectUsage(in MatrixFrame localIKFrame, in MatrixFrame boundEntityGlobalFrame, float animationHeightDifference = 0f)
		{
			if (this.GetCurrentActionValue(1) != ActionIndexValueCache.act_none && this.GetActionChannelWeight(1) > 0f)
			{
				MBAPI.IMBAgent.SetHandInverseKinematicsFrameForMissionObjectUsage(this.GetPtr(), localIKFrame, boundEntityGlobalFrame, animationHeightDifference);
				return;
			}
			this.ClearHandInverseKinematics();
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x00011586 File Offset: 0x0000F786
		public void SetWantsToYell()
		{
			this._wantsToYell = true;
			this._yellTimer = MBRandom.RandomFloat * 0.3f + 0.1f;
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x000115A8 File Offset: 0x0000F7A8
		public void SetCapeClothSimulator(GameEntityComponent clothSimulatorComponent)
		{
			ClothSimulatorComponent clothSimulatorComponent2 = clothSimulatorComponent as ClothSimulatorComponent;
			this._capeClothSimulator = clothSimulatorComponent2;
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x000115C3 File Offset: 0x0000F7C3
		public Vec2 GetTargetPosition()
		{
			return MBAPI.IMBAgent.GetTargetPosition(this.GetPtr());
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x000115D5 File Offset: 0x0000F7D5
		public Vec3 GetTargetDirection()
		{
			return MBAPI.IMBAgent.GetTargetDirection(this.GetPtr());
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x000115E7 File Offset: 0x0000F7E7
		public float GetAimingTimer()
		{
			return MBAPI.IMBAgent.GetAimingTimer(this.GetPtr());
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x000115FC File Offset: 0x0000F7FC
		public float GetInteractionDistanceToUsable(IUsable usable)
		{
			Agent agent;
			if ((agent = usable as Agent) != null)
			{
				if (!agent.IsMount)
				{
					return 3f;
				}
				return 1.75f;
			}
			else
			{
				SpawnedItemEntity spawnedItemEntity;
				if ((spawnedItemEntity = usable as SpawnedItemEntity) != null && spawnedItemEntity.IsBanner())
				{
					return 3f;
				}
				float interactionDistance = MissionGameModels.Current.AgentStatCalculateModel.GetInteractionDistance(this);
				if (!(usable is StandingPoint))
				{
					return interactionDistance;
				}
				if (!this.IsAIControlled || !this.WalkMode)
				{
					return 1f;
				}
				return 0.5f;
			}
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x00011675 File Offset: 0x0000F875
		public TextObject GetInfoTextForBeingNotInteractable(Agent userAgent)
		{
			if (this.IsMount && !userAgent.CheckSkillForMounting(this))
			{
				return GameTexts.FindText("str_ui_riding_skill_not_adequate_to_mount", null);
			}
			return TextObject.Empty;
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x0001169C File Offset: 0x0000F89C
		public T GetController<T>() where T : AgentController
		{
			for (int i = 0; i < this._agentControllers.Count; i++)
			{
				if (this._agentControllers[i] is T)
				{
					return (T)((object)this._agentControllers[i]);
				}
			}
			return default(T);
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x000116ED File Offset: 0x0000F8ED
		public EquipmentIndex GetWieldedItemIndex(Agent.HandIndex index)
		{
			return MBAPI.IMBAgent.GetWieldedItemIndex(this.GetPtr(), (int)index);
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x00011700 File Offset: 0x0000F900
		public float GetTrackDistanceToMainAgent()
		{
			float num = -1f;
			if (Agent.Main != null)
			{
				num = Agent.Main.Position.Distance(this.Position);
			}
			return num;
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x00011734 File Offset: 0x0000F934
		public string GetDescriptionText(GameEntity gameEntity = null)
		{
			return this.Name;
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x0001173C File Offset: 0x0000F93C
		public GameEntity GetWeaponEntityFromEquipmentSlot(EquipmentIndex slotIndex)
		{
			return new GameEntity(MBAPI.IMBAgent.GetWeaponEntityFromEquipmentSlot(this.GetPtr(), (int)slotIndex));
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x00011754 File Offset: 0x0000F954
		public WorldPosition GetRetreatPos()
		{
			return MBAPI.IMBAgent.GetRetreatPos(this.GetPtr());
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x00011766 File Offset: 0x0000F966
		public Agent.AIScriptedFrameFlags GetScriptedFlags()
		{
			return (Agent.AIScriptedFrameFlags)MBAPI.IMBAgent.GetScriptedFlags(this.GetPtr());
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x00011778 File Offset: 0x0000F978
		public Agent.AISpecialCombatModeFlags GetScriptedCombatFlags()
		{
			return (Agent.AISpecialCombatModeFlags)MBAPI.IMBAgent.GetScriptedCombatFlags(this.GetPtr());
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x0001178C File Offset: 0x0000F98C
		public GameEntity GetSteppedEntity()
		{
			UIntPtr steppedEntityId = MBAPI.IMBAgent.GetSteppedEntityId(this.GetPtr());
			if (!(steppedEntityId != UIntPtr.Zero))
			{
				return null;
			}
			return new GameEntity(steppedEntityId);
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x000117BF File Offset: 0x0000F9BF
		public AnimFlags GetCurrentAnimationFlag(int channelNo)
		{
			return (AnimFlags)MBAPI.IMBAgent.GetCurrentAnimationFlags(this.GetPtr(), channelNo);
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x000117D2 File Offset: 0x0000F9D2
		public ActionIndexCache GetCurrentAction(int channelNo)
		{
			return new ActionIndexCache(MBAPI.IMBAgent.GetCurrentAction(this.GetPtr(), channelNo));
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x000117EA File Offset: 0x0000F9EA
		public ActionIndexValueCache GetCurrentActionValue(int channelNo)
		{
			return new ActionIndexValueCache(MBAPI.IMBAgent.GetCurrentAction(this.GetPtr(), channelNo));
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x00011802 File Offset: 0x0000FA02
		public Agent.ActionCodeType GetCurrentActionType(int channelNo)
		{
			return (Agent.ActionCodeType)MBAPI.IMBAgent.GetCurrentActionType(this.GetPtr(), channelNo);
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x00011815 File Offset: 0x0000FA15
		public Agent.ActionStage GetCurrentActionStage(int channelNo)
		{
			return (Agent.ActionStage)MBAPI.IMBAgent.GetCurrentActionStage(this.GetPtr(), channelNo);
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x00011828 File Offset: 0x0000FA28
		public Agent.UsageDirection GetCurrentActionDirection(int channelNo)
		{
			return (Agent.UsageDirection)MBAPI.IMBAgent.GetCurrentActionDirection(this.GetPtr(), channelNo);
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x0001183B File Offset: 0x0000FA3B
		public int GetCurrentActionPriority(int channelNo)
		{
			return MBAPI.IMBAgent.GetCurrentActionPriority(this.GetPtr(), channelNo);
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x0001184E File Offset: 0x0000FA4E
		public float GetCurrentActionProgress(int channelNo)
		{
			return MBAPI.IMBAgent.GetCurrentActionProgress(this.GetPtr(), channelNo);
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x00011861 File Offset: 0x0000FA61
		public float GetActionChannelWeight(int channelNo)
		{
			return MBAPI.IMBAgent.GetActionChannelWeight(this.GetPtr(), channelNo);
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x00011874 File Offset: 0x0000FA74
		public float GetActionChannelCurrentActionWeight(int channelNo)
		{
			return MBAPI.IMBAgent.GetActionChannelCurrentActionWeight(this.GetPtr(), channelNo);
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x00011887 File Offset: 0x0000FA87
		public WorldFrame GetWorldFrame()
		{
			return new WorldFrame(this.LookRotation, this.GetWorldPosition());
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x0001189A File Offset: 0x0000FA9A
		public float GetLookDownLimit()
		{
			return MBAPI.IMBAgent.GetLookDownLimit(this.GetPtr());
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x000118AC File Offset: 0x0000FAAC
		public float GetEyeGlobalHeight()
		{
			return MBAPI.IMBAgent.GetEyeGlobalHeight(this.GetPtr());
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x000118BE File Offset: 0x0000FABE
		public float GetMaximumSpeedLimit()
		{
			return MBAPI.IMBAgent.GetMaximumSpeedLimit(this.GetPtr());
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x000118D0 File Offset: 0x0000FAD0
		public Vec2 GetCurrentVelocity()
		{
			return MBAPI.IMBAgent.GetCurrentVelocity(this.GetPtr());
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x000118E2 File Offset: 0x0000FAE2
		public float GetTurnSpeed()
		{
			return MBAPI.IMBAgent.GetTurnSpeed(this.GetPtr());
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x000118F4 File Offset: 0x0000FAF4
		public float GetCurrentSpeedLimit()
		{
			return MBAPI.IMBAgent.GetCurrentSpeedLimit(this.GetPtr());
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x00011906 File Offset: 0x0000FB06
		public Vec2 GetMovementDirection()
		{
			return MBAPI.IMBAgent.GetMovementDirection(this.GetPtr());
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x00011918 File Offset: 0x0000FB18
		public Vec3 GetCurWeaponOffset()
		{
			return MBAPI.IMBAgent.GetCurWeaponOffset(this.GetPtr());
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x0001192A File Offset: 0x0000FB2A
		public bool GetIsLeftStance()
		{
			return MBAPI.IMBAgent.GetIsLeftStance(this.GetPtr());
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x0001193C File Offset: 0x0000FB3C
		public float GetPathDistanceToPoint(ref Vec3 point)
		{
			return MBAPI.IMBAgent.GetPathDistanceToPoint(this.GetPtr(), ref point);
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x0001194F File Offset: 0x0000FB4F
		public int GetCurrentNavigationFaceId()
		{
			return MBAPI.IMBAgent.GetCurrentNavigationFaceId(this.GetPtr());
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x00011961 File Offset: 0x0000FB61
		public WorldPosition GetWorldPosition()
		{
			return MBAPI.IMBAgent.GetWorldPosition(this.GetPtr());
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x00011973 File Offset: 0x0000FB73
		public Agent GetLookAgent()
		{
			return this._lookAgentCache;
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x0001197B File Offset: 0x0000FB7B
		public Agent GetTargetAgent()
		{
			return MBAPI.IMBAgent.GetTargetAgent(this.GetPtr());
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x0001198D File Offset: 0x0000FB8D
		public AgentFlag GetAgentFlags()
		{
			return AgentHelper.GetAgentFlags(this.FlagsPointer);
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x0001199A File Offset: 0x0000FB9A
		public string GetAgentFacialAnimation()
		{
			return MBAPI.IMBAgent.GetAgentFacialAnimation(this.GetPtr());
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x000119AC File Offset: 0x0000FBAC
		public string GetAgentVoiceDefinition()
		{
			return MBAPI.IMBAgent.GetAgentVoiceDefinition(this.GetPtr());
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x000119BE File Offset: 0x0000FBBE
		public Vec3 GetEyeGlobalPosition()
		{
			return MBAPI.IMBAgent.GetEyeGlobalPosition(this.GetPtr());
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x000119D0 File Offset: 0x0000FBD0
		public Vec3 GetChestGlobalPosition()
		{
			return MBAPI.IMBAgent.GetChestGlobalPosition(this.GetPtr());
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x000119E2 File Offset: 0x0000FBE2
		public Agent.MovementControlFlag GetDefendMovementFlag()
		{
			return MBAPI.IMBAgent.GetDefendMovementFlag(this.GetPtr());
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x000119F4 File Offset: 0x0000FBF4
		public Agent.UsageDirection GetAttackDirection(bool doAiCheck)
		{
			return MBAPI.IMBAgent.GetAttackDirection(this.GetPtr(), doAiCheck);
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x00011A08 File Offset: 0x0000FC08
		public WeaponInfo GetWieldedWeaponInfo(Agent.HandIndex handIndex)
		{
			bool flag = false;
			bool flag2 = false;
			if (MBAPI.IMBAgent.GetWieldedWeaponInfo(this.GetPtr(), (int)handIndex, ref flag, ref flag2))
			{
				return new WeaponInfo(true, flag, flag2);
			}
			return new WeaponInfo(false, false, false);
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x00011A44 File Offset: 0x0000FC44
		public Vec2 GetBodyRotationConstraint(int channelIndex = 1)
		{
			return MBAPI.IMBAgent.GetBodyRotationConstraint(this.GetPtr(), channelIndex).AsVec2;
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x00011A6A File Offset: 0x0000FC6A
		public float GetTotalEncumbrance()
		{
			return this.AgentDrivenProperties.ArmorEncumbrance + this.AgentDrivenProperties.WeaponsEncumbrance;
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x00011A84 File Offset: 0x0000FC84
		public T GetComponent<T>() where T : AgentComponent
		{
			for (int i = 0; i < this._components.Count; i++)
			{
				if (this._components[i] is T)
				{
					return (T)((object)this._components[i]);
				}
			}
			return default(T);
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x00011AD5 File Offset: 0x0000FCD5
		public float GetAgentDrivenPropertyValue(DrivenProperty type)
		{
			return this.AgentDrivenProperties.GetStat(type);
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x00011AE4 File Offset: 0x0000FCE4
		public UsableMachine GetSteppedMachine()
		{
			GameEntity gameEntity = this.GetSteppedEntity();
			while (gameEntity != null && !gameEntity.HasScriptOfType<UsableMachine>())
			{
				gameEntity = gameEntity.Parent;
			}
			if (gameEntity != null)
			{
				return gameEntity.GetFirstScriptOfType<UsableMachine>();
			}
			return null;
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x00011B23 File Offset: 0x0000FD23
		public int GetAttachedWeaponsCount()
		{
			List<ValueTuple<MissionWeapon, MatrixFrame, sbyte>> attachedWeapons = this._attachedWeapons;
			if (attachedWeapons == null)
			{
				return 0;
			}
			return attachedWeapons.Count;
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x00011B36 File Offset: 0x0000FD36
		public MissionWeapon GetAttachedWeapon(int index)
		{
			return this._attachedWeapons[index].Item1;
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x00011B49 File Offset: 0x0000FD49
		public MatrixFrame GetAttachedWeaponFrame(int index)
		{
			return this._attachedWeapons[index].Item2;
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x00011B5C File Offset: 0x0000FD5C
		public sbyte GetAttachedWeaponBoneIndex(int index)
		{
			return this._attachedWeapons[index].Item3;
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x00011B6F File Offset: 0x0000FD6F
		public void DeleteAttachedWeapon(int index)
		{
			this._attachedWeapons.RemoveAt(index);
			MBAPI.IMBAgent.DeleteAttachedWeaponFromBone(this.Pointer, index);
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x00011B90 File Offset: 0x0000FD90
		public bool HasRangedWeapon(bool checkHasAmmo = false)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
			{
				int num;
				bool flag;
				bool flag2;
				if (!this.Equipment[equipmentIndex].IsEmpty && this.Equipment[equipmentIndex].GetRangedUsageIndex() >= 0 && (!checkHasAmmo || this.Equipment.HasAmmo(equipmentIndex, out num, out flag, out flag2)))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x00011BF0 File Offset: 0x0000FDF0
		public void GetFormationFileAndRankInfo(out int fileIndex, out int rankIndex)
		{
			fileIndex = ((IFormationUnit)this).FormationFileIndex;
			rankIndex = ((IFormationUnit)this).FormationRankIndex;
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x00011C10 File Offset: 0x0000FE10
		public void GetFormationFileAndRankInfo(out int fileIndex, out int rankIndex, out int fileCount, out int rankCount)
		{
			fileIndex = ((IFormationUnit)this).FormationFileIndex;
			rankIndex = ((IFormationUnit)this).FormationRankIndex;
			LineFormation lineFormation;
			if ((lineFormation = ((IFormationUnit)this).Formation as LineFormation) != null)
			{
				lineFormation.GetFormationInfo(out fileCount, out rankCount);
				return;
			}
			fileCount = -1;
			rankCount = -1;
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x00011C4F File Offset: 0x0000FE4F
		internal Vec2 GetWallDirectionOfRelativeFormationLocation()
		{
			return this.Formation.GetWallDirectionOfRelativeFormationLocation(this);
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x00011C5D File Offset: 0x0000FE5D
		public void SetMortalityState(Agent.MortalityState newState)
		{
			this.CurrentMortalityState = newState;
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x00011C66 File Offset: 0x0000FE66
		public void ToggleInvulnerable()
		{
			if (this.CurrentMortalityState == Agent.MortalityState.Invulnerable)
			{
				this.CurrentMortalityState = Agent.MortalityState.Mortal;
				return;
			}
			this.CurrentMortalityState = Agent.MortalityState.Invulnerable;
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x00011C80 File Offset: 0x0000FE80
		public float GetArmLength()
		{
			return this.Monster.ArmLength * this.AgentScale;
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x00011C94 File Offset: 0x0000FE94
		public float GetArmWeight()
		{
			return this.Monster.ArmWeight * this.AgentScale;
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x00011CA8 File Offset: 0x0000FEA8
		public void GetRunningSimulationDataUntilMaximumSpeedReached(ref float combatAccelerationTime, ref float maxSpeed, float[] speedValues)
		{
			MBAPI.IMBAgent.GetRunningSimulationDataUntilMaximumSpeedReached(this.GetPtr(), ref combatAccelerationTime, ref maxSpeed, speedValues);
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x00011CBD File Offset: 0x0000FEBD
		public void SetMaximumSpeedLimit(float maximumSpeedLimit, bool isMultiplier)
		{
			MBAPI.IMBAgent.SetMaximumSpeedLimit(this.GetPtr(), maximumSpeedLimit, isMultiplier);
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x00011CD4 File Offset: 0x0000FED4
		public float GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType bodyPart)
		{
			if (!this.IsHuman)
			{
				return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
			}
			if (bodyPart == BoneBodyPartType.None)
			{
				return 0f;
			}
			if (bodyPart == BoneBodyPartType.Head || bodyPart == BoneBodyPartType.Neck)
			{
				return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorHead);
			}
			if (bodyPart == BoneBodyPartType.Legs)
			{
				return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorLegs);
			}
			if (bodyPart == BoneBodyPartType.ArmLeft || bodyPart == BoneBodyPartType.ArmRight)
			{
				return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorArms);
			}
			if (bodyPart == BoneBodyPartType.ShoulderLeft || bodyPart == BoneBodyPartType.ShoulderRight || bodyPart == BoneBodyPartType.Chest || bodyPart == BoneBodyPartType.Abdomen)
			{
				return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Agent.cs", "GetBaseArmorEffectivenessForBodyPart", 2778);
			return this.GetAgentDrivenPropertyValue(DrivenProperty.ArmorTorso);
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x00011D64 File Offset: 0x0000FF64
		public AITargetVisibilityState GetLastTargetVisibilityState()
		{
			return (AITargetVisibilityState)MBAPI.IMBAgent.GetLastTargetVisibilityState(this.GetPtr());
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x00011D76 File Offset: 0x0000FF76
		public float GetMissileRange()
		{
			return MBAPI.IMBAgent.GetMissileRange(this.GetPtr());
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x00011D88 File Offset: 0x0000FF88
		public ItemObject GetWeaponToReplaceOnQuickAction(SpawnedItemEntity spawnedItem, out EquipmentIndex possibleSlotIndex)
		{
			EquipmentIndex equipmentIndex = MissionEquipment.SelectWeaponPickUpSlot(this, spawnedItem.WeaponCopy, spawnedItem.IsStuckMissile());
			possibleSlotIndex = equipmentIndex;
			if (equipmentIndex != EquipmentIndex.None && !this.Equipment[equipmentIndex].IsEmpty && ((!spawnedItem.IsStuckMissile() && !spawnedItem.WeaponCopy.IsAnyConsumable()) || this.Equipment[equipmentIndex].Item.PrimaryWeapon.WeaponClass != spawnedItem.WeaponCopy.Item.PrimaryWeapon.WeaponClass || !this.Equipment[equipmentIndex].IsAnyConsumable() || this.Equipment[equipmentIndex].Amount == this.Equipment[equipmentIndex].ModifiedMaxAmount))
			{
				return this.Equipment[equipmentIndex].Item;
			}
			return null;
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x00011E74 File Offset: 0x00010074
		public Agent.Hitter GetAssistingHitter(MissionPeer killerPeer)
		{
			Agent.Hitter hitter = null;
			foreach (Agent.Hitter hitter2 in this.HitterList)
			{
				if (hitter2.HitterPeer != killerPeer && (hitter == null || hitter2.Damage > hitter.Damage))
				{
					hitter = hitter2;
				}
			}
			if (hitter != null && hitter.Damage >= 35f)
			{
				return hitter;
			}
			return null;
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x00011EF4 File Offset: 0x000100F4
		public bool CanReachAgent(Agent otherAgent)
		{
			float interactionDistanceToUsable = this.GetInteractionDistanceToUsable(otherAgent);
			return this.Position.DistanceSquared(otherAgent.Position) < interactionDistanceToUsable * interactionDistanceToUsable;
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x00011F24 File Offset: 0x00010124
		public bool CanInteractWithAgent(Agent otherAgent, float userAgentCameraElevation)
		{
			bool flag = false;
			foreach (MissionBehavior missionBehavior in Mission.Current.MissionBehaviors)
			{
				flag = flag || missionBehavior.IsThereAgentAction(this, otherAgent);
			}
			if (!flag)
			{
				return false;
			}
			bool flag2 = this.CanReachAgent(otherAgent);
			if (!otherAgent.IsMount)
			{
				return this.IsOnLand() && flag2;
			}
			if ((this.MountAgent == null && this.GetCurrentActionValue(0) != ActionIndexValueCache.act_none) || (this.MountAgent != null && !this.IsOnLand()))
			{
				return false;
			}
			if (otherAgent.RiderAgent == null)
			{
				return this.MountAgent == null && flag2 && this.CheckSkillForMounting(otherAgent) && otherAgent.GetCurrentActionType(0) != Agent.ActionCodeType.Rear;
			}
			return otherAgent == this.MountAgent && (flag2 && userAgentCameraElevation < this.GetLookDownLimit() + 0.4f && this.GetCurrentVelocity().LengthSquared < 0.25f) && otherAgent.GetCurrentActionType(0) != Agent.ActionCodeType.Rear;
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x00012048 File Offset: 0x00010248
		public bool CanBeAssignedForScriptedMovement()
		{
			return this.IsActive() && this.IsAIControlled && !this.IsDetachedFromFormation && !this.IsRunningAway && (this.GetScriptedFlags() & Agent.AIScriptedFrameFlags.GoToPosition) == Agent.AIScriptedFrameFlags.None && !this.InteractingWithAnyGameObject();
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x0001207F File Offset: 0x0001027F
		public bool CanReachAndUseObject(UsableMissionObject gameObject, float distanceSq)
		{
			return this.CanReachObject(gameObject, distanceSq) && this.CanUseObject(gameObject);
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x00012094 File Offset: 0x00010294
		public bool CanReachObject(UsableMissionObject gameObject, float distanceSq)
		{
			if (this.IsItemUseDisabled || this.IsUsingGameObject)
			{
				return false;
			}
			float interactionDistanceToUsable = this.GetInteractionDistanceToUsable(gameObject);
			return distanceSq <= interactionDistanceToUsable * interactionDistanceToUsable && MathF.Abs(gameObject.InteractionEntity.GlobalPosition.z - this.Position.z) <= interactionDistanceToUsable * 2f;
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x000120F0 File Offset: 0x000102F0
		public bool CanUseObject(UsableMissionObject gameObject)
		{
			return !gameObject.IsDisabledForAgent(this) && gameObject.IsUsableByAgent(this);
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00012104 File Offset: 0x00010304
		public bool CanMoveDirectlyToPosition(in WorldPosition worldPosition)
		{
			return MBAPI.IMBAgent.CanMoveDirectlyToPosition(this.GetPtr(), worldPosition);
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x00012118 File Offset: 0x00010318
		public bool CanInteractableWeaponBePickedUp(SpawnedItemEntity spawnedItem)
		{
			EquipmentIndex equipmentIndex;
			return (!spawnedItem.IsBanner() || MissionGameModels.Current.BattleBannerBearersModel.IsInteractableFormationBanner(spawnedItem, this)) && (this.GetWeaponToReplaceOnQuickAction(spawnedItem, out equipmentIndex) != null || equipmentIndex == EquipmentIndex.None);
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00012153 File Offset: 0x00010353
		public bool CanQuickPickUp(SpawnedItemEntity spawnedItem)
		{
			return (!spawnedItem.IsBanner() || MissionGameModels.Current.BattleBannerBearersModel.IsInteractableFormationBanner(spawnedItem, this)) && MissionEquipment.SelectWeaponPickUpSlot(this, spawnedItem.WeaponCopy, spawnedItem.IsStuckMissile()) != EquipmentIndex.None;
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x0001218C File Offset: 0x0001038C
		public unsafe bool CanTeleport()
		{
			return this.Mission.IsTeleportingAgents && (this.Formation == null || this.Mission.Mode != MissionMode.Deployment || this.Formation.GetReadonlyMovementOrderReference()->OrderEnum == MovementOrder.MovementOrderEnum.Move);
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x000121D8 File Offset: 0x000103D8
		public bool IsActive()
		{
			return this.State == AgentState.Active;
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x000121E3 File Offset: 0x000103E3
		public bool IsRetreating()
		{
			return MBAPI.IMBAgent.IsRetreating(this.GetPtr());
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x000121F5 File Offset: 0x000103F5
		public bool IsFadingOut()
		{
			return MBAPI.IMBAgent.IsFadingOut(this.GetPtr());
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00012207 File Offset: 0x00010407
		public void SetAgentDrivenPropertyValueFromConsole(DrivenProperty type, float val)
		{
			this.AgentDrivenProperties.SetStat(type, val);
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x00012216 File Offset: 0x00010416
		public bool IsOnLand()
		{
			return MBAPI.IMBAgent.IsOnLand(this.GetPtr());
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x00012228 File Offset: 0x00010428
		public bool IsSliding()
		{
			return MBAPI.IMBAgent.IsSliding(this.GetPtr());
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x0001223C File Offset: 0x0001043C
		public bool IsSitting()
		{
			Agent.ActionCodeType currentActionType = this.GetCurrentActionType(0);
			return currentActionType == Agent.ActionCodeType.Sit || currentActionType == Agent.ActionCodeType.SitOnTheFloor || currentActionType == Agent.ActionCodeType.SitOnAThrone;
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x00012264 File Offset: 0x00010464
		public bool IsReleasingChainAttack()
		{
			bool flag = false;
			if (Mission.Current.CurrentTime - this._lastQuickReadyDetectedTime < 0.75f && this.GetCurrentActionStage(1) == Agent.ActionStage.AttackRelease)
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x00012298 File Offset: 0x00010498
		public bool IsCameraAttachable()
		{
			return !this._isDeleted && (!this._isRemoved || this._removalTime + 2.1f > this.Mission.CurrentTime) && this.IsHuman && this.AgentVisuals != null && this.AgentVisuals.IsValid() && (GameNetwork.IsSessionActive || this._agentControllerType > Agent.ControllerType.None);
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x00012305 File Offset: 0x00010505
		public bool IsSynchedPrefabComponentVisible(int componentIndex)
		{
			return this._synchedBodyComponents[componentIndex].GetVisible();
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x00012318 File Offset: 0x00010518
		public bool IsEnemyOf(Agent otherAgent)
		{
			return MBAPI.IMBAgent.IsEnemy(this.GetPtr(), otherAgent.GetPtr());
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x00012330 File Offset: 0x00010530
		public bool IsFriendOf(Agent otherAgent)
		{
			return MBAPI.IMBAgent.IsFriend(this.GetPtr(), otherAgent.GetPtr());
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x00012348 File Offset: 0x00010548
		public void OnFocusGain(Agent userAgent)
		{
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x0001234A File Offset: 0x0001054A
		public void OnFocusLose(Agent userAgent)
		{
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x0001234C File Offset: 0x0001054C
		public void OnItemRemovedFromScene()
		{
			this.StopUsingGameObjectMT(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x00012356 File Offset: 0x00010556
		public void OnUse(Agent userAgent)
		{
			this.Mission.OnAgentInteraction(userAgent, this);
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x00012365 File Offset: 0x00010565
		public void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
		{
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x00012368 File Offset: 0x00010568
		public void OnWeaponDrop(EquipmentIndex equipmentSlot)
		{
			MissionWeapon missionWeapon = this.Equipment[equipmentSlot];
			this.Equipment[equipmentSlot] = MissionWeapon.Invalid;
			this.WeaponEquipped(equipmentSlot, WeaponData.InvalidWeaponData, null, WeaponData.InvalidWeaponData, null, null, false, false);
			foreach (AgentComponent agentComponent in this._components)
			{
				agentComponent.OnWeaponDrop(missionWeapon);
			}
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x000123F0 File Offset: 0x000105F0
		public void OnItemPickup(SpawnedItemEntity spawnedItemEntity, EquipmentIndex weaponPickUpSlotIndex, out bool removeWeapon)
		{
			removeWeapon = true;
			bool flag = true;
			MissionWeapon weaponCopy = spawnedItemEntity.WeaponCopy;
			if (weaponPickUpSlotIndex == EquipmentIndex.None)
			{
				weaponPickUpSlotIndex = MissionEquipment.SelectWeaponPickUpSlot(this, weaponCopy, spawnedItemEntity.IsStuckMissile());
			}
			bool flag2 = false;
			if (weaponPickUpSlotIndex == EquipmentIndex.ExtraWeaponSlot)
			{
				if (!GameNetwork.IsClientOrReplay)
				{
					flag2 = true;
					if (!this.Equipment[weaponPickUpSlotIndex].IsEmpty)
					{
						this.DropItem(weaponPickUpSlotIndex, this.Equipment[weaponPickUpSlotIndex].Item.PrimaryWeapon.WeaponClass);
					}
				}
			}
			else if (weaponPickUpSlotIndex != EquipmentIndex.None)
			{
				int num = 0;
				if ((spawnedItemEntity.IsStuckMissile() || spawnedItemEntity.WeaponCopy.IsAnyConsumable()) && !this.Equipment[weaponPickUpSlotIndex].IsEmpty && this.Equipment[weaponPickUpSlotIndex].IsSameType(weaponCopy) && this.Equipment[weaponPickUpSlotIndex].IsAnyConsumable())
				{
					num = (int)(this.Equipment[weaponPickUpSlotIndex].ModifiedMaxAmount - this.Equipment[weaponPickUpSlotIndex].Amount);
				}
				if (num > 0)
				{
					short num2 = (short)MathF.Min(num, (int)weaponCopy.Amount);
					if (num2 != weaponCopy.Amount)
					{
						removeWeapon = false;
						if (!GameNetwork.IsClientOrReplay)
						{
							spawnedItemEntity.ConsumeWeaponAmount(num2);
							if (GameNetwork.IsServer)
							{
								GameNetwork.BeginBroadcastModuleEvent();
								GameNetwork.WriteMessage(new ConsumeWeaponAmount(spawnedItemEntity, num2));
								GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
							}
						}
					}
					if (!GameNetwork.IsClientOrReplay)
					{
						this.SetWeaponAmountInSlot(weaponPickUpSlotIndex, this.Equipment[weaponPickUpSlotIndex].Amount + num2, true);
						if (this.GetWieldedItemIndex(Agent.HandIndex.MainHand) == EquipmentIndex.None && (weaponCopy.Item.PrimaryWeapon.IsRangedWeapon || weaponCopy.Item.PrimaryWeapon.IsMeleeWeapon))
						{
							flag2 = true;
						}
					}
				}
				else if (!GameNetwork.IsClientOrReplay)
				{
					flag2 = true;
					if (!this.Equipment[weaponPickUpSlotIndex].IsEmpty)
					{
						this.DropItem(weaponPickUpSlotIndex, weaponCopy.Item.PrimaryWeapon.WeaponClass);
					}
				}
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				flag = MissionEquipment.DoesWeaponFitToSlot(weaponPickUpSlotIndex, weaponCopy);
				if (flag)
				{
					this.EquipWeaponFromSpawnedItemEntity(weaponPickUpSlotIndex, spawnedItemEntity, removeWeapon);
					if (flag2)
					{
						EquipmentIndex equipmentIndex = weaponPickUpSlotIndex;
						if (weaponCopy.Item.PrimaryWeapon.AmmoClass == weaponCopy.Item.PrimaryWeapon.WeaponClass)
						{
							for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < weaponPickUpSlotIndex; equipmentIndex2++)
							{
								if (!this.Equipment[equipmentIndex2].IsEmpty && weaponCopy.IsEqualTo(this.Equipment[equipmentIndex2]))
								{
									equipmentIndex = equipmentIndex2;
									break;
								}
							}
						}
						this.TryToWieldWeaponInSlot(equipmentIndex, Agent.WeaponWieldActionType.InstantAfterPickUp, false);
					}
					for (int i = 0; i < this._components.Count; i++)
					{
						this._components[i].OnItemPickup(spawnedItemEntity);
					}
					if (this.Controller == Agent.ControllerType.AI)
					{
						this.HumanAIComponent.ItemPickupDone(spawnedItemEntity);
					}
				}
			}
			if (flag)
			{
				foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
				{
					missionBehavior.OnItemPickup(this, spawnedItemEntity);
				}
			}
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x00012718 File Offset: 0x00010918
		public bool CheckTracked(BasicCharacterObject basicCharacter)
		{
			return this.Character == basicCharacter;
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x00012723 File Offset: 0x00010923
		public bool CheckPathToAITargetAgentPassesThroughNavigationFaceIdFromDirection(int navigationFaceId, Vec3 direction, float overridenCostForFaceId)
		{
			return MBAPI.IMBAgent.CheckPathToAITargetAgentPassesThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, ref direction, overridenCostForFaceId);
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x0001273C File Offset: 0x0001093C
		public void CheckEquipmentForCapeClothSimulationStateChange()
		{
			if (this._capeClothSimulator != null)
			{
				bool flag = false;
				EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
				{
					MissionWeapon missionWeapon = this.Equipment[equipmentIndex];
					if (!missionWeapon.IsEmpty && missionWeapon.IsShield() && equipmentIndex != wieldedItemIndex)
					{
						flag = true;
						break;
					}
				}
				this._capeClothSimulator.SetMaxDistanceMultiplier(flag ? 0f : 1f);
			}
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x000127B0 File Offset: 0x000109B0
		public void CheckToDropFlaggedItem()
		{
			if (this.GetAgentFlags().HasAnyFlag(AgentFlag.CanWieldWeapon))
			{
				for (int i = 0; i < 2; i++)
				{
					EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex((Agent.HandIndex)i);
					if (wieldedItemIndex != EquipmentIndex.None && this.Equipment[wieldedItemIndex].Item.ItemFlags.HasAnyFlag(ItemFlags.DropOnAnyAction))
					{
						this.DropItem(wieldedItemIndex, WeaponClass.Undefined);
					}
				}
			}
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x00012814 File Offset: 0x00010A14
		public bool CheckSkillForMounting(Agent mountAgent)
		{
			int skillValue = this.Character.GetSkillValue(DefaultSkills.Riding);
			return (this.GetAgentFlags() & AgentFlag.CanRide) > AgentFlag.None && (float)skillValue >= mountAgent.GetAgentDrivenPropertyValue(DrivenProperty.MountDifficulty);
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x00012852 File Offset: 0x00010A52
		public void InitializeSpawnEquipment(Equipment spawnEquipment)
		{
			this.SpawnEquipment = spawnEquipment;
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x0001285B File Offset: 0x00010A5B
		public void InitializeMissionEquipment(MissionEquipment missionEquipment, Banner banner)
		{
			this.Equipment = missionEquipment ?? new MissionEquipment(this.SpawnEquipment, banner);
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x00012874 File Offset: 0x00010A74
		public void InitializeAgentProperties(Equipment spawnEquipment, AgentBuildData agentBuildData)
		{
			this._propertyModifiers = default(Agent.AgentPropertiesModifiers);
			this.AgentDrivenProperties = new AgentDrivenProperties();
			float[] array = this.AgentDrivenProperties.InitializeDrivenProperties(this, spawnEquipment, agentBuildData);
			this.UpdateDrivenProperties(array);
			if (this.IsMount && this.RiderAgent == null)
			{
				Mission.Current.AddMountWithoutRider(this);
			}
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x000128CC File Offset: 0x00010ACC
		public void UpdateFormationOrders()
		{
			if (this.Formation != null)
			{
				this.SetFiringOrder((int)this.Formation.FiringOrder.OrderEnum);
				this.EnforceShieldUsage(ArrangementOrder.GetShieldDirectionOfUnit(this.Formation, this, this.Formation.ArrangementOrder.OrderEnum));
			}
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x00012919 File Offset: 0x00010B19
		public void UpdateWeapons()
		{
			MBAPI.IMBAgent.UpdateWeapons(this.GetPtr());
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x0001292C File Offset: 0x00010B2C
		public void UpdateAgentProperties()
		{
			if (this.AgentDrivenProperties != null)
			{
				float[] array = this.AgentDrivenProperties.UpdateDrivenProperties(this);
				this.UpdateDrivenProperties(array);
			}
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x00012955 File Offset: 0x00010B55
		public void UpdateCustomDrivenProperties()
		{
			if (this.AgentDrivenProperties != null)
			{
				this.UpdateDrivenProperties(this.AgentDrivenProperties.Values);
			}
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x00012970 File Offset: 0x00010B70
		public void UpdateBodyProperties(BodyProperties bodyProperties)
		{
			this.BodyPropertiesValue = bodyProperties;
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x00012979 File Offset: 0x00010B79
		public void UpdateSyncHealthToAllClients(bool value)
		{
			this.SyncHealthToAllClients = value;
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x00012984 File Offset: 0x00010B84
		public void UpdateSpawnEquipmentAndRefreshVisuals(Equipment newSpawnEquipment)
		{
			this.SpawnEquipment = newSpawnEquipment;
			this.AgentVisuals.ClearVisualComponents(false);
			this.Mission.OnEquipItemsFromSpawnEquipment(this, Agent.CreationType.FromCharacterObj);
			this.AgentVisuals.ClearAllWeaponMeshes();
			MissionEquipment equipment = this.Equipment;
			Equipment spawnEquipment = this.SpawnEquipment;
			IAgentOriginBase origin = this.Origin;
			equipment.FillFrom(spawnEquipment, (origin != null) ? origin.Banner : null);
			this.CheckEquipmentForCapeClothSimulationStateChange();
			this.EquipItemsFromSpawnEquipment(true);
			this.UpdateAgentProperties();
			if (!Mission.Current.DoesMissionRequireCivilianEquipment)
			{
				this.WieldInitialWeapons(Agent.WeaponWieldActionType.InstantAfterPickUp);
			}
			this.PreloadForRendering();
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00012A0C File Offset: 0x00010C0C
		public void UpdateCachedAndFormationValues(bool updateOnlyMovement, bool arrangementChangeAllowed)
		{
			if (!this.IsActive())
			{
				return;
			}
			if (!updateOnlyMovement)
			{
				Agent mountAgent = this.MountAgent;
				this.WalkSpeedCached = ((mountAgent != null) ? mountAgent.WalkingSpeedLimitOfMountable : this.Monster.WalkingSpeedLimit);
				this.RunSpeedCached = this.MaximumForwardUnlimitedSpeed;
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				if (!updateOnlyMovement && !this.IsDetachedFromFormation)
				{
					Formation formation = this.Formation;
					if (formation != null)
					{
						formation.Arrangement.OnTickOccasionallyOfUnit(this, arrangementChangeAllowed);
					}
				}
				if (this.IsAIControlled)
				{
					this.HumanAIComponent.UpdateFormationMovement();
				}
				if (!updateOnlyMovement)
				{
					Formation formation2 = this.Formation;
					if (formation2 != null)
					{
						formation2.Team.DetachmentManager.TickAgent(this);
					}
				}
				if (!updateOnlyMovement && this.IsAIControlled)
				{
					this.UpdateFormationOrders();
					if (this.Formation != null)
					{
						int num;
						int num2;
						int num3;
						int num4;
						this.GetFormationFileAndRankInfo(out num, out num2, out num3, out num4);
						Vec2 wallDirectionOfRelativeFormationLocation = this.GetWallDirectionOfRelativeFormationLocation();
						MBAPI.IMBAgent.SetFormationInfo(this.GetPtr(), num, num2, num3, num4, wallDirectionOfRelativeFormationLocation, this.Formation.UnitSpacing);
					}
				}
			}
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x00012B02 File Offset: 0x00010D02
		public void UpdateLastRangedAttackTimeDueToAnAttack(float newTime)
		{
			this.LastRangedAttackTime = newTime;
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x00012B0B File Offset: 0x00010D0B
		public void InvalidateTargetAgent()
		{
			MBAPI.IMBAgent.InvalidateTargetAgent(this.GetPtr());
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x00012B1D File Offset: 0x00010D1D
		public void InvalidateAIWeaponSelections()
		{
			MBAPI.IMBAgent.InvalidateAIWeaponSelections(this.GetPtr());
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x00012B2F File Offset: 0x00010D2F
		public void ResetLookAgent()
		{
			this.SetLookAgent(null);
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x00012B38 File Offset: 0x00010D38
		public void ResetGuard()
		{
			MBAPI.IMBAgent.ResetGuard(this.GetPtr());
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x00012B4A File Offset: 0x00010D4A
		public void ResetAgentProperties()
		{
			this.AgentDrivenProperties = null;
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x00012B53 File Offset: 0x00010D53
		public void ResetAiWaitBeforeShootFactor()
		{
			this._propertyModifiers.resetAiWaitBeforeShootFactor = true;
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x00012B61 File Offset: 0x00010D61
		public void ClearTargetFrame()
		{
			this._checkIfTargetFrameIsChanged = false;
			if (this.MovementLockedState != AgentMovementLockedState.None)
			{
				this.ClearTargetFrameAux();
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new ClearAgentTargetFrame(this));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
			}
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x00012B97 File Offset: 0x00010D97
		public void ClearEquipment()
		{
			MBAPI.IMBAgent.ClearEquipment(this.GetPtr());
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x00012BA9 File Offset: 0x00010DA9
		public void ClearHandInverseKinematics()
		{
			MBAPI.IMBAgent.ClearHandInverseKinematics(this.GetPtr());
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x00012BBB File Offset: 0x00010DBB
		public void ClearAttachedWeapons()
		{
			List<ValueTuple<MissionWeapon, MatrixFrame, sbyte>> attachedWeapons = this._attachedWeapons;
			if (attachedWeapons == null)
			{
				return;
			}
			attachedWeapons.Clear();
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x00012BD0 File Offset: 0x00010DD0
		public void SetDetachableFromFormation(bool value)
		{
			bool isDetachableFromFormation = this._isDetachableFromFormation;
			if (isDetachableFromFormation != value)
			{
				if (isDetachableFromFormation)
				{
					if (this.IsDetachedFromFormation)
					{
						this._detachment.RemoveAgent(this);
						Formation formation = this._formation;
						if (formation != null)
						{
							formation.AttachUnit(this);
						}
					}
					Formation formation2 = this._formation;
					if (formation2 != null)
					{
						Team team = formation2.Team;
						if (team != null)
						{
							team.DetachmentManager.RemoveScoresOfAgentFromDetachments(this);
						}
					}
				}
				this._isDetachableFromFormation = value;
				if (!this.IsPlayerControlled)
				{
					if (isDetachableFromFormation)
					{
						Formation formation3 = this._formation;
						if (formation3 == null)
						{
							return;
						}
						formation3.OnUndetachableNonPlayerUnitAdded(this);
						return;
					}
					else
					{
						Formation formation4 = this._formation;
						if (formation4 == null)
						{
							return;
						}
						formation4.OnUndetachableNonPlayerUnitRemoved(this);
					}
				}
			}
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x00012C6B File Offset: 0x00010E6B
		public void EnforceShieldUsage(Agent.UsageDirection shieldDirection)
		{
			MBAPI.IMBAgent.EnforceShieldUsage(this.GetPtr(), shieldDirection);
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x00012C7E File Offset: 0x00010E7E
		public bool ObjectHasVacantPosition(UsableMissionObject gameObject)
		{
			return !gameObject.HasUser || gameObject.HasAIUser;
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x00012C90 File Offset: 0x00010E90
		public bool InteractingWithAnyGameObject()
		{
			return this.IsUsingGameObject || (this.IsAIControlled && this.AIInterestedInAnyGameObject());
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x00012CAC File Offset: 0x00010EAC
		private void StopUsingGameObjectAux(bool isSuccessful, Agent.StopUsingGameObjectFlags flags)
		{
			UsableMachine usableMachine = ((this.Controller != Agent.ControllerType.AI || this.Formation == null) ? null : (this.Formation.GetDetachmentOrDefault(this) as UsableMachine));
			if (usableMachine == null)
			{
				flags &= ~Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject;
			}
			UsableMissionObject currentlyUsedGameObject = this.CurrentlyUsedGameObject;
			UsableMissionObject usableMissionObject = null;
			if (!this.IsUsingGameObject && this.IsAIControlled)
			{
				if (this.AIMoveToGameObjectIsEnabled())
				{
					usableMissionObject = this.HumanAIComponent.GetCurrentlyMovingGameObject();
				}
				else
				{
					usableMissionObject = this.HumanAIComponent.GetCurrentlyDefendingGameObject();
				}
			}
			if (this.IsUsingGameObject)
			{
				bool flag = this.CurrentlyUsedGameObject.LockUserFrames || this.CurrentlyUsedGameObject.LockUserPositions;
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new StopUsingObject(this, isSuccessful));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this.CurrentlyUsedGameObject.OnUseStopped(this, isSuccessful, this._usedObjectPreferenceIndex);
				this.CurrentlyUsedGameObject = null;
				if (this.IsAIControlled)
				{
					this.AIUseGameObjectDisable();
				}
				this._usedObjectPreferenceIndex = -1;
				if (flag)
				{
					this.ClearTargetFrame();
				}
			}
			else if (this.IsAIControlled)
			{
				if (this.AIDefendGameObjectIsEnabled())
				{
					this.AIDefendGameObjectDisable();
				}
				else
				{
					this.AIMoveToGameObjectDisable();
				}
			}
			if (this.IsAIControlled)
			{
				this.DisableScriptedMovement();
				if (usableMachine != null)
				{
					foreach (StandingPoint standingPoint in usableMachine.StandingPoints)
					{
						standingPoint.FavoredUser = this;
					}
				}
			}
			this.AfterStoppedUsingMissionObject(usableMachine, currentlyUsedGameObject, usableMissionObject, isSuccessful, flags);
			this.Mission.OnObjectStoppedBeingUsed(this, currentlyUsedGameObject);
			this._components.ForEach(delegate(AgentComponent ac)
			{
				ac.OnStopUsingGameObject();
			});
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x00012E58 File Offset: 0x00011058
		public void StopUsingGameObjectMT(bool isSuccessful = true, Agent.StopUsingGameObjectFlags flags = Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject)
		{
			object stopUsingGameObjectLock = Agent._stopUsingGameObjectLock;
			lock (stopUsingGameObjectLock)
			{
				this.StopUsingGameObjectAux(isSuccessful, flags);
			}
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x00012E9C File Offset: 0x0001109C
		public void StopUsingGameObject(bool isSuccessful = true, Agent.StopUsingGameObjectFlags flags = Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject)
		{
			using (new TWParallel.RecursiveSingleThreadTestBlock(TWParallel.RecursiveSingleThreadTestData.GlobalData))
			{
				this.StopUsingGameObjectAux(isSuccessful, flags);
			}
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x00012EE0 File Offset: 0x000110E0
		public void HandleStopUsingAction()
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestStopUsingObject());
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			this.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x00012F06 File Offset: 0x00011106
		public void HandleStartUsingAction(UsableMissionObject targetObject, int preferenceIndex)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new RequestUseObject(targetObject, preferenceIndex));
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			this.UseGameObject(targetObject, preferenceIndex);
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x00012F30 File Offset: 0x00011130
		public AgentController AddController(Type type)
		{
			AgentController agentController = null;
			if (type.IsSubclassOf(typeof(AgentController)))
			{
				agentController = Activator.CreateInstance(type) as AgentController;
			}
			if (agentController != null)
			{
				agentController.Owner = this;
				agentController.Mission = this.Mission;
				this._agentControllers.Add(agentController);
				agentController.OnInitialize();
			}
			return agentController;
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x00012F88 File Offset: 0x00011188
		public AgentController RemoveController(Type type)
		{
			for (int i = 0; i < this._agentControllers.Count; i++)
			{
				if (type.IsInstanceOfType(this._agentControllers[i]))
				{
					AgentController agentController = this._agentControllers[i];
					this._agentControllers.RemoveAt(i);
					return agentController;
				}
			}
			return null;
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x00012FDC File Offset: 0x000111DC
		public bool CanThrustAttackStickToBone(BoneBodyPartType bodyPart)
		{
			if (this.IsHuman)
			{
				BoneBodyPartType[] array = new BoneBodyPartType[]
				{
					BoneBodyPartType.Abdomen,
					BoneBodyPartType.Legs,
					BoneBodyPartType.Chest,
					BoneBodyPartType.Neck,
					BoneBodyPartType.ShoulderLeft,
					BoneBodyPartType.ShoulderRight,
					BoneBodyPartType.ArmLeft,
					BoneBodyPartType.ArmRight
				};
				for (int i = 0; i < array.Length; i++)
				{
					if (bodyPart == array[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x0001301A File Offset: 0x0001121A
		public void StartSwitchingWeaponUsageIndexAsClient(EquipmentIndex equipmentIndex, int usageIndex, Agent.UsageDirection currentMovementFlagUsageDirection)
		{
			MBAPI.IMBAgent.StartSwitchingWeaponUsageIndexAsClient(this.GetPtr(), (int)equipmentIndex, usageIndex, currentMovementFlagUsageDirection);
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x0001302F File Offset: 0x0001122F
		public void TryToWieldWeaponInSlot(EquipmentIndex slotIndex, Agent.WeaponWieldActionType type, bool isWieldedOnSpawn)
		{
			MBAPI.IMBAgent.TryToWieldWeaponInSlot(this.GetPtr(), (int)slotIndex, (int)type, isWieldedOnSpawn);
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x00013044 File Offset: 0x00011244
		public void PrepareWeaponForDropInEquipmentSlot(EquipmentIndex slotIndex, bool dropWithHolster)
		{
			MBAPI.IMBAgent.PrepareWeaponForDropInEquipmentSlot(this.GetPtr(), (int)slotIndex, dropWithHolster);
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00013058 File Offset: 0x00011258
		public void AddHitter(MissionPeer peer, float damage, bool isFriendlyHit)
		{
			Agent.Hitter hitter = this._hitterList.Find((Agent.Hitter h) => h.HitterPeer == peer && h.IsFriendlyHit == isFriendlyHit);
			if (hitter == null)
			{
				hitter = new Agent.Hitter(peer, damage, Environment.TickCount, isFriendlyHit);
				this._hitterList.Add(hitter);
				return;
			}
			hitter.IncreaseDamage(damage);
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x000130C0 File Offset: 0x000112C0
		public void TryToSheathWeaponInHand(Agent.HandIndex handIndex, Agent.WeaponWieldActionType type)
		{
			MBAPI.IMBAgent.TryToSheathWeaponInHand(this.GetPtr(), (int)handIndex, (int)type);
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x000130D4 File Offset: 0x000112D4
		public void RemoveHitter(MissionPeer peer, bool isFriendlyHit)
		{
			Agent.Hitter hitter = this._hitterList.Find((Agent.Hitter h) => h.HitterPeer == peer && h.IsFriendlyHit == isFriendlyHit);
			if (hitter != null)
			{
				this._hitterList.Remove(hitter);
			}
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x0001311D File Offset: 0x0001131D
		public void Retreat(WorldPosition retreatPos)
		{
			MBAPI.IMBAgent.SetRetreatMode(this.GetPtr(), retreatPos, true);
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00013131 File Offset: 0x00011331
		public void StopRetreating()
		{
			MBAPI.IMBAgent.SetRetreatMode(this.GetPtr(), WorldPosition.Invalid, false);
			this.IsRunningAway = false;
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x00013150 File Offset: 0x00011350
		public void UseGameObject(UsableMissionObject usedObject, int preferenceIndex = -1)
		{
			if (usedObject.LockUserFrames)
			{
				WorldFrame userFrameForAgent = usedObject.GetUserFrameForAgent(this);
				this.SetTargetPositionAndDirection(userFrameForAgent.Origin.AsVec2, userFrameForAgent.Rotation.f);
				this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.NoAttack);
			}
			else if (usedObject.LockUserPositions)
			{
				this.SetTargetPosition(usedObject.GetUserFrameForAgent(this).Origin.AsVec2);
				this.SetScriptedFlags(this.GetScriptedFlags() | Agent.AIScriptedFrameFlags.NoAttack);
			}
			if (this.IsActive() && this.IsAIControlled && this.AIMoveToGameObjectIsEnabled())
			{
				this.AIMoveToGameObjectDisable();
				Formation formation = this.Formation;
				if (formation != null)
				{
					formation.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(this);
				}
			}
			this.CurrentlyUsedGameObject = usedObject;
			this._usedObjectPreferenceIndex = preferenceIndex;
			if (this.IsAIControlled)
			{
				this.AIUseGameObjectEnable();
			}
			this._equipmentOnMainHandBeforeUsingObject = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			this._equipmentOnOffHandBeforeUsingObject = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			usedObject.OnUse(this);
			this.Mission.OnObjectUsed(this, usedObject);
			if (usedObject.IsInstantUse && !GameNetwork.IsClientOrReplay && this.IsActive())
			{
				this.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
			}
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x0001326D File Offset: 0x0001146D
		public void StartFadingOut()
		{
			MBAPI.IMBAgent.StartFadingOut(this.GetPtr());
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x0001327F File Offset: 0x0001147F
		public void SetRenderCheckEnabled(bool value)
		{
			MBAPI.IMBAgent.SetRenderCheckEnabled(this.GetPtr(), value);
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x00013292 File Offset: 0x00011492
		public bool GetRenderCheckEnabled()
		{
			return MBAPI.IMBAgent.GetRenderCheckEnabled(this.GetPtr());
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x000132A4 File Offset: 0x000114A4
		public Vec3 ComputeAnimationDisplacement(float dt)
		{
			return MBAPI.IMBAgent.ComputeAnimationDisplacement(this.GetPtr(), dt);
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x000132B7 File Offset: 0x000114B7
		public void TickActionChannels(float dt)
		{
			MBAPI.IMBAgent.TickActionChannels(this.GetPtr(), dt);
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x000132CC File Offset: 0x000114CC
		public void LockAgentReplicationTableDataWithCurrentReliableSequenceNo(NetworkCommunicator peer)
		{
			MBDebug.Print(string.Concat(new object[] { "peer: ", peer.UserName, " index: ", this.Index, " name: ", this.Name }), 0, Debug.DebugColor.White, 17592186044416UL);
			MBAPI.IMBAgent.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(this.GetPtr(), peer.Index);
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x00013344 File Offset: 0x00011544
		public void TeleportToPosition(Vec3 position)
		{
			if (this.MountAgent != null)
			{
				MBAPI.IMBAgent.SetPosition(this.MountAgent.GetPtr(), ref position);
			}
			MBAPI.IMBAgent.SetPosition(this.GetPtr(), ref position);
			if (this.RiderAgent != null)
			{
				MBAPI.IMBAgent.SetPosition(this.RiderAgent.GetPtr(), ref position);
			}
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x000133A1 File Offset: 0x000115A1
		public void FadeOut(bool hideInstantly, bool hideMount)
		{
			MBAPI.IMBAgent.FadeOut(this.GetPtr(), hideInstantly);
			if (hideMount && this.HasMount)
			{
				this.MountAgent.FadeOut(hideMount, false);
			}
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x000133CC File Offset: 0x000115CC
		public void FadeIn()
		{
			MBAPI.IMBAgent.FadeIn(this.GetPtr());
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x000133DE File Offset: 0x000115DE
		public void DisableScriptedMovement()
		{
			MBAPI.IMBAgent.DisableScriptedMovement(this.GetPtr());
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x000133F0 File Offset: 0x000115F0
		public void DisableScriptedCombatMovement()
		{
			MBAPI.IMBAgent.DisableScriptedCombatMovement(this.GetPtr());
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x00013402 File Offset: 0x00011602
		public void ForceAiBehaviorSelection()
		{
			MBAPI.IMBAgent.ForceAiBehaviorSelection(this.GetPtr());
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x00013414 File Offset: 0x00011614
		public bool HasPathThroughNavigationFaceIdFromDirectionMT(int navigationFaceId, Vec2 direction)
		{
			object pathCheckObjectLock = Agent._pathCheckObjectLock;
			bool flag2;
			lock (pathCheckObjectLock)
			{
				flag2 = MBAPI.IMBAgent.HasPathThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, ref direction);
			}
			return flag2;
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x00013464 File Offset: 0x00011664
		public bool HasPathThroughNavigationFaceIdFromDirection(int navigationFaceId, Vec2 direction)
		{
			return MBAPI.IMBAgent.HasPathThroughNavigationFaceIdFromDirection(this.GetPtr(), navigationFaceId, ref direction);
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x00013479 File Offset: 0x00011679
		public void DisableLookToPointOfInterest()
		{
			MBAPI.IMBAgent.DisableLookToPointOfInterest(this.GetPtr());
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x0001348B File Offset: 0x0001168B
		public CompositeComponent AddPrefabComponentToBone(string prefabName, sbyte boneIndex)
		{
			return MBAPI.IMBAgent.AddPrefabToAgentBone(this.GetPtr(), prefabName, boneIndex);
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x0001349F File Offset: 0x0001169F
		public void MakeVoice(SkinVoiceManager.SkinVoiceType voiceType, SkinVoiceManager.CombatVoiceNetworkPredictionType predictionType)
		{
			MBAPI.IMBAgent.MakeVoice(this.GetPtr(), voiceType.Index, (int)predictionType);
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x000134B9 File Offset: 0x000116B9
		public void WieldNextWeapon(Agent.HandIndex weaponIndex, Agent.WeaponWieldActionType wieldActionType = Agent.WeaponWieldActionType.WithAnimation)
		{
			MBAPI.IMBAgent.WieldNextWeapon(this.GetPtr(), (int)weaponIndex, (int)wieldActionType);
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x000134CD File Offset: 0x000116CD
		public Agent.MovementControlFlag AttackDirectionToMovementFlag(Agent.UsageDirection direction)
		{
			return MBAPI.IMBAgent.AttackDirectionToMovementFlag(this.GetPtr(), direction);
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x000134E0 File Offset: 0x000116E0
		public Agent.MovementControlFlag DefendDirectionToMovementFlag(Agent.UsageDirection direction)
		{
			return MBAPI.IMBAgent.DefendDirectionToMovementFlag(this.GetPtr(), direction);
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x000134F3 File Offset: 0x000116F3
		public bool KickClear()
		{
			return MBAPI.IMBAgent.KickClear(this.GetPtr());
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x00013505 File Offset: 0x00011705
		public Agent.UsageDirection PlayerAttackDirection()
		{
			return MBAPI.IMBAgent.PlayerAttackDirection(this.GetPtr());
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x00013518 File Offset: 0x00011718
		public ValueTuple<sbyte, sbyte> GetRandomPairOfRealBloodBurstBoneIndices()
		{
			sbyte b = -1;
			sbyte b2 = -1;
			if (this.Monster.BloodBurstBoneIndices.Length != 0)
			{
				int num = MBRandom.RandomInt(this.Monster.BloodBurstBoneIndices.Length / 2);
				b = this.Monster.BloodBurstBoneIndices[num * 2];
				b2 = this.Monster.BloodBurstBoneIndices[num * 2 + 1];
			}
			return new ValueTuple<sbyte, sbyte>(b, b2);
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x00013575 File Offset: 0x00011775
		public void CreateBloodBurstAtLimb(sbyte realBoneIndex, float scale)
		{
			MBAPI.IMBAgent.CreateBloodBurstAtLimb(this.GetPtr(), realBoneIndex, scale);
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x0001358C File Offset: 0x0001178C
		public void AddComponent(AgentComponent agentComponent)
		{
			this._components.Add(agentComponent);
			CommonAIComponent commonAIComponent;
			if ((commonAIComponent = agentComponent as CommonAIComponent) != null)
			{
				this.CommonAIComponent = commonAIComponent;
				return;
			}
			HumanAIComponent humanAIComponent;
			if ((humanAIComponent = agentComponent as HumanAIComponent) != null)
			{
				this.HumanAIComponent = humanAIComponent;
			}
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x000135C8 File Offset: 0x000117C8
		public bool RemoveComponent(AgentComponent agentComponent)
		{
			bool flag = this._components.Remove(agentComponent);
			if (this.CommonAIComponent == agentComponent)
			{
				this.CommonAIComponent = null;
				return flag;
			}
			if (this.HumanAIComponent == agentComponent)
			{
				this.HumanAIComponent = null;
			}
			return flag;
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x000135F8 File Offset: 0x000117F8
		public void CancelCheering()
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				this.SetActionChannel(1, ActionIndexCache.act_none, true, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x00013640 File Offset: 0x00011840
		public void HandleCheer(int indexOfCheer)
		{
			if (indexOfCheer < Agent.TauntCheerActions.Length && !GameNetwork.IsClientOrReplay)
			{
				this.MakeVoice(SkinVoiceManager.VoiceType.Victory, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				this.SetActionChannel(1, Agent.TauntCheerActions[indexOfCheer], false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x000136A0 File Offset: 0x000118A0
		public void HandleBark(int indexOfBark)
		{
			if (indexOfBark < SkinVoiceManager.VoiceType.MpBarks.Length && !GameNetwork.IsClientOrReplay)
			{
				this.MakeVoice(SkinVoiceManager.VoiceType.MpBarks[indexOfBark], SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
				if (GameNetwork.IsMultiplayer)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new BarkAgent(this, indexOfBark));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, this.MissionPeer.GetNetworkPeer());
				}
			}
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x000136FC File Offset: 0x000118FC
		public void HandleDropWeapon(bool isDefendPressed, EquipmentIndex forcedSlotIndexToDropWeaponFrom)
		{
			Agent.ActionCodeType currentActionType = this.GetCurrentActionType(1);
			if (this.State == AgentState.Active && currentActionType != Agent.ActionCodeType.ReleaseMelee && currentActionType != Agent.ActionCodeType.ReleaseRanged && currentActionType != Agent.ActionCodeType.ReleaseThrowing && currentActionType != Agent.ActionCodeType.WeaponBash)
			{
				EquipmentIndex equipmentIndex = forcedSlotIndexToDropWeaponFrom;
				if (equipmentIndex == EquipmentIndex.None)
				{
					EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
					EquipmentIndex wieldedItemIndex2 = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
					if (wieldedItemIndex2 >= EquipmentIndex.WeaponItemBeginSlot && isDefendPressed)
					{
						equipmentIndex = wieldedItemIndex2;
					}
					else if (wieldedItemIndex >= EquipmentIndex.WeaponItemBeginSlot)
					{
						equipmentIndex = wieldedItemIndex;
					}
					else if (wieldedItemIndex2 >= EquipmentIndex.WeaponItemBeginSlot)
					{
						equipmentIndex = wieldedItemIndex2;
					}
					else
					{
						for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex2++)
						{
							if (!this.Equipment[equipmentIndex2].IsEmpty && this.Equipment[equipmentIndex2].Item.PrimaryWeapon.IsConsumable)
							{
								if (this.Equipment[equipmentIndex2].Item.PrimaryWeapon.IsRangedWeapon)
								{
									if (this.Equipment[equipmentIndex2].Amount == 0)
									{
										equipmentIndex = equipmentIndex2;
										break;
									}
								}
								else
								{
									bool flag = false;
									for (EquipmentIndex equipmentIndex3 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex3 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex3++)
									{
										if (!this.Equipment[equipmentIndex3].IsEmpty && this.Equipment[equipmentIndex3].HasAnyUsageWithAmmoClass(this.Equipment[equipmentIndex2].Item.PrimaryWeapon.WeaponClass) && this.Equipment[equipmentIndex2].Amount > 0)
										{
											flag = true;
											break;
										}
									}
									if (!flag)
									{
										equipmentIndex = equipmentIndex2;
										break;
									}
								}
							}
						}
					}
				}
				if (equipmentIndex != EquipmentIndex.None && !this.Equipment[equipmentIndex].IsEmpty)
				{
					this.DropItem(equipmentIndex, WeaponClass.Undefined);
					this.UpdateAgentProperties();
				}
			}
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x000138CC File Offset: 0x00011ACC
		public void DropItem(EquipmentIndex itemIndex, WeaponClass pickedUpItemType = WeaponClass.Undefined)
		{
			if (this.Equipment[itemIndex].CurrentUsageItem.WeaponFlags.HasAllFlags(WeaponFlags.AffectsArea | WeaponFlags.Burning))
			{
				MatrixFrame boneEntitialFrameWithIndex = this.AgentVisuals.GetSkeleton().GetBoneEntitialFrameWithIndex(this.Monster.MainHandItemBoneIndex);
				MatrixFrame globalFrame = this.AgentVisuals.GetGlobalFrame();
				MatrixFrame matrixFrame = globalFrame.TransformToParent(boneEntitialFrameWithIndex);
				Vec3 vec = globalFrame.origin + globalFrame.rotation.f - matrixFrame.origin;
				vec.Normalize();
				Mat3 identity = Mat3.Identity;
				identity.f = vec;
				identity.Orthonormalize();
				Mission.Current.OnAgentShootMissile(this, itemIndex, matrixFrame.origin, vec, identity, false, false, -1);
				this.RemoveEquippedWeapon(itemIndex);
				return;
			}
			MBAPI.IMBAgent.DropItem(this.GetPtr(), (int)itemIndex, (int)pickedUpItemType);
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x000139A8 File Offset: 0x00011BA8
		public void EquipItemsFromSpawnEquipment(bool neededBatchedItems)
		{
			this.Mission.OnEquipItemsFromSpawnEquipmentBegin(this, this._creationType);
			switch (this._creationType)
			{
			case Agent.CreationType.FromRoster:
			case Agent.CreationType.FromCharacterObj:
			{
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
				{
					WeaponData weaponData = WeaponData.InvalidWeaponData;
					WeaponStatsData[] array = null;
					WeaponData weaponData2 = WeaponData.InvalidWeaponData;
					WeaponStatsData[] array2 = null;
					if (!this.Equipment[equipmentIndex].IsEmpty)
					{
						weaponData = this.Equipment[equipmentIndex].GetWeaponData(neededBatchedItems);
						array = this.Equipment[equipmentIndex].GetWeaponStatsData();
						weaponData2 = this.Equipment[equipmentIndex].GetAmmoWeaponData(neededBatchedItems);
						array2 = this.Equipment[equipmentIndex].GetAmmoWeaponStatsData();
					}
					this.WeaponEquipped(equipmentIndex, weaponData, array, weaponData2, array2, null, true, true);
					weaponData.DeinitializeManagedPointers();
					weaponData2.DeinitializeManagedPointers();
					for (int i = 0; i < this.Equipment[equipmentIndex].GetAttachedWeaponsCount(); i++)
					{
						MatrixFrame attachedWeaponFrame = this.Equipment[equipmentIndex].GetAttachedWeaponFrame(i);
						MissionWeapon attachedWeapon = this.Equipment[equipmentIndex].GetAttachedWeapon(i);
						this.AttachWeaponToWeaponAux(equipmentIndex, ref attachedWeapon, null, ref attachedWeaponFrame);
					}
				}
				this.AddSkinMeshes(!neededBatchedItems);
				break;
			}
			}
			this.UpdateAgentProperties();
			this.Mission.OnEquipItemsFromSpawnEquipment(this, this._creationType);
			this.CheckEquipmentForCapeClothSimulationStateChange();
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x00013B28 File Offset: 0x00011D28
		public void WieldInitialWeapons(Agent.WeaponWieldActionType wieldActionType = Agent.WeaponWieldActionType.InstantAfterPickUp)
		{
			EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			EquipmentIndex wieldedItemIndex2 = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			bool flag;
			this.SpawnEquipment.GetInitialWeaponIndicesToEquip(out wieldedItemIndex, out wieldedItemIndex2, out flag);
			if (wieldedItemIndex2 != EquipmentIndex.None)
			{
				this.TryToWieldWeaponInSlot(wieldedItemIndex2, wieldActionType, true);
			}
			if (wieldedItemIndex != EquipmentIndex.None)
			{
				this.TryToWieldWeaponInSlot(wieldedItemIndex, wieldActionType, true);
				if (this.GetWieldedItemIndex(Agent.HandIndex.MainHand) == EquipmentIndex.None)
				{
					this.WieldNextWeapon(Agent.HandIndex.MainHand, wieldActionType);
				}
			}
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x00013B84 File Offset: 0x00011D84
		public void ChangeWeaponHitPoints(EquipmentIndex slotIndex, short hitPoints)
		{
			this.Equipment.SetHitPointsOfSlot(slotIndex, hitPoints, false);
			this.SetWeaponHitPointsInSlot(slotIndex, hitPoints);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetWeaponNetworkData(this, slotIndex, hitPoints));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			foreach (AgentComponent agentComponent in this._components)
			{
				agentComponent.OnWeaponHPChanged(this.Equipment[slotIndex].Item, (int)hitPoints);
			}
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x00013C20 File Offset: 0x00011E20
		public bool HasWeapon()
		{
			for (int i = 0; i < 5; i++)
			{
				WeaponComponentData currentUsageItem = this.Equipment[i].CurrentUsageItem;
				if (currentUsageItem != null && currentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.WeaponMask))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x00013C63 File Offset: 0x00011E63
		public void AttachWeaponToWeapon(EquipmentIndex slotIndex, MissionWeapon weapon, GameEntity weaponEntity, ref MatrixFrame attachLocalFrame)
		{
			this.Equipment.AttachWeaponToWeaponInSlot(slotIndex, ref weapon, ref attachLocalFrame);
			this.AttachWeaponToWeaponAux(slotIndex, ref weapon, weaponEntity, ref attachLocalFrame);
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x00013C81 File Offset: 0x00011E81
		public void AttachWeaponToBone(MissionWeapon weapon, GameEntity weaponEntity, sbyte boneIndex, ref MatrixFrame attachLocalFrame)
		{
			if (this._attachedWeapons == null)
			{
				this._attachedWeapons = new List<ValueTuple<MissionWeapon, MatrixFrame, sbyte>>();
			}
			this._attachedWeapons.Add(new ValueTuple<MissionWeapon, MatrixFrame, sbyte>(weapon, attachLocalFrame, boneIndex));
			this.AttachWeaponToBoneAux(ref weapon, weaponEntity, boneIndex, ref attachLocalFrame);
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x00013CBC File Offset: 0x00011EBC
		public void RestoreShieldHitPoints()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				if (!this.Equipment[equipmentIndex].IsEmpty && this.Equipment[equipmentIndex].CurrentUsageItem.IsShield)
				{
					this.ChangeWeaponHitPoints(equipmentIndex, this.Equipment[equipmentIndex].ModifiedMaxHitPoints);
				}
			}
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x00013D24 File Offset: 0x00011F24
		public void Die(Blow b, Agent.KillInfo overrideKillInfo = Agent.KillInfo.Invalid)
		{
			if (this.Formation != null)
			{
				this.Formation.Team.QuerySystem.RegisterDeath();
				if (b.IsMissile)
				{
					this.Formation.Team.QuerySystem.RegisterDeathByRanged();
				}
			}
			this.Health = 0f;
			if (overrideKillInfo != Agent.KillInfo.TeamSwitch && (b.OwnerId == -1 || b.OwnerId == this.Index) && this.IsHuman && this._lastHitInfo.CanOverrideBlow)
			{
				b.OwnerId = this._lastHitInfo.LastBlowOwnerId;
				b.AttackType = this._lastHitInfo.LastBlowAttackType;
			}
			MBAPI.IMBAgent.Die(this.GetPtr(), ref b, (sbyte)overrideKillInfo);
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x00013DDF File Offset: 0x00011FDF
		public void MakeDead(bool isKilled, ActionIndexValueCache actionIndex)
		{
			MBAPI.IMBAgent.MakeDead(this.GetPtr(), isKilled, actionIndex.Index);
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x00013DF9 File Offset: 0x00011FF9
		public void RegisterBlow(Blow blow, in AttackCollisionData collisionData)
		{
			this.HandleBlow(ref blow, collisionData);
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x00013E04 File Offset: 0x00012004
		public void CreateBlowFromBlowAsReflection(in Blow blow, in AttackCollisionData collisionData, out Blow outBlow, out AttackCollisionData outCollisionData)
		{
			outBlow = blow;
			outBlow.InflictedDamage = blow.SelfInflictedDamage;
			outBlow.Position = this.Position;
			outBlow.BoneIndex = 0;
			outBlow.BlowFlag = BlowFlags.None;
			outCollisionData = collisionData;
			outCollisionData.UpdateCollisionPositionAndBoneForReflect(collisionData.InflictedDamage, this.Position, 0);
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x00013E64 File Offset: 0x00012064
		public void Tick(float dt)
		{
			if (this.IsActive())
			{
				if (this.GetCurrentActionStage(1) == Agent.ActionStage.AttackQuickReady)
				{
					this._lastQuickReadyDetectedTime = Mission.Current.CurrentTime;
				}
				if (this._checkIfTargetFrameIsChanged)
				{
					Vec2 vec = ((this.MovementLockedState != AgentMovementLockedState.None) ? this.GetTargetPosition() : this.LookFrame.origin.AsVec2);
					Vec3 vec2 = ((this.MovementLockedState != AgentMovementLockedState.None) ? this.GetTargetDirection() : this.LookFrame.rotation.f);
					AgentMovementLockedState movementLockedState = this.MovementLockedState;
					if (movementLockedState != AgentMovementLockedState.PositionLocked)
					{
						if (movementLockedState == AgentMovementLockedState.FrameLocked)
						{
							this._checkIfTargetFrameIsChanged = this._lastSynchedTargetPosition != vec || this._lastSynchedTargetDirection != vec2;
						}
					}
					else
					{
						this._checkIfTargetFrameIsChanged = this._lastSynchedTargetPosition != vec;
					}
					if (this._checkIfTargetFrameIsChanged)
					{
						if (this.MovementLockedState == AgentMovementLockedState.FrameLocked)
						{
							this.SetTargetPositionAndDirection(MBMath.Lerp(vec, this._lastSynchedTargetPosition, 5f * dt, 0.005f), MBMath.Lerp(vec2, this._lastSynchedTargetDirection, 5f * dt, 0.005f));
						}
						else
						{
							this.SetTargetPosition(MBMath.Lerp(vec, this._lastSynchedTargetPosition, 5f * dt, 0.005f));
						}
					}
				}
				if (this.Mission.AllowAiTicking && this.IsAIControlled)
				{
					this.TickAsAI(dt);
				}
				if (this._wantsToYell)
				{
					if (this._yellTimer > 0f)
					{
						this._yellTimer -= dt;
						return;
					}
					this.MakeVoice((this.MountAgent != null) ? SkinVoiceManager.VoiceType.HorseRally : SkinVoiceManager.VoiceType.Yell, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
					this._wantsToYell = false;
					return;
				}
			}
			else
			{
				MissionPeer missionPeer = this.MissionPeer;
				if (((missionPeer != null) ? missionPeer.ControlledAgent : null) == this && !this.IsCameraAttachable())
				{
					this.MissionPeer.ControlledAgent = null;
				}
			}
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x00014021 File Offset: 0x00012221
		[Conditional("DEBUG")]
		public void DebugMore()
		{
			MBAPI.IMBAgent.DebugMore(this.GetPtr());
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x00014034 File Offset: 0x00012234
		public void Mount(Agent mountAgent)
		{
			bool flag = mountAgent.GetCurrentActionType(0) == Agent.ActionCodeType.Rear;
			if (this.MountAgent == null && mountAgent.RiderAgent == null)
			{
				if (this.CheckSkillForMounting(mountAgent) && !flag && this.GetCurrentActionValue(0) == ActionIndexValueCache.act_none)
				{
					this.EventControlFlags |= Agent.EventControlFlag.Mount;
					this.SetInteractionAgent(mountAgent);
					return;
				}
			}
			else if (this.MountAgent == mountAgent && !flag)
			{
				this.EventControlFlags |= Agent.EventControlFlag.Dismount;
			}
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x000140AC File Offset: 0x000122AC
		public void EquipWeaponToExtraSlotAndWield(ref MissionWeapon weapon)
		{
			if (!this.Equipment[EquipmentIndex.ExtraWeaponSlot].IsEmpty)
			{
				this.DropItem(EquipmentIndex.ExtraWeaponSlot, WeaponClass.Undefined);
			}
			this.EquipWeaponWithNewEntity(EquipmentIndex.ExtraWeaponSlot, ref weapon);
			this.TryToWieldWeaponInSlot(EquipmentIndex.ExtraWeaponSlot, Agent.WeaponWieldActionType.InstantAfterPickUp, false);
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x000140E8 File Offset: 0x000122E8
		public void RemoveEquippedWeapon(EquipmentIndex slotIndex)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new RemoveEquippedWeapon(this, slotIndex));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.Equipment[slotIndex] = MissionWeapon.Invalid;
			this.WeaponEquipped(slotIndex, WeaponData.InvalidWeaponData, null, WeaponData.InvalidWeaponData, null, null, true, false);
			this.UpdateAgentProperties();
		}

		// Token: 0x06000A76 RID: 2678 RVA: 0x00014144 File Offset: 0x00012344
		public void EquipWeaponWithNewEntity(EquipmentIndex slotIndex, ref MissionWeapon weapon)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new EquipWeaponWithNewEntity(this, slotIndex, weapon));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.Equipment[slotIndex] = weapon;
			WeaponData weaponData = WeaponData.InvalidWeaponData;
			WeaponStatsData[] array = null;
			WeaponData weaponData2 = WeaponData.InvalidWeaponData;
			WeaponStatsData[] array2 = null;
			if (!weapon.IsEmpty)
			{
				weaponData = weapon.GetWeaponData(true);
				array = weapon.GetWeaponStatsData();
				weaponData2 = weapon.GetAmmoWeaponData(true);
				array2 = weapon.GetAmmoWeaponStatsData();
			}
			this.WeaponEquipped(slotIndex, weaponData, array, weaponData2, array2, null, true, true);
			weaponData.DeinitializeManagedPointers();
			weaponData2.DeinitializeManagedPointers();
			for (int i = 0; i < weapon.GetAttachedWeaponsCount(); i++)
			{
				MissionWeapon attachedWeapon = weapon.GetAttachedWeapon(i);
				MatrixFrame attachedWeaponFrame = weapon.GetAttachedWeaponFrame(i);
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new AttachWeaponToWeaponInAgentEquipmentSlot(attachedWeapon, this, slotIndex, attachedWeaponFrame));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this.AttachWeaponToWeaponAux(slotIndex, ref attachedWeapon, null, ref attachedWeaponFrame);
			}
			this.UpdateAgentProperties();
		}

		// Token: 0x06000A77 RID: 2679 RVA: 0x0001423C File Offset: 0x0001243C
		public void EquipWeaponFromSpawnedItemEntity(EquipmentIndex slotIndex, SpawnedItemEntity spawnedItemEntity, bool removeWeapon)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new EquipWeaponFromSpawnedItemEntity(this, slotIndex, spawnedItemEntity, removeWeapon));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			if (spawnedItemEntity.GameEntity.Parent != null && spawnedItemEntity.GameEntity.Parent.HasScriptOfType<SpawnedItemEntity>())
			{
				SpawnedItemEntity firstScriptOfType = spawnedItemEntity.GameEntity.Parent.GetFirstScriptOfType<SpawnedItemEntity>();
				int num = -1;
				for (int i = 0; i < firstScriptOfType.GameEntity.ChildCount; i++)
				{
					if (firstScriptOfType.GameEntity.GetChild(i) == spawnedItemEntity.GameEntity)
					{
						num = i;
						break;
					}
				}
				firstScriptOfType.WeaponCopy.RemoveAttachedWeapon(num);
			}
			if (removeWeapon)
			{
				if (!this.Equipment[slotIndex].IsEmpty)
				{
					using (new TWSharedMutexWriteLock(Scene.PhysicsAndRayCastLock))
					{
						spawnedItemEntity.GameEntity.Remove(73);
						return;
					}
				}
				GameEntity gameEntity = spawnedItemEntity.GameEntity;
				using (new TWSharedMutexWriteLock(Scene.PhysicsAndRayCastLock))
				{
					gameEntity.RemovePhysicsMT(false);
				}
				gameEntity.RemoveScriptComponent(spawnedItemEntity.ScriptComponent.Pointer, 10);
				gameEntity.SetVisibilityExcludeParents(true);
				MissionWeapon weaponCopy = spawnedItemEntity.WeaponCopy;
				this.Equipment[slotIndex] = weaponCopy;
				WeaponData weaponData = weaponCopy.GetWeaponData(true);
				WeaponStatsData[] weaponStatsData = weaponCopy.GetWeaponStatsData();
				WeaponData ammoWeaponData = weaponCopy.GetAmmoWeaponData(true);
				WeaponStatsData[] ammoWeaponStatsData = weaponCopy.GetAmmoWeaponStatsData();
				this.WeaponEquipped(slotIndex, weaponData, weaponStatsData, ammoWeaponData, ammoWeaponStatsData, gameEntity, true, false);
				weaponData.DeinitializeManagedPointers();
				for (int j = 0; j < weaponCopy.GetAttachedWeaponsCount(); j++)
				{
					MatrixFrame attachedWeaponFrame = weaponCopy.GetAttachedWeaponFrame(j);
					MissionWeapon attachedWeapon = weaponCopy.GetAttachedWeapon(j);
					this.AttachWeaponToWeaponAux(slotIndex, ref attachedWeapon, null, ref attachedWeaponFrame);
				}
				this.UpdateAgentProperties();
			}
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x00014428 File Offset: 0x00012628
		public void PreloadForRendering()
		{
			this.PreloadForRenderingAux();
		}

		// Token: 0x06000A79 RID: 2681 RVA: 0x00014430 File Offset: 0x00012630
		public int AddSynchedPrefabComponentToBone(string prefabName, sbyte boneIndex)
		{
			if (this._synchedBodyComponents == null)
			{
				this._synchedBodyComponents = new List<CompositeComponent>();
			}
			if (!GameEntity.PrefabExists(prefabName))
			{
				MBDebug.ShowWarning("Missing prefab for agent logic :" + prefabName);
				prefabName = "rock_001";
			}
			CompositeComponent compositeComponent = this.AddPrefabComponentToBone(prefabName, boneIndex);
			int count = this._synchedBodyComponents.Count;
			this._synchedBodyComponents.Add(compositeComponent);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new AddPrefabComponentToAgentBone(this, prefabName, boneIndex));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			return count;
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x000144B0 File Offset: 0x000126B0
		public bool WillDropWieldedShield(SpawnedItemEntity spawnedItem)
		{
			EquipmentIndex wieldedItemIndex = this.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			if (wieldedItemIndex != EquipmentIndex.None && spawnedItem.WeaponCopy.CurrentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.NotUsableWithOneHand) && spawnedItem.WeaponCopy.HasAllUsagesWithAnyWeaponFlag(WeaponFlags.NotUsableWithOneHand))
			{
				bool flag = false;
				for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
				{
					if (equipmentIndex != wieldedItemIndex && !this.Equipment[equipmentIndex].IsEmpty && this.Equipment[equipmentIndex].IsShield())
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x00014544 File Offset: 0x00012744
		public bool HadSameTypeOfConsumableOrShieldOnSpawn(WeaponClass weaponClass)
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				if (!this.SpawnEquipment[equipmentIndex].IsEmpty)
				{
					foreach (WeaponComponentData weaponComponentData in this.SpawnEquipment[equipmentIndex].Item.Weapons)
					{
						if ((weaponComponentData.IsConsumable || weaponComponentData.IsShield) && weaponComponentData.WeaponClass == weaponClass)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x000145E8 File Offset: 0x000127E8
		public bool CanAIWieldAsRangedWeapon(MissionWeapon weapon)
		{
			ItemObject item = weapon.Item;
			return !this.IsAIControlled || item == null || !item.ItemFlags.HasAnyFlag(ItemFlags.NotStackable);
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x0001461D File Offset: 0x0001281D
		public override int GetHashCode()
		{
			return this._creationIndex;
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x00014625 File Offset: 0x00012825
		public bool TryGetImmediateEnemyAgentMovementData(out float maximumForwardUnlimitedSpeed, out Vec3 position)
		{
			return MBAPI.IMBAgent.TryGetImmediateEnemyAgentMovementData(this.GetPtr(), out maximumForwardUnlimitedSpeed, out position);
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x0001463C File Offset: 0x0001283C
		public bool HasLostShield()
		{
			for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.ExtraWeaponSlot; equipmentIndex++)
			{
				if (this.Equipment[equipmentIndex].IsEmpty && this.SpawnEquipment[equipmentIndex].Item != null && this.SpawnEquipment[equipmentIndex].Item.PrimaryWeapon.IsShield)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x000146A4 File Offset: 0x000128A4
		internal void SetMountAgentBeforeBuild(Agent mount)
		{
			this.MountAgent = mount;
		}

		// Token: 0x06000A81 RID: 2689 RVA: 0x000146AD File Offset: 0x000128AD
		internal void SetMountInitialValues(TextObject name, string horseCreationKey)
		{
			this._name = name;
			this.HorseCreationKey = horseCreationKey;
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x000146BD File Offset: 0x000128BD
		internal void SetInitialAgentScale(float initialScale)
		{
			MBAPI.IMBAgent.SetAgentScale(this.GetPtr(), initialScale);
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x000146D0 File Offset: 0x000128D0
		internal void InitializeAgentRecord()
		{
			MBAPI.IMBAgent.InitializeAgentRecord(this.GetPtr());
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x000146E2 File Offset: 0x000128E2
		internal void OnDelete()
		{
			this._isDeleted = true;
			this.MissionPeer = null;
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x000146F2 File Offset: 0x000128F2
		internal void OnFleeing()
		{
			this.RelieveFromCaptaincy();
			if (this.Formation != null)
			{
				this.Formation.Team.DetachmentManager.OnAgentRemoved(this);
				this.Formation = null;
			}
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x00014720 File Offset: 0x00012920
		internal void OnRemove()
		{
			this._isRemoved = true;
			this._removalTime = this.Mission.CurrentTime;
			IAgentOriginBase origin = this.Origin;
			if (origin != null)
			{
				origin.OnAgentRemoved(this.Health);
			}
			this.RelieveFromCaptaincy();
			if (this.Formation != null)
			{
				this.Formation.Team.DetachmentManager.OnAgentRemoved(this);
				this.Formation = null;
			}
			if (this.IsUsingGameObject && !GameNetwork.IsClientOrReplay && this.Mission != null && !this.Mission.MissionEnded)
			{
				this.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
			}
			foreach (AgentComponent agentComponent in this._components)
			{
				agentComponent.OnAgentRemoved();
			}
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x000147F8 File Offset: 0x000129F8
		internal void InitializeComponents()
		{
			foreach (AgentComponent agentComponent in this._components)
			{
				agentComponent.Initialize();
			}
		}

		// Token: 0x06000A88 RID: 2696 RVA: 0x00014848 File Offset: 0x00012A48
		internal void Build(AgentBuildData agentBuildData, int creationIndex)
		{
			this.BuildAux();
			this.HasBeenBuilt = true;
			this.Controller = (this.GetAgentFlags().HasAnyFlag(AgentFlag.IsHumanoid) ? agentBuildData.AgentController : Agent.ControllerType.AI);
			this.Formation = ((!this.IsMount) ? ((agentBuildData != null) ? agentBuildData.AgentFormation : null) : null);
			MissionGameModels missionGameModels = MissionGameModels.Current;
			if (missionGameModels != null)
			{
				missionGameModels.AgentStatCalculateModel.InitializeMissionEquipment(this);
			}
			this.InitializeAgentProperties(this.SpawnEquipment, agentBuildData);
			this._creationIndex = creationIndex;
			if (GameNetwork.IsServerOrRecorder)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					if (!networkCommunicator.IsMine && networkCommunicator.IsSynchronized)
					{
						this.LockAgentReplicationTableDataWithCurrentReliableSequenceNo(networkCommunicator);
					}
				}
			}
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x00014920 File Offset: 0x00012B20
		private void PreloadForRenderingAux()
		{
			MBAPI.IMBAgent.PreloadForRendering(this.GetPtr());
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x00014932 File Offset: 0x00012B32
		internal void Clear()
		{
			this.Mission = null;
			this._pointer = UIntPtr.Zero;
			this._positionPointer = UIntPtr.Zero;
			this._flagsPointer = UIntPtr.Zero;
			this._indexPointer = UIntPtr.Zero;
			this._statePointer = UIntPtr.Zero;
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x00014972 File Offset: 0x00012B72
		public bool HasPathThroughNavigationFacesIDFromDirection(int navigationFaceID_1, int navigationFaceID_2, int navigationFaceID_3, Vec2 direction)
		{
			return MBAPI.IMBAgent.HasPathThroughNavigationFacesIDFromDirection(this.GetPtr(), navigationFaceID_1, navigationFaceID_2, navigationFaceID_3, ref direction);
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x0001498C File Offset: 0x00012B8C
		public bool HasPathThroughNavigationFacesIDFromDirectionMT(int navigationFaceID_1, int navigationFaceID_2, int navigationFaceID_3, Vec2 direction)
		{
			object pathCheckObjectLock = Agent._pathCheckObjectLock;
			bool flag2;
			lock (pathCheckObjectLock)
			{
				flag2 = MBAPI.IMBAgent.HasPathThroughNavigationFacesIDFromDirection(this.GetPtr(), navigationFaceID_1, navigationFaceID_2, navigationFaceID_3, ref direction);
			}
			return flag2;
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x000149DC File Offset: 0x00012BDC
		private void AfterStoppedUsingMissionObject(UsableMachine usableMachine, UsableMissionObject usedObject, UsableMissionObject movingToOrDefendingObject, bool isSuccessful, Agent.StopUsingGameObjectFlags flags)
		{
			if (this.IsAIControlled)
			{
				if (flags.HasAnyFlag(Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject))
				{
					Formation formation = this.Formation;
					if (formation != null)
					{
						formation.AttachUnit(this);
					}
				}
				if (flags.HasAnyFlag(Agent.StopUsingGameObjectFlags.DefendAfterStoppingUsingGameObject))
				{
					UsableMissionObject usableMissionObject = usedObject ?? movingToOrDefendingObject;
					this.AIDefendGameObjectEnable(usableMissionObject, usableMachine, Agent.AIScriptedFrameFlags.NoAttack);
				}
			}
			StandingPoint standingPoint;
			if (this.State == AgentState.Active && (standingPoint = usedObject as StandingPoint) != null && standingPoint.AutoEquipWeaponsOnUseStopped && !flags.HasAnyFlag(Agent.StopUsingGameObjectFlags.DoNotWieldWeaponAfterStoppingUsingGameObject))
			{
				bool flag = !isSuccessful;
				bool flag2 = this._equipmentOnMainHandBeforeUsingObject != EquipmentIndex.None;
				if (this._equipmentOnOffHandBeforeUsingObject != EquipmentIndex.None)
				{
					Agent.WeaponWieldActionType weaponWieldActionType = ((flag && !flag2) ? Agent.WeaponWieldActionType.WithAnimation : Agent.WeaponWieldActionType.Instant);
					this.TryToWieldWeaponInSlot(this._equipmentOnOffHandBeforeUsingObject, weaponWieldActionType, false);
				}
				if (flag2)
				{
					Agent.WeaponWieldActionType weaponWieldActionType2 = (flag ? Agent.WeaponWieldActionType.WithAnimation : Agent.WeaponWieldActionType.Instant);
					this.TryToWieldWeaponInSlot(this._equipmentOnMainHandBeforeUsingObject, weaponWieldActionType2, false);
				}
			}
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x00014A9E File Offset: 0x00012C9E
		private UIntPtr GetPtr()
		{
			return this.Pointer;
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x00014AA6 File Offset: 0x00012CA6
		private void SetWeaponHitPointsInSlot(EquipmentIndex equipmentIndex, short hitPoints)
		{
			MBAPI.IMBAgent.SetWeaponHitPointsInSlot(this.GetPtr(), (int)equipmentIndex, hitPoints);
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x00014ABA File Offset: 0x00012CBA
		private AgentMovementLockedState GetMovementLockedState()
		{
			return MBAPI.IMBAgent.GetMovementLockedState(this.GetPtr());
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x00014ACC File Offset: 0x00012CCC
		private void AttachWeaponToBoneAux(ref MissionWeapon weapon, GameEntity weaponEntity, sbyte boneIndex, ref MatrixFrame attachLocalFrame)
		{
			WeaponData weaponData = weapon.GetWeaponData(true);
			MBAPI.IMBAgent.AttachWeaponToBone(this.Pointer, weaponData, weapon.GetWeaponStatsData(), weapon.WeaponsCount, (weaponEntity != null) ? weaponEntity.Pointer : UIntPtr.Zero, boneIndex, ref attachLocalFrame);
			weaponData.DeinitializeManagedPointers();
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x00014B19 File Offset: 0x00012D19
		private Agent GetRiderAgentAux()
		{
			return this._cachedRiderAgent;
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x00014B24 File Offset: 0x00012D24
		private void AttachWeaponToWeaponAux(EquipmentIndex slotIndex, ref MissionWeapon weapon, GameEntity weaponEntity, ref MatrixFrame attachLocalFrame)
		{
			WeaponData weaponData = weapon.GetWeaponData(true);
			MBAPI.IMBAgent.AttachWeaponToWeaponInSlot(this.Pointer, weaponData, weapon.GetWeaponStatsData(), weapon.WeaponsCount, (weaponEntity != null) ? weaponEntity.Pointer : UIntPtr.Zero, (int)slotIndex, ref attachLocalFrame);
			weaponData.DeinitializeManagedPointers();
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x00014B71 File Offset: 0x00012D71
		private Agent GetMountAgentAux()
		{
			return this._cachedMountAgent;
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x00014B7C File Offset: 0x00012D7C
		private void SetMountAgent(Agent mountAgent)
		{
			int num = ((mountAgent == null) ? (-1) : mountAgent.Index);
			MBAPI.IMBAgent.SetMountAgent(this.GetPtr(), num);
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x00014BA8 File Offset: 0x00012DA8
		private void RelieveFromCaptaincy()
		{
			if (this._canLeadFormationsRemotely && this.Team != null)
			{
				using (List<Formation>.Enumerator enumerator = this.Team.FormationsIncludingSpecialAndEmpty.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Formation formation = enumerator.Current;
						if (formation.Captain == this)
						{
							formation.Captain = null;
						}
					}
					return;
				}
			}
			if (this.Formation != null && this.Formation.Captain == this)
			{
				this.Formation.Captain = null;
			}
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x00014C3C File Offset: 0x00012E3C
		private void SetTeamInternal(MBTeam team)
		{
			MBAPI.IMBAgent.SetTeam(this.GetPtr(), team.Index);
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x00014C54 File Offset: 0x00012E54
		private Agent.ControllerType GetController()
		{
			return this._agentControllerType;
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x00014C5C File Offset: 0x00012E5C
		private void SetController(Agent.ControllerType controllerType)
		{
			if (controllerType != this._agentControllerType)
			{
				if (controllerType == Agent.ControllerType.Player && this.IsDetachedFromFormation)
				{
					this._detachment.RemoveAgent(this);
					Formation formation = this._formation;
					if (formation != null)
					{
						formation.AttachUnit(this);
					}
				}
				this._agentControllerType = controllerType;
				MBAPI.IMBAgent.SetController(this.GetPtr(), controllerType);
			}
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x00014CB4 File Offset: 0x00012EB4
		private void WeaponEquipped(EquipmentIndex equipmentSlot, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, in WeaponData ammoWeaponData, WeaponStatsData[] ammoWeaponStatsData, GameEntity weaponEntity, bool removeOldWeaponFromScene, bool isWieldedOnSpawn)
		{
			MBAPI.IMBAgent.WeaponEquipped(this.GetPtr(), (int)equipmentSlot, weaponData, weaponStatsData, (weaponStatsData != null) ? weaponStatsData.Length : 0, ammoWeaponData, ammoWeaponStatsData, (ammoWeaponStatsData != null) ? ammoWeaponStatsData.Length : 0, (weaponEntity != null) ? weaponEntity.Pointer : UIntPtr.Zero, removeOldWeaponFromScene, isWieldedOnSpawn);
			this.CheckEquipmentForCapeClothSimulationStateChange();
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x00014D08 File Offset: 0x00012F08
		private Agent GetRiderAgent()
		{
			return MBAPI.IMBAgent.GetRiderAgent(this.GetPtr());
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x00014D1A File Offset: 0x00012F1A
		public void SetInitialFrame(in Vec3 initialPosition, in Vec2 initialDirection, bool canSpawnOutsideOfMissionBoundary = false)
		{
			MBAPI.IMBAgent.SetInitialFrame(this.GetPtr(), initialPosition, initialDirection, canSpawnOutsideOfMissionBoundary);
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x00014D2F File Offset: 0x00012F2F
		private void UpdateDrivenProperties(float[] values)
		{
			MBAPI.IMBAgent.UpdateDrivenProperties(this.GetPtr(), values);
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x00014D44 File Offset: 0x00012F44
		private void UpdateLastAttackAndHitTimes(Agent attackerAgent, bool isMissile)
		{
			float currentTime = this.Mission.CurrentTime;
			if (isMissile)
			{
				this.LastRangedHitTime = currentTime;
			}
			else
			{
				this.LastMeleeHitTime = currentTime;
			}
			if (attackerAgent != this && attackerAgent != null)
			{
				if (isMissile)
				{
					attackerAgent.LastRangedAttackTime = currentTime;
					return;
				}
				attackerAgent.LastMeleeAttackTime = currentTime;
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x00014D89 File Offset: 0x00012F89
		private void SetNetworkPeer(NetworkCommunicator newPeer)
		{
			MBAPI.IMBAgent.SetNetworkPeer(this.GetPtr(), (newPeer != null) ? newPeer.Index : (-1));
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00014DA7 File Offset: 0x00012FA7
		private void ClearTargetFrameAux()
		{
			MBAPI.IMBAgent.ClearTargetFrame(this.GetPtr());
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00014DB9 File Offset: 0x00012FB9
		[Conditional("_RGL_KEEP_ASSERTS")]
		private void CheckUnmanagedAgentValid()
		{
			AgentHelper.GetAgentIndex(this._indexPointer);
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x00014DC7 File Offset: 0x00012FC7
		private void BuildAux()
		{
			MBAPI.IMBAgent.Build(this.GetPtr(), this.Monster.EyeOffsetWrtHead);
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x00014DE4 File Offset: 0x00012FE4
		private float GetMissileRangeWithHeightDifference()
		{
			if (this.IsMount || !this.IsRangedCached || this.Formation == null || this.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				return 0f;
			}
			return this.GetMissileRangeWithHeightDifferenceAux(this.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.GetNavMeshZ());
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x00014E44 File Offset: 0x00013044
		private void AddSkinMeshes(bool useGPUMorph)
		{
			bool flag = this == Agent.Main;
			SkinMask skinMeshesMask = this.SpawnEquipment.GetSkinMeshesMask();
			bool flag2 = this.IsFemale && this.BodyPropertiesValue.Age >= 14f;
			SkinGenerationParams skinGenerationParams = new SkinGenerationParams((int)skinMeshesMask, this.SpawnEquipment.GetUnderwearType(flag2), (int)this.SpawnEquipment.BodyMeshType, (int)this.SpawnEquipment.HairCoverType, (int)this.SpawnEquipment.BeardCoverType, (int)this.SpawnEquipment.BodyDeformType, flag, this.Character.FaceDirtAmount, this.IsFemale ? 1 : 0, this.Character.Race, false, false);
			bool flag3 = this.Character != null && this.Character.FaceMeshCache;
			this.AgentVisuals.AddSkinMeshes(skinGenerationParams, this.BodyPropertiesValue, useGPUMorph, flag3);
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x00014F20 File Offset: 0x00013120
		private void HandleBlow(ref Blow b, in AttackCollisionData collisionData)
		{
			b.BaseMagnitude = MathF.Min(b.BaseMagnitude, 1000f);
			b.DamagedPercentage = (float)b.InflictedDamage / this.HealthLimit;
			Agent agent = ((b.OwnerId != -1) ? this.Mission.FindAgentWithIndex(b.OwnerId) : this);
			if (!b.BlowFlag.HasAnyFlag(BlowFlags.NoSound))
			{
				bool flag = b.IsBlowCrit(this.Monster.HitPoints * 4);
				bool flag2 = b.IsBlowLow(this.Monster.HitPoints);
				bool flag3 = agent != null && agent.IsHuman;
				bool flag4 = b.BlowFlag.HasAnyFlag(BlowFlags.NonTipThrust);
				int hitSound = b.WeaponRecord.GetHitSound(flag3, flag, flag2, flag4, b.AttackType, b.DamageType);
				float soundParameterForArmorType = Agent.GetSoundParameterForArmorType(this.GetProtectorArmorMaterialOfBone(b.BoneIndex));
				SoundEventParameter soundEventParameter = new SoundEventParameter("Armor Type", soundParameterForArmorType);
				this.Mission.MakeSound(hitSound, b.Position, false, true, b.OwnerId, this.Index, ref soundEventParameter);
				if (b.IsMissile && agent != null)
				{
					int soundCodeMissionCombatPlayerhit = CombatSoundContainer.SoundCodeMissionCombatPlayerhit;
					this.Mission.MakeSoundOnlyOnRelatedPeer(soundCodeMissionCombatPlayerhit, b.Position, agent.Index);
				}
				this.Mission.AddSoundAlarmFactorToAgents(b.OwnerId, b.Position, 15f);
			}
			if (b.InflictedDamage <= 0)
			{
				return;
			}
			this.UpdateLastAttackAndHitTimes(agent, b.IsMissile);
			float health = this.Health;
			float num = (((float)b.InflictedDamage > health) ? health : ((float)b.InflictedDamage));
			float num2 = health - num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			if (this.CurrentMortalityState != Agent.MortalityState.Immortal && !this.Mission.DisableDying)
			{
				this.Health = num2;
			}
			if (agent != null && agent != this && this.IsHuman)
			{
				if (agent.IsMount && agent.RiderAgent != null)
				{
					this._lastHitInfo.RegisterLastBlow(agent.RiderAgent.Index, b.AttackType);
				}
				else if (agent.IsHuman)
				{
					this._lastHitInfo.RegisterLastBlow(b.OwnerId, b.AttackType);
				}
			}
			this.Mission.OnAgentHit(this, agent, b, collisionData, false, num);
			if (this.Health < 1f)
			{
				Agent.KillInfo killInfo = (b.IsFallDamage ? Agent.KillInfo.Gravity : Agent.KillInfo.Invalid);
				this.Die(b, killInfo);
			}
			this.HandleBlowAux(ref b);
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x0001517C File Offset: 0x0001337C
		private void HandleBlowAux(ref Blow b)
		{
			MBAPI.IMBAgent.HandleBlowAux(this.GetPtr(), ref b);
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x00015190 File Offset: 0x00013390
		private ArmorComponent.ArmorMaterialTypes GetProtectorArmorMaterialOfBone(sbyte boneIndex)
		{
			if (boneIndex >= 0)
			{
				EquipmentIndex equipmentIndex = EquipmentIndex.None;
				switch (this.AgentVisuals.GetBoneTypeData(boneIndex).BodyPartType)
				{
				case BoneBodyPartType.Head:
				case BoneBodyPartType.Neck:
					equipmentIndex = EquipmentIndex.NumAllWeaponSlots;
					break;
				case BoneBodyPartType.Chest:
				case BoneBodyPartType.Abdomen:
				case BoneBodyPartType.ShoulderLeft:
				case BoneBodyPartType.ShoulderRight:
					equipmentIndex = EquipmentIndex.Body;
					break;
				case BoneBodyPartType.ArmLeft:
				case BoneBodyPartType.ArmRight:
					equipmentIndex = EquipmentIndex.Gloves;
					break;
				case BoneBodyPartType.Legs:
					equipmentIndex = EquipmentIndex.Leg;
					break;
				}
				if (equipmentIndex != EquipmentIndex.None && this.SpawnEquipment[equipmentIndex].Item != null)
				{
					return this.SpawnEquipment[equipmentIndex].Item.ArmorComponent.MaterialType;
				}
			}
			return ArmorComponent.ArmorMaterialTypes.None;
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x0001522C File Offset: 0x0001342C
		private void TickAsAI(float dt)
		{
			for (int i = 0; i < this._components.Count; i++)
			{
				this._components[i].OnTickAsAI(dt);
			}
			if (this.Formation != null && this._cachedAndFormationValuesUpdateTimer.Check(this.Mission.CurrentTime))
			{
				this.UpdateCachedAndFormationValues(false, true);
			}
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x0001528C File Offset: 0x0001348C
		private void SyncHealthToClients()
		{
			if (this.SyncHealthToAllClients && (!this.IsMount || this.RiderAgent != null))
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetAgentHealth(this, (int)this.Health));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				return;
			}
			NetworkCommunicator networkCommunicator;
			if (!this.IsMount)
			{
				MissionPeer missionPeer = this.MissionPeer;
				networkCommunicator = ((missionPeer != null) ? missionPeer.GetNetworkPeer() : null);
			}
			else
			{
				Agent riderAgent = this.RiderAgent;
				if (riderAgent == null)
				{
					networkCommunicator = null;
				}
				else
				{
					MissionPeer missionPeer2 = riderAgent.MissionPeer;
					networkCommunicator = ((missionPeer2 != null) ? missionPeer2.GetNetworkPeer() : null);
				}
			}
			NetworkCommunicator networkCommunicator2 = networkCommunicator;
			if (networkCommunicator2 != null && !networkCommunicator2.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(networkCommunicator2);
				GameNetwork.WriteMessage(new SetAgentHealth(this, (int)this.Health));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x00015334 File Offset: 0x00013534
		public static Agent.UsageDirection MovementFlagToDirection(Agent.MovementControlFlag flag)
		{
			if (flag.HasAnyFlag(Agent.MovementControlFlag.AttackDown))
			{
				return Agent.UsageDirection.AttackDown;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.AttackUp))
			{
				return Agent.UsageDirection.AttackUp;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.AttackLeft))
			{
				return Agent.UsageDirection.AttackLeft;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.AttackRight))
			{
				return Agent.UsageDirection.AttackRight;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.DefendDown))
			{
				return Agent.UsageDirection.DefendDown;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.DefendUp))
			{
				return Agent.UsageDirection.AttackEnd;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.DefendLeft))
			{
				return Agent.UsageDirection.DefendLeft;
			}
			if (flag.HasAnyFlag(Agent.MovementControlFlag.DefendRight))
			{
				return Agent.UsageDirection.DefendRight;
			}
			return Agent.UsageDirection.None;
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x000153B7 File Offset: 0x000135B7
		public static Agent.UsageDirection GetActionDirection(int actionIndex)
		{
			return MBAPI.IMBAgent.GetActionDirection(actionIndex);
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x000153C4 File Offset: 0x000135C4
		public static int GetMonsterUsageIndex(string monsterUsage)
		{
			return MBAPI.IMBAgent.GetMonsterUsageIndex(monsterUsage);
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x000153D1 File Offset: 0x000135D1
		private static float GetSoundParameterForArmorType(ArmorComponent.ArmorMaterialTypes armorMaterialType)
		{
			return (float)armorMaterialType * 0.1f;
		}

		// Token: 0x0400020B RID: 523
		public const float BecomeTeenagerAge = 14f;

		// Token: 0x0400020C RID: 524
		public const float MaxMountInteractionDistance = 1.75f;

		// Token: 0x0400020D RID: 525
		public const float DismountVelocityLimit = 0.5f;

		// Token: 0x0400020E RID: 526
		public const float HealthDyingThreshold = 1f;

		// Token: 0x0400020F RID: 527
		public const float CachedAndFormationValuesUpdateTime = 0.5f;

		// Token: 0x04000210 RID: 528
		public const float MaxInteractionDistance = 3f;

		// Token: 0x04000211 RID: 529
		public const float MaxFocusDistance = 10f;

		// Token: 0x04000212 RID: 530
		private const float ChainAttackDetectionTimeout = 0.75f;

		// Token: 0x04000213 RID: 531
		public static readonly ActionIndexCache[] TauntCheerActions = new ActionIndexCache[]
		{
			ActionIndexCache.Create("act_taunt_cheer_1"),
			ActionIndexCache.Create("act_taunt_cheer_2"),
			ActionIndexCache.Create("act_taunt_cheer_3"),
			ActionIndexCache.Create("act_taunt_cheer_4")
		};

		// Token: 0x04000214 RID: 532
		private static readonly object _stopUsingGameObjectLock = new object();

		// Token: 0x04000215 RID: 533
		private static readonly object _pathCheckObjectLock = new object();

		// Token: 0x04000216 RID: 534
		public Agent.OnMainAgentWieldedItemChangeDelegate OnMainAgentWieldedItemChange;

		// Token: 0x04000217 RID: 535
		public Action OnAgentMountedStateChanged;

		// Token: 0x04000218 RID: 536
		public Action OnAgentWieldedItemChange;

		// Token: 0x0400021B RID: 539
		public float LastDetachmentTickAgentTime;

		// Token: 0x0400021C RID: 540
		public MissionPeer OwningAgentMissionPeer;

		// Token: 0x0400021D RID: 541
		public MissionRepresentativeBase MissionRepresentative;

		// Token: 0x0400021E RID: 542
		private readonly MBList<AgentComponent> _components;

		// Token: 0x0400021F RID: 543
		private readonly Agent.CreationType _creationType;

		// Token: 0x04000220 RID: 544
		private readonly List<AgentController> _agentControllers;

		// Token: 0x04000221 RID: 545
		private readonly Timer _cachedAndFormationValuesUpdateTimer;

		// Token: 0x04000222 RID: 546
		private Agent.ControllerType _agentControllerType = Agent.ControllerType.AI;

		// Token: 0x04000223 RID: 547
		private Agent _cachedMountAgent;

		// Token: 0x04000224 RID: 548
		private Agent _cachedRiderAgent;

		// Token: 0x04000225 RID: 549
		private BasicCharacterObject _character;

		// Token: 0x04000226 RID: 550
		private uint? _clothingColor1;

		// Token: 0x04000227 RID: 551
		private uint? _clothingColor2;

		// Token: 0x04000228 RID: 552
		private EquipmentIndex _equipmentOnMainHandBeforeUsingObject;

		// Token: 0x04000229 RID: 553
		private EquipmentIndex _equipmentOnOffHandBeforeUsingObject;

		// Token: 0x0400022A RID: 554
		private float _defensiveness;

		// Token: 0x0400022B RID: 555
		private UIntPtr _positionPointer;

		// Token: 0x0400022C RID: 556
		private UIntPtr _pointer;

		// Token: 0x0400022D RID: 557
		private UIntPtr _flagsPointer;

		// Token: 0x0400022E RID: 558
		private UIntPtr _indexPointer;

		// Token: 0x0400022F RID: 559
		private UIntPtr _statePointer;

		// Token: 0x04000230 RID: 560
		private float _lastQuickReadyDetectedTime;

		// Token: 0x04000231 RID: 561
		private Agent _lookAgentCache;

		// Token: 0x04000232 RID: 562
		private IDetachment _detachment;

		// Token: 0x04000233 RID: 563
		private readonly MBList<Agent.Hitter> _hitterList;

		// Token: 0x04000234 RID: 564
		private List<ValueTuple<MissionWeapon, MatrixFrame, sbyte>> _attachedWeapons;

		// Token: 0x04000235 RID: 565
		private float _health;

		// Token: 0x04000236 RID: 566
		private MissionPeer _missionPeer;

		// Token: 0x04000237 RID: 567
		private TextObject _name;

		// Token: 0x04000238 RID: 568
		private float _removalTime;

		// Token: 0x04000239 RID: 569
		private List<CompositeComponent> _synchedBodyComponents;

		// Token: 0x0400023A RID: 570
		private Formation _formation;

		// Token: 0x0400023B RID: 571
		private bool _checkIfTargetFrameIsChanged;

		// Token: 0x0400023C RID: 572
		private Agent.AgentPropertiesModifiers _propertyModifiers;

		// Token: 0x0400023D RID: 573
		private int _usedObjectPreferenceIndex = -1;

		// Token: 0x0400023E RID: 574
		private bool _isDeleted;

		// Token: 0x0400023F RID: 575
		private bool _wantsToYell;

		// Token: 0x04000240 RID: 576
		private float _yellTimer;

		// Token: 0x04000241 RID: 577
		private Vec3 _lastSynchedTargetDirection;

		// Token: 0x04000242 RID: 578
		private Vec2 _lastSynchedTargetPosition;

		// Token: 0x04000243 RID: 579
		private Agent.AgentLastHitInfo _lastHitInfo;

		// Token: 0x04000244 RID: 580
		private ClothSimulatorComponent _capeClothSimulator;

		// Token: 0x04000245 RID: 581
		private bool _isRemoved;

		// Token: 0x04000246 RID: 582
		private WeakReference<MBAgentVisuals> _visualsWeakRef = new WeakReference<MBAgentVisuals>(null);

		// Token: 0x04000247 RID: 583
		private int _creationIndex;

		// Token: 0x04000248 RID: 584
		private bool _canLeadFormationsRemotely;

		// Token: 0x04000249 RID: 585
		private bool _isDetachableFromFormation = true;

		// Token: 0x0400024A RID: 586
		private ItemObject _formationBanner;

		// Token: 0x0400026E RID: 622
		public float DetachmentWeight;

		// Token: 0x02000416 RID: 1046
		public class Hitter
		{
			// Token: 0x17000940 RID: 2368
			// (get) Token: 0x060035AC RID: 13740 RVA: 0x000DEE14 File Offset: 0x000DD014
			// (set) Token: 0x060035AD RID: 13741 RVA: 0x000DEE1C File Offset: 0x000DD01C
			public float Damage { get; private set; }

			// Token: 0x060035AE RID: 13742 RVA: 0x000DEE25 File Offset: 0x000DD025
			public Hitter(MissionPeer peer, float damage, int time, bool isFriendlyHit)
			{
				this.HitterPeer = peer;
				this.Damage = damage;
				this.Time = time;
				this.IsFriendlyHit = isFriendlyHit;
			}

			// Token: 0x060035AF RID: 13743 RVA: 0x000DEE4A File Offset: 0x000DD04A
			public void IncreaseDamage(float amount)
			{
				this.Damage += amount;
			}

			// Token: 0x0400170D RID: 5901
			public const float AssistMinDamage = 35f;

			// Token: 0x0400170E RID: 5902
			public readonly MissionPeer HitterPeer;

			// Token: 0x0400170F RID: 5903
			public readonly bool IsFriendlyHit;

			// Token: 0x04001710 RID: 5904
			public readonly int Time;
		}

		// Token: 0x02000417 RID: 1047
		public struct AgentLastHitInfo
		{
			// Token: 0x17000941 RID: 2369
			// (get) Token: 0x060035B0 RID: 13744 RVA: 0x000DEE5A File Offset: 0x000DD05A
			// (set) Token: 0x060035B1 RID: 13745 RVA: 0x000DEE62 File Offset: 0x000DD062
			public int LastBlowOwnerId { get; private set; }

			// Token: 0x17000942 RID: 2370
			// (get) Token: 0x060035B2 RID: 13746 RVA: 0x000DEE6B File Offset: 0x000DD06B
			// (set) Token: 0x060035B3 RID: 13747 RVA: 0x000DEE73 File Offset: 0x000DD073
			public AgentAttackType LastBlowAttackType { get; private set; }

			// Token: 0x17000943 RID: 2371
			// (get) Token: 0x060035B4 RID: 13748 RVA: 0x000DEE7C File Offset: 0x000DD07C
			public bool CanOverrideBlow
			{
				get
				{
					return this.LastBlowOwnerId >= 0 && this._lastBlowTimer.ElapsedTime <= 5f;
				}
			}

			// Token: 0x060035B5 RID: 13749 RVA: 0x000DEE9E File Offset: 0x000DD09E
			public void Initialize()
			{
				this.LastBlowOwnerId = -1;
				this.LastBlowAttackType = AgentAttackType.Standard;
				this._lastBlowTimer = new BasicMissionTimer();
			}

			// Token: 0x060035B6 RID: 13750 RVA: 0x000DEEB9 File Offset: 0x000DD0B9
			public void RegisterLastBlow(int ownerId, AgentAttackType attackType)
			{
				this._lastBlowTimer.Reset();
				this.LastBlowOwnerId = ownerId;
				this.LastBlowAttackType = attackType;
			}

			// Token: 0x04001712 RID: 5906
			private BasicMissionTimer _lastBlowTimer;
		}

		// Token: 0x02000418 RID: 1048
		public struct AgentPropertiesModifiers
		{
			// Token: 0x04001715 RID: 5909
			public bool resetAiWaitBeforeShootFactor;
		}

		// Token: 0x02000419 RID: 1049
		public struct StackArray8Agent
		{
			// Token: 0x17000944 RID: 2372
			public Agent this[int index]
			{
				get
				{
					switch (index)
					{
					case 0:
						return this._element0;
					case 1:
						return this._element1;
					case 2:
						return this._element2;
					case 3:
						return this._element3;
					case 4:
						return this._element4;
					case 5:
						return this._element5;
					case 6:
						return this._element6;
					case 7:
						return this._element7;
					default:
						return null;
					}
				}
				set
				{
					switch (index)
					{
					case 0:
						this._element0 = value;
						return;
					case 1:
						this._element1 = value;
						return;
					case 2:
						this._element2 = value;
						return;
					case 3:
						this._element3 = value;
						return;
					case 4:
						this._element4 = value;
						return;
					case 5:
						this._element5 = value;
						return;
					case 6:
						this._element6 = value;
						return;
					case 7:
						this._element7 = value;
						return;
					default:
						return;
					}
				}
			}

			// Token: 0x04001716 RID: 5910
			private Agent _element0;

			// Token: 0x04001717 RID: 5911
			private Agent _element1;

			// Token: 0x04001718 RID: 5912
			private Agent _element2;

			// Token: 0x04001719 RID: 5913
			private Agent _element3;

			// Token: 0x0400171A RID: 5914
			private Agent _element4;

			// Token: 0x0400171B RID: 5915
			private Agent _element5;

			// Token: 0x0400171C RID: 5916
			private Agent _element6;

			// Token: 0x0400171D RID: 5917
			private Agent _element7;

			// Token: 0x0400171E RID: 5918
			public const int Length = 8;
		}

		// Token: 0x0200041A RID: 1050
		public enum ActionStage
		{
			// Token: 0x04001720 RID: 5920
			None = -1,
			// Token: 0x04001721 RID: 5921
			AttackReady,
			// Token: 0x04001722 RID: 5922
			AttackQuickReady,
			// Token: 0x04001723 RID: 5923
			AttackRelease,
			// Token: 0x04001724 RID: 5924
			ReloadMidPhase,
			// Token: 0x04001725 RID: 5925
			ReloadLastPhase,
			// Token: 0x04001726 RID: 5926
			Defend,
			// Token: 0x04001727 RID: 5927
			DefendParry,
			// Token: 0x04001728 RID: 5928
			NumActionStages
		}

		// Token: 0x0200041B RID: 1051
		[Flags]
		public enum AIScriptedFrameFlags
		{
			// Token: 0x0400172A RID: 5930
			None = 0,
			// Token: 0x0400172B RID: 5931
			GoToPosition = 1,
			// Token: 0x0400172C RID: 5932
			NoAttack = 2,
			// Token: 0x0400172D RID: 5933
			ConsiderRotation = 4,
			// Token: 0x0400172E RID: 5934
			NeverSlowDown = 8,
			// Token: 0x0400172F RID: 5935
			DoNotRun = 16,
			// Token: 0x04001730 RID: 5936
			GoWithoutMount = 32,
			// Token: 0x04001731 RID: 5937
			RangerCanMoveForClearTarget = 128,
			// Token: 0x04001732 RID: 5938
			InConversation = 256,
			// Token: 0x04001733 RID: 5939
			Crouch = 512
		}

		// Token: 0x0200041C RID: 1052
		[Flags]
		public enum AISpecialCombatModeFlags
		{
			// Token: 0x04001735 RID: 5941
			None = 0,
			// Token: 0x04001736 RID: 5942
			AttackEntity = 1,
			// Token: 0x04001737 RID: 5943
			SurroundAttackEntity = 2,
			// Token: 0x04001738 RID: 5944
			IgnoreAmmoLimitForRangeCalculation = 1024
		}

		// Token: 0x0200041D RID: 1053
		[Flags]
		public enum AIStateFlag
		{
			// Token: 0x0400173A RID: 5946
			None = 0,
			// Token: 0x0400173B RID: 5947
			Cautious = 1,
			// Token: 0x0400173C RID: 5948
			Alarmed = 2,
			// Token: 0x0400173D RID: 5949
			Paused = 4,
			// Token: 0x0400173E RID: 5950
			UseObjectMoving = 8,
			// Token: 0x0400173F RID: 5951
			UseObjectUsing = 16,
			// Token: 0x04001740 RID: 5952
			UseObjectWaiting = 32,
			// Token: 0x04001741 RID: 5953
			Guard = 64,
			// Token: 0x04001742 RID: 5954
			ColumnwiseFollow = 128
		}

		// Token: 0x0200041E RID: 1054
		public enum WatchState
		{
			// Token: 0x04001744 RID: 5956
			Patrolling,
			// Token: 0x04001745 RID: 5957
			Cautious,
			// Token: 0x04001746 RID: 5958
			Alarmed
		}

		// Token: 0x0200041F RID: 1055
		public enum MortalityState
		{
			// Token: 0x04001748 RID: 5960
			Mortal,
			// Token: 0x04001749 RID: 5961
			Invulnerable,
			// Token: 0x0400174A RID: 5962
			Immortal
		}

		// Token: 0x02000420 RID: 1056
		[EngineStruct("Agent_controller_type")]
		public enum ControllerType
		{
			// Token: 0x0400174C RID: 5964
			None,
			// Token: 0x0400174D RID: 5965
			AI,
			// Token: 0x0400174E RID: 5966
			Player,
			// Token: 0x0400174F RID: 5967
			Count
		}

		// Token: 0x02000421 RID: 1057
		public enum CreationType
		{
			// Token: 0x04001751 RID: 5969
			Invalid,
			// Token: 0x04001752 RID: 5970
			FromRoster,
			// Token: 0x04001753 RID: 5971
			FromHorseObj,
			// Token: 0x04001754 RID: 5972
			FromCharacterObj
		}

		// Token: 0x02000422 RID: 1058
		[Flags]
		public enum EventControlFlag : uint
		{
			// Token: 0x04001756 RID: 5974
			Dismount = 1U,
			// Token: 0x04001757 RID: 5975
			Mount = 2U,
			// Token: 0x04001758 RID: 5976
			Rear = 4U,
			// Token: 0x04001759 RID: 5977
			Jump = 8U,
			// Token: 0x0400175A RID: 5978
			Wield0 = 16U,
			// Token: 0x0400175B RID: 5979
			Wield1 = 32U,
			// Token: 0x0400175C RID: 5980
			Wield2 = 64U,
			// Token: 0x0400175D RID: 5981
			Wield3 = 128U,
			// Token: 0x0400175E RID: 5982
			Sheath0 = 256U,
			// Token: 0x0400175F RID: 5983
			Sheath1 = 512U,
			// Token: 0x04001760 RID: 5984
			ToggleAlternativeWeapon = 1024U,
			// Token: 0x04001761 RID: 5985
			Walk = 2048U,
			// Token: 0x04001762 RID: 5986
			Run = 4096U,
			// Token: 0x04001763 RID: 5987
			Crouch = 8192U,
			// Token: 0x04001764 RID: 5988
			Stand = 16384U,
			// Token: 0x04001765 RID: 5989
			Kick = 32768U,
			// Token: 0x04001766 RID: 5990
			DoubleTapToDirectionUp = 65536U,
			// Token: 0x04001767 RID: 5991
			DoubleTapToDirectionDown = 131072U,
			// Token: 0x04001768 RID: 5992
			DoubleTapToDirectionLeft = 196608U,
			// Token: 0x04001769 RID: 5993
			DoubleTapToDirectionRight = 262144U,
			// Token: 0x0400176A RID: 5994
			DoubleTapToDirectionMask = 458752U
		}

		// Token: 0x02000423 RID: 1059
		public enum FacialAnimChannel
		{
			// Token: 0x0400176C RID: 5996
			High,
			// Token: 0x0400176D RID: 5997
			Mid,
			// Token: 0x0400176E RID: 5998
			Low,
			// Token: 0x0400176F RID: 5999
			num_facial_anim_channels
		}

		// Token: 0x02000424 RID: 1060
		public enum ActionCodeType
		{
			// Token: 0x04001771 RID: 6001
			Other,
			// Token: 0x04001772 RID: 6002
			DefendFist,
			// Token: 0x04001773 RID: 6003
			DefendShield,
			// Token: 0x04001774 RID: 6004
			DefendForward2h,
			// Token: 0x04001775 RID: 6005
			DefendUp2h,
			// Token: 0x04001776 RID: 6006
			DefendRight2h,
			// Token: 0x04001777 RID: 6007
			DefendLeft2h,
			// Token: 0x04001778 RID: 6008
			DefendForward1h,
			// Token: 0x04001779 RID: 6009
			DefendUp1h,
			// Token: 0x0400177A RID: 6010
			DefendRight1h,
			// Token: 0x0400177B RID: 6011
			DefendLeft1h,
			// Token: 0x0400177C RID: 6012
			DefendForwardStaff,
			// Token: 0x0400177D RID: 6013
			DefendUpStaff,
			// Token: 0x0400177E RID: 6014
			DefendRightStaff,
			// Token: 0x0400177F RID: 6015
			DefendLeftStaff,
			// Token: 0x04001780 RID: 6016
			ReadyRanged,
			// Token: 0x04001781 RID: 6017
			ReleaseRanged,
			// Token: 0x04001782 RID: 6018
			ReleaseThrowing,
			// Token: 0x04001783 RID: 6019
			Reload,
			// Token: 0x04001784 RID: 6020
			ReadyMelee,
			// Token: 0x04001785 RID: 6021
			ReleaseMelee,
			// Token: 0x04001786 RID: 6022
			ParriedMelee,
			// Token: 0x04001787 RID: 6023
			BlockedMelee,
			// Token: 0x04001788 RID: 6024
			Fall,
			// Token: 0x04001789 RID: 6025
			JumpStart,
			// Token: 0x0400178A RID: 6026
			Jump,
			// Token: 0x0400178B RID: 6027
			JumpEnd,
			// Token: 0x0400178C RID: 6028
			JumpEndHard,
			// Token: 0x0400178D RID: 6029
			Kick,
			// Token: 0x0400178E RID: 6030
			KickContinue,
			// Token: 0x0400178F RID: 6031
			KickHit,
			// Token: 0x04001790 RID: 6032
			WeaponBash,
			// Token: 0x04001791 RID: 6033
			PassiveUsage,
			// Token: 0x04001792 RID: 6034
			EquipUnequip,
			// Token: 0x04001793 RID: 6035
			Idle,
			// Token: 0x04001794 RID: 6036
			Guard,
			// Token: 0x04001795 RID: 6037
			Mount,
			// Token: 0x04001796 RID: 6038
			Dismount,
			// Token: 0x04001797 RID: 6039
			Dash,
			// Token: 0x04001798 RID: 6040
			MountQuickStop,
			// Token: 0x04001799 RID: 6041
			HitObject,
			// Token: 0x0400179A RID: 6042
			Sit,
			// Token: 0x0400179B RID: 6043
			SitOnTheFloor,
			// Token: 0x0400179C RID: 6044
			SitOnAThrone,
			// Token: 0x0400179D RID: 6045
			LadderRaise,
			// Token: 0x0400179E RID: 6046
			LadderRaiseEnd,
			// Token: 0x0400179F RID: 6047
			Rear,
			// Token: 0x040017A0 RID: 6048
			StrikeLight,
			// Token: 0x040017A1 RID: 6049
			StrikeMedium,
			// Token: 0x040017A2 RID: 6050
			StrikeHeavy,
			// Token: 0x040017A3 RID: 6051
			StrikeKnockBack,
			// Token: 0x040017A4 RID: 6052
			MountStrike,
			// Token: 0x040017A5 RID: 6053
			Count,
			// Token: 0x040017A6 RID: 6054
			StrikeBegin = 47,
			// Token: 0x040017A7 RID: 6055
			StrikeEnd = 51,
			// Token: 0x040017A8 RID: 6056
			DefendAllBegin = 1,
			// Token: 0x040017A9 RID: 6057
			DefendAllEnd = 15,
			// Token: 0x040017AA RID: 6058
			AttackMeleeAllBegin = 19,
			// Token: 0x040017AB RID: 6059
			AttackMeleeAllEnd = 23,
			// Token: 0x040017AC RID: 6060
			CombatAllBegin = 1,
			// Token: 0x040017AD RID: 6061
			CombatAllEnd = 23,
			// Token: 0x040017AE RID: 6062
			JumpAllBegin,
			// Token: 0x040017AF RID: 6063
			JumpAllEnd = 28
		}

		// Token: 0x02000425 RID: 1061
		[EngineStruct("Agent_guard_mode")]
		public enum GuardMode
		{
			// Token: 0x040017B1 RID: 6065
			None = -1,
			// Token: 0x040017B2 RID: 6066
			Up,
			// Token: 0x040017B3 RID: 6067
			Down,
			// Token: 0x040017B4 RID: 6068
			Left,
			// Token: 0x040017B5 RID: 6069
			Right
		}

		// Token: 0x02000426 RID: 1062
		public enum HandIndex
		{
			// Token: 0x040017B7 RID: 6071
			MainHand,
			// Token: 0x040017B8 RID: 6072
			OffHand
		}

		// Token: 0x02000427 RID: 1063
		public enum KillInfo : sbyte
		{
			// Token: 0x040017BA RID: 6074
			Invalid = -1,
			// Token: 0x040017BB RID: 6075
			Headshot,
			// Token: 0x040017BC RID: 6076
			CouchedLance,
			// Token: 0x040017BD RID: 6077
			Punch,
			// Token: 0x040017BE RID: 6078
			MountHit,
			// Token: 0x040017BF RID: 6079
			Bow,
			// Token: 0x040017C0 RID: 6080
			Crossbow,
			// Token: 0x040017C1 RID: 6081
			ThrowingAxe,
			// Token: 0x040017C2 RID: 6082
			ThrowingKnife,
			// Token: 0x040017C3 RID: 6083
			Javelin,
			// Token: 0x040017C4 RID: 6084
			Stone,
			// Token: 0x040017C5 RID: 6085
			Pistol,
			// Token: 0x040017C6 RID: 6086
			Musket,
			// Token: 0x040017C7 RID: 6087
			OneHandedSword,
			// Token: 0x040017C8 RID: 6088
			TwoHandedSword,
			// Token: 0x040017C9 RID: 6089
			OneHandedAxe,
			// Token: 0x040017CA RID: 6090
			TwoHandedAxe,
			// Token: 0x040017CB RID: 6091
			Mace,
			// Token: 0x040017CC RID: 6092
			Spear,
			// Token: 0x040017CD RID: 6093
			Morningstar,
			// Token: 0x040017CE RID: 6094
			Maul,
			// Token: 0x040017CF RID: 6095
			Backstabbed,
			// Token: 0x040017D0 RID: 6096
			Gravity,
			// Token: 0x040017D1 RID: 6097
			ShieldBash,
			// Token: 0x040017D2 RID: 6098
			WeaponBash,
			// Token: 0x040017D3 RID: 6099
			Kick,
			// Token: 0x040017D4 RID: 6100
			TeamSwitch
		}

		// Token: 0x02000428 RID: 1064
		public enum MovementBehaviorType
		{
			// Token: 0x040017D6 RID: 6102
			Engaged,
			// Token: 0x040017D7 RID: 6103
			Idle,
			// Token: 0x040017D8 RID: 6104
			Flee
		}

		// Token: 0x02000429 RID: 1065
		[Flags]
		public enum MovementControlFlag : uint
		{
			// Token: 0x040017DA RID: 6106
			Forward = 1U,
			// Token: 0x040017DB RID: 6107
			Backward = 2U,
			// Token: 0x040017DC RID: 6108
			StrafeRight = 4U,
			// Token: 0x040017DD RID: 6109
			StrafeLeft = 8U,
			// Token: 0x040017DE RID: 6110
			TurnRight = 16U,
			// Token: 0x040017DF RID: 6111
			TurnLeft = 32U,
			// Token: 0x040017E0 RID: 6112
			AttackLeft = 64U,
			// Token: 0x040017E1 RID: 6113
			AttackRight = 128U,
			// Token: 0x040017E2 RID: 6114
			AttackUp = 256U,
			// Token: 0x040017E3 RID: 6115
			AttackDown = 512U,
			// Token: 0x040017E4 RID: 6116
			DefendLeft = 1024U,
			// Token: 0x040017E5 RID: 6117
			DefendRight = 2048U,
			// Token: 0x040017E6 RID: 6118
			DefendUp = 4096U,
			// Token: 0x040017E7 RID: 6119
			DefendDown = 8192U,
			// Token: 0x040017E8 RID: 6120
			DefendAuto = 16384U,
			// Token: 0x040017E9 RID: 6121
			DefendBlock = 32768U,
			// Token: 0x040017EA RID: 6122
			Action = 65536U,
			// Token: 0x040017EB RID: 6123
			AttackMask = 960U,
			// Token: 0x040017EC RID: 6124
			DefendMask = 31744U,
			// Token: 0x040017ED RID: 6125
			DefendDirMask = 15360U,
			// Token: 0x040017EE RID: 6126
			MoveMask = 63U
		}

		// Token: 0x0200042A RID: 1066
		public enum UnderAttackType
		{
			// Token: 0x040017F0 RID: 6128
			NotUnderAttack,
			// Token: 0x040017F1 RID: 6129
			UnderMeleeAttack,
			// Token: 0x040017F2 RID: 6130
			UnderRangedAttack
		}

		// Token: 0x0200042B RID: 1067
		[EngineStruct("Usage_direction")]
		public enum UsageDirection
		{
			// Token: 0x040017F4 RID: 6132
			None = -1,
			// Token: 0x040017F5 RID: 6133
			AttackUp,
			// Token: 0x040017F6 RID: 6134
			AttackDown,
			// Token: 0x040017F7 RID: 6135
			AttackLeft,
			// Token: 0x040017F8 RID: 6136
			AttackRight,
			// Token: 0x040017F9 RID: 6137
			AttackBegin = 0,
			// Token: 0x040017FA RID: 6138
			AttackEnd = 4,
			// Token: 0x040017FB RID: 6139
			DefendUp = 4,
			// Token: 0x040017FC RID: 6140
			DefendDown,
			// Token: 0x040017FD RID: 6141
			DefendLeft,
			// Token: 0x040017FE RID: 6142
			DefendRight,
			// Token: 0x040017FF RID: 6143
			DefendBegin = 4,
			// Token: 0x04001800 RID: 6144
			DefendAny = 8,
			// Token: 0x04001801 RID: 6145
			DefendEnd,
			// Token: 0x04001802 RID: 6146
			AttackAny = 9
		}

		// Token: 0x0200042C RID: 1068
		[EngineStruct("Weapon_wield_action_type")]
		public enum WeaponWieldActionType
		{
			// Token: 0x04001804 RID: 6148
			WithAnimation,
			// Token: 0x04001805 RID: 6149
			Instant,
			// Token: 0x04001806 RID: 6150
			InstantAfterPickUp,
			// Token: 0x04001807 RID: 6151
			WithAnimationUninterruptible
		}

		// Token: 0x0200042D RID: 1069
		[Flags]
		public enum StopUsingGameObjectFlags : byte
		{
			// Token: 0x04001809 RID: 6153
			None = 0,
			// Token: 0x0400180A RID: 6154
			AutoAttachAfterStoppingUsingGameObject = 1,
			// Token: 0x0400180B RID: 6155
			DoNotWieldWeaponAfterStoppingUsingGameObject = 2,
			// Token: 0x0400180C RID: 6156
			DefendAfterStoppingUsingGameObject = 4
		}

		// Token: 0x0200042E RID: 1070
		// (Invoke) Token: 0x060035BA RID: 13754
		public delegate void OnAgentHealthChangedDelegate(Agent agent, float oldHealth, float newHealth);

		// Token: 0x0200042F RID: 1071
		// (Invoke) Token: 0x060035BE RID: 13758
		public delegate void OnMountHealthChangedDelegate(Agent agent, Agent mount, float oldHealth, float newHealth);

		// Token: 0x02000430 RID: 1072
		// (Invoke) Token: 0x060035C2 RID: 13762
		public delegate void OnMainAgentWieldedItemChangeDelegate();
	}
}
