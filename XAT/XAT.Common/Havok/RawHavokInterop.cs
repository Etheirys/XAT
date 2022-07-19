using Serilog;
using System.Diagnostics;
using System.Text;

namespace XAT.Common.Havok;

public class RawHavokInterop
{
    private const string ItemDelim = "/";

    public static async Task<(int skeletonCount, int animCount, int bindingCount)> GetStats(string containerPath)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"getStats \"{containerPath}\"");
        var split = rawResult.Split(ItemDelim).Select(i => int.Parse(i)).ToArray();
        return (split[0], split[1], split[2]);
    }

    public static async Task CreateContainer(string containerPath)
    {
        await RawHavokInterop.ExecuteCommand($"createContainer \"{containerPath}\"");
    }

    public static async Task<(int animIdx, int bindingIdx)> AddAnimation(string targetContainer, string sourceContainer, int sourceAnimIdx, string outputContainer)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"addAnimation \"{targetContainer}\" \"{sourceContainer}\" {sourceAnimIdx} \"{outputContainer}\"");
        var split = rawResult.Split(ItemDelim).Select(i => int.Parse(i)).ToArray();
        return (split[0], split[1]);
    }

    public static async Task<(int animIdx, int bindingIdx)> ReplaceAnimation(string targetContainer, int targetAnimIdx, string sourceContainer, int sourceAnimIdx, string outputContainer)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"replaceAnimation \"{targetContainer}\" {targetAnimIdx} \"{sourceContainer}\" {sourceAnimIdx}  \"{outputContainer}\"");
        var split = rawResult.Split(ItemDelim).Select(i => int.Parse(i)).ToArray();
        return (split[0], split[1]);
    }

    public static async Task<(int newAnimCount, int newBindingCount)> RemoveAnimation(string targetContainer, int animIdx, string outputContainer)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"removeAnimation \"{targetContainer}\" {animIdx} \"{outputContainer}\"");
        var split = rawResult.Split(ItemDelim).Select(i => int.Parse(i)).ToArray();
        return (split[0], split[1]);
    }

    public static async Task<int> AddSkeleton(string targetContainer, string sourceContainer, int sourceSkeletonIdx, string outputContainer)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"addSkeleton \"{targetContainer}\" \"{sourceContainer}\" {sourceSkeletonIdx} \"{outputContainer}\"");
        return int.Parse(rawResult);
    }

    public static async Task<int> RemoveSkeleton(string targetContainer, int skeleIdx, string outputContainer)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"removeSkeleton \"{targetContainer}\" {skeleIdx} \"{outputContainer}\"");
        return int.Parse(rawResult);
    }

    public static async Task<List<string>> ListBones(string container, int skeletonIdx)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"listBones \"{container}\" {skeletonIdx}");

        if (string.IsNullOrEmpty(rawResult.Trim()))
            return new();

        return rawResult.Split(ItemDelim).ToList();
    }

    public static async Task ToPackFile(string sourceContainer, string outputContainer)
    {
        await RawHavokInterop.ExecuteCommand($"toPackFile \"{sourceContainer}\" \"{outputContainer}\"");
    }

    public static async Task ToTagFile(string sourceContainer, string outputContainer)
    {
        await RawHavokInterop.ExecuteCommand($"toTagFile \"{sourceContainer}\" \"{outputContainer}\"");
    }

    public static async Task ToXMLFile(string sourceContainer, string outputContainer)
    {
        await RawHavokInterop.ExecuteCommand($"toXMLFile \"{sourceContainer}\" \"{outputContainer}\"");
    }

    public static async Task CompressQuantized(string sourceContainer, int animIdx, int skeletonIdx, float floatingTolerance, float translationTolerance, float rotationTolerance, float scaleTolerance, string outputContainer)
    {
        await RawHavokInterop.ExecuteCommand($"compress quantized \"{sourceContainer}\" {animIdx} {skeletonIdx} {floatingTolerance} {translationTolerance} {rotationTolerance} {scaleTolerance} \"{outputContainer}\"");
    }

    public static async Task CompressPredictive(string sourceContainer, int animIdx, int skeletonIdx, float staticFloatingTolerance, float staticTranslationTolerance, float staticRotationTolerance, float staticScaleTolerance, float dynamicFloatingTolerance, float dynamicTranslationTolerance, float dynamicRotationTolerance, float dynamicScaleTolerance, string outputContainer)
    {
        await RawHavokInterop.ExecuteCommand($"compress predictive \"{sourceContainer}\" {animIdx} {skeletonIdx} {staticFloatingTolerance} {staticTranslationTolerance} {staticRotationTolerance} {staticScaleTolerance} {dynamicFloatingTolerance} {dynamicTranslationTolerance} {dynamicRotationTolerance} {dynamicScaleTolerance} \"{outputContainer}\"");
    }

    public static async Task<int> ToFbxSkeleton(string container, int skeletonIdx, string outputPath)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"toFbxSkeleton \"{container}\" {skeletonIdx} \"{outputPath}\"");
        return int.Parse(rawResult);
    }

    public static async Task<(int bonesConverted, int framesConverted)> ToFbxAnimation(string container, int skeletonIdx, int animIdx, string outputPath)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"toFbxAnimation \"{container}\" {skeletonIdx} {animIdx} \"{outputPath}\"");
        var split = rawResult.Split(ItemDelim).Select(i => int.Parse(i)).ToArray();
        return (split[0], split[1]);
    }

    public static async Task<List<string>> ListFbxStacks(string fbxPath)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"listFbxStacks \"{fbxPath}\"");

        if (string.IsNullOrEmpty(rawResult.Trim()))
            return new();

        return rawResult.Split(ItemDelim).ToList();
    }

    public static async Task<int> FromFbxSkeleton(string targetContainer, string fbxPath, List<string>? boneOrder, string outputPath)
    {
        string orderedBoneList = "";
        if (boneOrder?.Count > 0)
            orderedBoneList = boneOrder.Aggregate((i, x) => $"{i},{x}");

        string rawResult = await RawHavokInterop.ExecuteCommand($"fromFbxSkeleton \"{targetContainer}\" \"{fbxPath}\" \"{orderedBoneList}\" \"{outputPath}\"");
        return int.Parse(rawResult);
    }

    public static async Task<(int framesConverted, int bonesBound)> FromFbxAnimation(string targetContainer, string fbxPath, int animStackIdx, string sourceSkeleton, int skeletonId, List<string>? excludeBones, string outputPath)
    {
        string excludedBonesList = "";
        if (excludeBones?.Count > 0)
            excludedBonesList = excludeBones.Aggregate((i, x) => $"{i},{x}");

        string rawResult = await RawHavokInterop.ExecuteCommand($"fromFbxAnimation \"{targetContainer}\" \"{fbxPath}\" {animStackIdx} \"{sourceSkeleton}\" {skeletonId} \"{excludedBonesList}\" \"{outputPath}\"");
        var split = rawResult.Split(ItemDelim).Select(i => int.Parse(i)).ToArray();
        return (split[0], split[1]);
    }

    public static async Task<List<string>> ListFbxBones(string fbxFile)
    {
        string rawResult = await RawHavokInterop.ExecuteCommand($"listFbxBones \"{fbxFile}\"");
        if (string.IsNullOrEmpty(rawResult.Trim()))
            return new();

        return rawResult.Split(ItemDelim).ToList();
    }

    private async static Task<string> ExecuteCommand(string command)
    {
        string exePath = Path.Combine(AppContext.BaseDirectory, "XATHavokInterop.exe");

        Log.Debug($"Executing havok command: {command}");

        Process proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        proc.Start();

        // Regular logs come on stdout
        var stdOutResult = Task.Run(() =>
        {
            StringBuilder sb = new();
            try
            {
                if (proc.StandardOutput != null)
                {
                    while (!proc.StandardOutput!.EndOfStream)
                    {
                        string? line = proc.StandardOutput?.ReadLine();
                        if (line != null)
                        {
                            sb.AppendLine(line);
                            Log.Information(line);
                        }
                    }
                }
            }
            catch
            {
                // Who cares
            }

            try
            {
                string? final = proc.StandardOutput?.ReadToEnd();
                sb.Append(final);
            }
            catch
            {
                
            }

            return sb.ToString();
        });

        // Actual data will be on stderr
        var stdErrResult = Task.Run(() =>
        {
            StringBuilder sb = new();
            try
            {
                if (proc.StandardError != null)
                {
                    while (!proc.StandardError!.EndOfStream)
                    {
                        int? output = proc.StandardError?.Read();
                        if (output != null && output != -1)
                        {
                            sb.Append((char)output);
                        }
                    }
                }
            }
            catch
            {
                // Who cares
            }

            return sb.ToString();
        });

        await Task.WhenAll(proc.WaitForExitAsync(), stdErrResult, stdOutResult);

        if (proc.ExitCode != 0)
            throw new Exception("Execution failed: " + stdOutResult.Result);

        return stdErrResult.Result;
    }
}
