// using UnityEngine;
// using System.Collections.Generic;

// public class DataManager {
//     private static Dictionary<string, object> _prefabs = new Dictionary<string, object>();

//     public static Dictionary<string, object> Prefabs => _prefabs;

//     public void 

//     public void Destroy(GameObject go) {
//         if (go == null)
//             return;

//         Poolable poolable = go.GetComponent<Poolable>();
//         if (poolable != null) {
//             Managers.Pool.Push(poolable);
//             return;
//         }

//         Object.Destroy(go);
//     }
// }
