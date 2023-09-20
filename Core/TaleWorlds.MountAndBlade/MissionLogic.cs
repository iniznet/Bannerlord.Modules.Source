using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000275 RID: 629
	public abstract class MissionLogic : MissionBehavior
	{
		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06002194 RID: 8596 RVA: 0x0007AD90 File Offset: 0x00078F90
		public override MissionBehaviorType BehaviorType
		{
			get
			{
				return MissionBehaviorType.Logic;
			}
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x0007AD93 File Offset: 0x00078F93
		public virtual InquiryData OnEndMissionRequest(out bool canLeave)
		{
			canLeave = true;
			return null;
		}

		// Token: 0x06002196 RID: 8598 RVA: 0x0007AD99 File Offset: 0x00078F99
		public virtual bool MissionEnded(ref MissionResult missionResult)
		{
			return false;
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x0007AD9C File Offset: 0x00078F9C
		public virtual void OnBattleEnded()
		{
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x0007AD9E File Offset: 0x00078F9E
		public virtual void ShowBattleResults()
		{
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x0007ADA0 File Offset: 0x00078FA0
		public virtual void OnRetreatMission()
		{
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x0007ADA2 File Offset: 0x00078FA2
		public virtual void OnSurrenderMission()
		{
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x0007ADA4 File Offset: 0x00078FA4
		public virtual void OnAutoDeployTeam(Team team)
		{
		}

		// Token: 0x0600219C RID: 8604 RVA: 0x0007ADA6 File Offset: 0x00078FA6
		public virtual List<EquipmentElement> GetExtraEquipmentElementsForCharacter(BasicCharacterObject character, bool getAllEquipments = false)
		{
			return null;
		}

		// Token: 0x0600219D RID: 8605 RVA: 0x0007ADA9 File Offset: 0x00078FA9
		public virtual void OnMissionResultReady(MissionResult missionResult)
		{
		}
	}
}
