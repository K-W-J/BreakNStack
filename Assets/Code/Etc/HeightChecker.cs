using UnityEngine;

namespace Code.Etc
{
    public class HeightChecker : MonoBehaviour
    {
        private float _currentHeight;
        
        private void Update()
        {
            _currentHeight = transform.position.y;
        }
    }
}