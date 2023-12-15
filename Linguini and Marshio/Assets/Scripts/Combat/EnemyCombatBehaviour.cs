using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCombatBehaviour : MonoBehaviour
{
        public abstract IEnumerator EnemyMove(BattleSystem system);
}
