using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public static ArenaManager arenaManager;
    public GameObject arena;

    void Awake()
    {
        arenaManager = this;
    }
}
