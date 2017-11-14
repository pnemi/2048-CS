using System;
using Cairo;

public abstract class Grid
{

	private static int TILE_RADIUS = 7;
	private static int WIN_MARGIN = 20;
	private static int TILE_SIZE = 65;
	private static int TILE_MARGIN = 15;

	private static int DURATION = 150; // ms

	private static int _tilesAnimationsDone = 16;
	public static int TilesAnimationsDone { get { return _tilesAnimationsDone; } set { _tilesAnimationsDone = value; } }

	public static void paint(Gdk.Window w) {
		using (Cairo.Context c = Gdk.CairoHelper.Create (w)) {
			c.Antialias = Antialias.Subpixel;    // sets the anti-aliasing method
			drawBackground(c);
			drawTitle(c);
			drawScoreBoard(c);
			drawBoard(c);

			((IDisposable) c.GetTarget()).Dispose ();
		}
	}

	private static void drawBackground(Cairo.Context c) {
		c.SetSourceColor (ColorScheme.WIN_BG);
		c.Paint();
	}

	private static void drawTitle(Cairo.Context c) {
		c.SelectFontFace ("Arial", FontSlant.Normal, FontWeight.Bold);
		c.SetFontSize (38);
		c.SetSourceColor (ColorScheme.ToRGBA("776E65"));
		c.MoveTo (WIN_MARGIN, 50);
		c.ShowText("2048");
		c.Fill ();
	}

	private static void drawScoreBoard(Cairo.Context c) {
		int width = 160;
		int height = 40;
		int xOffset = Game.WINDOW.WIDTH - WIN_MARGIN - width;
		int yOffset = 20;
		string strScore = "SCORE";
		string strHighestScore = "HIGHEST";
		string score = Convert.ToString (Game.BOARD.Score);
		string highestScore = Convert.ToString (Game.BOARD.HighestScore);
		int xScore = Game.WINDOW.WIDTH - WIN_MARGIN - width + (width / 4);
		int xHighestScore = Game.WINDOW.WIDTH - WIN_MARGIN - (width / 4);

		DrawRoundedRectangle (c, xOffset, yOffset, width, height, TILE_RADIUS);
		c.Fill ();

		c.SetSourceColor( new Color(1, 1, 1) );
		c.SelectFontFace ("Arial", FontSlant.Normal, FontWeight.Normal);
		c.SetFontSize (8);
		c.MoveTo (xScore - ((int) c.TextExtents (strScore).Width / 2), yOffset + 15);
		c.ShowText (strScore);
		c.MoveTo (xHighestScore - ((int) c.TextExtents (strHighestScore).Width / 2), yOffset + 15);
		c.ShowText (strHighestScore);
		c.SetFontSize (14);
		c.MoveTo (xScore - ((int) c.TextExtents (score).Width / 2), yOffset + 30);
		c.ShowText(score);
		c.MoveTo (xHighestScore - ((int) c.TextExtents (highestScore).Width / 2), yOffset + 30);
		c.ShowText(highestScore);
		c.Fill ();
	}

	private static void drawTilesBackground(Cairo.Context c) {

		int posX;
		int posY;
		c.SetSourceColor(Game.COLORS.GetTileBackground(0));

		for (int row = 0; row < 4; row++) {
			for (int col = 0; col < 4; col++) {
				posX = col * (TILE_MARGIN + TILE_SIZE) + TILE_MARGIN;
				posY = row * (TILE_MARGIN + TILE_SIZE) + TILE_MARGIN;
				DrawRoundedRectangle (c, posX, posY, TILE_SIZE, TILE_SIZE, TILE_RADIUS);
			}
		}

		c.Fill ();
	}

	private static void drawBoard(Cairo.Context c) {
		c.Translate (WIN_MARGIN, 80);
		c.SetSourceColor (ColorScheme.GRID_BG);
		DrawRoundedRectangle (c, 0, 0, Game.WINDOW.WIDTH - (WIN_MARGIN * 2), 320 + TILE_MARGIN, TILE_RADIUS);
		c.Fill ();

		drawTilesBackground(c);

		for (int row = 0; row < 4; row++) {
			for (int col = 0; col < 4; col++) {
				drawTile(c, Game.BOARD.GetTileAt(row, col), col, row);
			}
		}

		if (Window.timer && TilesAnimationsDone < 16) {
			// continue timer
		} else {
			Window.timer = false;
			Game.BOARD.SetDefaultPositions ();
			TilesAnimationsDone = 16;
		}
	}

