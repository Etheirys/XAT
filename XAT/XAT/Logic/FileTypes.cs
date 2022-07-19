using System;
using System.Linq;

namespace XAT.Logic;

public enum AnimationFileType
{
    [AnimationFileType("Autodesk FBX", ".fbx")]
    FBX,

    [AnimationFileType("Havok Tag File", ".hkt")]
    HavokTagFile,

    [AnimationFileType("Havok Pack File", ".hkx")]
    HavokPackFile,

    [AnimationFileType("Havok XML Tag File", ".xml")]
    HavokXMLFile,
}

public static class AnimationFileTypeExtensions
{
    public static string GetDescription(this AnimationFileType exportType)
    {
        AnimationFileTypeAttribute[]? attributes = (AnimationFileTypeAttribute[]?)exportType.GetType()?.GetField(exportType.ToString())?.GetCustomAttributes(typeof(AnimationFileTypeAttribute), false);
        return attributes?.Length > 0 ? attributes[0].Description : string.Empty;
    }

    public static string GetExtension(this AnimationFileType exportType)
    {
        AnimationFileTypeAttribute[]? attributes = (AnimationFileTypeAttribute[]?)exportType.GetType()?.GetField(exportType.ToString())?.GetCustomAttributes(typeof(AnimationFileTypeAttribute), false);
        return attributes?.Length > 0 ? attributes[0].Extension : string.Empty;
    }

    public static AnimationFileType? GetTypeFromExtension(string? extension)
    {
        if (extension == null)
            return null;

        var list = Enum.GetValues(typeof(AnimationFileType)).Cast<AnimationFileType>();
        foreach (var entry in list)
        {
            var attribs = (AnimationFileTypeAttribute[]?)entry.GetType()?.GetField(entry.ToString())?.GetCustomAttributes(typeof(AnimationFileTypeAttribute), false);
            if (attribs != null && attribs.Length > 0 && extension == attribs[0].Extension)
            {
                return entry;
            }
        }
        return null;
    }

    public static string GetFileFormatFilters()
    {
        string result = Enum.GetValues(typeof(AnimationFileType)).Cast<AnimationFileType>().Select((i) => $"{i.GetDescription()}|*{i.GetExtension()}").Aggregate((i, x) => $"{i}|{x}");
        return result;
    }
}

public class AnimationFileTypeAttribute : Attribute
{
    public string Description { get; set; }
    public string Extension { get; set; }

    public AnimationFileTypeAttribute(string description, string extension)
    {
        this.Description = description;
        this.Extension = extension;
    }
}