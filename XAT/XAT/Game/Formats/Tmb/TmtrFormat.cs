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
    public override int ExtraSize => UnknownExtraEntries.Count == 0 ? 0 : 8 + (12 * UnknownExtraEntries.Count);
    public override int TimelineCount => Entries.Count;

    public ObservableCollection<TmbPointer<TmbItemWithTimeFormat>> Entries { get; set; } = new();

    public ObservableCollection<TmTrUnknownData> UnknownExtraEntries { get; set; } = new();


    public TmtrFormat(TmbReadContext context)
    {
        ReadHeader(context);

        Entries = new(context.ReadOffsetTimeline<TmbItemWithTimeFormat>());

        // Deal with unknown extras
        context.ReadAtOffset((reader) =>
        {
            _ = reader.ReadInt32(); // 8
            int count = reader.ReadInt32();

            for(int i = 0; i < count; i++)
            {
                UnknownExtraEntries.Add(new TmTrUnknownData(reader));
            }
        });
    }

    public TmtrFormat()
    {
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.WriteOffsetTimeline(Entries);

        // Deal with unknown extras
        if(UnknownExtraEntries.Count > 0)
        {
            context.WriteExtra((writer) =>
            {
                writer.Write((int)8);
                writer.Write(UnknownExtraEntries.Count);

                foreach (var entry in UnknownExtraEntries)
                {
                    entry.Serialize(writer);
                }
            });
        } 
        else
        {
            context.Writer.Write((int)0);
        }
       
    }
}