	private static void drawTile(Cairo.Context c, Tile tile, int x, int y) {
		int value = tile.Value;
		int prevX = tile.Col;
		int prevY = tile.Row;
		int fromX = tile.Col * (TILE_MARGIN + TILE_SIZE) + TILE_MARGIN;
		int fromY = tile.Row * (TILE_MARGIN + TILE_SIZE) + TILE_MARGIN;
		int toX = x * (TILE_MARGIN + TILE_SIZE) + TILE_MARGIN;
		int toY = y * (TILE_MARGIN + TILE_SIZE) + TILE_MARGIN;
		int posX = 0;
		int posY = 0;

		int distanceX = Math.Abs(fromX - toX);
		int distanceY = Math.Abs(fromY - toY);

		tile.ShiftX += (((double) Window.INTERVAL * distanceX) / DURATION);
		tile.ShiftY += (((double) Window.INTERVAL * distanceY) / DURATION);

		// Console.WriteLine ("[" + y + "," + x + "]: " + tile.Animating); // isAnimating() debug

		if (tile.Animating) { 
			bool done = false;
			if (prevX != x) { // horizontal moving
				if (prevX < x) {
					if ((posX = (int) (fromX + tile.ShiftX)) >= toX) {
						done = true;
					}
				} else if (prevX > x) {
					if ((posX = (int)(fromX - tile.ShiftX)) <= toX) {
						done = true;
					}
				}
				posY = toY;
			} else if (prevY != y) { // vertical moving
				if (prevY < y) {
					if ((posY = (int)(fromY + tile.ShiftY)) >= toY) {
						done = true;
					}
				} else if (prevY > y) {
					if ((posY = (int) (fromY - tile.ShiftY)) <= toY) {
						done = true;
					}
				}
				posX = toX;
			} else {
				Console.WriteLine("MOVE ERROR!");
			}

			if (done) {
				tile.Animating = false;
				TilesAnimationsDone++;
				tile.ResetShift ();
				posX = toX;
				posY = toY;
			}
		} else { // empty and not moved tiles holds their final positions
			posX = toX;
			posY = toY;
		}
		if (value != 0) {
		
		c.SetSourceColor(Game.COLORS.GetTileBackground(value));
		DrawRoundedRectangle (c, posX, posY, TILE_SIZE, TILE_SIZE, TILE_RADIUS);
		c.Fill ();
		c.SetSourceColor(Game.COLORS.GetTileColor(value));

		int size = value < 100 ? 36 : value < 1000 ? 32 : 24;
		c.SetFontSize (size);
		c.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Bold);

		string s = value.ToString();
		TextExtents te = c.TextExtents(s);
		double w = te.Width;
		double h = te.Height;

		c.MoveTo(posX + ((TILE_SIZE - w) / 2 - te.XBearing),
			posY + TILE_SIZE - ((TILE_SIZE - h) / 2 ));

			c.ShowText(s);
		}

		if (Game.BOARD.WonOrLost != null && Game.BOARD.WonOrLost.Length != 0) {
			c.SetSourceColor (new Color(1, 1, 1, 0.4));
			c.Rectangle (0, 0, Game.WINDOW.WIDTH, Game.WINDOW.HEIGHT);
			c.Fill ();
			c.SetSourceColor(ColorScheme.BRIGHT);
			c.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Normal);
			c.SetFontSize (30);
			c.MoveTo (68, 150);
			c.ShowText ("You " + Game.BOARD.WonOrLost + "!");
			c.Fill ();
		}

		c.Fill ();

	}


	static void DrawRoundedRectangle (Cairo.Context c, double x, double y, int width, int height, int radius)
	{
		c.Save ();

		if ((radius > height / 2) || (radius > width / 2))
			radius = Math.Min (height / 2, width / 2);

		c.MoveTo (x, y + radius);
		c.Arc (x + radius, y + radius, radius, Math.PI, -Math.PI / 2);
		c.LineTo (x + width - radius, y);
		c.Arc (x + width - radius, y + radius, radius, -Math.PI / 2, 0);
		c.LineTo (x + width, y + height - radius);
		c.Arc (x + width - radius, y + height - radius, radius, 0, Math.PI / 2);
		c.LineTo (x + radius, y + height);
		c.Arc (x + radius, y + height - radius, radius, Math.PI / 2, Math.PI);
		c.ClosePath ();
		c.Restore ();
	}
}

