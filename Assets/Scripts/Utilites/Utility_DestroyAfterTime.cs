using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class Utility_DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] float timeUntilDestroyed = 3;

        private void Awake()
        {
            Destroy(gameObject, timeUntilDestroyed);
        }
    }
}
