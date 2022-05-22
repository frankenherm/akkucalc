using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
    public static double GetDistance(this Player.Entity @this, Player.Entity other)
    {
        return GetDistance(@this, other.X, other.Y);
    }

    public static double GetDistance(this Player.Entity @this, int other_x, int other_y)
    {
        var x = Math.Abs(@this.X - other_x);
        var y = Math.Abs(@this.Y - other_y);
        return Math.Sqrt(x * x + y * y);
    }
}

public enum EntityType
{
    Monster = 0,
    OwnHero = 1,
    OppositeHero = 2
}

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
public class Player
{
    public class Entity
    {
        public int Id;
        public EntityType Type;
        public int X, Y;
        public int ShieldLife;
        public bool IsControlled;
        public int Health;
        public int Vx, Vy;
        public bool NearBase;
        public int ThreatFor;
        public List<Entity> TargetedBy = new List<Entity>();
        public Entity SuggestedTarget;

        public Entity(int id, EntityType type, int x, int y, int shieldLife, bool isControlled, int health, int vx, int vy, bool nearBase, int threatFor)
        {
            this.Id = id;
            this.Type = type;
            this.X = x;
            this.Y = y;
            this.ShieldLife = shieldLife;
            this.IsControlled = isControlled;
            this.Health = health;
            this.Vx = vx;
            this.Vy = vy;
            this.NearBase = nearBase;
            this.ThreatFor = threatFor;
        }
    }

