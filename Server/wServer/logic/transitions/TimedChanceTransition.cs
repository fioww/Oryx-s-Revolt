using wServer.realm;

namespace wServer.logic.transitions
{
    class TimedChanceTransition : Transition
    {
        //State storage: cooldown timer

        int time;
        bool randomized;
        int chance;

        public TimedChanceTransition(int time, string targetState, int chance = 100, bool randomized = false)
            : base(targetState)
        {
            this.time = time;
            this.randomized = randomized;
            this.chance = chance;
        }

        protected override bool TickCore(Entity host, RealmTime time, ref object state)
        {
            if (this.chance > 100)
                this.chance = 100;

            int cool;
            if (state == null) cool = randomized ? Random.Next(this.time) : this.time;
            else cool = (int)state;

            bool ret = false;
            if (cool <= 0 && this.chance <= Random.Next(this.chance, 100))
            {
                ret = true;
                cool = this.time;
            }
            else
                cool -= time.ElapsedMsDelta;

            state = cool;
            return ret;
        }
    }
}
