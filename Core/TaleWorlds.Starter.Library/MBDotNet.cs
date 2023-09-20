using System;
using System.Runtime.InteropServices;
using System.Security;

namespace TaleWorlds.Starter.Library
{
	internal static class MBDotNet
	{
		[SuppressUnmanagedCodeSecurity]
		[DllImport("TaleWorlds.Native.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WotsMainSDLL")]
		public static extern int WotsMainDotNet(string args);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("TaleWorlds.Native.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "pass_controller_methods")]
		public static extern void PassControllerMethods(Delegate currentDomainInitializer);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("TaleWorlds.Native.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "pass_managed_initialize_method_pointer")]
		public static extern void PassManagedInitializeMethodPointerDotNet([MarshalAs(UnmanagedType.FunctionPtr)] Delegate initalizer);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("TaleWorlds.Native.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "pass_managed_library_callback_method_pointers")]
		public static extern void PassManagedEngineCallbackMethodPointersDotNet([MarshalAs(UnmanagedType.FunctionPtr)] Delegate methodDelegate);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern int SetCurrentDirectory(string args);

		public const string MainDllName = "TaleWorlds.Native.dll";
	}
}
