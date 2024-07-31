using UnityEngine;

public class SudokuHandler : MonoBehaviour {
    private const int GridSize = 9;

    // Generate a Sudoku puzzle
    public int[,] GenerateSudoku() {
        int[,] board = new int[GridSize, GridSize];
        GenerateSudokuHelper(board);
        return board;
    }

    // Helper function to generate Sudoku recursively
    private bool GenerateSudokuHelper(int[,] board) {
        int row, col;

        // Find an unassigned cell
        if (!FindUnassignedCell(board, out row, out col)) {
            return true; // If all cells are assigned, puzzle is complete
        }

        // Generate random permutation of numbers 1 to GridSize
        int[] numbers = GenerateRandomPermutation();

        // Try placing a number in the cell
        foreach (int num in numbers) {
            if (IsSafe(board, row, col, num)) {
                board[row, col] = num;

                // Recur to fill remaining cells
                if (GenerateSudokuHelper(board)) {
                    return true; // If solution found, return true
                }

                // If placing num at (row, col) doesn't lead to solution, backtrack
                board[row, col] = 0;
            }
        }
        // No number can be placed, return false to trigger backtracking
        return false;
    }

    // Find an unassigned cell
    private static bool FindUnassignedCell(int[,] board, out int row, out int col) {
        for (row = 0; row < GridSize; row++) {
            for (col = 0; col < GridSize; col++) {
                if (board[row, col] == 0) {
                    return true;
                }
            }
        }
        row = -1; col = -1; // If no unassigned cell found
        return false;
    }

    // Check if it's safe to place num at (row, col)
    public bool IsSafe(int[,] board, int row, int col, int num) {
        // Check row and column
        for (int i = 0; i < GridSize; i++) {
            if (board[row, i] == num || board[i, col] == num) {
                return false;
            }
        }

        // Check 3x3 square
        int boxRow = row - row % 3;
        int boxCol = col - col % 3;
        for (int i = boxRow; i < boxRow + 3; i++) {
            for (int j = boxCol; j < boxCol + 3; j++) {
                if (board[i, j] == num) {
                    return false;
                }
            }
        }

        return true;
    }

    // Check if it's safe placed num at (row, col)
    public bool IsSafePlaced(int[,] board, int row, int col, int num) {
        // Check row and column
        for (int i = 0; i < GridSize; i++) {
            if (board[row, i] == num && i == col) {
                continue;
            }
            if (board[i, col] == num && i == row) {
                continue;
            }
            if (board[row, i] == num || board[i, col] == num) {
                return false;
            }
            if (board[row,i]==0 & IsSafe(board, row,i,num)) { 
                //return false;
            }
            if (board[i, col] == 0 & IsSafe(board, i, col, num)) {
                //return false;
            }
        }

        // Check 3x3 square
        int boxRow = row - row % 3;
        int boxCol = col - col % 3;
        for (int i = boxRow; i < boxRow + 3; i++) {
            for (int j = boxCol; j < boxCol + 3; j++) {
                if (i == row && j == col) {
                    continue;
                }
                if (board[i, j] == num) {
                    return false;
                }
                if (board[i, j] == 0 & IsSafe(board, i, j, num)) {
                    //return false;
                }
            }
        }

        return true;
    }

    // Generate a random permutation of numbers 1 to GridSize
    private int[] GenerateRandomPermutation() {
        int[] numbers = new int[GridSize];
        for (int i = 0; i < GridSize; i++) {
            numbers[i] = i + 1;
        }

        System.Random rand = new System.Random();
        for (int i = GridSize - 1; i > 0; i--) {
            int j = rand.Next(i + 1);
            int temp = numbers[i];
            numbers[i] = numbers[j];
            numbers[j] = temp;
        }

        return numbers;
    }



    public int[,] RemoveNumbers(int[,] board, int numberOfCellsToRemove) {// numberOfCellsToRemove [40-70]
        System.Random random = new System.Random();
        int cellsRemoved = 0;

        while (cellsRemoved < numberOfCellsToRemove) {
            int row = random.Next(GridSize);
            int col = random.Next(GridSize);

            // Skip if the cell is already empty
            if (board[row, col] == 0) {
                continue;
            }

            // Backup the current value in case we need to backtrack
            int backup = board[row, col];
            board[row, col] = 0; // Remove the number from the cell

            // Check if the puzzle still has a unique solution
            if (!HasUniqueSolution(board)) {
                // If the puzzle doesn't have a unique solution, revert the change
                board[row, col] = backup;
            }
            else {
                cellsRemoved++;
            }
        }
        return board;
    }

    public bool[,] AsignSafePositionsBoard(int[,] board) {
        bool[,] boardSafePosition = new bool[board.GetLength(0), board.GetLength(1)];
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                if (board[i, j] == 0) boardSafePosition[i, j] = false;
                else boardSafePosition[i, j] = true;
            }
        }
        return boardSafePosition;
    }

    private bool HasUniqueSolution(int[,] board) {
        int[,] tempBoard = (int[,])board.Clone(); // Clone the board to avoid modifying the original
        return SolveSudoku(tempBoard);
    }

    public bool SolveSudoku(int[,] board) {
        int row = 0, col = 0;
        bool isDone = true;

        // Find unassigned cell
        for (row = 0; row < GridSize; row++) {
            for (col = 0; col < GridSize; col++) {
                if (board[row, col] == 0) {
                    isDone = false;
                    break;
                }
            }
            if (!isDone) {
                break;
            }
        }

        // No unassigned cells left
        if (isDone) {
            return true;
        }

        // Try every digit from 1 to 9
        for (int num = 1; num <= GridSize; num++) {
            if (IsSafe(board, row, col, num)) {
                board[row, col] = num;
                if (SolveSudoku(board)) {
                    return true;
                }
                board[row, col] = 0; // backtrack
            }
        }

        return false;
    }

    public Hint GetHint(int[,] board) {
        int row = 0, col = 0;
        bool isDone = true;

        // Find unassigned cell
        for (row = 0; row < GridSize; row++) {
            for (col = 0; col < GridSize; col++) {
                if (board[row, col] == 0) {
                    for (int num = 1; num <= GridSize; num++) {
                        if (IsSafe(board, row, col, num)) {
                            // Check 3x3 square
                            int boxRow = row - row % 3;
                            int boxCol = col - col % 3;
                            bool definitely = true;
                            for (int i = boxRow; i < boxRow + 3; i++) {
                                for (int j = boxCol; j < boxCol + 3; j++) {
                                    if ((i == row && j == col) || !definitely) {
                                        continue;
                                    }
                                    if (board[i, j] == 0) {
                                        // Check row and column
                                        if (IsSafe(board, i, j, num)) {
                                            definitely = false;
                                        }
                                    }
                                }
                            }
                            if (definitely) {
                                 return new Hint(row, col, num);
                            }
                        }
                    }
                }
            }
            if (!isDone) {
                break;
            }
        }
        throw new System.Exception("Can`t find hint.");
    }

}