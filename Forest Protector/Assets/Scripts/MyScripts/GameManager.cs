using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isGameRunning;
    [SerializeField]
    private static Transform ReleasedWeaponsStash;
    // Start is called before the first frame update
    void Awake()
    {
        isGameRunning=true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void addToReleasedWeaponStash(Transform weaponTransform)
    {
        weaponTransform.SetParent(ReleasedWeaponsStash);
    }
}
