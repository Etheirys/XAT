using PropertyChanged;
using System;
using System.IO;
using System.Text;
using XAT.Core;
using XAT.Game.Formats.Utils;

namespace XAT.Game.Formats.Tmb;

[AddINotifyPropertyChangedInterface]
public abstract class TmbItemFormat
{
    public abstract string Magic { get; }

    public abstract int Size { get; }
    public abstract int ExtraSize { get; }
    public abstract int TimelineCount { get;  }

    protected virtual void ReadHeader(TmbReadContext context)
    {
        string magic = context.Reader.ReadEncodedString(4);
        if (magic != Magic)
            throw new Exception($"Invalid file - magic incorrect. Expected {Magic}, got {magic}.");

        int size = context.Reader.ReadInt32();
        if (size != Size)
            throw new Exception($"Unexpected size. Expected {Size}, got {size}");
    }

    protected virtual void WriteHeader(TmbWriteContext context)
    {
        context.Writer.WriteEncodedString(Magic);
        context.Writer.Write(Size);
    }

    public abstract void Serialize(TmbWriteContext context);

    public static TmbItemFormat ParseItem(TmbReadContext context)
    {
        var startPos = context.Reader.BaseStream.Position;
        string magic = Encoding.ASCII.GetString(context.Reader.ReadBytes(4));
        context.Reader.BaseStream.Seek(startPos, SeekOrigin.Begin);

        if (!TmbUtils.ItemTypes.ContainsKey(magic))
            throw new Exception($"Unknown tmb item type: {magic}.");

        var type = TmbUtils.ItemTypes[magic];
        var ctr = type.GetConstructor(new Type[] { typeof(TmbReadContext) });

        if (ctr == null)
            throw new Exception($"Not suitable tmb item constructor for: {magic}");

        var item = ctr.Invoke(new object[] { context });

        if(item == null)
            throw new Exception($"Could not construct: {magic}");

        return (TmbItemFormat)item;
    }
}

[AddINotifyPropertyChangedInterface]
public abstract class TmbItemWithIdFormat : TmbItemFormat
{
    public short Id { get; set; }

    protected override void ReadHeader(TmbReadContext context)
    {
        base.ReadHeader(context);

        Id = context.Reader.ReadInt16();
    }

    protected override void WriteHeader(TmbWriteContext context)
    {
        base.WriteHeader(context);

        context.Writer.Write(Id);
    }
}

[AddINotifyPropertyChangedInterface]
public abstract class TmbItemWithTimeFormat : TmbItemWithIdFormat
{
    [UserType]
    public short Time { get; set; }

    protected override void ReadHeader(TmbReadContext context)
    {
        base.ReadHeader(context);

        Time = context.Reader.ReadInt16();
    }

    protected override void WriteHeader(TmbWriteContext context)
    {
        base.WriteHeader(context);

        context.Writer.Write(Time);
    }
}

