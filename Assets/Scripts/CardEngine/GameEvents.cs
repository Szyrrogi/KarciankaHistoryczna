// Polyfill for record types in older Unity/.NET versions
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace CardEngine.GameEvents
{
    public record GameEvent;
    public record AttackEvent(int AttackerInstanceId, int DefenderInstanceId) : GameEvent;
    public record DrawCardEvent(int PlayerId, int CardId, int InstanceCardId) : GameEvent;
    public record PlayerWonEvent(int PlayerId) : GameEvent;
    public record CardDestroyedEvent(int PlayerId, int InstanceCardId) : GameEvent;
    public record CardUpdatedEvent(int PlayerId, int InstanceCardId) : GameEvent;
    public record TurnEndEvent(int PlayerId) : GameEvent;
    public record PutCardOnBattlefieldEvent(int PlayerId, int InstanceCardId) : GameEvent;
}

