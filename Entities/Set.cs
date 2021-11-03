using System;

namespace fxl.codes.kisekae.Entities
{
    [Flags]
    public enum Set
    {
        Unset = -1,
        Zero = 2 ^ 0,
        One = 2 ^ 1,
        Two = 2 ^ 2,
        Three = 2 ^ 3,
        Four = 2 ^ 4,
        Five = 2 ^ 5,
        Six = 2 ^ 6,
        Seven = 2 ^ 7,
        Eight = 2 ^ 8,
        Nine = 2 ^ 9
    }
}