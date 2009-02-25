using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Arena.Weapons
{
    // Used to lay atomic land mines. Returns 0 if read, places a mine with energy investment equal
    // to the amount written. The mine is stationary and becomes active ten chronons after
    // placement. Once active, it will detonate against any target that hits it, causing damage
    // equal to 2*(energy investment-5). This is twice as effective as mines in RoboWar 2.3 and
    // earlier. Mines cannot be used unless they were first enabled at the hardware store.

    // FIXME: implement
}
