namespace Plugins.UMCommandConsole.Usage
{
    /// <summary>
    /// Allows the registry of console targets so they can be found by commands
    /// </summary>
    public interface IConsoleTargetRegistry
    {
        void RegisterTarget<T>(T target) where T : IConsoleTarget;
        void UnregisterTarget<T>(T target) where T : IConsoleTarget;
    }
}
