using UnityEngine;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.StealSystem
{
    public class ItemMemento : MonoBehaviour
    {
        [Title("В радиусе красного круга НЕ ДОЛЖНО быть других поднимаемых объектов")]
        [SerializeField]public int ItemID=0;
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 2f);
        }
    }
}