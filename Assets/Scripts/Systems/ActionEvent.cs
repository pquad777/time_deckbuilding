using System;

public enum Team { Player, Enemy }
public enum TargetPolicy { Self, Opponent }
public enum ActionType { Damage, GainBlock, Dodge, ClearBlock,Filter }

[Serializable]
public struct ActionEvent
{
    public ActionType type;
    public Team source;
    public TargetPolicy target;

    public int value;
    public int repeat;

    public bool isAttack;
    public bool cancelled;
    public IEventFilter filter;

    public string id; // 예약 취소/연결용(나중 확장)

    public static ActionEvent Damage(Team src, TargetPolicy target, int value, bool isAttack = false, int repeat = 1, string id = null)
        => new ActionEvent
        {
            type = ActionType.Damage,
            source = src,
            target = target,
            value = value,
            repeat = Math.Max(1, repeat),
            isAttack = isAttack,
            cancelled = false,
            id = id
        };

    public static ActionEvent GainBlock(Team src, TargetPolicy target, int value, string id = null)
        => new ActionEvent
        {
            type = ActionType.GainBlock,
            source = src,
            target = target,
            value = value,
            repeat = 1,
            isAttack = false,
            cancelled = false,
            id = id
        };

    public static ActionEvent Dodge(Team src, string id = null)
        => new ActionEvent
        {
            type = ActionType.Dodge,
            source = src,
            target = TargetPolicy.Self,
            value = 0,
            repeat = 1,
            isAttack = false,
            cancelled = false,
            id = id
        };

    public static ActionEvent ClearBlock(Team src, string id = null)
        => new ActionEvent
        {
            type = ActionType.ClearBlock,
            source = src,
            target = TargetPolicy.Self,
            value = 0,
            repeat = 1,
            isAttack = false,
            cancelled = false,
            id = id
        };
    public static ActionEvent ApplyFilter(Team src,TargetPolicy target,IEventFilter newFilter, string id = null)
        => new ActionEvent
        {
            type = ActionType.Filter,
            source = src,
            target = TargetPolicy.Self,
            value = 0,
            repeat = 1,
            filter=newFilter,
            isAttack = false,
            cancelled = false,
            id = id
        };
}