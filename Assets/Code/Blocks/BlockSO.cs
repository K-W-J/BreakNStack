using Code.Defines;
using UnityEngine;

namespace Code.Blocks
{
    [CreateAssetMenu(fileName = "BlockSO", menuName = "SO/Block", order = 0)]
    public class BlockSO : ScriptableObject
    {
        public int maxHealth;
        public int weight;
        [Space]
        public Enums.BlockType blockType;
        [Space] 
        public int maxHeight;
        public int minHeight;
        public int maxDistance;
        public int minDistance;
        [Space]
        public Sprite default_Sprite;
        public Sprite break_2_Sprite;
        public Sprite break_3_Sprite;
        public Sprite break_4_Sprite;
    }
}