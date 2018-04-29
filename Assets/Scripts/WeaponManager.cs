using UnityEngine.Networking;
using UnityEngine;

public class WeaponManager : NetworkBehaviour {
    private WepGraphics weaponGraphics;

    public WepGraphics GetCurrGraphics()
    {
        return weaponGraphics;
    }
}
