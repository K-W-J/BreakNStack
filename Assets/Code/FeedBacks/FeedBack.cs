using UnityEngine;

namespace Code.FeedBacks
{
    public abstract class FeedBack : MonoBehaviour
    {
        public abstract void CreateFeedback();
        public abstract void StopFeedback();
    }
}