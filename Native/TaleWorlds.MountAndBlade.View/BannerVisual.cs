using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Tableaus;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000006 RID: 6
	public class BannerVisual : IBannerVisual
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00003407 File Offset: 0x00001607
		// (set) Token: 0x06000036 RID: 54 RVA: 0x0000340F File Offset: 0x0000160F
		public Banner Banner { get; private set; }

		// Token: 0x06000037 RID: 55 RVA: 0x00003418 File Offset: 0x00001618
		public BannerVisual(Banner banner)
		{
			this.Banner = banner;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003427 File Offset: 0x00001627
		public void ValidateCreateTableauTextures()
		{
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003429 File Offset: 0x00001629
		public Texture GetTableauTextureSmall(Action<Texture> setAction, bool isTableauOrNineGrid = true)
		{
			return TableauCacheManager.Current.BeginCreateBannerTexture(BannerCode.CreateFrom(this.Banner), setAction, isTableauOrNineGrid, false);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003443 File Offset: 0x00001643
		public Texture GetTableauTextureLarge(Action<Texture> setAction, bool isTableauOrNineGrid = true)
		{
			return TableauCacheManager.Current.BeginCreateBannerTexture(BannerCode.CreateFrom(this.Banner), setAction, isTableauOrNineGrid, true);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003460 File Offset: 0x00001660
		public static MatrixFrame GetMeshMatrix(ref Mesh mesh, float marginLeft, float marginTop, float width, float height, bool mirrored, float rotation, float deltaZ)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			float num = width / 1528f;
			float num2 = height / 1528f;
			float num3 = num / mesh.GetBoundingBoxWidth();
			float num4 = num2 / mesh.GetBoundingBoxHeight();
			identity.rotation.RotateAboutUp(rotation);
			if (mirrored)
			{
				identity.rotation.RotateAboutForward(3.1415927f);
			}
			identity.rotation.ApplyScaleLocal(new Vec3(num3, num4, 1f, -1f));
			identity.origin.x = 0f;
			identity.origin.y = 0f;
			identity.origin.x = identity.origin.x + marginLeft / 1528f;
			identity.origin.y = identity.origin.y - marginTop / 1528f;
			identity.origin.z = identity.origin.z + deltaZ;
			return identity;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000353C File Offset: 0x0000173C
		public MetaMesh ConvertToMultiMesh()
		{
			BannerData bannerData = this.Banner.BannerDataList[0];
			MetaMesh metaMesh = MetaMesh.CreateMetaMesh(null);
			Mesh fromResource = Mesh.GetFromResource(BannerManager.Instance.GetBackgroundMeshName(bannerData.MeshId));
			Mesh mesh = fromResource.CreateCopy();
			fromResource.ManualInvalidate();
			mesh.Color = BannerManager.GetColor(bannerData.ColorId2);
			mesh.Color2 = BannerManager.GetColor(bannerData.ColorId);
			MatrixFrame matrixFrame = BannerVisual.GetMeshMatrix(ref mesh, bannerData.Position.x, bannerData.Position.y, bannerData.Size.x, bannerData.Size.y, bannerData.Mirror, bannerData.RotationValue * 2f * 3.1415927f, 0.5f);
			mesh.SetLocalFrame(matrixFrame);
			metaMesh.AddMesh(mesh);
			mesh.ManualInvalidate();
			for (int i = 1; i < this.Banner.BannerDataList.Count; i++)
			{
				BannerData bannerData2 = this.Banner.BannerDataList[i];
				BannerIconData iconDataFromIconId = BannerManager.Instance.GetIconDataFromIconId(bannerData2.MeshId);
				Material fromResource2 = Material.GetFromResource(iconDataFromIconId.MaterialName);
				if (fromResource2 != null)
				{
					Mesh mesh2 = Mesh.CreateMeshWithMaterial(fromResource2);
					float num = (float)(iconDataFromIconId.TextureIndex % 4) * 0.25f;
					float num2 = 1f - (float)(iconDataFromIconId.TextureIndex / 4) * 0.25f;
					Vec2 vec;
					vec..ctor(num, num2);
					Vec2 vec2;
					vec2..ctor(num + 0.25f, num2 - 0.25f);
					UIntPtr uintPtr = mesh2.LockEditDataWrite();
					int num3 = mesh2.AddFaceCorner(new Vec3(-0.5f, -0.5f, 0f, -1f), new Vec3(0f, 0f, 1f, -1f), vec + new Vec2(0f, -0.25f), uint.MaxValue, uintPtr);
					int num4 = mesh2.AddFaceCorner(new Vec3(0.5f, -0.5f, 0f, -1f), new Vec3(0f, 0f, 1f, -1f), vec2, uint.MaxValue, uintPtr);
					int num5 = mesh2.AddFaceCorner(new Vec3(0.5f, 0.5f, 0f, -1f), new Vec3(0f, 0f, 1f, -1f), vec + new Vec2(0.25f, 0f), uint.MaxValue, uintPtr);
					int num6 = mesh2.AddFaceCorner(new Vec3(-0.5f, 0.5f, 0f, -1f), new Vec3(0f, 0f, 1f, -1f), vec, uint.MaxValue, uintPtr);
					mesh2.AddFace(num3, num4, num5, uintPtr);
					mesh2.AddFace(num5, num6, num3, uintPtr);
					mesh2.UnlockEditDataWrite(uintPtr);
					mesh2.SetColorAndStroke(BannerManager.GetColor(bannerData2.ColorId), BannerManager.GetColor(bannerData2.ColorId2), bannerData2.DrawStroke);
					matrixFrame = BannerVisual.GetMeshMatrix(ref mesh2, bannerData2.Position.x, bannerData2.Position.y, bannerData2.Size.x, bannerData2.Size.y, bannerData2.Mirror, bannerData2.RotationValue * 2f * 3.1415927f, (float)i);
					mesh2.SetLocalFrame(matrixFrame);
					metaMesh.AddMesh(mesh2);
				}
			}
			return metaMesh;
		}
	}
}
