﻿using Dalamud.Interface.Colors;
using Dalamud.Interface.ImGuiFileDialog;
using ImGuiNET;
using System.IO;
using XAT.Plugin.Files;

namespace XAT.Plugin.UI;

public class CutsceneTab
{
    private XATPlugin Plugin { get; }

    private FileDialogManager FileDialogManager { get; }

    private string cameraPath = string.Empty;
    private bool closeWindowOnPlay = false;

    public CutsceneTab(XATPlugin plugin)
    {
        this.Plugin = plugin;

        this.FileDialogManager = new FileDialogManager
        {
            AddedWindowFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking,
        };
    }

    public void Draw()
    {
        ImGui.Text("Camera Path");
        ImGui.SameLine();
        ImGui.InputText(string.Empty, ref cameraPath, 260, ImGuiInputTextFlags.ReadOnly);
        ImGui.SameLine();
        if (ImGui.Button("Browse"))
        {
            
            this.FileDialogManager.OpenFileDialog("Test", "XAT Camera File {.xcp}", (success, name) =>
            {
                if (!success)
                {
                    cameraPath = string.Empty;
                    return;
                }

                cameraPath = name;
                XATCameraPathFile camPath = new(new BinaryReader(File.OpenRead(cameraPath)));
                Plugin.CutsceneManager.CameraPath = camPath;
            });
        }

        ImGui.Separator();

        if(Plugin.CutsceneManager.CameraPath == null) ImGui.BeginDisabled();

        ImGui.InputFloat3("Scale", ref Plugin.CutsceneManager.CameraSettings.Scale);
        ImGui.InputFloat3("Offset", ref Plugin.CutsceneManager.CameraSettings.Offset);
        ImGui.Checkbox("Loop", ref Plugin.CutsceneManager.CameraSettings.Loop);

        ImGui.Separator();

        ImGui.Checkbox("Close Window On Play", ref closeWindowOnPlay);

        ImGui.Separator();

        if (!Plugin.GPoseService.IsInGPose) ImGui.BeginDisabled();
        if (!Plugin.GPoseService.IsInGPose) ImGui.TextColored(ImGuiColors.DPSRed, "Must be in GPose");

        if (Plugin.CutsceneManager.IsRunning) ImGui.BeginDisabled();
        if (ImGui.Button("Play"))
        {
            Play();
        }

        ImGui.SameLine();

        if (Plugin.CutsceneManager.IsRunning) ImGui.EndDisabled();

        if (!Plugin.CutsceneManager.IsRunning) ImGui.BeginDisabled();
        if (ImGui.Button("Stop"))
        {
            Plugin.CutsceneManager.StopPlayback();
        }
        if (!Plugin.CutsceneManager.IsRunning) ImGui.EndDisabled();

        if (Plugin.CutsceneManager.CameraPath == null) ImGui.EndDisabled();

        if (!Plugin.GPoseService.IsInGPose) ImGui.EndDisabled();

        this.FileDialogManager.Draw();
    }

    private void Play()
    {
        Plugin.CutsceneManager.StartPlayback();

        if (closeWindowOnPlay)
            Plugin.Window.IsOpen = false;
    }
}
