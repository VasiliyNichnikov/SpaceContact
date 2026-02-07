using UnityEngine.UI;

namespace Client.UI.Extensions
{
    public static class TextExtensions
    {
        public static void SetText(this Text text, int value)
        {
            text.SetText(value.ToString());
        }
        
        public static void SetText(this Text text, string? value)
        {
            text.text = value ?? string.Empty;
        }
    }
}