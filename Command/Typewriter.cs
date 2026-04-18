using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShellLibrary.Cmd.Game.Command
{
    public class TypeWriter
    {
        public async Task Write(string text,int speed=10,bool addnewline=false,bool clearscreen=false,string? lefttext=null,string? righttext = null)
        {
            if(clearscreen)Console.Clear();
            for (int i = 0; i < text.Length; i++) 
            {
                Console.Write(lefttext+text[i]+righttext);
                Console.Out.Flush();
                await Task.Delay(speed);
            }
            if(addnewline)Console.WriteLine();
        }
    }
}
