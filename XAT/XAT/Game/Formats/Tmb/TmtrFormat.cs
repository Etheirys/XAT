using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using XAT.Core;

namespace XAT.Game.Formats.Tmb;

[AddINotifyPropertyChangedInterface]
public class TmtrFormat : TmbItemWithTimeFormat
{
    public const string MAGIC = "TMTR";
    public override string Magic => MAGIC;

    public override int Size => 0x18;
    public override int ExtraSize => 0;
    public override int TimelineCount => Entries.Count;

    [UserType]
    public int Unk1 { get; set; }

    [UserType]
    public int Unk2 { get; set; }

    public ObservableCollection<TmbPointer<TmbItemWithTimeFormat>> Entries { get; set; } = new();


    public TmtrFormat(TmbReadContext context)
    {
        ReadHeader(context);

        Entries = new(context.ReadOffsetTimeline<TmbItemWithTimeFormat>());

        int unknownOffset = context.Reader.ReadInt32();
        if (unknownOffset != 0)
            throw new Exception("Not yet");
    }

    public TmtrFormat()
    {
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.WriteOffsetTimeline(Entries);

        context.Writer.Write((int)0);
    }
}