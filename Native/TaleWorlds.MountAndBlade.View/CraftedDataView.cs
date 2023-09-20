using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View
{
	public class CraftedDataView
	{
		public WeaponDesign CraftedData { get; private set; }

		public MetaMesh WeaponMesh
		{
			get
			{
				if (!(this._weaponMesh != null) || !this._weaponMesh.HasVertexBufferOrEditDataOrPackageItem())
				{
					return this._weaponMesh = this.GenerateWeaponMesh(true);
				}
				return this._weaponMesh;
			}
		}

		public MetaMesh HolsterMesh
		{
			get
			{
				MetaMesh metaMesh;
				if ((metaMesh = this._holsterMesh) == null)
				{
					metaMesh = (this._holsterMesh = this.GenerateHolsterMesh());
				}
				return metaMesh;
			}
		}

		public MetaMesh HolsterMeshWithWeapon
		{
			get
			{
				if (!(this._holsterMeshWithWeapon != null) || !this._holsterMeshWithWeapon.HasVertexBufferOrEditDataOrPackageItem())
				{
					return this._holsterMeshWithWeapon = this.GenerateHolsterMeshWithWeapon(true);
				}
				return this._holsterMeshWithWeapon;
			}
		}

		public MetaMesh NonBatchedWeaponMesh
		{
			get
			{
				MetaMesh metaMesh;
				if ((metaMesh = this._nonBatchedWeaponMesh) == null)
				{
					metaMesh = (this._nonBatchedWeaponMesh = this.GenerateWeaponMesh(false));
				}
				return metaMesh;
			}
		}

		public MetaMesh NonBatchedHolsterMesh
		{
			get
			{
				MetaMesh metaMesh;
				if ((metaMesh = this._nonBatchedHolsterMesh) == null)
				{
					metaMesh = (this._nonBatchedHolsterMesh = this.GenerateHolsterMesh());
				}
				return metaMesh;
			}
		}

		public MetaMesh NonBatchedHolsterMeshWithWeapon
		{
			get
			{
				MetaMesh metaMesh;
				if ((metaMesh = this._nonBatchedHolsterMeshWithWeapon) == null)
				{
					metaMesh = (this._nonBatchedHolsterMeshWithWeapon = this.GenerateHolsterMeshWithWeapon(false));
				}
				return metaMesh;
			}
		}

		public CraftedDataView(WeaponDesign craftedData)
		{
			this.CraftedData = craftedData;
		}

		public void Clear()
		{
			this._weaponMesh = null;
			this._holsterMesh = null;
			this._holsterMeshWithWeapon = null;
			this._nonBatchedWeaponMesh = null;
			this._nonBatchedHolsterMesh = null;
			this._nonBatchedHolsterMeshWithWeapon = null;
		}

		private MetaMesh GenerateWeaponMesh(bool batchMeshes)
		{
			if (this.CraftedData.UsedPieces != null)
			{
				return CraftedDataView.BuildWeaponMesh(this.CraftedData, 0f, false, batchMeshes);
			}
			return null;
		}

		private MetaMesh GenerateHolsterMesh()
		{
			if (this.CraftedData.UsedPieces != null)
			{
				return CraftedDataView.BuildHolsterMesh(this.CraftedData);
			}
			return null;
		}

		private MetaMesh GenerateHolsterMeshWithWeapon(bool batchMeshes)
		{
			if (this.CraftedData.UsedPieces != null)
			{
				return CraftedDataView.BuildHolsterMeshWithWeapon(this.CraftedData, 0f, batchMeshes);
			}
			return null;
		}

		public static MetaMesh BuildWeaponMesh(WeaponDesign craftedData, float pivotDiff, bool pieceTypeHidingEnabledForHolster, bool batchAllMeshes)
		{
			CraftingTemplate template = craftedData.Template;
			MetaMesh metaMesh = MetaMesh.CreateMetaMesh(null);
			List<MetaMesh> list = new List<MetaMesh>();
			List<MetaMesh> list2 = new List<MetaMesh>();
			List<MetaMesh> list3 = new List<MetaMesh>();
			foreach (PieceData pieceData in template.BuildOrders)
			{
				if (!pieceTypeHidingEnabledForHolster || !template.IsPieceTypeHiddenOnHolster(pieceData.PieceType))
				{
					WeaponDesignElement weaponDesignElement = craftedData.UsedPieces[pieceData.PieceType];
					float num = craftedData.PiecePivotDistances[pieceData.PieceType];
					if (weaponDesignElement != null && weaponDesignElement.IsValid && !float.IsNaN(num))
					{
						MetaMesh copy = MetaMesh.GetCopy(weaponDesignElement.CraftingPiece.MeshName, true, false);
						if (!batchAllMeshes)
						{
							copy.ClearMeshesForOtherLods(0);
						}
						MatrixFrame matrixFrame;
						matrixFrame..ctor(Mat3.Identity, num * Vec3.Up);
						if (weaponDesignElement.IsPieceScaled)
						{
							Vec3 vec = (weaponDesignElement.CraftingPiece.FullScale ? (Vec3.One * weaponDesignElement.ScaleFactor) : new Vec3(1f, 1f, weaponDesignElement.ScaleFactor, -1f));
							matrixFrame.Scale(vec);
						}
						copy.Frame = matrixFrame;
						if (copy.HasClothData())
						{
							list3.Add(copy);
						}
						else
						{
							list2.Add(copy);
						}
					}
				}
			}
			foreach (MetaMesh metaMesh2 in list2)
			{
				if (batchAllMeshes)
				{
					list.Add(metaMesh2);
				}
				else
				{
					metaMesh.MergeMultiMeshes(metaMesh2);
				}
			}
			if (batchAllMeshes)
			{
				metaMesh.BatchMultiMeshesMultiple(list);
			}
			foreach (MetaMesh metaMesh3 in list3)
			{
				metaMesh.MergeMultiMeshes(metaMesh3);
				metaMesh.AssignClothBodyFrom(metaMesh3);
			}
			metaMesh.SetEditDataPolicy(1);
			if (batchAllMeshes)
			{
				metaMesh.SetLodBias(1);
			}
			MatrixFrame frame = metaMesh.Frame;
			frame.Elevate(pivotDiff);
			metaMesh.Frame = frame;
			return metaMesh;
		}

		public static MetaMesh BuildHolsterMesh(WeaponDesign craftedData)
		{
			if (craftedData.Template.UseWeaponAsHolsterMesh)
			{
				return null;
			}
			BladeData bladeData = craftedData.UsedPieces[0].CraftingPiece.BladeData;
			if (craftedData.Template.AlwaysShowHolsterWithWeapon || string.IsNullOrEmpty(bladeData.HolsterMeshName))
			{
				return null;
			}
			float num = craftedData.PiecePivotDistances[0];
			MetaMesh copy = MetaMesh.GetCopy(bladeData.HolsterMeshName, false, false);
			MatrixFrame frame = copy.Frame;
			frame.origin += new Vec3(0f, 0f, num, -1f);
			WeaponDesignElement weaponDesignElement = craftedData.UsedPieces[0];
			if (MathF.Abs(weaponDesignElement.ScaledLength - weaponDesignElement.CraftingPiece.Length) > 1E-05f)
			{
				Vec3 vec = (weaponDesignElement.CraftingPiece.FullScale ? (Vec3.One * weaponDesignElement.ScaleFactor) : new Vec3(1f, 1f, weaponDesignElement.ScaleFactor, -1f));
				frame.Scale(vec);
			}
			copy.Frame = frame;
			MetaMesh metaMesh = MetaMesh.CreateMetaMesh(bladeData.HolsterMeshName);
			metaMesh.MergeMultiMeshes(copy);
			return metaMesh;
		}

		public static MetaMesh BuildHolsterMeshWithWeapon(WeaponDesign craftedData, float pivotDiff, bool batchAllMeshes)
		{
			if (craftedData.Template.UseWeaponAsHolsterMesh)
			{
				return null;
			}
			WeaponDesignElement weaponDesignElement = craftedData.UsedPieces[0];
			BladeData bladeData = weaponDesignElement.CraftingPiece.BladeData;
			if (string.IsNullOrEmpty(bladeData.HolsterMeshName))
			{
				return null;
			}
			MetaMesh metaMesh = MetaMesh.CreateMetaMesh(null);
			MetaMesh copy = MetaMesh.GetCopy(bladeData.HolsterMeshName, false, true);
			string text = bladeData.HolsterMeshName + "_skeleton";
			if (Skeleton.SkeletonModelExist(text))
			{
				MetaMesh metaMesh2 = CraftedDataView.BuildWeaponMesh(craftedData, 0f, true, batchAllMeshes);
				float num = craftedData.PiecePivotDistances[0];
				float scaledDistanceToPreviousPiece = craftedData.UsedPieces[0].ScaledDistanceToPreviousPiece;
				float num2 = num - scaledDistanceToPreviousPiece;
				List<MetaMesh> list = new List<MetaMesh>();
				Skeleton skeleton = Skeleton.CreateFromModel(text);
				for (sbyte b = 1; b < skeleton.GetBoneCount(); b += 1)
				{
					MatrixFrame boneEntitialRestFrame = skeleton.GetBoneEntitialRestFrame(b, false);
					if (craftedData.Template.RotateWeaponInHolster)
					{
						boneEntitialRestFrame.rotation.RotateAboutForward(3.1415927f);
					}
					MetaMesh metaMesh3 = metaMesh2.CreateCopy();
					MatrixFrame matrixFrame;
					matrixFrame..ctor(boneEntitialRestFrame.rotation, boneEntitialRestFrame.origin);
					matrixFrame.Elevate(-num2);
					metaMesh3.Frame = matrixFrame;
					if (batchAllMeshes)
					{
						int num3 = (int)(8 - (b - 1));
						metaMesh3.SetMaterial(Material.GetFromResource("weapon_crafting_quiver_deformer"));
						metaMesh3.SetFactor1Linear((uint)(419430400L * (long)num3));
						list.Add(metaMesh3);
					}
					else
					{
						metaMesh.MergeMultiMeshes(metaMesh3);
					}
				}
				if (list.Count > 0)
				{
					metaMesh.BatchMultiMeshesMultiple(list);
				}
				if (craftedData.Template.PieceTypeToScaleHolsterWith != -1)
				{
					WeaponDesignElement weaponDesignElement2 = craftedData.UsedPieces[craftedData.Template.PieceTypeToScaleHolsterWith];
					MatrixFrame frame = copy.Frame;
					int num4 = -MathF.Sign(skeleton.GetBoneEntitialRestFrame(0, false).rotation.u.z);
					float num5 = weaponDesignElement.CraftingPiece.BladeData.HolsterMeshLength * (weaponDesignElement2.ScaleFactor - 1f) * 0.5f * (float)num4;
					WeaponDesignElement weaponDesignElement3 = craftedData.UsedPieces[craftedData.Template.PieceTypeToScaleHolsterWith];
					if (weaponDesignElement3.IsPieceScaled)
					{
						Vec3 vec = (weaponDesignElement3.CraftingPiece.FullScale ? (Vec3.One * weaponDesignElement3.ScaleFactor) : new Vec3(1f, 1f, weaponDesignElement3.ScaleFactor, -1f));
						frame.Scale(vec);
					}
					frame.origin += new Vec3(0f, 0f, -num5, -1f);
					copy.Frame = frame;
				}
			}
			else
			{
				if (craftedData.Template.PieceTypeToScaleHolsterWith != -1)
				{
					MatrixFrame frame2 = copy.Frame;
					frame2.origin += new Vec3(0f, 0f, craftedData.PiecePivotDistances[craftedData.Template.PieceTypeToScaleHolsterWith], -1f);
					WeaponDesignElement weaponDesignElement4 = craftedData.UsedPieces[craftedData.Template.PieceTypeToScaleHolsterWith];
					if (weaponDesignElement4.IsPieceScaled)
					{
						Vec3 vec2 = (weaponDesignElement4.CraftingPiece.FullScale ? (Vec3.One * weaponDesignElement4.ScaleFactor) : new Vec3(1f, 1f, weaponDesignElement4.ScaleFactor, -1f));
						frame2.Scale(vec2);
					}
					copy.Frame = frame2;
				}
				metaMesh.MergeMultiMeshes(CraftedDataView.BuildWeaponMesh(craftedData, 0f, true, batchAllMeshes));
			}
			metaMesh.MergeMultiMeshes(copy);
			MatrixFrame frame3 = metaMesh.Frame;
			frame3.origin += new Vec3(0f, 0f, pivotDiff, -1f);
			metaMesh.Frame = frame3;
			return metaMesh;
		}

		private MetaMesh _weaponMesh;

		private MetaMesh _holsterMesh;

		private MetaMesh _holsterMeshWithWeapon;

		private MetaMesh _nonBatchedWeaponMesh;

		private MetaMesh _nonBatchedHolsterMesh;

		private MetaMesh _nonBatchedHolsterMeshWithWeapon;
	}
}
