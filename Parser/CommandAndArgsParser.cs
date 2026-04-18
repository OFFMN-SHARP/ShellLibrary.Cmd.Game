using ShellLibrary.Cmd.Game.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShellLibrary.Cmd.Game.Parser
{
    public class CommandAndArgsParser
    {
        public class CommandAndArgs
        {
            public string Command { get; set; }
            public string? rawArgs { get; set; }
            public string[]? Args { get; set; }
            public bool IsPipeNext { get; set; } = false;
            public bool TheLastCommandIsPipe { get; set; } =false;
        }
        public enum CommandType
        {
            Hard,Pipe,Last
        }
        public async Task<bool> CommandAndArgsParse(string inputcommand)
        {
            string? ReturnValue = "";
            string? PipeToCommand = "";
            List<CommandAndArgs> InputCmd;
            List<CommandAndArgs> InitializeInputCmd()
            {
                
                if (inputcommand.Contains(MainLibrary.BuildShell.ShellSetting.ShellDelimiter))
                {
                    
                    List<CommandAndArgs> Cmdas = new List<CommandAndArgs>();
                    string[] commands = inputcommand.Split(MainLibrary.BuildShell.ShellSetting.ShellDelimiter);
                    string firstcmd = commands[0];
                    string lastcmd = commands[commands.Length - 1];
                    commands= commands.Skip(1).Take(commands.Length - 2).ToArray();
                    void Cut(string command,CommandType type)
                    {
                        string[]? args;
                        string? rawArgs;
                        string cmd;
                        bool isPipe = false;
                        bool isLastCommand = false;
                        switch (type)
                        {
                            case CommandType.Hard:
                                isPipe = false;
                                isLastCommand = false;
                                break;
                            case CommandType.Pipe:
                                isPipe = true;
                                isLastCommand = false;
                                break;
                            case CommandType.Last:
                                isPipe = true;
                                isLastCommand = true;
                                break;
                        }
                        if (command.Contains(' '))
                        {
                            args = command.Split(' ');
                            cmd = args[0];
                            rawArgs = string.Join(" ", args.Skip(1));
                            args = args.Skip(1).ToArray();
                        }
                        else
                        {
                            cmd = command;
                            args = new string[] { };
                            rawArgs = "";
                        }
                        Cmdas.Add(new CommandAndArgs
                        {
                            Command = cmd,
                            Args = args,
                            rawArgs = rawArgs,
                            IsPipeNext = isPipe,
                            TheLastCommandIsPipe = isLastCommand
                        });
                    }
                    Cut(firstcmd, CommandType.Hard);
                    foreach (string command in commands)
                    {
                        Cut(command, CommandType.Pipe);
                    }
                    Cut(lastcmd, CommandType.Last);
                    return Cmdas;
                }
                else
                {
                    string[]? args;
                    string? rawArgs;
                    string cmd;
                    if (inputcommand.Contains(' '))
                    {
                        args = inputcommand.Split(' ');
                        cmd = args[0];
                        rawArgs = string.Join(" ", args.Skip(1));
                        args = args.Skip(1).ToArray();
                    }
                    else
                    {
                        cmd = inputcommand;
                        args = new string[] { };
                        rawArgs = "";
                    }
                    return new List<CommandAndArgs> { 
                        new CommandAndArgs {
                            Command = cmd,
                            Args = args,
                            rawArgs = rawArgs,
                            IsPipeNext = false,
                            TheLastCommandIsPipe =true 
                        }
                    };
                }
            }
            InputCmd = InitializeInputCmd();
            foreach (CommandAndArgs commandinnfo in InputCmd) 
            {
                string command = commandinnfo.Command;
                try
                {
                    try
                    {
                        string[] command_Split = command.Split(' ');
                        string command_Name = command_Split[0];
                        bool CheckIsFile = File.Exists(command_Name);
                        if (CheckIsFile && MainLibrary.BuildShell.ShellSetting.ShellRunBinary)
                        {
                            MainLibrary.BuildShell.DockerStartInfo.FileName = command_Name;
                            MainLibrary.BuildShell.DockerStartInfo.Arguments = commandinnfo.rawArgs;
                            MainLibrary.BuildShell.DockerStartInfo.UseShellExecute = false;
                            MainLibrary.BuildShell.DockerStartInfo.RedirectStandardInput = true;
                            MainLibrary.BuildShell.DockerStartInfo.RedirectStandardOutput = true;
                            MainLibrary.BuildShell.DockerStartInfo.RedirectStandardError = true;
                            MainLibrary.BuildShell.DockerStartInfo.CreateNoWindow = true;
                            MainLibrary.BuildShell.ProcessDocker.StartInfo = MainLibrary.BuildShell.DockerStartInfo;
                            MainLibrary.BuildShell.ProcessDocker.Start();
                            MainLibrary.BuildShell.ProcessDocker.WaitForExit();
                        }
                        else throw new FileNotFoundException();
                    }
                    catch (FileNotFoundException)
                    {
                        string[] command_Split = command.Split(' ');
                        string command_Name = command_Split[0];
                        if (MainLibrary.BuildShell.CommandRepository.Commands.ContainsKey(command_Name))
                        {
                            var commandInfo = MainLibrary.BuildShell.CommandRepository.Commands[command_Name];
                            if (commandInfo.CommandAction != null)
                            {
                                switch (commandinnfo.IsPipeNext, commandinnfo.TheLastCommandIsPipe)
                                {
                                    case (true, true)://last command
                                        await commandInfo.CommandAction(PipeToCommand, commandinnfo.Args ?? Array.Empty<string>());
                                        ReturnValue = "";
                                        PipeToCommand = "";
                                        break;
                                    case (false, false): //hard commsnd
                                        ReturnValue = await commandInfo.CommandAction(null, commandinnfo.Args ?? Array.Empty<string>());
                                        PipeToCommand = ReturnValue;
                                        break;
                                    case (true, false): //mid command
                                        ReturnValue = await commandInfo.CommandAction(PipeToCommand, commandinnfo.Args ?? Array.Empty<string>());
                                        PipeToCommand = ReturnValue;
                                        break;
                                    case (false, true): //only one command
                                        await commandInfo.CommandAction(null, commandinnfo.Args ?? Array.Empty<string>());
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string ErrorMessage = ex.Message;
                        if(MainLibrary.BuildShell.MessageReops.Messages.TryGetValue(ErrorMessage,out string message))
                        {
                            Console.WriteLine(message);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
            /*
            try
            {
                if (command.StartsWith('\"')) return false;
                String[] group_text = command.Split('\"').Where((part, index) => index % 2 == 0).ToArray();
                String[] group_command_First = command.Split('\"').Where((part, index) => index % 2 == 1).ToArray();
                Dictionary<String, String[]> group_command_Second = new Dictionary<String, String[]>();
                string key = string.Join(" ", group_command_First);
                String[] command_Second = key.Split(MainLibrary.BuildShell.ShellSetting.ShellDelimiter.ToString());
                for (int i = 0; i < command_Second.Length; i++)
                {
                    if (!string.IsNullOrEmpty(command_Second[i])) return false;
                    String[] command_Second_Split = command_Second[i].Split(' ');
                    group_command_Second.Add(command_Second_Split[0], command_Second_Split.Skip(1).ToArray());
                }
                for (int i = 0;i < group_command_Second.Count; i++)
                {
                    if (MainLibrary.BuildShell.CommandRepository.Commands.ContainsKey(group_command_Second.Keys.ToArray()[i]))
                    {
                        var ReturnValue = MainLibrary.BuildShell.CommandRepository.Commands[group_command_Second.Keys.ToArray()[i]];
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;*/
        }
    }
}
