using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using RoboWarX;
using RoboWarX.FileFormats;

namespace RoboWarX.Arena
{
    public class Arena
    {
        // Random seed. We should get the exact same results for a given seed, no matter when and how
        // many times the simulation is run.
        public readonly int seed;
        // FIXME: I doubt Random's behavior matches the routines used two decades ago.
        internal Random prng { get; set; }
        
        // The robots in the arena
        private List<Robot> robots_;
        public IList<Robot> robots { get; private set; }
        
        // The object list to walk when updating. EXCLUDES robots!
        private List<ArenaObject> objects_;
        public ICollection<ArenaObject> objects { get; private set; }
        // Objects scheduled to be added into the update list
        internal List<ArenaObject> newObjects { get; private set; }
        // Objects scheduled to be deleted from the update list
        internal List<ArenaObject> delObjects { get; private set; }

        // This matters when deciding a win
        private bool teamBattle_;
        public bool teamBattle
        {
            get { return teamBattle_; }
            set
            {
                if (chronon > 0)
                    throw new FieldAccessException("Simulation has already been started.");
                teamBattle_ = value;
            }
        }
        
        // Simulation time
        public int chronon { get; private set; }
        public bool finished { get; private set; }
        private int chrononLimit_;
        public int chrononLimit
        {
            get { return chrononLimit_; }
            set
            {
                if (chronon > 0)
                    throw new FieldAccessException("Simulation has already been started.");
                chrononLimit_ = value;
            }
        }

        // Some additional internal state
        private byte numAlive;
        private byte onlyTrackingShots; // game has ended, but loop just a tad more

        public Arena(int seed)
        {
            robots_ = new List<Robot>(Constants.MAX_ROBOTS);
            robots = new ReadOnlyCollection<Robot>(robots_);
            
            objects_ = new List<ArenaObject>();
            objects = new ReadOnlyCollection<ArenaObject>(objects_);
            newObjects = new List<ArenaObject>();
            delObjects = new List<ArenaObject>();

            teamBattle = false;

            chronon = 0;
            finished = false;
            chrononLimit = -1;
            
            numAlive = 0;
            onlyTrackingShots = 0;

            this.seed = seed;
            prng = new Random(seed);
        }

        public Arena() : this(new Random().Next()) {}

        // Check whether a robot is colliding with another or the wall.
        // Also sets some critical attributes used by registers, like friend and wall.
        // Pretty much an exact translation of the original routine.
        private void checkCollisions(Robot who)
        {
            long deltaX, deltaY;

            foreach (Robot robot in robots)
            {
                if (robot == who || !robot.alive)
                    continue;

                deltaX = (long)(who.x - robot.x);
                deltaY = (long)(who.y - robot.y);
                if (Math.Abs(deltaX) >= Constants.ROBOT_RADIUS << 1 ||
                    Math.Abs(deltaY) >= Constants.ROBOT_RADIUS << 1 ||
                    deltaX * deltaX + deltaY * deltaY >= (Constants.ROBOT_RADIUS * Constants.ROBOT_RADIUS << 2))
                    continue;

                if (who.energy > 0 && who.stunned == 0)
                {
                    who.x -= who.speedx;
                    who.y -= who.speedy;
                }
                who.collision = true;
                robot.collision = true;
                if (who.team > 0 && who.team == robot.team)
                {
                    who.friend = true;
                    robot.friend = true;
                }
            }

            if (who.x < Constants.ROBOT_RADIUS || who.x > Constants.ARENA_SIZE - Constants.ROBOT_RADIUS)
            {
                who.wall = true;

                who.x = (short)Math.Max((ushort)0, who.x);
                who.x = (short)Math.Min((ushort)Constants.ARENA_SIZE, who.x);
            }
            if (who.y < Constants.ROBOT_RADIUS || who.y > Constants.ARENA_SIZE - Constants.ROBOT_RADIUS)
            {
                who.wall = true;

                who.y = (short)Math.Max((ushort)0, who.y);
                who.y = (short)Math.Min((ushort)Constants.ARENA_SIZE, who.y);
            }
        }

