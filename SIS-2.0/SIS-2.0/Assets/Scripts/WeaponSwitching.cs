using UnityEngine;

//DEPRECATED

public class WeaponSwitching : MonoBehaviour
{ 
    // Start is called before the first frame update
    
    public GameObject SelectWeapon(int index)
    {
        int i = 0;
        GameObject returned = null;
        foreach(Transform weapon in transform)
        {
            if(i == index)
            {
                weapon.gameObject.SetActive(true);
                returned = weapon.gameObject;
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
        return returned;
    }
}
