using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Deform_Key_Data")]
	public struct DeformKeyData
	{
		public int GroupId;

		public int KeyTimePoint;

		public float Min;

		public float Max;

		public float Value;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string Id;
	}
}
