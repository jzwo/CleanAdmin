namespace NcpAdminAntBlazor.Client.Components;

public interface IPTable
{
    public bool IsLoading { get; }
    public void ReloadData();
}