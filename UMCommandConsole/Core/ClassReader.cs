using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Plugins.UMCommandConsole.Usage;

namespace Plugins.UMCommandConsole.Core
{
    internal enum CommandTarget
    {
        InstanceMethod,
        StaticMethod,
        Field,
        Property
    }
    internal class FailedCommand
    {
        public readonly CommandTarget Target;
        public readonly string CommandName;

        public FailedCommand(CommandTarget target, string commandName)
        {
            Target = target;
            CommandName = commandName;
        }
    }
    
    internal class ClassReader
    {

        public Command[] ExtractCommands(Type targetType, out FailedCommand[] failedCommands)
        {
            List<Command> generatedCommands = new List<Command>();
            List<FailedCommand> failedCmds = new List<FailedCommand>();
            var staticMethods = targetType.GetMethods(BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(x => x.HasAttribute<CommandAttribute>());
            var instanceMethods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.HasAttribute<CommandAttribute>());
            var fields = targetType.GetFields(BindingFlags.Instance).Where(x => x.HasAttribute<CommandAttribute>());
            var properties = targetType.GetProperties(BindingFlags.Instance).Where(x => x.HasAttribute<CommandAttribute>());

            foreach (var methodInfo in staticMethods)
            {
                if (GenerateCommandFromStatic(methodInfo, out var command, out var failed))
                {
                    generatedCommands.Add(command);
                }
                else
                {
                    failedCmds.Add(failed);
                }
            }

            failedCommands = failedCmds.ToArray();
            return generatedCommands.ToArray();
        }

        internal static bool GenerateCommandFromStatic(MethodInfo methodInfo, out Command command,
            out FailedCommand failedCommand)
        {
            var commandAttribute = methodInfo.GetCustomAttribute<CommandAttribute>();
            command = new Command();
            failedCommand = new FailedCommand(CommandTarget.StaticMethod,commandAttribute.CommandName);
            if (!methodInfo.IsStatic) return false;
            if (methodInfo.IsGenericMethod) return false;
            if (methodInfo.IsConstructor) return false;
            if (methodInfo.IsVirtual) return false;
            if (methodInfo.ContainsGenericParameters) return false;
            command.RequiresTarget = false;
            command.CommandName = commandAttribute.CommandName;
            var parameters = methodInfo.GetParameters();
            foreach (var parameterInfo in parameters)
            {
                if (parameterInfo.IsIn || parameterInfo.IsLcid || parameterInfo.IsOptional || parameterInfo.IsOut ||
                    parameterInfo.IsRetval) return false;
            }
            command.Parameters = parameters.Select(x => x.ParameterType).ToArray();
            command.ReturnValue = methodInfo.ReturnType;
            if(command.ReturnValue!=null)
                command.CommandAction = methodInfo.CreateDelegate(Expression.GetFuncType(
                command.Parameters.Concat(new[] { methodInfo.ReturnType })
                .ToArray()));
            else
                command.CommandAction = methodInfo.CreateDelegate(Expression.GetActionType(
                    command.Parameters));

            return true;
        }
    }
}