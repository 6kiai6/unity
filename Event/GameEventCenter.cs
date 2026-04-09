using UnityEngine;

/// <summary>
/// 全局事件中心，继承 EventCenterBase&lt;GameEventPayload&gt;。
/// 场景中挂一个即可，通过 GameEventCenter.Instance 派发/订阅。
/// </summary>
public class GameEventCenter : EventCenterBase<GameEventPayload>
{
}
