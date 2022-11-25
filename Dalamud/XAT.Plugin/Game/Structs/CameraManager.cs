using System.Runtime.InteropServices;

namespace XAT.Plugin.Game.Structs;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct CameraManager
{
    [FieldOffset(0x0)] public GameCamera* WorldCamera;
    [FieldOffset(0x8)] public GameCamera* IdleCamera;
    [FieldOffset(0x10)] public GameCamera* MenuCamera;
    [FieldOffset(0x18)] public GameCamera* SpectatorCamera;
}
