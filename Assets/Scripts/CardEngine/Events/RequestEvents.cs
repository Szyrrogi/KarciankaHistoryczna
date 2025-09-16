namespace CardEngine.RequestEvents
{
    public record RequestEvent;
    public record StartGameRequest() : RequestEvent;
    public record AttackUnitRequest(int attackerInstanceId, int defenderInstanceId) : RequestEvent;
    public record EndTurnRequest(int PlayerId) : RequestEvent;
    public record PutCardOnBattlefieldRequest(int InstanceCardId) : RequestEvent;
}