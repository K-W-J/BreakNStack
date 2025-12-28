using System.Collections.Generic;
using Code.Defines;
using UnityEngine;

namespace Code.Blocks
{
    [CreateAssetMenu(fileName = "BlockSO", menuName = "SO/Block", order = 0)]
    public class BlockSO : ScriptableObject
    {
        public int maxHealth;
        public int weight;
        public int destroyCount;
        public int stackCount;
        [Space]
        public string blockName;
        public Enums.BlockType blockType;
        [Space] 
        public Vector2 size;
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
        public List<Sprite> break_Sprites; //첫번째 인덱스부터 마지막 인덱스까지 부서지는 순서대로 스프라이트 넣기
        public bool isFlip;
        [Space]
        public GameObject colliderPrefab;
    }
}