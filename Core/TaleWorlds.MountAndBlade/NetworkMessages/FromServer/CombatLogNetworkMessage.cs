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
		public int AttackerAgentIndex { get; private set; }

		public int VictimAgentIndex { get; private set; }

		public bool IsVictimEntity { get; private set; }

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

		public CombatLogNetworkMessage(int attackerAgentIndex, int victimAgentIndex, GameEntity hitEntity, CombatLogData combatLogData)
		{
			this.AttackerAgentIndex = attackerAgentIndex;
			this.VictimAgentIndex = victimAgentIndex;
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

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AttackerAgentIndex);
			GameNetworkMessage.WriteAgentIndexToPacket(this.VictimAgentIndex);
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
			this.AttackerAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.VictimAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
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
