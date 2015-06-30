using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace PuzzleSpace
{
    [Serializable]
    public enum DirectionCell
    {
        Null = 0,
        Top,
        Right,
        Bottom,
        Left        
    }

    [Serializable]
    public struct DirectionVector
    {
        public DirectionCell Direction;
        public int Length;
        
        public DirectionVector(DirectionCell direct, int length)
        {
            Direction = direct;
            Length = length;
        }

        public DirectionCell RevertDirection()
        {
            switch (Direction)
            {
                case DirectionCell.Top:
                    return DirectionCell.Bottom;

                case DirectionCell.Right:
                    return DirectionCell.Left;

                case DirectionCell.Bottom:
                    return DirectionCell.Top;

                case DirectionCell.Left:
                    return DirectionCell.Right;

                default:
                    return DirectionCell.Null;
            }
        }

        public PointCell PointNow(PointCell pointZero)
        {
            int x = pointZero.X;
            int y = pointZero.Y;

            switch (Direction)
            {
                case DirectionCell.Top:
                    y++;
                    break;
                case DirectionCell.Right:
                    x--;
                    break;
                case DirectionCell.Bottom:
                    y--;
                    break;
                case DirectionCell.Left:
                    x++;
                    break;
            }

            return new PointCell(x, y);
        }

        public static DirectionVector operator !(DirectionVector vector)
        {
            if (vector.Direction == DirectionCell.Top)
                vector.Direction = DirectionCell.Bottom;
            else
                if (vector.Direction == DirectionCell.Right)
                    vector.Direction = DirectionCell.Left;
                else
                    if (vector.Direction == DirectionCell.Bottom)
                        vector.Direction = DirectionCell.Top;
                    else
                        if (vector.Direction == DirectionCell.Left)
                            vector.Direction = DirectionCell.Right;

            return vector;
        }

        public static bool operator ==(DirectionVector vector1, DirectionVector vector2)
        {
            if ((vector1.Direction == vector2.Direction) && (vector1.Length == vector2.Length))
                return true;
            else
                return false;
        }

        public static bool operator !=(DirectionVector vector1, DirectionVector vector2)
        {
            if ((vector1.Direction == vector2.Direction) && (vector1.Length == vector2.Length))
                return false;
            else
                return true;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Direction.ToString(), Length);
        }
    }

    [Serializable]
    public struct PointCell
    {
        public int X;// { get; set; }
        public int Y;// { get; set; }

        public PointCell(PointCell point)
        {
            this = DeepCopy<PointCell>(point);
        }

        public PointCell(int x, int y, bool invert = false)
        {
            if (invert)
            {
                x += y;
                y = x - y;
                x -= y;
            }

            X = x;// Math.Abs(x);
            Y = y;// Math.Abs(y);
        }

        /*public DirectionCell IsNear(PointCell point)
        {

        }*/

        static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

        public static int Difference(PointCell point1, PointCell point2)
        {
            return (int)Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }
        
        public override bool Equals(object o)
        {
            try
            {
                PointCell p = (PointCell)o;
                return (this == p) ? true : false;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return X + Y;
        }

        public static PointCell GetLeftPointFromList(PointCell[] points)
        {
            PointCell result = points[0];
            int left = 128;

            foreach (PointCell point in points)
            {
                if (point.X < left)
                {
                    left = point.X;
                    result = point;
                }
            }

            return result;
        }

        public static PointCell GetRightPointFromList(PointCell[] points)
        {
            PointCell result = points[0];
            int right = -1;

            foreach (PointCell point in points)
            {
                if (point.X > right)
                {
                    right = point.X;
                    result = point;
                }
            }

            return result;
        }

        public static PointCell operator -(PointCell point1, int plus)
        {
            return new PointCell(point1.X - plus, point1.Y - plus);
        }

        public static PointCell operator +(PointCell point1, int plus)
        {
            return new PointCell(point1.X + plus, point1.Y + plus);
        }

        public static PointCell operator -(PointCell point1, PointCell point2)
        {
            return new PointCell(point1.X - point2.X, point1.Y - point2.Y);
        }

        public static bool operator >(PointCell point1, PointCell point2)
        {
            if ((point1.X > point2.X) || (point1.Y > point2.Y))
                return true;
            else
                return false;
        }

        public static bool operator <(PointCell point1, PointCell point2)
        {
            return !(point1 > point2);
        }

        public static bool operator ==(PointCell point1, PointCell point2)
        {
            if ((point1.X == point2.X) && (point1.Y == point2.Y))
                return true;
            else
                return false;
        }

        public static bool operator !=(PointCell point1, PointCell point2)
        {
            if ((point1.X == point2.X) && (point1.Y == point2.Y))
                return false;
            else
                return true;
        }

        public override string ToString()
        {
            return String.Format("({0} {1})", X, Y);
        }
    };

    [Serializable]
    public struct BaseProgressItem
    {
        //public int Number;
        public PointCell PointSourceMove;
        public DirectionVector Direction;

        public BaseProgressItem(PointCell point, DirectionVector direction)
        {
            PointSourceMove = point;
            Direction = direction;
        }

        public BaseProgressItem Revert()
        {
            BaseProgressItem bpi = new BaseProgressItem(PointSourceMove, Direction);

            switch (bpi.Direction.Direction)
            {
                case DirectionCell.Top:
                    bpi.PointSourceMove.Y -= bpi.Direction.Length;
                    bpi.Direction.Direction = DirectionCell.Bottom;
                    break;

                case DirectionCell.Right:
                    bpi.PointSourceMove.X += bpi.Direction.Length;
                    bpi.Direction.Direction = DirectionCell.Left;
                    break;

                case DirectionCell.Bottom:
                    bpi.PointSourceMove.Y += bpi.Direction.Length;
                    bpi.Direction.Direction = DirectionCell.Top;
                    break;

                case DirectionCell.Left:
                    bpi.PointSourceMove.X -= bpi.Direction.Length;
                    bpi.Direction.Direction = DirectionCell.Right;
                    break;
            }

            return bpi;
        }

        public PointCell NewPoint()
        {
            int x = PointSourceMove.X;
            int y = PointSourceMove.Y;

            switch (Direction.Direction)
            {
                case DirectionCell.Top:
                    y--;
                    break;
                case DirectionCell.Right:
                    x++;
                    break;
                case DirectionCell.Bottom:
                    y++;
                    break;
                case DirectionCell.Left:
                    x--;
                    break;
            }

            return new PointCell(x, y);
        }

        public override string ToString()
        {
            return String.Format("BPI: {0} {1}", PointSourceMove.ToString(), Direction.ToString());
        }
    }

    public class PuzzleElementForWPF : Canvas
    {
        public int Dimension = 4;

        public Puzzle PuzzleObject;
        public List<Grid> AllCells;

        public delegate void DelegateSTD();
        public delegate void CellMoveHander(BaseProgressItem ProgressItem);
        public delegate void CompletedHander();
        public event CellMoveHander CellMove;
        public event CompletedHander Completed;

        private bool findRecursion = false;
        delegate void DelegateBaseProgressItem(BaseProgressItem bpi);

        public PuzzleElementForWPF(int dimension = 4)
        {
            Dimension = dimension;

            PuzzleObject = new Puzzle(4, true);
            AllCells = new List<System.Windows.Controls.Grid>(Dimension);

            /*PuzzleObject.Matrix[1][2] = 8;
            PuzzleObject.Matrix[1][3] = 7;*/

            /*PuzzleObject.Matrix[0][0] = 11;
            PuzzleObject.Matrix[2][2] = 1;*/

           /* PuzzleObject.Matrix[3][3] = 12;
            PuzzleObject.Matrix[1][0] = 0;
            PuzzleObject.PointZero = new PointCell(1, 0, true);*/

            for (int i = 0; i < Math.Pow(PuzzleObject.WidthPuzzle, 2); i++)
                AllCells.Add(new Grid());

            InitializeComponent();

            this.SizeChanged += PuzzleElementForWPF_SizeChanged;
        }

        private void InitializeComponent()
        {
            this.Background = Brushes.White;

            double map_w = this.ActualWidth;
            double square_w = map_w / PuzzleObject.WidthPuzzle;
            int[][] matrix = PuzzleObject.Matrix;

            for (int i = 0; i < PuzzleObject.WidthPuzzle; i++)
                for (int j = 0; j < PuzzleObject.WidthPuzzle; j++)
                {
                    if (matrix[i][j] != 0)
                    {
                        Grid square = new Grid();
                        Border border = new Border();
                        TextBlock tb = new TextBlock();

                        border.BorderBrush = new SolidColorBrush(Colors.White);
                        border.BorderThickness = new Thickness(1);
                        border.Margin = new Thickness(0);
                        border.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        border.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                        //square.Width = square_w;
                        //square.Height = square_w;
                        square.Background = new SolidColorBrush(Colors.MediumPurple);
                        square.Name = "CellSquare" + matrix[i][j].ToString();
                        square.Tag = new PointCell(i, j, true);
                        square.SetBinding(Grid.WidthProperty, new Binding("ActualHeight") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Source = square });
                        square.MouseLeftButtonDown += square_MouseLeftButtonDown;
                        square.MouseRightButtonDown += square_MouseRightButtonDown;

                        tb.Text = matrix[i][j].ToString();
                        //tb.FontSize = (square_w / 100 * 61 == 0) ? 10 : square_w / 100 * 61;
                        tb.Foreground = Brushes.White;
                        tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                        border.Child = tb;
                        square.Children.Add(border);

                        this.Children.Add(square);
                        AllCells[matrix[i][j] - 1] = square;

                        Canvas.SetLeft(square, j * square_w);
                        Canvas.SetTop(square, i * square_w);
                    }
                }
        }

        #region Events

        private void PuzzleElementForWPF_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size oldSize = e.PreviousSize;
            Size newSize = e.NewSize;
            double square_w = newSize.Width / PuzzleObject.WidthPuzzle;

            foreach (UIElement element in this.Children)
            {
                Grid grid = (Grid)element;

                if (grid != null)
                {
                    TextBlock tb = ((Border)grid.Children[0]).Child as TextBlock;
                    PointCell point = (PointCell)grid.Tag;
                    DoubleAnimation animateLeft = new DoubleAnimation();
                    DoubleAnimation animateTop = new DoubleAnimation();
                    DoubleAnimation animateSize = new DoubleAnimation();

                    animateLeft.Duration = TimeSpan.FromMilliseconds(100);
                    animateLeft.From = (double)grid.GetValue(Canvas.LeftProperty);
                    animateLeft.To = point.X * square_w;

                    animateTop.Duration = TimeSpan.FromMilliseconds(100);
                    animateTop.From = (double)grid.GetValue(Canvas.TopProperty);
                    animateTop.To = point.Y * square_w;

                    animateSize.Duration = TimeSpan.FromMilliseconds(100);
                    animateSize.From = grid.ActualHeight;
                    animateSize.To = square_w;

                    grid.BeginAnimation(Canvas.HeightProperty, animateSize);
                    grid.BeginAnimation(Canvas.LeftProperty, animateLeft);
                    grid.BeginAnimation(Canvas.TopProperty, animateTop);
                    tb.FontSize = square_w / 100 * 61;
                }
            }
        }

        private void square_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid grid = (Grid)sender;
            PointCell point = (PointCell)grid.Tag;
            DirectionVector directionVector = PuzzleObject.GetAllowedVectorDirection(point);

            Console.WriteLine(PuzzleObject.GetDistanceManhattenPoint(point));
            //Console.WriteLine("Cell{0} {1}: {2} ({3}) | {4}", PuzzleObject.GetIndexCell(point), point.ToString(), directionVector.Direction, directionVector.Length, PuzzleObject.Matrix[point.Y][point.X]);
        }

        private void square_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MoveCellToAllowedDirection((Grid)sender);
        }

        #endregion

        public void MoveCellToAllowedDirection(BaseProgressItem bpi)
        {
            MoveCellToDirection(bpi.PointSourceMove, bpi.Direction);
        }

        public void MoveCellToAllowedDirection(PointCell point, bool addToHistory = true)
        {
            int id = PuzzleObject.GetIndexCell(point);
            MoveCellToAllowedDirection(AllCells[id-1], addToHistory);
        }

        public void MoveCellToAllowedDirection(Grid grid, bool addToHistory = true)
        {
            PointCell point = (PointCell)grid.Tag;
            DirectionVector directionVector = PuzzleObject.GetAllowedVectorDirection(point);
            MoveCellToDirection(grid, directionVector, addToHistory);
        }

        public void MoveCellToDirection(PointCell point, DirectionVector directionVector, bool addToHistory = true)
        {
            if (directionVector.Length > 0)
            {
                int id = PuzzleObject.GetIndexCell(point);
                MoveCellToDirection(AllCells[id - 1], directionVector, addToHistory);
            }
        }

        public void MoveCellToDirection(Grid grid, DirectionVector directionVector, bool addToHistory = true)
        {
            if (directionVector.Length > 0)
            {
                PointCell point = (PointCell)grid.Tag;
                DoubleAnimation animate = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(100) };

                if (directionVector.Length > 0)
                {
                    List<Grid> moveCells = new List<Grid>(directionVector.Length);
                    PointCell pathPoint = point;
                    DependencyProperty property = null;
                    int addAXIS = 1;

                    switch (directionVector.Direction)
                    {
                        case DirectionCell.Top:
                            addAXIS = -1;
                            break;

                        case DirectionCell.Left:
                            addAXIS = -1;
                            property = Canvas.LeftProperty;
                            break;
                    }

                    if ((directionVector.Direction == DirectionCell.Left) || (directionVector.Direction == DirectionCell.Right))
                    {
                        property = Canvas.LeftProperty;

                        for (int i = 0; i < directionVector.Length; i++)
                        {
                            int index = PuzzleObject.GetIndexCell(pathPoint);
                            Grid cell = AllCells[index - 1];
                            cell.Tag = new PointCell(pathPoint.X + addAXIS, pathPoint.Y);
                            moveCells.Add(cell);
                            pathPoint.X += addAXIS;
                        }
                        pathPoint.X -= addAXIS;
                    }
                    if ((directionVector.Direction == DirectionCell.Top) || (directionVector.Direction == DirectionCell.Bottom))
                    {
                        property = Canvas.TopProperty;

                        for (int i = 0; i < directionVector.Length; i++)
                        {
                            int index = PuzzleObject.GetIndexCell(pathPoint);
                            Grid cell = AllCells[index - 1];
                            cell.Tag = new PointCell(pathPoint.X, pathPoint.Y + addAXIS);
                            moveCells.Add(cell);
                            pathPoint.Y += addAXIS;
                        }
                        pathPoint.Y -= addAXIS;
                    }
                    //Console.WriteLine("");

                    foreach (Grid g in moveCells)
                    {
                        //Console.WriteLine("Move {0}", g.Name);

                        animate.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
                        animate.From = (double)g.GetValue(property);
                        animate.To = animate.From + addAXIS * grid.ActualWidth;

                        g.BeginAnimation(property, animate);
                    }

                    /*PuzzleObject = */
                    PuzzleObject.GoDirection(directionVector);

                    if (addToHistory)
                    {
                        PuzzleObject.History.Push(new BaseProgressItem(point, directionVector));
                        if (CellMove != null)
                            CellMove(new BaseProgressItem(point, directionVector));

                        //vis_lb_history.Items.Add(String.Format("Cell №{0} move {1}", PuzzleObject.GetIndexCell(point), directionVector.ToString()));
                    }
                }
            }
        }

        public void UndoMove()
        {
            if (PuzzleObject.History.Count > 0)
                MoveCellToAllowedDirection(PuzzleObject.History.Pop().Revert().PointSourceMove, false);
        }

        public void StartLogical()
        {
            #region hide
            //PuzzleObject.MultiAction = false;
            /*queue.Clear();
            findRecursion = false;

            Puzzle puzzle = new Puzzle(PuzzleObject);
            sH = puzzle.GetHeuristic();
            shb = sH;

            puzzle.History.Clear();

            int k = 0;
            while (findRecursion == false)
            {
                recursion(ref puzzle);
                sH = shb;
                k++;
            }

            puzzle = queue.Dequeue();

            for (int i = puzzle.History.Count - 1; i >= 0; i--)
            {
                BaseProgressItem bpi = puzzle.History.ToArray()[i];
                MoveCellToAllowedDirection(bpi.PointSourceMove);
                MessageBox.Show(PuzzleObject.ToString());
                //Thread.Sleep(1000);
            }*/
            #endregion

            new Thread(new ThreadStart(thread_analyzeSolution)).Start();
        }

        void thread_analyzeSolution()
        {
            Puzzle puzzle = new Puzzle(PuzzleObject) { MultiAction = false }; puzzle.History.Clear();
            List<BaseProgressItem> Moves = SolutionLogical(puzzle);
            countRec = 0;
            
            foreach (BaseProgressItem bpi in Moves)
            {
                Thread.Sleep(120);
                Dispatcher.BeginInvoke(new DelegateBaseProgressItem(MoveCellToAllowedDirection), bpi);
            }

            Dispatcher.BeginInvoke(new DelegateSTD(Complete));
        }

        void Complete()
        {
            Completed();
        }

        public void buf()
        {
            
        }

        //List<Puzzle> openList = new List<Puzzle>();
        Queue<Puzzle> queueList = new Queue<Puzzle>();
        List<Puzzle> openList = new List<Puzzle>();
        List<Puzzle> closeList = new List<Puzzle>();
        List<PointCell> dontMoveList = new List<PointCell>();
        int count_recurs = 0;

        List<BaseProgressItem> SolutionRecursion(Puzzle puzzle)
        {
            List<BaseProgressItem> moves = new List<BaseProgressItem>();
            Puzzle puzzleOriginal = new Puzzle(puzzle);
            openList.Clear();
            closeList.Clear();
            dontMoveList.Clear();

            for (int i = 1; i <= 4; i++)
            {
                PointCell pointStart = puzzle.GetPoint(i);
                PointCell pointEnd = puzzle.PointsCanonicalMatrix[i];
                int itterationCount = 1;

                if (pointEnd.X == puzzle.WidthPuzzle - 2)
                {
                    pointEnd.X++;
                }
                else
                    if (pointEnd.X == puzzle.WidthPuzzle - 1)
                    {
                        pointEnd.Y++;
                        dontMoveList.Add(new PointCell(pointEnd.X - 1, pointEnd.Y));
                    }

                openList.Clear();
                closeList.Clear();
                //dontMoveList.Clear();

                openList.Add(puzzle);

                while (puzzle.GetPoint(i) != pointEnd)
                {
                    openList.Remove(puzzle);
                    closeList.Add(puzzle);

                    PointCell pointNow = puzzle.GetPoint(i);
                    List<PointCell> near = puzzle.GetPointsNearPoint(pointNow);
                    PointCell pointZeroNeed = near[0];

                    /*foreach (PointCell nearPoint in near)
                    {
                        if (dontMoveList.IndexOf(nearPoint) != -1)
                            continue;

                        pointZeroNeed = nearPoint;
                        break;
                    }*/

                    if (pointNow.X < pointEnd.X)
                        if (dontMoveList.IndexOf(near[0]) != -1)
                        {
                            pointZeroNeed = PointCell.GetRightPointFromList(near.ToArray());
                        }

                    #region Move zero

                    //if ((near[0] != puzzle.PointZero) && (near.Count > 0))
                    //{
                    //    bool revert = false;
                    //    if ((puzzle.PointZero.X == pointNow.X) && (puzzle.PointZero.Y > pointZeroNeed.Y))
                    //    {
                    //        DirectionCell directCell = (puzzle.PointZero.X == puzzle.WidthPuzzle - 1) ? DirectionCell.Right : DirectionCell.Left;
                    //        DirectionVector direct = new DirectionVector(directCell, 1);

                    //        puzzle.GoDirection(direct);
                    //        puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                    //    }

                    //    if (puzzle.PointZero.Y == pointNow.Y)
                    //    {
                    //        revert = true;
                    //        DirectionCell directCell = (puzzle.PointZero.Y >= puzzle.WidthPuzzle - 1) ? DirectionCell.Bottom : DirectionCell.Top;
                    //        DirectionVector direct = new DirectionVector(directCell, 1);

                    //        puzzle.GoDirection(direct);
                    //        puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                    //    }

                    //    if (!revert)
                    //    {
                    //        if (puzzle.PointZero.Y != pointZeroNeed.Y)
                    //        {
                    //            int mo = Math.Abs(puzzle.PointZero.Y - pointZeroNeed.Y);
                    //            DirectionCell directCell = (puzzle.PointZero.Y > pointZeroNeed.Y) ? DirectionCell.Bottom : DirectionCell.Top;
                    //            DirectionVector direct = new DirectionVector(directCell, mo);

                    //            puzzle.GoDirection(direct);
                    //            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                    //        }

                    //        if (puzzle.PointZero.X != pointZeroNeed.X)
                    //        {
                    //            int mo = Math.Abs(puzzle.PointZero.X - pointZeroNeed.X);
                    //            DirectionCell directCell = (puzzle.PointZero.X > pointZeroNeed.X) ? DirectionCell.Right : DirectionCell.Left;
                    //            DirectionVector direct = new DirectionVector(directCell, mo);

                    //            puzzle.GoDirection(direct);
                    //            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (puzzle.PointZero.X != pointZeroNeed.X)
                    //        {
                    //            int mo = Math.Abs(puzzle.PointZero.X - pointZeroNeed.X);
                    //            DirectionCell directCell = (puzzle.PointZero.X > pointZeroNeed.X) ? DirectionCell.Right : DirectionCell.Left;
                    //            DirectionVector direct = new DirectionVector(directCell, mo);

                    //            puzzle.GoDirection(direct);
                    //            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                    //        }

                    //        if (puzzle.PointZero.Y != pointZeroNeed.Y)
                    //        {
                    //            int mo = Math.Abs(puzzle.PointZero.Y - pointZeroNeed.Y);
                    //            DirectionCell directCell = (puzzle.PointZero.Y > pointZeroNeed.Y) ? DirectionCell.Bottom : DirectionCell.Top;
                    //            DirectionVector direct = new DirectionVector(directCell, mo);

                    //            puzzle.GoDirection(direct);
                    //            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                    //        }
                    //    }
                    //}

                    #endregion

                    if (puzzle.PointZero != pointZeroNeed)
                    {
                        List<PointCell> dontMoveZero = new List<PointCell>(dontMoveList);

                        dontMoveZero.Add(pointNow);

                        bool error = false;
                        int k = 0;

                        /*while (puzzle.PointZero != pointZeroNeed)
                        {
                            pointZeroNeed = near[k];
                            MoveZeroToPoint(ref puzzle, pointZeroNeed, dontMoveZero, ref error);
                            k++;
                        }*/
                        MoveZeroToPoint(ref puzzle, pointZeroNeed, dontMoveZero, ref error);
                        if (error)
                            break;
                    }

                    #region Move cell

                    Puzzle[] puzzles = puzzle.GetPuzzleListFromPoints(puzzle.GetPointCanMove());
                    Puzzle minHPuzzle = null;
                    int minHeuristic = 1024;

                    foreach (Puzzle p in puzzles)
                    {
                        bool allow = true;

                        foreach (Puzzle closePuzzle in closeList)
                            if (EqualDoubleArray(p.Matrix, closePuzzle.Matrix))
                            {
                                allow = false;
                                break;
                            }

                        foreach (PointCell point in dontMoveList)
                            if (p.History.Peek().PointSourceMove == point)
                            {
                                allow = false;
                                break;
                            }

                        Console.Write("{0} ", p.History.Peek().Direction.Direction);

                        if (!allow)
                        {
                            Console.WriteLine("don`t allow!");
                            continue;
                        }

                        PointCell pointIn = p.GetPoint(i);
                        int H = p.GetHeuristicForOne(pointIn), G = PointCell.Difference(pointIn, pointStart);
                        int id = openList.IndexOf(p);

                        if (id != -1)
                        {
                            Puzzle puzzleInOpen = openList[id];
                            int GInOpen = puzzleInOpen.GetCostMovesByOriginalPuzzle(puzzleOriginal);
                            G = p.GetCostMovesByOriginalPuzzle(puzzleOriginal);

                            if (GInOpen < G)
                            {
                                BaseProgressItem bpiLast = p.History.Peek();
                                p.History = new Stack<BaseProgressItem>(puzzleInOpen.History);
                                p.History.Push(bpiLast);
                            }
                        }

                        Console.WriteLine("{0} {1}", H, G);

                        if (H < minHeuristic)
                        {
                            minHeuristic = H;
                            minHPuzzle = p;
                        }
                    }

                    Console.WriteLine("Select {0}", minHPuzzle.History.Peek().Direction.Direction);
                    puzzle = minHPuzzle;

                    itterationCount++;

                    if (itterationCount > 50)
                        break;

                    #endregion

                    //break;
                }

                dontMoveList.Add(pointEnd);

                if (puzzle.GetPoint(i) != pointEnd)
                    break;
            }

            moves.AddRange(puzzle.History.Reverse<BaseProgressItem>().ToList());
            
            return moves;
        }

        void MoveZeroToPoint(ref Puzzle puzzle, PointCell end, List<PointCell> dontMovePoint, ref bool Error)
        {
            Queue<Puzzle> queue_puzzle = new Queue<Puzzle>();
            List<Puzzle> closePuzzle = new List<Puzzle>();
            Puzzle puzzleThis = puzzle;
            int itteration_count = 0;

            queue_puzzle.Enqueue(puzzle);

            //Console.WriteLine("<GetMovesZeroToPoint>");

            while (puzzleThis.PointZero != end)
            {
                puzzleThis = queue_puzzle.Dequeue();
                Puzzle[] puzzles = puzzleThis.GetPuzzleListFromPoints(puzzleThis.GetPointCanMove());

                foreach (Puzzle p in puzzles)
                {
                    if (dontMovePoint.IndexOf(p.History.Peek().PointSourceMove) != -1)
                        continue;

                    bool allow = true;

                    foreach (Puzzle cP in closePuzzle)
                        if (EqualDoubleArray(p.Matrix, cP.Matrix))
                        {
                            allow = false;
                            break;
                        }

                    if (!allow)
                        continue;

                    //Console.WriteLine("{0}: {1}", itteration_count, p.History.Peek().Direction.Direction);
                    queue_puzzle.Enqueue(p);
                }

                //Console.WriteLine("");

                closePuzzle.Add(puzzleThis);

                itteration_count++;

                if (itteration_count > 64)
                {
                    Error = true;
                    break;
                }
            }
            puzzle = puzzleThis;

            //Console.WriteLine("</GetMovesZeroToPoint>");

            //return puzzle.History.ToList();
        }

        //List<BaseProgressItem> SolutionRecursion(Puzzle puzzle)
        //{
        //    List<BaseProgressItem> moves = new List<BaseProgressItem>();
        //    Puzzle puzzleOriginal = new Puzzle(puzzle);
        //    int OriginalHeuristic = puzzleOriginal.GetHeuristic();

        //    openList.Clear();
        //    closeList.Clear();

        //    #region old
        //    /*puzzle.History.Clear();
        //    puzzle.MultiAction = false;

        //    queueList.Clear();
        //    queueList.Enqueue(puzzle);
        //    closeList.Clear();
        //    closeList.Add(puzzle.Matrix);
        //    count_recurs = 1;
        //    //findRecurs = true;

        //    Recursion();

        //    Puzzle result = queueList.Dequeue();

        //    List<BaseProgressItem> history = result.History.ToList();

        //    for (int i = history.Count - 1; i >= 0; i--)
        //    {
        //        moves.Add(history[i]);
        //    }*/
        //    #endregion

        //    #region Start

        //    openList.Add(puzzle);

        //    List<Puzzle> puzzleList = puzzle.GetPuzzleListFromPoints(puzzle.GetPointCanMove(), true);
        //    Puzzle minHPuzzle = null;
        //    int minHeuristic = 1024;

        //    openList.Remove(puzzle);
        //    closeList.Add(puzzle);

        //    foreach (Puzzle p in puzzleList)
        //    {
        //        openList.Add(p);

        //        Console.Write("{0} ", p.History.Peek().Direction.Direction);
        //        int H = p.GetHeuristic();
        //        int G = p.GetCostMovesByOriginalPuzzle(puzzleOriginal);

        //        Console.WriteLine("{0} {1}", H, G);

        //        if (H + G < minHeuristic)
        //        {
        //            minHeuristic = H;
        //            minHPuzzle = p;
        //        }
        //    }

        //    #endregion

        //    Console.WriteLine("Select {0}", minHPuzzle.History.Peek().Direction.Direction);
        //    puzzle = new Puzzle(minHPuzzle);
        //    moves = puzzle.History.ToList();

        //    #region Continue

        //    int itterationCount = 0;

        //    while (!puzzle.IsWIN())
        //    {
        //        openList.Remove(minHPuzzle);
        //        closeList.Add(minHPuzzle);

        //        puzzleList = puzzle.GetPuzzleListFromPoints(puzzle.GetPointCanMove(), true);

        //        minHPuzzle = null;
        //        minHeuristic = 1024;

        //        foreach (Puzzle p in puzzleList)
        //        {
        //            bool allow = true;

        //            /*if (p.History.Count > 1)
        //                if (p.History.ElementAt<BaseProgressItem>(0).Revert().Direction == p.History.ElementAt<BaseProgressItem>(1).Direction)
        //                    allow = false;*/

        //            foreach (Puzzle closePuzzle in closeList)
        //                if (EqualDoubleArray(p.Matrix, closePuzzle.Matrix))/*(p.Matrix.Equals(closePuzzle.Matrix))*/
        //                {
        //                    allow = false;
        //                    break;
        //                }

        //            Console.Write("{0} ", p.History.Peek().Direction.Direction);

        //            if (!allow)
        //            {
        //                Console.WriteLine("don`t allow!");
        //                continue;
        //            }

        //            int id = openList.IndexOf(p);
        //            int H, G;

        //            if (id != -1)
        //            {
        //                Puzzle puzzleInOpen = openList[id];
        //                int GInOpen = puzzleInOpen.GetCostMovesByOriginalPuzzle(puzzleOriginal);
        //                G = p.GetCostMovesByOriginalPuzzle(puzzleOriginal);

        //                if (GInOpen < G)
        //                {
        //                    BaseProgressItem bpiLast = p.History.Peek();
        //                    p.History = new Stack<BaseProgressItem>(puzzleInOpen.History);
        //                    p.History.Push(bpiLast);
        //                }
        //            }

        //            H = p.GetHeuristic();
        //            G = p.GetCostMovesByOriginalPuzzle(puzzleOriginal);

        //            Console.WriteLine("{0} {1}", H, G);

        //            if (G + H < minHeuristic)
        //            {
        //                minHeuristic = G + H;
        //                minHPuzzle = p;
        //            }
        //        }

        //        puzzle = new Puzzle(minHPuzzle);
        //        moves = puzzle.History.ToList();

        //        itterationCount++;

        //        if (itterationCount > 550)
        //            break;
        //    }

        //    #endregion

        //    return moves.Reverse<BaseProgressItem>().ToList();
        //}

        #region Recursion

        //void Recursion()
        //{
        //    if (queueList.Count > 0)
        //    {
        //        Puzzle puzzle = queueList.Peek();

        //        if ((puzzle != null) && (!puzzle.IsWIN()))
        //        {
        //            PointCell[] points = puzzle.GetPointCanMove();
        //            Puzzle puzzleMin = null;
        //            int hMin = 100;

        //            foreach (PointCell point in points)
        //            {
        //                DirectionVector vector = puzzle.GetAllowedVectorDirection(point);

        //                if (((puzzle.History.Count > 0) && (vector != puzzle.History.Peek().Revert().Direction)) || (puzzle.History.Count == 0))
        //                {
        //                    Puzzle p = new Puzzle(puzzle);
        //                    bool allow = true;

        //                    p.GoDirection(vector, true);

        //                    /*foreach (int[][] matrix in closeList)
        //                        if (matrix.Equals(p.Matrix))
        //                            allow = false;*/

        //                    if (allow)
        //                    {
        //                        int heuristic = p.GetHeuristic();

        //                        Console.WriteLine("{0}: {1} H:{2}", count_recurs, 0/*puzzle.GetIndexCell(point)*/, heuristic);

        //                        if (heuristic < hMin)
        //                        {
        //                            hMin = heuristic;
        //                            puzzleMin = p;
        //                        }
        //                    }
        //                }
        //            }

        //            queueList.Dequeue();
        //            queueList.Enqueue(puzzleMin);
        //            closeList.Add(puzzleMin);

        //            count_recurs++;
        //            if (count_recurs < 500)
        //                Recursion();
        //        }
        //    }
        //}

        #endregion

        bool EqualDoubleArray(int[][] array1, int[][] array2)
        {
            try
            {
                if (array1.Length == array2.Length)
                {
                    for (int i = 0; i < array1.Length; i++)
                        if (array1[i].Length == array2[i].Length)
                        {
                            for (int j = 0; j < array1[i].Length; j++)
                                if (array1[i][j] != array2[i][j])
                                    return false;
                        }
                        else
                            return false;
                }
                else
                    return false;

                return true;
            }
            catch (ArgumentOutOfRangeException exc)
            {
                Console.WriteLine("Error: {0}", exc.Message);
                return false;
            }
        }

        //bool EqualDoubleArray(int[][] array1, int[][] array2)
        //{
        //    for (int i = 0; i < array1.Length; i++)
        //        for (int j = 0; j < array1[i].Length; j++)
        //            if (array1[i][j] != array2[i][j])
        //                return false;

        //    return true;
        //}

        //List<BaseProgressItem> SolutionLogical2(Puzzle puzzle)
        //{
        //    List<BaseProgressItem> moves = new List<BaseProgressItem>();

        //    puzzle.History.Clear();

        //    queue_list.Clear();
        //    closeList_matrix.Clear();
        //    queue_list.Enqueue(new Puzzle(puzzle) { MultiAction = false });
        //    count_recurs = 1;
        //    findRecurs = true;

        //    Recursion2();

        //    Puzzle result = queue_list.Dequeue();

        //    List<BaseProgressItem> history = result.History.ToList();

        //    for (int i = history.Count - 1; i >= 0; i--)
        //    {
        //        moves.Add(history[i]);
        //    }

        //    return moves;
        //}

        //Queue<Puzzle> queue_list = new Queue<Puzzle>();
        //List<int[][]> closeList_matrix = new List<int[][]>();
        //int count_recurs = 0;
        //bool findRecurs = true;

        //void Recursion2()
        //{
        //    if (queue_list.Count > 0)
        //    {
        //        Puzzle puzzle = queue_list.Dequeue();

        //        closeList_matrix.Add(puzzle.Matrix);

        //        if (!puzzle.IsWIN())
        //        {
        //            PointCell[] points = puzzle.GetPointCanMove();
        //            Puzzle puzzleMin = null;
        //            int hMin = 160;

        //            foreach (PointCell point in points)
        //            {
        //                Puzzle p = new Puzzle(puzzle);
        //                DirectionVector direct = p.GetAllowedVectorDirection(point);

        //                if (((p.History.Count > 0) && (direct.Direction != p.History.Peek().Revert().Direction.Direction)) || (p.History.Count == 0))
        //                {
        //                    Console.WriteLine("{0} {1}", count_recurs, p.GetIndexCell(point));
        //                    p.GoDirection(direct, true);

        //                    bool inClose = false;

        //                    foreach (int[][] matrix in closeList_matrix)
        //                        if (EqualDoubleArray(p.Matrix, matrix))
        //                        {
        //                            inClose = true;
        //                            break;
        //                        }

        //                    if (!inClose)
        //                    {
        //                        int heuristic = p.GetHeuristic();

        //                        if (heuristic < hMin)
        //                        {
        //                            hMin = heuristic;
        //                            puzzleMin = p;
        //                        }
        //                    }
        //                }
        //            }

        //            if (findRecurs)
        //            {
        //                count_recurs++;
        //                queue_list.Enqueue(puzzleMin);
        //                if (count_recurs < 10000)
        //                {
        //                    Recursion2();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            queue_list.Clear();
        //            queue_list.Enqueue(puzzle);
        //        }
        //    }
        //}

        #region old

        List<BaseProgressItem> SolutionLogical(Puzzle puzzle)
        {
            List<BaseProgressItem> moves = new List<BaseProgressItem>();
            PointCell necPoint, point, pointZero;
            List<BaseProgressItem> list, listZero;
            DirectionVector direct;
            bool Error = false;

            #region 1-8

            for (int i = 1; i <= 8; i++)
            {
                Console.WriteLine("Solution logical cell {0}", i);

                necPoint = puzzle.PointsCanonicalMatrix[i];
                point = puzzle.GetPoint(i);

                if (point != necPoint)
                {
                    if (necPoint.X == puzzle.WidthPuzzle - 1)
                        necPoint.Y++;

                    if (necPoint.X == puzzle.WidthPuzzle - 2)
                        necPoint.X++;

                    if ((point == necPoint - 1) && (necPoint.X == puzzle.WidthPuzzle - 1) && (i == 8))
                    {
                        direct = new DirectionVector(DirectionCell.Left, Math.Abs(puzzle.WidthPuzzle - 1 - puzzle.PointZero.X));

                        puzzle.GoDirection(direct);
                        moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));

                        if (puzzle.PointZero.Y == puzzle.WidthPuzzle - 1)
                        {
                            direct = new DirectionVector(DirectionCell.Bottom, 1);

                            puzzle.GoDirection(direct);
                            moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));
                        }

                        direct = new DirectionVector(DirectionCell.Right, 1);
                        puzzle.GoDirection(direct);
                        moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));

                        direct = new DirectionVector(DirectionCell.Bottom, 1);
                        puzzle.GoDirection(direct);
                        moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));

                        direct = new DirectionVector(DirectionCell.Left, 1);
                        puzzle.GoDirection(direct);
                        moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));

                        direct = new DirectionVector(DirectionCell.Top, 2);
                        puzzle.GoDirection(direct);
                        moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));

                        direct = new DirectionVector(DirectionCell.Right, 1);
                        puzzle.GoDirection(direct);
                        moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));

                        direct = new DirectionVector(DirectionCell.Bottom, 2);
                        puzzle.GoDirection(direct);
                        moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));

                        i -= 2;
                        continue;
                    }

                    try
                    {
                        list = GetCellMoves2(puzzle, point, necPoint);
                        moves.AddRange(list.Reverse<BaseProgressItem>());

                        if (puzzle.PointsCanonicalMatrix[i].X == puzzle.WidthPuzzle - 1)
                        {
                            pointZero = new PointCell(puzzle.PointsCanonicalMatrix[i].X - 1, puzzle.PointsCanonicalMatrix[i].Y);
                            listZero = GetZeroMove(puzzle, pointZero);

                            for (int j = listZero.Count - 1; j >= 0; j--)
                            {
                                moves.Add(listZero[j]);
                            }

                            direct = new DirectionVector(DirectionCell.Left, 1);

                            puzzle.GoDirection(direct);
                            moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));

                            direct = new DirectionVector(DirectionCell.Top, 1);

                            puzzle.GoDirection(direct);
                            moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Error!");
                        Console.WriteLine("Error: {0}", exc.Message);
                        Error = true;
                        break;
                    }
                }
            }

            #endregion

            Error = false;

            #region 13, 9

            if ((!Error) && ((puzzle.GetPoint(9) != puzzle.PointsCanonicalMatrix[9]) || (puzzle.GetPoint(13) != puzzle.PointsCanonicalMatrix[13])))
            {
                Console.WriteLine("Solution logical cell {0}", 13);

                necPoint = puzzle.PointsCanonicalMatrix[9];
                point = puzzle.GetPoint(13);

                if (puzzle.GetPoint(9) + 1 == point)
                {
                    direct = new DirectionVector(DirectionCell.Left, 1);

                    puzzle.GoDirection(direct);
                    moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));

                    direct = new DirectionVector(DirectionCell.Top, 1);

                    puzzle.GoDirection(direct);
                    moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));
                }

                list = GetCellMoves2(puzzle, point, new PointCell(necPoint));

                for (int j = list.Count - 1; j >= 0; j--)
                {
                    moves.Add(list[j]);
                }

                Console.WriteLine("Solution logical cell {0}", 9);

                necPoint = puzzle.PointsCanonicalMatrix[10];
                point = puzzle.GetPoint(9);

                list = GetCellMoves2(puzzle, point, new PointCell(necPoint));

                for (int j = list.Count - 1; j >= 0; j--)
                {
                    moves.Add(list[j]);
                }

                if (puzzle.PointZero.Y != puzzle.WidthPuzzle - 1)
                {
                    direct = new DirectionVector(DirectionCell.Top, 1);

                    puzzle.GoDirection(direct);
                    moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));
                }

                pointZero = new PointCell(puzzle.PointsCanonicalMatrix[13]);
                listZero = GetZeroMove(puzzle, pointZero);

                for (int j = listZero.Count - 1; j >= 0; j--)
                {
                    moves.Add(listZero[j]);
                }

                direct = new DirectionVector(DirectionCell.Bottom, 1);

                puzzle.GoDirection(direct);
                moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));

                direct = new DirectionVector(DirectionCell.Left, 1);

                puzzle.GoDirection(direct);
                moves.Add(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));
            }

            #endregion

            Error = false;

            #region 10-12, 14, 15

            if (!Error)
            {
                findRecursion = false;

                Puzzle puzzleLast = new Puzzle(puzzle) { MultiAction = false };
                queue = new Queue<Puzzle>();

                puzzleLast.History.Clear();
                sH = puzzle.GetHeuristic();

                Recursion(ref puzzleLast, 0, 2);

                if (queue.Count > 0)
                {
                    list = queue.Dequeue().History.ToList();

                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        moves.Add(list[i]);
                    }
                }
            }

            #endregion

            return moves;
        }

        Queue<Puzzle> queue = new Queue<Puzzle>();
        List<Puzzle> closeListPuzzle = new List<Puzzle>();
        int sH = 0, countRec = 0;

        void Recursion(ref Puzzle puzzle, int minX, int minY)
        {
            if (!findRecursion)
            {
                PointCell[] points = puzzle.GetPointCanMove();
                closeListPuzzle.Add(puzzle);

                foreach (PointCell point in points)
                {
                    if ((point.X >= minX) && (point.Y >= minY))
                    {
                        Puzzle p = new Puzzle(puzzle);
                        DirectionVector direct = p.GetAllowedVectorDirection(point);
                        DirectionCell d = new DirectionCell();
                        if (p.History.Count > 0)
                            d = (p.History.Peek().Direction).Direction;

                        if (((p.History.Count > 0) && (d != direct.RevertDirection())) || (p.History.Count == 0))
                        {
                            p.GoDirection(direct);
                            int sh = p.GetHeuristic();
                            bool allow = true;
                            Console.WriteLine("{0}: {1}", countRec, puzzle.GetIndexCell(point));


                            foreach (Puzzle puz in closeListPuzzle)
                                if (EqualDoubleArray(puz.Matrix, p.Matrix))
                                {
                                    allow = false;
                                    break;
                                }

                            /*if (sh <= sH + 1)
                            {*/
                            //sH = sh;
                            //MessageBox.Show(p.ToString());

                            if (!allow)
                                break;

                            p.History.Push(new BaseProgressItem(point, direct));
                            queue.Enqueue(p);
                            //}
                        }
                    }
                }

                if (queue.Count > 0)
                {
                    Puzzle p2 = queue.Peek();
                    if (p2.IsWIN())
                    {
                        findRecursion = true;
                    }
                    else
                    {
                        queue.Dequeue();
                        countRec++;
                        if (countRec < 2000)
                            Recursion(ref p2, minX, minY);
                    }
                }
            }
        }

        public static List<BaseProgressItem> GetCellMoves2(Puzzle puzzleM, PointCell start, PointCell end)
        {
            Puzzle puzzle = puzzleM;

            puzzle.History.Clear();

            if (start != end)
            {
                PointCell pointCell = new PointCell(start);
                int numberCell = puzzle.GetIndexCell(pointCell);
                int countItteration = 0, countMax = 12;
                int x1 = end.X;
                int y1 = end.Y;
                int w = 2;
                int h = 2;
                //проверить чтобы 0 не был перед 3 в момент построения захода на позиции

                if (x1 >= puzzle.WidthPuzzle - 1)
                    x1--;

                if (pointCell.X < x1)
                {
                    if (puzzle.PointZero.Y != pointCell.Y)
                    {
                        int mo;
                        DirectionCell directCell;
                        DirectionVector direct;

                        if (puzzle.PointZero.X == pointCell.X)
                        {
                            direct = new DirectionVector(DirectionCell.Left, 1);

                            puzzle.GoDirection(direct);
                            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                            pointCell = puzzle.GetPoint(numberCell);
                        }

                        mo = Math.Abs(puzzle.PointZero.Y - pointCell.Y);
                        directCell = (puzzle.PointZero.Y > pointCell.Y) ? DirectionCell.Bottom : DirectionCell.Top;
                        direct = new DirectionVector(directCell, mo);

                        puzzle.GoDirection(direct);
                        puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                        pointCell = puzzle.GetPoint(numberCell);
                    }

                    if (puzzle.PointZero.Y == pointCell.Y)
                    {
                        int mo = puzzle.WidthPuzzle - 1 - puzzle.PointZero.X;
                        DirectionVector direct = new DirectionVector(DirectionCell.Left, mo);

                        puzzle.GoDirection(direct);
                        puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                        pointCell = puzzle.GetPoint(numberCell);
                    }

                    countItteration = 0;
                    int x2 = pointCell.X;
                    int y2 = (pointCell.Y >= puzzle.WidthPuzzle - 1) ? pointCell.Y - 1 : pointCell.Y;
                    int w2 = Math.Max(Math.Abs(x1 - pointCell.X) + 2, 2);

                    while (pointCell.X != x1)
                    {
                        foreach (BaseProgressItem bpi in Puzzle.ManeuversRotate(puzzle, x2, y2, w2, 2, false))
                        {
                            puzzle.GoDirection(bpi.Direction);
                            puzzle.History.Push(bpi);
                        }
                        pointCell = puzzle.GetPoint(numberCell);
                        x2 = pointCell.X;
                        w2 = Math.Max(Math.Abs(puzzle.PointZero.X - x2) + 1, 2);

                        countItteration++;
                        if (countItteration > countMax)
                            break;
                    }

                    pointCell = puzzle.GetPoint(numberCell);
                }

                if ((pointCell.X >= x1) && (pointCell.Y >= y1))
                {
                    h = Math.Max(pointCell.Y - y1 + 1, 2);

                    if (puzzle.PointZero.Y < y1)
                    {
                        int mo = y1 - puzzle.PointZero.Y + 1;
                        DirectionCell directCell = DirectionCell.Top;
                        DirectionVector direct = new DirectionVector(directCell, mo);

                        puzzle.GoDirection(direct);
                        puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                        pointCell = puzzle.GetPoint(numberCell);
                    }

                    if (puzzle.PointZero.X < x1)
                    {
                        if (puzzle.PointZero.Y == pointCell.Y)
                        {
                            DirectionCell directCell = (puzzle.PointZero.Y >= puzzle.WidthPuzzle - 1) ? DirectionCell.Bottom : DirectionCell.Top;
                            DirectionVector direct = new DirectionVector(directCell, 1);

                            puzzle.GoDirection(direct);
                            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                            pointCell = puzzle.GetPoint(numberCell);
                        }

                        if (puzzle.PointZero.Y != pointCell.Y)
                        {
                            int mo = puzzle.WidthPuzzle - 1 - puzzle.PointZero.X;
                            DirectionVector direct = new DirectionVector(DirectionCell.Left, mo);

                            puzzle.GoDirection(direct);
                            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                            pointCell = puzzle.GetPoint(numberCell);
                        }
                    }

                    if (puzzle.PointZero.X != pointCell.X)
                    {
                        if (puzzle.PointZero.Y != pointCell.Y)
                        {
                            int mo = Math.Abs(puzzle.PointZero.Y - pointCell.Y);
                            DirectionCell directCell = (puzzle.PointZero.Y > pointCell.Y) ? DirectionCell.Bottom : DirectionCell.Top;
                            DirectionVector direct = new DirectionVector(directCell, mo);

                            puzzle.GoDirection(direct);
                            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                            pointCell = puzzle.GetPoint(numberCell);
                        }

                        if (puzzle.PointZero.X > pointCell.X)
                        {
                            w = Math.Max(puzzle.PointZero.X - x1 + 1, 2);
                        }
                        else
                        {
                            w = Math.Max(pointCell.X - x1 + 1, 2);
                        }
                    }
                    else
                    {
                        /*if ((puzzle.PointZero.X == pointCell.X) && (pointCell.X < puzzle.WidthPuzzle - 1) && (pointCell.Y == puzzle.WidthPuzzle - 1))
                        {
                            DirectionVector direct = new DirectionVector(DirectionCell.Left, 1);

                            puzzle.GoDirection(direct);
                            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                            pointCell = puzzle.GetPoint(numberCell);
                        }*/

                        w = Math.Max(pointCell.X - x1 + 1, 2);

                        if (puzzle.PointZero.Y > pointCell.Y)
                        {
                            h = Math.Max(puzzle.PointZero.Y - y1 + 1, 2);
                        }
                        else
                        {
                            h = Math.Max(pointCell.Y - y1 + 1, 2);
                        }
                    }
                }

                countItteration = 0;

                while (pointCell != end)
                {
                    foreach (BaseProgressItem bpi in Puzzle.ManeuversRotate(puzzle, x1, y1, w, h, false))
                    {
                        puzzle.GoDirection(bpi.Direction);
                        puzzle.History.Push(bpi);
                    }
                    pointCell = puzzle.GetPoint(numberCell);

                    countItteration++;
                    if (countItteration > countMax)
                        break;
                }

                pointCell = puzzle.GetPoint(numberCell);
            }

            return puzzle.History.ToList();
        }

        public static List<BaseProgressItem> GetZeroMove(Puzzle puzzleM, PointCell end)
        {
            Puzzle puzzle = puzzleM;

            puzzle.History.Clear();

            if (puzzle.PointZero.X != end.X)
            {
                int mo = Math.Abs(puzzle.PointZero.X - end.X);
                DirectionCell directCell = (puzzle.PointZero.X > end.X) ? DirectionCell.Right : DirectionCell.Left;
                DirectionVector direct = new DirectionVector(directCell, mo);

                puzzle.GoDirection(direct);
                puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));
            }

            if (puzzle.PointZero.Y != end.Y)
            {
                int mo = Math.Abs(puzzle.PointZero.Y - end.Y);
                DirectionCell directCell = (puzzle.PointZero.Y > end.Y) ? DirectionCell.Bottom : DirectionCell.Top;
                DirectionVector direct = new DirectionVector(directCell, mo);

                puzzle.GoDirection(direct);
                puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero), direct));
            }

            return puzzle.History.ToList();
        }

        public static List<BaseProgressItem> GetCellMoves(Puzzle puzzleM, PointCell start, PointCell end)
        {
            Puzzle puzzle = puzzleM;//new Puzzle(puzzleM.WidthPuzzle) { Matrix = puzzleM.Matrix, PointZero = puzzleM.PointZero };
            puzzle.History.Clear();
            PointCell pointCell = new PointCell(start);
            int numberCell = puzzle.GetIndexCell(pointCell);

            try
            {
                if (start != end)
                {
                    // Move ZERO to START

                    #region Start move

                    if (puzzle.PointZero == end)
                    {
                        if ((start.X == end.X) && (start.Y - 1 == end.Y))
                        {
                            DirectionVector direct = new DirectionVector(DirectionCell.Top, 1);

                            puzzle.GoDirection(direct);
                            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                        }
                        if ((start.Y == end.Y) && (start.X - 1 == end.X))
                        {
                            DirectionVector direct = new DirectionVector(DirectionCell.Left, 1);

                            puzzle.GoDirection(direct);
                            puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                        }
                    }

                    #endregion

                    if (pointCell.X != puzzle.WidthPuzzle - 1)
                    {
                        if (pointCell != end)
                        {
                            if (puzzle.PointZero.X <= pointCell.X)
                            {
                                int mo = pointCell.X + 1 - puzzle.PointZero.X;

                                if (pointCell.Y == puzzle.PointZero.Y)
                                    mo--;

                                DirectionVector direct = new DirectionVector(DirectionCell.Left, mo);

                                puzzle.GoDirection(direct);
                                puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                                pointCell = puzzle.GetPoint(numberCell);
                            }

                            if (puzzle.PointZero.Y != pointCell.Y)
                            {
                                int mo = Math.Abs(puzzle.PointZero.Y - pointCell.Y);
                                DirectionCell directCell = (puzzle.PointZero.Y < pointCell.Y) ? DirectionCell.Top : DirectionCell.Bottom;
                                DirectionVector direct = new DirectionVector(directCell, mo);

                                puzzle.GoDirection(direct);
                                puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                                pointCell = puzzle.GetPoint(numberCell);
                            }

                            if ((pointCell.X != end.X) && (pointCell.Y != end.Y))
                            {
                                int mToLeft = -1;
                                bool cToLeft = false;

                                if (pointCell.Y == end.Y + 1)
                                {
                                    mToLeft = 0;
                                    cToLeft = true;
                                }

                                while (pointCell.X != puzzle.WidthPuzzle - 1)
                                {
                                    foreach (BaseProgressItem bpi in Puzzle.ManeuversRotate(puzzle, pointCell.X, pointCell.Y + mToLeft, puzzle.WidthPuzzle - pointCell.X, 2, cToLeft))
                                    {
                                        puzzle.GoDirection(bpi.Direction);
                                        puzzle.History.Push(bpi);
                                    }
                                    pointCell = puzzle.GetPoint(numberCell);
                                }

                                if ((puzzle.PointZero.X != 0) && (puzzle.PointZero.X != puzzle.WidthPuzzle - 1) && (puzzle.PointZero.Y != 0) && (puzzle.PointZero.Y != puzzle.WidthPuzzle - 1))
                                {
                                    if (puzzle.PointZero.Y == pointCell.Y)
                                    {
                                        foreach (BaseProgressItem bpi in Puzzle.ManeuversRotate(puzzle, puzzle.PointZero.X, puzzle.PointZero.Y, 2, 2))
                                        {
                                            puzzle.GoDirection(bpi.Direction);
                                            puzzle.History.Push(bpi);
                                        }
                                        pointCell = puzzle.GetPoint(numberCell);
                                    }
                                    else
                                    {
                                        DirectionVector direct = new DirectionVector(DirectionCell.Left, 1);

                                        puzzle.GoDirection(direct);
                                        puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                                    }
                                }
                            }
                        }
                    }

                    if ((puzzle.PointZero.X != end.X) && (puzzle.PointZero.X != pointCell.X) && (puzzle.PointZero.Y != end.Y) && (puzzle.PointZero.Y != pointCell.Y))
                    {
                        int mo = puzzle.WidthPuzzle - 1 - puzzle.PointZero.X;
                        DirectionVector direct = new DirectionVector(DirectionCell.Left, mo);

                        puzzle.GoDirection(direct);
                        puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                        pointCell = puzzle.GetPoint(numberCell);
                    }

                    bool cRotate = false;
                    pointCell = puzzle.GetPoint(numberCell);

                    if (pointCell.X == end.X)
                    {
                        cRotate = true;
                    }

                    if (pointCell.Y == end.Y)
                    {
                        cRotate = false;
                    }

                    int countItteration = 0, countMax = 12;

                    while (pointCell != end)
                    {
                        int x1 = end.X;
                        int y1 = end.Y;
                        int x2 = Math.Max(pointCell.X - end.X + 1, 2);
                        int y2 = Math.Max(pointCell.Y - end.Y + 1, 2);

                        if (x1 >= puzzle.WidthPuzzle - 1)
                            x1--;

                        if (y1 >= puzzle.WidthPuzzle - 1)
                            y1--;

                        if (puzzle.PointZero > pointCell)
                        {
                            x2 = Math.Max(puzzle.PointZero.X - end.X + 1, 2);
                            y2 = Math.Max(puzzle.PointZero.Y - end.Y + 1, 2);
                        }

                        foreach (BaseProgressItem bpi in Puzzle.ManeuversRotate(puzzle, x1, y1, x2, y2, cRotate))
                        {
                            puzzle.GoDirection(bpi.Direction);
                            puzzle.History.Push(bpi);
                        }
                        pointCell = puzzle.GetPoint(numberCell);

                        countItteration++;
                        if (countItteration > countMax)
                            break;
                    }

                    pointCell = puzzle.GetPoint(numberCell);

                    if (pointCell != end)
                    {
                        countItteration = 0;
                        countMax = 12;

                        while (pointCell != end)
                        {
                            int x2 = Math.Min(pointCell.X, end.X);
                            int y2 = Math.Min(pointCell.Y, end.Y);

                            if (x2 >= puzzle.WidthPuzzle - 1)
                                x2--;

                            if (y2 >= puzzle.WidthPuzzle - 1)
                                y2--;

                            foreach (BaseProgressItem bpi in Puzzle.ManeuversRotate(puzzle, end.X, end.Y, 2, 2))
                            {
                                puzzle.GoDirection(bpi.Direction);
                                puzzle.History.Push(bpi);
                            }
                            pointCell = puzzle.GetPoint(numberCell);

                            countItteration++;
                            if (countItteration > countMax)
                                break;
                        }
                    }

                    #region 1
                    /*
                PointCell pointZeroStart = new PointCell(start);
                pointZeroStart.Y--;
                
                if (pointCell.X != end.X)
                {

                }

                if ((puzzle.PointZero.X < pointCell.X) && (puzzle.PointZero.Y <= pointCell.Y))
                {
                    int mo = pointCell.Y - puzzle.PointZero.Y + 1;
                    DirectionVector direct = new DirectionVector(DirectionCell.Top, mo);

                    puzzle.GoDirection(direct);
                    puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                }

                if (puzzle.PointZero.X <= pointCell.X)
                {
                    int mo = pointCell.X - puzzle.PointZero.X + 1;
                    DirectionVector direct = new DirectionVector(DirectionCell.Left, mo);

                    puzzle.GoDirection(direct);
                    puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, puzzle.PointZero.Y), direct));
                }

                if (pointZeroStart.Y != puzzle.PointZero.Y)
                {
                    int mo = Math.Abs(pointZeroStart.Y - puzzle.PointZero.Y);
                    DirectionCell directCell = (pointZeroStart.Y > puzzle.PointZero.Y) ? DirectionCell.Top : DirectionCell.Bottom;
                    DirectionVector direct = new DirectionVector(directCell, mo);

                    puzzle.GoDirection(direct);
                    puzzle.History.Push(new BaseProgressItem(new PointCell(puzzle.PointZero.X, pointZeroStart.Y), direct));
                }

                if (pointZeroStart.X != puzzle.PointZero.X)
                {
                    int mo = Math.Abs(pointZeroStart.X - puzzle.PointZero.X);
                    DirectionCell directCell = (pointZeroStart.X > puzzle.PointZero.X) ? DirectionCell.Left : DirectionCell.Right;
                    DirectionVector direct = new DirectionVector(directCell, mo);

                    puzzle.GoDirection(direct);
                    puzzle.History.Push(new BaseProgressItem(new PointCell(pointZeroStart.X, puzzle.PointZero.Y), direct));
                }

                //Move top 1
                DirectionVector vector = new DirectionVector(DirectionCell.Top, 1);

                puzzle.GoDirection(vector);
                puzzle.History.Push(new BaseProgressItem(puzzle.PointZero, vector));

                pointZeroStart.Y--;
                pointCell.Y--;

                //Maneuvers
                if ((pointCell.X == end.X) && (pointCell.Y < end.Y))
                    while (pointCell != end)
                    {
                        foreach (BaseProgressItem bpi in puzzle.ManeuversBypassSide(puzzle.PointZero, true, false))
                        {
                            puzzle.History.Push(bpi.Revert());
                            puzzle.GoDirection(bpi.Revert().Direction);
                        }

                        puzzle.GoDirection(new DirectionVector(DirectionCell.Top, 1));
                        puzzle.History.Push(new BaseProgressItem(puzzle.PointZero, new DirectionVector(DirectionCell.Top, 1)));

                        pointZeroStart.Y--;
                        pointCell.Y--;
                    }*/
                    #endregion
                }
            }
            catch
            {
                return puzzle.History.ToList();
            }


            return puzzle.History.ToList();
        }

        /*void recursion(ref Puzzle puzzle)
        {
            if (!findRecursion)
            {
                PointCell[] points = puzzle.GetPointCanMove();

                foreach (PointCell point in points)
                {
                    Puzzle p = new Puzzle(puzzle);
                    DirectionVector direct = p.GetAllowedVectorDirection(point);
                    DirectionCell d = new DirectionCell();
                    if (p.History.Count > 0)
                        d = (p.History.Peek().Direction).Direction;

                    if (((p.History.Count > 0) && (d != direct.Direction)) || (p.History.Count == 0))
                    {
                        p.GoDirection(direct);
                        int sh = p.GetHeuristic();

                        if (sh <= sH)
                        {
                            //sH = sh;
                            sH--;
                            //MessageBox.Show(p.ToString());

                            p.History.Push(new BaseProgressItem(point, direct));
                            queue.Enqueue(p);
                        }
                    }
                }

                if (queue.Count > 0)
                {
                    Puzzle p2 = queue.Peek();
                    if (p2.IsWIN())
                    {
                        findRecursion = true;
                    }
                    else
                    {
                        queue.Dequeue();
                        recursion(ref p2);
                    }
                }
            }
        }*/

        #endregion
    }

    [Serializable]
    public class Puzzle
    {
        public bool MultiAction = true;
        public int WidthPuzzle = 0;
        public int CountCells = 0;
        public int[][] Matrix;          //public List<List<int>> Matrix = new List<List<int>>();
        public PointCell PointZero;
        public Stack<BaseProgressItem> History = new Stack<BaseProgressItem>();

        private int[][] CanonicalMatrix;
        public List<PointCell> PointsCanonicalMatrix;

        public Puzzle(Puzzle puzzle)
        {
            puzzle = DeepCopy<Puzzle>(puzzle);
            this.MultiAction = puzzle.MultiAction;
            this.WidthPuzzle = puzzle.WidthPuzzle;
            this.CountCells = puzzle.CountCells;
            this.Matrix = puzzle.Matrix;
            this.PointZero = puzzle.PointZero;
            this.History = puzzle.History;
            this.CanonicalMatrix = puzzle.CanonicalMatrix;
            this.PointsCanonicalMatrix = puzzle.PointsCanonicalMatrix;
        }

        public Puzzle(int width, bool shake = false)
        {
            Matrix = new int[width][];
            PointsCanonicalMatrix = new List<PointCell>(width) { new PointCell() };
            WidthPuzzle = width;
            CountCells = (int)Math.Pow(width, 2) - 1;

            int k = 1;

            for (int i = 0; i < WidthPuzzle; i++)
            {
                Matrix[i] = new int[width];

                for (int j = 0; j < WidthPuzzle; j++, k++)
                {
                    Matrix[i][j] = k;
                    PointsCanonicalMatrix.Add(new PointCell(i, j, true));
                }
            }

            #region Test bad matrix !without shake!
            /*Matrix[WidthPuzzle - 1][WidthPuzzle - 3] += Matrix[WidthPuzzle - 1][WidthPuzzle - 2];
            Matrix[WidthPuzzle - 1][WidthPuzzle - 2] = Matrix[WidthPuzzle - 1][WidthPuzzle - 3] - Matrix[WidthPuzzle - 1][WidthPuzzle - 2];
            Matrix[WidthPuzzle - 1][WidthPuzzle - 3] -= Matrix[WidthPuzzle - 1][WidthPuzzle - 2];*/
            #endregion

            PointZero = new PointCell(WidthPuzzle - 1, WidthPuzzle - 1);
            Matrix[WidthPuzzle - 1][WidthPuzzle - 1] = 0;
            PointsCanonicalMatrix[0] = PointZero;
            CanonicalMatrix = DeepCopy<int[][]>(Matrix);

            if (shake)
                ShakeMatrix();
        }

        static T DeepCopy<T>(T other)
        {
            if (other == null)
                return default(T);

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
        
        public void ShakeMatrix()
        {
            Random r = new Random();

            for (int j = 0; j < WidthPuzzle; j++)
                for (int i = WidthPuzzle - 1; i >= 0; i--)
                {
                    int i2 = r.Next(0, i);
                    int column1 = Matrix[j][i];

                    Matrix[j][i] = Matrix[j][i2];
                    Matrix[j][i2] = column1;
                }

            for (int i = WidthPuzzle - 1; i >= 0; i--)
            {
                int i2 = r.Next(0, i);
                int[] row1 = Matrix[i];

                Matrix[i] = Matrix[i2];
                Matrix[i2] = row1;
            }

            for (int j = 0; j < WidthPuzzle; j++)
                for (int i = WidthPuzzle - 1; i >= 0; i--)
                {
                    int j2 = r.Next(0, j);
                    int column1 = Matrix[j][i];

                    Matrix[j][i] = Matrix[j2][i];
                    Matrix[j2][i] = column1;
                }

            for (int i = 0; i < WidthPuzzle; i++)
                for (int j = 0; j < WidthPuzzle; j++)
                    if (Matrix[i][j] == 0)
                        PointZero = new PointCell(j, i);

            if (IsFAIL())
                ShakeMatrix();
        }
        
        public int GetIndexCell(PointCell point)
        {
            return GetIndexCell(point.X, point.Y);
        }

        public int GetIndexCell(int x, int y)
        {
            int res = Matrix[y][x];
            return res;
        }

        public PointCell GetPoint(int number)
        {
            for (int i = 0; i < WidthPuzzle; i++)
                for (int j = 0; j < WidthPuzzle; j++)
                    if (Matrix[i][j] == number)
                        return new PointCell(i, j, true);

            return new PointCell(0, 0);
        }

        private DirectionCell GetAllowedDirectionCell(PointCell point)
        {
            int x = point.X, y = point.Y;

            //Console.WriteLine("Get AllowedDirection: {0} {1}", x, y);

            PointCell newPoint = PointZero - point;

            if ((newPoint.X == 0) || (newPoint.Y == 0))
            {
                if ((!MultiAction) && ((Math.Abs(newPoint.X) > 1) || (Math.Abs(newPoint.Y) > 1)))
                    return DirectionCell.Null;

                if (newPoint.Y < 0)
                    return DirectionCell.Top;

                if (newPoint.X > 0)
                    return DirectionCell.Right;

                if (newPoint.Y > 0)
                    return DirectionCell.Bottom;

                if (newPoint.X < 0)
                    return DirectionCell.Left;
            }

            return DirectionCell.Null;
        }

        public DirectionVector GetAllowedVectorDirection(PointCell point)
        {
            DirectionVector vector = new DirectionVector();

            vector.Direction = GetAllowedDirectionCell(point);

            if (vector.Direction != DirectionCell.Null)
                if ((vector.Direction == DirectionCell.Left) || (vector.Direction == DirectionCell.Right))
                    vector.Length = Math.Abs(PointZero.X - point.X);
                else
                    vector.Length = Math.Abs(PointZero.Y - point.Y);

            if ((!MultiAction) && (vector.Length > 1))
                vector = new DirectionVector();


            return vector;
        }

        public PointCell[] GetPointCanMove()
        {
            List<PointCell> points = new List<PointCell>();

            for (int i = 0; i < WidthPuzzle; i++)
                for (int j = 0; j < WidthPuzzle; j++)
                {
                    PointCell point = new PointCell(i, j);
                    DirectionCell direct = GetAllowedDirectionCell(point);

                    if (direct != DirectionCell.Null)
                    {
                        points.Add(point);
                    }
                }

            return points.ToArray();
        }

        public List<PointCell> GetPointsNearPoint(PointCell point)
        {
            List<PointCell> points = new List<PointCell>();
            int X = point.X, Y = point.Y;

            if (Y > 0)
                points.Add(new PointCell(X, Y - 1));
            if (X > 0)
                points.Add(new PointCell(X - 1, Y));
            if (Y < WidthPuzzle - 1)
                points.Add(new PointCell(X, Y + 1));
            if (X < WidthPuzzle - 1)
                points.Add(new PointCell(X + 1, Y));

            return points;
        }

        public int GetCostMovesByOriginalPuzzle(Puzzle OriginalPuzzle)
        {
            Console.WriteLine("GET COST!!!");

            int G = 0;

            for (int i = 0; i < Matrix.Length; i++)
                for (int j = 0; j < Matrix[i].Length; j++)
                    if (Matrix[i][j] != OriginalPuzzle.Matrix[i][j])
                    {
                        PointCell point1 = new PointCell(j, i);
                        int indexPoint1 = GetIndexCell(point1);

                        if (indexPoint1 != 0)
                        {
                            int Gmin = PointCell.Difference(point1, OriginalPuzzle.GetPoint(indexPoint1));
                            G += Gmin;
                        }
                    }

            return 10 * G;
        }

        public Puzzle[] GetPuzzleListFromPoints(PointCell[] points, bool addToHistory = true)
        {
            List<Puzzle> puzzles = new List<Puzzle>();

            foreach (PointCell point in GetPointCanMove())
            {
                Puzzle p = new Puzzle(this);
                DirectionVector vector = p.GetAllowedVectorDirection(point);

                p.GoDirection(vector, addToHistory);
                puzzles.Add(p);
            }

            return puzzles.ToArray();
        }

        public Puzzle PuzzleGoDirect(DirectionVector directionVector)
        {
            Puzzle puzzle = new Puzzle(this);
            puzzle.GoDirection(directionVector);

            return puzzle;
        }

        public void GoDirection(DirectionVector directionVector, bool addToHistory = false)
        {
            //Puzzle puzzle = new Puzzle(this);
            if (addToHistory)
                History.Push(new BaseProgressItem(directionVector.PointNow(PointZero), directionVector));

            for (int i = 0; i < directionVector.Length; i++)
            {
                PointCell newPointZero = PointZero;

                switch (directionVector.Direction)
                {
                    case DirectionCell.Top:
                        newPointZero.Y++;
                        break;

                    case DirectionCell.Right:
                        newPointZero.X--;
                        break;

                    case DirectionCell.Bottom:
                        newPointZero.Y--;
                        break;

                    case DirectionCell.Left:
                        newPointZero.X++;
                        break;
                }

                //Console.WriteLine("Go direct cell {0}: {1} {2} {3}", GetIndexCell(newPointZero), newPointZero, directionVector.Direction, PointZero);

                Matrix[PointZero.Y][PointZero.X] = Matrix[newPointZero.Y][newPointZero.X];
                Matrix[newPointZero.Y][newPointZero.X] = 0;

                PointZero = newPointZero;
            }
        }

        public int GetDistance(PointCell point1, PointCell point2)
        {
            return PointCell.Difference(point1, point2);
        }

        public int GetDistanceManhattenPoint(PointCell point)
        {
            PointCell pointCannonical = PointsCanonicalMatrix[GetIndexCell(point)];

            if (pointCannonical == point)
                return 0;
            else
                return GetDistance(point, pointCannonical);
        }

        public int GetDistanceManhatten()
        {
            int res = 0;

            for (int i = 0; i < WidthPuzzle; i++)
                for (int j = 0; j < WidthPuzzle; j++)
                {
                    PointCell pc = new PointCell(i, j, true);

                    if (GetIndexCell(pc) != 0)
                        res += GetDistanceManhattenPoint(pc);
                }

            return res;
        }

        public PointCell[] GetLinearConflict()
        {
            List<PointCell> points = new List<PointCell>();

            for (int i = 0; i < WidthPuzzle; i++)
                for (int j = 0; j < WidthPuzzle; j++)
                {
                    int id1 = GetIndexCell(j, i);
                    int id2 = GetIndexCell(i, j);

                    if (((id1 != 0) && (PointsCanonicalMatrix[id1].Y == i)) || ((id2 != 0) && (PointsCanonicalMatrix[id2].X == i)))
                    {
                        for (int k = j + 1; k < WidthPuzzle; k++)
                        {
                            int id12 = GetIndexCell(k, i);
                            int id22 = GetIndexCell(i, k);

                            if ((id12 != 0) && (PointsCanonicalMatrix[id12].Y == i) && (id1 > id12))
                            {
                                points.Add(new PointCell(j, i));
                                points.Add(new PointCell(k, i));
                            }

                            if ((id22 != 0) && (PointsCanonicalMatrix[id22].X == i) && (id2 > id22))
                            {
                                points.Add(new PointCell(i, j));
                                points.Add(new PointCell(i, k));
                            }
                        }
                    }
                }
            
            return points.ToArray();
        }

        public PointCell[] GetCornerConflict()
        {
            List<PointCell> points = new List<PointCell>();

            // Check top-left corner
            if ((Matrix[0][1] == CanonicalMatrix[0][1]) && (Matrix[1][0] == CanonicalMatrix[1][0]) && (Matrix[0][0] != CanonicalMatrix[0][0]))
                points.Add(new PointCell(0, 0));

            // Check top-right corner
            if ((Matrix[0][WidthPuzzle - 2] == CanonicalMatrix[0][WidthPuzzle - 2]) && (Matrix[1][WidthPuzzle - 1] == CanonicalMatrix[1][WidthPuzzle - 1]) && (Matrix[0][WidthPuzzle - 1] != CanonicalMatrix[0][WidthPuzzle - 1]))
                points.Add(new PointCell(0, 0));

            // Check bottom-right corner
            if ((Matrix[WidthPuzzle - 1][WidthPuzzle - 2] == CanonicalMatrix[WidthPuzzle - 1][WidthPuzzle - 2]) && (Matrix[WidthPuzzle - 2][WidthPuzzle - 1] == CanonicalMatrix[WidthPuzzle - 2][WidthPuzzle - 1]) && (Matrix[WidthPuzzle - 1][WidthPuzzle - 1] != CanonicalMatrix[WidthPuzzle - 1][WidthPuzzle - 1]))
                points.Add(new PointCell(0, 0));

            // Check bottom-left corner
            if ((Matrix[WidthPuzzle - 1][1] == CanonicalMatrix[WidthPuzzle - 1][1]) && (Matrix[WidthPuzzle - 2][0] == CanonicalMatrix[WidthPuzzle - 2][0]) && (Matrix[WidthPuzzle - 1][0] != CanonicalMatrix[WidthPuzzle - 1][0]))
                points.Add(new PointCell(0, 0));

            return points.ToArray();
        }

        public int GetHeuristic()
        {
            PointCell[] LinearConflict = GetLinearConflict();
            List<PointCell> CornerConflict = GetCornerConflict().ToList<PointCell>();

            for (int i = 0; i < CornerConflict.Count; i++)
                for (int j = 0; j < LinearConflict.Length; j++)
                    try
                    {
                        if (CornerConflict[i] == LinearConflict[j])
                        {
                            CornerConflict.RemoveAt(i);
                            i--;
                        }
                    }
                    catch { }

            int M = 10 * GetDistanceManhatten();
            int L = LinearConflict.Length;
            int C = CornerConflict.Count;

            return M;
        }

        public int GetHeuristicForOne(PointCell point)
        {
            return GetDistanceManhattenPoint(point);
        }

        public bool IsWIN()
        {
            return (GetHeuristic() == 0) ? true : false;
        }

        public bool IsFAIL()
        {
            int[] array = new int[15];
            int k = 0, e = 0;

            for (int i = 0; i < WidthPuzzle; i++)
                for (int j = 0; j < WidthPuzzle; j++, k++)
                    if (Matrix[i][j] != 0)
                        array[k] = Matrix[i][j];
                    else
                    {
                        k--;
                        e = i + 1;
                    }

            int sum = 0;

            for (int i = 0; i < array.Length; i++)
                for (int j = i + 1; j < array.Length; j++)
                    if (array[j] < array[i])
                        sum++;
            sum += e;

            //Console.WriteLine(sum);

            if (sum % 2 == 0)
                return false;
            else
                return true;
        }

        #region Running
        //Zero must be a near point!!!

        public List<BaseProgressItem> ManeuversRotateOuterRing(bool clockwise, int stepCount = 4)
        {
            List<BaseProgressItem> list = new List<BaseProgressItem>();



            return list;
        }

        public List<BaseProgressItem> ManeuversBypassSide(PointCell point, bool atop, bool left)
        {
            List<BaseProgressItem> list = new List<BaseProgressItem>();
            DirectionCell side = DirectionCell.Left;
            DirectionCell top = DirectionCell.Top;
            int ox = left ? 1 : -1;
            int multiX = -1, multiY = -1;
            
            if (!left)
            {
                side = DirectionCell.Right;
                multiX = 1;
            }

            if (!atop)
            {
                top = DirectionCell.Bottom;
                multiY = 1;
            }

            DirectionVector directX = new DirectionVector(side, 1);
            DirectionVector directY = new DirectionVector(top, 2);

            list.Add(new BaseProgressItem(point, directX));
            point.X += multiX * 1;

            list.Add(new BaseProgressItem(point, directY));
            point.Y += multiY * 2;

            list.Add(new BaseProgressItem(point, !directX));
            point.X -= multiX * 1;

            return list;
        }

        public static List<BaseProgressItem> ManeuversRotate(Puzzle puzzle, int x, int y, int width, int height, bool rotateToRight = true, int countStep = 1)
        {
            //Puzzle puzzle = new Puzzle(p);
            List<BaseProgressItem> list = new List<BaseProgressItem>();
            int x1 = x + width - 1, y1 = y + height - 1;
            int x0 = puzzle.PointZero.X, y0 = puzzle.PointZero.Y;
            bool rotate = false;

            if ((x0 >= x) && (y0 >= y) && (x0 <= x1) && (y0 <= y1))
            {
                if (rotateToRight)
                {
                    if ((y0 == y) && (x0 != x) && (x0 != x1))
                    {
                        list.Add(ManeuversMoveToLeftRight(x, y, x0 - x, false));
                        list.Add(ManeuversMoveToTopBottom(x, y, y1, true));
                        list.Add(ManeuversMoveToLeftRight(x, y1, x1, true));
                        list.Add(ManeuversMoveToTopBottom(x1, y, y1, false));
                        if (x0 != x1 - 1)
                            list.Add(ManeuversMoveToLeftRight(x0 + 1, y0, x1 - x0 - 1, false));

                        rotate = true;
                    }

                    if ((y0 == y1) && (x0 != x) && (x0 != x1))
                    {
                        list.Add(ManeuversMoveToLeftRight(x0, y1, x1, true));
                        list.Add(ManeuversMoveToTopBottom(x1, y, y1, false));
                        list.Add(ManeuversMoveToLeftRight(x, y, x1, false));
                        list.Add(ManeuversMoveToTopBottom(x, y, y1, true));
                        if (x0 != x + 1)
                            list.Add(ManeuversMoveToLeftRight(x, y1, y1 - x0, true));

                        rotate = true;
                    }

                    if ((x0 == x) && (y0 != y) && (y0 != y1))
                    {
                        list.Add(ManeuversMoveToTopBottom(x, y0, y1, true));
                        list.Add(ManeuversMoveToLeftRight(x, y1, x1, true));
                        list.Add(ManeuversMoveToTopBottom(x1, y, y1, false));
                        list.Add(ManeuversMoveToLeftRight(x, y, x1, false));
                        if (y0 != y + 1)
                            list.Add(ManeuversMoveToTopBottom(x, y, y0 - 1, true));

                        rotate = true;
                    }

                    if ((x0 == x1) && (y0 != y) && (y0 != y1))
                    {
                        list.Add(ManeuversMoveToTopBottom(x0, y, y0, false));
                        list.Add(ManeuversMoveToLeftRight(x, y, x1, false));
                        list.Add(ManeuversMoveToTopBottom(x, y, y1, true));
                        list.Add(ManeuversMoveToLeftRight(x, y1, x1, true));
                        if (y0 != y1 - 1)
                            list.Add(ManeuversMoveToTopBottom(x0, y0 + 1, y1, false));

                        rotate = true;
                    }

                    if (!rotate)
                        if (y0 == y)
                            if (x0 == x)
                            {
                                list.Add(ManeuversMoveToTopBottom(x, y0, y1, true));
                                list.Add(ManeuversMoveToLeftRight(x, y1, x1, true));
                                list.Add(ManeuversMoveToTopBottom(x1, y, y1, false));
                                if (width > 2)
                                    list.Add(ManeuversMoveToLeftRight(x + 1, y, x1, false));
                            }
                            else
                            {
                                list.Add(ManeuversMoveToLeftRight(x, y, x0, false));
                                list.Add(ManeuversMoveToTopBottom(x, y, y1, true));
                                list.Add(ManeuversMoveToLeftRight(x, y1, x1, true));
                                if (height > 2)
                                    list.Add(ManeuversMoveToTopBottom(x1, y + 1, y1, false));
                            }
                        else
                            if (x0 == x)
                            {
                                list.Add(ManeuversMoveToLeftRight(x0, y0, x1, true));
                                list.Add(ManeuversMoveToTopBottom(x1, y, y1, false));
                                list.Add(ManeuversMoveToLeftRight(x, y, x1, false));
                                if (height > 2)
                                    list.Add(ManeuversMoveToTopBottom(x, y, y1 - 1, true));
                            }
                            else
                            {
                                list.Add(ManeuversMoveToTopBottom(x0, y, y0, false));
                                list.Add(ManeuversMoveToLeftRight(x, y, x1, false));
                                list.Add(ManeuversMoveToTopBottom(x, y, y1, true));
                                if (width > 2)
                                    list.Add(ManeuversMoveToLeftRight(x, y1, x1 - 1, true));
                            }
                }
                else
                {
                    //FALSE
                    if ((y0 == y) && (x0 != x) && (x0 != x1))
                    {
                        list.Add(ManeuversMoveToLeftRight(x0, y, x1, true));
                        list.Add(ManeuversMoveToTopBottom(x1, y, y1, true));
                        list.Add(ManeuversMoveToLeftRight(x, y1, x1, false));
                        list.Add(ManeuversMoveToTopBottom(x, y, y1, false));
                        if (x0 != x + 1)
                            list.Add(ManeuversMoveToLeftRight(x, y0, x0 - 1, true));
                        
                        rotate = true;
                    }

                    if ((y0 == y1) && (x0 != x) && (x0 != x1))
                    {
                        list.Add(ManeuversMoveToLeftRight(x, y1, x0, false));
                        list.Add(ManeuversMoveToTopBottom(x, y, y1, false));
                        list.Add(ManeuversMoveToLeftRight(x, y, x1, true));
                        list.Add(ManeuversMoveToTopBottom(x1, y, y1, true));
                        if (x0 != x1 - 1)
                            list.Add(ManeuversMoveToLeftRight(x0 + 1, y1, x1, false));

                        rotate = true;
                    }

                    if ((x0 == x) && (y0 != y) && (y0 != y1))
                    {
                        list.Add(ManeuversMoveToTopBottom(x, y, y0, false));
                        list.Add(ManeuversMoveToLeftRight(x, y, x1, true));
                        list.Add(ManeuversMoveToTopBottom(x1, y, y1, true));
                        list.Add(ManeuversMoveToLeftRight(x, y1, x1, false));
                        if (y0 != y1 - 1)
                            list.Add(ManeuversMoveToTopBottom(x, y0 + 1, y1, false));

                        rotate = true;
                    }

                    if ((x0 == x1) && (y0 != y) && (y0 != y1))
                    {
                        list.Add(ManeuversMoveToTopBottom(x1, y0, y1, true));
                        list.Add(ManeuversMoveToLeftRight(x, y1, x1, false));
                        list.Add(ManeuversMoveToTopBottom(x, y, y1, false));
                        list.Add(ManeuversMoveToLeftRight(x, y, x1, true));
                        if (y0 != y + 1)
                            list.Add(ManeuversMoveToTopBottom(x0, y, y0 - 1, true));

                        rotate = true;
                    }

                    if (!rotate)
                        if (y0 == y)
                            if (x0 == x)
                            {
                                list.Add(ManeuversMoveToLeftRight(x, y, x1, true));
                                list.Add(ManeuversMoveToTopBottom(x1, y, y1, true));
                                list.Add(ManeuversMoveToLeftRight(x, y1, x1, false));
                                if (height > 2)
                                    list.Add(ManeuversMoveToTopBottom(x, y0 + 1, y1, false));
                            }
                            else
                            {
                                list.Add(ManeuversMoveToTopBottom(x1, y, y1, true));
                                list.Add(ManeuversMoveToLeftRight(x, y1, x1, false));
                                list.Add(ManeuversMoveToTopBottom(x, y, y1, false));
                                if (width > 2)
                                    list.Add(ManeuversMoveToLeftRight(x, y, x0 - 1, true));
                            }
                        else
                            if (x0 == x)
                            {
                                list.Add(ManeuversMoveToTopBottom(x, y, y1, false));
                                list.Add(ManeuversMoveToLeftRight(x, y, x1, true));
                                list.Add(ManeuversMoveToTopBottom(x1, y, y1, true));
                                if (width > 2)
                                    list.Add(ManeuversMoveToLeftRight(x0 + 1, y1, x1, false));
                            }
                            else
                            {
                                list.Add(ManeuversMoveToLeftRight(x, y1, x1, false));
                                list.Add(ManeuversMoveToTopBottom(x, y, y1, false));
                                list.Add(ManeuversMoveToLeftRight(x, y, x1, true));
                                if (height > 2)
                                    list.Add(ManeuversMoveToTopBottom(x1, y, y0 - 1, true));
                            }
                }
            }

            return list;
        }

        public static BaseProgressItem ManeuversMoveToLeftRight(int x, int y, int x1, bool left)
        {
            if (left)
                return new BaseProgressItem(new PointCell(x1, y), new DirectionVector(DirectionCell.Left, Math.Abs(x1 - x)));
            else
                return new BaseProgressItem(new PointCell(x, y), new DirectionVector(DirectionCell.Right, Math.Abs(x1 - x)));
        }

        public static BaseProgressItem ManeuversMoveToTopBottom(int x, int y, int y1, bool top)
        {
            if (top)
                return new BaseProgressItem(new PointCell(x, y1), new DirectionVector(DirectionCell.Top, Math.Abs(y1 - y)));
            else
                return new BaseProgressItem(new PointCell(x, y), new DirectionVector(DirectionCell.Bottom, Math.Abs(y1 - y)));
        }

        #endregion

        #region System override

        public override string ToString()
        {
            string res = "";

            for (int i = 0; i < WidthPuzzle; i++)
            {
                for (int j = 0; j < WidthPuzzle; j++)
                    res += String.Format("{0}\t", Matrix[i][j]);
                res += Environment.NewLine;
            }

            return res;
        }

        #endregion
    }
}