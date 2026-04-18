using System;
using System.IO;
using System.Text;

namespace ShellLibrary.Cmd.Game
{
    /// <summary>
    /// 一个通用的 TextWriter，它会将控制台的输出，
    /// 通过一个事件分发给任何订阅了它的游戏UI系统。
    /// </summary>
    public class ConsoleTextWriter : TextWriter
    {
        // 当有新消息时触发
        public event Action<string> OnMessage;
        // 当有新的一行消息时触发（可选，方便UI处理）
        public event Action<string> OnLine;

        private readonly StringBuilder _buffer = new StringBuilder();

        // 必须实现的抽象属性
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            // 这里可以根据需要实现，比如缓冲字符直到遇到换行
            // 最简单的就是直接触发事件
            OnMessage?.Invoke(value.ToString());
        }

        public override void Write(string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            OnMessage?.Invoke(value);

            // 处理换行逻辑，触发 OnLine 事件，方便UI直接追加一行
            _buffer.Append(value);
            var text = _buffer.ToString();
            int index;
            while ((index = text.IndexOf(Environment.NewLine, StringComparison.Ordinal)) != -1)
            {
                var line = text.Substring(0, index);
                OnLine?.Invoke(line);
                text = text.Substring(index + Environment.NewLine.Length);
            }
            _buffer.Clear();
            _buffer.Append(text);
        }

        // 这个方法方便开发者在游戏启动时一行调用，完成设置
        public static ConsoleTextWriter Install()
        {
            var writer = new ConsoleTextWriter();
            Console.SetOut(writer);
            Console.SetError(writer); // 也把错误输出重定向过来
            return writer;
        }
    }
}