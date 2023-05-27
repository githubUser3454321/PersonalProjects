using UnityEngine;
using UnityEngine.Events;

namespace InvisibleMouse
{

    internal class Controller
    {
        [Serializable]
        public struct Gesture
        {
            // Name of stored Pose
            public string name;

            // Bone vector space positional list
            public List<Vector3> fingerDatas;

            // Unity recognition event
            public UnityEvent onRecognized;

        }
    }
}
