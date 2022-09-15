using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.List
{
    [CreateAssetMenu]
    public class ListSO : ScriptableObject
    {
        [field: SerializeField] public List<GameObject> lists { get; private set; }
        [field: SerializeField] public List<Vector2> initialPositions { get; private set; }

        private void OnEnable()
        {
            lists.Clear();
            initialPositions.Clear();
        }

        // Adiciona o gameobject em questão
        public void AddToList(GameObject gameObject, Vector2 pos)
        {
            if (lists.Contains(gameObject)) return;
            lists.Add(gameObject);
            if (initialPositions.Contains(pos)) return;
            initialPositions.Add(pos);
        }

        // remove o gameobject
        public void RemoveObject(GameObject gameObject, Vector2 pos)
        {
            lists.Remove(gameObject);
            initialPositions.Remove(pos);
        }

        // Retorna a lista
        public List<GameObject> GetList()
        {
            lists.Reverse();
            return lists;
        }
        public List<Vector2> GetListPos()
        {
            initialPositions.Reverse();
            return initialPositions;
        }
    }
}

