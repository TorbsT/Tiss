using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class DoNotRotate : MonoBehaviour
    {
        private void Update()
        {
            ResetRotation();
        }
        private void ResetRotation()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
