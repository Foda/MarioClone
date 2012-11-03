using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;

namespace SFML_Test
{
    public static class RandomExtensions
    {
        public static int NextByte(this Random random)
        {
            return Colorizer.FloatToByte(random.NextDouble());
        }

        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }
    }

    class Colorizer
    {
        private static Random random = new Random();

        public static List<Color> GenerateColors_Uniform(int colorCount)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < colorCount; i++)
            {
                Color newColor = Color.FromArgb(
                    255,
                    random.NextByte(),
                    random.NextByte(),
                    random.NextByte());

                colors.Add(newColor);
            }

            return colors;
        }

        public static List<Color> GenerateColors_Harmony(
            int colorCount,
            float offsetAngle1,
            float offsetAngle2,
            float rangeAngle0,
            float rangeAngle1,
            float rangeAngle2,
            float saturation, float luminance)
        {
            List<Color> colors = new List<Color>();

            float referenceAngle = random.NextFloat() * 360;

            for (int i = 0; i < colorCount; i++)
            {
                float randomAngle = random.NextFloat() * (rangeAngle0 + rangeAngle1 + rangeAngle2);

                if (randomAngle > rangeAngle0)
                {
                    if (randomAngle < rangeAngle0 + rangeAngle1)
                    {
                        randomAngle += offsetAngle1;
                    }
                    else
                    {
                        randomAngle += offsetAngle2;
                    }
                }

                HSL hslColor = new HSL(((referenceAngle + randomAngle) / 360.0f) % 1.0f, saturation, luminance);

                colors.Add(hslColor.Color);
            }

            return colors;
        }

        public static List<Color> GenerateColors_Harmony2(
            int colorCount,
            float offsetAngle1,
            float offsetAngle2,
            float rangeAngle0,
            float rangeAngle1,
            float rangeAngle2,
            float saturation, float saturationRange,
            float luminance, float luminanceRange)
        {
            List<Color> colors = new List<Color>();

            float referenceAngle = random.NextFloat() * 360;

            for (int i = 0; i < colorCount; i++)
            {
                float randomAngle = random.NextFloat() * (rangeAngle0 + rangeAngle1 + rangeAngle2);

                if (randomAngle > rangeAngle0)
                {
                    if (randomAngle < rangeAngle0 + rangeAngle1)
                    {
                        randomAngle += offsetAngle1;
                    }
                    else
                    {
                        randomAngle += offsetAngle2;
                    }
                }

                float newSaturation = saturation + (random.NextFloat() - 0.5f) * saturationRange;
                float newLuminance = luminance + +(random.NextFloat() - 0.5f) * luminanceRange;

                HSL hslColor = new HSL(((referenceAngle + randomAngle) / 360.0f) % 1.0f, newSaturation, newLuminance);

                colors.Add(hslColor.Color);
            }

            return colors;
        }


        public static List<Color> GenerateColors_RandomWalk(int colorCount, Color startColor, float min, float max)
        {
            List<Color> colors = new List<Color>();

            Color newColor = startColor;

            float range = max - min;

            for (int i = 0; i < colorCount; i++)
            {
                int rSign = random.Next(2) % 2 == 0 ? 1 : -1;
                int gSign = random.Next(2) % 2 == 0 ? 1 : -1;
                int bSign = random.Next(2) % 2 == 0 ? 1 : -1;

                newColor = Color.FromArgb(
                    255,
                    FloatToByte(ByteToFloat(newColor.R) + rSign * (min + random.NextDouble() * range)),
                    FloatToByte(ByteToFloat(newColor.G) + rSign * (min + random.NextDouble() * range)),
                    FloatToByte(ByteToFloat(newColor.B) + rSign * (min + random.NextDouble() * range)));

                colors.Add(newColor);
            }

            return colors;
        }

        public static List<Color> GenerateColors_RandomMix(int colorCount, Color color1, Color color2, Color color3, float greyControl, bool paint)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < colorCount; i++)
            {
                Color newColor;
                if (paint)
                {
                    newColor = RandomMixHSL(color1, color2, color3, greyControl);
                }
                else
                {
                    newColor = RandomMix(color1, color2, color3, greyControl);
                }
                colors.Add(newColor);
            }

            return colors;
        }

        public static List<Color> GenerateColors_RandomAdd(int colorCount, Color color1, Color color2, Color color3, float nonGrayBias)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < colorCount; i++)
            {
                Color newColor = RandomAdd(color1, color2, color3, nonGrayBias);

                colors.Add(newColor);
            }

            return colors;
        }

        public static List<Color> GenerateColors_Offset(int colorCount, Color color, float maxRange)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < colorCount; i++)
            {
                Color newColor = Color.FromArgb(
                    255,
                    FloatToByte((ByteToFloat(color.R) + random.NextDouble() * 2 * maxRange - maxRange)),
                    FloatToByte((ByteToFloat(color.G) + random.NextDouble() * 2 * maxRange - maxRange)),
                    FloatToByte((ByteToFloat(color.B) + random.NextDouble() * 2 * maxRange - maxRange)));

                colors.Add(newColor);
            }

            return colors;
        }

        public static List<Color> GenerateColors_Hue(int colorCount, float saturation, float luminance)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < colorCount; i++)
            {
                HSL hslColor = new HSL(random.NextDouble(), saturation, luminance);

                colors.Add(hslColor.Color);
            }

            return colors;
        }

        public static List<Color> GenerateColors_Saturation(int colorCount, float hue, float luminance)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < colorCount; i++)
            {
                HSL hslColor = new HSL(hue, random.NextDouble(), luminance);

                colors.Add(hslColor.Color);
            }

            return colors;
        }

        public static List<Color> GenerateColors_Luminance(int colorCount, float hue, float saturation)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < colorCount; i++)
            {
                HSL hslColor = new HSL(hue, saturation, random.NextDouble());

                colors.Add(hslColor.Color);
            }

            return colors;
        }

        public static List<Color> GenerateColors_SaturationLuminance(int colorCount, float hue)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < colorCount; i++)
            {
                HSL hslColor = new HSL(hue, random.NextDouble(), random.NextDouble());

                colors.Add(hslColor.Color);
            }

            return colors;
        }

        public static List<Color> GenerateColors_GoldenRatioRainbow(int colorCount, float saturation, float luminance)
        {
            List<Color> colors = new List<Color>();

            float goldenRatioConjugate = 0.618033988749895f;
            float currentHue = (float)random.NextDouble();


            for (int i = 0; i < colorCount; i++)
            {
                HSL hslColor = new HSL(currentHue, saturation, luminance);

                colors.Add(hslColor.Color);

                currentHue += goldenRatioConjugate;
                currentHue %= 1.0f;

            }

            return colors;
        }

        public static List<Color> GenerateColors_GoldenRatioGradient(int colorCount, Color[] gradient, float saturation, float luminance)
        {
            List<Color> colors = new List<Color>();

            float goldenRatioConjugate = 0.618033988749895f;
            float currentHue = (float)random.NextDouble();


            for (int i = 0; i < colorCount; i++)
            {
                HSL hslColor = new HSL(currentHue, saturation, luminance);

                Color newColor = SampleLinearGradient(gradient, currentHue);

                colors.Add(newColor);

                currentHue += goldenRatioConjugate;
                currentHue %= 1.0f;

            }

            return colors;
        }

        public static List<Color> GenerateColors_HueRange(int colorCount, float startHue, float endHue, float saturation, float luminance)
        {
            List<Color> colors = new List<Color>();
            float hueRange = endHue - startHue;

            if (hueRange < 0)
            {
                hueRange += 1.0f;
            }

            for (int i = 0; i < colorCount; i++)
            {
                float newHue = (float)(hueRange * random.NextDouble() + startHue);

                if (newHue > 1.0)
                {
                    newHue -= 1.0f;
                }

                HSL hslColor = new HSL(newHue, saturation, luminance);

                colors.Add(hslColor.Color);
            }

            return colors;
        }

        public static List<Color> GenerateColors_JitteredRainbow(int colorCount, float startHue, float endHue, float saturation, float luminance, bool jitter)
        {
            List<Color> colors = new List<Color>();
            float hueRange = endHue - startHue;

            if (hueRange < 0)
            {
                hueRange = 1 + hueRange;
            }

            float cellRange = hueRange / colorCount;
            float cellOffset = (float)(random.NextDouble() * cellRange);

            for (int i = 0; i < colorCount; i++)
            {
                float newHue;
                if (jitter)
                {
                    newHue = (float)(cellRange * i + random.NextDouble() * cellRange + startHue);
                }
                else
                {
                    newHue = (cellRange * i + cellOffset + startHue);
                }

                if (newHue > 1)
                {
                    newHue -= 1.0f;
                }

                HSL hslColor = new HSL(newHue, saturation, luminance);

                colors.Add(hslColor.Color);
            }

            return colors;
        }

        public static int FloatToByte(double value)
        {
            int byteValue = (int)(value * 256);

            byteValue = SaturateByte(byteValue);

            return byteValue;
        }

        private static int SaturateByte(int byteValue)
        {
            if (byteValue > 255)
            {
                byteValue = 255;
            }
            else if (byteValue < 0)
            {
                byteValue = 0;
            }
            return byteValue;
        }

        public static float ByteToFloat(int byteValue)
        {
            float floatValue = byteValue / 256.0f;

            return floatValue;
        }

        public static Color RandomMix(Color color1, Color color2, Color color3, float greyControl)
        {
            int randomIndex = random.NextByte() % 3;

            float mixRatio1 = (randomIndex == 0) ? random.NextFloat() * greyControl : random.NextFloat();
            float mixRatio2 = (randomIndex == 1) ? random.NextFloat() * greyControl : random.NextFloat();
            float mixRatio3 = (randomIndex == 2) ? random.NextFloat() * greyControl : random.NextFloat();

            float sum = mixRatio1 + mixRatio2 + mixRatio3;

            mixRatio1 /= sum;
            mixRatio2 /= sum;
            mixRatio3 /= sum;

            return Color.FromArgb(
                255,
                (byte)(mixRatio1 * color1.R + mixRatio2 * color2.R + mixRatio3 * color3.R),
                (byte)(mixRatio1 * color1.G + mixRatio2 * color2.G + mixRatio3 * color3.G),
                (byte)(mixRatio1 * color1.B + mixRatio2 * color2.B + mixRatio3 * color3.B));
        }

        public static Color RandomMixHSL(Color color1, Color color2, Color color3, float greyControl)
        {
            int randomIndex = random.NextByte() % 3;

            float mixRatio1 = (randomIndex == 0) ? random.NextFloat() * greyControl : random.NextFloat();
            float mixRatio2 = (randomIndex == 1) ? random.NextFloat() * greyControl : random.NextFloat();
            float mixRatio3 = (randomIndex == 2) ? random.NextFloat() * greyControl : random.NextFloat();

            float sum = mixRatio1 + mixRatio2 + mixRatio3;

            mixRatio1 /= sum;
            mixRatio2 /= sum;
            mixRatio3 /= sum;

            HSL hsl1 = new HSL(color1);
            HSL hsl2 = new HSL(color2);
            HSL hsl3 = new HSL(color3);

            return new HSL(
                (mixRatio1 * hsl1.H + mixRatio2 * hsl2.H + mixRatio3 * hsl3.H),
                (mixRatio1 * hsl1.S + mixRatio2 * hsl2.S + mixRatio3 * hsl3.S),
                (mixRatio1 * hsl1.L + mixRatio2 * hsl2.L + mixRatio3 * hsl3.L)).Color;
        }

        public static Color RandomMixPaint(Color color1, Color color2, Color color3, float greyControl)
        {
            int randomIndex = random.NextByte() % 3;

            float mixRatio1 = (randomIndex == 0) ? random.NextFloat() * greyControl : random.NextFloat();
            float mixRatio2 = (randomIndex == 1) ? random.NextFloat() * greyControl : random.NextFloat();
            float mixRatio3 = (randomIndex == 2) ? random.NextFloat() * greyControl : random.NextFloat();

            float sum = mixRatio1 + mixRatio2 + mixRatio3;

            mixRatio1 /= sum;
            mixRatio2 /= sum;
            mixRatio3 /= sum;

            return Color.FromArgb(
                255,
                255 - (byte)(mixRatio1 * (255 - color1.R) + mixRatio2 * (255 - color2.R) + mixRatio3 * (255 - color3.R)),
                255 - (byte)(mixRatio1 * (255 - color1.G) + mixRatio2 * (255 - color2.G) + mixRatio3 * (255 - color3.G)),
                255 - (byte)(mixRatio1 * (255 - color1.B) + mixRatio2 * (255 - color2.B) + mixRatio3 * (255 - color3.B)));
        }

        public static Color RandomAdd(Color color1, Color color2, Color color3, float nonGrayBias)
        {
            int rIndex = random.NextByte() % 3;

            float r1 = (rIndex == 0) ? random.NextFloat() * nonGrayBias : random.NextFloat();
            float r2 = (rIndex == 1) ? random.NextFloat() * nonGrayBias : random.NextFloat();
            float r3 = (rIndex == 2) ? random.NextFloat() * nonGrayBias : random.NextFloat();

            float p = 20;
            float sum = (float)Math.Pow(
                Math.Pow(r1, p) +
                Math.Pow(r2, p) +
                Math.Pow(r3, p), 1 / p);



            r1 /= sum;
            r2 /= sum;
            r3 /= sum;

            return Color.FromArgb(
                255,
                SaturateToByte(r1 * color1.R + r2 * color2.R + r3 * color3.R),
                SaturateToByte(r1 * color1.G + r2 * color2.G + r3 * color3.G),
                SaturateToByte(r1 * color1.B + r2 * color2.B + r3 * color3.B));
        }

        public static int SaturateToByte(float floatValue)
        {
            int intValue = (int)floatValue;

            if (intValue > 255)
            {
                intValue = 255;
            }
            else if (intValue < 0)
            {
                intValue = 0;
            }

            return intValue;
        }

        public static Color SampleLinearGradient(Color[] colors, float t)
        {
            int colorCount = colors.Length;
            int leftIndex = (int)(t * colorCount);

            float cellRange = 1.0f / colorCount;
            float alpha = (t - leftIndex * cellRange) / cellRange;

            Color leftColor = colors[leftIndex];
            Color rightColor = colors[(leftIndex + 1) % colorCount];

            return Color.FromArgb(
                255,
                (byte)(leftColor.R * (1 - alpha) + rightColor.R * (alpha)),
                (byte)(leftColor.G * (1 - alpha) + rightColor.G * (alpha)),
                (byte)(leftColor.B * (1 - alpha) + rightColor.B * (alpha)));
        }

        public static Bitmap GeneratePaletteImage(List<Color> colors, int width, int height, int spacing)
        {
            Bitmap image = new Bitmap(width, height);
            int colorCount = colors.Count;
            int blockWidth = (width - ((colorCount - 1) * spacing)) / colorCount;

            for (int i = 0; i < colorCount; i++)
            {
                DrawBlock(image, i * (blockWidth + spacing), 0, blockWidth, height, colors[i]);
            }

            return image;
        }



        public static void DrawBlock(Bitmap image, int x, int y, int width, int height, Color color)
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    image.SetPixel(i, j, color);
                }
            }
        }
    }
}
