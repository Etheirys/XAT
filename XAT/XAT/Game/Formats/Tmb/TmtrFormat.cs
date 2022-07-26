using PropertyChanged;
using System;
using System.Collections.Generic;

namespace XAT.Game.Formats.Tmb;

[AddINotifyPropertyChangedInterface]
public class TmtrFormat : TmbItemWithTimeFormat
{
    public const string MAGIC = "TMTR";
    public override string Magic => MAGIC;

    public override int Size => 0x18;
    public override int ExtraSize => 0;
    public override int TimelineCount => Entries.Count;


    public int Unk1 { get; set; }
    public int Unk2 { get; set; }

    public List<TmbPointer<TmbItemWithTimeFormat>> Entries = new();


    public TmtrFormat(TmbReadContext context)
    {
        var startPos = context.Reader.BaseStream.Position;

        ReadHeader(context);

        Entries = context.ReadOffsetTimeline<TmbItemWithTimeFormat>();

        int unknownOffset = context.Reader.ReadInt32();
        if (unknownOffset != 0)
            throw new Exception("Not yet");
    }


    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.WriteOffsetTimeline(Entries);

        context.Writer.Write((int)0);
    }
}