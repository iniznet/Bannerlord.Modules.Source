using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox
{
	// Token: 0x02000005 RID: 5
	public class CampaignMapSiegePrefabEntityCache : ScriptComponentBehavior
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002D68 File Offset: 0x00000F68
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

		// Token: 0x06000009 RID: 9 RVA: 0x00003130 File Offset: 0x00001330
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

		// Token: 0x0600000A RID: 10 RVA: 0x000031E8 File Offset: 0x000013E8
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

		// Token: 0x04000001 RID: 1
		[EditableScriptComponentVariable(true)]
		private string _attackerBallistaPrefab = "ballista_a_mapicon";

		// Token: 0x04000002 RID: 2
		[EditableScriptComponentVariable(true)]
		private string _defenderBallistaPrefab = "ballista_b_mapicon";

		// Token: 0x04000003 RID: 3
		[EditableScriptComponentVariable(true)]
		private string _attackerFireBallistaPrefab = "ballista_a_fire_mapicon";

		// Token: 0x04000004 RID: 4
		[EditableScriptComponentVariable(true)]
		private string _defenderFireBallistaPrefab = "ballista_b_fire_mapicon";

		// Token: 0x04000005 RID: 5
		[EditableScriptComponentVariable(true)]
		private string _attackerMangonelPrefab = "mangonel_a_mapicon";

		// Token: 0x04000006 RID: 6
		[EditableScriptComponentVariable(true)]
		private string _defenderMangonelPrefab = "mangonel_b_mapicon";

		// Token: 0x04000007 RID: 7
		[EditableScriptComponentVariable(true)]
		private string _attackerFireMangonelPrefab = "mangonel_a_fire_mapicon";

		// Token: 0x04000008 RID: 8
		[EditableScriptComponentVariable(true)]
		private string _defenderFireMangonelPrefab = "mangonel_b_fire_mapicon";

		// Token: 0x04000009 RID: 9
		[EditableScriptComponentVariable(true)]
		private string _attackerTrebuchetPrefab = "trebuchet_a_mapicon";

		// Token: 0x0400000A RID: 10
		[EditableScriptComponentVariable(true)]
		private string _defenderTrebuchetPrefab = "trebuchet_b_mapicon";

		// Token: 0x0400000B RID: 11
		private MatrixFrame _attackerBallistaLaunchEntitialFrame;

		// Token: 0x0400000C RID: 12
		private MatrixFrame _defenderBallistaLaunchEntitialFrame;

		// Token: 0x0400000D RID: 13
		private MatrixFrame _attackerFireBallistaLaunchEntitialFrame;

		// Token: 0x0400000E RID: 14
		private MatrixFrame _defenderFireBallistaLaunchEntitialFrame;

		// Token: 0x0400000F RID: 15
		private MatrixFrame _attackerMangonelLaunchEntitialFrame;

		// Token: 0x04000010 RID: 16
		private MatrixFrame _defenderMangonelLaunchEntitialFrame;

		// Token: 0x04000011 RID: 17
		private MatrixFrame _attackerFireMangonelLaunchEntitialFrame;

		// Token: 0x04000012 RID: 18
		private MatrixFrame _defenderFireMangonelLaunchEntitialFrame;

		// Token: 0x04000013 RID: 19
		private MatrixFrame _attackerTrebuchetLaunchEntitialFrame;

		// Token: 0x04000014 RID: 20
		private MatrixFrame _defenderTrebuchetLaunchEntitialFrame;

		// Token: 0x04000015 RID: 21
		private Vec3 _attackerBallistaScale;

		// Token: 0x04000016 RID: 22
		private Vec3 _defenderBallistaScale;

		// Token: 0x04000017 RID: 23
		private Vec3 _attackerFireBallistaScale;

		// Token: 0x04000018 RID: 24
		private Vec3 _defenderFireBallistaScale;

		// Token: 0x04000019 RID: 25
		private Vec3 _attackerMangonelScale;

		// Token: 0x0400001A RID: 26
		private Vec3 _defenderMangonelScale;

		// Token: 0x0400001B RID: 27
		private Vec3 _attackerFireMangonelScale;

		// Token: 0x0400001C RID: 28
		private Vec3 _defenderFireMangonelScale;

		// Token: 0x0400001D RID: 29
		private Vec3 _attackerTrebuchetScale;

		// Token: 0x0400001E RID: 30
		private Vec3 _defenderTrebuchetScale;
	}
}
