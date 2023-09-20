using System;
using System.Runtime.InteropServices;
using System.Security;

namespace TaleWorlds.Starter.Library
{
	// Token: 0x02000003 RID: 3
	internal static class MBDotNet
	{
		// Token: 0x06000004 RID: 4
		[SuppressUnmanagedCodeSecurity]
		[DllImport("TaleWorlds.Native.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WotsMainSDLL")]
		public static extern int WotsMainDotNet(string args);

		// Token: 0x06000005 RID: 5
		[SuppressUnmanagedCodeSecurity]
		[DllImport("TaleWorlds.Native.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "pass_controller_methods")]
		public static extern void PassControllerMethods(Delegate currentDomainInitializer);

		// Token: 0x06000006 RID: 6
		[SuppressUnmanagedCodeSecurity]
		[DllImport("TaleWorlds.Native.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "pass_managed_initialize_method_pointer")]
		public static extern void PassManagedInitializeMethodPointerDotNet([MarshalAs(UnmanagedType.FunctionPtr)] Delegate initalizer);

		// Token: 0x06000007 RID: 7
		[SuppressUnmanagedCodeSecurity]
		[DllImport("TaleWorlds.Native.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "pass_managed_library_callback_method_pointers")]
		public static extern void PassManagedEngineCallbackMethodPointersDotNet([MarshalAs(UnmanagedType.FunctionPtr)] Delegate methodDelegate);

		// Token: 0x06000008 RID: 8
		[SuppressUnmanagedCodeSecurity]
		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern int SetCurrentDirectory(string args);

		// Token: 0x04000002 RID: 2
		public const string MainDllName = "TaleWorlds.Native.dll";
	}
}
