using System;
using System.Linq;

namespace XAT.Common.Interop;

public enum ContainerFileType
{
    [ContainerFileType("Autodesk FBX", ".fbx")]
    FBX,

    [ContainerFileType("Havok Tag File", ".hkt")]
    HavokTagFile,

    [ContainerFileType("Havok Pack File", ".hkx")]
    HavokPackFile,

    [ContainerFileType("Havok XML Tag File", ".xml")]
    HavokXMLFile,
}

public static class ContainerFIleTypeExtensions
{
    public static string GetDescription(this ContainerFileType exportType)
    {
        ContainerFileTypeAttribute[]? attributes = (ContainerFileTypeAttribute[]?)exportType.GetType()?.GetField(exportType.ToString())?.GetCustomAttributes(typeof(ContainerFileTypeAttribute), false);
        return attributes?.Length > 0 ? attributes[0].Description : string.Empty;
    }

    public static string GetExtension(this ContainerFileType exportType)
    {
        ContainerFileTypeAttribute[]? attributes = (ContainerFileTypeAttribute[]?)exportType.GetType()?.GetField(exportType.ToString())?.GetCustomAttributes(typeof(ContainerFileTypeAttribute), false);
        return attributes?.Length > 0 ? attributes[0].Extension : string.Empty;
    }

    public static ContainerFileType? GetTypeFromExtension(string? extension)
    {
        if (extension == null)
            return null;

        var list = Enum.GetValues(typeof(ContainerFileType)).Cast<ContainerFileType>();
        foreach (var entry in list)
        {
            var attribs = (ContainerFileTypeAttribute[]?)entry.GetType()?.GetField(entry.ToString())?.GetCustomAttributes(typeof(ContainerFileTypeAttribute), false);
            if (attribs != null && attribs.Length > 0 && extension == attribs[0].Extension)
            {
                return entry;
            }
        }
        return null;
    }

    public static string GetFileFormatFilters()
    {
        string result = Enum.GetValues(typeof(ContainerFileType)).Cast<ContainerFileType>().Select((i) => $"{i.GetDescription()}|*{i.GetExtension()}").Aggregate((i, x) => $"{i}|{x}");
        return result;
    }
}

public class ContainerFileTypeAttribute : Attribute
{
    public string Description { get; set; }
    public string Extension { get; set; }

    public ContainerFileTypeAttribute(string description, string extension)
    {
        this.Description = description;
        this.Extension = extension;
    }
}