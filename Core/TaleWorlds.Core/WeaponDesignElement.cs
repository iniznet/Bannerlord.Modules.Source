﻿using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
	// Token: 0x02000038 RID: 56
	public class WeaponDesignElement
	{
		// Token: 0x06000410 RID: 1040 RVA: 0x0000FAE0 File Offset: 0x0000DCE0
		internal static void AutoGeneratedStaticCollectObjectsWeaponDesignElement(object o, List<object> collectedObjects)
		{
			((WeaponDesignElement)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x0000FAEE File Offset: 0x0000DCEE
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._craftingPiece);
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x0000FAFC File Offset: 0x0000DCFC
		internal static object AutoGeneratedGetMemberValueScalePercentage(object o)
		{
			return ((WeaponDesignElement)o).ScalePercentage;
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x0000FB0E File Offset: 0x0000DD0E
		internal static object AutoGeneratedGetMemberValue_craftingPiece(object o)
		{
			return ((WeaponDesignElement)o)._craftingPiece;
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000414 RID: 1044 RVA: 0x0000FB1B File Offset: 0x0000DD1B
		public float ScaleFactor
		{
			get
			{
				return (float)this.ScalePercentage * 0.01f;
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000415 RID: 1045 RVA: 0x0000FB2A File Offset: 0x0000DD2A
		public bool IsPieceScaled
		{
			get
			{
				return this.ScalePercentage != 100;
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x0000FB39 File Offset: 0x0000DD39
		public CraftingPiece CraftingPiece
		{
			get
			{
				return this._craftingPiece;
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x0000FB41 File Offset: 0x0000DD41
		public bool IsValid
		{
			get
			{
				return this.CraftingPiece.IsValid;
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000418 RID: 1048 RVA: 0x0000FB4E File Offset: 0x0000DD4E
		public float ScaledLength
		{
			get
			{
				if (!this.IsPieceScaled)
				{
					return this.CraftingPiece.Length;
				}
				return this.CraftingPiece.Length * this.ScaleFactor;
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000419 RID: 1049 RVA: 0x0000FB78 File Offset: 0x0000DD78
		public float ScaledWeight
		{
			get
			{
				if (!this.IsPieceScaled)
				{
					return this.CraftingPiece.Weight;
				}
				float num = (this._craftingPiece.FullScale ? (this.ScaleFactor * this.ScaleFactor * this.ScaleFactor) : this.ScaleFactor);
				return this.CraftingPiece.Weight * num;
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x0600041A RID: 1050 RVA: 0x0000FBD0 File Offset: 0x0000DDD0
		public float ScaledCenterOfMass
		{
			get
			{
				if (!this.IsPieceScaled)
				{
					return this.CraftingPiece.CenterOfMass;
				}
				return this.CraftingPiece.CenterOfMass * this.ScaleFactor;
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x0600041B RID: 1051 RVA: 0x0000FBF8 File Offset: 0x0000DDF8
		public float ScaledDistanceToNextPiece
		{
			get
			{
				if (!this.IsPieceScaled)
				{
					return this.CraftingPiece.DistanceToNextPiece;
				}
				return this.CraftingPiece.DistanceToNextPiece * this.ScaleFactor;
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x0000FC20 File Offset: 0x0000DE20
		public float ScaledDistanceToPreviousPiece
		{
			get
			{
				if (!this.IsPieceScaled)
				{
					return this.CraftingPiece.DistanceToPreviousPiece;
				}
				return this.CraftingPiece.DistanceToPreviousPiece * this.ScaleFactor;
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x0600041D RID: 1053 RVA: 0x0000FC48 File Offset: 0x0000DE48
		public float ScaledBladeLength
		{
			get
			{
				if (!this.IsPieceScaled)
				{
					return this.CraftingPiece.BladeData.BladeLength;
				}
				return this.CraftingPiece.BladeData.BladeLength * this.ScaleFactor;
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600041E RID: 1054 RVA: 0x0000FC7A File Offset: 0x0000DE7A
		public float ScaledPieceOffset
		{
			get
			{
				if (!this.IsPieceScaled)
				{
					return this.CraftingPiece.PieceOffset;
				}
				return this.CraftingPiece.PieceOffset * this.ScaleFactor;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600041F RID: 1055 RVA: 0x0000FCA2 File Offset: 0x0000DEA2
		public float ScaledPreviousPieceOffset
		{
			get
			{
				if (!this.IsPieceScaled)
				{
					return this.CraftingPiece.PreviousPieceOffset;
				}
				return this.CraftingPiece.PreviousPieceOffset * this.ScaleFactor;
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000420 RID: 1056 RVA: 0x0000FCCA File Offset: 0x0000DECA
		public float ScaledNextPieceOffset
		{
			get
			{
				if (!this.IsPieceScaled)
				{
					return this.CraftingPiece.NextPieceOffset;
				}
				return this.CraftingPiece.NextPieceOffset * this.ScaleFactor;
			}
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x0000FCF2 File Offset: 0x0000DEF2
		public void SetScale(int scalePercentage)
		{
			this.ScalePercentage = scalePercentage;
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x0000FCFB File Offset: 0x0000DEFB
		private WeaponDesignElement(CraftingPiece craftingPiece, int scalePercentage = 100)
		{
			this._craftingPiece = craftingPiece;
			this.ScalePercentage = scalePercentage;
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x0000FD11 File Offset: 0x0000DF11
		public WeaponDesignElement GetCopy()
		{
			return new WeaponDesignElement(this.CraftingPiece, this.ScalePercentage);
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0000FD24 File Offset: 0x0000DF24
		public static WeaponDesignElement GetInvalidPieceForType(CraftingPiece.PieceTypes pieceType)
		{
			return new WeaponDesignElement(CraftingPiece.GetInvalidCraftingPiece(pieceType), 100);
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x0000FD33 File Offset: 0x0000DF33
		public static WeaponDesignElement CreateUsablePiece(CraftingPiece craftingPiece, int scalePercentage = 100)
		{
			return new WeaponDesignElement(craftingPiece, scalePercentage);
		}

		// Token: 0x04000219 RID: 537
		[SaveableField(10)]
		private readonly CraftingPiece _craftingPiece;

		// Token: 0x0400021A RID: 538
		[SaveableField(20)]
		public int ScalePercentage;
	}
}
