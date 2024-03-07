### Unity Editor Tools And Game Framework
Unity编辑器工具和游戏框架，只在Windows中进行过测试<br>
Unity版本：2021.3.18f<br>

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
- 对象池模块<br>
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

<details>
<summary>10 其他功能</summary>
1、广告牌<br>
2、屏幕适配<br>
3、屏幕LOG<br>
4、计时器<br>
5、FPS<br>
6、震动（ios、android用）<br>
7、图片首次导入到指定文件夹自动替换类型<br>
8、图片自动裁剪留白<br>
9、单例模式父类<br>
10、扩展功能<br>
 - AsyncOperationExtensions<br>
 - ColorExtensions<br>
 - ComponentExtensions<br>
 - EngineExtensions<br> 
 - EngineToolExtensions<br>
 - FindExtensions<br>
 - GameObjectExtensions<br>
 - ResourcesExtensions<br>
 - TransformExtensions<br>
 - Vector3Extensions<br>
</details>


## 文件夹结构：<a name="FolderStructure"></a>
- 3RD                   用于存放第三方库<br>
- Editor                编辑器文件夹<br>
  - Scenes              用于存放生成UI的场景<br>
- Resources             资源文件夹<br>
- ReunionMovement       模块化插件（每个单独的模块都会放入其中）<br>
  - APP                   APP是用于启动各个模块系统<br>
    - Base                APP的基础功能<br>
    - Module              APP的各个模块<br>
      - DatabaseModule    数据模块<br>
      - EventModule       事件模块<br>
      - LanguagesModule   多语言模块<br>
      - ObjectPoolModule  对象池模块<br>
      - ResourcesModule   资源模块<br>
      - SceneModule       场景模块<br>
      - SoundModule       声音模块<br>
      - TerminalModule    终端模块<br>
      - UIModule          UI模块<br>
    - StartApp            APP的启动脚本<br>
  - Common                公用类<br>
    - Observer            观察者模式基类<br>
    - SingleToneMgr       单例模式基类<br>
    - PathUtils           单例模式基类<br>
    - Editor              <br>
      - ScriptingDefineSymbols  脚本宏<br>
  - ExcelTool             表格导出工具(用于导出.cs|.xml|.json|.lua文件)<br>
  - Extensions            扩展功能脚本<br>
  - GenerateScriptTool    创建脚本工具<br>
  - LogTool               日志工具，集成了宏命令可开启和关闭Log<br>
  - OpenPath              打开指定路径文件夹<br>
  - SmallFunctions        小功能<br>
  - SplitImg              图片自动裁切留白区域，需要此EXE【../Tools/SplitImg/SplitImg.exe】<br>
  - Spotlight             焦点工具、用于搜索对象<br>
  - Toolbox               工具箱<br>
    - Application         控制Android状态栏<br>
    - ColliderGizmo       编辑器下碰撞器显示绘制边框<br>
    - DownloadFile        下载文件<br>
    - Effect              效果<br>
      - Arrow             箭头工具<br>
      - Billboard         广告牌<br>
      - Camera            摄像机（旋转、漫游）<br>
      - FPS               FPS计数器<br>
      - Ripple            UI点击后波浪效果<br>
      - Screen            屏幕适配<br>
      - ScreenLogger      Log显示在屏幕上<br>
      - TextTools         Text工具<br>
      - VibrationUtil     震动<br>
    - Http                Http工具用来快速访问网络、调用API<br>
    - LitJson             JSON工具<br>
    - LoadImage           加载Image<br>
    - Timer               计时器<br>
- Scenes                  场景文件夹<br>
- Scripts                 脚本文件夹<br>
- Settings                URP配置文件<br>