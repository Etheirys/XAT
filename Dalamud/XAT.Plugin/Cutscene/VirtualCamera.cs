using System;
using System.Numerics;

namespace XAT.Plugin.Cutscene;

public class VirtualCamera
{
    public record class CameraState(Vector3 Position, Quaternion Rotation, float FoV);

    public bool IsActive { get; set; } = false;

    public CameraState State { get; set; } = new CameraState(Vector3.Zero, Quaternion.Identity, 0.78f);
}
