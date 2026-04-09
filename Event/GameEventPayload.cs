using UnityEngine;

/// <summary>
/// 全局事件携带数据，供 EventCenterBase&lt;GameEventPayload&gt; 使用。
/// </summary>
[System.Serializable]
public struct GameEventPayload
{
    [Tooltip("通用整型：如敌人 questEnemyTypeId、物品 id 等")]
    public int intArg;

    [Tooltip("备用整型")]
    public int intArg2;

    [Tooltip("分类/标签等")]
    public string stringArg;
}
