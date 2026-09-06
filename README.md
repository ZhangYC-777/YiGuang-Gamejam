# 虫来 | YiGuang Game Jam

主题为「生长」的 Unity Game Jam 项目：先操控藤蔓搭建通路，再控制角色沿通路跳跃、躲避并前进。

游戏名：**虫来**。菜单副标题：「毛毛虫出击！」。

团队：策划 **膨胀面包边**；程序 **张一测**、**LyzMing**。

## 打开工程

使用 **Unity 2022.3.62f1c1** 打开 `YiGuangGameJam/`。源工程仍在原位置，整理提交材料没有移动或修改游戏资源。

编译步骤见 [source/README.md](source/README.md)，操作与运行说明见 [release/README.txt](release/README.txt)。

## 提交材料

依据用户提供的《如何提交游戏至官网(3).pdf》第 21–26 页整理。

| 仓库路径 | 用途 |
| --- | --- |
| `YiGuangGameJam/` | 日常开发的 Unity 工程 |
| `source/README.md` | 打开及编译工程的说明 |
| `release/` | 各平台运行包与运行说明，目前尚缺运行包 |
| `press/` | 已有主菜单截图、截图与视频要求 |
| `other/` | 官网文案、团队资料、素材来源及提交核对清单 |
| `license.txt` | 授权待确认说明，尚非正式许可 |
| `tools/package_submission.py` | 将当前源工程与材料整理为提交草稿 ZIP |

## 生成提交草稿

在仓库根目录使用 Python 3 执行：

```sh
python3 tools/package_submission.py
```

输出至 `YiGuangGameJam/Builds/Submission/`（已被 Git 忽略），每次生成独立的带时间戳草稿。打包读取当前已保存的文件，包括尚未提交的源文件；请先在 Unity 保存场景和资源，打包时暂停修改。

ZIP 中的结构为：

```text
YiGuangGameJam/
  license.txt
  source/
    README.md
    YiGuangGameJam/
      Assets/
      Packages/
      ProjectSettings/
  release/
  press/
  other/
```

打包会保留素材与 `.meta`，不包含 Unity 编辑器、Library、Temp、Logs、个人设置或 Git 历史。仓库保持原工程路径，只有导出的 ZIP 将工程放进 `source/`。

**当前草稿不能视为提交就绪。** 还需补齐运行包、关卡截图、正式授权、素材来源和团队资料，并从解压后的源工程重新构建、运行验证。详见 [提交核对清单](other/SUBMISSION_CHECKLIST.md)。
