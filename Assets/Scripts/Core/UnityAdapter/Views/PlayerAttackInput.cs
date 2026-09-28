using UnityEngine;
using Core.Controllers;
using UnityAdapter.Views;

public class PlayerAttackInput : MonoBehaviour
{
    public CoreGameController gameController;
    public int myUnitId = 1; 

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPos);

            if (hitCollider != null)
            {
                UnityUnitView enemyView = hitCollider.GetComponent<UnityUnitView>();
                if (enemyView != null)
                {
                
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