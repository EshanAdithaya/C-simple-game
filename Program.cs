using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace SimpleGame
{
    public class Game : Form
    {
        private PictureBox squirrel;
        private Label scoreLabel;
        private Label gameOverLabel;
        private System.Windows.Forms.Timer gameTimer;
        private Random random = new Random();
        private int score = 0;
        private double currentShowTime = 2000; // Start with 2 seconds
        private double currentHideTime = 1000;
        private bool isSquirrelVisible = false;
        private bool isGameOver = false;
        private long squirrelAppearTime;  // Track when squirrel appears

        public Game()
        {
            // Form settings
            this.Size = new Size(800, 600);
            this.Text = "Whack-a-Squirrel!";
            this.BackColor = Color.ForestGreen;

            // Create score label
            scoreLabel = new Label
            {
                Location = new Point(10, 10),
                Size = new Size(200, 30),
                Text = "Score: 0",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.White
            };
            this.Controls.Add(scoreLabel);

            // Create game over label (initially hidden)
            gameOverLabel = new Label
            {
                Location = new Point(300, 250),
                Size = new Size(200, 100),
                Text = "Game Over!\nFinal Score: 0\n\nPress R to restart",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            this.Controls.Add(gameOverLabel);

            // Create squirrel
            squirrel = new PictureBox
            {
                Size = new Size(80, 80),
                BackColor = Color.Transparent,
                Image = CreateSquirrelImage(),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            squirrel.Click += Squirrel_Click;
            this.Controls.Add(squirrel);

            // Create timer
            gameTimer = new System.Windows.Forms.Timer
            {
                Interval = 16 // About 60 FPS for smooth updates
            };
            gameTimer.Tick += GameTimer_Tick;

            // Start the game
            this.KeyPress += Game_KeyPress;
            StartGame();
        }

        private void StartGame()
        {
            score = 0;
            currentShowTime = 2000;
            currentHideTime = 1000;
            isGameOver = false;
            gameOverLabel.Visible = false;
            scoreLabel.Text = "Score: 0";
            gameTimer.Start();
            ShowNextSquirrel();
        }

        private void ShowNextSquirrel()
        {
            // Wait for hide time before showing next squirrel
            System.Threading.Thread.Sleep((int)currentHideTime);
            ShowSquirrel();
        }

        private void Game_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (isGameOver && char.ToUpper(e.KeyChar) == 'R')
            {
                StartGame();
            }
        }

        private Image CreateSquirrelImage()
        {
            Bitmap bmp = new Bitmap(80, 80);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                
                // Body
                g.FillEllipse(Brushes.Brown, 20, 20, 40, 40);
                
                // Tail
                g.FillEllipse(Brushes.Brown, 45, 10, 25, 50);
                
                // Ear
                g.FillEllipse(Brushes.Brown, 15, 15, 15, 15);
                
                // Eye
                g.FillEllipse(Brushes.Black, 25, 30, 5, 5);
            }
            return bmp;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!isGameOver && isSquirrelVisible)
            {
                // Check if squirrel has been visible for longer than currentShowTime
                if (Environment.TickCount - squirrelAppearTime > currentShowTime)
                {
                    // Player missed the squirrel
                    GameOver();
                }
            }
        }

        private void ShowSquirrel()
        {
            int maxX = this.ClientSize.Width - squirrel.Width;
            int maxY = this.ClientSize.Height - squirrel.Height;
            squirrel.Location = new Point(
                random.Next(maxX),
                random.Next(maxY)
            );
            squirrel.Visible = true;
            isSquirrelVisible = true;
            squirrelAppearTime = Environment.TickCount;
        }

        private void HideSquirrel()
        {
            squirrel.Visible = false;
            isSquirrelVisible = false;
        }

        private void Squirrel_Click(object sender, EventArgs e)
        {
            if (!isGameOver && isSquirrelVisible)
            {
                score++;
                scoreLabel.Text = $"Score: {score}";
                
                // Increase difficulty
                currentShowTime = Math.Max(500, currentShowTime * 0.95); // Minimum 0.5 seconds show time
                currentHideTime = Math.Max(200, currentHideTime * 0.95); // Minimum 0.2 seconds hide time
                
                HideSquirrel();
                ShowNextSquirrel();
            }
        }

        private void GameOver()
        {
            isGameOver = true;
            gameTimer.Stop();
            HideSquirrel();
            gameOverLabel.Text = $"Game Over!\nFinal Score: {score}\n\nPress R to restart";
            gameOverLabel.Visible = true;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new Game());
        }
    }
}