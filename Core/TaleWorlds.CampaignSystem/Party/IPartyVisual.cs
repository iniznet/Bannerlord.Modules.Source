using System;
using System.Collections.ObjectModel;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Party
{
	// Token: 0x020002A3 RID: 675
	public interface IPartyVisual
	{
		// Token: 0x0600266C RID: 9836
		void OnStartup(PartyBase party);

		// Token: 0x0600266D RID: 9837
		void OnPartyRemoved();

		// Token: 0x0600266E RID: 9838
		void OnBesieged(Vec3 soundPosition);

		// Token: 0x0600266F RID: 9839
		void OnSiegeLifted();

		// Token: 0x06002670 RID: 9840
		void SetMapIconAsDirty();

		// Token: 0x06002671 RID: 9841
		void Tick(float realDt, float dt, PartyBase party, ref int dirtyPartiesCount, ref PartyBase[] dirtyPartiesList);

		// Token: 0x06002672 RID: 9842
		MatrixFrame GetFrame();

		// Token: 0x06002673 RID: 9843
		MatrixFrame GetGlobalFrame();

		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x06002674 RID: 9844
		MatrixFrame CircleLocalFrame { get; }

		// Token: 0x06002675 RID: 9845
		void SetFrame(ref MatrixFrame frame);

		// Token: 0x06002676 RID: 9846
		void SetVisualVisible(bool visible);

		// Token: 0x06002677 RID: 9847
		bool IsVisibleOrFadingOut();

		// Token: 0x06002678 RID: 9848
		void ReleaseResources();

		// Token: 0x06002679 RID: 9849
		void RefreshWallState(PartyBase party);

		// Token: 0x0600267A RID: 9850
		void RefreshLevelMask(PartyBase party);

		// Token: 0x0600267B RID: 9851
		ReadOnlyCollection<MatrixFrame> GetSiegeCamp1GlobalFrames();

		// Token: 0x0600267C RID: 9852
		ReadOnlyCollection<MatrixFrame> GetSiegeCamp2GlobalFrames();

		// Token: 0x0600267D RID: 9853
		MatrixFrame GetAttackerTowerSiegeEngineFrameAtIndex(int index);

		// Token: 0x0600267E RID: 9854
		int GetAttackerTowerSiegeEngineFrameCount();

		// Token: 0x0600267F RID: 9855
		MatrixFrame GetAttackerBatteringRamSiegeEngineFrameAtIndex(int index);

		// Token: 0x06002680 RID: 9856
		int GetAttackerBatteringRamSiegeEngineFrameCount();

		// Token: 0x06002681 RID: 9857
		MatrixFrame GetAttackerRangedSiegeEngineFrameAtIndex(int index);

		// Token: 0x06002682 RID: 9858
		int GetAttackerRangedSiegeEngineFrameCount();

		// Token: 0x06002683 RID: 9859
		MatrixFrame GetDefenderSiegeEngineFrameAtIndex(int index);

		// Token: 0x06002684 RID: 9860
		int GetDefenderSiegeEngineFrameCount();

		// Token: 0x06002685 RID: 9861
		MatrixFrame GetBreacableWallFrameAtIndex(int index);

		// Token: 0x06002686 RID: 9862
		int GetBreacableWallFrameCount();

		// Token: 0x06002687 RID: 9863
		IMapEntity GetMapEntity();

		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x06002688 RID: 9864
		// (set) Token: 0x06002689 RID: 9865
		bool EntityMoving { get; set; }

		// Token: 0x0600268A RID: 9866
		void ValidateIsDirty(PartyBase party, float realDt, float dt);

		// Token: 0x0600268B RID: 9867
		void TickFadingState(float realDt, float dt);
	}
}
