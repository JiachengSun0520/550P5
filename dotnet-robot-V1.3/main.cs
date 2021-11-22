using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Z3;
namespace dotnet
{
    //位置类型
    public enum LocType
    {
        SpaceLoc    = 0,    //将空白的位置设置为0
        GoalLoc     = 1,    //目标位置设置为1
        RobotLoc    = 2,    //机器人位置设置为2
        MoveLoc     = 3,    //可移动的冰块设置为3
        ObsLoc      = 4,    //障碍物设置为4
        RunLoc      = 5     //机器人走过的位置为5
    }
    //方位类型
    public enum DirType
    {
        Up_Dir          = 0, //往上走
        Down_Dir        = 1, //往下走
        Left_Dir        = 2, //往左走
        Right_Dir       = 3  //往右走
    }


    //表示物体位置信息
    public class nowLoc
    {
        private int lx;
        private int ly;
        //location type
        private int lt;
        //dir type
        private int dt;
        //坐标位置
        public nowLoc(int x, int y, int t)
        {
            Lx = x;
            Ly = y;
            Lt = t;
        }
         
        public int Ly { get => ly; set => ly = value; }
        public int Lx { get => lx; set => lx = value; }
        public int Dt { get => dt; set => dt = value; }
        internal int Lt { get => lt; set => lt = value; }
    }
    class main
    {
        //判断当前位置是否为目标位置
        public static List<nowLoc> listVist = new List<nowLoc>();
        //判断当前位置是否为目标位置
        public static nowLoc startLc;
        //判断当前位置是否为目标位置
        public static nowLoc endLc;
        public static int[,] nowSecen;
        //判断获取到的下一个位置
        public static nowLoc GetNextLoc()
        {
            nowLoc nl = new nowLoc(listVist[listVist.Count - 1].Lx, listVist[listVist.Count - 1].Ly, listVist[listVist.Count - 1].Lt);
            nl.Dt = listVist[listVist.Count - 1].Dt;
            //直接按照前进方向进行获取
            if (nl.Dt == (int)DirType.Up_Dir)
            {
                nl.Lx--;
                if (nl.Lx < 0)
                {
                    nl.Lx = 0;
                    if (nl.Ly < endLc.Ly && nowSecen[nl.Lx,nl.Ly+1] != (int)LocType.ObsLoc)
                    {
                        nl.Ly++;
                        nl.Dt = (int)DirType.Right_Dir;
                    }
                    else
                    {
                        nl.Ly--;
                        nl.Dt = (int)DirType.Left_Dir;
                    }
                }
            }
            else if (nl.Dt == (int)DirType.Down_Dir)
            {
                nl.Lx++;
                if (nl.Lx >= sizeX)
                {
                    nl.Lx = sizeX - 1;
                    if (nl.Ly < endLc.Ly && nowSecen[nl.Lx, nl.Ly+1] != (int)LocType.ObsLoc)
                    {
                        nl.Ly++;
                        nl.Dt = (int)DirType.Right_Dir;
                    }
                    else
                    {
                        nl.Ly--;
                        nl.Dt = (int)DirType.Left_Dir;
                    }
                }
            }
            else if (nl.Dt == (int)DirType.Left_Dir)
            {
                nl.Ly--;
                if (nl.Ly < 0)
                {
                    nl.Ly = 0;
                    if (nl.Lx < endLc.Lx && nowSecen[nl.Lx+1, nl.Ly] != (int)LocType.ObsLoc)
                    {
                        nl.Lx++;
                        nl.Dt = (int)DirType.Down_Dir;
                    }
                    else
                    {
                        nl.Lx--;
                        nl.Dt = (int)DirType.Up_Dir;
                    }
                }
            }
            else if (nl.Dt == (int)DirType.Right_Dir)
            {
                nl.Ly++;
                if (nl.Ly >= sizeY)
                {
                    nl.Ly = sizeY - 1;
                    if (nl.Lx < endLc.Lx && nowSecen[nl.Lx+1, nl.Ly] != (int)LocType.ObsLoc)
                    {
                        nl.Lx++;
                        nl.Dt = (int)DirType.Down_Dir;
                    }
                    else
                    {
                        nl.Lx--;
                        nl.Dt = (int)DirType.Up_Dir;
                    }
                }
            }
            nl.Lt = nowSecen[nl.Lx, nl.Ly];
            
            //如果是障碍物
            if (nl.Lt == (int)LocType.ObsLoc)
            {
                //先增加障碍物
                //listVist.Add(new nowLoc(nl.Lx, nl.Ly, nowSecen[nl.Lx, nl.Ly]));
                if (listVist[listVist.Count - 1].Lt == (int)LocType.MoveLoc)
                {
                    nl.Lx = listVist[listVist.Count - 2].Lx;
                    nl.Ly = listVist[listVist.Count - 2].Ly;
                    nl.Dt = listVist[listVist.Count - 2].Dt;
                }
                else
                {
                    //先退回坐标
                    nl.Lx = listVist[listVist.Count - 1].Lx;
                    nl.Ly = listVist[listVist.Count - 1].Ly;
                    nl.Lt = listVist[listVist.Count - 1].Lt;
                }
                //然后改变方向
                if (nl.Dt == (int)DirType.Left_Dir || nl.Dt == (int)DirType.Right_Dir) //左右改上下
                {
                    if(nl.Lx < endLc.Lx && nl.Lx + 1 < sizeX && nowSecen[nl.Lx+1, nl.Ly] != (int)LocType.ObsLoc)
                    {
                        nl.Dt = (int)DirType.Down_Dir;
                        nl.Lx++;
                    }
                    else if (nl.Lx > endLc.Lx && nl.Lx - 1 >= 0 && nowSecen[nl.Lx-1, nl.Ly] != (int)LocType.ObsLoc)
                    {
                        nl.Dt = (int)DirType.Up_Dir;
                        nl.Lx--;
                    }
                    else
                    {
                        if (nl.Dt == (int)DirType.Left_Dir && nowSecen[nl.Lx, nl.Ly + 1] != (int)LocType.ObsLoc)
                        {

                            nl.Dt = (int)DirType.Right_Dir;
                            nl.Ly++;
                        }
                        else
                        {
                            nl.Dt = (int)DirType.Left_Dir;
                            nl.Ly--;
                        } 
                    }
                }
                else  //上下改左右
                {
                    if (nl.Ly > endLc.Ly && nl.Ly - 1 >= 0 && nowSecen[nl.Lx, nl.Ly - 1] != (int)LocType.ObsLoc)
                    {
                        nl.Dt = (int)DirType.Left_Dir;
                        nl.Ly--;
                    }
                    else if (nl.Ly < endLc.Ly && nl.Ly + 1 < sizeY && nowSecen[nl.Lx, nl.Ly + 1] != (int)LocType.ObsLoc)
                    {
                        nl.Dt = (int)DirType.Right_Dir;
                        nl.Ly++;
                    }
                    else
                    {
                        if (nl.Dt == (int)DirType.Up_Dir && nowSecen[nl.Lx + 1, nl.Ly] != (int)LocType.ObsLoc)
                        {
                            nl.Dt = (int)DirType.Down_Dir;
                            nl.Lx++;
                        }
                        else
                        {
                            nl.Dt = (int)DirType.Up_Dir;
                            nl.Lx--;
                        }
                    }
                }
                nl.Lt = nowSecen[nl.Lx, nl.Ly];
            }
            //判断目标类型
            if (listVist[listVist.Count - 1].Lt == (int)LocType.MoveLoc && listVist[listVist.Count - 1].Dt == nl.Dt)
            {
                listVist[listVist.Count - 1].Lt = 0;
                nl.Lt = (int)LocType.MoveLoc;
            }
            //当前为冰块
            if(nl.Lt == (int)LocType.MoveLoc)
            {
                //到达边界时，自动转换为障碍物
                if(nl.Lx == 0 || nl.Ly == 0 || nl.Lx == sizeX - 1 || nl.Ly == sizeY -1)
                {
                    //listVist.Add(nl);
                    //先退回坐标
                    nl.Lx = listVist[listVist.Count - 1].Lx;
                    nl.Ly = listVist[listVist.Count - 1].Ly;
                    nl.Lt = listVist[listVist.Count - 1].Lt;
                    //然后改变方向
                    if (nl.Dt == (int)DirType.Left_Dir || nl.Dt == (int)DirType.Right_Dir) //左右改上下
                    {
                        if (nl.Lx < endLc.Lx && nl.Lx + 1 < sizeX && nowSecen[nl.Lx + 1, nl.Ly] != (int)LocType.ObsLoc)
                        {
                            nl.Dt = (int)DirType.Down_Dir;
                            nl.Lx++;
                        }
                        else if (nl.Lx > endLc.Lx && nl.Lx - 1 >= 0 && nowSecen[nl.Lx - 1, nl.Ly] != (int)LocType.ObsLoc)
                        {
                            nl.Dt = (int)DirType.Up_Dir;
                            nl.Lx--;
                        }
                        else
                        {
                            if (nl.Dt == (int)DirType.Left_Dir && nowSecen[nl.Lx, nl.Ly + 1] != (int)LocType.ObsLoc)
                            {

                                nl.Dt = (int)DirType.Right_Dir;
                                nl.Ly++;
                            }
                            else
                            {
                                nl.Dt = (int)DirType.Left_Dir;
                                nl.Ly--;
                            }
                        }
                    }
                    else  //上下改左右
                    {
                        if (nl.Ly > endLc.Ly && nl.Ly - 1 >= 0 && nowSecen[nl.Lx, nl.Ly - 1] != (int)LocType.ObsLoc)
                        {
                            nl.Dt = (int)DirType.Left_Dir;
                            nl.Ly--;
                        }
                        else if (nl.Ly < endLc.Ly && nl.Ly + 1 < sizeY && nowSecen[nl.Lx, nl.Ly + 1] != (int)LocType.ObsLoc)
                        {
                            nl.Dt = (int)DirType.Right_Dir;
                            nl.Ly++;
                        }
                        else
                        {
                            if (nl.Dt == (int)DirType.Up_Dir && nowSecen[nl.Lx + 1, nl.Ly] != (int)LocType.ObsLoc)
                            {
                                nl.Dt = (int)DirType.Down_Dir;
                                nl.Lx++;
                            }
                            else
                            {
                                nl.Dt = (int)DirType.Up_Dir;
                                nl.Lx--;
                            }
                        }
                    }
                    nl.Lt = nowSecen[nl.Lx, nl.Ly];
                }
            }
            return nl;
        }


