using UnityEngine;
using UnityEngine.UI; // Required for UI components

namespace Art.UI.ProgressBar
{
    public class SimpleProgressBar : MonoBehaviour
    {
        [Header("UI Reference")] [SerializeField]
        private Image fillImage;
        // Drag your "ProgressBar_Fill" object here in the Inspector!

        [Header("Optional Settings")] [SerializeField]
        private Color fullColor = Color.green;

        [SerializeField] private Color emptyColor = Color.red;
        [SerializeField] private bool useColorGradient = true;

        /// <summary>
        /// Updates the bar fill based on min, max, and current value.
        /// </summary>
        public void SetProgress(float currentValue, float minValue, float maxValue)
        {
            // 1. Calculate the normalized percentage (0.0 to 1.0)
            float range = maxValue - minValue;
            float adjustedValue = currentValue - minValue;

            // Prevent divide by zero errors
            float percentage = (range != 0) ? adjustedValue / range : 0f;

            // 2. Clamp it (so the bar doesn't overfill or go negative)
            percentage = Mathf.Clamp01(percentage);

            // 3. Apply to the UI Image
            if (fillImage != null)
            {
                fillImage.fillAmount = percentage;

                // Optional: Change color based on health (Green -> Red)
                if (useColorGradient)
                {
                    fillImage.color = Color.Lerp(emptyColor, fullColor, percentage);
                }
            }
        }
    }
}