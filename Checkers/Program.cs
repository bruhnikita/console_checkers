using System;

namespace Checkers
{
    public class Game
    {
        private Checker[,] board;
        private Color currentPlayer;

        public enum Color { White, Black }

        public class Checker
        {
            public bool IsKing { get; set; }
            public Color Color { get; set; }

            public Checker(Color color)
            {
                Color = color;
                IsKing = false;
            }
        }

        public Game()
        {
            board = new Checker[8, 8];
            currentPlayer = Color.White;
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        if (i < 3)
                            board[i, j] = new Checker(Color.Black);
                        else if (i > 4)
                            board[i, j] = new Checker(Color.White);
                    }
                }
            }
        }

        private Checker GetCheckerAtPosition(int x, int y)
        {
            return (x >= 0 && x < 8 && y >= 0 && y < 8) ? board[y, x] : null;
        }

        private bool IsValidMove(int fromX, int fromY, int toX, int toY)
        {
            Checker fromChecker = GetCheckerAtPosition(fromX, fromY);
            Checker toChecker = GetCheckerAtPosition(toX, toY);

            if (fromChecker == null || fromChecker.Color != currentPlayer || toChecker != null)
                return false;

            int direction = (currentPlayer == Color.White) ? 1 : -1;

            // Обычный ход
            if (Math.Abs(toX - fromX) == 1 && (toY - fromY == direction))
                return true;

            // Ход с рубкой
            if (Math.Abs(toX - fromX) == 2 && (toY - fromY == direction * 2))
            {
                int midX = (fromX + toX) / 2;
                int midY = (fromY + toY) / 2;
                Checker midChecker = GetCheckerAtPosition(midX, midY);
                if (midChecker != null && midChecker.Color != currentPlayer)
                    return true;
            }

            return false;
        }

        private void MakeMove(int fromX, int fromY, int toX, int toY)
        {
            if (IsValidMove(fromX, fromY, toX, toY))
            {
                Checker checker = GetCheckerAtPosition(fromX, fromY);
                board[toY, toX] = checker;
                board[fromY, fromX] = null;

                // Удаляем срубленную шашку
                if (Math.Abs(toX - fromX) == 2)
                {
                    int midX = (fromX + toX) / 2;
                    int midY = (fromY + toY) / 2;
                    board[midY, midX] = null;
                }

                // Проверка на королеву
                if ((currentPlayer == Color.White && toY == 7) || (currentPlayer == Color.Black && toY == 0))
                    checker.IsKing = true;

                currentPlayer = (currentPlayer == Color.White) ? Color.Black : Color.White;
            }
            else
            {
                Console.WriteLine("Неверный ход. Попробуйте еще раз.");
            }
        }

        private void DrawBoard()
        {
            Console.Clear();
            Console.WriteLine("  A B C D E F G H");
            Console.WriteLine(" +-----------------+");
            for (int y = 0; y < 8; y++)
            {
                Console.Write(y + 1 + "|");
                for (int x = 0; x < 8; x++)
                {
                    if ((x + y) % 2 == 0)
                    {
                        Console.Write("  ");
                    }
                    else
                    {
                        Checker checker = GetCheckerAtPosition(x, y);
                        if (checker != null)
                        {
                            string piece = checker.IsKing ? (checker.Color == Color.White ? "W+" : "B+") : (checker.Color == Color.White ? "W" : "B");
                            Console.Write(piece + " ");
                        }
                        else
                        {
                            Console.Write(". ");
                        }
                    }
                }
                Console.WriteLine("|");
            }
            Console.WriteLine(" +-----------------+");
        }

        private bool CheckGameOver()
        {
            bool whiteExists = false;
            bool blackExists = false;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Checker checker = GetCheckerAtPosition(x, y);
                    if (checker != null)
                    {
                        if (checker.Color == Color.White)
                            whiteExists = true;
                        if (checker.Color == Color.Black)
                            blackExists = true;
                    }
                }
            }

            if (!whiteExists)
            {
                Console.WriteLine("Черные выиграли!");
                return true;
            }
            else if (!blackExists)
            {
                Console.WriteLine("Белые выиграли!");
                return true;
            }

            return false;
        }

        static void Main(string[] args)
        {
            Game game = new Game();

            while (true)
            {
                game.DrawBoard();
                Console.WriteLine($"Ход: {game.currentPlayer}");
                Console.WriteLine("Введите координаты хода (например, A 1 B 2):");
                string input = Console.ReadLine();
                var parts = input.Split(' ');

                if (parts.Length != 4 || parts[0].Length != 1 || parts[2].Length != 1)
                {
                    Console.WriteLine("Неверный ввод. Попробуйте еще раз.");
                    continue;
                }

                int fromX = parts[0][0] - 'A';
                int fromY = int.Parse(parts[1]) - 1;
                int toX = parts[2][0] - 'A';
                int toY = int.Parse(parts[3]) - 1;

                game.MakeMove(fromX, fromY, toX, toY);

                if (game.CheckGameOver())
                    break;
            }
        }
    }
}
