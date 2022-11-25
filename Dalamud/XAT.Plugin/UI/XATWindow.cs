using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;

namespace XAT.Plugin.UI;

public class XATWindow : Window, IDisposable
{
    private XATPlugin Plugin { get; }

    private CutsceneTab CutsceneTab { get; }

    public XATWindow(XATPlugin plugin) : base(
        XATPlugin.PluginName, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse
    )
    {
        this.Plugin = plugin;

        this.CutsceneTab = new CutsceneTab(plugin);

        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public override void Draw()
    {
        if(ImGui.BeginTabBar("main_tabs"))
        {
            if(ImGui.BeginTabItem("Cutscene"))
            { 
                CutsceneTab.Draw();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    public void Dispose()
    {
    }
}
