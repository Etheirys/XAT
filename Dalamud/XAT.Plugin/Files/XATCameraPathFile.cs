using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Linq;

namespace XAT.Plugin.Files;

public class XATCameraPathFile
{
    public List<CameraKeyframe> CameraFrames { get; private set; }

    public XATCameraPathFile(BinaryReader reader)
    {
        // Header
        string fileVersion = Encoding.ASCII.GetString(reader.ReadBytes(4));
        if (fileVersion != "XCP1")
        {
            throw new Exception("Unknown file type");
        }

        // Camera
        CameraFrames = new();
        int keyframeCount = (int)reader.ReadUInt32();
        for (int i = 0; i < keyframeCount - 1; i++)
        {
            float xPos = reader.ReadSingle();
            float yPos = reader.ReadSingle();
            float zPos = reader.ReadSingle();
            Vector3 position = new Vector3(xPos, yPos, zPos);

            float xRot = reader.ReadSingle();
            float yRot = reader.ReadSingle();
            float zRot = reader.ReadSingle();
            float wRot = reader.ReadSingle();
            Quaternion rotation = new Quaternion(xRot, yRot, zRot, wRot);

            float fov = reader.ReadSingle();

            CameraKeyframe cameraKeyframe = new CameraKeyframe(i, position, rotation, fov);
            CameraFrames.Add(cameraKeyframe);
        }
        CameraFrames = CameraFrames.OrderBy(x => x.Frame).ToList();
    }

    public record CameraKeyframe(int Frame, Vector3 Position, Quaternion Rotation, float FoV);
}
