using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltraMagnet
{
    public class MagnetScript : MonoBehaviour
    {
        public List<Magnet> magnets = new List<Magnet>();
        public event Action<Boolean> onMagnet;
        public bool isOnMagnet;

        int magnetRotationDirection;
        Rigidbody rb;


        Magnet GetTargetMagnet()
        {
            Magnet magnet = null;
            float num = float.PositiveInfinity;
            for (int i = 0; i < magnets.Count; i++)
            {
                Vector3 vector = magnets[i].transform.position - base.transform.position;
                float sqrMagnitude = vector.sqrMagnitude;
                if (sqrMagnitude < num)
                {
                    num = sqrMagnitude;
                    magnet = magnets[i];
                    Vector3 normalized = new Vector3(rb.velocity.z, rb.velocity.y, -rb.velocity.x).normalized;
                    if (Vector3.Dot(vector, normalized) > 0f)
                    {
                        magnetRotationDirection = -1;
                    }
                    else
                    {
                        magnetRotationDirection = 1;
                    }
                }
            }
            return magnet;
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void LateUpdate()
        {
            if (magnets.Count > 0)
            {
                magnets.RemoveAll((Magnet magnet) => magnet == null);
                if (magnets.Count == 0)
                {
                    return;
                }
                Magnet targetMagnet = GetTargetMagnet();
                if (!targetMagnet)
                {
                    return;
                }
                isOnMagnet = true;
                rb.velocity = Vector3.RotateTowards(rb.velocity, Quaternion.Euler(0f, (float)(ConfigManager.spinning.value * magnetRotationDirection), 0f) * (targetMagnet.transform.position - transform.position).normalized * rb.velocity.magnitude, float.PositiveInfinity, rb.velocity.magnitude);
            }
            else
            {
                isOnMagnet = false;
            }
        }
    }
}
