
# ShellLibrary.Cmd.Game  
*(ShellLibrary.Cmd.Game)*

**通用游戏控制台框架后端**  
*(A universal game console framework backend)*

---

## 简介  
*(Introduction)*

`ShellLibrary.Cmd.Game` 是一个专为游戏引擎（Unity、Godot、Unreal 等）设计的超轻量级交互式控制台框架。它基于 .NET Standard 2.1，仅 **28 KB**，无任何外部依赖。通过重定向 `Console` 输出流，可无缝嵌入任何游戏 UI 系统，为开发者提供开箱即用的命令注册、管道、历史记录与安全锁定功能。  
*(`ShellLibrary.Cmd.Game` is an ultra-lightweight interactive console framework designed for game engines (Unity, Godot, Unreal, etc.). Built on .NET Standard 2.1, it weighs only **28 KB** with zero external dependencies. By redirecting the `Console` output stream, it seamlessly integrates with any game UI system, providing out-of-the-box command registration, piping, history, and security locking.)*

---

## 特性  
*(Features)*

- **零样板代码**：只需拖拽预制体或挂载脚本，即刻拥有功能完整的控制台。  
  *(Zero boilerplate: Just drop a prefab or attach a script to get a fully functional console.)*
- **跨引擎通用**：核心逻辑与渲染分离，适配任何 C# 游戏引擎。  
  *(Engine-agnostic: Core logic decoupled from rendering, adaptable to any C# game engine.)*
- **极致轻量**：编译后仅 28 KB，对移动端与 WebGL 友好。  
  *(Ultra-lightweight: Only 28 KB compiled, friendly to mobile and WebGL.)*
- **安全可控**：支持白名单命令注册，可彻底禁止外部二进制执行，杜绝作弊与恶意输入。  
  *(Secure & controllable: Whitelist command registration, can completely disable external binary execution to prevent cheating and malicious input.)*
- **管道支持**：允许命令组合（如 `give sword | enchant fire`），提升玩家互动深度。  
  *(Pipeline support: Enables command composition (e.g., `give sword | enchant fire`), enhancing player interaction depth.)*
- **打字机效果**：可选复古风格文本动画，提升沉浸感。  
  *(Typewriter effect: Optional retro-style text animation for enhanced immersion.)*
- **MIT 开源**：自由使用、修改、商用。  
  *(MIT Licensed: Free to use, modify, and commercialize.)*

---

## 快速开始  
*(Quick Start)*

### 1. 安装  
*(1. Installation)*

通过 NuGet 安装：  
*(Install via NuGet:)*  
```
dotnet add package ShellLibrary.Cmd.Game
```

或直接下载源码，将项目添加至你的解决方案。  
*(Or download the source and add the project to your solution.)*

### 2. 在 Unity 中使用  
*(2. Usage in Unity)*

```csharp
using UnityEngine;
using ShellLibrary.Cmd.Game;
using ShellLibrary.Cmd.Game.Command;

public class GameConsole : MonoBehaviour
{
    private ConsoleTextWriter writer;

    void Start()
    {
        // 安装重定向，使 Console 输出流向 UI
        writer = ConsoleTextWriter.Install();

        // 订阅行输出事件
        writer.OnLine += (line) => {
            // 在此更新你的 UI 文本组件
            Debug.Log($"[CONSOLE] {line}");
        };

        // 配置 Shell（可选）
        MainLibrary.BuildShell.ShellSetting.ShellName = "DevConsole";
        MainLibrary.BuildShell.ShellSetting.ShellTypwriterstyle = true;
        MainLibrary.BuildShell.ShellSetting.ShellRunBinary = false; // 游戏内强烈建议关闭

        // 注册游戏命令
        var registrar = new Register.CommandInfoMake();
        registrar.MakeCommandInfo("heal", "恢复生命值", "heal", "将玩家生命回满",
            async (stdin, args) => {
                // 调用你的游戏逻辑
                Player.Heal(100);
                return "Player healed.";
            });
    }

    // 当用户输入命令时调用（例如在 UI 输入框按下回车）
    public void ExecuteCommand(string input)
    {
        MainLibrary.BuildShell.CommandAndArgsParser.CommandAndArgsParse(input);
    }
}
```

### 3. 在 Godot 中使用  
*(3. Usage in Godot)*

```csharp
using Godot;
using ShellLibrary.Cmd.Game;
using ShellLibrary.Cmd.Game.Command;

public class GameConsole : Node
{
    private ConsoleTextWriter writer;
    private RichTextLabel outputLabel;

    public override void _Ready()
    {
        outputLabel = GetNode<RichTextLabel>("ConsoleOutput");
        writer = ConsoleTextWriter.Install();
        writer.OnLine += (line) => {
            outputLabel.Text += line + "\n";
        };

        MainLibrary.BuildShell.ShellSetting.ShellRunBinary = false;
        // ... 注册命令
    }

    public void ExecuteCommand(string input)
    {
        MainLibrary.BuildShell.CommandAndArgsParser.CommandAndArgsParse(input);
    }
}
```

---

## 命令注册示例  
*(Command Registration Example)*

```csharp
var registrar = new Register.CommandInfoMake();
registrar.MakeCommandInfo(
    name: "give",
    description: "给予物品",
    usage: "give <itemId> [amount]",
    commandHelp: "将指定物品添加到玩家背包。",
    commandAction: async (stdin, args) => {
        string itemId = args[0];
        int amount = args.Length > 1 ? int.Parse(args[1]) : 1;
        Inventory.AddItem(itemId, amount);
        return $"Gave {amount} x {itemId}.";
    }
);
```

---

## 安全性说明  
*(Security Notes)*

- 在游戏发布版中，**务必设置 `ShellRunBinary = false`**，禁止玩家执行任何外部程序。  
  *(In release builds, **always set `ShellRunBinary = false`** to prevent players from executing external programs.)*
- 仅注册必要的游戏命令，切勿暴露敏感操作（如文件删除、系统信息）。  
  *(Register only necessary game commands and never expose sensitive operations (e.g., file deletion, system info).)*
- 可配合 `SpecializedControlScriptParser` 实现自动化脚本，但需对脚本来源进行严格校验。  
  *(You can use `SpecializedControlScriptParser` for automation scripts, but always validate the script source.)*

---

## 许可证  
*(License)*

本项目采用 [MIT 许可证](LICENSE)。  
*(This project is licensed under the [MIT License](LICENSE).)*

---

## 致谢  
*(Acknowledgments)*

`ShellLibrary.Cmd.Game` 脱胎于 `ShellLibrary.Cmd`，一个为运维与受限环境设计的声明式 Shell 框架。感谢所有早期使用者的反馈。  
*(`ShellLibrary.Cmd.Game` originated from `ShellLibrary.Cmd`, a declarative shell framework for ops and restricted environments. Thanks to all early users for their feedback.)*