        // Subtract points for intimacy. This is war, gentlemen!
        private void doCollisionDamage(Robot who)
        {
            if (who.collision)
                who.doDamage(1);
            if (who.wall)
                who.doDamage(5);
        }

        private void checkDeath(Robot who)
        {
            if (who.damage > 0 || who.deathTime >= 0)
                return;

            numAlive--;

            who.explode();

            // Check if the game has ended
            if (numAlive > 1)
            {
                int loop;
                for (loop = 0; loop < robots.Count; loop++)
                    if (robots[loop].alive)
                        break;

                int firstTeam = robots[loop].team;
                if (firstTeam == 0)
                    return;
                else
                    foreach (Robot robot in robots)
                        if (robot.team != firstTeam && robot.alive)
                            return;
            }

            // End it after 20 chronons
            onlyTrackingShots = 20;
        }

        private void calcKillPoints(Robot who)
        {
            if (who.alive)
                who.kills = (short)(
                    (who.killTime[0] != -1 ? 1 : 0) +
                    (who.killTime[1] != -1 ? 1 : 0) +
                    (who.killTime[2] != -1 ? 1 : 0) +
                    (who.killTime[3] != -1 ? 1 : 0) +
                    (who.killTime[4] != -1 ? 1 : 0) +
                    (who.killTime[5] != -1 ? 1 : 0));
            else
                who.kills = (short)(
                    (who.killTime[0] != -1 && who.deathTime - who.killTime[0] >= 20 ? 1 : 0) +
                    (who.killTime[1] != -1 && who.deathTime - who.killTime[1] >= 20 ? 1 : 0) +
                    (who.killTime[2] != -1 && who.deathTime - who.killTime[2] >= 20 ? 1 : 0) +
                    (who.killTime[3] != -1 && who.deathTime - who.killTime[3] >= 20 ? 1 : 0) +
                    (who.killTime[4] != -1 && who.deathTime - who.killTime[4] >= 20 ? 1 : 0) +
                    (who.killTime[5] != -1 && who.deathTime - who.killTime[5] >= 20 ? 1 : 0));

            // Give every robot alive, if there's less than 4 and more than 1, a survival point.
            // If there's only one robot, wait 5 chronons before awarding a survival point.
            if (robots.Count > 2 && teamBattle)
            {
                if (numAlive == 3)
                {
                    foreach (Robot robot in robots)
                        if (robot.alive)
                            robot.survival = 1;
                }
                else if (numAlive == 2)
                {
                    foreach (Robot robot in robots)
                        if (robot.alive)
                            robot.survival = 2;
                }
            }
        }

        // This is so the loop doesn't do unexpected things when the
        // object lists change during an object update.
        private void updateObjects()
        {
            foreach (ArenaObject obj in newObjects)
                objects_.Add(obj);
            newObjects.Clear();
            foreach (ArenaObject obj in delObjects)
                objects_.Remove(obj);
            delObjects.Clear();
        }

