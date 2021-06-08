using System;

namespace Assets.Scripts.Mathx
{
    public static class Mathematic
    {
        /// <summary>
        /// Sayıyı verilen değerin katlarına yuvarlar
        /// </summary>
        /// <param name="value">Verilecek değer</param>
        /// <param name="multiple">Yuvarlanacak kat</param>
        /// <returns></returns>
        public static float RoundToMultiple(float value, float multiple)
        {
            return (float) Math.Round(value / multiple) * multiple;
        }
    }
}