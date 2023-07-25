using common.resources;
using wServer.networking;
using wServer.realm.entities;

namespace wServer.realm.worlds.logic
{
    class ExpSpot : World
    {
        public ExpSpot(ProtoWorld proto, Client client = null) : base(proto) {
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            foreach (var player in Players)
                if (player.Value.Level >= 20 && player.Value.Rank < 80)
                    player.Value.ReconnectToNexus(false);
        }

        public override int EnterWorld(Entity entity)
        {
            var ret = base.EnterWorld(entity);

            if (entity is Player p)
                p.SendInfo("Welcome to the EXP Zone! Here you can level up to 20, then you will be disconnected.");

            return ret;
        }
    }
}
