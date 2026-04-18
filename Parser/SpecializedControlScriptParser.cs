using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ShellLibrary.Cmd.Game;

namespace ShellLibrary.Cmd.Game.Parser
{
    public class SpecializedControlScriptParser
    {
        public async Task<string> Parser(string FilePath)
        {
            var Script = File.ReadAllLines(FilePath);
            foreach (var line in Script)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                await MainLibrary.BuildShell.CommandAndArgsParser.CommandAndArgsParse(line);
            }
            return "Done";
        }
    }
}
