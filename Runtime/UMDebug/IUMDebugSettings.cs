namespace UM.Runtime.UMDebug
{
    public enum BuildMode
    {
        Development,
        DevelopmentRelease,
        CustomerRelease
    }
    public interface IUMDebugSettings
    {
        bool IsDevelopmentMode { get; }
        bool IsDevReleaseMode { get; }
        bool IsCustomerRelease { get; }
        BuildMode BuildMode { get; }
    }
}