using ShellLibrary.Cmd.Game.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ShellLibrary.Cmd.Game;

namespace ShellLibrary.Cmd.Game.Command
{
    public class MainLoop
    {
        public int CancelPressCounter = 0;
        public bool SafeExitEnabled { get; set; }=true;
        public bool EnabledShellExit { get; set; } = true;
        public bool HistoryCommandWriteEnabled { get; set; } = true;
        public enum SIGINT
        {
            Process_Runing=0, Process_Stop=1, Process_Exiting=2 , Process_Error=3,Shell_Exit=4,Shell_Empty=5
        }
        public SIGINT Sigint { get; set; } = SIGINT.Shell_Empty;
        public string GetTipText { get; set; } = ">";
        public void Loop()
        {
            if (!File.Exists("history.txt"))
            {
                File.Create("history.txt").Close();
            }
            Console.CancelKeyPress +=(sender, e) =>
            {
                if (CancelPressCounter >= 2&&SafeExitEnabled)
                {
                    if (MainLibrary.BuildShell.ProcessDocker != null && !MainLibrary.BuildShell.ProcessDocker.HasExited)
                    {
                        try
                        {
                            MainLibrary.BuildShell.ProcessDocker.Kill();
                            MainLibrary.BuildShell.ProcessDocker.WaitForExit(100);
                        } catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("Failed to kill process.");
                        }
                    }
                    Console.Clear();
                    Console.WriteLine("Safely Exiting...");
                    Sigint = SIGINT.Shell_Exit;//SafeMode
                    CancelPressCounter = 0;
                }
                CancelPressCounter++;
                e.Cancel = true;
                switch (Sigint)
                {
                    case SIGINT.Shell_Empty:
                        break;
                    case SIGINT.Process_Runing:
                        break;
                    case SIGINT.Process_Stop:
                        break;
                    case SIGINT.Process_Exiting:
                        break;
                    case SIGINT.Process_Error:
                        break;
                    case SIGINT.Shell_Exit:
                        if (EnabledShellExit)
                        {
                            Console.WriteLine("Exiting...");
                            Environment.Exit(0);
                        }
                        break;
                }
            };
            while (true)
            {
                Console.Write(GetTipText);
                string? GetInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(GetInput)&&!MainLibrary.BuildShell.CommandAndArgsParser.CommandAndArgsParse(GetInput).Result) Console.WriteLine(MainLibrary.BuildShell.MessageReops.Messages.TryGetValue("CommandNotFound", out string? msg) ? msg : "Command not found.");
                if (HistoryCommandWriteEnabled) File.AppendAllTextAsync("history.txt", "["+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"] " +GetInput + Environment.NewLine);
            }
        }
    }
}
