// UnityAdapter/Views/UnityUnitView.cs
using UnityEngine;
using Core.Model.Entities;
using Core.Model.Enums;

namespace UnityAdapter.Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class UnityUnitView : MonoBehaviour
    {
        [Header("Componentes Visuales")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private CircleCollider2D unitCollider;

        [Header("Barra de Vida UI")]
        [SerializeField] private HealthBarView healthBarView;

        public int UnitId { get; private set; }
        public int FactionId { get; private set; }
        public UnitType Type { get; private set; }

        private float currentHealth = 100f;
        private float maxHealth = 100f;
        private UnitModel _model;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (unitCollider == null) unitCollider = GetComponent<CircleCollider2D>();

            unitCollider.isTrigger = false;
            unitCollider.radius = 0.4f;
        }

        private void Start()
        {
            if (healthBarView == null)
            {
                CreateHealthBarDynamically();
            }
        }

        
        private void Update()
        {
            if (_model == null) return;

            UpdatePosition(_model.PositionX, _model.PositionY);
            UpdateVisuals(_model);
        }

        public void Initialize(UnitModel model, Sprite playerWarrior, Sprite enemyWarrior, Sprite playerBuilder, Sprite enemyBuilder)
        {
            _model = model;
            UnitId = model.Id;
            FactionId = model.FactionId;
            Type = model.Type;
            currentHealth = model.Health;
            maxHealth = model.MaxHealth;

            UpdatePosition(model.PositionX, model.PositionY);

            if (spriteRenderer != null)
            {
                bool isPlayer = (model.FactionId == 1);

                spriteRenderer.sprite = model.Type switch
                {
                    UnitType.Builder => isPlayer ? playerBuilder : enemyBuilder,
                    UnitType.Warrior => isPlayer ? playerWarrior : enemyWarrior,
                    _ => isPlayer ? playerWarrior : enemyWarrior
                };

                spriteRenderer.sortingOrder = 10;
            }

            if (healthBarView == null)
            {
                CreateHealthBarDynamically();
            }

            if (healthBarView != null)
            {
                healthBarView.UpdateHealth(currentHealth, maxHealth);
            }
        }

        private void CreateHealthBarDynamically()
        {
            Texture2D texture = Texture2D.whiteTexture;
            Sprite defaultSquare = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            GameObject barContainer = new GameObject("HealthBar");
            barContainer.transform.SetParent(transform, false);
            barContainer.transform.localPosition = new Vector3(0f, 0.75f, -2f);

            healthBarView = barContainer.AddComponent<HealthBarView>();

            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(barContainer.transform, false);
            bgObj.transform.localScale = new Vector3(1.0f, 0.15f, 1f);

            SpriteRenderer bgRenderer = bgObj.AddComponent<SpriteRenderer>();
            bgRenderer.sprite = defaultSquare;
            bgRenderer.color = Color.black;
            bgRenderer.sortingOrder = 20;

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(barContainer.transform, false);
            fillObj.transform.localPosition = new Vector3(-0.48f, 0f, -0.1f);

            GameObject fillSpriteObj = new GameObject("Sprite");
            fillSpriteObj.transform.SetParent(fillObj.transform, false);
            fillSpriteObj.transform.localPosition = new Vector3(0.48f, 0f, 0f);
            fillSpriteObj.transform.localScale = new Vector3(0.96f, 0.11f, 1f);

            SpriteRenderer fillRenderer = fillSpriteObj.AddComponent<SpriteRenderer>();
            fillRenderer.sprite = defaultSquare;
            fillRenderer.color = Color.green;
            fillRenderer.sortingOrder = 21;

            healthBarView.SetupComponents(fillObj.transform, fillRenderer, bgRenderer);
            healthBarView.UpdateHealth(currentHealth, maxHealth);
        }

        public void UpdatePosition(float x, float y)
        {
            transform.position = new Vector3(x, y, -1f);
        }

        public void UpdateVisuals(UnitModel model)
        {
            if (spriteRenderer == null) return;

            if (model.State == UnitState.Attack)
            {
                spriteRenderer.color = Color.red;
            }
            else if (model.State == UnitState.Moving)
            {
                spriteRenderer.color = Color.yellow; 
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }

        public void OnHealthChanged(float newHealth, float newMaxHealth)
        {
            currentHealth = newHealth;
            maxHealth = newMaxHealth;

            if (healthBarView != null)
            {
                healthBarView.UpdateHealth(currentHealth, maxHealth);
            }

            if (currentHealth <= 0f)
            {
                Debug.Log($"[RTS] Unidad {UnitId} destruida.");
                Destroy(gameObject);
            }
        }
    }
}