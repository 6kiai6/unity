using UnityEngine;

public interface IFactory<T>
{
    T Create();
}
