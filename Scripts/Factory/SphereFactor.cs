using UnityEngine;
using UnityEngine.ProBuilder.Shapes;


[CreateAssetMenu(fileName = "SphereFactor", menuName = "Factory/SphereFactor")]
public class SphereFactor : FactorySO<Sphere>
{
    [SerializeField]
    private Sphere prefab;
    public override Sphere Create()
    {
        return Instantiate(prefab);
    }
}
