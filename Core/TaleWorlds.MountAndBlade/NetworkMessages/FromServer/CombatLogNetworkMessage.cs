using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CombatLogNetworkMessage : GameNetworkMessage
	{
		private Agent AttackerAgent { get; set; }

		private Agent VictimAgent { get; set; }

		private bool IsVictimEntity { get; set; }

		public DamageTypes DamageType { get; private set; }

		public bool CrushedThrough { get; private set; }

		public bool Chamber { get; private set; }

		public bool IsRangedAttack { get; private set; }

		public bool IsFriendlyFire { get; private set; }

		public bool IsFatalDamage { get; private set; }

		public BoneBodyPartType BodyPartHit { get; private set; }

		public float HitSpeed { get; private set; }

		public float Distance { get; private set; }

		public int InflictedDamage { get; private set; }

		public int AbsorbedDamage { get; private set; }

		public int ModifiedDamage { get; private set; }

		public CombatLogNetworkMessage()
		{
		}

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

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Agent got hit.";
		}
	}
}
