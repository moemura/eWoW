namespace eWoW.Common
{
    public class Vector3
    {
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public class Vector4 : Vector3
    {
        public Vector4(float x, float y, float z, float w) : base(x, y, z)
        {
            W = w;
        }

        public float W { get; set; }
    }
}