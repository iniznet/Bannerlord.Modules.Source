using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000330 RID: 816
	public class AnimatedFlag : ScriptComponentBehavior
	{
		// Token: 0x06002C14 RID: 11284 RVA: 0x000AA984 File Offset: 0x000A8B84
		public AnimatedFlag()
		{
			this._prevFlagMeshFrame = new Vec3(0f, 0f, 0f, -1f);
			this._prevTheta = 0f;
			this._prevSkew = 0f;
			this._time = 0f;
		}

		// Token: 0x06002C15 RID: 11285 RVA: 0x000AA9D7 File Offset: 0x000A8BD7
		protected internal override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
			MBDebug.Print("AnimatedFlag : OnInit called.", 0, Debug.DebugColor.Yellow, 17592186044416UL);
		}

		// Token: 0x06002C16 RID: 11286 RVA: 0x000AAA04 File Offset: 0x000A8C04
		private void SmoothTheta(ref float theta, float dt)
		{
			float num = theta - this._prevTheta;
			if (num > 3.1415927f)
			{
				num -= 6.2831855f;
				this._prevTheta += 6.2831855f;
			}
			else if (num < -3.1415927f)
			{
				num += 6.2831855f;
				this._prevTheta -= 6.2831855f;
			}
			num = MathF.Min(num, 150f * dt);
			theta = this._prevTheta + num * 0.05f;
		}

		// Token: 0x06002C17 RID: 11287 RVA: 0x000AAA7F File Offset: 0x000A8C7F
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		// Token: 0x06002C18 RID: 11288 RVA: 0x000AAA8C File Offset: 0x000A8C8C
		protected internal override void OnTick(float dt)
		{
			if (dt == 0f)
			{
				return;
			}
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			MetaMesh metaMesh = base.GameEntity.GetMetaMesh(0);
			if (metaMesh == null)
			{
				return;
			}
			Vec3 vec = globalFrame.origin - this._prevFlagMeshFrame;
			vec.x /= dt;
			vec.y /= dt;
			vec.z /= dt;
			vec = new Vec3(20f, 0f, -10f, -1f) * 0.1f - vec;
			if (vec.LengthSquared < 1E-08f)
			{
				return;
			}
			Vec3 vec2 = globalFrame.rotation.TransformToLocal(vec);
			vec2.z = 0f;
			vec2.Normalize();
			float num = MathF.Atan2(vec2.y, vec2.x);
			this.SmoothTheta(ref num, dt);
			Vec3 scaleVector = metaMesh.Frame.rotation.GetScaleVector();
			MatrixFrame identity = MatrixFrame.Identity;
			identity.Scale(scaleVector);
			identity.rotation.RotateAboutUp(num);
			this._prevTheta = num;
			float num2 = MathF.Acos(Vec3.DotProduct(vec, globalFrame.rotation.u) / vec.Length);
			float num3 = num2 - this._prevSkew;
			num3 = MathF.Min(num3, 150f * dt);
			num2 = this._prevSkew + num3 * 0.05f;
			this._prevSkew = num2;
			float num4 = MBMath.ClampFloat(vec.Length, 0.001f, 10000f);
			this._time += dt * num4 * 0.5f;
			metaMesh.Frame = identity;
			metaMesh.VectorUserData = new Vec3(MathF.Cos(num2), 1f - MathF.Sin(num2), 0f, this._time);
			this._prevFlagMeshFrame = globalFrame.origin;
		}

		// Token: 0x06002C19 RID: 11289 RVA: 0x000AAC6F File Offset: 0x000A8E6F
		protected internal override bool IsOnlyVisual()
		{
			return true;
		}

		// Token: 0x040010AA RID: 4266
		private float _prevTheta;

		// Token: 0x040010AB RID: 4267
		private float _prevSkew;

		// Token: 0x040010AC RID: 4268
		private Vec3 _prevFlagMeshFrame;

		// Token: 0x040010AD RID: 4269
		private float _time;
	}
}
