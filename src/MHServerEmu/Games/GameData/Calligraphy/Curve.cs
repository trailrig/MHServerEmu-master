﻿namespace MHServerEmu.Games.GameData.Calligraphy
{
    public class Curve
    {
        public double[] Values { get; }

        public Curve(byte[] data)
        {
            using (MemoryStream stream = new(data))
            using (BinaryReader reader = new(stream))
            {
                CalligraphyHeader header = new(reader);
                int startPosition = reader.ReadInt32();
                int endPosition = reader.ReadInt32();

                Values = new double[endPosition - startPosition + 1];
                for (int i = 0; i < Values.Length; i++)
                    Values[i] = reader.ReadDouble();
            }
        }
    }
}
