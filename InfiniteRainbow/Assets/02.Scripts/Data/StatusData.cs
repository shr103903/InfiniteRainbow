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
        floor = 0;

        hpUpgrade = new int[] { 0, 0, 0 };
        atkUpgrade = new int[] { 0, 0, 0 };
        defUpgrade = new int[] { 0, 0, 0 };
        dodgeUpgrade = new int[] { 0, 0, 0 };
        criChanceUpgrade = new int[] { 0, 0, 0 };
        criMultiUpgrade = new int[] { 0, 0, 0 };
        speedUpgrade = new int[] { 0, 0, 0 };

        GameManager.instance.SaveGame();
    }

    public static void SaveData()
    {
        JsonData data = new JsonData();
        data.floor = floor;
        data.hpUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.hpUpgrade[i] = hpUpgrade[i];
        }
        data.atkUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.atkUpgrade[i] = atkUpgrade[i];
        }
        data.defUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.defUpgrade[i] = defUpgrade[i];
        }
        data.dodgeUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.dodgeUpgrade[i] = dodgeUpgrade[i];
        }
        data.criChanceUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.criChanceUpgrade[i] = criChanceUpgrade[i];
        }
        data.criMultiUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.criMultiUpgrade[i] = criMultiUpgrade[i];
        }
        data.speedUpgrade = new int[3];
        for (int i = 0; i < 3; i++)
        {
            data.speedUpgrade[i] = speedUpgrade[i];
        }
    }
}

