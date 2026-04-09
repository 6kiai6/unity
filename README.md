# 项目代码总览与流程说明

Unity **6000.3.10f1**（见 `ProjectSettings/ProjectVersion.txt`），URP + Input System + Cinemachine。下文与 `Assets/Scripts` 下 **61** 个 C# 脚本一致。

---

## 一、项目结构概览

```
Assets/Scripts/
├── Base/                       # 基类与框架
│   ├── SingleMonoBase.cs       # 单例 MonoBehaviour 基类
│   ├── StateBase.cs            # 状态机状态基类
│   ├── PlayerStateBase.cs      # 玩家状态基类
│   ├── EnemyStateBase.cs       # 敌人状态基类
│   ├── EnemyBase.cs            # 敌人基类（含 questEnemyTypeId、死亡时派发 EnemyKilled）
│   ├── PoolBase.cs             # 泛型对象池基类（Stack + inactiveRoot）
│   ├── ObjPoolsBase.cs         # 通用 GameObject 多预制体池 + ObjPoolBase<T> 壳
│   └── EventCenterBase.cs      # 泛型事件中心（string + UnityAction<T>）
├── Utils/
│   └── StateMachine.cs         # 通用状态机
├── Manager/
│   ├── GameManger.cs           # 游戏管理器（玩家模型列表）
│   ├── MonoManager.cs          # 统一 Update 派发
│   ├── UIManager.cs            # UI（世界空间画布）
│   ├── GamePoolManager.cs      # 空壳（待扩展）
│   └── ObjPoolManager.cs       # 整段注释（旧版对象池实现）
├── Other/
│   ├── BulletPool.cs           # BulletPools / BulletPool / BulletEvent（子弹池与事件类占位）
│   ├── EnemyPool.cs            # 敌人对象池 EnemyPool : PoolBase<EnemyPool, ZombieEnemy>
│   └── Enemymanger.cs          # 触发器内定时从 EnemyPool 取僵尸（拼写为 manger）
├── Factory/
│   ├── IFactory.cs             # 工厂接口 Create()
│   ├── FactorySO.cs            # ScriptableObject 工厂基类
│   ├── SphereFactor.cs         # 示例：CreateAssetMenu 创建 Sphere
│   └── Sphere.cs               # 示例产品（空脚本）
├── Player/
│   ├── PlayerController.cs     # 输入、相机、切换角色
│   ├── PlayerModel.cs          # 角色模型 + 状态机
│   ├── PlayerWeapon.cs         # 武器发射；装饰者管道 + 可选 WeaponBuffTable 药水
│   ├── PlayerWeaponBullet.cs   # 子弹逻辑与碰撞；ApplyWeaponStats 写入最终伤害/初速
│   └── State/
│       ├── PlayerIdelState.cs
│       ├── PlayerMoveState.cs
│       ├── PlayerHoverState.cs
│       └── PlayerAmingState.cs
├── Enemy/
│   ├── ZombieEnemy.cs
│   ├── Spherekk.cs             # 可与子弹等玩法扩展
│   └── ZombieState/
│       ├── ZombieIdleState.cs
│       ├── ZombieMoveState.cs
│       ├── ZombieAttackState.cs
│       └── ZombieDeadState.cs  # 动画结束后 EnemyPool.ReleaseObj（回池）
├── UI/
│   ├── BasePanel.cs            # 面板基类；Close 时从 UISettings.panelDict 移除并 Destroy
│   ├── UISettings.cs           # 单例；pathDict + OpenPanel/ClosePanel（Resources/Prefab/Panel/...）
│   └── EnemyHealthBarUI.cs     # 世界空间血条
├── Package/                    # 背包：静态表 + 本地存档 + 运行时入口
│   ├── PackageTable.cs         # ScriptableObject 物品定义列表
│   ├── PackageLocalData.cs     # PlayerPrefs 持久化 PackageLocalItem
│   └── PackageManager.cs       # TryAddItem / TryRemoveItem、OnInventoryChanged
├── Dialogue/                   # 对话
│   ├── DialogueData.cs         # DialogueNode / Choice / DialogueItemGrant
│   ├── DialogueSystem.cs       # 流程控制；可选 UISettings 打开 DialoguePanel
│   ├── DialoguePanel.cs        # 继承 BasePanel 的对话 UI
│   └── DialogueInteractable.cs # Trigger + Interact 触发对话
├── Event/                      # 全局事件（EventCenterBase<GameEventPayload>）
│   ├── GameEventPayload.cs
│   ├── GameEventNames.cs       # 如 EnemyKilled
│   └── GameEventCenter.cs
├── Quest/                      # 任务（订阅击杀等事件）
│   ├── QuestTable.cs           # ScriptableObject 任务定义
│   ├── QuestLocalData.cs       # 进度存档
│   └── QuestManager.cs         # 完成时 PackageManager 发奖
└── Weapon/                     # 武器数值装饰者管道
    ├── WeaponStatSnapshot.cs
    ├── IWeaponStatisticsPipeline.cs
    ├── WeaponStatsRoot.cs
    ├── WeaponStatDecorator.cs
    ├── DamageMultiplierWeaponDecorator.cs
    ├── FireIntervalMultiplierWeaponDecorator.cs
    ├── ProjectileSpeedMultiplierWeaponDecorator.cs
    ├── ConfiguredWeaponBuffDecorator.cs
    ├── WeaponBuffTable.cs      # itemId → 乘数与持续时间
    └── IWeaponStatisticsPipelineProvider.cs  # MonoBehaviour 扩展包一层管道
```

