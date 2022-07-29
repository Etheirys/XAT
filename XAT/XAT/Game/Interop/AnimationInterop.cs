using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using XAT.Game.Formats.Pap;
using XAT.Game.Formats.Sklb;
using XAT.Game.Havok;

namespace XAT.Game.Interop;

public static class AnimationInterop
{
    public static async Task<(int bonesConverted, int framesConverted)> ExportFBX(PapAnimation anim, SklbFormat sklb, string outputPath)
    {
        string targetPath = Path.GetTempFileName();

        await CreateSingle(anim, sklb, true, targetPath);

        var result = await RawHavokInterop.ToFbxAnimation(targetPath, 0, 0, targetPath);

        File.Copy(targetPath, outputPath, true);
        File.Delete(targetPath);

        return result;
    }

    public static async Task ExportHavok(RawAnimationFileType exportType, PapAnimation anim, SklbFormat sklb, bool bundleSkeleton, string outputPath)
    {
        string targetPath = Path.GetTempFileName();

        await CreateSingle(anim, sklb, bundleSkeleton, targetPath);

        string extension = Path.GetExtension(outputPath);
        switch(exportType)
        {
            case RawAnimationFileType.HavokTagFile:
                await RawHavokInterop.ToTagFile(targetPath, targetPath);
                break;

            case RawAnimationFileType.HavokPackFile:
                await RawHavokInterop.ToPackFile(targetPath, targetPath);
                break;

            case RawAnimationFileType.HavokXMLFile:
                await RawHavokInterop.ToXMLFile(targetPath, targetPath);
                break;
            default:
                throw new Exception($"Havok file extension '{extension}' unknown.");
        }

        File.Copy(targetPath, outputPath, true);
        File.Delete(targetPath);
    }

    public static async Task<(int framesConverted, int bonesBound)> ImportFBX(PapAnimation anim, SklbFormat sklb, string sourceFbx, int animStackIdx, List<string> excludedBones)
    {
        string targetPath = Path.GetTempFileName();

        await File.WriteAllBytesAsync(targetPath, sklb.HavokData);

        var fromResult = await RawHavokInterop.FromFbxAnimation(targetPath, sourceFbx, animStackIdx, targetPath, 0, excludedBones, targetPath);

        string animPath = Path.GetTempFileName();
        int havokIndex = anim.Data.HavokIndex;
        await File.WriteAllBytesAsync(animPath, anim.Container.HavokData);

        await RawHavokInterop.ReplaceAnimation(animPath, havokIndex, targetPath, 0, targetPath);

        anim.Container.HavokData = await File.ReadAllBytesAsync(targetPath);

        File.Delete(targetPath);
        File.Delete(animPath);

        return (fromResult.framesConverted, fromResult.bonesBound);
    }

    public static async Task ImportHavok(PapAnimation anim, SklbFormat sklb, string sourceHavok, int sourceHavokIndex)
    {
        string targetPath = Path.GetTempFileName();
        int havokIndex = anim.Data.HavokIndex;
        await File.WriteAllBytesAsync(targetPath, anim.Container.HavokData);

        await RawHavokInterop.ReplaceAnimation(targetPath, havokIndex, sourceHavok, sourceHavokIndex, targetPath);

        anim.Container.HavokData = await File.ReadAllBytesAsync(targetPath);

        File.Delete(targetPath);
    }

    public static async Task QuantizedCompress(PapAnimation anim, SklbFormat sklb, float floatingTolerance, float translationTolerance, float rotationTolerance, float scaleTolerance)
    {
        string targetPath = Path.GetTempFileName();

        await CreateSingle(anim, sklb, true, targetPath);

        await RawHavokInterop.CompressQuantized(targetPath, anim.Data.HavokIndex, 0, floatingTolerance, translationTolerance, rotationTolerance, scaleTolerance, targetPath);

        string tmpNewTarget = Path.GetTempFileName();
        await File.WriteAllBytesAsync(tmpNewTarget, anim.Container.HavokData);
        await RawHavokInterop.ReplaceAnimation(tmpNewTarget, anim.Data.HavokIndex, targetPath, 0, targetPath);
        anim.Container.HavokData = await File.ReadAllBytesAsync(targetPath);

        File.Delete(targetPath);
        File.Delete(tmpNewTarget);
    }

    public static async Task PredictiveCompress(PapAnimation anim, SklbFormat sklb, float staticFloatingTolerance, float staticTranslationTolerance, float staticRotationTolerance, float staticScaleTolerance, float dynamicFloatingTolerance, float dynamicTranslationTolerance, float dynamicRotationTolerance, float dynamicScaleTolerance)
    {
        string targetPath = Path.GetTempFileName();

        await CreateSingle(anim, sklb, true, targetPath);

        await RawHavokInterop.CompressPredictive(targetPath, 0, 0, staticFloatingTolerance, staticTranslationTolerance, staticRotationTolerance, staticScaleTolerance, dynamicFloatingTolerance, dynamicTranslationTolerance, dynamicRotationTolerance, dynamicScaleTolerance, targetPath);

        string tmpNewTarget = Path.GetTempFileName();
        await File.WriteAllBytesAsync(tmpNewTarget, anim.Container.HavokData);
        await RawHavokInterop.ReplaceAnimation(tmpNewTarget, anim.Data.HavokIndex, targetPath, 0, tmpNewTarget);
        anim.Container.HavokData = await File.ReadAllBytesAsync(tmpNewTarget);

        File.Delete(targetPath);
        File.Delete(tmpNewTarget);
    }

    private static async Task CreateSingle(PapAnimation anim, SklbFormat sklb, bool includeSkeleton, string targetPath)
    {
        await RawHavokInterop.CreateContainer(targetPath);

        if (includeSkeleton)
        {
            string skelePath = Path.GetTempFileName();
            await File.WriteAllBytesAsync(skelePath, sklb.HavokData);
            await RawHavokInterop.AddSkeleton(targetPath, skelePath, 0, targetPath);
            File.Delete(skelePath);
        }

        string animPath = Path.GetTempFileName();
        int havokIndex = anim.Data.HavokIndex;
        await File.WriteAllBytesAsync(animPath, anim.Container.HavokData);
        await RawHavokInterop.AddAnimation(targetPath, animPath, havokIndex, targetPath);
        File.Delete(animPath);      
    }
}