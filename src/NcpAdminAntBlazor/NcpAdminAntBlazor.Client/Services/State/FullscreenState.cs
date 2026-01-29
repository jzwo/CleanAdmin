namespace NcpAdminAntBlazor.Client.Services.State;

public class FullscreenState
{
    public event Action? OnChanged;

    public bool IsFullscreen
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            OnChanged?.Invoke();
        }
    }
}