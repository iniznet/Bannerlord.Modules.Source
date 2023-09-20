using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public struct CombatLogData
	{
		private bool IsValidForPlayer
		{
			get
			{
				return this.IsImportant && (this.IsAttackerPlayer || this.IsVictimPlayer);
			}
		}

		private bool IsImportant
		{
			get
			{
				return this.TotalDamage > 0 || this.CrushedThrough || this.Chamber;
			}
		}

		private bool IsAttackerPlayer
		{
			get
			{
				if (!this.IsAttackerAgentHuman)
				{
					return this.DoesAttackerAgentHaveRiderAgent && this.IsAttackerAgentRiderAgentMine;
				}
				return this.IsAttackerAgentMine;
			}
		}

		private bool IsVictimPlayer
		{
			get
			{
				if (!this.IsVictimAgentHuman)
				{
					return this.DoesVictimAgentHaveRiderAgent && this.IsVictimAgentRiderAgentMine;
				}
				return this.IsVictimAgentMine;
			}
		}

		private bool IsAttackerMount
		{
			get
			{
				return this.IsAttackerAgentMount;
			}
		}

		private bool IsVictimMount
		{
			get
			{
				return this.IsVictimAgentMount;
			}
		}

		public int TotalDamage
		{
			get
			{
				return this.InflictedDamage + this.ModifiedDamage;
			}
		}

		public float AttackProgress { get; internal set; }

		public List<ValueTuple<string, uint>> GetLogString()
		{
			CombatLogData._logStringCache.Clear();
			if (this.IsValidForPlayer && ManagedOptions.GetConfig(ManagedOptions.ManagedOptionsType.ReportDamage) > 0f)
			{
				if (this.IsRangedAttack && this.IsAttackerPlayer && this.BodyPartHit == BoneBodyPartType.Head)
				{
					CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(GameTexts.FindText("ui_head_shot", null).ToString(), 4289612505U));
				}
				if (this.IsFriendlyFire)
				{
					CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(GameTexts.FindText("combat_log_friendly_fire", null).ToString(), 4289612505U));
				}
				if (this.CrushedThrough && !this.IsFriendlyFire)
				{
					if (this.IsAttackerPlayer)
					{
						CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(GameTexts.FindText("combat_log_crushed_through_attacker", null).ToString(), 4289612505U));
					}
					else
					{
						CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(GameTexts.FindText("combat_log_crushed_through_victim", null).ToString(), 4289612505U));
					}
				}
				if (this.Chamber)
				{
					CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(GameTexts.FindText("combat_log_chamber_blocked", null).ToString(), 4289612505U));
				}
				uint num = 4290563554U;
				GameTexts.SetVariable("DAMAGE", this.TotalDamage);
				string text = "DAMAGE_TYPE";
				string text2 = "combat_log_damage_type";
				int num2 = (int)this.DamageType;
				GameTexts.SetVariable(text, GameTexts.FindText(text2, num2.ToString()));
				MBStringBuilder mbstringBuilder = default(MBStringBuilder);
				mbstringBuilder.Initialize(16, "GetLogString");
				if (this.IsVictimAgentSameAsAttackerAgent)
				{
					mbstringBuilder.Append<TextObject>(GameTexts.FindText("ui_received_number_damage_fall", null));
					num = 4292917946U;
				}
				else if (this.IsVictimMount)
				{
					if (this.IsVictimRiderAgentSameAsAttackerAgent)
					{
						mbstringBuilder.Append<TextObject>(GameTexts.FindText("ui_received_number_damage_fall_to_horse", null));
						num = 4292917946U;
					}
					else
					{
						mbstringBuilder.Append<TextObject>(GameTexts.FindText(this.IsAttackerPlayer ? "ui_delivered_number_damage_to_horse" : "ui_horse_received_number_damage", null));
						num = (this.IsAttackerPlayer ? 4210351871U : 4292917946U);
					}
				}
				else if (this.IsVictimEntity)
				{
					mbstringBuilder.Append<TextObject>(GameTexts.FindText("ui_delivered_number_damage_to_entity", null));
				}
				else if (this.IsAttackerMount)
				{
					mbstringBuilder.Append<TextObject>(GameTexts.FindText(this.IsAttackerPlayer ? "ui_horse_charged_for_number_damage" : "ui_received_number_damage", null));
					num = (this.IsAttackerPlayer ? 4210351871U : 4292917946U);
				}
				else if (this.TotalDamage > 0)
				{
					mbstringBuilder.Append<TextObject>(GameTexts.FindText(this.IsAttackerPlayer ? "ui_delivered_number_damage" : "ui_received_number_damage", null));
					num = (this.IsAttackerPlayer ? 4210351871U : 4292917946U);
				}
				if (this.BodyPartHit != BoneBodyPartType.None)
				{
					string text3 = "BODY_PART";
					string text4 = "body_part_type";
					num2 = (int)this.BodyPartHit;
					GameTexts.SetVariable(text3, GameTexts.FindText(text4, num2.ToString()));
					mbstringBuilder.Append<string>("<Detail>");
					mbstringBuilder.Append<TextObject>(GameTexts.FindText("combat_log_detail_body_part", null));
					mbstringBuilder.Append<string>("</Detail>");
				}
				if (this.HitSpeed > 1E-05f)
				{
					GameTexts.SetVariable("SPEED", MathF.Round(this.HitSpeed, 2));
					mbstringBuilder.Append<string>("<Detail>");
					mbstringBuilder.Append<TextObject>(this.IsRangedAttack ? GameTexts.FindText("combat_log_detail_missile_speed", null) : GameTexts.FindText("combat_log_detail_move_speed", null));
					mbstringBuilder.Append<string>("</Detail>");
				}
				if (this.IsRangedAttack)
				{
					GameTexts.SetVariable("DISTANCE", MathF.Round(this.Distance, 1));
					mbstringBuilder.Append<string>("<Detail>");
					mbstringBuilder.Append<TextObject>(GameTexts.FindText("combat_log_detail_distance", null));
					mbstringBuilder.Append<string>("</Detail>");
				}
				if (this.AbsorbedDamage > 0)
				{
					GameTexts.SetVariable("ABSORBED_DAMAGE", this.AbsorbedDamage);
					mbstringBuilder.Append<string>("<Detail>");
					mbstringBuilder.Append<TextObject>(GameTexts.FindText("combat_log_detail_absorbed_damage", null));
					mbstringBuilder.Append<string>("</Detail>");
				}
				if (this.ModifiedDamage != 0)
				{
					GameTexts.SetVariable("MODIFIED_DAMAGE", MathF.Abs(this.ModifiedDamage));
					mbstringBuilder.Append<string>("<Detail>");
					if (this.ModifiedDamage > 0)
					{
						mbstringBuilder.Append<TextObject>(GameTexts.FindText("combat_log_detail_extra_damage", null));
					}
					else if (this.ModifiedDamage < 0)
					{
						mbstringBuilder.Append<TextObject>(GameTexts.FindText("combat_log_detail_reduced_damage", null));
					}
					mbstringBuilder.Append<string>("</Detail>");
				}
				CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(mbstringBuilder.ToStringAndRelease(), num));
			}
			return CombatLogData._logStringCache;
		}

		public CombatLogData(bool isVictimAgentSameAsAttackerAgent, bool isAttackerAgentHuman, bool isAttackerAgentMine, bool doesAttackerAgentHaveRiderAgent, bool isAttackerAgentRiderAgentMine, bool isAttackerAgentMount, bool isVictimAgentHuman, bool isVictimAgentMine, bool isVictimAgentDead, bool doesVictimAgentHaveRiderAgent, bool isVictimAgentRiderAgentIsMine, bool isVictimAgentMount, bool isVictimEntity, bool isVictimRiderAgentSameAsAttackerAgent, bool crushedThrough, bool chamber, float distance)
		{
			this.IsVictimAgentSameAsAttackerAgent = isVictimAgentSameAsAttackerAgent;
			this.IsAttackerAgentHuman = isAttackerAgentHuman;
			this.IsAttackerAgentMine = isAttackerAgentMine;
			this.DoesAttackerAgentHaveRiderAgent = doesAttackerAgentHaveRiderAgent;
			this.IsAttackerAgentRiderAgentMine = isAttackerAgentRiderAgentMine;
			this.IsAttackerAgentMount = isAttackerAgentMount;
			this.IsVictimAgentHuman = isVictimAgentHuman;
			this.IsVictimAgentMine = isVictimAgentMine;
			this.DoesVictimAgentHaveRiderAgent = doesVictimAgentHaveRiderAgent;
			this.IsVictimAgentRiderAgentMine = isVictimAgentRiderAgentIsMine;
			this.IsVictimAgentMount = isVictimAgentMount;
			this.IsVictimEntity = isVictimEntity;
			this.IsVictimRiderAgentSameAsAttackerAgent = isVictimRiderAgentSameAsAttackerAgent;
			this.IsFatalDamage = isVictimAgentDead;
			this.DamageType = DamageTypes.Blunt;
			this.CrushedThrough = crushedThrough;
			this.Chamber = chamber;
			this.IsRangedAttack = false;
			this.IsFriendlyFire = false;
			this.VictimAgentName = null;
			this.HitSpeed = 0f;
			this.InflictedDamage = 0;
			this.AbsorbedDamage = 0;
			this.ModifiedDamage = 0;
			this.AttackProgress = 0f;
			this.BodyPartHit = BoneBodyPartType.None;
			this.Distance = distance;
		}

		public void SetVictimAgent(Agent victimAgent)
		{
			if (((victimAgent != null) ? victimAgent.MissionPeer : null) != null)
			{
				this.VictimAgentName = victimAgent.MissionPeer.DisplayedName;
				return;
			}
			this.VictimAgentName = ((victimAgent != null) ? victimAgent.Name : null);
		}

		private const string DetailTagStart = "<Detail>";

		private const string DetailTagEnd = "</Detail>";

		private const uint DamageReceivedColor = 4292917946U;

		private const uint DamageDealedColor = 4210351871U;

		private static List<ValueTuple<string, uint>> _logStringCache = new List<ValueTuple<string, uint>>();

		public readonly bool IsVictimAgentSameAsAttackerAgent;

		public readonly bool IsVictimRiderAgentSameAsAttackerAgent;

		public readonly bool IsAttackerAgentHuman;

		public readonly bool IsAttackerAgentMine;

		public readonly bool DoesAttackerAgentHaveRiderAgent;

		public readonly bool IsAttackerAgentRiderAgentMine;

		public readonly bool IsAttackerAgentMount;

		public readonly bool IsVictimAgentHuman;

		public readonly bool IsVictimAgentMine;

		public readonly bool DoesVictimAgentHaveRiderAgent;

		public readonly bool IsVictimAgentRiderAgentMine;

		public readonly bool IsVictimAgentMount;

		public bool IsVictimEntity;

		public DamageTypes DamageType;

		public bool CrushedThrough;

		public bool Chamber;

		public bool IsRangedAttack;

		public bool IsFriendlyFire;

		public bool IsFatalDamage;

		public BoneBodyPartType BodyPartHit;

		public string VictimAgentName;

		public float HitSpeed;

		public int InflictedDamage;

		public int AbsorbedDamage;

		public int ModifiedDamage;

		public float Distance;
	}
}
