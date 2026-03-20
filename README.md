# 项目代码总览与流程说明

## 一、项目结构概览

```
Assets/Scripts/
├── Base/                       # 基类与框架
│   ├── SingleMonoBase.cs       # 单例 MonoBehaviour 基类
│   ├── StateBase.cs            # 状态机状态基类
│   ├── PlayerStateBase.cs      # 玩家状态基类
│   ├── EnemyStateBase.cs       # 敌人状态基类
│   ├── EnemyBase.cs            # 敌人基类
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
│   ├── PlayerWeapon.cs         # 武器发射（当前仍为 Instantiate）
│   ├── PlayerWeaponBullet.cs   # 子弹逻辑与碰撞
│   └── State/
│       ├── PlayerIdelState.cs
│       ├── PlayerMoveState.cs
│       ├── PlayerHoverState.cs
│       └── PlayerAmingState.cs
├── Enemy/
│   ├── ZombieEnemy.cs
│   ├── Spherekk.cs             # 占位（可与子弹 Tag 交互扩展）
│   └── ZombieState/
│       ├── ZombieIdleState.cs
│       ├── ZombieMoveState.cs
│       ├── ZombieAttackState.cs
│       └── ZombieDeadState.cs
└── UI/
    └── EnemyHealthBarUI.cs
```

---

## 二、整体运行流程

```
游戏启动
    │
    ├─► SingleMonoBase 单例初始化
    │       PlayerController / GameManager / MonoManager / UIManager
    │       PoolBase 派生类：BulletPools、EnemyPool（若场景挂载且先于访问初始化）
    │       EventCenterBase 派生：BulletEvent（若使用）
    │       ObjPoolsBase（若场景挂载）
    │
    ├─► PlayerModel / EnemyBase：Awake 内创建 StateMachine，组件引用就绪
    │
    ├─► PlayerController.Start：当前角色 Enter()、相机绑定、ExitAim()
    ├─► PlayerModel.Start：SwitchState(Idle)、ExitAim()
    ├─► EnemyBase.Start：SwitchState(Idle)、FindAttackTarget()、生成血条
    │
    ├─► Enemymanger（可选）：玩家 Tag 停留在触发器内时，按间隔调用
    │       EnemyPool.Instance.GetObj(enemyPrefab) 生成僵尸，并累计 enemyCount
    │
    └─► 每帧
            PlayerController：读输入 → worldMovement / localMovement → 角色切换
            MonoManager：Invoke 已注册的各状态 Update
            EnemyBase：血条显示计时（未死亡时）
```

---

## 三、玩家流程（状态机）

### 3.1 玩家状态枚举与切换

| 状态       | 类名              | 触发条件 |
|------------|-------------------|----------|
| Idle       | PlayerIdelState   | 默认进入；移动输入为 0 时从 Move 切回 |
| Move       | PlayerMoveState   | 有移动输入 或 非当前控制角色距离 > stoppingDistance |
| Hover      | PlayerHoverState  | 跳跃输入 或 离地且满足悬空判定 |
| Aming      | PlayerAmingState  | 瞄准或攻击按下且当前被控制 |

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

- **输入**：`PlayerController.Update` 从 `MyInputSystem` 读取 Move / Sprint / Attack / Aim / Jump，并计算相对相机的 `worldMovement`、`localMovement`。
- **当前控制角色**：`PlayerController.currentPlayerModel`；用数字键 1/2 可 `SwitchPlayerModel` 切换，非当前角色用 NavMeshAgent 跟随当前角色。
- **状态 Update**：通过 `PlayerStateBase.Enter` 向 `MonoManager` 注册 `Update`，在 `MonoManager.Update` 中统一执行。
- **重力与悬空**：在 `PlayerStateBase.Update` 中根据 `IsGroundNear` 累加 `verticalSpeed`，满足 `IsHover()` 时切到 Hover；Hover 中 `IsGroundNear()` 为真时落地回 Idle。
- **瞄准**：Aming 状态下 `EnterAim/ExitAim` 切换 Cinemachine 相机优先级与 IK 权重，`UpdateAimTarget` 用屏幕中心射线更新 `aimTarget.position`。
- **射击**：Aming 下 `attackInput` 时调用 `playerModel.weapon.Fire(aimTarget.position)`，生成子弹与枪口火花；子弹用 Rigidbody 直线飞行。

