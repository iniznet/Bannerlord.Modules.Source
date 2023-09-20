using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001D7 RID: 471
	public struct CombatLogData
	{
		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06001A67 RID: 6759 RVA: 0x0005D330 File Offset: 0x0005B530
		private bool IsValidForPlayer
		{
			get
			{
				return this.IsImportant && (this.IsAttackerPlayer || this.IsVictimPlayer);
			}
		}

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06001A68 RID: 6760 RVA: 0x0005D34C File Offset: 0x0005B54C
		private bool IsImportant
		{
			get
			{
				return this.TotalDamage > 0 || this.CrushedThrough || this.Chamber;
			}
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06001A69 RID: 6761 RVA: 0x0005D367 File Offset: 0x0005B567
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

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06001A6A RID: 6762 RVA: 0x0005D388 File Offset: 0x0005B588
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

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06001A6B RID: 6763 RVA: 0x0005D3A9 File Offset: 0x0005B5A9
		private bool IsAttackerMount
		{
			get
			{
				return this.IsAttackerAgentMount;
			}
		}

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06001A6C RID: 6764 RVA: 0x0005D3B1 File Offset: 0x0005B5B1
		private bool IsVictimMount
		{
			get
			{
				return this.IsVictimAgentMount;
			}
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06001A6D RID: 6765 RVA: 0x0005D3B9 File Offset: 0x0005B5B9
		public int TotalDamage
		{
			get
			{
				return this.InflictedDamage + this.ModifiedDamage;
			}
		}

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x06001A6E RID: 6766 RVA: 0x0005D3C8 File Offset: 0x0005B5C8
		// (set) Token: 0x06001A6F RID: 6767 RVA: 0x0005D3D0 File Offset: 0x0005B5D0
		public float AttackProgress { get; internal set; }

		// Token: 0x06001A70 RID: 6768 RVA: 0x0005D3DC File Offset: 0x0005B5DC
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

		// Token: 0x06001A71 RID: 6769 RVA: 0x0005D860 File Offset: 0x0005BA60
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

		// Token: 0x06001A72 RID: 6770 RVA: 0x0005D940 File Offset: 0x0005BB40
		public void SetVictimAgent(Agent victimAgent)
		{
			if (((victimAgent != null) ? victimAgent.MissionPeer : null) != null)
			{
				this.VictimAgentName = victimAgent.MissionPeer.DisplayedName;
				return;
			}
			this.VictimAgentName = ((victimAgent != null) ? victimAgent.Name : null);
		}

		// Token: 0x04000878 RID: 2168
		private const string DetailTagStart = "<Detail>";

		// Token: 0x04000879 RID: 2169
		private const string DetailTagEnd = "</Detail>";

		// Token: 0x0400087A RID: 2170
		private const uint DamageReceivedColor = 4292917946U;

		// Token: 0x0400087B RID: 2171
		private const uint DamageDealedColor = 4210351871U;

		// Token: 0x0400087C RID: 2172
		private static List<ValueTuple<string, uint>> _logStringCache = new List<ValueTuple<string, uint>>();

		// Token: 0x0400087D RID: 2173
		public readonly bool IsVictimAgentSameAsAttackerAgent;

		// Token: 0x0400087E RID: 2174
		public readonly bool IsVictimRiderAgentSameAsAttackerAgent;

		// Token: 0x0400087F RID: 2175
		public readonly bool IsAttackerAgentHuman;

		// Token: 0x04000880 RID: 2176
		public readonly bool IsAttackerAgentMine;

		// Token: 0x04000881 RID: 2177
		public readonly bool DoesAttackerAgentHaveRiderAgent;

		// Token: 0x04000882 RID: 2178
		public readonly bool IsAttackerAgentRiderAgentMine;

		// Token: 0x04000883 RID: 2179
		public readonly bool IsAttackerAgentMount;

		// Token: 0x04000884 RID: 2180
		public readonly bool IsVictimAgentHuman;

		// Token: 0x04000885 RID: 2181
		public readonly bool IsVictimAgentMine;

		// Token: 0x04000886 RID: 2182
		public readonly bool DoesVictimAgentHaveRiderAgent;

		// Token: 0x04000887 RID: 2183
		public readonly bool IsVictimAgentRiderAgentMine;

		// Token: 0x04000888 RID: 2184
		public readonly bool IsVictimAgentMount;

		// Token: 0x04000889 RID: 2185
		public bool IsVictimEntity;

		// Token: 0x0400088A RID: 2186
		public DamageTypes DamageType;

		// Token: 0x0400088B RID: 2187
		public bool CrushedThrough;

		// Token: 0x0400088C RID: 2188
		public bool Chamber;

		// Token: 0x0400088D RID: 2189
		public bool IsRangedAttack;

		// Token: 0x0400088E RID: 2190
		public bool IsFriendlyFire;

		// Token: 0x0400088F RID: 2191
		public bool IsFatalDamage;

		// Token: 0x04000890 RID: 2192
		public BoneBodyPartType BodyPartHit;

		// Token: 0x04000891 RID: 2193
		public string VictimAgentName;

		// Token: 0x04000892 RID: 2194
		public float HitSpeed;

		// Token: 0x04000893 RID: 2195
		public int InflictedDamage;

		// Token: 0x04000894 RID: 2196
		public int AbsorbedDamage;

		// Token: 0x04000895 RID: 2197
		public int ModifiedDamage;

		// Token: 0x04000897 RID: 2199
		public float Distance;
	}
}
