public class SaveData
{
    [System.Serializable]
    public class RoomSaveData
    {
        public int X;
        public int Y;
        public int EnemiesToSpawn;
        public Room.RoomExitLayout Layout;
        public string RoomName;
    }

    public bool saveExists = false;
    public bool isTutorialCompleted = false;

    public int[] selectedWeapons = { 0, 1 };
    public int selectedWeapon = 0;
    public int[] ammo = { 0, 0 };
    public int[] reserve = { 0, 0 };

    public int currentLevel = 0;
    public RoomSaveData[] currentLayout = new RoomSaveData[0];
    public int currentRoomX = 0;
    public int currentRoomY = 0;
    public float playerX = 0f;
    public float playerY = 0f;

    public float health = 100f;
    public float damageReceived = 100f;
    public float staminaCost = 100f;
    public float staminaRegenRate = 100f;
    public float moveSpeed = 3f;
}
