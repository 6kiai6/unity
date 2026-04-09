using System.Collections.Generic;
using UnityEngine;

public enum QuestObjectiveType
{
    [Tooltip("击杀敌人，累计到 targetCount；enemyTypeIdFilter 为 0 时任意敌人都计数")]
    KillEnemyCount = 0,
}

[System.Serializable]
public class QuestDefinition
{
    public string questId;
    public string title;

    public QuestObjectiveType objectiveType;
    [Min(1)]
    public int targetCount = 1;

    [Tooltip("仅 KillEnemy：0=任意敌人；与 EnemyBase.questEnemyTypeId 对应")]
    public int enemyTypeIdFilter;

    [Tooltip("完成后发放的道具（如枪械在 PackageTable 里配置为物品）")]
    public DialogueItemGrant[] rewards;

    [Tooltip("需先完成的任务 id，全部完成才会开始统计本任务")]
    public string[] requiredCompletedQuestIds;
}

[CreateAssetMenu(menuName = "Quest/QuestTable", fileName = "QuestTable")]
public class QuestTable : ScriptableObject
{
    public List<QuestDefinition> quests = new List<QuestDefinition>();
}
