using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType : MonoBehaviour
{
    public Type type;
}
public enum Type
{
    FLYING,
    NORMAL,
    HEAVY,
    BOSS
}
