using System;
using Gtk;

class MainClass
{
	public static void Main (string[] args)
	{
		Application.Init ();
		new Game ();
		Application.Run ();
	}
}