        // Step a full chronon for all objects
        public void stepChronon()
        {
            if (finished)
                throw new StateException("Game has already finished.");

            // Reset some variables at the start of a chronon
            foreach (Robot robot in robots)
            {
                robot.collision = false;
                robot.friend = false;
                robot.wall = false;
                robot.hit = 0;
            }
            
            // FIXME: handle non-robot extension errors in a special way.
            List<RobotException> errors = new List<RobotException>();

            updateObjects();
            foreach (ArenaObject obj in objects_)
                obj.update();
            updateObjects();

            // Update all robots
            foreach (Robot robot in robots)
            {
                try
                {
                    robot.update();
                }
                catch (RobotException e)
                {
                    errors.Add(e);
                    robot.damage = -10;
                    robot.deathReason = DeathReason.Buggy;
                    checkDeath(robot);
                }

                if (robot.alive)
                    checkCollisions(robot);

                calcKillPoints(robot);
            }

            foreach (Robot robot in robots)
            {
                if (!robot.alive)
                    continue;

                doCollisionDamage(robot);
                checkDeath(robot);
            }

            // Interpret all robots FIXME
            byte[] order = { 0, 1, 2, 3, 4, 5 };
            for (int loop = 0; loop < robots.Count; loop++)
            {
                // Pick a random order
                int pos = prng.Next() % (robots.Count - loop);
                Robot who = robots[order[pos]];
                order[pos] = order[robots.Count - loop - 1];

                try
                {
                    who.executeChronon();
                }
                catch (RobotException e)
                {
                    errors.Add(e);
                    who.damage = -10;
                    who.deathReason = DeathReason.Buggy;
                }
                // catch all
                catch (Exception e)
                {
                    errors.Add(new RobotException(who,e.Message, e));
                    who.damage = -10;
                    who.deathReason = DeathReason.Buggy;
                }

                // This happens when a robot suicides from excessive energy usage or is buggy.
                checkDeath(who);
            }

            chronon++;
            if (chrononLimit > 0 && chronon >= chrononLimit)
                finished = true;

            if (onlyTrackingShots > 0)
            {
                onlyTrackingShots--;

                // Conclude after 20 chronons
                if (onlyTrackingShots == 0)
                    finished = true;
            }
            
            // Finish the chronon and throw every exception in one go.
            if (errors.Count == 1)
                throw errors[0];
            else if (errors.Count > 1)
                throw new MultipleErrorsException(errors.ToArray());
        }

        // Instantiate a new ArenaObject subclass
        internal ArenaObject spawn(Type objtype, double x, double y, params object[] parameters)
        {
            if (!objtype.IsSubclassOf(typeof(ArenaObject)))
                throw new ArenaObjectExtensionException(
                    "Object type is not an ArenaObject subclass");
            if (objtype == typeof(Robot))
                throw new ArgumentException("I won't allow you to spawn Robots!");

            List<Type> types = new List<Type>(parameters.Length);

            // Create the ArenaObject. Subclasses should implement
            // minimal constructors, and instead do most things in onSpawn.
            ArenaObject retval = Activator.CreateInstance(objtype, this, x, y) as ArenaObject;

            // Call onSpawn if applicable
            types.Clear();
            foreach (object parameter in parameters)
                types.Add(parameter.GetType());
            MethodInfo minfo = objtype.GetMethod("onSpawn", types.ToArray());
            if (minfo != null)
                minfo.Invoke(retval, parameters);
            else if (parameters.Length > 0)
                throw new ArenaObjectExtensionException(
                    "Cannot find a suitable onSpawn method for ArenaObject");

            newObjects.Add(retval);
            return retval;
        }

        internal void RegisterObject(ArenaObject obj)
        {
            newObjects.Add(obj);
        }

        // Instantiate a Robot based on the loaded RobotFile
        public Robot loadRobot(RobotFile f)
        {
            // FIXME: Can this be done during a match?
            
            // Arena full?
            if (robots.Count == Constants.MAX_ROBOTS)
                return null;

            // Try to find a starting position not too close to the other robots.
            // FIXME: construct a list of starting positions in the constructor
            // so the outcome of prng.Next() is consistent.
            int x, y;
            double dist;
            do
            {
                x = prng.Next(Constants.ARENA_SIZE - 30) + 15;
                y = prng.Next(Constants.ARENA_SIZE - 30) + 15;
                dist = 1000;
                foreach (Robot other in robots)
                {
                    double test = Math.Pow(x - other.x, 2) + Math.Pow(y - other.y, 2);
                    if (test < dist)
                        dist = test;
                }
            } while (dist < 625);

            // Instantiate
            Robot robot = new Robot(this, x, y, robots.Count, f);

            // Update state
            robots_.Add(robot);
            numAlive++;

            return robot;
        }

        public void draw(Graphics gfx)
        {
            foreach (ArenaObject obj in objects_)
                obj.draw(gfx);
            foreach (Robot robot in robots)
                robot.draw(gfx);
        }
    }
}
