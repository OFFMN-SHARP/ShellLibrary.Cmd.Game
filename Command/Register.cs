using ShellLibrary.Cmd.Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShellLibrary.Cmd.Game.Command
{
    public class Register
    {
        public class CommandRegisterInfo
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Usage { get; set; }
            public string? CommandHelp { get; set; }
            public Func<string?,string[], Task<string>>? CommandAction { get; set; }
        }
        public class CommandInfoMake
        {
            public void MakeCommandInfo(string name, string description, string usage, string commandHelp, Func<string?,string[], Task<string>> commandAction)
            {
                CommandRegisterInfo info = new CommandRegisterInfo {
                    Name = name,
                    Description = description,
                    Usage = usage,
                    CommandHelp = commandHelp,
                    CommandAction = commandAction
                };
                MainLibrary.BuildShell.CommandRepository.Commands.Add(name, info);
            }
            public void BatchMakeCommandInfo(List<CommandRegisterInfo> commandInfos)
            {
                foreach (var info in commandInfos)
                {
                    MainLibrary.BuildShell.CommandRepository.Commands.Add(info.Name!, info);
                }
            }
            public void AllListCommandInfo(List<string>names, List<string>descriptions, List<string>usages, List<string>commandHelps, List<Func<string?,string[], Task<string>>> commandActions)
            {
                for (int i = 0; i < names.Count; i++)
                {
                    CommandRegisterInfo info = new CommandRegisterInfo
                    {
                        Name = names[i],
                        Description = descriptions[i],
                        Usage = usages[i],
                        CommandHelp = commandHelps[i],
                        CommandAction = commandActions[i]
                    };
                    MainLibrary.BuildShell.CommandRepository.Commands.Add(names[i], info);
                }
            }
        }
    }
}
