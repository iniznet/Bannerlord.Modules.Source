using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000010 RID: 16
	public static class ItemObjectViewExtensions
	{
		// Token: 0x0600006D RID: 109 RVA: 0x00005138 File Offset: 0x00003338
		public static MetaMesh GetCraftedMultiMesh(this ItemObject itemObject, bool needBatchedVersion)
		{
			CraftedDataView craftedDataView = CraftedDataViewManager.GetCraftedDataView(itemObject.WeaponDesign);
			if (!needBatchedVersion)
			{
				if (craftedDataView == null)
				{
					return null;
				}
				return craftedDataView.NonBatchedWeaponMesh.CreateCopy();
			}
			else
			{
				if (craftedDataView == null)
				{
					return null;
				}
				return craftedDataView.WeaponMesh.CreateCopy();
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00005178 File Offset: 0x00003378
		public static MetaMesh GetMultiMeshCopy(this ItemObject itemObject)
		{
			MetaMesh craftedMultiMesh = itemObject.GetCraftedMultiMesh(true);
			if (craftedMultiMesh != null)
			{
				return craftedMultiMesh;
			}
			if (string.IsNullOrEmpty(itemObject.MultiMeshName))
			{
				return null;
			}
			return MetaMesh.GetCopy(itemObject.MultiMeshName, true, false);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000051B4 File Offset: 0x000033B4
		public static MetaMesh GetMultiMeshCopyWithGenderData(this ItemObject itemObject, bool isFemale, bool hasGloves, bool needBatchedVersion)
		{
			MetaMesh craftedMultiMesh = itemObject.GetCraftedMultiMesh(needBatchedVersion);
			if (craftedMultiMesh != null)
			{
				return craftedMultiMesh;
			}
			if (string.IsNullOrEmpty(itemObject.MultiMeshName))
			{
				return null;
			}
			MetaMesh metaMesh = MetaMesh.GetCopy(isFemale ? (itemObject.MultiMeshName + "_female") : (itemObject.MultiMeshName + "_male"), false, true);
			if (metaMesh != null)
			{
				return metaMesh;
			}
			string text = itemObject.MultiMeshName;
			if (isFemale)
			{
				text += (hasGloves ? "_converted_slim" : "_converted");
			}
			else
			{
				text += (hasGloves ? "_slim" : "");
			}
			metaMesh = MetaMesh.GetCopy(text, false, true);
			if (metaMesh != null)
			{
				return metaMesh;
			}
			metaMesh = MetaMesh.GetCopy(itemObject.MultiMeshName, true, true);
			if (metaMesh != null)
			{
				return metaMesh;
			}
			return null;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00005284 File Offset: 0x00003484
		public static MatrixFrame GetScaledFrame(this ItemObject itemObject, Mat3 rotationMatrix, MetaMesh metaMesh, float scaleFactor, Vec3 positionShift)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			Vec3 vec;
			vec..ctor(1000000f, 1000000f, 1000000f, -1f);
			Vec3 vec2;
			vec2..ctor(-1000000f, -1000000f, -1000000f, -1f);
			for (int num = 0; num != metaMesh.MeshCount; num++)
			{
				Vec3 boundingBoxMin = metaMesh.GetMeshAtIndex(num).GetBoundingBoxMin();
				Vec3 boundingBoxMax = metaMesh.GetMeshAtIndex(num).GetBoundingBoxMax();
				Vec3[] array = new Vec3[]
				{
					rotationMatrix.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMin.y, boundingBoxMin.z, -1f)),
					rotationMatrix.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMin.y, boundingBoxMax.z, -1f)),
					rotationMatrix.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMax.y, boundingBoxMin.z, -1f)),
					rotationMatrix.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMax.y, boundingBoxMax.z, -1f)),
					rotationMatrix.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMin.y, boundingBoxMin.z, -1f)),
					rotationMatrix.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMin.y, boundingBoxMax.z, -1f)),
					rotationMatrix.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMax.y, boundingBoxMin.z, -1f)),
					rotationMatrix.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMax.y, boundingBoxMax.z, -1f))
				};
				for (int i = 0; i < 8; i++)
				{
					vec = Vec3.Vec3Min(vec, array[i]);
					vec2 = Vec3.Vec3Max(vec2, array[i]);
				}
			}
			float num2 = 1f;
			if (itemObject.PrimaryWeapon != null && itemObject.PrimaryWeapon.IsMeleeWeapon)
			{
				num2 = 0.3f + (float)itemObject.WeaponComponent.PrimaryWeapon.WeaponLength / 1.6f;
				num2 = MBMath.ClampFloat(num2, 0.5f, 1f);
			}
			Vec3 vec3 = (vec + vec2) * 0.5f;
			float num3 = MathF.Max(vec2.x - vec.x, vec2.y - vec.y);
			float num4 = scaleFactor * num2 / num3;
			identity.origin -= vec3 * num4;
			identity.origin += positionShift;
			identity.rotation = rotationMatrix;
			identity.rotation.ApplyScaleLocal(num4);
			return identity;
		}
	}
}
