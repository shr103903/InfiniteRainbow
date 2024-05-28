using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatusData
{
    public static int floor = 0;

    public static int[] hpUpgrade = new int[3];
    public static int[] atkUpgrade = new int[3];
    public static int[] defUpgrade = new int[3];
    public static int[] dodgeUpgrade = new int[3];
    public static int[] criChanceUpgrade = new int[3];
    public static int[] criMultiUpgrade = new int[3];
    public static int[] speedUpgrade = new int[3];

    public static void SetData()
    {
        hpUpgrade = new int[] { 0, 0, 0 };
        atkUpgrade = new int[] { 0, 0, 0 };
        defUpgrade = new int[] { 0, 0, 0 };
        dodgeUpgrade = new int[] { 0, 0, 0 };
        criChanceUpgrade = new int[] { 0, 0, 0 };
        criMultiUpgrade = new int[] { 0, 0, 0 };
        speedUpgrade = new int[] { 0, 0, 0 };
    }
}
