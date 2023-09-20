﻿using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	[EngineClass("rglTwo_dimension_view")]
	public sealed class TwoDimensionView : View
	{
		internal TwoDimensionView(UIntPtr pointer)
			: base(pointer)
		{
		}

		public static TwoDimensionView CreateTwoDimension()
		{
			return EngineApplicationInterface.ITwoDimensionView.CreateTwoDimensionView();
		}

		public void BeginFrame()
		{
			EngineApplicationInterface.ITwoDimensionView.BeginFrame(base.Pointer);
		}

		public void EndFrame()
		{
			EngineApplicationInterface.ITwoDimensionView.EndFrame(base.Pointer);
		}

		public void Clear()
		{
			EngineApplicationInterface.ITwoDimensionView.Clear(base.Pointer);
		}

		public void CreateMeshFromDescription(float[] vertices, float[] uvs, uint[] indices, int indexCount, Material material, TwoDimensionMeshDrawData meshDrawData)
		{
			EngineApplicationInterface.ITwoDimensionView.AddNewMesh(base.Pointer, vertices, uvs, indices, vertices.Length / 2, indexCount, material.Pointer, ref meshDrawData);
		}

		public void CreateMeshFromDescription(Material material, TwoDimensionMeshDrawData meshDrawData)
		{
			EngineApplicationInterface.ITwoDimensionView.AddNewQuadMesh(base.Pointer, material.Pointer, ref meshDrawData);
		}

		public bool CreateTextMeshFromCache(Material material, TwoDimensionTextMeshDrawData meshDrawData)
		{
			return EngineApplicationInterface.ITwoDimensionView.AddCachedTextMesh(base.Pointer, material.Pointer, ref meshDrawData);
		}

		public void CreateTextMeshFromDescription(float[] vertices, float[] uvs, uint[] indices, int indexCount, Material material, TwoDimensionTextMeshDrawData meshDrawData)
		{
			EngineApplicationInterface.ITwoDimensionView.AddNewTextMesh(base.Pointer, vertices, uvs, indices, vertices.Length / 2, indexCount, material.Pointer, ref meshDrawData);
		}
	}
}
