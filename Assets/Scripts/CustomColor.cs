using System;
using System.Reflection;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine;

public class CustomColor
{
    public static Color damage() { return new Color(1f, 0.75f, 0.33f); }
    public static Color miss() { return Color.yellow; }
    public static Color heal() { return new Color(0.33f, 1f, 0.5f); }
    public static Color invisible() { return new Color(1,1,1,0); }
    public static Color SP() { return new Color(0.75f, 0.75f, 1.00f); }
}
