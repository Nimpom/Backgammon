using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Backgammon.Logic;
using System.Windows.Media.Effects;
using Backgammon.Graphics;
using Backgammon.DataLayer;
using Backgammon.Logic.Entities.Fields;
using System.Diagnostics;
using Backgammon.Logic.Entities;

namespace Backgammon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameEngine gameEngine;
        Grid selectionGrid1, selectionGrid2;
        Move move = new Move();

        public MainWindow()
        {
            String saveFile = "saveFile.xml";
            DataStorage dataStorage = new DataStorage(saveFile);

            // Set player colors
            String p1Color = "White";
            String p2Color = "Black";

            // Create Players
            Player player1 = new Player(1, "Player1", p1Color);
            Player player2 = new Player(2, "Player2", p2Color);

            // Create checkers
            Checker[] P1Checkers = new Checker[15];
            for (int i = 0; i < 15; i++)
            {
                P1Checkers[i] = new Checker(i, player1);
            }
            Checker[] P2Checkers = new Checker[15];
            for (int i = 0; i < 15; i++)
            {
                P2Checkers[i] = new Checker(i + 15, player2);
            }

            // Create dices
            Dice D1 = new Dice();
            Dice D2 = new Dice();

            // Create diceCup
            DiceCup diceCup = new DiceCup(D1, D2);

            // Create board fields
            BoardField[] boardFields = new BoardField[24];
            for (int i = 0; i < 24; i++)
            {
                boardFields[i] = new BoardField(new LinkedList<Checker>(), i);

            }

            // Create elimination field
            EliminatedField eliminatedField = new EliminatedField();

            // Create goal fields
            GoalField goalFieldP1 = new GoalField(player1, 27);
            GoalField goalFieldP2 = new GoalField(player2, 26);

            // Create rules
            Rules rules = new Rules(boardFields, eliminatedField, goalFieldP1, goalFieldP2, diceCup, player1, player2);

            // Create game board
            GameBoard gameBoard = new GameBoard(boardFields, eliminatedField, goalFieldP1, goalFieldP2, P1Checkers, P2Checkers, diceCup, rules, player1, player2);

            // selection -1 = No selected field
            move.from = -1;
            move.to = -1;

            gameEngine = new GameEngine(dataStorage, gameBoard);

            InitializeComponent();

            // If a saved game exist show the continue button
            if (gameEngine.saveGameExist())
            {
                continueButton.Visibility = Visibility.Visible;
            }
            else
            {
                continueButton.Visibility = Visibility.Hidden;
            }
    
        }

        // Starts a new game
        private void startGame(object sender, RoutedEventArgs e)
        {
            // Checkers in start position
            // Boardfield     0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 -  E  G2 G1
            int[] p1Array = { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 3, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0  };
            int[] p2Array = { 0, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0  };
            //int[] p1Array = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 2, 5, 5, 0, 0, 0, 0 };
            //int[] p2Array = { 0, 5, 5, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            // Place checkers at start position
            gameEngine.initStartState(p1Array, p2Array);

            gameEngine.startGame();

            exceptionMessage.Text = "";
            dice1Image.Source = null;
            dice2Image.Source = null;
            startGrid.Visibility = Visibility.Hidden;
            printBoard();
            diceButton.IsEnabled = true;
            
        }

        // Continues a previous game
        private void continueGame(object sender, RoutedEventArgs e)
        {
            
            try
            {

                gameEngine.continueGame();
                startGrid.Visibility = Visibility.Hidden;

                if (!gameEngine.anyMoreMoves())
                {
                    diceButton.IsEnabled = true;
                }
                printBoard();
                displayDices();

            } catch (NoFileException exception)
            {
                errorMessage.Text = "No file was found!\nTry to create a new game";
                Debug.WriteLine(exception.Message);
            }
            
            
        }

        // Prints the checkers on the board
        public void printBoard()
        {
           
            displayActivePlayer();

            // Loop through each boardField
            for (int i = 0; i <= 23; i++)
            {
                String stackPanelName = "f" + i;
                StackPanel panel = (StackPanel)this.FindName(stackPanelName);
                panel.Children.Clear();

                // Draw each checker in field
                for (int j = 0; j < gameEngine.getNumberOfCheckersInField(i); j++)
                {
                    // If bottom half
                    if (i > 11)
                    {
                        panel.VerticalAlignment = VerticalAlignment.Bottom;
                    }

                    Color color = (Color)ColorConverter.ConvertFromString(gameEngine.getColor(i));
                    CheckerGraphics checker = new CheckerGraphics(30, color);
                    
                    // Adjust margin if more than 5 chekers in stackpanel
                    Thickness margin = new Thickness();
                    if (gameEngine.getNumberOfCheckersInField(i) > 5)
                    {
                        if (j > 0)
                        {
                            double checkerHeight = checker.checker.Height;
                            int numOfCheckers = gameEngine.getNumberOfCheckersInField(i);
                            double x = ((numOfCheckers - 5) * checkerHeight) / numOfCheckers;
                            margin = new Thickness(0, -x, 0, 0);
                        }
                    }

                    checker.checker.Margin = margin;
                    panel.Children.Add(checker.checker);
                }
            }

            //Print eliminated chekckers in eliminated field
            eliminatedField.Children.Clear();
            for (int i = 0; i <gameEngine.getNumberOfEliminatedCheckers(); i++)
           
            {
                Color color = (Color)ColorConverter.ConvertFromString(gameEngine.getEliminatedCheckerColor(i));
                CheckerGraphics checker = new CheckerGraphics(30, color);
                eliminatedField.Children.Add(checker.checker);
            }

            //Print checkers in goal field 1
            f27.Children.Clear();
            for (int i = 0; i < gameEngine.getNumberOfChekersInGoalFieldP1(); i++)
            {
                Color color = (Color)ColorConverter.ConvertFromString(gameEngine.getGoalColorP1());
                CheckerGoalGraphics checker = new CheckerGoalGraphics(30, color);
                f27.Children.Add(checker.checker);
            }

            //Print checkers in goal field 2
            f26.Children.Clear();
            for (int i = 0; i < gameEngine.getNumberOfChekersInGoalFieldP2(); i++)
            {
                Color color = (Color)ColorConverter.ConvertFromString(gameEngine.getGoalColorP2());
                CheckerGoalGraphics checker = new CheckerGoalGraphics(30, color);
                f26.Children.Add(checker.checker);
            }

            //Print player info
            p1Name.Text = gameEngine.getPlayer1Name();
            p2Name.Text = gameEngine.getPlayer2Name();
            SolidColorBrush b = new SolidColorBrush();
            b.Color = (Color)ColorConverter.ConvertFromString(gameEngine.player1Color());
            p1Checker.Fill = b;

            SolidColorBrush b1 = new SolidColorBrush();
            b1.Color = (Color)ColorConverter.ConvertFromString(gameEngine.player2Color());
            p2Checker.Fill = b1;
        }

        // Rolls the dices
        private void RollDices(object sender, RoutedEventArgs e)
        {
            exceptionMessage.Text = "";
            gameEngine.rollDices();
            displayDices();
            diceButton.IsEnabled = false;

            if (!gameEngine.anyMoreMoves())
            {
                exceptionMessage.Text = "There was no valid moves for " + gameEngine.getNonActivePlayerName();
                diceButton.IsEnabled = true;
              
                displayActivePlayer();
            }
        }

        // Displays dice values on the board
        public void displayDices()
        {
            List<int> moves = gameEngine.getDices();

            if(moves.Count > 0)
            {
                BitmapImage Img = new BitmapImage(new Uri(@"Graphics\DiceFaces\dice" + moves.ElementAt(0) + ".png", UriKind.Relative));
                dice1Image.Source = Img;
            }
            else
            {
                dice1Image.Source = null;
            }
            if (moves.Count > 1)
            {
                BitmapImage Img = new BitmapImage(new Uri(@"Graphics\DiceFaces\dice" + moves.ElementAt(1) + ".png", UriKind.Relative));
                dice2Image.Source = Img;
            }
            else
            {
                dice2Image.Source = null;
            }
            
        }

        // Highlights possible moves from selected position
        public bool highlightPossibleMoves(int index)
        {
            List<int> list = gameEngine.getPossibleMoveFromPos(index);

            for (int i = 0; i <list.Count; i++)
            {
                StackPanel panel = (StackPanel) this.FindName("f" + list.ElementAt(i));
                Grid grid = (Grid) panel.Parent;
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.SandyBrown;
                brush.Opacity = 50;
                grid.Background = brush;
            }
            
            if (list.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        // Removes highlights
        public void removeHighlight(int index)
        {
            List<int> list = gameEngine.getPossibleMoveFromPos(index);

            for (int i = 0; i < list.Count; i++)
            {
                StackPanel panel = (StackPanel)this.FindName("f" + list.ElementAt(i));
                Grid grid = (Grid)panel.Parent;
                grid.Background = null;  
            }
        }

        // Show on board which player that is active
        public void displayActivePlayer()
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Colors.SandyBrown;
            brush.Opacity = 50;
            if (gameEngine.getActivePlayersName().Equals(gameEngine.getPlayer1Name()))
            {
                P1Info.Background = brush;
                P2Info.Background = null;
                
            } else
            {
                P1Info.Background = null;
                P2Info.Background = brush;
            }
        }

        // When a grid is clicked
        private void gridClick(object sender, MouseButtonEventArgs e)
        {
            exceptionMessage.Text = "";
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            //Bubbles up in the elements hierarchy to find the grid
            while ((dep != null) && !(dep is Grid)){
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null)
            {
                return;
            }

            // if object is a grid and startgridd is hidden
            if (dep is Grid && !dep.Equals(exceptionOutput) && (startGrid.Visibility == Visibility.Hidden))
            {
                DropShadowEffect effect = new DropShadowEffect();
                effect.Color = Colors.Brown;
                effect.BlurRadius = 7;
                effect.Direction = 380;
                
                Grid pressedGrid = dep as Grid;

                int selection = int.Parse(pressedGrid.Tag.ToString());

                if ((move.from >= 0) && (selection == move.from))
                {
                    // Reset selection 1
                    pressedGrid.Effect = null;
                    removeHighlight(move.from);
                    move.from = -1;
                }
                // First selection and not any goal field
                else if((move.from < 0) && ((selection >= 0) && (selection != 26) && (selection != 27)))
                {
                    // Make selection 1
                    if (highlightPossibleMoves(selection))
                    {
                        pressedGrid.Effect = effect;
                        move.from = selection;
                        selectionGrid1 = pressedGrid;
                    }
                }
                // Make selection 2
                else if ((move.from >= 0) && (move.to < 0))
                {
                    move.to = selection;
                    selectionGrid2 = pressedGrid;
                }
                // If selection 1 & 2, make move
                if((move.from >= 0) && (move.to >= 0))
                {
                    removeHighlight(move.from);
                    try
                    {
                        gameEngine.makeMove(move);
                    }
                    catch(Backgammon.Logic.Exceptions.NoValidMoveException info)
                    {
                        // Outputs exception message
                        exceptionMessage.Text = info.Message;
                    }

                    // If the player has no more valid moves
                    if (!gameEngine.anyMoreMoves())
                    {
                        if (gameEngine.getDices().Count >= 1)
                        {
                            exceptionMessage.Text = "No more valid moves. Switching to " + gameEngine.getActivePlayersName();
                        }
                        
                        diceButton.IsEnabled = true;
                    }
                    // If the player winns
                    if (gameEngine.hasAWinner())
                    {
                        startGrid.Visibility = Visibility.Visible;
                        continueButton.Visibility = Visibility.Hidden;
                        winnerText.Text = gameEngine.returnWinner() + " won the game!";
                        exceptionMessage.Text = "";
                    }

                    // Reset selections
                    selectionGrid1.Effect = null;
                    printBoard();
                    move.from = -1;
                    move.to = -1;
                }

                displayDices();
            }
        }
    }
}
