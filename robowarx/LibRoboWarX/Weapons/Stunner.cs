using System;
using RoboWarX;
using RoboWarX.Arena;

namespace RoboWarX.Weapons.Stunner
{
    // Used to fire a stasis capsule. Returns 0 if read, shoots stasis capsule with speed 14 in the
    // directon that the robot's turret points. The amound written is removed from the robot's
    // energy supply; if a stasis capsule hits a robot, the robot is placed in stasis for one
    // chronon for every four points of energy invested in the capsule. While in stasis, a robot
    // does not move, interpret instructions, or regain energy; however, the robot's shields do not
    // decay. Stunners cannot be used unless they are first enabled in the Hardware Store.

    // FIXME: implement
}
