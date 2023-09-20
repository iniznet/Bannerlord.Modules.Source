using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	public class CampaignMapSiegePrefabEntityCache : ScriptComponentBehavior
	{
		protected override void OnInit()
		{
			base.OnInit();
			GameEntity gameEntity = GameEntity.Instantiate(((MapScene)Campaign.Current.MapSceneWrapper).Scene, this._attackerBallistaPrefab, true);
			this._attackerBallistaLaunchEntitialFrame = MBExtensions.GetFirstChildEntityWithTag(gameEntity.GetChild(0), "projectile_position").GetGlobalFrame();
			MatrixFrame matrixFrame = gameEntity.GetChild(0).GetFrame();
			this._attackerBallistaScale = matrixFrame.rotation.GetScaleVector();
			GameEntity gameEntity2 = GameEntity.Instantiate(((MapScene)Campaign.Current.MapSceneWrapper).Scene, this._defenderBallistaPrefab, true);
			this._defenderBallistaLaunchEntitialFrame = MBExtensions.GetFirstChildEntityWithTag(gameEntity2.GetChild(0), "projectile_position").GetGlobalFrame();
			matrixFrame = gameEntity2.GetChild(0).GetFrame();
			this._defenderBallistaScale = matrixFrame.rotation.GetScaleVector();
			GameEntity gameEntity3 = GameEntity.Instantiate(((MapScene)Campaign.Current.MapSceneWrapper).Scene, this._attackerFireBallistaPrefab, true);
			this._attackerFireBallistaLaunchEntitialFrame = MBExtensions.GetFirstChildEntityWithTag(gameEntity3.GetChild(0), "projectile_position").GetGlobalFrame();
			matrixFrame = gameEntity3.GetChild(0).GetFrame();
			this._attackerFireBallistaScale = matrixFrame.rotation.GetScaleVector();
			GameEntity gameEntity4 = GameEntity.Instantiate(((MapScene)Campaign.Current.MapSceneWrapper).Scene, this._defenderFireBallistaPrefab, true);
			this._defenderFireBallistaLaunchEntitialFrame = MBExtensions.GetFirstChildEntityWithTag(gameEntity4.GetChild(0), "projectile_position").GetGlobalFrame();
			matrixFrame = gameEntity4.GetChild(0).GetFrame();
			this._defenderFireBallistaScale = matrixFrame.rotation.GetScaleVector();
			GameEntity gameEntity5 = GameEntity.Instantiate(((MapScene)Campaign.Current.MapSceneWrapper).Scene, this._attackerMangonelPrefab, true);
			this._attackerMangonelLaunchEntitialFrame = MBExtensions.GetFirstChildEntityWithTag(gameEntity5.GetChild(0), "projectile_position").GetGlobalFrame();
			matrixFrame = gameEntity5.GetChild(0).GetFrame();
			this._attackerMangonelScale = matrixFrame.rotation.GetScaleVector();
			GameEntity gameEntity6 = GameEntity.Instantiate(((MapScene)Campaign.Current.MapSceneWrapper).Scene, this._defenderMangonelPrefab, true);
			this._defenderMangonelLaunchEntitialFrame = MBExtensions.GetFirstChildEntityWithTag(gameEntity6.GetChild(0), "projectile_position").GetGlobalFrame();
			matrixFrame = gameEntity6.GetChild(0).GetFrame();
			this._defenderMangonelScale = matrixFrame.rotation.GetScaleVector();
			GameEntity gameEntity7 = GameEntity.Instantiate(((MapScene)Campaign.Current.MapSceneWrapper).Scene, this._attackerFireMangonelPrefab, true);
			this._attackerFireMangonelLaunchEntitialFrame = MBExtensions.GetFirstChildEntityWithTag(gameEntity7.GetChild(0), "projectile_position").GetGlobalFrame();
			matrixFrame = gameEntity7.GetChild(0).GetFrame();
			this._attackerFireMangonelScale = matrixFrame.rotation.GetScaleVector();
			GameEntity gameEntity8 = GameEntity.Instantiate(((MapScene)Campaign.Current.MapSceneWrapper).Scene, this._defenderFireMangonelPrefab, true);
			this._defenderFireMangonelLaunchEntitialFrame = MBExtensions.GetFirstChildEntityWithTag(gameEntity8.GetChild(0), "projectile_position").GetGlobalFrame();
			matrixFrame = gameEntity8.GetChild(0).GetFrame();
			this._defenderFireMangonelScale = matrixFrame.rotation.GetScaleVector();
			GameEntity gameEntity9 = GameEntity.Instantiate(((MapScene)Campaign.Current.MapSceneWrapper).Scene, this._attackerTrebuchetPrefab, true);
			this._attackerTrebuchetLaunchEntitialFrame = MBExtensions.GetFirstChildEntityWithTag(gameEntity9.GetChild(0), "projectile_position").GetGlobalFrame();
			matrixFrame = gameEntity9.GetChild(0).GetFrame();
			this._attackerTrebuchetScale = matrixFrame.rotation.GetScaleVector();
			GameEntity gameEntity10 = GameEntity.Instantiate(((MapScene)Campaign.Current.MapSceneWrapper).Scene, this._defenderTrebuchetPrefab, true);
			this._defenderTrebuchetLaunchEntitialFrame = MBExtensions.GetFirstChildEntityWithTag(gameEntity10.GetChild(0), "projectile_position").GetGlobalFrame();
			matrixFrame = gameEntity10.GetChild(0).GetFrame();
			this._defenderTrebuchetScale = matrixFrame.rotation.GetScaleVector();
		}

		public MatrixFrame GetLaunchEntitialFrameForSiegeEngine(SiegeEngineType type, BattleSideEnum side)
		{
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			if (type == DefaultSiegeEngineTypes.Onager)
			{
				matrixFrame = this._attackerMangonelLaunchEntitialFrame;
			}
			else if (type == DefaultSiegeEngineTypes.FireOnager)
			{
				matrixFrame = this._attackerFireMangonelLaunchEntitialFrame;
			}
			else if (type == DefaultSiegeEngineTypes.Catapult)
			{
				matrixFrame = this._defenderMangonelLaunchEntitialFrame;
			}
			else if (type == DefaultSiegeEngineTypes.FireCatapult)
			{
				matrixFrame = this._defenderFireMangonelLaunchEntitialFrame;
			}
			else if (type == DefaultSiegeEngineTypes.Ballista)
			{
				matrixFrame = ((side == 1) ? this._attackerBallistaLaunchEntitialFrame : this._defenderBallistaLaunchEntitialFrame);
			}
			else if (type == DefaultSiegeEngineTypes.FireBallista)
			{
				matrixFrame = ((side == 1) ? this._attackerFireBallistaLaunchEntitialFrame : this._defenderFireBallistaLaunchEntitialFrame);
			}
			else if (type == DefaultSiegeEngineTypes.Trebuchet)
			{
				matrixFrame = this._attackerTrebuchetLaunchEntitialFrame;
			}
			else if (type == DefaultSiegeEngineTypes.Bricole)
			{
				matrixFrame = this._defenderTrebuchetLaunchEntitialFrame;
			}
			return matrixFrame;
		}

		public Vec3 GetScaleForSiegeEngine(SiegeEngineType type, BattleSideEnum side)
		{
			Vec3 vec = Vec3.Zero;
			if (type == DefaultSiegeEngineTypes.Onager)
			{
				vec = this._attackerMangonelScale;
			}
			else if (type == DefaultSiegeEngineTypes.FireOnager)
			{
				vec = this._attackerFireMangonelScale;
			}
			else if (type == DefaultSiegeEngineTypes.Catapult)
			{
				vec = this._defenderMangonelScale;
			}
			else if (type == DefaultSiegeEngineTypes.FireCatapult)
			{
				vec = this._defenderFireMangonelScale;
			}
			else if (type == DefaultSiegeEngineTypes.Ballista)
			{
				vec = ((side == 1) ? this._attackerBallistaScale : this._defenderBallistaScale);
			}
			else if (type == DefaultSiegeEngineTypes.FireBallista)
			{
				vec = ((side == 1) ? this._attackerFireBallistaScale : this._defenderFireBallistaScale);
			}
			else if (type == DefaultSiegeEngineTypes.Trebuchet)
			{
				vec = this._attackerTrebuchetScale;
			}
			else if (type == DefaultSiegeEngineTypes.Bricole)
			{
				vec = this._defenderTrebuchetScale;
			}
			return vec;
		}

		[EditableScriptComponentVariable(true)]
		private string _attackerBallistaPrefab = "ballista_a_mapicon";

		[EditableScriptComponentVariable(true)]
		private string _defenderBallistaPrefab = "ballista_b_mapicon";

		[EditableScriptComponentVariable(true)]
		private string _attackerFireBallistaPrefab = "ballista_a_fire_mapicon";

		[EditableScriptComponentVariable(true)]
		private string _defenderFireBallistaPrefab = "ballista_b_fire_mapicon";

		[EditableScriptComponentVariable(true)]
		private string _attackerMangonelPrefab = "mangonel_a_mapicon";

		[EditableScriptComponentVariable(true)]
		private string _defenderMangonelPrefab = "mangonel_b_mapicon";

		[EditableScriptComponentVariable(true)]
		private string _attackerFireMangonelPrefab = "mangonel_a_fire_mapicon";

		[EditableScriptComponentVariable(true)]
		private string _defenderFireMangonelPrefab = "mangonel_b_fire_mapicon";

		[EditableScriptComponentVariable(true)]
		private string _attackerTrebuchetPrefab = "trebuchet_a_mapicon";

		[EditableScriptComponentVariable(true)]
		private string _defenderTrebuchetPrefab = "trebuchet_b_mapicon";

		private MatrixFrame _attackerBallistaLaunchEntitialFrame;

		private MatrixFrame _defenderBallistaLaunchEntitialFrame;

		private MatrixFrame _attackerFireBallistaLaunchEntitialFrame;

		private MatrixFrame _defenderFireBallistaLaunchEntitialFrame;

		private MatrixFrame _attackerMangonelLaunchEntitialFrame;

		private MatrixFrame _defenderMangonelLaunchEntitialFrame;

		private MatrixFrame _attackerFireMangonelLaunchEntitialFrame;

		private MatrixFrame _defenderFireMangonelLaunchEntitialFrame;

		private MatrixFrame _attackerTrebuchetLaunchEntitialFrame;

		private MatrixFrame _defenderTrebuchetLaunchEntitialFrame;

		private Vec3 _attackerBallistaScale;

		private Vec3 _defenderBallistaScale;

		private Vec3 _attackerFireBallistaScale;

		private Vec3 _defenderFireBallistaScale;

		private Vec3 _attackerMangonelScale;

		private Vec3 _defenderMangonelScale;

		private Vec3 _attackerFireMangonelScale;

		private Vec3 _defenderFireMangonelScale;

		private Vec3 _attackerTrebuchetScale;

		private Vec3 _defenderTrebuchetScale;
	}
}
