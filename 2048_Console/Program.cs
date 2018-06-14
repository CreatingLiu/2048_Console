using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace liushifu.game.game2048_Console
{
    class Program
    {
        const string loadPath = "2048save.dat";

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = "2048小游戏-By:刘师傅 V0.0.2 alpha";
            Console.SetWindowSize(68, 30);
            Console.SetBufferSize(68, 30);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

        start:
            int[,] num = new int[4, 4];
            int[,] last = new int[4, 4];
            num = SetNewNum(num);
            num = SetNewNum(num);
            last = CopyToB(num);

            RePaint(num, "", "");

            while (true)
            {
                ConsoleKeyInfo k = Console.ReadKey();
                switch (k.Key)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        last = CopyToB(num);
                        num = SquareRot90(num, 3);
                        num = Merge(num);
                        num = SquareRot90(num, 1);
                        break;
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        last = CopyToB(num);
                        num = SquareRot90(num, 1);
                        num = Merge(num);
                        num = SquareRot90(num, 3);
                        break;
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        last = CopyToB(num);
                        num = Merge(num);
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        last = CopyToB(num);
                        num = SquareRot90(num, 2);
                        num = Merge(num);
                        num = SquareRot90(num, 2);
                        break;
                    case ConsoleKey.H:
                        Help();
                        RePaint(num, "", "");
                        continue; ;
                    case ConsoleKey.Z:
                        num = CopyToB(last);
                        RePaint(num, "", "");
                        continue;
                    case ConsoleKey.X:
                        Save(num);
                        RePaint(num, "", "                             存档保存成功！");
                        continue;
                    case ConsoleKey.L:
                        num = Load();
                        RePaint(num, "", "                             读取存档成功！");
                        continue;
                    case ConsoleKey.R:
                        goto start;
                    case ConsoleKey.Q:
                        Console.SetWindowSize(68, 30);
                        RePaint(num, "", "");
                        continue;
                    default:
                        RePaint(num, "", "");
                        continue;
                }

                if (!CanMove(num))
                {
                    RePaint(num, "                               Game Over!", "                       请按“R”键重新开始游戏");
                    ConsoleKeyInfo r;
                    do
                    {
                        r = Console.ReadKey();
                        RePaint(num, "                               Game Over!", "                       请按“R”键重新开始游戏");
                    } while (r.Key != ConsoleKey.R);

                    goto start;
                }
                else
                {
                    num = SetNewNum(num);
                    RePaint(num, "", "");
                }
            }
        }


        public static bool CanMove(int[,] a)
        {
            bool res = false;
            int[,] b = CopyToB(a);
            b = Merge(b);
            if (!IsEquals(a, b))
                res = true;
            b = CopyToB(a);
            b = SquareRot90(b, 1);
            b = Merge(b);
            b = SquareRot90(b, -1);
            if (!IsEquals(a, b))
                res = true;
            b = CopyToB(a);
            b = SquareRot90(b, 2);
            b = Merge(b);
            b = SquareRot90(b, -2);
            if (!IsEquals(a, b))
                res = true;
            b = CopyToB(a);
            b = SquareRot90(b, 3);
            b = Merge(b);
            b = SquareRot90(b, -3);
            if (!IsEquals(a, b))
                res = true;
            return res;
        }

        public static bool IsEquals(int[,] a, int[,] b)
        {
            bool res = true;
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (b[i, j] != a[i, j])
                    {
                        res = false;
                        break;
                    }
                }
                if (!res)
                    break;
            }
            return res;
        }

        public static int[,] CopyToB(int[,] a)
        {
            int[,] b = new int[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    b[i, j] = a[i, j];
                }
            }
            return b;
        }

        public static int[,] Merge(int[,] a)
        {
            for (int i = 0; i < a.GetLength(0); i++)
            {
                int lastNum = 0;
                int last_j = 0;
                for (int j = 0; j < a.GetLength(1); j++)//合并
                {
                    if (lastNum != a[i, j] && a[i, j] != 0)
                    {
                        lastNum = a[i, j];
                        last_j = j;
                    }
                    else if (lastNum == a[i, j])
                    {
                        a[i, last_j] = 0;
                        a[i, j] = lastNum + a[i, j];
                    }
                }
                last_j = 0;
                for (int j = 0; j < a.GetLength(1); j++)//整理
                {
                    if (a[i, j] != 0)
                    {
                        a[i, last_j] = a[i, j];
                        if (last_j != j)
                            a[i, j] = 0;
                        last_j++;
                    }
                }
            }
            return a;
        }

        public static int[,] SquareRot90(int[,] a, int rotNum)
        {
            while (rotNum < 0)
            {
                rotNum += 4;
            }
            for (int rot_i = 0; rot_i < rotNum; rot_i++)
            {
                int[,] b = new int[a.GetLength(1), a.GetLength(0)];
                for (int i = 0; i < a.GetLength(0); i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        b[j, a.GetLength(0) - i - 1] = a[i, j];
                    }
                }
                a = b;
            }
            return a;
        }

        public class Point
        {
            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
            public int X
            {
                get;
                set;
            }
            public int Y
            {
                get;
                set;
            }
        }

        public static Point RandomPoint(int[,] a)
        {
            List<Point> lstP = new List<Point>();
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (a[i, j] == 0)
                    {
                        lstP.Add(new Point(i, j));
                    }
                }
            }
            if (lstP.Count == 0)
            {
                return null;
            }
            int rnd = new Random().Next(lstP.Count);
            return lstP[rnd];
        }

        public static void RePaint(int[,] a, string tip1, string tip2)
        {
            string[,] b = new string[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    switch (a[i, j])
                    {
                        case 0:
                            b[i, j] = "          ";
                            break;
                        case 2:
                        case 4:
                        case 8:
                            b[i, j] = "     " + a[i, j].ToString() + "    ";
                            break;
                        case 16:
                        case 32:
                        case 64:
                            b[i, j] = "    " + a[i, j].ToString() + "    ";
                            break;
                        case 128:
                        case 256:
                        case 512:
                            b[i, j] = "    " + a[i, j].ToString() + "   ";
                            break;
                        case 1024:
                        case 2048:
                        case 4096:
                        case 8192:
                            b[i, j] = "   " + a[i, j].ToString() + "   ";
                            break;
                        default:
                            b[i, j] = "   " + a[i, j].ToString() + "  ";
                            break;
                    }
                }
            }


            Console.Clear();
            Console.Write("\n");
            Console.WriteLine("    ┌────────────────────────────────────────────────────────────┐");
            Console.WriteLine("    │                                                            │");
            Console.WriteLine("    │               2048小游戏   version:V0.0.2 alpha            │");
            Console.WriteLine("    │                                                            │");
            Console.WriteLine("    │                      刘师出品，必属精品                    │");
            Console.WriteLine("    │                                                            │");
            Console.WriteLine("    │                       按“H”键显示帮助                    │");
            Console.WriteLine("    │                                                            │");
            Console.WriteLine("    └────────────────────────────────────────────────────────────┘");
            Console.WriteLine(tip1);
            Console.WriteLine(tip2);
            Console.WriteLine("            ┏━━━━━━━━━━┳━━━━━━━━━━┳━━━━━━━━━━┳━━━━━━━━━━┓");
            Console.WriteLine("            ┃          ┃          ┃          ┃          ┃");
            Console.WriteLine("            ┃" + b[0, 0] + "┃" + b[0, 1] + "┃" + b[0, 2] + "┃" + b[0, 3] + "┃");
            Console.WriteLine("            ┃          ┃          ┃          ┃          ┃");
            Console.WriteLine("            ┣━━━━━━━━━━╋━━━━━━━━━━╋━━━━━━━━━━╋━━━━━━━━━━┫");
            Console.WriteLine("            ┃          ┃          ┃          ┃          ┃");
            Console.WriteLine("            ┃" + b[1, 0] + "┃" + b[1, 1] + "┃" + b[1, 2] + "┃" + b[1, 3] + "┃");
            Console.WriteLine("            ┃          ┃          ┃          ┃          ┃");
            Console.WriteLine("            ┣━━━━━━━━━━╋━━━━━━━━━━╋━━━━━━━━━━╋━━━━━━━━━━┫");
            Console.WriteLine("            ┃          ┃          ┃          ┃          ┃");
            Console.WriteLine("            ┃" + b[2, 0] + "┃" + b[2, 1] + "┃" + b[2, 2] + "┃" + b[2, 3] + "┃");
            Console.WriteLine("            ┃          ┃          ┃          ┃          ┃");
            Console.WriteLine("            ┣━━━━━━━━━━╋━━━━━━━━━━╋━━━━━━━━━━╋━━━━━━━━━━┫");
            Console.WriteLine("            ┃          ┃          ┃          ┃          ┃");
            Console.WriteLine("            ┃" + b[3, 0] + "┃" + b[3, 1] + "┃" + b[3, 2] + "┃" + b[3, 3] + "┃");
            Console.WriteLine("            ┃          ┃          ┃          ┃          ┃");
            Console.WriteLine("            ┗━━━━━━━━━━┻━━━━━━━━━━┻━━━━━━━━━━┻━━━━━━━━━━┛");
        }

        public static void Save(int[,] a)
        {
            string b = "";

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    b = b + a[i, j] + "|";
                }
            }

            b = b.Remove(b.Length - 1);

            File.WriteAllText(loadPath, b);

        }

        public static int[,] Load()
        {
            if (!File.Exists(loadPath))
            {
                return null;
            }

            string b = File.ReadAllText(loadPath);

            string[] c = b.Split('|');
            int[,] d = new int[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    d[i, j] = Convert.ToInt32(c[i * 4 + j]);
                }
            }
            return d;
        }

        public static void Help()
        {
            Console.Clear();

            Console.Write("\n");
            Console.WriteLine("    ┌────────────────────────────────────────────────────────────┐");
            Console.WriteLine("    │                        2048游戏帮助                        │");
            Console.WriteLine("    ├────────────────────────────────────────────────────────────┤");
            Console.WriteLine("    │                          按键提示                          │");
            Console.WriteLine("    │                                                            │");
            Console.WriteLine("    │     ↑↓←→、WASD：游戏操作                               │");
            Console.WriteLine("    │                  R：重玩                                   │");
            Console.WriteLine("    │                  Z：回到上一步（只能撤销一次）             │");
            Console.WriteLine("    │                  X：保存（只能保存一个存档）               │");
            Console.WriteLine("    │                  L：加载存档                               │");
            Console.WriteLine("    │                  Q：重绘窗口                               │");
            Console.WriteLine("    │                     如果显示出现问题请按此键               │");
            Console.WriteLine("    ├────────────────────────────────────────────────────────────┤");
            Console.WriteLine("    │         请勿改变游戏窗口大小，否则显示将会出现异常         │");
            Console.WriteLine("    │         如果不小心导致显示出现问题请按“Q”键重置          │");
            Console.WriteLine("    ├────────────────────────────────────────────────────────────┤");
            Console.WriteLine("    │                    请切换至英文输入法操作                  │");
            Console.WriteLine("    ├────────────────────────────────────────────────────────────┤");
            Console.WriteLine("    │         存档保存在游戏根目录下，文件名为2048save.dat       │");
            Console.WriteLine("    │                可根据自己需要备份或发送给好友              │");
            Console.WriteLine("    ├────────────────────────────────────────────────────────────┤");
            Console.WriteLine("    │                                                            │");
            Console.WriteLine("    │                      请按任意键回到游戏                    │");
            Console.WriteLine("    │                                                            │");
            Console.WriteLine("    ├────────────────────────────────────────────────────────────┤");
            Console.WriteLine("    │                 2048    version 0.0.2 alpha                │");
            Console.WriteLine("    │                      刘师出品，必属精品！                  │");
            Console.WriteLine("    └────────────────────────────────────────────────────────────┘");

            Console.ReadKey();
        }

        public static int[,] SetNewNum(int[,] a)
        {
            Point rp;
            rp = RandomPoint(a);
            if (rp != null)
                a[rp.X, rp.Y] = 2;
            return a;
        }
    }
}