        public static bool FindPath()
        {
            //初始进行
            if(listVist.Count <= 0)
            {
                //先往下走
                if(endLc.Lx > startLc.Lx && nowSecen[startLc.Lx + 1, startLc.Ly] != (int)(LocType.ObsLoc))
                {
                    nowLoc nl = new nowLoc(startLc.Lx + 1, startLc.Ly, nowSecen[startLc.Lx + 1, startLc.Ly]);
                    nl.Dt = (int)DirType.Down_Dir;
                    listVist.Add(nl);
                    if (FindPath() == true)
                        return true;
                }
                //先往上走
                if (endLc.Lx < startLc.Lx && nowSecen[startLc.Lx - 1, startLc.Ly] != (int)(LocType.ObsLoc))
                {
                    nowLoc nl = new nowLoc(startLc.Lx - 1, startLc.Ly, nowSecen[startLc.Lx - 1, startLc.Ly]);
                    nl.Dt = (int)DirType.Up_Dir;
                    listVist.Add(nl);
                    if (FindPath() == true)
                        return true;
                }
                //先往右走
                if (endLc.Ly > startLc.Ly && nowSecen[startLc.Lx, startLc.Ly + 1] != (int)(LocType.ObsLoc))
                {
                    nowLoc nl = new nowLoc(startLc.Lx, startLc.Ly + 1, nowSecen[startLc.Lx, startLc.Ly + 1]);
                    nl.Dt = (int)DirType.Right_Dir;
                    listVist.Add(nl);
                    if (FindPath() == true)
                        return true;
                }
                //先往左走
                if (endLc.Ly < startLc.Ly && nowSecen[startLc.Lx, startLc.Ly - 1] != (int)(LocType.ObsLoc))
                {
                    nowLoc nl = new nowLoc(startLc.Lx, startLc.Ly - 1, nowSecen[startLc.Lx, startLc.Ly - 1]);
                    nl.Dt = (int)DirType.Left_Dir;
                    listVist.Add(nl);
                    if (FindPath() == true)
                        return true;
                }
                return false;
            }
            else
            { 
                //达到终止条件
                if (listVist[listVist.Count - 1].Lx == endLc.Lx && listVist[listVist.Count - 1].Ly == endLc.Ly)
                {
                    return true;
                }
                else
                {
                    bool find1 = false;
                    if(listVist.Count > 1)
                    {
                        for (int i = 0; i < listVist.Count - 1; i++)
                        {
                            if (listVist[listVist.Count - 1].Lx == listVist[i].Lx && listVist[listVist.Count - 1].Ly == listVist[i].Ly)
                            {
                                find1 = true; 
                            } 
                        }
                        if(find1 == true)
                        { 
                            listVist.Clear();
                            return false; 
                        }
                    }
                    //可以继续行驶
                    listVist.Add(GetNextLoc());
                    return FindPath(); 
                }
            }
        }
        static int sizeX = 0, sizeY = 0;
        //read tthe scence data, and return the data
        //将空白的位置设置为0 目标位置设置为1 机器人的位置为2  可移动的冰块设置为3 障碍物设置为4 机器人走过的位置为5 
        public static void LoadScence(string sPath)
        {
            string[] lstr = File.ReadAllLines(sPath);
            for (int i = 0; i < lstr.Length; i++)
            {
                if(lstr[i].StartsWith("size "))
                {
                    sizeY = Convert.ToInt32(lstr[i].Split(' ')[1]);
                    sizeX = Convert.ToInt32(lstr[i].Split(' ')[2]);
                    break;
                } 
            }
            //初始化棋盘大小，默认位置都为空
            int[,] nowSc = new int[sizeX, sizeY];
            for(int i = 0; i < sizeX;i++)
            {
                for(int j = 0; j < sizeY; j++)
                {
                    nowSc[i, j] = (int)LocType.SpaceLoc;
                }
            }
            int locX, locY;
            //初始类型为可移动的冰块
            LocType nowType = LocType.MoveLoc;
            for (int i = 0; i < lstr.Length; i++)
            {
                //绑定目标位置
                if (lstr[i].StartsWith("g "))
                {
                    locY = Convert.ToInt32(lstr[i].Split(' ')[1]);
                    locX = Convert.ToInt32(lstr[i].Split(' ')[2]);
                    nowSc[locX, locY] = (int)LocType.GoalLoc;
                    endLc = new nowLoc(locX, locY, nowSc[locX, locY]);
                }
                //绑定机器人位置
                else if (lstr[i].StartsWith("r "))
                {
                    locY = Convert.ToInt32(lstr[i].Split(' ')[1]);
                    locX = Convert.ToInt32(lstr[i].Split(' ')[2]);
                    nowSc[locX, locY] = (int)LocType.RobotLoc;
                    startLc = new nowLoc(locX, locY, nowSc[locX, locY]);
                }
                //绑定障碍物类型
                else if (lstr[i].StartsWith("obstacle"))
                {
                    nowType = LocType.ObsLoc;
                }
                //如果最后一行，则跳出
                else if(lstr[i].StartsWith("end"))
                {
                    break;
                }
                //判断是否为位置类型，绑定冰块和障碍物
                else if(lstr[i].Split(' ').Length == 2)
                {
                    locY = Convert.ToInt32(lstr[i].Split(' ')[0]);
                    locX = Convert.ToInt32(lstr[i].Split(' ')[1]);
                    nowSc[locX, locY] = (int)nowType;
                }
            }
            nowSecen = nowSc;
            Console.WriteLine("nowSecen:");
            for (uint i = 0; i < sizeX; i++)
            {
                for (uint j = 0; j < sizeY; j++)
                { 
                    Console.Write(" " + nowSecen[i, j]); 
                }
                Console.WriteLine();
            }
        }

