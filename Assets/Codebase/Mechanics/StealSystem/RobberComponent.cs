using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.StealSystem
{
    public class RobberComponent : MonoBehaviour, IRobber<int>
    {
        public List<int> StealedItems{get; private set;} = new List<int>();
        public bool Stealing{get; private set;} = false;
        private const float STEALING_BOOLEMENT_TIME = 1.5F;
        private float _stealingTimer;

        [ListDrawerSettings]
        public List<int> InventoryVisualizer=new List<int>();

        public void Steal(int item){
            StealedItems.Add(item);
            Stealing = true;
        }
        public bool CanSteal(string otherObjectTag)=>otherObjectTag=="Item";

        private void Start(){
            _stealingTimer = 0;
            InventoryVisualizer=StealedItems;
        }
        private void Update(){
            if(Stealing)
            {
                _stealingTimer+=Time.deltaTime;
                //Debug.Log($"{name} stealing is detectable");
                if(_stealingTimer>=STEALING_BOOLEMENT_TIME){
                    Stealing=false;
                    _stealingTimer=0;
                }
            }
        }
    } 
}