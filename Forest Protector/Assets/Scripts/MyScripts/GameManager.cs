using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Transform ReleasedWeaponsStash;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void addToReleasedWeaponStash(Transform weaponTransform)
    {
        weaponTransform.SetParent(ReleasedWeaponsStash);
    }
}
