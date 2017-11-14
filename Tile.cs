using System;

public class Tile
{
	// Types:
	// NORMAL
	// GENERATED
	// DELETED
	// MOVING
	// MERGING

	public Tile (int value, int row, int col)
	{
		Value = value;
		Row = row;
		Col = col;
		PreviousValue = value;
		PreviousRow = row;
		PreviousCol = col;
		TypeOf = "NORMAL";
		Animating = false;
		ResetShift ();
	}

	public int Value { get; set; }

	public int Row { get; set; }

	public int Col { get; set; }

	public int PreviousValue { get; set; }

	public int PreviousRow { get; set; }

	public int PreviousCol { get; set; }

	public double ShiftX { get; set; }

	public double ShiftY { get; set; }

	public string TypeOf { get; set; }

	public bool Animating { get; set; }

	public void SetPosition (int row, int col)
	{
		Row = row;
		Col = col;
	}

	public void ResetShift ()
	{
		ShiftX = 0;
		ShiftY = 0;
	}

	public int Merging ()
	{
		return (Value += Value);
	}

	/**
	 * checks whether numbered (nonzero) tile changes its position
	 * @param row
	 * @param col
	 * @return true if tile changed its position, false if not
	 */
	public bool HasMoved (int row, int col)
	{
		if (!IsEmpty () && ((Row != row) || (Col != col))) {
			Animating = true;
			return true;
		} else {
			return false;
		}
	}

	public bool IsEmpty ()
	{
		return (Value == 0);
	}

}