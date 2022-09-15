using UnityEngine;
namespace RPG.UI.Dialogue
{

    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Object")]
    public class DialogueObject : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] [TextArea] private string[] _dialogue;

        public string Name => _name;
        public string[] Dialogue => _dialogue;
    }
}
