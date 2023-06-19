using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Components
{
    [ExecuteInEditMode]
    internal class ImageFillGradient : MonoBehaviour
    {
        [SerializeField] private Gradient gradient;
        [SerializeField] private Image image;

        private void Update()
        {
            Validate();
        }
        private void OnValidate()
        {
            Validate();
        }
        private void Validate()
        {
            if (image == null)
                image = GetComponent<Image>();
            float fillAmount = image.fillAmount;
            Color color = gradient.Evaluate(fillAmount);
            image.color = color;
        }
    }
}
