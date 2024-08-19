using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace XAT.Plugin.UI;

public class XATWindow : Window
{
    private XATPlugin _plugin { get; }

    public XATWindow(XATPlugin plugin) : base($" {XATPlugin.PluginName} - Attention!", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        _plugin = plugin;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(520, 360),
            MaximumSize = new Vector2(520, 360)
        };
    }

    public override void Draw()
    {
        var segmentSize = ImGui.GetWindowSize().X / 4.5f;
        var buttonSize = new Vector2(segmentSize, ImGui.GetTextLineHeight() * 1.8f);

        using (var textGroup = ImRaii.Group())
        {
            if (textGroup.Success)
            {
                var text = $"""

                    The XAT Plugin has been merged into Brio 0.4.0!

                    Once you have installed Brio you can find the XAT Cutscene Control
                    under the `Advanced Animation Control` Window, found by clicking
                    the 'point up-right' button on the Animation Control Tab!

                    For additional help, you can view the help page,
                    you may DM me on Twitter/Discord or, join the Discord.

                    After you have installed Brio, please uninstall the XAT plugin!

                    Thank You!



                    """;

                ImGui.PushTextWrapPos(segmentSize * 4);
                ImGui.TextWrapped(text);
                ImGui.PopTextWrapPos();
            }
        }

        using var buttonGroup = ImRaii.Group();
        if (buttonGroup.Success)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(255, 0, 0, 255) / 255);
            if (ImGui.Button("Get Brio", buttonSize))
                Process.Start(new ProcessStartInfo { FileName = "https://github.com/Etheirys/Brio/?tab=readme-ov-file#installation", UseShellExecute = true });
            ImGui.PopStyleColor();
            ImGui.SameLine();

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(86, 98, 246, 255) / 255);
            if (ImGui.Button("Join the Discord", buttonSize))
                Process.Start(new ProcessStartInfo { FileName = "https://discord.gg/KvGJCCnG8t", UseShellExecute = true });
            ImGui.PopStyleColor();
            ImGui.SameLine();

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0, 100, 255, 255) / 255);
            if (ImGui.Button("View my Twitter", buttonSize))
                Process.Start(new ProcessStartInfo { FileName = "https://twitter.com/MiniatureMoosey", UseShellExecute = true });
            ImGui.PopStyleColor();
            ImGui.SameLine();

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(110, 84, 148, 255) / 255);
            if (ImGui.Button("View Help page", buttonSize))
                Process.Start(new ProcessStartInfo { FileName = "https://etheirys-tools.gitbook.io/brio", UseShellExecute = true });
            ImGui.PopStyleColor();
            ImGui.SameLine();
        }
    }
}
