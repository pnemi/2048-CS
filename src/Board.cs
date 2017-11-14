using System;
using System.IO;
using System.Collections.Generic;

public class Board {

	public Board (int size) {
		Size = size;
		Score = 0;
		EmptyTiles = Size * Size;
		InitTiles = 2;
		GameOver = false;
		GenNewTile = false;
		Tiles = new List<List<Tile>>();

		Start();
	}

	private readonly string FILEPATH = @"highest-score.txt";

	public int Size { get; set; } 						// Size of the grid
	public int Score { get; set; }						// overall Score
	public int HighestScore { get; set; }				// overall Score
	public int EmptyTiles { get; set; }					// number of tiles with zero value
	public int InitTiles { get; set; } 					// number of tiles board starts with (usually two tiles)
	public bool GameOver { get; set; } 					// game is over when 2048 tile is found
	public string WonOrLost { get; set; }				// won or lost
	public bool GenNewTile { get; set; }				// generate new tile when any tile moved
	public List<List<Tile>> Tiles { get; set; }			// board

	private void Initialize() {
		for (int row = 0; row < Size; row++) {
			Tiles.Add(new List<Tile>());
			for (int col = 0; col < Size; col++) {
				Tiles[row].Add(new Tile(0, row, col));
			}
		}

		if (!File.Exists (FILEPATH)) {
			HighestScore = 0;
			File.WriteAllText (FILEPATH, "0");
		} else {
			HighestScore = Convert.ToInt32(File.ReadAllText (FILEPATH));
		}

	}

	private void Start() {
		Initialize();
		GenInitTiles();
		//Show();
	}
		
	public Tile GetTileAt(int row, int col) {
		return Tiles[row][col];
	}

	public void SetTileAt(int row, int col, Tile t) {
		Tiles[row][col] = t;
	}

	public void RemTileAt(int row, int col) {
		Tiles[row].RemoveAt(col);
	}

	public void updateScore(int addition) {
		Score += addition;
		if (Score > HighestScore) {
			HighestScore = Score;
			File.WriteAllText (FILEPATH, Convert.ToString(HighestScore));
		}
	}

	/**
	 * merges two touching {@link Tile} with the same number into one
	 * @param sequence of {@link Tile}
	 * @return merged sequence of {@link Tile}
	 */
	private List<Tile> MergeTilesToRight(List<Tile> sequence) {
		for (int l = 0; l < sequence.Count - 1; l++) {
			if (sequence[l].Value == sequence[l + 1].Value) {
				int value;
				if ((value = sequence[l].Merging()) == 2048) {
					GameOver = true;
				}
				updateScore (value);
				sequence.RemoveAt(l + 1);
				GenNewTile = true; // board has changed its state
				EmptyTiles++;
			}
		}
		return sequence;
	}

	/**
	 * merges two touching {@link Tile} with the same number into one
	 * @param sequence of {@link Tile}
	 * @return merged sequence of {@link Tile}
	 */
	private List<Tile> MergeTilesToLeft(List<Tile> sequence) {
		for (int l = sequence.Count - 1; l > 0; l--) {
			if (sequence[l].Value == sequence[l - 1].Value) {
				int value;
				if ((value = sequence[l].Merging()) == 2048) {
					GameOver = true;
				}
				updateScore (value);
				sequence.RemoveAt(l - 1);
				GenNewTile = true; // board has changed its state
				EmptyTiles++;
			}
		}
		return sequence;
	}

	/**
	 * creates empty {@link Tile} instances and adds them to the left (resp. top) of merged sequence to fit the board
	 * @param merged sequence of {@link Tile}
	 * @return refilled sequence with required number of empty {@link Tile}
	 */
	private List<Tile> AddEmptyTilesFirst(List<Tile> merged) {
		for (int k = merged.Count; k < Size; k++) {
			merged.Insert(0, new Tile(0, 0, 0));
		}
		return merged;
	}

	/**
	 * creates empty {@link Tile} instances and adds them to the right (resp. bottom) of merged sequence to fit the board
	 * @param merged sequence of {@link Tile}
	 * @return refilled sequence with required number of empty {@link Tile}
	 */
	private List<Tile> AddEmptyTilesLast(List<Tile> merged) { // boolean last/first
		for (int k = merged.Count; k < Size; k++) {
			merged.Insert(k, new Tile(0, 0, 0));
		}
		return merged;
	}

	private List<Tile> RemoveEmptyTilesRows(int row) {

		List<Tile> moved = new List<Tile>();

		for (int col = 0; col < Size; col++) {
			if (!GetTileAt(row, col).IsEmpty()) { // NOT empty
				moved.Add(GetTileAt(row, col));
			}
		}

		return moved;
	}

	private List<Tile> RemoveEmptyTilesCols(int row) {

		List<Tile> moved = new List<Tile>();

		for (int col = 0; col < Size; col++) {
			if (!GetTileAt(col, row).IsEmpty()) { // NOT empty
				moved.Add(GetTileAt(col, row));
			}
		}

		return moved;
	}

