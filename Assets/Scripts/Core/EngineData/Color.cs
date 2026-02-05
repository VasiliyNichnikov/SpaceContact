using System.Globalization;

namespace Core.EngineData
{
    public readonly struct Color
    {
        private const float MaxAlpha = 1.0f;
        
        public readonly float R;
        
        public readonly float G;
        
        public readonly float B;
        
        public readonly float A;
        
        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static Color FromHex(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            
            var value = int.Parse(hex, NumberStyles.HexNumber);
            
            var r = (value >> 16) & 0xFF;
            var g = (value >> 8) & 0xFF;
            var b = value & 0xFF;

            return new Color(r, g, b, MaxAlpha);
        }
    }
}