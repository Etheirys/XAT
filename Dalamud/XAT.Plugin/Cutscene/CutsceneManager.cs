﻿using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;
using System.Diagnostics;
using System.Numerics;
using XAT.Plugin.Files;
using XAT.Plugin.Utils;

namespace XAT.Plugin.Cutscene;

public class CutsceneManager : IDisposable
{
    private const double FRAME_STEP = 33.33333333333333;

    private XATPlugin Plugin { get; }

    public XATCameraPathFile? CameraPath { get; set; }

    public CameraSettings CameraSettings { get; } = new();

    public VirtualCamera VirtualCamera => Plugin.VirtualCamera;

    public bool IsRunning => Stopwatch.IsRunning;
    private Stopwatch Stopwatch = new();

    private Vector3 BasePosition { get; set; }
    private Quaternion BaseRotation { get; set; }


    public CutsceneManager(XATPlugin plugin)
    {
        this.Plugin = plugin;
        Plugin.GPoseService.OnGPoseChange += GPoseService_OnGPoseChange; 
    }

    private void GPoseService_OnGPoseChange(bool newGPoseState)
    {
        if (!newGPoseState && IsRunning)
            StopPlayback();
    }

    public void StartPlayback()
    {
        unsafe
        {
            TargetSystem* targetSystem = (TargetSystem*)Plugin.TargetManager.Address;
            GameObject* gameObject = targetSystem->GPoseTarget;
            
            if (gameObject == null)
                return;

            BasePosition = new Vector3(gameObject->DrawObject->Object.Position.X, gameObject->DrawObject->Object.Position.Y, gameObject->DrawObject->Object.Position.Z);
            BaseRotation = new Quaternion(gameObject->DrawObject->Object.Rotation.X, gameObject->DrawObject->Object.Rotation.Y, gameObject->DrawObject->Object.Rotation.Z, gameObject->DrawObject->Object.Rotation.W);
        }
       

        Stopwatch.Reset();
        Stopwatch.Start();
        this.VirtualCamera.IsActive = true;

    }

    public void StopPlayback()
    {
        if (IsRunning)
        {
            Stopwatch.Reset();
            this.VirtualCamera.IsActive = false;
        }

    }

    public void Update()
    {
        if (!IsRunning || CameraPath == null)
            return;

        double totalMillis = Stopwatch.ElapsedMilliseconds;

        XATCameraPathFile.CameraKeyframe? previousKey = CameraPath.CameraFrames[0];
        XATCameraPathFile.CameraKeyframe? nextKey = null;

        foreach (var key in CameraPath.CameraFrames)
        {
            double frameStart = key.Frame * FRAME_STEP;
            if (frameStart > totalMillis)
            {
                nextKey = key;
                break;
            }
            else
            {
                previousKey = key;
            }
        }

        if (previousKey == null || nextKey == null)
        {
            if(CameraSettings.Loop)
            {
                Stopwatch.Restart();
                Update();
            }
            else
            {
                StopPlayback();
            }
            
            return;
        }

        double previousFrameStart = previousKey.Frame * FRAME_STEP;
        double nextFrameStart = nextKey.Frame * FRAME_STEP;
        double blendLength = nextFrameStart - previousFrameStart;
        double pastPreviousKey = totalMillis - previousFrameStart;
        float frameProgress = (float)(pastPreviousKey / blendLength);

        // First we calculate the raw position/rotation/fov based on the frame progress
        var rawPosition = Vector3.Lerp(previousKey.Position, nextKey.Position, frameProgress);
        var rawRotation = Quaternion.Lerp(previousKey.Rotation, nextKey.Rotation, frameProgress);
        float rawFoV = previousKey.FoV + (nextKey.FoV - previousKey.FoV) * frameProgress;

        // Apply the user adjustmenets for the position 
        var adjustedPosition = (rawPosition * CameraSettings.Scale) + CameraSettings.Offset; 

        // Now we apply the rotation from the base to the raw values and get a matrix for each
        Vector3 rotatedLocalPosition = BaseRotation.RotatePosition(adjustedPosition);
        Quaternion localRotation = BaseRotation * rawRotation;
        var localRotationMatrix = Matrix4x4.CreateFromQuaternion(localRotation);
        Matrix4x4.Invert(localRotationMatrix, out Matrix4x4 invertedLocalRotationMatrix);
        var localTranslationMatrix = Matrix4x4.CreateTranslation(-rotatedLocalPosition);

        // Create a matrix with the base position
        var basePositionMatrix = Matrix4x4.CreateTranslation(-BasePosition);      

        // Create the final matrix
        var finalMat = basePositionMatrix * (localTranslationMatrix * invertedLocalRotationMatrix);

        this.VirtualCamera.State = new
        (
            Matrix: finalMat,
            FoV: rawFoV
        );

    }

    public void Dispose()
    {
        
    }
}