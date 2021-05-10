using UnityEngine;
public class Money : MonoBehaviour
{
        public int money;
        //Money is a script added to all entity with a link to money
        //      Players -> Money each player have
        //      Enemy -> Money each enemy drop on their death (see Health script for attribution)
        //      Turret -> Money needed to upgrade to the next level (on destroy give back a % of the price to the next level)
}