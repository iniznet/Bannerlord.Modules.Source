using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000029 RID: 41
	[ApplicationInterfaceBase]
	internal interface ILight
	{
		// Token: 0x060003EB RID: 1003
		[EngineMethod("create_point_light", false)]
		Light CreatePointLight(float lightRadius);

		// Token: 0x060003EC RID: 1004
		[EngineMethod("set_radius", false)]
		void SetRadius(UIntPtr lightpointer, float radius);

		// Token: 0x060003ED RID: 1005
		[EngineMethod("set_light_flicker", false)]
		void SetLightFlicker(UIntPtr lightpointer, float magnitude, float interval);

		// Token: 0x060003EE RID: 1006
		[EngineMethod("enable_shadow", false)]
		void EnableShadow(UIntPtr lightpointer, bool shadowEnabled);

		// Token: 0x060003EF RID: 1007
		[EngineMethod("is_shadow_enabled", false)]
		bool IsShadowEnabled(UIntPtr lightpointer);

		// Token: 0x060003F0 RID: 1008
		[EngineMethod("set_volumetric_properties", false)]
		void SetVolumetricProperties(UIntPtr lightpointer, bool volumelightenabled, float volumeparameter);

		// Token: 0x060003F1 RID: 1009
		[EngineMethod("set_visibility", false)]
		void SetVisibility(UIntPtr lightpointer, bool value);

		// Token: 0x060003F2 RID: 1010
		[EngineMethod("get_radius", false)]
		float GetRadius(UIntPtr lightpointer);

		// Token: 0x060003F3 RID: 1011
		[EngineMethod("set_shadows", false)]
		void SetShadows(UIntPtr lightPointer, int shadowType);

		// Token: 0x060003F4 RID: 1012
		[EngineMethod("set_light_color", false)]
		void SetLightColor(UIntPtr lightpointer, Vec3 color);

		// Token: 0x060003F5 RID: 1013
		[EngineMethod("get_light_color", false)]
		Vec3 GetLightColor(UIntPtr lightpointer);

		// Token: 0x060003F6 RID: 1014
		[EngineMethod("set_intensity", false)]
		void SetIntensity(UIntPtr lightPointer, float value);

		// Token: 0x060003F7 RID: 1015
		[EngineMethod("get_intensity", false)]
		float GetIntensity(UIntPtr lightPointer);

		// Token: 0x060003F8 RID: 1016
		[EngineMethod("release", false)]
		void Release(UIntPtr lightpointer);

		// Token: 0x060003F9 RID: 1017
		[EngineMethod("set_frame", false)]
		void SetFrame(UIntPtr lightPointer, ref MatrixFrame frame);

		// Token: 0x060003FA RID: 1018
		[EngineMethod("get_frame", false)]
		void GetFrame(UIntPtr lightPointer, out MatrixFrame result);
	}
}
