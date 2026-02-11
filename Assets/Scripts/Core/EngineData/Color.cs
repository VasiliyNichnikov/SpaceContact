using System;
using System.Globalization;

namespace Core.EngineData
{
    public readonly struct Color : IEquatable<Color>
    {
        private const float MaxAlpha = 1.0f;
        private const float MaxColorValue = 255.0f;
        
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
            
            var rawR = (value >> 16) & 0xFF;
            var rawG = (value >> 8) & 0xFF;
            var rabB = value & 0xFF;

            var r = rawR / MaxColorValue;
            var g = rawG / MaxColorValue;
            var b =  rabB / MaxColorValue;
            
            return new Color(r, g, b, MaxAlpha);
        }

        public bool Equals(Color other)
        {
            return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B) && A.Equals(other.A);
        }

        public override bool Equals(object? obj)
        {
            return obj is Color other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }
    }
}