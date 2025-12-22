using UnityEngine;

namespace Code
{
    public class MoveToUp : MonoBehaviour
    {
        [SerializeField] private float _speed;
        
        private void Update()
        {
            transform.position += Vector3.up * (Time.deltaTime * _speed);
        }

        private void SetSpeed(float speed)
        {
            _speed = speed;
        }
    }
}