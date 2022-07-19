using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using XAT.Common.FFXIV.Files;
using XAT.Common.Havok;

namespace XAT.Logic;

public static class SkeletonInterop
{
    public static async Task<int> ExportFBX(Sklb sklb, string outputPath)
    {
        string targetPath = Path.GetTempFileName();

        await File.WriteAllBytesAsync(targetPath, sklb.HavokData);

        var result = await RawHavokInterop.ToFbxSkeleton(targetPath, 0, targetPath);

        File.Copy(targetPath, outputPath, true);
        File.Delete(targetPath);

        return result;
    }

    public static async Task ExportHavok(Sklb sklb, AnimationFileType exportType, string outputPath)
    {
        string targetPath = Path.GetTempFileName();

        await File.WriteAllBytesAsync(targetPath, sklb.HavokData);

        string extension = Path.GetExtension(outputPath);
        switch (exportType)
        {
            case AnimationFileType.HavokTagFile:
                await RawHavokInterop.ToTagFile(targetPath, targetPath);
                break;

            case AnimationFileType.HavokPackFile:
                await RawHavokInterop.ToPackFile(targetPath, targetPath);
                break;

            case AnimationFileType.HavokXMLFile:
                await RawHavokInterop.ToXMLFile(targetPath, targetPath);
                break;
            default:
                throw new Exception($"Havok file extension '{extension}' unknown.");
        }

        File.Copy(targetPath, outputPath, true);
        File.Delete(targetPath);
    }

    public static async Task<int> ImportFBX(Sklb sklb, string sourceFbx, bool preserveCompat)
    {
        string originalSkele = Path.GetTempFileName();
        string targetPath = Path.GetTempFileName();

        await File.WriteAllBytesAsync(originalSkele, sklb.HavokData);
        var originalBoneList = await RawHavokInterop.ListBones(originalSkele, 0);

        await RawHavokInterop.CreateContainer(targetPath);
        var fromResult = await RawHavokInterop.FromFbxSkeleton(targetPath, sourceFbx, preserveCompat ? originalBoneList : null, targetPath);

        sklb.HavokData = await File.ReadAllBytesAsync(targetPath);

        File.Delete(originalSkele);
        File.Delete(targetPath);

        return fromResult;
    }

    public static async Task ImportHavok(Sklb sklb, AnimationFileType exportType, string sourceFile)
    {
        string targetPath = Path.GetTempFileName();

        await File.WriteAllBytesAsync(targetPath, sklb.HavokData);

        string extension = Path.GetExtension(sourceFile);
        switch (exportType)
        {
            case AnimationFileType.HavokTagFile:
                await RawHavokInterop.ToTagFile(targetPath, targetPath);
                break;

            case AnimationFileType.HavokPackFile:
                await RawHavokInterop.ToPackFile(targetPath, targetPath);
                break;

            case AnimationFileType.HavokXMLFile:
                await RawHavokInterop.ToXMLFile(targetPath, targetPath);
                break;
            default:
                throw new Exception($"Havok file extension '{extension}' unknown.");
        }

        sklb.HavokData = await File.ReadAllBytesAsync(targetPath);

        File.Delete(targetPath);
    }
}
