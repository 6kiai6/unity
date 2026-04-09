using System;
using UnityEngine;

/// <summary>
/// 监听 GameEventCenter，根据 QuestTable 校验进度并发放奖励（PackageManager）。
/// </summary>
public class QuestManager : SingleMonoBase<QuestManager>
{
    [SerializeField] private QuestTable questTable;

    public event Action<QuestDefinition> OnQuestCompleted;

    protected override void Awake()
    {
        base.Awake();
        QuestLocalData.Instance.Load();
    }

    private void OnEnable()
    {
        var hub = GameEventCenter.Instance;
        if (hub != null)
            hub.AddEvent(GameEventNames.EnemyKilled, OnEnemyKilled);
    }

    private void OnDisable()
    {
        var hub = GameEventCenter.Instance;
        if (hub != null)
            hub.RemoveEvent(GameEventNames.EnemyKilled, OnEnemyKilled);
    }

    private void OnEnemyKilled(GameEventPayload payload)
    {
        if (questTable == null || questTable.quests == null)
            return;

        int killedTypeId = payload.intArg;

        foreach (var def in questTable.quests)
        {
            if (def == null || string.IsNullOrEmpty(def.questId))
                continue;
            if (def.objectiveType != QuestObjectiveType.KillEnemyCount)
                continue;

            var progress = QuestLocalData.Instance.GetOrCreate(def.questId);
            if (progress.completed)
                continue;

            if (!ArePrerequisitesMet(def))
                continue;

            if (def.enemyTypeIdFilter != 0 && def.enemyTypeIdFilter != killedTypeId)
                continue;

            progress.currentCount++;
            TryComplete(def, progress);
        }

        QuestLocalData.Instance.Save();
    }

    private bool ArePrerequisitesMet(QuestDefinition def)
    {
        if (def.requiredCompletedQuestIds == null || def.requiredCompletedQuestIds.Length == 0)
            return true;

        foreach (var req in def.requiredCompletedQuestIds)
        {
            if (string.IsNullOrEmpty(req))
                continue;
            if (!QuestLocalData.Instance.IsCompleted(req))
                return false;
        }

        return true;
    }

    private void TryComplete(QuestDefinition def, QuestRuntimeProgress progress)
    {
        if (progress.currentCount < def.targetCount)
            return;

        progress.completed = true;
        GrantRewards(def);
        OnQuestCompleted?.Invoke(def);
    }

    private static void GrantRewards(QuestDefinition def)
    {
        if (def.rewards == null || def.rewards.Length == 0)
            return;

        var pm = PackageManager.Instance;
        if (pm == null)
        {
            Debug.LogWarning("QuestManager: 未找到 PackageManager，无法发放任务奖励");
            return;
        }

        foreach (var g in def.rewards)
        {
            if (g == null || g.count <= 0)
                continue;
            pm.TryAddItem(g.itemId, g.count);
        }
    }

    /// <summary>用于调试或读档后刷新 UI</summary>
    public QuestRuntimeProgress GetProgress(string questId)
    {
        return QuestLocalData.Instance.GetOrCreate(questId);
    }
}