---

## 四、武器与子弹流程

```
PlayerAmingState.Update
    │
    └─ playerController.attackInput == true
            │
            └─ PlayerWeapon.Fire(aimTarget.position)
                    │
                    ├─ 间隔检测 bulletInterval
                    ├─ 计算方向 dirction，Instantiate 子弹 + 枪口火花
                    └─ 设置 bullet.forward、Spark.forward
                            │
                            ▼
                    PlayerWeaponBullet
                    ├─ Start：rb.linearVelocity = forward * flyPower，Destroy(lifeTime)
                    ├─ CheckInltOverlap：生成时 OverlapSphere 检测 Tag "Enemy"，命中则 Hurt + 销毁
                    └─ Update：CheckCollision 用 Raycast(prevPosition → position) 检测 "Enemy"，命中则 Hurt + 销毁
                            │
                            ▼
                    EnemyBase.Hurt(bullet)
                    ├─ 播放 Hit 动画、减速 SlowMoveSpeed
                    ├─ 生成血液特效
                    ├─ currentHealth -= bullet.damage，更新血条、healthBarShowTimer
                    └─ currentHealth <= 0 → SwitchState(Dead)，禁用 NavMeshAgent/Collider，Destroy(healthBar)
```

---

## 五、敌人流程（以僵尸为例）

### 5.1 敌人状态枚举

- **Idle** / **Move** / **Attack** / **Dead**

### 5.2 僵尸状态切换（ZombieEnemy + 各 State）

- **Start**：先进入 `Idle`，`FindAttackTarget()` 选最近玩家为 `attackTarget`，在 `UIManager.WorldSpaceCanvas` 下生成血条。
- **ZombieIdleState**：若**不在**攻击距离内 → `SwitchState(Move)`。
- **ZombieMoveState**：`ChaseAttackTarget()` 设 NavMesh 目标；若**在**攻击距离内 → `SwitchState(Idle)`（当前实现中未切到 Attack）。
- **ZombieAttackState**：仅播放 Attack 动画（由其他逻辑切入，或预留）。
- **ZombieDeadState**：播放 Dead 动画，`IsAnimationBreak()` 为真时 `enemyBase.Clear()`（Stop 状态机并 Destroy 自身）。

### 5.3 敌人受击与死亡

- **Hurt(bullet)**：见上面「武器与子弹流程」。
- **血条**：`EnemyHealthBarUI` 在 WorldSpaceCanvas 下，随 `healthBarTransform` 位置更新，面向相机；受伤时显示一段时间 `healthBarShowTime`。

---

## 六、数据与依赖关系

| 单例/核心类       | 职责                         | 被依赖方 |
|-------------------|------------------------------|----------|
| GameManager       | 持有 `playerModels[]`         | PlayerController 切换角色；EnemyBase 寻敌 |
| PlayerController  | 输入、相机、当前角色         | 所有 PlayerStateBase、PlayerModel |
| MonoManager       | 统一执行状态机 Update        | PlayerStateBase、EnemyStateBase |
| UIManager         | WorldSpaceCanvas             | EnemyBase 血条父节点 |
| StateMachine      | 状态缓存、EnterState/LoadState | PlayerModel、EnemyBase |

---

## 七、流程小结（按“玩一局”的顺序）

1. **启动**：单例与状态机初始化，玩家默认 Idle，敌人默认 Idle 并寻敌、生成血条。
2. **输入**：PlayerController 每帧读输入，计算移动方向；可选切换 1/2 号角色。
3. **玩家状态**：Idle ↔ Move（移动/跟随）、Idle/Move → Hover（跳跃/离地）、任意 → Aming（瞄准/攻击）。
4. **瞄准与射击**：Aming 下射线更新准心，按下攻击则 Weapon.Fire → 生成子弹，子弹碰撞/Overlap 检测到 "Enemy" 调用 Hurt。
5. **敌人**：Hurt 扣血、播受击、减速、更新血条；血量≤0 进 Dead，动画结束后 Clear 销毁。
6. **血条**：敌人生成时挂在 UIManager 的世界画布下，受伤后显示一段时间，死亡时销毁。

以上即为当前项目从框架、玩家、武器到敌人的整体代码总览与运行流程。

