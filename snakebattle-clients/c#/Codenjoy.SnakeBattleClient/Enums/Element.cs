namespace Codenjoy.SnakeBattleClient.Enums
{
    public enum Element : short
    {
        None = (short)' ',
        Apple = (short)'○',
        Stone = (short)'●',
        Wall = (short)'☼',

        FlyingPill = (short)'©',
        FuryPill = (short)'®',
        Gold = (short)'$',

        HeadDead = (short)'☻',
        HeadEvil = (short)'♥',
        HeadFly = (short)'♠',
        HeadSleep = (short)'&',

        HeadDown = (short)'▼',
        HeadLeft = (short)'◄',
        HeadRight = (short)'►',
        HeadUp = (short)'▲',

        TailEndDown = (short)'╙',
        TailEndLeft = (short)'╘',
        TailEndUp = (short)'╓',
        TailEndRight = (short)'╕',
        TailInactive = (short)'~',


        BodyHorizontal = (short)'═',
        BodyVertical = (short)'║',
        BodyLeftDown = (short)'╗',
        BodyLeftUp = (short)'╝',
        BodyRightDown = (short)'╔',
        BodyRightUp = (short)'╚',

        StartFloor = (short)'#',

        EnemyHeadDown = (short)'˅',
        EnemyHeadLeft = (short)'<',
        EnemyHeadRight = (short)'>',
        EnemyHeadUp = (short)'˄',
        EnemyHeadDead = (short)'☺',
        EnemyHeadEvil = (short)'♣',
        EnemyHeadFly = (short)'♦',
        EnemyHeadSleep = (short)'ø',

        EnemyTailEndDown = (short)'¤',
        EnemyTailEndLeft = (short)'×',
        EnemyTailEndUp = (short)'æ',
        EnemyTailEndRight = (short)'ö',
        EnemyTailInactive = (short)'*',

        EnemyBodyHorizontal = (short)'─',
        EnemyBodyVertical = (short)'│',
        EnemyBodyLeftDown = (short)'┐',
        EnemyBodyLeftUp = (short)'┘',
        EnemyBodyRightDown = (short)'┌',
        EnemyBodyRightUp = (short)'└'
    }
}