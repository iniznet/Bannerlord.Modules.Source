using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000388 RID: 904
	public class WindMill : ScriptComponentBehavior
	{
		// Token: 0x060031A7 RID: 12711 RVA: 0x000CE157 File Offset: 0x000CC357
		protected internal override void OnInit()
		{
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x060031A8 RID: 12712 RVA: 0x000CE168 File Offset: 0x000CC368
		private void Rotate(float dt)
		{
			GameEntity gameEntity = base.GameEntity;
			float num = this.rotationSpeed * 0.001f * dt;
			MatrixFrame frame = gameEntity.GetFrame();
			frame.rotation.RotateAboutForward(num);
			gameEntity.SetFrame(ref frame);
			this.currentRotation += num;
		}

		// Token: 0x060031A9 RID: 12713 RVA: 0x000CE1B3 File Offset: 0x000CC3B3
		private static bool IsRotationPhaseInsidePhaseBoundaries(float currentPhase, float startPhase, float endPhase)
		{
			if (endPhase <= startPhase)
			{
				return currentPhase > startPhase;
			}
			return currentPhase > startPhase && currentPhase < endPhase;
		}

		// Token: 0x060031AA RID: 12714 RVA: 0x000CE1C8 File Offset: 0x000CC3C8
		public static int GetIntegerFromStringEnd(string str)
		{
			string text = "";
			for (int i = str.Length - 1; i > -1; i--)
			{
				char c = str[i];
				if (c < '0' || c > '9')
				{
					break;
				}
				text = c.ToString() + text;
			}
			return Convert.ToInt32(text);
		}

		// Token: 0x060031AB RID: 12715 RVA: 0x000CE214 File Offset: 0x000CC414
		private void DoWaterMillCalculation()
		{
			float num = (float)base.GameEntity.ChildCount;
			if (num > 0f)
			{
				IEnumerable<GameEntity> children = base.GameEntity.GetChildren();
				float num2 = 6.28f / num;
				foreach (GameEntity gameEntity in children)
				{
					int integerFromStringEnd = WindMill.GetIntegerFromStringEnd(gameEntity.Name);
					float num3 = this.currentRotation % 6.28f;
					float num4 = (num2 * (float)integerFromStringEnd + this.waterSplashPhaseOffset) % 6.28f;
					float num5 = (num4 + num2 * this.waterSplashIntervalMultiplier) % 6.28f;
					if (WindMill.IsRotationPhaseInsidePhaseBoundaries(num3, num4, num5))
					{
						gameEntity.ResumeParticleSystem(true);
					}
					else
					{
						gameEntity.PauseParticleSystem(true);
					}
				}
			}
		}

		// Token: 0x060031AC RID: 12716 RVA: 0x000CE2DC File Offset: 0x000CC4DC
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.TickParallel;
		}

		// Token: 0x060031AD RID: 12717 RVA: 0x000CE2E6 File Offset: 0x000CC4E6
		protected internal override void OnTickParallel(float dt)
		{
			this.Rotate(dt);
			if (this.isWaterMill)
			{
				this.DoWaterMillCalculation();
			}
		}

		// Token: 0x060031AE RID: 12718 RVA: 0x000CE2FD File Offset: 0x000CC4FD
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.Rotate(dt);
			if (this.isWaterMill)
			{
				this.DoWaterMillCalculation();
			}
		}

		// Token: 0x060031AF RID: 12719 RVA: 0x000CE31C File Offset: 0x000CC51C
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "testMesh")
			{
				if (this.testMesh != null)
				{
					base.GameEntity.AddMultiMesh(this.testMesh, true);
					return;
				}
			}
			else if (variableName == "testTexture")
			{
				if (this.testTexture != null)
				{
					Material material = base.GameEntity.GetFirstMesh().GetMaterial().CreateCopy();
					material.SetTexture(Material.MBTextureType.DiffuseMap, this.testTexture);
					base.GameEntity.SetMaterialForAllMeshes(material);
					return;
				}
			}
			else
			{
				if (variableName == "testEntity")
				{
					this.testEntity != null;
					return;
				}
				if (variableName == "testButton")
				{
					this.rotationSpeed *= 2f;
				}
			}
		}

		// Token: 0x040014D9 RID: 5337
		public float rotationSpeed = 100f;

		// Token: 0x040014DA RID: 5338
		public float waterSplashPhaseOffset;

		// Token: 0x040014DB RID: 5339
		public float waterSplashIntervalMultiplier = 1f;

		// Token: 0x040014DC RID: 5340
		public MetaMesh testMesh;

		// Token: 0x040014DD RID: 5341
		public Texture testTexture;

		// Token: 0x040014DE RID: 5342
		public GameEntity testEntity;

		// Token: 0x040014DF RID: 5343
		public bool isWaterMill;

		// Token: 0x040014E0 RID: 5344
		private float currentRotation;
	}
}
