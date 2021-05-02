using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{ 
    // Start is called before the first frame update
    
    public void SelectWeapon(int index)
    {
        int i = 0;

        foreach(Transform weapon in transform)
        {
            if(i == index)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
