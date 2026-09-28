// UnityAdapter/Views/HealthBarView.cs
using UnityEngine;

namespace UnityAdapter.Views
{
    public class HealthBarView : MonoBehaviour
    {
        [Header("Componentes Visuales")]
        [SerializeField] private Transform fillTransform;
        [SerializeField] private SpriteRenderer fillRenderer;
        [SerializeField] private SpriteRenderer backgroundRenderer;

        [Header("Configuración de Colores")]
        [SerializeField] private Color fullHealthColor = Color.green;
        [SerializeField] private Color mediumHealthColor = Color.yellow;
        [SerializeField] private Color lowHealthColor = Color.red;

        public void SetupComponents(Transform fill, SpriteRenderer fillRend, SpriteRenderer bgRend)
        {
            fillTransform = fill;
            fillRenderer = fillRend;
            backgroundRenderer = bgRend;
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            if (maxHealth <= 0f || fillTransform == null) return;

            float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);

     
            fillTransform.localScale = new Vector3(healthPercent, 1f, 1f);

      
            if (fillRenderer != null)
            {
                if (healthPercent > 0.5f)
                {
                    fillRenderer.color = Color.Lerp(mediumHealthColor, fullHealthColor, (healthPercent - 0.5f) * 2f);
                }
                else
                {
                    fillRenderer.color = Color.Lerp(lowHealthColor, mediumHealthColor, healthPercent * 2f);
                }
            }
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}