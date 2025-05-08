### Unity Game Framework
Unity编辑器工具和游戏框架，只在Windows、Android、Webgl中进行过使用<br>
Unity版本：2022.3.28fc1<br>

- [功能](#Function)
- [ImageEx](#ImageEx)
- [感谢](#感谢)

## 功能：<a name="Function"></a>

<summary>1 游戏框架</summary>
<br>
其中包含了以下模块<br>
- 事件模块<br>
- 输入模块<br>
- 数据模块<br>
- 多语言模块<br>
- 消息模块<br>
- 网络模块<br>
- 资源模块<br>
- 场景模块<br>
- 声音模块<br>
- 状态模块<br>
- 任务模块<br>
- 终端模块<br>
- UI模块<br>
<br>

<summary>2 Excel导出</summary>
目前支持通过表格生成类，也可以通过表格生成Json、Sqlite、xml、lua、ScriptableObject<br>
<br>
在指定的表格中的数据可以保存为以下格式文件<br>
- .cs<br>
- .xml<br>
- .json<br>
- .lua<br>
- .db<br>
<br>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130523.png)

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
<summary>4 打开指定路径文件夹</summary>
<br>
该功能用于打开Unity的一些路径<br>
<br>
  
![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130517.png)

<summary>5 编译器模式碰撞器绘制</summary>
<br>
启用PHYSICS宏（一般只用这个），后在编辑器视图可以看见碰撞器范围<br>
<br>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130511.png)

<summary>6 UI工具</summary>
<br>
UI工具使用说明<br>
1、输入要生成类的名称<br>
2、点击创建场景（创建的场景后缀为UIPlane的对象就是UI）<br>
2、点击创建脚本（创建一个UI类用于管理UI）<br>
3、点击绑定脚本（将创建的UI脚本绑定到UI上）<br>
4、将当前场景中的UI导出为预制体（该项在编辑完UI后，点击一次就可以将UI导出到指定路径，替换掉老的UI）<br>
<br>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130414.png)

<summary>7 小功能</summary>
<br>
包含的功能有给场景添加屏幕日志、给场景添加FPS、给场景选中的对象添加多语言用脚本、场景切换、修改版本号<br>
<br>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130336.png)

<summary>8 焦点工具</summary>
<br>
查找资源并将该资源设为焦点 快捷键Control + L<br>
<br>

![inspector](https://github.com/JackeAstray/EditorTools/blob/main/Screenshot/%E5%B1%8F%E5%B9%95%E6%88%AA%E5%9B%BE%202023-09-11%20130454.png)

## ImageEx：<a name="ImageEx"></a>
ImageEx是一个Image扩展工具。可以轻松的编辑Image样式，而无需美术绘制图片。<br>
<br>

![inspector](https://github.com/JackeAstray/LLAFramework/blob/main/Screenshot/ImageEx/ImageEx-Inspector.png)
![inspector](https://github.com/JackeAstray/LLAFramework/blob/main/Screenshot/ImageEx/ImageEx-Example.png)


## 感谢：<a name="感谢"></a>
感谢以下的开源项目对我的帮助。<br>
https://github.com/netpyoung/SqlCipher4Unity3D<br>
https://github.com/snikit/CSG---3d-boolean-operations-Unity-<br>
https://github.com/mob-sakai/UIEffect<br>