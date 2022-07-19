using XAT.Common.FFXIV.Files;
using XAT.Common.Havok;

namespace XAT.Common.Interop;

public static class AnimationInterop
{
    public static async Task<(int bonesConverted, int framesConverted)> ExportFBX(Pap pap, PapAnimInfo anim, Sklb sklb, string outputPath)
    {
        string targetPath = Path.GetTempFileName();

        await CreateSingle(pap, anim, sklb, true, targetPath);

        var result = await RawHavokInterop.ToFbxAnimation(targetPath, 0, 0, targetPath);

        File.Copy(targetPath, outputPath, true);
        File.Delete(targetPath);

        return result;
    }

    public static async Task ExportHavok(ContainerFileType exportType, Pap pap, PapAnimInfo anim, Sklb sklb, bool bundleSkeleton, string outputPath)
    {
        string targetPath = Path.GetTempFileName();

        await CreateSingle(pap, anim, sklb, bundleSkeleton, targetPath);

        string extension = Path.GetExtension(outputPath);
        switch(exportType)
        {
            case ContainerFileType.HavokTagFile:
                await RawHavokInterop.ToTagFile(targetPath, targetPath);
                break;

            case ContainerFileType.HavokPackFile:
                await RawHavokInterop.ToPackFile(targetPath, targetPath);
                break;

            case ContainerFileType.HavokXMLFile:
                await RawHavokInterop.ToXMLFile(targetPath, targetPath);
                break;
            default:
                throw new Exception($"Havok file extension '{extension}' unknown.");
        }

        File.Copy(targetPath, outputPath, true);
        File.Delete(targetPath);
    }

    public static async Task<(int framesConverted, int bonesBound)> ImportFBX(Pap pap, PapAnimInfo anim, Sklb sklb, string sourceFbx, int animStackIdx, List<string> excludedBones)
    {
        string targetPath = Path.GetTempFileName();

        await File.WriteAllBytesAsync(targetPath, sklb.HavokData);

        var fromResult = await RawHavokInterop.FromFbxAnimation(targetPath, sourceFbx, animStackIdx, targetPath, 0, excludedBones, targetPath);

        string animPath = Path.GetTempFileName();
        int havokIndex = anim.HavokIndex;
        await File.WriteAllBytesAsync(animPath, pap.HavokData);

        await RawHavokInterop.ReplaceAnimation(animPath, havokIndex, targetPath, 0, targetPath);

        pap.HavokData = await File.ReadAllBytesAsync(targetPath);

        File.Delete(targetPath);
        File.Delete(animPath);

        return (fromResult.framesConverted, fromResult.bonesBound);
    }

    public static async Task ImportHavok(Pap pap, PapAnimInfo anim, Sklb sklb, string sourceHavok, int sourceHavokIndex)
    {
        string targetPath = Path.GetTempFileName();
        int havokIndex = anim.HavokIndex;
        await File.WriteAllBytesAsync(targetPath, pap.HavokData);

        await RawHavokInterop.ReplaceAnimation(targetPath, havokIndex, sourceHavok, sourceHavokIndex, targetPath);

        pap.HavokData = await File.ReadAllBytesAsync(targetPath);

        File.Delete(targetPath);
    }

    public static async Task QuantizedCompress(Pap pap, PapAnimInfo anim, Sklb sklb, float floatingTolerance, float translationTolerance, float rotationTolerance, float scaleTolerance)
    {
        string targetPath = Path.GetTempFileName();

        await CreateSingle(pap, anim, sklb, true, targetPath);

        await RawHavokInterop.CompressQuantized(targetPath, anim.HavokIndex, 0, floatingTolerance, translationTolerance, rotationTolerance, scaleTolerance, targetPath);

        string tmpNewTarget = Path.GetTempFileName();
        await File.WriteAllBytesAsync(tmpNewTarget, pap.HavokData);
        await RawHavokInterop.ReplaceAnimation(tmpNewTarget, anim.HavokIndex, targetPath, 0, targetPath);
        pap.HavokData = await File.ReadAllBytesAsync(targetPath);

        File.Delete(targetPath);
        File.Delete(tmpNewTarget);
    }

    public static async Task PredictiveCompress(Pap pap, PapAnimInfo anim, Sklb sklb, float staticFloatingTolerance, float staticTranslationTolerance, float staticRotationTolerance, float staticScaleTolerance, float dynamicFloatingTolerance, float dynamicTranslationTolerance, float dynamicRotationTolerance, float dynamicScaleTolerance)
    {
        string targetPath = Path.GetTempFileName();

        await CreateSingle(pap, anim, sklb, true, targetPath);

        await RawHavokInterop.CompressPredictive(targetPath, 0, 0, staticFloatingTolerance, staticTranslationTolerance, staticRotationTolerance, staticScaleTolerance, dynamicFloatingTolerance, dynamicTranslationTolerance, dynamicRotationTolerance, dynamicScaleTolerance, targetPath);

        string tmpNewTarget = Path.GetTempFileName();
        await File.WriteAllBytesAsync(tmpNewTarget, pap.HavokData);
        await RawHavokInterop.ReplaceAnimation(tmpNewTarget, anim.HavokIndex, targetPath, 0, tmpNewTarget);
        pap.HavokData = await File.ReadAllBytesAsync(tmpNewTarget);

        File.Delete(targetPath);
        File.Delete(tmpNewTarget);
    }

    private static async Task CreateSingle(Pap pap, PapAnimInfo anim, Sklb sklb, bool includeSkeleton, string targetPath)
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
        int havokIndex = anim.HavokIndex;
        await File.WriteAllBytesAsync(animPath, pap.HavokData);
        await RawHavokInterop.AddAnimation(targetPath, animPath, havokIndex, targetPath);
        File.Delete(animPath);      
    }
}