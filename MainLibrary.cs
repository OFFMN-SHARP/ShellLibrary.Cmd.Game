using ShellLibrary.Cmd.Game.Command;
using ShellLibrary.Cmd.Game.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
namespace ShellLibrary.Cmd.Game
{
    public class MainLibrary
    {
        public class ShellSetting
        {
            public string? ShellName { get; set; }
            public string? ShellVerion { get; set; }
            public string? ShellDescription { get; set; }
            public bool? ShellTypwriterstyle { get; set; }
            public char ShellDelimiter { get; set; } = '|';
            public bool ShellWelcomeTemplate { get; set; } = true;
            public bool ShellCursorVisible {  get; set; } = true;
            public bool ShellTitle { get; set; }= true;
            public bool ShellRunBinary { get; set; } = true;
        }
        public class BuildShell
        {
            public static ShellSetting ShellSetting = new ShellSetting();
            public static TypeWriter TypeWriter = new TypeWriter();
            public static MainLoop MainLoop = new MainLoop();
            public static CommandRepository CommandRepository = new CommandRepository();
            public static CommandAndArgsParser CommandAndArgsParser = new CommandAndArgsParser();
            public static Register Register = new Register();
            public static MessageReops MessageReops = new MessageReops();
            public static Process ProcessDocker = new Process();
            public static StringBuilder Pipeline = new StringBuilder();
            public static List<string> CommandHistory = new List<string>();
            public static ProcessStartInfo DockerStartInfo = new ProcessStartInfo();

            public async Task WelcomeScreenAsync()
            {
                string WriteHistory = MainLoop.HistoryCommandWriteEnabled? "Enabled" : "Disabled";
                string WelcomeScreen = $@"{ShellSetting.ShellName} {ShellSetting.ShellVerion}
====================
{ShellSetting.ShellDescription}
HistoryWrite:{WriteHistory}
Try 'help' to see available commands.
Welcome back ,{Environment.UserName}";
                await TypeWriter.Write(WelcomeScreen,addnewline:true,clearscreen:true);
            }
            public async Task BuildAsync()
            {
                if (ShellSetting.ShellWelcomeTemplate)
                {
                    await WelcomeScreenAsync();
                }
                if (ShellSetting.ShellTitle&&!string.IsNullOrEmpty(ShellSetting.ShellName))
                {
                    Console.Title = ShellSetting.ShellName;
                }
                Console.CursorVisible = ShellSetting.ShellCursorVisible;
                MainLoop.Loop();
            }
        }
    }
}
