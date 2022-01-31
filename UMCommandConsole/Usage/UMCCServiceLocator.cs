// ReSharper disable InconsistentNaming
namespace Plugins.UMCommandConsole.Usage
{
    public static class UMCCServiceLocator
    {
        internal static IConsoleTargetRegistry _consoleTargetRegistry;
        public static IConsoleTargetRegistry ConsoleTargetRegistry => _consoleTargetRegistry;
    }
}