using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000070 RID: 112
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CombatLogNetworkMessage : GameNetworkMessage
	{
		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x00007AC9 File Offset: 0x00005CC9
		// (set) Token: 0x0600040E RID: 1038 RVA: 0x00007AD1 File Offset: 0x00005CD1
		private Agent AttackerAgent { get; set; }

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x00007ADA File Offset: 0x00005CDA
		// (set) Token: 0x06000410 RID: 1040 RVA: 0x00007AE2 File Offset: 0x00005CE2
		private Agent VictimAgent { get; set; }

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000411 RID: 1041 RVA: 0x00007AEB File Offset: 0x00005CEB
		// (set) Token: 0x06000412 RID: 1042 RVA: 0x00007AF3 File Offset: 0x00005CF3
		private bool IsVictimEntity { get; set; }

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000413 RID: 1043 RVA: 0x00007AFC File Offset: 0x00005CFC
		// (set) Token: 0x06000414 RID: 1044 RVA: 0x00007B04 File Offset: 0x00005D04
		public DamageTypes DamageType { get; private set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000415 RID: 1045 RVA: 0x00007B0D File Offset: 0x00005D0D
		// (set) Token: 0x06000416 RID: 1046 RVA: 0x00007B15 File Offset: 0x00005D15
		public bool CrushedThrough { get; private set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x00007B1E File Offset: 0x00005D1E
		// (set) Token: 0x06000418 RID: 1048 RVA: 0x00007B26 File Offset: 0x00005D26
		public bool Chamber { get; private set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000419 RID: 1049 RVA: 0x00007B2F File Offset: 0x00005D2F
		// (set) Token: 0x0600041A RID: 1050 RVA: 0x00007B37 File Offset: 0x00005D37
		public bool IsRangedAttack { get; private set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600041B RID: 1051 RVA: 0x00007B40 File Offset: 0x00005D40
		// (set) Token: 0x0600041C RID: 1052 RVA: 0x00007B48 File Offset: 0x00005D48
		public bool IsFriendlyFire { get; private set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x0600041D RID: 1053 RVA: 0x00007B51 File Offset: 0x00005D51
		// (set) Token: 0x0600041E RID: 1054 RVA: 0x00007B59 File Offset: 0x00005D59
		public bool IsFatalDamage { get; private set; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x0600041F RID: 1055 RVA: 0x00007B62 File Offset: 0x00005D62
		// (set) Token: 0x06000420 RID: 1056 RVA: 0x00007B6A File Offset: 0x00005D6A
		public BoneBodyPartType BodyPartHit { get; private set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000421 RID: 1057 RVA: 0x00007B73 File Offset: 0x00005D73
		// (set) Token: 0x06000422 RID: 1058 RVA: 0x00007B7B File Offset: 0x00005D7B
		public float HitSpeed { get; private set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x00007B84 File Offset: 0x00005D84
		// (set) Token: 0x06000424 RID: 1060 RVA: 0x00007B8C File Offset: 0x00005D8C
		public float Distance { get; private set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000425 RID: 1061 RVA: 0x00007B95 File Offset: 0x00005D95
		// (set) Token: 0x06000426 RID: 1062 RVA: 0x00007B9D File Offset: 0x00005D9D
		public int InflictedDamage { get; private set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x00007BA6 File Offset: 0x00005DA6
		// (set) Token: 0x06000428 RID: 1064 RVA: 0x00007BAE File Offset: 0x00005DAE
		public int AbsorbedDamage { get; private set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x00007BB7 File Offset: 0x00005DB7
		// (set) Token: 0x0600042A RID: 1066 RVA: 0x00007BBF File Offset: 0x00005DBF
		public int ModifiedDamage { get; private set; }

		// Token: 0x0600042B RID: 1067 RVA: 0x00007BC8 File Offset: 0x00005DC8
		public CombatLogNetworkMessage()
		{
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00007BD0 File Offset: 0x00005DD0
		public CombatLogNetworkMessage(Agent attackerAgent, Agent victimAgent, GameEntity hitEntity, CombatLogData combatLogData)
		{
			this.AttackerAgent = attackerAgent;
			this.VictimAgent = victimAgent;
			this.IsVictimEntity = hitEntity != null;
			this.DamageType = combatLogData.DamageType;
			this.CrushedThrough = combatLogData.CrushedThrough;
			this.Chamber = combatLogData.Chamber;
			this.IsRangedAttack = combatLogData.IsRangedAttack;
			this.IsFriendlyFire = combatLogData.IsFriendlyFire;
			this.IsFatalDamage = combatLogData.IsFatalDamage;
			this.BodyPartHit = combatLogData.BodyPartHit;
			this.HitSpeed = combatLogData.HitSpeed;
			this.Distance = combatLogData.Distance;
			this.InflictedDamage = combatLogData.InflictedDamage;
			this.AbsorbedDamage = combatLogData.AbsorbedDamage;
			this.ModifiedDamage = combatLogData.ModifiedDamage;
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00007C9C File Offset: 0x00005E9C
		public CombatLogData GetData()
		{
			bool flag = this.AttackerAgent != null;
			bool flag2 = flag && this.AttackerAgent.IsHuman;
			bool flag3 = flag && this.AttackerAgent.IsMine;
			bool flag4 = flag && this.AttackerAgent.RiderAgent != null;
			bool flag5 = flag4 && this.AttackerAgent.RiderAgent.IsMine;
			bool flag6 = flag && this.AttackerAgent.IsMount;
			Agent victimAgent = this.VictimAgent;
			bool flag7 = victimAgent != null && victimAgent.Health <= 0f;
			bool flag8;
			if (this.AttackerAgent != null)
			{
				Agent victimAgent2 = this.VictimAgent;
				flag8 = ((victimAgent2 != null) ? victimAgent2.RiderAgent : null) == this.AttackerAgent;
			}
			else
			{
				flag8 = false;
			}
			bool flag9 = flag8;
			bool flag10 = this.AttackerAgent == this.VictimAgent;
			bool flag11 = flag2;
			bool flag12 = flag3;
			bool flag13 = flag4;
			bool flag14 = flag5;
			bool flag15 = flag6;
			Agent victimAgent3 = this.VictimAgent;
			bool flag16 = victimAgent3 != null && victimAgent3.IsHuman;
			Agent victimAgent4 = this.VictimAgent;
			bool flag17 = victimAgent4 != null && victimAgent4.IsMine;
			bool flag18 = flag7;
			Agent victimAgent5 = this.VictimAgent;
			bool flag19 = ((victimAgent5 != null) ? victimAgent5.RiderAgent : null) != null;
			Agent victimAgent6 = this.VictimAgent;
			bool? flag20;
			if (victimAgent6 == null)
			{
				flag20 = null;
			}
			else
			{
				Agent riderAgent = victimAgent6.RiderAgent;
				flag20 = ((riderAgent != null) ? new bool?(riderAgent.IsMine) : null);
			}
			bool flag21 = flag20 ?? false;
			Agent victimAgent7 = this.VictimAgent;
			CombatLogData combatLogData = new CombatLogData(flag10, flag11, flag12, flag13, flag14, flag15, flag16, flag17, flag18, flag19, flag21, victimAgent7 != null && victimAgent7.IsMount, this.IsVictimEntity, flag9, this.CrushedThrough, this.Chamber, this.Distance);
			combatLogData.DamageType = this.DamageType;
			combatLogData.IsRangedAttack = this.IsRangedAttack;
			combatLogData.IsFriendlyFire = this.IsFriendlyFire;
			combatLogData.IsFatalDamage = this.IsFatalDamage;
			combatLogData.BodyPartHit = this.BodyPartHit;
			combatLogData.HitSpeed = this.HitSpeed;
			combatLogData.InflictedDamage = this.InflictedDamage;
			combatLogData.AbsorbedDamage = this.AbsorbedDamage;
			combatLogData.ModifiedDamage = this.ModifiedDamage;
			Agent victimAgent8 = this.VictimAgent;
			string text;
			if (victimAgent8 == null)
			{
				text = null;
			}
			else
			{
				MissionPeer missionPeer = victimAgent8.MissionPeer;
				text = ((missionPeer != null) ? missionPeer.DisplayedName : null);
			}
			string text2;
			if ((text2 = text) == null)
			{
				Agent victimAgent9 = this.VictimAgent;
				text2 = ((victimAgent9 != null) ? victimAgent9.Name : null) ?? "";
			}
			combatLogData.VictimAgentName = text2;
			return combatLogData;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x00007EE0 File Offset: 0x000060E0
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.AttackerAgent);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.VictimAgent);
			GameNetworkMessage.WriteBoolToPacket(this.IsVictimEntity);
			GameNetworkMessage.WriteIntToPacket((int)this.DamageType, CompressionBasic.AgentHitDamageTypeCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.CrushedThrough);
			GameNetworkMessage.WriteBoolToPacket(this.Chamber);
			GameNetworkMessage.WriteBoolToPacket(this.IsRangedAttack);
			GameNetworkMessage.WriteBoolToPacket(this.IsFriendlyFire);
			GameNetworkMessage.WriteBoolToPacket(this.IsFatalDamage);
			GameNetworkMessage.WriteIntToPacket((int)this.BodyPartHit, CompressionBasic.AgentHitBodyPartCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.HitSpeed, CompressionBasic.AgentHitRelativeSpeedCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.Distance, CompressionBasic.AgentHitRelativeSpeedCompressionInfo);
			this.AbsorbedDamage = MBMath.ClampInt(this.AbsorbedDamage, 0, 2000);
			this.InflictedDamage = MBMath.ClampInt(this.InflictedDamage, 0, 2000);
			this.ModifiedDamage = MBMath.ClampInt(this.ModifiedDamage, -2000, 2000);
			GameNetworkMessage.WriteIntToPacket(this.AbsorbedDamage, CompressionBasic.AgentHitDamageCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.InflictedDamage, CompressionBasic.AgentHitDamageCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.ModifiedDamage, CompressionBasic.AgentHitModifiedDamageCompressionInfo);
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00008000 File Offset: 0x00006200
		protected override bool OnRead()
		{
			bool flag = true;
			this.AttackerAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.VictimAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			this.IsVictimEntity = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.DamageType = (DamageTypes)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AgentHitDamageTypeCompressionInfo, ref flag);
			this.CrushedThrough = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.Chamber = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsRangedAttack = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsFriendlyFire = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsFatalDamage = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.BodyPartHit = (BoneBodyPartType)GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AgentHitBodyPartCompressionInfo, ref flag);
			this.HitSpeed = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AgentHitRelativeSpeedCompressionInfo, ref flag);
			this.Distance = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AgentHitRelativeSpeedCompressionInfo, ref flag);
			this.AbsorbedDamage = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AgentHitDamageCompressionInfo, ref flag);
			this.InflictedDamage = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AgentHitDamageCompressionInfo, ref flag);
			this.ModifiedDamage = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AgentHitModifiedDamageCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x000080F9 File Offset: 0x000062F9
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x00008101 File Offset: 0x00006301
		protected override string OnGetLogFormat()
		{
			return "Agent got hit.";
		}
	}
}
