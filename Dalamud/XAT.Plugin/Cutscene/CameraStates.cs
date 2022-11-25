using System.Numerics;

namespace XAT.Plugin.Cutscene;

public record class CameraState(Vector3 Position, Vector3 Rotation, float Zoom, float FoV);
public class CameraSettings
{
    public Vector3 Scale = Vector3.One;
    public Vector3 Offset = Vector3.Zero;
    public float Zoom = 0.1f;
    public bool Loop = true;
}