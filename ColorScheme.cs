using System;
using System.Collections.Generic;
using Cairo;

public class ColorScheme
{
	public static readonly Color WIN_BG = ToRGBA ("FAF8EF");
	public static readonly Color GRID_BG = ToRGBA ("BBADA0");

	public static readonly Color BRIGHT = ToRGBA ("776E65");
	public static readonly Color LIGHT = ToRGBA ("F9F6F2");

	private Dictionary<int, Color> background = new Dictionary<int, Color>();

	public ColorScheme() {
		InitBackrounds();
	}

	public static Color ToRGBA(int r, int g, int b, double a) {
		return new Color ((double) r / 255, (double) g / 255, (double) b / 255, a);
	}

	public static Color ToRGBA(int r, int g, int b) {
		return ToRGBA (r, g, b, (double) 1.0);
	}

	public static Color ToRGBA(string value) {
		int r = Convert.ToInt32(value.Substring (0, 2), 16);
		int g = Convert.ToInt32(value.Substring (2, 2), 16);
		int b = Convert.ToInt32(value.Substring (4, 2), 16);
		return ToRGBA (r, g, b);
	}

	private void InitBackrounds() {
		background.Add(0,		ToRGBA(238, 228, 218, 0.35));
		background.Add(2,		ToRGBA("EEE4DA"));
		background.Add(4,		ToRGBA("EDE0C8"));
		background.Add(8,		ToRGBA("F2B179"));
		background.Add(16,		ToRGBA("F59563"));
		background.Add(32,		ToRGBA("F67C5F"));
		background.Add(64,		ToRGBA("F65E3B"));
		background.Add(128,		ToRGBA("EDCF72"));
		background.Add(256,		ToRGBA("EDCC61"));
		background.Add(512,		ToRGBA("EDC850"));
		background.Add(1024, 	ToRGBA("EDC53F"));
		background.Add(2048, 	ToRGBA("EDC22E"));
	}

	public Color GetTileBackground(int value) {
		return background[value];
	}

	public Color GetTileColor(int value) {
		if (value <= 8) {
			return BRIGHT;
		} else {
			return LIGHT;
		}
	}

}