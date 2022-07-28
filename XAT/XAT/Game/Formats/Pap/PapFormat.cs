using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using XAT.Game.Formats.Tmb;
using XAT.Game.Formats.Utils;

namespace XAT.Game.Formats.Pap;

[AddINotifyPropertyChangedInterface]
public class PapFormat
{
    private const string MAGIC = "pap ";

    public int Skeleton { get; set; }
    public byte[] HavokData { get; set; }
    public List<PapAnimation> Animations { get; private set; } = new();

    public PapFormat(BinaryReader reader)
    {
        // Magic
        string magic = reader.ReadEncodedString(4);
        if (magic != MAGIC)
            throw new Exception("Invalid pap file - magic incorrect");

        // Version
        _ = reader.ReadInt32();

        // Num Anims
        int numAnims = reader.ReadInt16();

        // Skeleton
        this.Skeleton = reader.ReadInt32();

        // Offsets
        int infoOffset = reader.ReadInt32(); // Anim Info Offset
        int havokOffset = reader.ReadInt32(); // Havok Offset
        int timelineOffset = reader.ReadInt32(); // Timeline Offset

        // Anims
        List<PapAnimDataFormat> dataList = new();
        reader.BaseStream.Seek(infoOffset, SeekOrigin.Begin);
        for (int i = 0; i < numAnims; i++)
        {
            PapAnimDataFormat animInfo = new(reader);
            dataList.Add(animInfo);
        }

        // Havok Data
        reader.BaseStream.Seek(havokOffset, SeekOrigin.Begin);
        this.HavokData = reader.ReadBytes(timelineOffset - havokOffset);

        // Timeline Data
        List<TmbFormat> timelineList = new();
        reader.BaseStream.Seek(timelineOffset, SeekOrigin.Begin);
        for (int i = 0; i < numAnims; i++)
        {
            timelineList.Add(new TmbFormat(reader));
            var requiredPadding = AlignBoundary(reader.BaseStream.Position, i, numAnims);
            _ = reader.ReadBytes(requiredPadding);
        }

        // Build the final list
        if (timelineList.Count != dataList.Count)
            throw new Exception("Anim data header count and timeline count does not match.");

        for (int i = 0; i < numAnims; i++)
        {
            Animations.Add(new PapAnimation(this, i, dataList[i], timelineList[i]));
        }
    }

    public void Serialize(BinaryWriter writer)
    {
        int startPos = (int)writer.BaseStream.Position;

        // Magic
        writer.WriteEncodedString(MAGIC);

        // Version
        writer.Write(0x00020001);

        // Num Anims
        writer.Write((short)Animations.Count);

        // Skeleton
        writer.Write(this.Skeleton);

        // Offsets
        int offsetsPosition = (int)writer.BaseStream.Position; // We will calculate offsets later
        writer.Write(0); // Anim Info Offset
        writer.Write(0); // Havok Offset
        writer.Write(0); // Timeline Offset

        // Anims
        int infoOffset = (int)writer.BaseStream.Position;
        foreach (var animInfo in Animations)
        {
            animInfo.Data.Serialize(writer);
        }

        // Havok Data
        int havokOffset = (int)writer.BaseStream.Position;
        writer.Write(this.HavokData);

        // Timeline Data
        int timelineOffset = (int)writer.BaseStream.Position;
        for (int i = 0; i < Animations.Count; i++)
        {
            var timeline = Animations[i].Timeline;
            timeline.Serialize(writer);
            var requiredPadding = AlignBoundary(writer.BaseStream.Position, i, Animations.Count);
            writer.Write(new byte[requiredPadding]);
        }

        // Fix Offsets
        writer.BaseStream.Seek(offsetsPosition, SeekOrigin.Begin);
        writer.Write(infoOffset - startPos); // Anim Info Offset
        writer.Write(havokOffset - startPos); // Havok Offset
        writer.Write(timelineOffset - startPos); // Timeline Offset
    }

    public byte[] ToBytes()
    {
        using var stream = new MemoryStream();
        this.Serialize(new BinaryWriter(stream));
        return stream.ToArray();
    }

    public void ToFile(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Create);
        this.Serialize(new BinaryWriter(stream));
    }

    public static PapFormat FromFile(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Open);
        return new PapFormat(new BinaryReader(stream));
    }

    public static PapFormat FromBytes(byte[] data)
    {
        using var stream = new MemoryStream(data);
        return new PapFormat(new BinaryReader(stream));
    }

    private static int AlignBoundary(long position, int idx, int max)
    {
        if (max > 1 && idx < max - 1)
        {
            var leftOver = position % 4;
            return (int)(leftOver == 0 ? 0 : 4 - leftOver);
        }
        return 0;
    }
}
