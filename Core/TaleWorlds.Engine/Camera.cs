﻿using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineClass("rglCamera_object")]
	public sealed class Camera : NativeObject
	{
		internal Camera(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		public static Camera CreateCamera()
		{
			return EngineApplicationInterface.ICamera.CreateCamera();
		}

		public void ReleaseCamera()
		{
			EngineApplicationInterface.ICamera.Release(base.Pointer);
		}

		public void ReleaseCameraEntity()
		{
			EngineApplicationInterface.ICamera.ReleaseCameraEntity(base.Pointer);
			this.ReleaseCamera();
		}

		~Camera()
		{
		}

		public void LookAt(Vec3 position, Vec3 target, Vec3 upVector)
		{
			EngineApplicationInterface.ICamera.LookAt(base.Pointer, position, target, upVector);
		}

		public void ScreenSpaceRayProjection(Vec2 screenPosition, ref Vec3 rayBegin, ref Vec3 rayEnd)
		{
			EngineApplicationInterface.ICamera.ScreenSpaceRayProjection(base.Pointer, screenPosition, ref rayBegin, ref rayEnd);
			if (this.Entity != null)
			{
				rayBegin = this.Entity.GetGlobalFrame().TransformToParent(rayBegin);
				rayEnd = this.Entity.GetGlobalFrame().TransformToParent(rayEnd);
			}
		}

		public bool CheckEntityVisibility(GameEntity entity)
		{
			return EngineApplicationInterface.ICamera.CheckEntityVisibility(base.Pointer, entity.Pointer);
		}

		public void SetViewVolume(bool perspective, float dLeft, float dRight, float dBottom, float dTop, float dNear, float dFar)
		{
			EngineApplicationInterface.ICamera.SetViewVolume(base.Pointer, perspective, dLeft, dRight, dBottom, dTop, dNear, dFar);
		}

		public static void GetNearPlanePointsStatic(ref MatrixFrame cameraFrame, float verticalFov, float aspectRatioXY, float newDNear, float newDFar, Vec3[] nearPlanePoints)
		{
			EngineApplicationInterface.ICamera.GetNearPlanePointsStatic(ref cameraFrame, verticalFov, aspectRatioXY, newDNear, newDFar, nearPlanePoints);
		}

		public void GetNearPlanePoints(Vec3[] nearPlanePoints)
		{
			EngineApplicationInterface.ICamera.GetNearPlanePoints(base.Pointer, nearPlanePoints);
		}

		public void SetFovVertical(float verticalFov, float aspectRatioXY, float newDNear, float newDFar)
		{
			EngineApplicationInterface.ICamera.SetFovVertical(base.Pointer, verticalFov, aspectRatioXY, newDNear, newDFar);
		}

		public void SetFovHorizontal(float horizontalFov, float aspectRatioXY, float newDNear, float newDFar)
		{
			EngineApplicationInterface.ICamera.SetFovHorizontal(base.Pointer, horizontalFov, aspectRatioXY, newDNear, newDFar);
		}

		public void GetViewProjMatrix(ref MatrixFrame viewProj)
		{
			EngineApplicationInterface.ICamera.GetViewProjMatrix(base.Pointer, ref viewProj);
		}

		public float GetFovVertical()
		{
			return EngineApplicationInterface.ICamera.GetFovVertical(base.Pointer);
		}

		public float GetFovHorizontal()
		{
			return EngineApplicationInterface.ICamera.GetFovHorizontal(base.Pointer);
		}

		public float GetAspectRatio()
		{
			return EngineApplicationInterface.ICamera.GetAspectRatio(base.Pointer);
		}

		public void FillParametersFrom(Camera otherCamera)
		{
			EngineApplicationInterface.ICamera.FillParametersFrom(base.Pointer, otherCamera.Pointer);
		}

		public void RenderFrustrum()
		{
			EngineApplicationInterface.ICamera.RenderFrustrum(base.Pointer);
		}

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

		public Vec3 Direction
		{
			get
			{
				return -this.Frame.rotation.u;
			}
		}

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

		public float Near
		{
			get
			{
				return EngineApplicationInterface.ICamera.GetNear(base.Pointer);
			}
		}

		public float Far
		{
			get
			{
				return EngineApplicationInterface.ICamera.GetFar(base.Pointer);
			}
		}

		public float HorizontalFov
		{
			get
			{
				return EngineApplicationInterface.ICamera.GetHorizontalFov(base.Pointer);
			}
		}

		public void ViewportPointToWorldRay(ref Vec3 rayBegin, ref Vec3 rayEnd, Vec2 viewportPoint)
		{
			EngineApplicationInterface.ICamera.ViewportPointToWorldRay(base.Pointer, ref rayBegin, ref rayEnd, viewportPoint.ToVec3(0f));
		}

		public Vec3 WorldPointToViewPortPoint(ref Vec3 worldPoint)
		{
			return EngineApplicationInterface.ICamera.WorldPointToViewportPoint(base.Pointer, ref worldPoint);
		}

		public bool EnclosesPoint(Vec3 pointInWorldSpace)
		{
			return EngineApplicationInterface.ICamera.EnclosesPoint(base.Pointer, pointInWorldSpace);
		}

		public static MatrixFrame ConstructCameraFromPositionElevationBearing(Vec3 position, float elevation, float bearing)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.ICamera.ConstructCameraFromPositionElevationBearing(position, elevation, bearing, ref matrixFrame);
			return matrixFrame;
		}
	}
}
