using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _footBallZombie
{
    public class DropHead : MonoBehaviour
    {
        public void Damage()
        {
            Destroy(gameObject);
        }
    }
}
