using UnityEngine;
using Core.Controllers;
using UnityAdapter.Views;

public class PlayerAttackInput : MonoBehaviour
{
    public CoreGameController gameController;
    public int myUnitId = 1; // ID de tu unidad de prueba

    void Update()
    {
        // Al presionar la letra 'E'
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPos);

            if (hitCollider != null)
            {
                UnityUnitView enemyView = hitCollider.GetComponent<UnityUnitView>();
                if (enemyView != null)
                {
                    // Orden de ataque usando el UnitId correcto
                    gameController.OrderAttackUnit(myUnitId, enemyView.UnitId);
                    Debug.Log($"¡Orden enviada a CombatController contra enemigo ID: {enemyView.UnitId}!");
                }
                else
                {
                    Debug.LogWarning("El objeto seleccionado tiene collider pero no contiene el componente UnityUnitView.");
                }
            }
            else
            {
                Debug.Log("No hay ningún colisionador bajo el cursor del mouse.");
            }
        }
    }
}