	private List<Tile> setRowToBoard(List<Tile> moved, int row) {
		for (int col = 0; col < Tiles.Count; col++) {
			if (moved[col].HasMoved(row, col)) {
				GenNewTile = true;
				Grid.TilesAnimationsDone--;
			}
			SetTileAt(row, col, moved[col]);
		}

		return moved;
	}

	private List<Tile> SetColToBoard(List<Tile> moved, int row) {
		for (int col = 0; col < Tiles.Count; col++) {
			if (moved[col].HasMoved(col, row)) {
				GenNewTile = true;
				Grid.TilesAnimationsDone--;
			}			
			SetTileAt(col, row, moved[col]);
		}

		return moved;
	}

	public void MoveUp() {

		List<Tile> moved;

		for (int row = 0; row < Size; row++) {

			moved = RemoveEmptyTilesCols(row);
			moved = MergeTilesToRight(moved);
			moved = AddEmptyTilesLast(moved);
			moved = SetColToBoard(moved, row);

		}

	}

	public void MoveDown() {

		List<Tile> moved;

		for (int row = 0; row < Size; row++) {

			moved = RemoveEmptyTilesCols(row);
			moved = MergeTilesToLeft(moved);
			moved = AddEmptyTilesFirst(moved);
			moved = SetColToBoard(moved, row);

		}

	}

	public void MoveLeft() {

		List<Tile> moved;

		for (int row = 0; row < Size; row++) {

			moved = RemoveEmptyTilesRows(row);
			moved = MergeTilesToLeft(moved);
			moved = AddEmptyTilesLast(moved);
			moved = setRowToBoard(moved, row);

		}

	}

	public void MoveRight() {

		List<Tile> moved;

		for (int row = 0; row < Size; row++) {

			moved = RemoveEmptyTilesRows(row);
			moved = MergeTilesToRight(moved);
			moved = AddEmptyTilesFirst(moved);
			moved = setRowToBoard(moved, row);

		}

	}

	public void IsGameOver() {
		if (GameOver) {
			// vyhrál jsi (na desce je dláždice 2048)
			// End(true);
			WonOrLost = "WON";
		} else {
			if (IsFull()) {
				if (!IsMovePossible()) {
					// you lost (board is full with no tiles to merge)
					// End(false);
					WonOrLost = "LOST";
				}

			} else {
				NewRandomTile(); // game continues
			}
		}
	}

	private bool IsFull() {
		return EmptyTiles == 0;
	}

	private bool IsMovePossible() {
		for (int row = 0; row < Size; row++) {
			for (int col = 0; col < Size - 1; col++) {
				if (GetTileAt(row, col).Value == GetTileAt(row, col + 1).Value) {
					return true;
				}
			}
		}

		for (int row = 0; row < Size - 1; row++) {
			for (int col = 0; col < Size; col++) {
				if (GetTileAt(col, row).Value == GetTileAt(col, row + 1).Value) {
					return true;
				}
			}
		}
		return false;
	}

	private void GenInitTiles() {
		for (int i = 0; i < InitTiles; i++) {
			GenNewTile = true;
			NewRandomTile();
		}
	}

	private void NewRandomTile() {
		if (GenNewTile) {
			int row;
			int col;
			Random rnd = new Random();
			int value = rnd.Next(11) < 9 ? 2 : 4;
			do {
				row = rnd.Next(0, 4);
				col = rnd.Next(0, 4);
			} while (GetTileAt(row, col).Value != 0);
			SetTileAt(row, col, new Tile(value, row, col));
			EmptyTiles--;
			GenNewTile = false;
		}
	}

	public void SetDefaultPositions() {
		for (int i = 0; i < Tiles.Count; i++) {
			for (int j = 0; j < Tiles[i].Count; j++) {
				if (GetTileAt (i, j).Value != 0) {
					GetTileAt (i, j).SetPosition (i, j);
				}
			}
		}
	}

	public void Show() {
		for (int i = 0; i < 2; ++i) Console.WriteLine();
		Console.WriteLine("Score: {0}", Score);
		for (int i = 0; i < Tiles.Count; i++) {
			for (int j = 0; j < Tiles[i].Count; j++) {
				Console.Write ("{0}", String.Format("{0,5:D}", GetTileAt(i, j).Value));
				Console.Write (" [{0}, {1}]", GetTileAt(i,j).Row, GetTileAt(i,j).Col);
			}
			Console.WriteLine();
		}
	}

	public void PreviousShow() {
		for (int i = 0; i < 2; ++i) Console.WriteLine();
		Console.WriteLine("Score: {0}", Score);
		for (int i = 0; i < Tiles.Count; i++) {
			for (int j = 0; j < Tiles[i].Count; j++) {
				Console.Write("{0}", String.Format("{0,5:D}", GetTileAt(i, j).PreviousValue));
				Console.Write (" [{0}, {1}]", GetTileAt(i,j).PreviousRow, GetTileAt(i,j).PreviousCol);
			}
			Console.WriteLine();
		}
	}

}