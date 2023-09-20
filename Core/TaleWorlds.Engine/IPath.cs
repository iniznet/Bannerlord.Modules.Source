using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000016 RID: 22
	[ApplicationInterfaceBase]
	internal interface IPath
	{
		// Token: 0x060000E7 RID: 231
		[EngineMethod("get_number_of_points", false)]
		int GetNumberOfPoints(UIntPtr ptr);

		// Token: 0x060000E8 RID: 232
		[EngineMethod("get_hermite_frame_wrt_dt", false)]
		void GetHermiteFrameForDt(UIntPtr ptr, ref MatrixFrame frame, float phase, int firstPoint);

		// Token: 0x060000E9 RID: 233
		[EngineMethod("get_hermite_frame_wrt_distance", false)]
		void GetHermiteFrameForDistance(UIntPtr ptr, ref MatrixFrame frame, float distance);

		// Token: 0x060000EA RID: 234
		[EngineMethod("get_nearest_hermite_frame_with_valid_alpha_wrt_distance", false)]
		void GetNearestHermiteFrameWithValidAlphaForDistance(UIntPtr ptr, ref MatrixFrame frame, float distance, bool searchForward, float alphaThreshold);

		// Token: 0x060000EB RID: 235
		[EngineMethod("get_hermite_frame_and_color_wrt_distance", false)]
		void GetHermiteFrameAndColorForDistance(UIntPtr ptr, out MatrixFrame frame, out Vec3 color, float distance);

		// Token: 0x060000EC RID: 236
		[EngineMethod("get_arc_length", false)]
		float GetArcLength(UIntPtr ptr, int firstPoint);

		// Token: 0x060000ED RID: 237
		[EngineMethod("get_points", false)]
		void GetPoints(UIntPtr ptr, MatrixFrame[] points);

		// Token: 0x060000EE RID: 238
		[EngineMethod("get_path_length", false)]
		float GetTotalLength(UIntPtr ptr);

		// Token: 0x060000EF RID: 239
		[EngineMethod("get_path_version", false)]
		int GetVersion(UIntPtr ptr);

		// Token: 0x060000F0 RID: 240
		[EngineMethod("set_frame_of_point", false)]
		void SetFrameOfPoint(UIntPtr ptr, int pointIndex, ref MatrixFrame frame);

		// Token: 0x060000F1 RID: 241
		[EngineMethod("set_tangent_position_of_point", false)]
		void SetTangentPositionOfPoint(UIntPtr ptr, int pointIndex, int tangentIndex, ref Vec3 position);

		// Token: 0x060000F2 RID: 242
		[EngineMethod("add_path_point", false)]
		int AddPathPoint(UIntPtr ptr, int newNodeIndex);

		// Token: 0x060000F3 RID: 243
		[EngineMethod("delete_path_point", false)]
		void DeletePathPoint(UIntPtr ptr, int newNodeIndex);

		// Token: 0x060000F4 RID: 244
		[EngineMethod("has_valid_alpha_at_path_point", false)]
		bool HasValidAlphaAtPathPoint(UIntPtr ptr, int nodeIndex, float alphaThreshold);

		// Token: 0x060000F5 RID: 245
		[EngineMethod("get_name", false)]
		string GetName(UIntPtr ptr);
	}
}