    static void Main(string[] args)
    {
        const int mid_x = 17630 / 2;
        const int mid_y = 9000 / 2;

        string[] inputs;
        inputs = Console.ReadLine().Split(' ');

        // base_x,base_y: The corner of the map representing your base
        int baseX = int.Parse(inputs[0]);
        int baseY = int.Parse(inputs[1]);

        // heroesPerPlayer: Always 3
        int heroesPerPlayer = int.Parse(Console.ReadLine());

        int monsterReachesBaseIn(Entity monster)
        {
            var distance_x = Math.Abs(baseX - monster.X - 300);
            var distance_y = Math.Abs(baseY - monster.Y - 300);
            var turns_x = monster.Vx != 0 ? distance_x / Math.Abs(monster.Vx) : 0;
            var turns_y = monster.Vy != 0 ? distance_y / Math.Abs(monster.Vy) : 0;
            var result = turns_x > turns_y ? turns_x : turns_y;
            Console.Error.WriteLine($"({monster.X}|{monster.Y}) moves with ({monster.Vx}|{monster.Vy}), reach base ({baseX}|{baseY}) in {result}");
            return result;
        }

        int monsterIsKilledIn(Entity monster, List<Entity> nearestHero)
        {
            var heros = nearestHero.OrderBy(h => h.GetDistance(monster));
            var herosNearby = nearestHero.Where(h => h.GetDistance(monster) < 800).Count();

            var turns = (herosNearby > 0 ? (monster.Health + 1) / (herosNearby * 2) : (monster.Health + 1) / 2);
            turns += 1; // kulanz

            var distance = (int)Math.Floor(heros.First().GetDistance(monster));
            if (distance > 800)
            {
                turns += (distance - 400) / 800;
            }
            Console.Error.WriteLine($"monster with health {monster.Health} has hero in distance {distance} and is killed in {turns}");
            return turns;
        }

        // game loop
        while (true)
        {

            inputs = Console.ReadLine().Split(' ');
            int myHealth = int.Parse(inputs[0]); // Your base health
            int myMana = int.Parse(inputs[1]); // Ignore in the first league; Spend ten mana to cast a spell

            inputs = Console.ReadLine().Split(' ');
            int oppHealth = int.Parse(inputs[0]);
            int oppMana = int.Parse(inputs[1]);

            int entityCount = int.Parse(Console.ReadLine()); // Amount of heros and monsters you can see

            List<Entity> myHeroes = new List<Entity>(entityCount);
            List<Entity> oppHeroes = new List<Entity>(entityCount);
            List<Entity> monsters = new List<Entity>(entityCount);

            for (int i = 0; i < entityCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int id = int.Parse(inputs[0]); // Unique identifier
                int type = int.Parse(inputs[1]); // 0=monster, 1=your hero, 2=opponent hero
                int x = int.Parse(inputs[2]); // Position of this entity
                int y = int.Parse(inputs[3]);
                int shieldLife = int.Parse(inputs[4]); // Ignore for this league; Count down until shield spell fades
                int isControlled = int.Parse(inputs[5]); // Ignore for this league; Equals 1 when this entity is under a control spell
                int health = int.Parse(inputs[6]); // Remaining health of this monster
                int vx = int.Parse(inputs[7]); // Trajectory of this monster
                int vy = int.Parse(inputs[8]);
                int nearBase = int.Parse(inputs[9]); // 0=monster with no target yet, 1=monster targeting a base
                int threatFor = int.Parse(inputs[10]); // Given this monster's trajectory, is it a threat to 1=your base, 2=your opponent's base, 0=neither

                Entity entity = new Entity(id, (EntityType)type, x, y, shieldLife, isControlled == 1, health, vx, vy, nearBase == 1, threatFor);

                switch ((EntityType)type)
                {
                    case EntityType.Monster:
                        monsters.Add(entity);
                        break;
                    case EntityType.OwnHero:
                        myHeroes.Add(entity);
                        break;
                    case EntityType.OppositeHero:
                        oppHeroes.Add(entity);
                        break;
                }
            }
            var spread = new Tuple<int, int>[] {
                new Tuple<int, int>(mid_x-2000, mid_y),
                new Tuple<int, int>(6500,1500),
                new Tuple<int, int>(3000,5000),
            };

            var threats = monsters.Where(m => m.ThreatFor == 1);

            foreach (var monster in threats.OrderBy(m => {
                var distance = m.GetDistance(baseX, baseY);
                Console.Error.WriteLine($"Threat with ID {m.Id} has Distance {distance}");
                return distance;
            }).Take(3))
            {
                var nearestHero = myHeroes.OrderBy(h => h.GetDistance(monster)).First();
                nearestHero.SuggestedTarget = monster;
            }

            Entity heroCastsWind = null;
            for (int i = 0; i < myHeroes.Count(); ++i)
            {
                var hero = myHeroes[i];
                var isOffensive = true;

                if (hero.SuggestedTarget != null)
                {
                    hero.SuggestedTarget.TargetedBy.Add(hero);
                    if (myMana >= 10 && hero.SuggestedTarget.ShieldLife == 0)
                    {
                        if ((hero.GetDistance(hero.SuggestedTarget) < 800 && monsterReachesBaseIn(hero.SuggestedTarget) < 4) ||
                            (monsters.Count(m => hero.GetDistance(m) < 800 && m.GetDistance(baseX, baseY) < 8000) > 1 && (heroCastsWind == null || hero.GetDistance(heroCastsWind) > 1200)))
                        {
                            Console.WriteLine($"SPELL WIND {mid_x} {mid_y}");
                            heroCastsWind = hero;
                            myMana -= 10;
                            continue;
                        }
                        else if (hero.GetDistance(hero.SuggestedTarget) < 2000 && (monsterReachesBaseIn(hero.SuggestedTarget) < (monsterIsKilledIn(hero.SuggestedTarget, hero.SuggestedTarget.TargetedBy))))
                        {
                            Console.WriteLine($"SPELL CONTROL {hero.SuggestedTarget.Id} {mid_x} {mid_y}");
                            myMana -= 10;
                            continue;
                        }
                    }
                    Console.WriteLine($"MOVE {hero.SuggestedTarget.X + hero.SuggestedTarget.Vx} {hero.SuggestedTarget.Y + hero.SuggestedTarget.Vy}");
                    continue;
                }

                if (threats.Any())
                {
                    var ordered = threats.OrderBy(m => m.GetDistance(hero));
                    var target = ordered.FirstOrDefault(m => !m.TargetedBy.Any() || monsterReachesBaseIn(m) < (monsterIsKilledIn(m, m.TargetedBy)));
                    if (target != null)
                    {
                        target.TargetedBy.Add(hero);
                        Console.WriteLine($"MOVE {target.X + target.Vx} {target.Y + target.Vy}");
                        continue;
                    }
                }

                if (!isOffensive || !monsters.Where(m => !m.TargetedBy.Any()).Any())
                {
                    var tuple = spread[i];
                    var x = baseX == 0 ? baseX + tuple.Item1 : baseX - tuple.Item1;
                    var y = baseY == 0 ? baseY + tuple.Item2 : baseY - tuple.Item2;
                    if (hero.GetDistance(x, y) > 400.0)
                    {
                        Console.WriteLine($"MOVE {x} {y}");
                    }
                    else Console.WriteLine("WAIT");
                    continue;
                }

                if (monsters.Any())
                {
                    var nearest = monsters.Where(m => !m.TargetedBy.Any()).OrderBy(m => m.GetDistance(hero)).FirstOrDefault();
                    nearest?.TargetedBy.Add(hero);
                    Console.WriteLine($"MOVE {nearest.X + nearest.Vx} {nearest.Y + nearest.Vy}");
                }
                else
                {
                    Console.WriteLine($"WAIT");
                }
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
        }
    }
}