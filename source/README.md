# 从源工程生成游戏

## 工程位置与环境

- 在开发仓库中打开仓库根目录的 `YiGuangGameJam/`。
- 在提交 ZIP 解压后打开本文件旁的 `YiGuangGameJam/`。
- Unity 版本：**2022.3.62f1c1**，以工程内 `ProjectSettings/ProjectVersion.txt` 为准。
- 使用 Unity Hub 安装对应编辑器及目标平台的 Build Support 模块。
- 保留 `Assets/`、`Packages/`、`ProjectSettings/` 和全部 `.meta` 文件。
- 首次打开需要等待包恢复及资源导入；不要把本机 `Library/` 当作源码提交。
- 已记录 Cinemachine 2.10.7。其他包及锁定依赖见 `Packages/manifest.json` 与 `packages-lock.json`。
- manifest 包含 `com.coplaydev.unity-mcp` 的 Git 依赖，地址指向 CoplayDev/unity-mcp v10.0.0；首次恢复需要 Git 及网络访问。提交包未附带 Unity 编辑器和下载缓存。

## 构建步骤

1. 通过 Unity Hub 添加上述工程目录，使用指定版本打开，等待导入并检查 Console。
2. 打开 `File > Build Settings`。
3. 核对 `Scenes In Build` 中已启用的场景和顺序：
   - 0：`Assets/Scenes/MainMenu.unity`
   - 1：`Assets/Scenes/Stage_1.unity`
   - 2：`Assets/Scenes/Stage_2.unity`
   - 3：`Assets/Scenes/Stage_3.unity`
4. 选择团队实际要提交的平台；如需切换，执行 `Switch Platform`，等待导入完成。
5. 点击 `Build`，输出到提交材料的 `release/Windows/` 或 `release/macOS/` 等相应目录。平台和架构以实际构建为准。
6. 保留完整构建输出。Windows 不可只复制 `.exe`，必须连同同次生成的数据目录、DLL 等一起提供；macOS 保留完整 `.app`。
7. 在目标系统上从运行包启动，检查主菜单、三关入口、藤蔓搭路、阶段切换、角色移动与跳跃、失败重试、过关及退出。
8. 更新 `release/README.txt` 的系统、架构、启动文件和实际测试结果。

本说明根据当前工程配置整理，尚未证明从干净环境构建成功。

Unity 官方操作参考：[2022.3 Build Settings](https://docs.unity3d.com/2022.3/Documentation/Manual/BuildSettings.html)。
