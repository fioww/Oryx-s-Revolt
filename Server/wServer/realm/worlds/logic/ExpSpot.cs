using System.Linq;
using common.resources;
using wServer.networking;
using Player = wServer.realm.entities.Player;

namespace wServer.realm.worlds.logic
{
    class ExpSpot : World
    {
        public ExpSpot(ProtoWorld proto, Client client = null) : base(proto) {
        }

        protected override void Init() {
            base.Init();
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            Timers.Add(new WorldTimer(500, (w, t) => {

                foreach (var player in w.Players.Values)
                {
                    if (player.Level >= 20 && player.Rank < 150)
                    {
                        player.DisconnectPlayer();
                    }
                }
            }));
        }

        public override int EnterWorld(Entity entity)
        {
            var ret = base.EnterWorld(entity);

            if (entity is Player p)
                p.SendInfo("Welcome to the EXP Zone! Here you can level up to 20, then you will be disconnected!");

            return ret;
        }
    }
}
