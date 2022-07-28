using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using XAT.Game.Formats.Utils;

namespace XAT.Game.Formats.Tmb;

public class TmbReadContext
{
    private Dictionary<short, TmbRawPointer> actorIds = new();

    public long StartPosition { get; init; }
    public long SubDocumentStartPosition { get; set; }
    public BinaryReader Reader { get; init; }
    public TmbFormat Tmbl { get; init; }

    public TmbReadContext(BinaryReader reader, TmbFormat tmbl)
    {
        Reader = reader;
        StartPosition = reader.BaseStream.Position;
        Tmbl = tmbl;
    }

    public TmbRawPointer GetRawPointerAtId(short id)
    {
        if (actorIds.ContainsKey(id))
            return actorIds[id];

        var newActor = new TmbRawPointer();
        actorIds[id] = newActor;
        return newActor;
    }

    public TmbPointer<T> GetPointerAtId<T>(short id) where T : TmbItemWithIdFormat
    {
        var raw = GetRawPointerAtId(id);
        return new TmbPointer<T>(raw);
    }

    public string ReadOffsetString()
    {
        int offset = Reader.ReadInt32();
        var currentPos = Reader.BaseStream.Position;
        Reader.BaseStream.Seek(SubDocumentStartPosition + 8 + offset, SeekOrigin.Begin);

        string result = Reader.ReadEncodedString();

        Reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return result;
    }

    public List<TmbPointer<T>> ReadOffsetTimeline<T>() where T : TmbItemWithIdFormat
    {
        int offset = Reader.ReadInt32();
        int count = Reader.ReadInt32();

        var currentPos = Reader.BaseStream.Position;

        Reader.BaseStream.Seek(offset + SubDocumentStartPosition + 8, SeekOrigin.Begin);

        short[] result = new short[count];
        for (var i = 0; i < count; i++)
        {
            var trackId = Reader.ReadInt16();
            result[i] = trackId;
        }

        Reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return result.Select(x => new TmbPointer<T>(GetRawPointerAtId(x))).ToList();
    }

    public Vector3 ReadOffsetVector3()
    {
        int offset = Reader.ReadInt32();
        int count = Reader.ReadInt32();

        if (count != 3)
            throw new Exception($"Vector3 should be 3 entries but was {count}.");

        var currentPos = Reader.BaseStream.Position;

        Reader.BaseStream.Seek(offset + SubDocumentStartPosition + 8, SeekOrigin.Begin);

        var result = new Vector3()
        {
            X = Reader.ReadSingle(),
            Y = Reader.ReadSingle(),
            Z = Reader.ReadSingle(),
        };

        Reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return result;
    }

    public Vector4 ReadOffsetVector4()
    {
        int offset = Reader.ReadInt32();
        int count = Reader.ReadInt32();

        if (count != 4)
            throw new Exception($"Vector4 should be 4 entries but was {count}.");

        var currentPos = Reader.BaseStream.Position;

        Reader.BaseStream.Seek(offset + SubDocumentStartPosition + 8, SeekOrigin.Begin);

        var result = new Vector4()
        {
            X = Reader.ReadSingle(),
            Y = Reader.ReadSingle(),
            Z = Reader.ReadSingle(),
            W = Reader.ReadSingle(),
        };

        Reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return result;
    }

    public void ReadAtOffset(Action<BinaryReader> func)
    {
        int offset = Reader.ReadInt32();
        var currentPos = Reader.BaseStream.Position;

        Reader.BaseStream.Seek(offset + SubDocumentStartPosition + 8, SeekOrigin.Begin);

        func(Reader);

        Reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
    }
}

public class TmbWriteContext
{
    public const int BaseSize = 0x10;
    public int BodySize { get; init; }
    public int ExtraSize { get; init; }
    public int TimelineSize { get; init; }
    public int Size => BaseSize + TimelineSize + ExtraSize + (int) StringWriter.BaseStream.Length;

