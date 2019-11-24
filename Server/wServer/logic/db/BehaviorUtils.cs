#region

using System;

#endregion

namespace wServer.logic
{
    partial class BehaviorDb
    {
        static int RandomNumbers(int n1, int n2)
        {
            var rng = new Random();
            return rng.Next(n1, n2);
        }

    }
}