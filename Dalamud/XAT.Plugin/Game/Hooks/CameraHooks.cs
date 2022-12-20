using Dalamud.Hooking;
using System;
using System.Numerics;
using XAT.Plugin.Cutscene;
using XAT.Plugin.Utils;

namespace XAT.Plugin.Game.Hooks;

public unsafe class CameraHooks : IDisposable
{
    private XATPlugin Plugin { get; }
    private VirtualCamera VirtualCamera => Plugin.VirtualCamera;

    private delegate Matrix4x4* MakeProjectionMatrix2(IntPtr ptr, float fov, float aspect, float nearPlane, float farPlane, float a6, float a7);
    private static Hook<MakeProjectionMatrix2> ProjectionHook = null!;

    private delegate Matrix4x4* CalculateViewMatrix(IntPtr a1);
    private static Hook<CalculateViewMatrix> ViewHook = null!;

    public CameraHooks(XATPlugin plugin)
    {
        this.Plugin = plugin;

        var proj = Plugin.SigScanner.ScanText("E8 ?? ?? ?? ?? 4C 8B 2D ?? ?? ?? ?? 41 0F 28 C2");
        ProjectionHook = Hook<MakeProjectionMatrix2>.FromAddress(proj, ProjectionDetour);

        var view = Plugin.SigScanner.ScanText("E8 ?? ?? ?? ?? 33 C0 48 89 83 ?? ?? ?? ?? 48 8B 9C 24 ?? ?? ?? ??");
        ViewHook = Hook<CalculateViewMatrix>.FromAddress(view, ViewMatrixDetour);

        ProjectionHook.Enable();
        ViewHook.Enable();
    }

    private unsafe Matrix4x4* ProjectionDetour(IntPtr ptr, float fov, float aspect, float nearPlane, float farPlane, float a6, float a7)
    {
        if (VirtualCamera.IsActive)
            fov = VirtualCamera.State.FoV;

        var exec = ProjectionHook.Original(ptr, fov, aspect, nearPlane, farPlane, a6, a7);

        return exec;
    }

    private unsafe Matrix4x4* ViewMatrixDetour(IntPtr a1)
    {
        var exec = ViewHook.Original(a1);

        if (!VirtualCamera.IsActive)
            return exec;

        var cameraState = VirtualCamera.State;
        var rotMat = Matrix4x4.CreateFromQuaternion(cameraState.Rotation);
        Matrix4x4.Invert(rotMat, out Matrix4x4 invRotMat);
        var transMat = Matrix4x4.CreateTranslation(-cameraState.Position);
        var finalMat = transMat * invRotMat;

        *exec = finalMat;

        return exec;
    }

    public void Dispose()
    {
        ProjectionHook.Dispose();
        ViewHook.Dispose();
    }

}