    public BinaryWriter Writer { get; } = new BinaryWriter(new MemoryStream());
    public BinaryWriter ExtraWriter { get; } = new BinaryWriter(new MemoryStream());
    public BinaryWriter TimelineWriter { get; } = new BinaryWriter(new MemoryStream());
    public BinaryWriter StringWriter { get; } = new BinaryWriter(new MemoryStream());

    public TmbFormat Tmbl { get; init; }

    public long SubDocumentStartPosition { get; set; }

    private Dictionary<string, int> writtenStrings = new();

    public TmbWriteContext(int bodySize, int extraSize, int timelineSize, TmbFormat tmbl)
    {
        BodySize = bodySize;
        TimelineSize = timelineSize;
        ExtraSize = extraSize;
        Tmbl = tmbl;
    }

    public void WriteOffsetString(string str)
    {
        if(writtenStrings.ContainsKey(str))
        {
            int stringOffset = writtenStrings[str];
            int actualPos = (int)((BodySize - (SubDocumentStartPosition + 8)) + ExtraSize + TimelineSize + stringOffset);
            Writer.Write(actualPos);
        }
        else
        {
            int stringOffset = (int)StringWriter.BaseStream.Position;
            int actualPos = (int)((BodySize - (SubDocumentStartPosition + 8)) + ExtraSize + TimelineSize + stringOffset);
            Writer.Write(actualPos);

            StringWriter.WriteEncodedString(str, true);
            writtenStrings[str] = stringOffset;
        }
    }
       
    public void WriteOffsetTimeline<T>(IEnumerable<TmbPointer<T>> entries) where T :TmbItemWithIdFormat
    {
        int actualPos = (int)((BodySize - (SubDocumentStartPosition + 8)) + ExtraSize + TimelineWriter.BaseStream.Position);
        Writer.Write(actualPos);
        Writer.Write(entries.Count());

        foreach(var entry in entries)
        {
            TimelineWriter.Write(entry.Item.Id);
        }
    }

    public void WriteExtraVector4(Vector4 input)
    {
        int actualPos = (int)((BodySize - (SubDocumentStartPosition + 8)) + ExtraWriter.BaseStream.Position);
        Writer.Write(actualPos);
        Writer.Write((int) 4);
        ExtraWriter.WriteVector4(input);
    }

    public void WriteExtraVector3(Vector3 input)
    {
        int actualPos = (int)((BodySize - (SubDocumentStartPosition + 8)) + ExtraWriter.BaseStream.Position);
        Writer.Write(actualPos);
        Writer.Write((int)3);
        ExtraWriter.WriteVector3(input);
    }

    public void WriteExtra(Action<BinaryWriter> func)
    {
        int actualPos = (int)((BodySize - (SubDocumentStartPosition + 8)) + ExtraWriter.BaseStream.Position);
        Writer.Write(actualPos);

        func(ExtraWriter);
    }
}

public class TmbRawPointer
{
    public TmbItemWithIdFormat? Item { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is TmbRawPointer other)
            return other.Item == Item;

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Item?.GetHashCode());
    }
}

public class TmbPointer<T> where T : TmbItemWithIdFormat
{
    public TmbRawPointer Raw { get; set; }

    public T Item
    {
        get
        {
            if (Raw.Item == null)
                throw new System.Exception("Attempting to access unresolved tmb item.");

            return (T)Raw.Item;
        }

        set
        {
            Raw.Item = value;
        }
    }

    public TmbPointer(TmbRawPointer raw)
    {
        Raw = raw;
    }

    public TmbPointer(T raw)
    {
        Raw = new TmbRawPointer();
        Raw.Item = raw;
    }

    public override bool Equals(object? obj)
    {
        if (obj is TmbPointer<T> other)
            return other.Item == Item;

        if (obj is TmbRawPointer other2)
            return other2.Item == Raw.Item;

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Item?.GetHashCode(), Raw?.GetHashCode());
    }
}