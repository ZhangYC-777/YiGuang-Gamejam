# Caterpillar is Coming —— 源码与构建说明（source/README）

本目录是 Unity 工程源码（未包含 Library 等由编辑器生成的目录）。
游戏名：**Caterpillar is Coming**（中文名：**虫来**；队伍：Pig vs Vegetable，Global Game Jam 作品）

## 0. 本目录的两种形态（同一份内容快照）
- `Assets/`、`Packages/`、`ProjectSettings/`：**展开形式**的工程，
  直接用 Unity 打开本 `source` 文件夹即可。
- `虫来_Unity完整源工程_20260906_102529.zip`：**完整源工程压缩包**。
  解压后得到 `YiGuangGameJam/` 文件夹，其中是完整工程（不含 Library，
  约 33MB）。用 Unity Hub 打开时选择解压后的 **`YiGuangGameJam`** 文件夹。
  注意两点：
  1) 该压缩包快照的 Company 名为 `DefaultCompany`，外层展开版为
     `Pig vs Vegetable`（不影响运行）；若重新构建并希望署名一致，
     可在 Player Settings 中把 Company 改为 `Pig vs Vegetable`。
  2) 压缩包内多包含一个未使用的旧场景 `Assets/Scenes/SampleScene.unity`，
     不影响构建（Build Settings 中未启用）。

## 1. 工程内容
```
source/
├── Assets/            # 脚本、场景、预制体、美术与音频素材
│   ├── Scenes/        # MainMenu / Stage_1 / Stage_2 / Stage_3
│   ├── Scripts/       # 游戏逻辑源码（详见下方“主要脚本”）
│   ├── Prefabs/       # 预制体（藤蔓、叶子、敌人等）
│   ├── Audios/        # 游戏音乐
│   └── Resources/     # 运行时按名加载的资源（含 BGM）
├── Packages/          # Unity 包管理清单
└── ProjectSettings/   # 工程设置
```
- 目录中**不包含** `Library/`、`Logs/`、`Temp/` 等编辑器自动生成内容，
  首次打开由 Unity 自动生成。
- 依赖的 Unity 包由 `Packages/manifest.json` 声明，打开工程后自动恢复。

## 2. 环境要求
- Unity 版本：**2022.3.62f1c1**（建议使用完全一致的版本或同 2022.3 LTS 系列）
- 目标平台：Windows（64 位）独立版（本工程已按该平台配置）
- 系统：Windows 10 / 11 均可构建

## 3. 如何生成可执行版本（Build）
1. 打开 **Unity Hub** → “添加项目/Add project from disk” → 选择本 `source` 文件夹 → 打开。
2. 首次导入需要较长时间，等待编辑器进入主界面且右下角无编译报错。
3. 菜单 **File → Build Settings**：
   - 确认 Scenes In Build 按以下顺序且全部勾选：
     1. `Assets/Scenes/MainMenu.unity`
     2. `Assets/Scenes/Stage_1.unity`
     3. `Assets/Scenes/Stage_2.unity`
     4. `Assets/Scenes/Stage_3.unity`
   - Platform 选择 **Windows, Mac, Linux → Windows**（当前目标为 x86_64）。
4. （可选）**Player Settings** 中确认：
   - Product Name：`Caterpillar is Coming`
   - Company Name：`Pig vs Vegetable`
5. 点击 **Build**，选择输出目录（例如本工程外层 `release/` 下的新文件夹），
   等待构建完成。生成结果为一个 `.exe` + 同名 `_Data` 文件夹，运行时需整体保留。

## 4. 主要脚本
| 路径 | 说明 |
| --- | --- |
| Assets/Scripts/vine/Vine.cs | 藤蔓生长：头移动、鼠标控制方向、撞墙贴墙滑行、叶子生成 |
| Assets/Scripts/vine/OneWayLeaf.cs | 叶子单向平台（可自下穿过、自上踩住） |
| Assets/Scripts/vine/VineGrow.cs | （早期竖直藤蔓版本，可保留参考） |
| Assets/Scripts/Player/PlayerMove.cs | 毛毛虫移动 / 跳跃 / 落地判定 |
| Assets/Scripts/Player/PlayerAnim.cs | 玩家动画状态联动 |
| Assets/Scripts/Health/Health.cs 等 | 生命、受伤、死亡 |
| Assets/Scripts/Enemy/* | 敌人与生成器（含毛毛虫 Caterpillar） |
| Assets/Scripts/GameManager/gameManager.cs | 生长阶段 ↔ 玩家阶段 状态切换 |
| Assets/Scripts/GameEnd/* | 通关/失败面板与按钮逻辑 |
| Assets/Scripts/MainMenu/* | 主菜单 |
| Assets/Scripts/Audio/BgmManager.cs | 全局背景音乐 |
| Assets/Scripts/UI/Stage1Guide.cs | 第一关开局引导牌 |

## 5. 常见问题
- 打开工程报错：请先确认 Unity 版本为 2022.3.62f1c1，并等待包还原完成。
- 构建为空场景：检查 Build Settings 中的场景列表与顺序。
- 重新构建后请把新的 exe 与 `_Data` 一起复制到 `release/` 使用。

## 6. 素材来源与许可
详见外层根目录 `LICENSE.txt`（音乐：IndieDevs / OpenGameArt；
美术：SunnyLand Artwork 免费素材包 + 豆包 AI 生成；代码使用 AI 辅助优化）。
