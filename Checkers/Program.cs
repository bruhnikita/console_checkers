using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkers
{
    public class Game
    {
        private Item[] checkersList;
        private State currentState;
        private Item pickedUpItem;
        public class Item
        {
            public bool IsKing { get; set; }
            public int posX { get; set; }
            public int posY { get; set; }
            public Color Color { get; set; }
        }

        public Game()
        {
            checkersList = new Item[24];
            currentState = State.WhiteTurn;
            pickedUpItem = null;
            InitializeCheckers();
            InitializeBoard();
        }

        private enum State
        {
            WhiteTurn,
            WhiteTurnItemPickedUp,
            BlackTurn,
            BlackTurnItemPickedUp,
            GameOver
        }

        private void InitializeCheckers()
        {
            for (int i = 0; i < 12; i++)
            {
                Item cheker = new Item();
                cheker.IsKing = false;
                cheker.Color = Color.White;

                int row = i / 4;
                int col = i % 4;

                cheker.posX = col;
                cheker.posY = row;

                checkersList[i] = cheker;
            }

            for (int i = 12; i < 24; i++)
            {
                Item cheker = new Item();
                cheker.IsKing = false;
                cheker.Color = Color.Black;

                int row = (i - 12) / 4;
                int col = (i - 12) % 4;
                cheker.posX = col;
                cheker.posY = 7 - row;

                checkersList[i] = cheker;
            }
        }

        private Item GetCheckerAtPosition(int fromX, int fromY)
        {
            foreach (Item item in checkersList)
            {
                if (item != null && item.posX == fromX && item.posY == fromY)
                {
                    return item;
                }
            }

            return null;
        }

        private bool IsValidMove(int fromX, int fromY, int toX, int toY, Color color)
        {
            if (fromX < 0 || fromX >= 8 || fromY < 0 || fromY >= 8 || toX < 0 || toX >= 8 || toY < 0 || toY >= 8)
            {
                return false;
            }

            if (fromX == toX && fromY == toY)
            {
                return false;
            }

            Item fromPositionChecker = GetCheckerAtPosition(fromX, fromY);
            if (fromPositionChecker == null && fromPositionChecker.Color != color)
            {
                return false;
            }

            Item checkerAtPos = GetCheckerAtPosition(toX, toY);
            if (checkerAtPos != null)
            {
                return false;
            }

            if (Math.Abs(toX - fromX) != Math.Abs(toY - fromY))
            {
                return false;
            }

            if (color == Color.White && fromY > toY)
            {
                return false;
            }
            if (color == Color.Black && fromY < toY)
            {
                return false;
            }

            if (Math.Abs(toX - fromX) == 2)
            {
                int midX = (fromX + toX) / 2;
                int midY = (fromY + toY) / 2;
                Item midPiece = GetCheckerAtPosition(midX, midY);
                if (midPiece == null || midPiece.Color == color)
                {
                    return false;
                }
            }

            return true;
        }


        private void MakeMove(int fromX, int fromY, int toX, int toY)
        {
            switch (currentState)
            {
                case State.WhiteTurn:
                    MakeWhiteMove(fromX, fromY, toX, toY);
                    UpdateBoard();
                    CheckGameOver();
                    break;

                case State.WhiteTurnItemPickedUp:
                    MakeWhiteMoveWithPickedUpItem(fromX, fromY, toX, toY);
                    UpdateBoard();
                    CheckGameOver();
                    break;

                case State.BlackTurn:
                    MakeBlackMove(fromX, fromY, toX, toY);
                    UpdateBoard();
                    CheckGameOver();
                    break;

                case State.BlackTurnItemPickedUp:
                    MakeBlackMoveWithPickedUpItem(fromX, fromY, toX, toY);
                    UpdateBoard();
                    CheckGameOver();
                    break;

                case State.GameOver:
                    break;
            }
        }

        private void UpdateBoard()
        {
            Console.Clear();
            InitializeBoard();
        }

        private void MakeWhiteMove(int fromX, int fromY, int toX, int toY)
        {
            Item checker = GetCheckerAtPosition(fromX, fromY);

            if (checker != null && IsValidMove(fromX, fromY, toX, toY, Color.White))
            {
                checker.posX = toX;
                checker.posY = toY;

                if (Math.Abs(toX - fromX) == 2)
                {
                    int midX = (fromX + toX) / 2;
                    int midY = (fromY + toY) / 2;
                    Item capturedChecker = GetCheckerAtPosition(midX, midY);
                    if (capturedChecker != null)
                    {
                        for (int i = 0; i < checkersList.Length; i++)
                        {
                            if (checkersList[i] == capturedChecker)
                            {
                                checkersList[i] = null;
                                break;
                            }
                        }
                    }
                }

                currentState = State.WhiteTurnItemPickedUp;
                pickedUpItem = checker;
            }
        }

        private void MakeBlackMove(int fromX, int fromY, int toX, int toY)
        {
            Item checker = GetCheckerAtPosition(fromX, fromY);

            if (checker != null && IsValidMove(fromX, fromY, toX, toY, Color.Black))
            {
                checker.posX = toX;
                checker.posY = toY;

                if (Math.Abs(toX - fromX) == 2)
                {
                    int midX = (fromX + toX) / 2;
                    int midY = (fromY + toY) / 2;
                    Item capturedChecker = GetCheckerAtPosition(midX, midY);
                    if (capturedChecker != null)
                    {
                        for (int i = 0; i < checkersList.Length; i++)
                        {
                            if (checkersList[i] == capturedChecker)
                    {
                            checkersList[i] = null;
                            break;
                        }
                    }
                }
            }

            currentState = State.BlackTurnItemPickedUp;
            pickedUpItem = checker;
        }
    

        private void MakeWhiteMoveWithPickedUpItem(int fromX, int fromY, int toX, int toY)
        {
            if (IsValidMove(fromX, fromY, toX, toY, Color.White))
            {
                pickedUpItem.posX = toX;
                pickedUpItem.posY = toY;

                if (Math.Abs(toX - fromX) == 2)
                {
                    int midX = (fromX + toX) / 2;
                    int midY = (fromY + toY) / 2;
                    Item capturedChecker = GetCheckerAtPosition(midX, midY);
                    if (capturedChecker != null)
                    {
                        for (int i = 0; i < checkersList.Length; i++)
                        {
                            if (checkersList[i] == capturedChecker)
                            {
                                checkersList[i] = null;
                                break;
                            }
                        }
                    }
                }

                currentState = State.BlackTurn;
                pickedUpItem = null;
            }
        }

        private void MakeBlackMove(int fromX, int fromY, int toX, int toY)
        {
            Item checker = GetCheckerAtPosition(fromX, fromY);

            if (checker != null && IsValidMove(fromX, fromY, toX, toY, Color.Black))
            {
                checker.posX = toX;
                checker.posY = toY;

                if (Math.Abs(toX - fromX) == 2)
                {
                    int midX = (fromX + toX) / 2;
                    int midY = (fromY + toY) / 2;
                    Item capturedChecker = GetCheckerAtPosition(midX, midY);
                    if (capturedChecker != null)
                    {
                        for (int i = 0; i < checkersList.Length; i++)
                        {
                            if (checkersList[i] == capturedChecker)
                            {
                                checkersList[i] = null;
                                break;
                            }
                        }
                    }
                }

                currentState = State.BlackTurnItemPickedUp;
                pickedUpItem = checker;
            }
        }

        private void MakeBlackMoveWithPickedUpItem(int fromX, int fromY, int toX, int toY)
        {
            if (IsValidMove(fromX, fromY, toX, toY, Color.Black))
            {
                pickedUpItem.posX = toX;
                pickedUpItem.posY = toY;

                if (Math.Abs(toX - fromX) == 2)
                {
                    int midX = (fromX + toX) / 2;
                    int midY = (fromY + toY) / 2;
                    Item capturedChecker = GetCheckerAtPosition(midX, midY);
                    if (capturedChecker != null)
                    {
                        for (int i = 0; i < checkersList.Length; i++)
                        {
                            if (checkersList[i] == capturedChecker)
                            {
                                checkersList[i] = null;
                                break;
                            }
                        }
                    }
                }

                currentState = State.WhiteTurn;
                pickedUpItem = null;
            }
        }

        private void CheckGameOver()
        {
            bool whiteCheckersExist = false;
            foreach (Item checker in checkersList)
            {
                if (checker != null && checker.Color == Color.White)
                {
                    whiteCheckersExist = true;
                    break;
                }
            }

            bool blackCheckersExist = false;
            foreach (Item checker in checkersList)
            {
                if (checker != null && checker.Color == Color.Black)
                {
                    blackCheckersExist = true;
                    break;
                }
            }

            if (!whiteCheckersExist)
            {
                Console.WriteLine("Черные выиграли!");
                currentState = State.GameOver;
            }
            else if (!blackCheckersExist)
            {
                Console.WriteLine("Белые выиграли!");
                currentState = State.GameOver;
            }
        }

        private void InitializeBoard()
        {
            char[,] board = new char[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        board[i, j] = ' ';
                    }
                    else
                    {
                        board[i, j] = '.';
                    }
                }
            }

            for (int i = 0; i < 12; i++)
            {
                int row = i / 4;
                int col = i % 4;
                board[row, col] = 'W';
            }

            for (int i = 12; i < 24; i++)
            {
                int row = 7 - (i - 12) / 4;
                int col = (i - 12) % 4;
                board[row, col] = 'B';
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        private void DrawChecker(int x, int y)
        {
            Item checker = GetCheckerAtPosition(x, y);
            if (checker != null)
            {
                Console.ForegroundColor = checker.Color == Color.White ? ConsoleColor.White : ConsoleColor.Black;
                Console.Write("O");
                Console.ResetColor();
            }
            else
            {
                Console.Write(" ");
            }
        }

        static void Main(string[] args)
        {
            Game checkers = new Game();

            while (checkers.currentState != Game.State.GameOver)
            {
                Console.WriteLine("Введите координаты хода (например, 1 2 3 4):");
                string[] input = Console.ReadLine().Split(' ');
                if (input.Length != 4)
                {
                    Console.WriteLine("Invalid input. Please enter four numbers separated by spaces.");
                    continue;
                }

                if (!int.TryParse(input[0], out int fromX) || !int.TryParse(input[1], out int fromY) || !int.TryParse(input[2], out int toX) || !int.TryParse(input[3], out int toY))
                {
                    Console.WriteLine("Invalid input. Please enter four numbers separated by spaces.");
                    continue;
                }

                if (fromX < 0 || fromX >= 8 || fromY < 0 || fromY >= 8 || toX < 0 || toX >= 8 || toY < 0 || toY >= 8)
                {
                    Console.WriteLine("Invalid input. Coordinates must be between 0 and 7.");
                    continue;
                }

                checkers.MakeMove(fromX, fromY, toX, toY);
            }
        }
    }
}