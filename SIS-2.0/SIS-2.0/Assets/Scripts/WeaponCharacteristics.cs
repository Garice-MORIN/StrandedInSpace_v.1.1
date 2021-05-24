using UnityEngine;

/* Class used to give weapons their charachteristics */

public enum Action
{
    RELOAD,
    SHOOT,
    PULSE
}

public class WeaponCharacteristics : MonoBehaviour
{
    public Animator animator;
    public int munitions;
    public float reloadSpeed;
    public int damage;
    public float fireRate;
    public int currentAmmo;
    public bool isPulse = false;

    public void UpdateAmmo(bool reload = false,int munition = -1)
    {
        if(reload)
        {
            currentAmmo = munition;
        }
        else
        {
            currentAmmo += munition;
        }
        
    }
}