        public static void GOALValid(Context ctx)
        {
            Console.WriteLine("GOAL Valid");

            // 9x9 matrix of integer variables
            //初始一个 9 * 9 的二维数组
            IntExpr[][] X = new IntExpr[sizeX][];
            for (uint i = 0; i < sizeX; i++)
            {
                X[i] = new IntExpr[sizeY];
                for (uint j = 0; j < sizeY; j++)
                    X[i][j] = (IntExpr)ctx.MkConst(ctx.MkSymbol("x_" + (i + 1) + "_" + (j + 1)), ctx.IntSort);
            }

            // each cell contains a value in {0, ..., 5}
            // 每个单元格包含的数值范围为 0-5 及枚举值的 SpaceLoc 到 RunLoc
            Expr[][] cells_c = new Expr[sizeX][];
            for (uint i = 0; i < sizeX; i++)
            {
                cells_c[i] = new BoolExpr[sizeY];
                for (uint j = 0; j < sizeY; j++)
                    cells_c[i][j] = ctx.MkAnd(ctx.MkLe(ctx.MkInt((int)LocType.SpaceLoc), X[i][j]),
                                              ctx.MkLe(X[i][j], ctx.MkInt((int)LocType.RunLoc)));
            }

            // robot instance, we use '0' for empty cells
            int[,] instance = nowSecen;

            FindPath();
            for (int i = 0; i < listVist.Count; i++)
            {
                if(nowSecen[listVist[i].Lx, listVist[i].Ly] != listVist[i].Lt)
                {
                    if (listVist[i].Lt == (int)(LocType.SpaceLoc))
                    {
                        nowSecen[listVist[i].Lx, listVist[i].Ly] = 5;
                    }
                    else if (listVist[i].Lt == (int)(LocType.MoveLoc))
                    {
                        nowSecen[listVist[i].Lx, listVist[i].Ly] = listVist[i].Lt;
                    }
                }
                else if(nowSecen[listVist[i].Lx, listVist[i].Ly] == (int)(LocType.SpaceLoc) && listVist[i].Lt == (int)(LocType.SpaceLoc))
                {
                    nowSecen[listVist[i].Lx, listVist[i].Ly] = 5;
                }
            }
            // each row contains a digit at most once
            //BoolExpr[] rows_c = new BoolExpr[sizeX];
            //for (uint i = 0; i < sizeX; i++)
            //rows_c[i] = ctx.MkDistinct(X[i]); 

            BoolExpr sudoku_c = ctx.MkTrue();
            foreach (BoolExpr[] t in cells_c)
                sudoku_c = ctx.MkAnd(ctx.MkAnd(t), sudoku_c);
            // sudoku_c = ctx.MkAnd(ctx.MkAnd(rows_c), sudoku_c); 
            //foreach (BoolExpr[] t in sq_c)
            //    sudoku_c = ctx.MkAnd(ctx.MkAnd(t), sudoku_c);


            BoolExpr instance_c = ctx.MkTrue();
            for (uint i = 0; i < sizeX; i++)
                for (uint j = 0; j < sizeY; j++)
                    instance_c = ctx.MkAnd(instance_c,
                        (BoolExpr)
                        ctx.MkITE(ctx.MkEq(ctx.MkInt(instance[i, j]), ctx.MkInt(0)),
                                    ctx.MkTrue(),
                                    ctx.MkEq(X[i][j], ctx.MkInt(instance[i, j]))));

            Solver s = ctx.MkSolver();
            s.Assert(sudoku_c);
            s.Assert(instance_c);

            if (s.Check() == Status.SATISFIABLE)
            {
                Model m = s.Model;
                Expr[,] R = new Expr[sizeX, sizeY];
                for (uint i = 0; i < sizeX; i++) {
                    for (uint j = 0; j < sizeY; j++)
                    {
                        if (instance[i, j] != 0)
                        {
                            R[i, j] = m.Evaluate(X[i][j]);
                        } 
                    }
                }
                Console.WriteLine("Robot solution:");
                for (uint i = 0; i < sizeX; i++)
                {
                    for (uint j = 0; j < sizeY; j++)
                    { 
                        if(R[i,j] != null) { 
                            Console.Write(" " + R[i, j]);
                        }
                        else
                        {
                            Console.Write(" 0");
                        }
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Failed to solve goal"); 
            }
        }


        static void Main(string[] args)
        {
            string sc = "scene1";
            Console.WriteLine("note:"+ sc);
            Console.WriteLine("0:space  1:goal  2:robot 3:movable 4;obstacle 5:path ");
            LoadScence(sc);
            using (Context ctx = new Context(new Dictionary<string, string>() { { "model", "true" } }))
            {
                GOALValid(ctx); 
            }
            Console.ReadLine();
        }
    }
}
