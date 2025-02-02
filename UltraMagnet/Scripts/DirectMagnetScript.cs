using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltraMagnet
{
    public class DirectMagnetScript : MonoBehaviour
    {
        public List<Magnet> magnets = new List<Magnet>();
        public Rigidbody rb;
        private void Awake()
        {
            rb = base.GetComponent<Rigidbody>();

        }
        void FixedUpdate()
        {
            this.rb.velocity = base.transform.forward * 150f;
            if (this.magnets.Count > 0)
            {
                int j = this.magnets.Count - 1;
                while (j >= 0)
                {
                    if (this.magnets[j] == null)
                    {
                        this.magnets.RemoveAt(j);
                        j--;
                    }
                    else
                    {
                        base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(this.magnets[j].transform.position - base.transform.position), Time.fixedDeltaTime * 180f);
                        break;
                    }
                }
            }
        }
    }
}
