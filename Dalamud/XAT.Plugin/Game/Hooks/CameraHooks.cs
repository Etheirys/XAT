using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;
using System.Diagnostics;
using System.Numerics;
using XAT.Plugin.Cutscene;
using XAT.Plugin.Game.Structs;
using XAT.Plugin.Utils;

namespace XAT.Plugin.Game.Hooks;

public unsafe class CameraHooks : IDisposable
{
    public CameraManager* CameraManager;

    private XATPlugin Plugin { get; }

    private delegate void GetCameraPositionDelegate(GameCamera* camera, IntPtr target, float* vectorPosition, bool swapPerson);
    private static Hook<GetCameraPositionDelegate>? GetCameraPositionHook;

    public delegate void SetCameraLookAtDelegate(GameCamera* camera, float* lookAtPosition, float* cameraPosition, float* a4);
    public static Hook<SetCameraLookAtDelegate>? SetCameraLookAtHook;

    public CameraHooks(XATPlugin plugin)
    {
        this.Plugin = plugin;

        this.CameraManager = (CameraManager*)Plugin.SigScanner.GetStaticAddressFromSig("4C 8D 35 ?? ?? ?? ?? 85 D2");

        var vtbl = CameraManager->WorldCamera->VTable;
        GetCameraPositionHook = Hook<GetCameraPositionDelegate>.FromAddress(vtbl[15], GetCameraPositionDetour);

        SetCameraLookAtHook = Hook<SetCameraLookAtDelegate>.FromAddress(vtbl[14], SetCameraLookAtDetour);

        GetCameraPositionHook.Enable();
        SetCameraLookAtHook.Enable();
    }

    private void GetCameraPositionDetour(GameCamera* camera, IntPtr target, float* vectorPosition, bool swapPerson)
    {
        if (CameraManager->WorldCamera != camera)
        {
            GetCameraPositionHook?.Original(camera, target, vectorPosition, swapPerson);
            return;
        }

        if (!Plugin.CutsceneManager.IsRunning)
        {
            GetCameraPositionHook?.Original(camera, target, vectorPosition, swapPerson);
            return;
        }

        var cameraState = Plugin.CutsceneManager.CameraState;
        if (cameraState == null)
        {
            GetCameraPositionHook?.Original(camera, target, vectorPosition, swapPerson);
            return;
        }

        GetCameraPositionHook?.Original(camera, target, vectorPosition, swapPerson);
        vectorPosition[0] = cameraState.Position.X;
        vectorPosition[1] = cameraState.Position.Y;
        vectorPosition[2] = cameraState.Position.Z;

        camera->CurrentHRotation = cameraState.Rotation.X;
        camera->CurrentVRotation = cameraState.Rotation.Y;
        camera->Tilt = cameraState.Rotation.Z;

        camera->CurrentFoV = cameraState.FoV;
        camera->CurrentZoom = cameraState.Zoom;
    }

    private void SetCameraLookAtDetour(GameCamera* camera, float* lookAtPosition, float* cameraPosition, float* a4)
    {
        if (!Plugin.CutsceneManager.IsRunning)
            SetCameraLookAtHook?.Original(camera, lookAtPosition, cameraPosition, a4);
    }

    public void Dispose()
    {
        GetCameraPositionHook?.Dispose();
        SetCameraLookAtHook?.Dispose();
    }

}