using System.Numerics;

namespace XAT.Plugin.Cutscene;

public record class CameraState(Vector3 Position, Quaternion Rotation, float FoV);
public class CameraSettings
{
    public Vector3 Scale = Vector3.One;
    public Vector3 Offset = Vector3.Zero;
    public bool Loop = true;
}