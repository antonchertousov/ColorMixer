using System.Collections.Generic;
using System.Linq;
using Scrtwpns.Mixbox;

namespace ColorMixing.Helpers
{
    public static class ColorMixer
    {
        /// <summary>
        /// Converts Drawing to Media color format
        /// </summary>
        private static System.Windows.Media.Color ColorToColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts Media to Drawing color format
        /// </summary>
        private static System.Drawing.Color ColorToColor(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        
        /// <summary>
        /// Colors mixing in drawing color format
        /// </summary>
        public static System.Drawing.Color Mix(IEnumerable<System.Drawing.Color> colors)
        {
            MixboxLatent zMix = new MixboxLatent();
            float mixingRatio = 1.0f / colors.Count();
            foreach (var color in colors)
            {
                zMix += Mixbox.RGBToLatent(color.ToArgb())*mixingRatio;
            }

            return System.Drawing.Color.FromArgb(Mixbox.LatentToRGB(zMix));
        }

        /// <summary>
        /// Colors mixing in Windows.Media.Color format.
        /// </summary>
        public static System.Windows.Media.Color Mix(IEnumerable<System.Windows.Media.Color> colors)
        {
            var drawingColors = colors.Select(ColorToColor);
            return ColorToColor(Mix(drawingColors));
        }
    }
}