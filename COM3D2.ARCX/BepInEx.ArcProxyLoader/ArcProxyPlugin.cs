using System;
using System.IO;
using System.Runtime.InteropServices;
using BepInEx.Logging;

namespace BepInEx.ArcProxyLoader
{
    [BepInPlugin("horse.coder.cm3d2.arcproxyloader", "ArcProxyLoader", "1.0")]
    public class ArcProxyPlugin : BaseUnityPlugin
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr module, [MarshalAs(UnmanagedType.LPStr)] string proc);

        private delegate void InstallArcXDelegate();

        private static InstallArcXDelegate InstallArcX;

        public ArcProxyPlugin()
        {
            Logger.Log(LogLevel.Info, "ArcProxy Launched!");

            string nativeDll = Path.GetFullPath(Path.Combine(Paths.PluginPath, "ArcProxy.dll"));

            Logger.Log(LogLevel.Info, $"Native DLL path: {nativeDll}");

            IntPtr dll = LoadLibrary(nativeDll);

            Logger.Log(LogLevel.Info, $"Got DLL at: {dll.ToString("X")}");

            if (dll == IntPtr.Zero)
            {
                Logger.Log(LogLevel.Error, "Failed to load dll!");
                return;
            }

            IntPtr installProc = GetProcAddress(dll, "InstallArcX");

            Logger.Log(LogLevel.Info, $"Got entry point at: {installProc.ToString("X")}");

            if (installProc == IntPtr.Zero)
            {
                Logger.Log(LogLevel.Error, "Failed to find entrypoint!");
                return;
            }

            InstallArcX = (InstallArcXDelegate)Marshal.GetDelegateForFunctionPointer(installProc, typeof(InstallArcXDelegate));

            Logger.Log(LogLevel.Info, "Running entry point!");

            InstallArcX();
        }
    }
}
