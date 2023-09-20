using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200000C RID: 12
	[EngineClass("rglCamera_object")]
	public sealed class Camera : NativeObject
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00002657 File Offset: 0x00000857
		internal Camera(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002666 File Offset: 0x00000866
		public static Camera CreateCamera()
		{
			return EngineApplicationInterface.ICamera.CreateCamera();
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002672 File Offset: 0x00000872
		public void ReleaseCamera()
		{
			EngineApplicationInterface.ICamera.Release(base.Pointer);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002684 File Offset: 0x00000884
		public void ReleaseCameraEntity()
		{
			EngineApplicationInterface.ICamera.ReleaseCameraEntity(base.Pointer);
			this.ReleaseCamera();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000269C File Offset: 0x0000089C
		~Camera()
		{
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000026C4 File Offset: 0x000008C4
		public void LookAt(Vec3 position, Vec3 target, Vec3 upVector)
		{
			EngineApplicationInterface.ICamera.LookAt(base.Pointer, position, target, upVector);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000026DC File Offset: 0x000008DC
		public void ScreenSpaceRayProjection(Vec2 screenPosition, ref Vec3 rayBegin, ref Vec3 rayEnd)
		{
			EngineApplicationInterface.ICamera.ScreenSpaceRayProjection(base.Pointer, screenPosition, ref rayBegin, ref rayEnd);
			if (this.Entity != null)
			{
				rayBegin = this.Entity.GetGlobalFrame().TransformToParent(rayBegin);
				rayEnd = this.Entity.GetGlobalFrame().TransformToParent(rayEnd);
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002748 File Offset: 0x00000948
		public bool CheckEntityVisibility(GameEntity entity)
		{
			return EngineApplicationInterface.ICamera.CheckEntityVisibility(base.Pointer, entity.Pointer);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002760 File Offset: 0x00000960
		public void SetViewVolume(bool perspective, float dLeft, float dRight, float dBottom, float dTop, float dNear, float dFar)
		{
			EngineApplicationInterface.ICamera.SetViewVolume(base.Pointer, perspective, dLeft, dRight, dBottom, dTop, dNear, dFar);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002788 File Offset: 0x00000988
		public static void GetNearPlanePointsStatic(ref MatrixFrame cameraFrame, float verticalFov, float aspectRatioXY, float newDNear, float newDFar, Vec3[] nearPlanePoints)
		{
			EngineApplicationInterface.ICamera.GetNearPlanePointsStatic(ref cameraFrame, verticalFov, aspectRatioXY, newDNear, newDFar, nearPlanePoints);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000279C File Offset: 0x0000099C
		public void GetNearPlanePoints(Vec3[] nearPlanePoints)
		{
			EngineApplicationInterface.ICamera.GetNearPlanePoints(base.Pointer, nearPlanePoints);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000027AF File Offset: 0x000009AF
		public void SetFovVertical(float verticalFov, float aspectRatioXY, float newDNear, float newDFar)
		{
			EngineApplicationInterface.ICamera.SetFovVertical(base.Pointer, verticalFov, aspectRatioXY, newDNear, newDFar);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000027C6 File Offset: 0x000009C6
		public void SetFovHorizontal(float horizontalFov, float aspectRatioXY, float newDNear, float newDFar)
		{
			EngineApplicationInterface.ICamera.SetFovHorizontal(base.Pointer, horizontalFov, aspectRatioXY, newDNear, newDFar);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000027DD File Offset: 0x000009DD
		public void GetViewProjMatrix(ref MatrixFrame viewProj)
		{
			EngineApplicationInterface.ICamera.GetViewProjMatrix(base.Pointer, ref viewProj);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000027F0 File Offset: 0x000009F0
		public float GetFovVertical()
		{
			return EngineApplicationInterface.ICamera.GetFovVertical(base.Pointer);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002802 File Offset: 0x00000A02
		public float GetFovHorizontal()
		{
			return EngineApplicationInterface.ICamera.GetFovHorizontal(base.Pointer);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002814 File Offset: 0x00000A14
		public float GetAspectRatio()
		{
			return EngineApplicationInterface.ICamera.GetAspectRatio(base.Pointer);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002826 File Offset: 0x00000A26
		public void FillParametersFrom(Camera otherCamera)
		{
			EngineApplicationInterface.ICamera.FillParametersFrom(base.Pointer, otherCamera.Pointer);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000283E File Offset: 0x00000A3E
		public void RenderFrustrum()
		{
			EngineApplicationInterface.ICamera.RenderFrustrum(base.Pointer);
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002850 File Offset: 0x00000A50
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00002862 File Offset: 0x00000A62
		public GameEntity Entity
		{
			get
			{
				return EngineApplicationInterface.ICamera.GetEntity(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.ICamera.SetEntity(base.Pointer, value.Pointer);
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000040 RID: 64 RVA: 0x0000287A File Offset: 0x00000A7A
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00002887 File Offset: 0x00000A87
		public Vec3 Position
		{
			get
			{
				return this.Frame.origin;
			}
			set
			{
				EngineApplicationInterface.ICamera.SetPosition(base.Pointer, value);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000042 RID: 66 RVA: 0x0000289A File Offset: 0x00000A9A
		public Vec3 Direction
		{
			get
			{
				return -this.Frame.rotation.u;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000028B4 File Offset: 0x00000AB4
		// (set) Token: 0x06000044 RID: 68 RVA: 0x000028DC File Offset: 0x00000ADC
		public MatrixFrame Frame
		{
			get
			{
				MatrixFrame matrixFrame = default(MatrixFrame);
				EngineApplicationInterface.ICamera.GetFrame(base.Pointer, ref matrixFrame);
				return matrixFrame;
			}
			set
			{
				EngineApplicationInterface.ICamera.SetFrame(base.Pointer, ref value);
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000028F0 File Offset: 0x00000AF0
		public float Near
		{
			get
			{
				return EngineApplicationInterface.ICamera.GetNear(base.Pointer);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00002902 File Offset: 0x00000B02
		public float Far
		{
			get
			{
				return EngineApplicationInterface.ICamera.GetFar(base.Pointer);
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002914 File Offset: 0x00000B14
		public float HorizontalFov
		{
			get
			{
				return EngineApplicationInterface.ICamera.GetHorizontalFov(base.Pointer);
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002926 File Offset: 0x00000B26
		public void ViewportPointToWorldRay(ref Vec3 rayBegin, ref Vec3 rayEnd, Vec2 viewportPoint)
		{
			EngineApplicationInterface.ICamera.ViewportPointToWorldRay(base.Pointer, ref rayBegin, ref rayEnd, viewportPoint.ToVec3(0f));
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002946 File Offset: 0x00000B46
		public Vec3 WorldPointToViewPortPoint(ref Vec3 worldPoint)
		{
			return EngineApplicationInterface.ICamera.WorldPointToViewportPoint(base.Pointer, ref worldPoint);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002959 File Offset: 0x00000B59
		public bool EnclosesPoint(Vec3 pointInWorldSpace)
		{
			return EngineApplicationInterface.ICamera.EnclosesPoint(base.Pointer, pointInWorldSpace);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000296C File Offset: 0x00000B6C
		public static MatrixFrame ConstructCameraFromPositionElevationBearing(Vec3 position, float elevation, float bearing)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.ICamera.ConstructCameraFromPositionElevationBearing(position, elevation, bearing, ref matrixFrame);
			return matrixFrame;
		}
	}
}
