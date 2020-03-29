using System;

namespace CardGames
{
    public static class Tool
    {
        /// <summary>
        /// Mixes an array.
        /// </summary>
        public static void Mix<T>(T[] array)
        {
            var guid = Guid.NewGuid().ToByteArray();
            var seed = guid[0] + 256 * guid[1] + 65536 * guid[2] + 16777216 * guid[3];
            var rnd = new Random(seed);

            for (var n = 0; n < 100; n++)
            {
                for (var p1 = 0; p1 < array.Length; p1++)
                {
                    var p2 = rnd.Next(0, array.Length - 1);
                    var prev = array[p1];
                    array[p1] = array[p2];
                    array[p2] = prev;
                }
            }
        }
    }
}
