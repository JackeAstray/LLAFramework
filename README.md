### Unity Editor Tools And Game Framework
Unity编辑器工具和游戏框架，只在Windows中进行过测试<br>
Unity版本：2021.3.15f<br>

- [功能](#Function)
- [文件夹结构](#FolderStructure)

## 功能：<a name="Function"></a>
<details>
<summary>1 游戏框架</summary>
其中包含了以下模块<br>
- 数据模块<br>
- 事件模块<br>
- 多语言模块<br>
- 对象池模块<br>
- 资源模块<br>
- 场景模块<br>
- 声音模块<br>
- 终端模块<br>
- UI模块<br>
</details>
<details>
  
<summary>2 Excel导出</summary>
在指定的表格中的数据可以保存为以下格式文件<br>
- .cs<br>
- .xml<br>
- .json<br>
- lua<br>
</details>

<details>
<summary>3 日志系统</summary>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130502.png)

开启宏之后输入以下代码就可以使用，关闭宏之后日志就不会输出

```csharp
Log.Debug("Hello Word");
Log.Info("Hello Word");
Log.Warning("Hello Word");
Log.Error("Hello Word");
Log.Fatal("Hello Word");
```
</details>

<details>
<summary>4 打开指定路径文件夹</summary>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130517.png)

该功能用于打开Unity的一些路径
</details>

<details>
<summary>5 生成脚本工具</summary>

生成脚本工具使用说明<br>
1、输入要生成类的名称<br>
2、选择要生成的模板<br>
3、点击创建脚本即可<br>
【ReunionMovement\Editor\Resources\Txt】该路径用于存放模板<br>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130428.png)

</details>

<details>
<summary>6 编译器模式碰撞器绘制</summary>

</details>

<details>
<summary>7 UI工具</summary>

</details>

<details>
<summary>8 小功能</summary>
包含的功能有给场景添加屏幕日志、给场景添加FPS、给场景选中的对象添加多语言用脚本、场景切换、修改版本号
</details>

<details>
<summary>9 焦点工具</summary>
查找资源并将该资源设为焦点 快捷键Control + L
</details>


## 文件夹结构：<a name="FolderStructure"></a>
- 3RD                   用于存放第三方库<br>
- Editor                用于存放第三方库<br>
  - Scenes              用于存放生成UI的场景<br>
- Resources             资源文件夹<br>
- ReunionMovement       模块化插件（每个单独的模块都会放入其中）<br>
  - APP                APP是用于启动各个模块系统<br>
    - Base            APP的基础功能<br>
    - Module          APP的各个模块<br>
      - DatabaseModule    数据模块<br>
      - EventModule       事件模块<br>
      - LanguagesModule   多语言模块<br>
      - ObjectPoolModule  对象池模块<br>
      - ResourcesModule   资源模块<br>
      - SceneModule       场景模块<br>
      - SoundModule       声音模块<br>
      - TerminalModule    终端模块<br>
      - UIModule          UI模块<br>
    - StartApp        APP的启动脚本<br>
  - Common             公用类<br>
    - Observer        观察者模式基类<br>
    - SingleToneMgr   单例模式基类<br>
    - PathUtils       单例模式基类<br>
    - Editor          <br>
      - ScriptingDefineSymbols  脚本宏<br>
  - ExcelTool          表格导出工具(用于导出.cs|.xml|.json|.lua文件)<br>
  - Extensions         扩展功能脚本<br>
  - GenerateScriptTool 创建脚本工具<br>
  - LogTool            日志工具，集成了宏命令可开启和关闭Log<br>
  - OpenPath           打开指定路径文件夹<br>
  - SmallFunctions     小功能<br>
  - SplitImg           图片自动裁切留白区域  需要此路径../Tools/SplitImg/SplitImg.exe下的EXE<br>
  - Spotlight          焦点工具、用于搜索对象<br>
  - TextureImporter    图片导入Assets\Resources\Sprites路径时自动修改图片配置<br>
  - Toolbox            工具箱<br>
    - Billboard       广告牌<br>
    - ColliderGizmo   碰撞器绘制（编辑器下碰撞器会有颜色）<br>
    - DownloadFile    下载文件<br>
    - Http            Http网络访问<br>
    - LitJson         LitJson源码<br>
    - LoadImage       加载Image<br>
    - Screen          屏幕适配<br>
    - ScreenLogger    Log显示在屏幕上<br>
    - TextRect        Text自动改变宽高<br>
    - Timer           时间管理器<br>
    - FPSCounter      FPS计数器<br>
- Scenes                场景文件夹<br>
- Scripts               脚本文件夹<br>
- Settings              URP配置文件<br>