**UI 预制体约定**：`Resources/Prefab/Panel/{pathDict 配置的路径}`，例如对话面板 `Dialogue/DialoguePanel`（根物体挂 `DialoguePanel`）。

---

## 二、整体运行流程

```
游戏启动
    │
    ├─► SingleMonoBase 单例初始化
    │       PlayerController / GameManager / UIManager / UISettings / …
    │       PoolBase 派生：EnemyPool（若场景挂载）
    │       GameEventCenter、QuestManager、PackageManager、DialogueSystem（若使用对应玩法）
    │
    ├─► PlayerModel / EnemyBase：Awake 内创建 StateMachine，组件引用就绪
    │
    ├─► PlayerController.Start：当前角色 Enter()、相机绑定、ExitAim()
    ├─► PlayerModel.Start：SwitchState(Idle)、ExitAim()
    ├─► EnemyBase.Start：SwitchState(Idle)、FindAttackTarget()、生成血条
    │
    ├─► Enemymanger（可选）：玩家在触发器内时，按间隔 EnemyPool.GetObj 生成僵尸
    │
    └─► 每帧
            PlayerController：读输入 → worldMovement / localMovement → 角色切换
            MonoManager：Invoke 已注册的状态 Update
            EnemyBase：血条显示计时（未死亡时）
            PlayerWeapon：可选 Update 递减药水 Buff 剩余时间
```

**扩展数据流（与主循环并行）**

- **击杀 → 任务**：`EnemyBase` 死亡 → `GameEventCenter.TriggerEvent(EnemyKilled)` → `QuestManager` 累加 → 完成则 `PackageManager.TryAddItem`。  
- **对话发奖**：`DialogueSystem` 中 `DialogueItemGrant` → `PackageManager`。  
- **打开 UI 面板**：`UISettings.OpenPanel(UIConst.xxx)` → 实例化 `BasePanel` 子类并写入 `panelDict`。

---

## 三、玩家流程（状态机）

### 3.1 玩家状态枚举与切换

| 状态   | 类名              | 触发条件 |
|--------|-------------------|----------|
| Idle   | PlayerIdelState   | 默认进入；移动输入为 0 时从 Move 切回 |
| Move   | PlayerMoveState   | 有移动输入或非当前控制角色距离 > stoppingDistance |
| Hover  | PlayerHoverState  | 跳跃输入或离地且满足悬空判定 |
| Aming  | PlayerAmingState  | 瞄准或攻击按下且当前被控制 |

### 3.2 玩家状态流转图

```
                    ┌─────────────┐
                    │    Idle     │
                    └──────┬──────┘
        移动输入/跟随距离  │  跳跃  │  瞄准/攻击
                    ▼      │      ▼
              ┌─────────┐   │  ┌─────────┐     ┌─────────┐
              │  Move   │───┘  │  Hover  │     │  Aming  │
              └────┬────┘      └────┬────┘     └────┬────┘
                   │                │                │
                   │ 停止移动/到达  │ 落地           │ 松开瞄准/攻击
                   └────────────────┴────────────────┘
                                    │
                                    ▼
                            ┌─────────────┐
                            │    Idle     │
                            └─────────────┘
```

### 3.3 玩家逻辑要点

- **输入**：`PlayerController.Update` 从 `MyInputSystem` 读取 Move / Sprint / Attack / Aim / Jump 等，并计算相对相机的 `worldMovement`、`localMovement`。
- **当前控制角色**：`PlayerController.currentPlayerModel`；数字键 1/2 可 `SwitchPlayerModel`。
- **状态 Update**：`PlayerStateBase` 通过 `MonoManager` 注册，在 `MonoManager.Update` 中统一执行。
- **重力与悬空**：`PlayerStateBase.Update` 中根据 `IsGroundNear` 累加 `verticalSpeed`；`IsHover()` 时切 Hover，落地回 Idle。
- **瞄准**：Aming 下 `EnterAim/ExitAim` 切换 Cinemachine 与 IK；`UpdateAimTarget` 更新 `aimTarget.position`。
- **射击**：Aming 下 `attackInput` 时 `playerModel.weapon.Fire(aimTarget.position)`（见下节武器管道）。

---

## 四、武器与子弹流程

