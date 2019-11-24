using System;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using common;
using common.resources;
using log4net;
using Newtonsoft.Json;
using wServer.networking.packets.outgoing;
using wServer.realm.setpieces;
using wServer.realm.entities;
using wServer.realm.worlds;
using wServer.realm.worlds.logic;

namespace wServer.realm.commands
{
    internal class DevSpawnCommand : Command
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SpawnCommand));

        private struct JsonSpawn
        {
            public string notif;
            public SpawnProperties[] spawns;
        }

        private struct SpawnProperties
        {
            public string name;
            public int? hp;
            public int? size;
            public int? count;
            public int[] x;
            public int[] y;
            public bool? target;
        }

        private const int Delay = 3; // in seconds

        public DevSpawnCommand() : base("devspawn", permLevel: 0, alias: "spawnb") { }

        protected override bool Process(Player player, RealmTime time, string args)
        {
            args = args.Trim();
            return args.StartsWith("{") ?
                SpawnJson(player, args) :
                SpawnBasic(player, args);
        }

        private bool SpawnJson(Player player, string json)
        {
            var gameData = player.Manager.Resources.GameData;

            JsonSpawn props;
            try
            {
                props = JsonConvert.DeserializeObject<JsonSpawn>(json);
            }
            catch (Exception)
            {
                player.SendError("JSON not formatted correctly!");
                return false;
            }

            if (player.Owner.Name == "Nexus" && player.Rank < 150)
            {
                player.SendError("Can't spawn in the nexus.");
            }

            if (player.Owner.Name != "Vault" && player.Rank < 100)
            {
                player.SendError("Forbidden.");
            }

            if (props.spawns != null)
                foreach (var spawn in props.spawns)
                {
                    if (spawn.name == null)
                    {
                        player.SendError("No mob specified. Every entry needs a name property.");
                        return false;
                    }

                    var objType = GetSpawnObjectType(gameData, spawn.name);
                    if (objType == null)
                    {
                        player.SendError("Unknown entity!");
                        return false;
                    }

                    var desc = gameData.ObjectDescs[objType.Value];

                    var hp = desc.MaxHP;
                    if (spawn.hp > hp && spawn.hp < int.MaxValue)
                        hp = spawn.hp.Value;

                    var size = desc.MinSize;
                    if (spawn.size >= 25 && spawn.size <= 500)
                        size = spawn.size.Value;

                    var count = 1;
                    if (spawn.count > count && spawn.count <= 500)
                        count = spawn.count.Value;

                    int[] x = null;
                    int[] y = null;

                    if (spawn.x != null)
                        x = new int[spawn.x.Length];

                    if (spawn.y != null)
                        y = new int[spawn.y.Length];

                    if (x != null)
                    {
                        for (var i = 0; i < x.Length && i < count; i++)
                        {
                            if (spawn.x[i] > 0 && spawn.x[i] <= player.Owner.Map.Width)
                            {
                                x[i] = spawn.x[i];
                            }

                        }

                    }

                    if (y != null)
                    {
                        for (var i = 0; i < y.Length && i < count; i++)
                        {
                            if (spawn.y[i] > 0 && spawn.y[i] <= player.Owner.Map.Height)
                            {
                                y[i] = spawn.y[i];
                            }

                        }

                    }

                    var target = false;
                    if (spawn.target != null)
                        target = spawn.target.Value;

                    QueueSpawnEvent(player, count, objType.Value, hp, size, x, y, target);
                }

            if (props.notif != null)
            {
                NotifySpawn(player, props.notif);
            }
            var rng = new Random();
            if (player.Zombification())
            {
                QueueSpawnEvent(player, rng.Next(1, 3), 200);
            }
            return true;
        }

        private bool SpawnBasic(Player player, string args)
        {

            if (player.Owner.Name == "Nexus" && player.Rank < 150)
            {
                player.SendError("Can't spawn in the nexus.");
            }

            if (player.Owner.Name != "Vault" && player.Rank < 100)
            {
                player.SendError("Forbidden.");
            }
            var gameData = player.Manager.Resources.GameData;

            // split argument
            var index = args.IndexOf(' ');
            int num;
            var name = args;
            if (args.IndexOf(' ') > 0 && int.TryParse(args.Substring(0, args.IndexOf(' ')), out num)) //multi
                name = args.Substring(index + 1);
            else
                num = 1;

            var objType = GetSpawnObjectType(gameData, name);
            if (objType == null)
            {
                player.SendError("Unknown entity!");
                return false;
            }

            if (num <= 0)
            {
                player.SendInfo("You can not summon negative amounts.");
                return false;
            }

            var id = player.Manager.Resources.GameData.ObjectTypeToId[objType.Value];
            if (player.Client.Account.Rank < 100 &&
                player.Owner is DeathArena &&
                id.Contains("Fountain"))
            {
                player.SendError("Insufficient rank.");
                return false;
            }

            NotifySpawn(player, id, num);
            QueueSpawnEvent(player, num, objType.Value);
            return true;
        }

        private ushort? GetSpawnObjectType(XmlData gameData, string name)
        {
            ushort objType;
            if (!gameData.IdToObjectType.TryGetValue(name, out objType) ||
                !gameData.ObjectDescs.ContainsKey(objType))
            {
                // no match found, try to get partial match
                var mobs = gameData.IdToObjectType
                    .Where(m => m.Key.ContainsIgnoreCase(name) && gameData.ObjectDescs.ContainsKey(m.Value))
                    .Select(m => gameData.ObjectDescs[m.Value]);

                if (!mobs.Any())
                    return null;

                var maxHp = mobs.Max(e => e.MaxHP);
                objType = mobs.First(e => e.MaxHP == maxHp).ObjectType;
            }

            return objType;
        }

        private void NotifySpawn(Player player, string mob, int? num = null)
        {
            var w = player.Owner;

            var notif = mob;
            if (num != null)
                notif = "Spawning " + ((num > 1) ? num + " " : "") + mob + "...";

            w.BroadcastPacket(new Notification
            {
                Color = new ARGB(0xffff0000),
                ObjectId = (player.IsControlling) ? player.SpectateTarget.Id : player.Id,
                Message = notif
            }, null);

            if (player.IsControlling)
                w.BroadcastPacket(new Text
                {
                    Name = $"#{player.SpectateTarget.ObjectDesc.DisplayId}",
                    NumStars = -1,
                    BubbleTime = 0,
                    Txt = notif
                }, null);
            else
                w.BroadcastPacket(new Text
                {
                    Name = $"#{player.Name}",
                    NumStars = player.Stars,
                    Admin = player.Admin,
                    BubbleTime = 0,
                    Txt = notif
                }, null);
        }

        private void QueueSpawnEvent(
            Player player,
            int num,
            ushort mobObjectType, int? hp = null, int? size = null,
            int[] x = null, int[] y = null,
            bool? target = false)
        {
            var pX = player.X;
            var pY = player.Y;

            player.Owner.Timers.Add(new WorldTimer(Delay * 1000, (world, t) => // spawn mob in delay seconds
            {
                for (var i = 0; i < num && i < 500; i++)
                {
                    Entity entity;
                    try
                    {
                        entity = Entity.Resolve(world.Manager, mobObjectType);
                    }
                    catch (Exception e)
                    {
                        log.ErrorFormat(e.ToString());
                        return;
                    }

                    entity.DevSpawned = true;

                    if (entity is Enemy enemy)
                    {
                        if (hp != null)
                        {
                            enemy.HP = hp.Value;
                            enemy.MaximumHP = enemy.HP;
                        }

                        if (size != null)
                            enemy.SetDefaultSize(size.Value);

                        if (target == true)
                            enemy.AttackTarget = player;

                        enemy.ApplyConditionEffect(new ConditionEffect()
                        {
                            Effect = ConditionEffectIndex.Alliance,
                            DurationMS = -1
                        });
                    }

                    var sX = (x != null && i < x.Length) ? x[i] : pX;
                    var sY = (y != null && i < y.Length) ? y[i] : pY;

                    entity.Move(sX, sY);

                    if (!world.Deleted)
                        world.EnterWorld(entity);
                }
            }));
        }
    }

    internal class ClearAllSpawnsCommand : Command
    {
        public ClearAllSpawnsCommand() : base("clearallspawned", permLevel: 0, alias: "cas") { }

        protected override bool Process(Player player, RealmTime time, string args)
        {
            var iterations = 0;
            var lastKilled = -1;
            var removed = 0;
            while (removed != lastKilled)
            {
                lastKilled = removed;
                foreach (var entity in player.Owner.Enemies.Values.Where(e => e.Spawned || e.DevSpawned))
                {
                    entity.Death(time);
                    removed++;
                }
                foreach (var entity in player.Owner.StaticObjects.Values.Where(e => e.Spawned || e.DevSpawned))
                {
                    player.Owner.LeaveWorld(entity);
                    removed++;
                }
                if (++iterations >= 5)
                    break;
            }

            player.SendInfo($"{removed} spawned entities removed!");
            return true;
        }
    }

    internal class ClearAllGravesCommand : Command
    {
        public ClearAllGravesCommand() : base("clearallgraves", permLevel: 100, alias: "cag") { }

        protected override bool Process(Player player, RealmTime time, string args)
        {
            var removed = 0;
            foreach (var entity in player.Owner.StaticObjects.Values)
            {
                if (entity is Container || entity.ObjectDesc == null)
                    continue;

                if (entity.ObjectDesc.ObjectId.StartsWith("Gravestone"))
                {
                    player.Owner.LeaveWorld(entity);
                    removed++;
                }
            }

            player.SendInfo($"{removed} gravestones removed!");
            return true;
        }
    }

    internal class RankCommand : Command
    {
        public RankCommand() : base("rank", permLevel: 100) { }

        protected override bool Process(Player player, RealmTime time, string args)
        {
            var index = args.IndexOf(' ');
            if (string.IsNullOrEmpty(args) || index == -1)
            {
                player.SendInfo("Usage: /rank <player name> <rank>");
                return false;
            }

            var name = args.Substring(0, index);
            var rank = int.Parse(args.Substring(index + 1));

            if (Database.GuestNames.Contains(name, StringComparer.InvariantCultureIgnoreCase))
            {
                player.SendError("Cannot rank unnamed accounts.");
                return false;
            }

            var id = player.Manager.Database.ResolveId(name);
            if (id == player.AccountId)
            {
                player.SendError("Cannot rank self.");
                return false;
            }

            var acc = player.Manager.Database.GetAccount(id);
            if (id == 0 || acc == null)
            {
                player.SendError("Account not found!");
                return false;
            }

            // kick player from server to set rank
            foreach (var i in player.Manager.Clients.Keys)
                if (i.Account.Name.EqualsIgnoreCase(name))
                    i.Disconnect();

            if (acc.Admin && rank < 80)
            {
                // reset account
                player.Manager.Database.WipeAccount(
                    acc, player.Manager.Resources.GameData, player.Name);
                acc.Reload();
            }

            acc.Admin = rank >= 80;
            acc.LegacyRank = rank;
            acc.Hidden = false;
            acc.FlushAsync();

            player.SendInfo($"{acc.Name} given legacy rank {acc.LegacyRank}{((acc.Admin) ? " and now has admin status" : "")}.");
            return true;
        }
    }

    internal class GiftCommand : Command
    {
        public GiftCommand() : base("gift", permLevel: 0) { }

        protected override bool Process(Player player, RealmTime time, string args)
        {
            if (player == null)
                return false;

            var manager = player.Manager;

            // verify argument
            var index = args.IndexOf(' ');
            if (string.IsNullOrWhiteSpace(args) || index == -1)
            {
                player.SendInfo("Usage: /gift <player name> <item name>");
                return false;
            }

            // get command args
            var playerName = args.Substring(0, index);
            var item = GetItem(player, args.Substring(index + 1));
            if (item == null)
            {
                return false;
            }

            // get player account
            if (Database.GuestNames.Contains(playerName, StringComparer.InvariantCultureIgnoreCase))
            {
                player.SendError("Cannot gift the unnamed...");
                return false;
            }
            var id = manager.Database.ResolveId(playerName);
            var acc = manager.Database.GetAccount(id);
            if (id == 0 || acc == null)
            {
                player.SendError("Account not found!");
                return false;
            }

            // add gift
            var result = player.Manager.Database.AddGift(acc, item.ObjectType);
            if (!result)
            {
                player.SendError("Gift not added. Something happened with the adding process.");
                return false;
            }

            // send out success notifications
            player.SendInfoFormat("You gifted {0} one {1}.", acc.Name, item.DisplayName);
            var gifted = player.Manager.Clients.Keys
                .SingleOrDefault(p => p.Account.AccountId == acc.AccountId);
            gifted?.Player?.SendInfoFormat(
                "You received a gift from {0}. Enjoy your {1}.",
                player.Name,
                item.DisplayName);
            return true;
        }

        private Item GetItem(Player player, string itemName)
        {
            var gameData = player.Manager.Resources.GameData;

            //ushort objType;

            // allow both DisplayId and Id for query
            if (!gameData.DisplayIdToObjectType.TryGetValue(itemName, out var objType))
            {
                if (!gameData.IdToObjectType.TryGetValue(itemName, out objType))
                {
                    player.SendError("Unknown item type!");
                    return null;
                }
            }

            if (!gameData.Items.ContainsKey(objType))
            {
                player.SendError("Not an item!");
                return null;
            }


            return gameData.Items[objType];
        }
    }

    internal class OverrideAccountCommand : Command
    {
        public OverrideAccountCommand() : base("overrideacc", permLevel: 150) { }

        protected override bool Process(Player player, RealmTime time, string name)
        {
            var acc = player.Client.Account;

            if (string.IsNullOrWhiteSpace(name))
            {
                player.SendError("Usage: /override <player name>");
                return false;
            }

            var id = player.Manager.Database.ResolveId(name);
            if (id == 0)
            {
                player.SendError("Account not found!");
                return false;
            }

            acc.AccountIdOverride = id;
            acc.FlushAsync();
            player.SendInfo("Account override set.");
            return true;
        }
    }
    internal class WargCommand : Command
    {
        public WargCommand() : base("warg", permLevel: 100) { }

        protected override bool Process(Player player, RealmTime time, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                player.SendError("Usage: /warg <mob name>");
                return false;
            }

            var target = player.GetNearestEntityByName(2900, name);
            if (target == null)
            {
                player.SendError("Mob not found.");
                return false;
            }

            if (target.Controller != null)
            {
                player.SendError("Only one person can control a mob at a time.");
                return false;
            }

            if (player.SpectateTarget != null)
            {
                player.SpectateTarget.FocusLost -= player.ResetFocus;
                player.SpectateTarget.Controller = null;
            }

            player.ApplyConditionEffect(ConditionEffectIndex.Paused);
            target.FocusLost += player.ResetFocus;
            target.Controller = player;
            player.SpectateTarget = target;
            player.Sight.UpdateCount++;

            player.Owner.Timers.Add(new WorldTimer(500, (w, t) =>
            {
                player.Client.SendPacket(new SetFocus()
                {
                    ObjectId = target.Id
                });
            }));
            return true;
        }
    }
    internal class KillPlayerCommand : Command
    {
        public KillPlayerCommand() : base("killPlayer", permLevel: 150) { }

        protected override bool Process(Player player, RealmTime time, string args)
        {
            foreach (var i in player.Manager.Clients.Keys)
            {
                if (i.Account.Name.EqualsIgnoreCase(args))
                {
                    i.Player.HP = 0;
                    i.Player.Death(player.Name);
                    player.SendInfo("Player killed!");
                    return true;
                }
            }
            player.SendError($"Player '{args}' could not be found!");
            return false;
        }
    }
    internal class RemovePets : Command
    {
        public RemovePets() : base("removeAllPets", permLevel: 150) { }

        protected override bool Process(Player player, RealmTime time, string args)
        {
            // close all worlds / disconnect all players
            foreach (var w in player.Manager.Worlds.Values)
            {
                w.Closed = true;
                foreach (var p in w.Players.Values)
                    p.Client.Disconnect();
            }

            player.Manager.Database.RemoveAllPets(player.Manager.Resources.GameData);

            Program.Stop();
            return true;
        }
    }

    internal class RemoveServerGold : Command
    {
        public RemoveServerGold() : base("removeAllGold", permLevel: 150) { }

        protected override bool Process(Player player, RealmTime time, string args)
        {
            // close all worlds / disconnect all players
            foreach (var w in player.Manager.Worlds.Values)
            {
                w.Closed = true;
                foreach (var p in w.Players.Values)
                    p.Client.Disconnect();
            }

            player.Manager.Database.RemoveAllGold();

            Program.Stop();
            return true;
        }
    }
    internal class UnnameCommand : Command
    {
        public UnnameCommand() : base("unname", permLevel: 100) { }

        protected override bool Process(Player player, RealmTime time, string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                player.SendInfo("Usage: /unname <player name>");
                return false;
            }

            var playerName = args;

            var id = player.Manager.Database.ResolveId(playerName);
            if (id == 0)
            {
                player.SendError("Player account not found!");
                return false;
            }

            string lockToken = null;
            var key = Database.NAME_LOCK;
            var db = player.Manager.Database;

            try
            {
                while ((lockToken = db.AcquireLock(key)) == null) ;

                var acc = db.GetAccount(id);
                if (acc == null)
                {
                    player.SendError("Account doesn't exist.");
                    return false;
                }

                using (var l = db.Lock(acc))
                    if (db.LockOk(l))
                    {
                        while (!db.UnnameIGN(acc, lockToken)) ;
                        player.SendInfo("Account succesfully unnamed.");
                    }
                    else
                        player.SendError("Account in use.");
            }
            finally
            {
                if (lockToken != null)
                    db.ReleaseLock(key, lockToken);
            }

            return true;
        }
    }

}
