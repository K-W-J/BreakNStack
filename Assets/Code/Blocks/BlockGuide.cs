using UnityEngine;

namespace Code.Blocks
{
    public class BlockGuide : MonoBehaviour
    {
        public void SetScale(Vector2 scale)
        {
            transform.localScale = new Vector2(scale.x, transform.localScale.y);
        }
        
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
        
        public void SetGuiding(bool isGuiding, Transform trm = null)
        {
            gameObject.SetActive(isGuiding);
            transform.SetParent(trm);
        }
    }
}
