using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XAT.Game.Formats.Utils;

namespace XAT.Game.Formats.Tmb;

[AddINotifyPropertyChangedInterface]
public class TmbFormat
{
    public const string MAGIC = "TMLB";

    private TmdhFormat Header;
    private TmppFormat? FaceLibrary;
    private TmalFormat ActorHeader;

    public TmbFormat(BinaryReader reader)
    {
        var startPos = reader.BaseStream.Position;

        string magic = reader.ReadEncodedString(4);
        if (magic != MAGIC)
            throw new Exception($"Invalid file - magic incorrect. Expected {MAGIC}, got {magic}.");

        int size = reader.ReadInt32();

        int numEntries = reader.ReadInt32();

        // We read every tmb item and do id resolution
        // id resolution is not complete until a full pass has been made
        TmbReadContext readContext = new(reader, this);
        Queue<TmbItemFormat> items = new();
        for (int i = 0; i < numEntries; ++i)
        {
            var startPosition = reader.BaseStream.Position;
            readContext.SubDocumentStartPosition = startPosition;
            TmbItemFormat entry = TmbItemFormat.ParseItem(readContext);
            items.Enqueue(entry);

            if (entry is TmbItemWithIdFormat withId)
                readContext.GetPointerAtId<TmbItemWithIdFormat>(withId.Id).Item= withId;

            reader.BaseStream.Seek(startPosition + entry.Size, SeekOrigin.Begin);
        }

        reader.BaseStream.Seek(startPos + size, SeekOrigin.Begin);

        // Header
        if (items.Dequeue() is TmdhFormat tmdh)
        {
            Header = tmdh;
        }
        else
        {
            throw new Exception("Expected first entry to be TMDH");
        }

        // Facial Library - optional
        if (items.Peek() is TmppFormat)
        {
            FaceLibrary = items.Dequeue() as TmppFormat;
        }

        // Actor list / header
        if (items.Dequeue() is TmalFormat tmal)
        {
            ActorHeader = tmal;
        }
        else
        {
            throw new Exception("Expected entry to be TMAL");
        }
    }
    public void Serialize(BinaryWriter writer)
    {
        var startPos = writer.BaseStream.Position;
        writer.WriteEncodedString(MAGIC);
        writer.Write((int)0);

        List<TmbItemFormat> items = new();

        short currentId = 1;
        Header.Id = currentId++;
        items.Add(Header);

        if(FaceLibrary != null)
            items.Add(FaceLibrary);

        items.Add(ActorHeader);

        // Now we crawl from the actor header finding all the actors, tracks and entries - we assign new ids as we go.
        {
            foreach (var actorPointer in ActorHeader.Actors)
            {
                actorPointer.Item.Id = currentId++;
                items.Add(actorPointer.Item);
            }

            foreach (var actorPointer in ActorHeader.Actors)
            {
                foreach (var trackPointer in actorPointer.Item.Tracks)
                {
                    if (trackPointer.Item == null || trackPointer.Item is not TmtrFormat)
                        throw new Exception("Unresolved track");


                    items.Add(trackPointer.Item);
                    trackPointer.Item.Id = currentId++;
                }
            }

            foreach (var actor in ActorHeader.Actors)
            {
                foreach (var trackPointer in actor.Item.Tracks)
                {
                    if (trackPointer.Item == null || trackPointer.Item is not TmtrFormat)
                        throw new Exception("Unresolved track");

                    foreach (var entryPointer in trackPointer.Item.Entries)
                    {
                        if (entryPointer.Item == null)
                            throw new Exception("Unresolved entry");

                        items.Add(entryPointer.Item);
                        entryPointer.Item.Id = currentId++;
                    }
                }
            }
        }

        int itemLength = items.Sum(x => x.Size);
        int extraLength = items.Sum(x => x.ExtraSize);
        int timelineLength = items.Sum(x => x.TimelineCount) * sizeof(short);
        var context = new TmbWriteContext(itemLength, extraLength, timelineLength, this);

        writer.Write(items.Count);
        foreach (var item in items)
        {
            context.SubDocumentStartPosition = context.Writer.BaseStream.Position;
            item.Serialize(context);

            var writtenSize = context.Writer.BaseStream.Position - context.SubDocumentStartPosition;
            if (writtenSize != item.Size)
                throw new Exception($"Item did not write correct amount of data: {item.Magic} expected {item.Size} but got {writtenSize}.");
        }

        // Put it all together
        writer.Write(context.Writer.ToArray());
        writer.Write(context.ExtraWriter.ToArray());
        writer.Write(context.TimelineWriter.ToArray());
        writer.Write(context.StringWriter.ToArray());

        // Fix size
        var endPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(startPos + 4, SeekOrigin.Begin);
        writer.Write((int)(endPos - startPos));
        writer.BaseStream.Seek(endPos, SeekOrigin.Begin);
    }
}