```
PlayerAmingState.Update
    │
    └─ playerController.attackInput == true
            │
            └─ PlayerWeapon.Fire(aimTarget.position)
                    │
                    ├─ GetCurrentStats()：WeaponStatsRoot + 各层 ConfiguredWeaponBuffDecorator + 可选 IWeaponStatisticsPipelineProvider
                    ├─ 间隔检测 stats.fireInterval（相对 Time.time）
                    ├─ Instantiate 子弹 + 枪口火花，设置 forward
                    └─ bullet.ApplyWeaponStats(stats.damage, stats.projectileSpeed)
                            │
                            ▼
                    PlayerWeaponBullet
                    ├─ Start：rb.linearVelocity = forward * flyPower（与 ApplyWeaponStats 一致），Destroy(lifeTime)
                    ├─ CheckInltOverlap：OverlapSphere 检测 Tag "Enemy" → Hurt + 销毁
                    └─ Update：Raycast(prev→curr) 检测 "Enemy" → Hurt + 销毁
                            │
                            ▼
                    EnemyBase.Hurt(bullet)
                    ├─ 播放 Hit、减速、血液特效
                    ├─ currentHealth -= bullet.damage，更新血条
                    └─ currentHealth <= 0 → SwitchState(Dead)，禁用 NavMeshAgent/Collider，Destroy(healthBar)
                            │
                            └─ NotifyEnemyKilledForQuests() → GameEventCenter.TriggerEvent(EnemyKilled)
```

**药水**：`WeaponBuffTable` 配置 `itemId` 与乘数；`PlayerWeapon.TryApplyWeaponBuffFromInventory` 会 `PackageManager.TryRemoveItem` 后叠一层 Buff（详见 `WeaponBuffTable.cs` 内持续时间约定）。

---

## 五、敌人流程（以僵尸为例）

### 5.1 敌人状态枚举

Idle / Move / Attack / Dead

### 5.2 僵尸状态切换（ZombieEnemy + 各 State）

- **Start**：进入 Idle，`FindAttackTarget()`，在 `UIManager.WorldSpaceCanvas` 下生成血条。
- **ZombieIdleState**：不在攻击距离内 → Move。
- **ZombieMoveState**：追击；在进入攻击距离时切回 Idle（当前逻辑未自动切 Attack，可按需扩展）。
- **ZombieAttackState**：攻击动画（预留/由其他逻辑切入）。
- **ZombieDeadState**：播放 Dead；`IsAnimationBreak()` 为真时 **`EnemyPool.Instance.ReleaseObj(zombie)`** 回池（非 `Clear()` 销毁，与池中复用一致）。

### 5.3 敌人受击与死亡

- **Hurt(bullet)**：见第四节；血条由 `EnemyHealthBarUI` 显示在世界画布下。
- **任务统计**：血量归零分支内派发全局击杀事件，供 `QuestManager` 使用；`questEnemyTypeId` 与 `QuestDefinition.enemyTypeIdFilter` 对应（0 常表示不区分类型，以表配置为准）。

---

## 六、数据与依赖关系

| 单例/核心类        | 职责 | 被依赖方 |
|--------------------|------|----------|
| GameManager        | `playerModels[]` | PlayerController、EnemyBase 寻敌 |
| PlayerController   | 输入、相机、当前角色 | 各 PlayerState、PlayerModel |
| MonoManager        | 统一状态 Update | PlayerStateBase、EnemyStateBase |
| UIManager          | WorldSpaceCanvas | EnemyBase 血条父节点 |
| UISettings         | 面板路径、打开/关闭、panelDict | BasePanel、DialogueSystem |
| PackageManager     | 背包增删、校验 PackageTable | Dialogue、Quest、PlayerWeapon（药水） |
| GameEventCenter    | 全局事件 EnemyKilled 等 | QuestManager；EnemyBase |
| QuestManager       | 任务进度、完成发奖 | 需场景挂载 + QuestTable 引用 |
| DialogueSystem     | 对话流程、可选弹 DialoguePanel | DialoguePanel、DialogueInteractable |
| StateMachine       | EnterState / LoadState | PlayerModel、EnemyBase |

---

## 七、流程小结（按「玩一局」顺序）

1. **启动**：单例就绪；玩家 Idle；敌人 Idle 并寻敌、生成血条。  
2. **输入**：PlayerController 读输入、切人。  
3. **玩家状态**：Idle ↔ Move、Hover、Aming。  
4. **瞄准与射击**：Aming 下 Fire → **装饰者管道** 得到伤害/间隔/弹速 → 生成子弹并 **ApplyWeaponStats**。  
5. **敌人**：Hurt 扣血；死亡切 Dead → 动画结束后 **EnemyPool 回收**；同时 **派发击杀事件**（任务用）。  
6. **血条**：挂世界画布，受伤显示一段时间，死亡时销毁。  
7. **可选**：对话/UISettings 面板、背包、任务、喝药水改枪——均依赖对应管理器在场景中存在且 SO 配置完整。

