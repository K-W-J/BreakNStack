using UnityEngine;

namespace Code.Blocks
{
    public class BlockGuide : MonoBehaviour
    {
        public void SetScale(Vector2 scale)
        {
            transform.localScale = new Vector2(scale.x, scale.y);
        }
    }
}
