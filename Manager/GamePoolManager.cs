// using UnityEngine;
// using System.Collections.Generic;
// using System.Collections;



// public class ObjPoolManager : SingleMonoBase<ObjPoolManager>
// {
//     Dictionary<string, Stack<GameObject>> objectPools = new Dictionary<string, Stack<GameObject>>();

//     public GameObject GetObj(GameObject objPrefab){
//         if(!objectPools.ContainsKey(objPrefab.name)){
//             objectPools.Add(objPrefab.name, new Stack<GameObject>());
//         }

//         if(objectPools[objPrefab.name].Count == 0){
//             return Instantiate(objPrefab);
//         }
//         else{
//             GameObject obj = objectPools[objPrefab.name].Pop();
//             obj.name = objPrefab.name;
//             obj.SetActive(true);
//             return obj;
//         }
//     }

//     public GameObject GetObj(GameObject objPrefab, Vector3 position, Quaternion rotation){
//         if(!objectPools.ContainsKey(objPrefab.name)){
//             objectPools.Add(objPrefab.name, new Stack<GameObject>());
//         }
//         Debug.Log(objectPools[objPrefab.name].Count);
//         if(objectPools[objPrefab.name].Count == 0){
//             return Instantiate(objPrefab, position, rotation);
//         }
//         else{
//             GameObject obj = objectPools[objPrefab.name].Pop();
//             obj.name = objPrefab.name;
//             obj.transform.position = position;
//             obj.transform.rotation = rotation;
//             obj.SetActive(true);
//             return obj;
//         }
//     }


//     public void PushObj(GameObject objPrefab){
//         objPrefab.SetActive(false);
//         objectPools[objPrefab.name].Push(objPrefab);
//     }

//     public void ReleaseObj(GameObject objPrefab, float lifetime = 0f){
//         Debug.Log("Push");
//         if(lifetime == 0){
//             PushObj(objPrefab);
//         }
//         else{
//             StartCoroutine(ReleaseObjCoroutine(objPrefab, lifetime));
//         }
//     }

//     private IEnumerator ReleaseObjCoroutine(GameObject objPrefab, float lifetime){
//         yield return new WaitForSeconds(lifetime);
//         PushObj(objPrefab);
//     }

// }
