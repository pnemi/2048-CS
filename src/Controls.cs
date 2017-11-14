using System;


public class Controls
{
	public void keyPressed (Gdk.Key keyCode)
	{
		if (Grid.TilesAnimationsDone == 16) { // all animations done
			switch (keyCode) {
			case Gdk.Key.Up:
				Console.WriteLine ("UP");
				Game.BOARD.MoveUp ();
				break;
			case Gdk.Key.Down:
				Console.WriteLine ("DOWN");
				Game.BOARD.MoveDown ();
				break;
			case Gdk.Key.Left:
				Console.WriteLine ("LEFT");
				Game.BOARD.MoveLeft ();
				break;
			case Gdk.Key.Right:
				Console.WriteLine ("RIGHT");
				Game.BOARD.MoveRight ();
				break;
			default:
				break;
			}
		}

		Game.BOARD.IsGameOver ();
		//Game.BOARD.Show ();
		Game.WINDOW.StartAnimation ();
		//Game.WINDOW.QueueDraw ();
	}

}