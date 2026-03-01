namespace Services.Raid
{
    public enum DeckEnumRaiderState
    {
        NotInitialized = 0,
        MoveTowardsChest = 100,
        LootingChest = 200,
        GuardLooter = 300,
        RunAway = 10000,
        Finished = 100000
    }
}