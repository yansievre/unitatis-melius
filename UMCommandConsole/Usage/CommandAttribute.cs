using System;

namespace Plugins.UMCommandConsole.Usage
{
    [AttributeUsage(
        AttributeTargets.Event|
        AttributeTargets.Field|
        AttributeTargets.Method|
        AttributeTargets.Property
    ,AllowMultiple = false,Inherited = false)]
    public class CommandAttribute : Attribute
    {
        private readonly string _commandName;
        private readonly int _requiresTarget;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="requiresTarget">Default -1. 0: false, 1: true, -1: automated</param>
        public CommandAttribute(string commandName, int requiresTarget=-1)
        {
            _commandName = commandName;
            requiresTarget = requiresTarget < -1 ? -1 : requiresTarget;
            requiresTarget = requiresTarget > 1 ? 1 : requiresTarget;
            _requiresTarget = requiresTarget;
        }

        public int RequiresTarget => _requiresTarget;

        public string CommandName => _commandName;
    }
}
