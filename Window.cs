using System;
using Gtk;

public partial class Window: Gtk.Window
{

	public readonly int WIDTH = 375;
	public readonly int HEIGHT = 450;

	public static bool timer = false;
	public static readonly uint INTERVAL = 16;

	private DrawingArea darea;

	public Window () : base (Gtk.WindowType.Toplevel)
	{

		this.Name = "Window";
		this.Title = "2048";
		this.Resize (WIDTH, HEIGHT);
		this.SetPosition (WindowPosition.Center);

		this.ExposeEvent += OnExposeEvent;
		this.DeleteEvent += OnDeleteEvent;
		this.KeyPressEvent += OnKeyPressEvent;

		darea = new DrawingArea ();
		darea.ExposeEvent += OnExposeEvent;
		Add (darea);

		this.ShowAll ();

	}

	public void StartAnimation ()
	{
		timer = true;
		GLib.Timeout.Add (Window.INTERVAL, new GLib.TimeoutHandler (Animation));
	}

	public bool Animation ()
	{
		if (!timer)
			return false;
		darea.QueueDraw ();
		return true;
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnExposeEvent (object sender, ExposeEventArgs a)
	{
		DrawingArea area = (DrawingArea)sender;
		Grid.paint (area.GdkWindow);
	}

	[GLib.ConnectBefore ()]
	protected virtual void OnKeyPressEvent (object o, Gtk.KeyPressEventArgs a)
	{
		Game.CONTROLS.keyPressed (a.Event.Key); 
	}
		
}