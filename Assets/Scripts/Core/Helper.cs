using System;

using UnityEngine;

namespace Sim.Util
{
    using Random = System.Random;

    public static class Helper
    {
        static Random rand;
        static Helper()
        {
            rand = new Random();
        }

        public static int RandomRange(int min, int max)
        {
            return rand.Next(min, max);
        }

        public static Vector3 RandomDirection()
        {
            return new Vector3(RandomValue(), RandomValue(), RandomValue());
        }

        public static float RandomValue()
        {
            return rand.Next(-1000, 1001) * 0.001f;
        }
        public static float RandomValue01()
        {
            return rand.Next(0, 1000001) * 0.000001f;
        }
    }
}