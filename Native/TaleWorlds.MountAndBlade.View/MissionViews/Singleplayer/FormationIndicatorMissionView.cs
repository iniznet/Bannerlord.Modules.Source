using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	public class FormationIndicatorMissionView : MissionView
	{
		public override void AfterStart()
		{
			base.AfterStart();
			this.mission = Mission.Current;
			this._indicators = new FormationIndicatorMissionView.Indicator[this.mission.Teams.Count, 9];
			for (int i = 0; i < this.mission.Teams.Count; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					this._indicators[i, j] = new FormationIndicatorMissionView.Indicator
					{
						missionScreen = base.MissionScreen
					};
				}
			}
		}

		private GameEntity CreateBannerEntity(Formation formation)
		{
			GameEntity gameEntity = GameEntity.CreateEmpty(this.mission.Scene, true);
			gameEntity.EntityFlags |= 512;
			uint num = 4278190080U;
			uint num2;
			if (formation.Team.IsPlayerAlly)
			{
				num2 = 2130747904U;
			}
			else
			{
				num2 = 2144798212U;
			}
			gameEntity.AddMultiMesh(MetaMesh.GetCopy("billboard_unit_mesh", true, false), true);
			gameEntity.GetFirstMesh().Color = uint.MaxValue;
			Material formationMaterial = Material.GetFromResource("formation_icon").CreateCopy();
			if (formation.Team != null)
			{
				Banner banner = ((formation.Captain != null) ? formation.Captain.Origin.Banner : formation.Team.Banner);
				if (banner != null)
				{
					Action<Texture> action = delegate(Texture tex)
					{
						formationMaterial.SetTexture(1, tex);
					};
					banner.GetTableauTextureLarge(action);
				}
				else
				{
					Texture fromResource = Texture.GetFromResource("plain_red");
					formationMaterial.SetTexture(1, fromResource);
				}
			}
			else
			{
				Texture fromResource2 = Texture.GetFromResource("plain_red");
				formationMaterial.SetTexture(1, fromResource2);
			}
			int num3 = formation.FormationIndex % 4;
			gameEntity.GetFirstMesh().SetMaterial(formationMaterial);
			gameEntity.GetFirstMesh().Color = num2;
			gameEntity.GetFirstMesh().Color2 = num;
			gameEntity.GetFirstMesh().SetVectorArgument(0f, 1f, (float)num3, 1f);
			return gameEntity;
		}

		private int GetFormationTeamIndex(Formation formation)
		{
			int num;
			if (this.mission.Teams.Count > 2 && (formation.Team == this.mission.AttackerAllyTeam || formation.Team == this.mission.DefenderAllyTeam))
			{
				num = ((this.mission.Teams.Count == 3) ? 2 : ((formation.Team == this.mission.DefenderAllyTeam) ? 2 : 3));
			}
			else
			{
				num = formation.Team.Side;
			}
			return num;
		}

		public override void OnMissionScreenTick(float dt)
		{
			this.OnMissionTick(dt);
			bool flag;
			if (base.Input.IsGameKeyDown(5))
			{
				flag = false;
				this._isEnabled = false;
			}
			else
			{
				flag = false;
			}
			IEnumerable<Formation> enumerable = this.mission.Teams.SelectMany((Team t) => t.FormationsIncludingEmpty);
			if (flag)
			{
				IEnumerable<Formation> enumerable2 = this.mission.Teams.SelectMany((Team t) => t.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0));
				foreach (Formation formation in enumerable2)
				{
					int formationTeamIndex = this.GetFormationTeamIndex(formation);
					FormationIndicatorMissionView.Indicator indicator = this._indicators[formationTeamIndex, formation.FormationIndex];
					indicator.DetermineIndicatorState(dt, formation.QuerySystem.MedianPosition.GetGroundVec3());
					if (indicator.indicatorEntity == null)
					{
						if (indicator.indicatorVisible)
						{
							GameEntity gameEntity = this.CreateBannerEntity(formation);
							gameEntity.SetFrame(ref indicator.indicatorFrame);
							gameEntity.SetVisibilityExcludeParents(true);
							this._indicators[formationTeamIndex, formation.FormationIndex].indicatorEntity = gameEntity;
							gameEntity.SetAlpha(0f);
							this._indicators[formationTeamIndex, formation.FormationIndex].indicatorAlpha = 0f;
						}
					}
					else
					{
						if (indicator.indicatorEntity.IsVisibleIncludeParents() != indicator.indicatorVisible)
						{
							if (!indicator.indicatorVisible)
							{
								if (indicator.indicatorAlpha > 0f)
								{
									indicator.indicatorAlpha -= 0.01f;
									if (indicator.indicatorAlpha < 0f)
									{
										indicator.indicatorAlpha = 0f;
									}
									indicator.indicatorEntity.SetAlpha(indicator.indicatorAlpha);
								}
								else
								{
									indicator.indicatorEntity.SetVisibilityExcludeParents(indicator.indicatorVisible);
								}
							}
							else
							{
								indicator.indicatorEntity.SetVisibilityExcludeParents(indicator.indicatorVisible);
							}
						}
						if (indicator.indicatorVisible && indicator.indicatorAlpha < 1f)
						{
							indicator.indicatorAlpha += 0.01f;
							if (indicator.indicatorAlpha > 1f)
							{
								indicator.indicatorAlpha = 1f;
							}
							indicator.indicatorEntity.SetAlpha(indicator.indicatorAlpha);
						}
						if (!indicator._isMovingTooFast && indicator.indicatorEntity.IsVisibleIncludeParents())
						{
							indicator.indicatorEntity.SetFrame(ref indicator.indicatorFrame);
						}
					}
				}
				using (IEnumerator<Formation> enumerator = enumerable.Except(enumerable2).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Formation formation2 = enumerator.Current;
						if (formation2.FormationIndex >= 8)
						{
							break;
						}
						FormationIndicatorMissionView.Indicator indicator2 = this._indicators[this.GetFormationTeamIndex(formation2), formation2.FormationIndex];
						if (indicator2.indicatorEntity != null && indicator2.indicatorEntity.IsVisibleIncludeParents())
						{
							indicator2.indicatorEntity.SetVisibilityExcludeParents(false);
							indicator2.indicatorVisible = false;
						}
					}
					return;
				}
			}
			if (this._isEnabled)
			{
				foreach (Formation formation3 in enumerable)
				{
					int formationTeamIndex2 = this.GetFormationTeamIndex(formation3);
					FormationIndicatorMissionView.Indicator indicator3 = this._indicators[formationTeamIndex2, formation3.FormationIndex];
					if (indicator3 != null && indicator3.indicatorEntity != null)
					{
						indicator3.indicatorAlpha = 0f;
						indicator3.indicatorEntity.SetVisibilityExcludeParents(false);
					}
				}
				this._isEnabled = false;
			}
		}

		private FormationIndicatorMissionView.Indicator[,] _indicators;

		private Mission mission;

		private bool _isEnabled;

		public class Indicator
		{
			private Vec3? GetCurrentPosition()
			{
				if (Mission.Current.MainAgent != null)
				{
					return new Vec3?(Mission.Current.MainAgent.AgentVisuals.GetGlobalFrame().origin + new Vec3(0f, 0f, 1f, -1f));
				}
				if (this.missionScreen.CombatCamera != null)
				{
					return new Vec3?(this.missionScreen.CombatCamera.Position);
				}
				return null;
			}

			public void DetermineIndicatorState(float dt, Vec3 position)
			{
				Mission mission = Mission.Current;
				Vec3? currentPosition = this.GetCurrentPosition();
				if (currentPosition == null)
				{
					this.indicatorVisible = false;
					return;
				}
				if (this.firstTime)
				{
					this.prevIndicatorPosition = position;
					this.nextIndicatorPosition = position;
					this.firstTime = false;
				}
				Vec3 vec;
				if (this.nextIndicatorPosition.Distance(this.prevIndicatorPosition) / 0.5f > 30f)
				{
					vec = position;
					this._isMovingTooFast = true;
				}
				else
				{
					vec = Vec3.Lerp(this.prevIndicatorPosition, this.nextIndicatorPosition, MBMath.ClampFloat(this._drawIndicatorElapsedTime / 0.5f, 0f, 1f));
				}
				float num = currentPosition.Value.Distance(vec);
				if (this._drawIndicatorElapsedTime < 0.5f)
				{
					this._drawIndicatorElapsedTime += dt;
				}
				else
				{
					this.prevIndicatorPosition = this.nextIndicatorPosition;
					this.nextIndicatorPosition = position;
					this._isSeenByPlayer = num < 60f && (num < 15f || mission.Scene.CheckPointCanSeePoint(currentPosition.Value, position, null) || mission.Scene.CheckPointCanSeePoint(currentPosition.Value + new Vec3(0f, 0f, 2f, -1f), position + new Vec3(0f, 0f, 2f, -1f), null));
					this._drawIndicatorElapsedTime = 0f;
				}
				if (this._isSeenByPlayer)
				{
					this.indicatorVisible = false;
					if (!this._isMovingTooFast && this.indicatorEntity != null && this.indicatorEntity.IsVisibleIncludeParents())
					{
						float num2 = MBMath.ClampFloat(num * 0.02f, 1f, 10f) * 3f;
						MatrixFrame identity = MatrixFrame.Identity;
						identity.origin = vec;
						identity.origin.z = identity.origin.z + num2 * 0.75f;
						identity.rotation.ApplyScaleLocal(num2);
						this.indicatorFrame = identity;
					}
					return;
				}
				float num3 = MBMath.ClampFloat(num * 0.02f, 1f, 10f) * 3f;
				MatrixFrame identity2 = MatrixFrame.Identity;
				identity2.origin = vec;
				identity2.origin.z = identity2.origin.z + num3 * 0.75f;
				identity2.rotation.ApplyScaleLocal(num3);
				this.indicatorFrame = identity2;
				if (!this._isMovingTooFast)
				{
					this.indicatorVisible = true;
					return;
				}
				if (this.indicatorEntity != null && this.indicatorEntity.IsVisibleIncludeParents())
				{
					this.indicatorVisible = false;
					return;
				}
				this._isMovingTooFast = false;
				this.indicatorVisible = true;
			}

			public MissionScreen missionScreen;

			public bool indicatorVisible;

			public MatrixFrame indicatorFrame;

			public bool firstTime = true;

			public GameEntity indicatorEntity;

			public Vec3 nextIndicatorPosition;

			public Vec3 prevIndicatorPosition;

			public float indicatorAlpha = 1f;

			private float _drawIndicatorElapsedTime;

			private const float IndicatorExpireTime = 0.5f;

			private bool _isSeenByPlayer = true;

			internal bool _isMovingTooFast;
		}
	}
}
