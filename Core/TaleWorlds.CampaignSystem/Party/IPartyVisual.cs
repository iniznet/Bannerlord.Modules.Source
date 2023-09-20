using System;
using System.Collections.ObjectModel;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Party
{
	public interface IPartyVisual
	{
		void OnStartup(PartyBase party);

		void OnPartyRemoved();

		void OnBesieged(Vec3 soundPosition);

		void OnSiegeLifted();

		void SetMapIconAsDirty();

		void Tick(float realDt, float dt, PartyBase party, ref int dirtyPartiesCount, ref PartyBase[] dirtyPartiesList);

		MatrixFrame GetFrame();

		MatrixFrame GetGlobalFrame();

		MatrixFrame CircleLocalFrame { get; }

		void SetFrame(ref MatrixFrame frame);

		void SetVisualVisible(bool visible);

		bool IsVisibleOrFadingOut();

		void ReleaseResources();

		void RefreshWallState(PartyBase party);

		void RefreshLevelMask(PartyBase party);

		ReadOnlyCollection<MatrixFrame> GetSiegeCamp1GlobalFrames();

		ReadOnlyCollection<MatrixFrame> GetSiegeCamp2GlobalFrames();

		MatrixFrame GetAttackerTowerSiegeEngineFrameAtIndex(int index);

		int GetAttackerTowerSiegeEngineFrameCount();

		MatrixFrame GetAttackerBatteringRamSiegeEngineFrameAtIndex(int index);

		int GetAttackerBatteringRamSiegeEngineFrameCount();

		MatrixFrame GetAttackerRangedSiegeEngineFrameAtIndex(int index);

		int GetAttackerRangedSiegeEngineFrameCount();

		MatrixFrame GetDefenderSiegeEngineFrameAtIndex(int index);

		int GetDefenderSiegeEngineFrameCount();

		MatrixFrame GetBreacableWallFrameAtIndex(int index);

		int GetBreacableWallFrameCount();

		IMapEntity GetMapEntity();

		bool EntityMoving { get; set; }

		void ValidateIsDirty(PartyBase party, float realDt, float dt);

		void TickFadingState(float realDt, float dt);
	}
}
