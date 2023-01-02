using System;
using System.Numerics;

namespace XAT.Plugin.Cutscene;

public class VirtualCamera
{
    public record class CameraState(Matrix4x4 Matrix, float FoV);

    public bool IsActive { get; set; } = false;

    public CameraState State { get; set; } = new CameraState(Matrix4x4.Identity, 0.78f);
}
