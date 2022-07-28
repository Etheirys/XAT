using System;
using System.Collections.Generic;
using System.Linq;

namespace XAT.Game.Interop;

public enum RawAnimationFileType
{
    [RawAnimationFileType("Autodesk FBX", ".fbx")]
    FBX,

    [RawAnimationFileType("Havok Tag File", ".hkt")]
    HavokTagFile,

    [RawAnimationFileType("Havok Pack File", ".hkx")]
    HavokPackFile,

    [RawAnimationFileType("Havok XML Tag File", ".xml")]
    HavokXMLFile,
}

public static class RawAnimationFileTypeExtensions
{
    public static string GetDescription(this RawAnimationFileType exportType)
    {
        RawAnimationFileTypeAttribute[]? attributes = (RawAnimationFileTypeAttribute[]?)exportType.GetType()?.GetField(exportType.ToString())?.GetCustomAttributes(typeof(RawAnimationFileTypeAttribute), false);
        return attributes?.Length > 0 ? attributes[0].Description : string.Empty;
    }

    public static string GetExtension(this RawAnimationFileType exportType)
    {
        RawAnimationFileTypeAttribute[]? attributes = (RawAnimationFileTypeAttribute[]?)exportType.GetType()?.GetField(exportType.ToString())?.GetCustomAttributes(typeof(RawAnimationFileTypeAttribute), false);
        return attributes?.Length > 0 ? attributes[0].Extension : string.Empty;
    }

    public static string GetFileFilter(this RawAnimationFileType exportType)
    {
        RawAnimationFileTypeAttribute[]? attributes = (RawAnimationFileTypeAttribute[]?)exportType.GetType()?.GetField(exportType.ToString())?.GetCustomAttributes(typeof(RawAnimationFileTypeAttribute), false);
        return attributes?.Length > 0 ? attributes[0].FileFilter : string.Empty;
    }

    public static RawAnimationFileType? GetTypeFromExtension(string? extension)
    {
        if (extension == null)
            return null;

        var list = Enum.GetValues(typeof(RawAnimationFileType)).Cast<RawAnimationFileType>();
        foreach (var entry in list)
        {
            var attribs = (RawAnimationFileTypeAttribute[]?)entry.GetType()?.GetField(entry.ToString())?.GetCustomAttributes(typeof(RawAnimationFileTypeAttribute), false);
            if (attribs != null && attribs.Length > 0 && extension == attribs[0].Extension)
            {
                return entry;
            }
        }
        return null;
    }

    public static string GetFileFormatFilters(bool includeAll)
    {
        string allExtensions = Enum.GetValues(typeof(RawAnimationFileType)).Cast<RawAnimationFileType>().Select((i) => $"*{i.GetExtension()}").Aggregate((i, x) => $"{i};{x}");
        string result = Enum.GetValues(typeof(RawAnimationFileType)).Cast<RawAnimationFileType>().Select((i) => i.GetFileFilter()).Aggregate((i, x) => $"{i}|{x}");

        if (includeAll)
            result = $"All|{allExtensions}|{result}";

        return result;
    }

    public static RawAnimationFileTypeAttribute[] GetFileFormatAttributes()
    {
        List<RawAnimationFileTypeAttribute> results = new();

        var list = Enum.GetValues(typeof(RawAnimationFileType)).Cast<RawAnimationFileType>();
        foreach (var entry in list)
        {
            var attribs = (RawAnimationFileTypeAttribute[]?)entry.GetType()?.GetField(entry.ToString())?.GetCustomAttributes(typeof(RawAnimationFileTypeAttribute), false);
            if (attribs != null && attribs.Length > 0)
            {
                results.Add(attribs[0]);
            }
        }

        return results.ToArray();
    }
}

public class RawAnimationFileTypeAttribute : Attribute
{
    public string Description { get; set; }
    public string Extension { get; set; }

    public string FriendlyName => $"{Description} ({Extension})";
    public string FileFilter => $"{Description}|*{Extension}";

    public RawAnimationFileTypeAttribute(string description, string extension)
    {
        this.Description = description;
        this.Extension = extension;
    }
}