using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x0200000D RID: 13
	public static class CraftingPieceCollectionElementViewExtensions
	{
		// Token: 0x0600005A RID: 90 RVA: 0x0000437C File Offset: 0x0000257C
		public static MatrixFrame GetCraftingPieceFrameForInventory(this CraftingPiece craftingPiece)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			Mat3 identity2 = Mat3.Identity;
			float num = 0.85f;
			Vec3 vec;
			vec..ctor(0f, 0f, 0f, -1f);
			MetaMesh copy = MetaMesh.GetCopy(craftingPiece.MeshName, true, false);
			if (copy != null)
			{
				identity2.RotateAboutSide(-1.5707964f);
				identity2.RotateAboutForward(-0.7853982f);
				Vec3 vec2;
				vec2..ctor(1000000f, 1000000f, 1000000f, -1f);
				Vec3 vec3;
				vec3..ctor(-1000000f, -1000000f, -1000000f, -1f);
				for (int num2 = 0; num2 != copy.MeshCount; num2++)
				{
					Vec3 boundingBoxMin = copy.GetMeshAtIndex(num2).GetBoundingBoxMin();
					Vec3 boundingBoxMax = copy.GetMeshAtIndex(num2).GetBoundingBoxMax();
					Vec3[] array = new Vec3[]
					{
						identity2.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMin.y, boundingBoxMin.z, -1f)),
						identity2.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMin.y, boundingBoxMax.z, -1f)),
						identity2.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMax.y, boundingBoxMin.z, -1f)),
						identity2.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMax.y, boundingBoxMax.z, -1f)),
						identity2.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMin.y, boundingBoxMin.z, -1f)),
						identity2.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMin.y, boundingBoxMax.z, -1f)),
						identity2.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMax.y, boundingBoxMin.z, -1f)),
						identity2.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMax.y, boundingBoxMax.z, -1f))
					};
					for (int i = 0; i < 8; i++)
					{
						vec2 = Vec3.Vec3Min(vec2, array[i]);
						vec3 = Vec3.Vec3Max(vec3, array[i]);
					}
				}
				float num3 = 1f;
				Vec3 vec4 = (vec2 + vec3) * 0.5f;
				float num4 = MathF.Max(vec3.x - vec2.x, vec3.y - vec2.y);
				float num5 = num * num3 / num4;
				identity.origin -= vec4 * num5;
				identity.origin += vec;
				identity.rotation = identity2;
				identity.rotation.ApplyScaleLocal(num5);
				identity.origin.z = identity.origin.z - 5f;
			}
			return identity;
		}
	}
}
