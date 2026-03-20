using UnityEngine;

public class BulletPools : PoolBase<BulletPools, PlayerWeaponBullet>
{

}

public class BulletPool : ObjPoolBase<PlayerWeaponBullet>
{

}


public class BulletEvent : EventCenterBase<PlayerWeaponBullet>
{

}
