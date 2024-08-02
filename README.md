### Unity Game Framework
Unity编辑器工具和游戏框架，只在Windows、Android、Webgl中进行过使用<br>
Unity版本：2022.3.28fc1<br>

- [功能](#Function)
- [文件夹结构](#FolderStructure)

## 功能：<a name="Function"></a>
<details>
<summary>1 游戏框架</summary>
<br>
其中包含了以下模块<br>

- 数据模块<br>
- 事件模块<br>
- 多语言模块<br>
- 消息模块<br>
- 网络模块<br>
- 资源模块<br>
- 场景模块<br>
- 声音模块<br>
- 终端模块<br>
- UI模块<br>
<br>
</details>
<details>
  
<summary>2 Excel导出</summary>
<br>
在指定的表格中的数据可以保存为以下格式文件<br>
- .cs<br>
- .xml<br>
- .json<br>
- .lua<br>
<br>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130523.png)
</details>

<details>
<summary>3 日志系统</summary>
<br>
  
![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130502.png)<br>
<br>
开启宏之后输入以下代码就可以使用，关闭宏之后日志就不会输出<br>
<br>
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
<br>
该功能用于打开Unity的一些路径<br>
<br>
  
![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130517.png)
</details>

<details>
<summary>5 生成脚本工具</summary>
<br>
生成脚本工具使用说明<br>
1、输入要生成类的名称<br>
2、选择要生成的模板<br>
3、点击创建脚本即可<br>
【ReunionMovement\Editor\Resources\Txt】该路径用于存放模板<br>
<br>
  
![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130428.png)

</details>

<details>
<summary>6 编译器模式碰撞器绘制</summary>
<br>
启用PHYSICS宏（一般只用这个），后在编辑器视图可以看见碰撞器范围<br>
<br>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130511.png)
</details>

<details>
<summary>7 UI工具</summary>
<br>
UI工具使用说明<br>
1、输入要生成类的名称<br>
2、点击创建场景（创建的场景后缀为UIPlane的对象就是UI）<br>
2、点击创建脚本（创建一个UI类用于管理UI）<br>
3、点击绑定脚本（将创建的UI脚本绑定到UI上）<br>
4、将当前场景中的UI导出为预制体（该项在编辑完UI后，点击一次就可以将UI导出到指定路径，替换掉老的UI）<br>
<br>
  
![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130414.png)
</details>

<details>
<summary>8 小功能</summary>
<br>
包含的功能有给场景添加屏幕日志、给场景添加FPS、给场景选中的对象添加多语言用脚本、场景切换、修改版本号<br>
<br>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130336.png)
</details>

<details>
<summary>9 焦点工具</summary>
<br>
查找资源并将该资源设为焦点 快捷键Control + L<br>
<br>
  
![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130454.png)
</details>

## 文件夹结构：<a name="FolderStructure"></a>
- 3RD                             |用于存放第三方库<br>
  - Animated Loading Icons        |加载动画<br>
  - FancyScrollView               |花式滚动视图<br>
  - InputSystem                   |输入系统例子<br>
  - Joystick Pack                 |摇杆<br>
  - SqlCipher4Unity3D             |Sqlite插件<br>
  - TouchScript                   |触摸插件<br>
  - UIEffect                      |UI特效<br>
- Editor                          |编辑器文件夹<br>
  - Scenes                        |用于存放生成UI的场景<br>
- Plugins                         |插件文件夹<br>
- Resources                       |资源文件夹<br>
  - AutoDatabase                  |此文件夹用于存放生成的数据文件<br>
  - Fonts                         |字体<br>
  - Materials                     |材质球<br>
  - Models                        |模型<br>
  - Prefabs                       |预制体<br>
  - ScriptableObject              |脚本化对象<br>
  - Shader                        |Shader<br>
  - SkyBox                        |天空盒<br>
  - Sounds                        |音频<br>
  - Sprites                       |精灵<br>
  - Terminal                      |终端预制体<br>
  - UI                            |UI<br>
- ResourcesFile                   |此文件夹暂时无用<br>
- ReunionMovement                 |模块化插件（每个单独的模块都会放入其中）<br>
  - APP                           |APP是用于启动各个模块系统<br>
    - Base                        |APP的基础功能<br>
    - Module                      |APP的各个模块<br>
      - AssetBundleModule         |AB模块（暂时无用）<br>
      - ColorPaletteModule        |调色板模块，用于记录调整配色方案<br>
      - DatabaseModule            |数据模块<br>
      - EventModule               |事件模块<br>
      - LanguagesModule           |多语言模块<br>
      - MessageModule             |消息模块<br>
      - NetworkModule             |网络模块<br>
      - ResourcesModule           |资源模块<br>
      - SceneModule               |场景模块<br>
      - SoundModule               |声音模块<br>
      - TerminalModule            |终端模块<br>
      - UIModule                  |UI模块<br>
    - StartApp                    |APP的启动脚本<br>
  - Common                        |公用类<br>
    - Base                        |基础类（射线基类、单例基类、观察者基类）<br>
    - Editor                      <br>
      - ScriptingDefineSymbols    |脚本宏<br>
    - Observer                    |观察者模式基类<br>
    - SingleToneMgr               |单例模式基类<br>
    - PathUtils                   |单例模式基类<br>
    - Extensions                  |扩展脚本<br>
      - Algorithm                 |计算扩展<br>
      - Collection                |集合扩展<br>
      - Object                    |Object扩展<br>
      - Other                     |其他扩展<br>
      - Random                    |随机扩展<br>
      - UGUIExtensions            |Image扩展<br>
    - LitJson                     |LitJson<br>
    - LogTool                     |日志工具<br>
    - ObjectPool                  |对象池<br>
  - ExcelTool                     |表格导出工具(用于导出.cs|.xml|.json|.lua文件)<br>
  - Managers                      |管理器（协程、Sqlite、通用状态机、任务）<br>
  - Runtime                       |跳过Unity Logo<br>
  - Toolbox                       |工具箱<br>
    - Application                 |Android状态栏<br>
    - Assembly                    |程序集工具<br>
    - ColliderGizmo               |碰撞器绘制 在编辑器窗口看碰撞器框（编辑是使用）<br>
    - Encrypt                     |加墨解密工具<br>
    - GenerateScriptTool          |生成脚本用工具<br>
    - OpenPath                    |打开指定路径工具<br>
    - SmallFunctions              |小功能<br>
    - SplitImg                    |图片自动裁切留白区域，需要此EXE【../Tools/SplitImg/SplitImg.exe】<br>
    - Spotlight                   |焦点工具、用于搜索对象<br>
    - Timer                       |计时器<br>
  - Tools                         |工具箱<br>
    - Arrow                       |箭头工具<br>
    - Billboard                   |广告牌<br>
    - Camera                      |摄像机（旋转、漫游）<br>
    - FPS                         |FPS计数器<br>
    - Ripple                      |UI点击后波浪效果<br>
    - Screen                      |屏幕适配<br>
    - ScreenLogger                |Log显示在屏幕上<br>
    - TextTools                   |Text工具<br>
    - VibrationUtil               |震动<br>
- Scenes                          |场景文件夹<br>
- Scripts                         |脚本文件夹<br>
- Settings                        |URP配置文件<br>
- StreamingAssets                 |流文件夹<br>