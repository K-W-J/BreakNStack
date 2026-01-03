using Code.Blocks;
using UnityEngine;

namespace Code
{
    public class BlockDeathZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            other.GetComponentInParent<Block>()?.DestroyBlock();
        }
    }
}