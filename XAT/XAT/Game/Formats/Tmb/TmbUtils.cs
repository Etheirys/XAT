using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using XAT.Game.Formats.Tmb.Entries;

namespace XAT.Game.Formats.Tmb;

public static class TmbUtils
{
    private static Dictionary<string, Type> ItemTypesRaw = new()
    {
        // Main Types
        { TmdhFormat.MAGIC, typeof(TmdhFormat) },
        { TmppFormat.MAGIC, typeof(TmppFormat) },
        { TmalFormat.MAGIC, typeof(TmalFormat) },
        { TmacFormat.MAGIC, typeof(TmacFormat) },
        { TmtrFormat.MAGIC, typeof(TmtrFormat) },
        { TmfcFormat.MAGIC, typeof(TmfcFormat) },

        // Entry Types
        { C002Format.MAGIC, typeof(C002Format) },
        { C006Format.MAGIC, typeof(C006Format) },
        { C009Format.MAGIC, typeof(C009Format) },
        { C010Format.MAGIC, typeof(C010Format) },
        { C011Format.MAGIC, typeof(C011Format) },
        { C012Format.MAGIC, typeof(C012Format) },
        { C013Format.MAGIC, typeof(C013Format) },
        { C014Format.MAGIC, typeof(C014Format) },
        { C015Format.MAGIC, typeof(C015Format) },
        { C031Format.MAGIC, typeof(C031Format) },
        { C042Format.MAGIC, typeof(C042Format) },
        { C043Format.MAGIC, typeof(C043Format) },
        { C053Format.MAGIC, typeof(C053Format) },
        { C063Format.MAGIC, typeof(C063Format) },
        { C067Format.MAGIC, typeof(C067Format) },
        { C075Format.MAGIC, typeof(C075Format) },
        { C088Format.MAGIC, typeof(C088Format) },
        { C093Format.MAGIC, typeof(C093Format) },
        { C094Format.MAGIC, typeof(C094Format) },
        { C107Format.MAGIC, typeof(C107Format) },
        { C118Format.MAGIC, typeof(C118Format) },
        { C120Format.MAGIC, typeof(C120Format) },
        { C124Format.MAGIC, typeof(C124Format) },
        { C125Format.MAGIC, typeof(C125Format) },
        { C131Format.MAGIC, typeof(C131Format) },
        { C173Format.MAGIC, typeof(C173Format) },
        { C174Format.MAGIC, typeof(C174Format) },
        { C175Format.MAGIC, typeof(C175Format) },
        { C187Format.MAGIC, typeof(C187Format) },
        { C198Format.MAGIC, typeof(C198Format) },
        { C203Format.MAGIC, typeof(C203Format) },
        { C204Format.MAGIC, typeof(C204Format) },
        { C211Format.MAGIC, typeof(C211Format) },
    };

    public static ReadOnlyDictionary<string, Type> ItemTypes = new(ItemTypesRaw);

    public static ReadOnlyDictionary<string, Type> EntryTypes = new(ItemTypesRaw.Where(x => x.Value.IsAssignableTo(typeof(TmbEntry))).ToDictionary((x) => x.Key, y => y.Value));
}
