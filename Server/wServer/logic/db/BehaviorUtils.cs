using common;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        static int RandomNumbers(int min, int max)
        {
            return Utils.Random.Next(min, max);
        }

    }
}