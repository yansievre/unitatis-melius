using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugins.UMCommandConsole.Core
{
    internal class Command
    {
        public string CommandName;
        public Type[] Parameters;
        public Type ReturnValue;
        public bool RequiresTarget;
        public Delegate CommandAction;
        

        public Command(string commandName, Type returnValue, Type[] parameters, Delegate commandAction, bool requiresTarget)
        {
            CommandName = commandName;
            ReturnValue = returnValue;
            Parameters = parameters;
            RequiresTarget = requiresTarget;
            CommandAction = commandAction;
        }
        
      
        public Command()
        {
        }
    }
}