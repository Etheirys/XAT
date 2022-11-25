namespace XAT.Plugin.Game;

public class GPoseService
{
    private XATPlugin Plugin { get; }

    public delegate void GPoseChange(bool newGPoseState);
    public event GPoseChange? OnGPoseChange = null;

    public GPoseService(XATPlugin plugin)
    {
        this.Plugin = plugin;
    }

    public bool IsInGPose { get; private set; }



    public void Update()
    {
        bool nextGPoseState = Plugin.PluginInterface.UiBuilder.GposeActive;

        if (IsInGPose != Plugin.PluginInterface.UiBuilder.GposeActive)
        {
            IsInGPose= nextGPoseState;
            OnGPoseChange?.Invoke(IsInGPose);
        }
    }
}
