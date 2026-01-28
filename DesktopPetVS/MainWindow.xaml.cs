using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace DesktopPetVS
{
    public partial class MainWindow : Window
    {
        // Importy z Windows systému (Win32 API)
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TRANSPARENT = 0x00000020;

        double groundY = SystemParameters.WorkArea.Bottom;
        //double velocityY = 0;
        //double velocityX = 2;
        const double gravity = 0.1;
        //bool isGrounded = false;
        DispatcherTimer timer;
        int direction = 1;
        double leftLimit = 0;
        double rightLimit = SystemParameters.WorkArea.Width - 128;

        Stopwatch stopwatch = new Stopwatch();
        double lastTime = 0;

        private readonly Random rnd = new Random();

        public int PetCount => spawnedPets.Count;

        public MainWindow()
        {
            InitializeComponent();

            Left = 0;
            Top = 0;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Update;
            timer.Start();

            stopwatch.Start();
            lastTime = stopwatch.Elapsed.TotalSeconds;


        }

        void Update(object? sender, EventArgs e)
        {
            double currentTime = stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            double groundY = SystemParameters.WorkArea.Bottom;

            Point mouseScreen = MouseTracker.MousePosition; // screen coordinates
            Point mouseOnCanvas = PetCanvas.PointFromScreen(mouseScreen);
            bool mouseOverPet = false;

            


            foreach (var pet in spawnedPets)
            {
                if (dragging && pet.Sprite == draggedPet)
                {
                    pet.VelocityY = 0; // Resetuj pádovú rýchlosť, aby po pustení neletel k zemi ako kameň
                    continue;
                }
                pet.StateTime -= deltaTime;

                if (pet.StateTime <= 0)
                {
                    if (pet.State == PetState.Idle)
                    {
                        pet.State = PetState.Walk;
                        pet.StateTime = rnd.NextDouble() * 4 + 2;
                        pet.Direction = rnd.Next(0, 2) == 0 ? 1 : -1;
                    }
                    else
                    {
                        pet.State = PetState.Idle;
                        pet.StateTime = rnd.NextDouble() * 2 + 1;
                    }
                }

                double petX = Canvas.GetLeft(pet.Sprite);
                double petY = Canvas.GetTop(pet.Sprite);
                double petWidth = pet.Sprite.Width;
                double petHeight = pet.Sprite.Height;

                if (pet.IsGrounded && pet.State == PetState.Walk)
                {
                    petX += pet.VelocityX * pet.Direction;

                    if (petX + petWidth >= SystemParameters.WorkArea.Width)
                    {
                        petX = SystemParameters.WorkArea.Width - petWidth;
                        pet.Direction *= -1;
                    }
                    else if (petX <= 0)
                    {
                        petX = 0;
                        pet.Direction *= -1;
                    }

                    Canvas.SetLeft(pet.Sprite, petX);
                }

                // gravitácia
                if (petY + petHeight < groundY)
                {
                    pet.VelocityY += gravity;
                    petY += pet.VelocityY;
                    pet.IsGrounded = false;
                }
                else
                {
                    petY = groundY - petHeight;
                    pet.VelocityY = 0;
                    pet.IsGrounded = true;
                }

                Rect petRect = new Rect(
                    Canvas.GetLeft(pet.Sprite),
                    Canvas.GetTop(pet.Sprite),
                    pet.Sprite.Width,
                    pet.Sprite.Height
                );

                if (petRect.Contains(mouseOnCanvas))
                {
                    mouseOverPet = true;
                }


                Canvas.SetTop(pet.Sprite, petY);
            }

            SetClickThrough(!mouseOverPet);


        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Získame adresu okna
            //var hwnd = new WindowInteropHelper(this).Handle;

            // Toto nastaví oknu vlastnosť, že ho myš "ignoruje" (click-through)
            //int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            //SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        private int petCounter = 0; // pre unikátne mená (ak treba)
        List<Pet> spawnedPets = new List<Pet>();

        private readonly string[] petSprites = new string[]
        {
            "pack://application:,,,/Assets/pet_idle_0.png",
            "pack://application:,,,/Assets/pet2_idle_0.png",
            "pack://application:,,,/Assets/pet3_idle_0.png",
        };


        public void SpawnPet(int typeOfPet)
        {
            //Random rnd = new Random();
            string selectedSprite;
            if (typeOfPet == 0)
            {
                selectedSprite = petSprites[rnd.Next(petSprites.Length)];
            }
            else if (typeOfPet == 1) 
            {
                selectedSprite = petSprites[typeOfPet - 1];
            }
            else if (typeOfPet == 2)
            {
                selectedSprite = petSprites[typeOfPet - 1];
            }
            else if (typeOfPet == 3)
            {
                selectedSprite = petSprites[typeOfPet - 1];
            }
            else
            {
                selectedSprite = petSprites[rnd.Next(petSprites.Length)];
            }


                Image newSprite = new Image
                {
                    Width = 73, // -20
                    Height = 93, // -40
                    Source = new BitmapImage(new Uri(selectedSprite))
                };

            newSprite.MouseLeftButtonDown += Pet_MouseDown;
            newSprite.MouseMove += Pet_MouseMove;
            newSprite.MouseLeftButtonUp += Pet_MouseUp;


            RenderOptions.SetBitmapScalingMode(newSprite, BitmapScalingMode.NearestNeighbor);

            double startX = rnd.Next(0, (int)(PetCanvas.ActualWidth - newSprite.Width));
            double startY = 0;

            Canvas.SetLeft(newSprite, startX);
            Canvas.SetTop(newSprite, startY);

            PetCanvas.Children.Add(newSprite);

            Pet newPet = new Pet
            {
                Id = petCounter++,
                Sprite = newSprite,
                VelocityX = rnd.Next(1, 4),
                Direction = rnd.Next(0, 2) == 0 ? 1 : -1,
                State = PetState.Idle,
                StateTime = rnd.NextDouble() * 2 + 1 // 1–4 sekundy
            };

            spawnedPets.Add(newPet);
        }

        public void SetClickThrough(bool enabled)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int style = GetWindowLong(hwnd, GWL_EXSTYLE);

            if (enabled)
            {
                SetWindowLong(hwnd, GWL_EXSTYLE, style | WS_EX_TRANSPARENT);
            }
            else
            {
                SetWindowLong(hwnd, GWL_EXSTYLE, style & ~WS_EX_TRANSPARENT);

            }
        }

        bool dragging = false;
        Image? draggedPet;
        Point dragOffset;

        void Pet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            draggedPet = (Image)sender;
            dragging = true;

            dragOffset = e.GetPosition(draggedPet);

            
            draggedPet.CaptureMouse();
        }

        void Pet_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dragging || draggedPet == null) return;

            Point pos = e.GetPosition(PetCanvas);
            Canvas.SetLeft(draggedPet, pos.X - dragOffset.X);
            Canvas.SetTop(draggedPet, pos.Y - dragOffset.Y);
        }

        void Pet_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragging = false;
            draggedPet?.ReleaseMouseCapture();
            draggedPet = null;
        }





        private void SpawnPet_Click(object sender, RoutedEventArgs e)
        {
            SpawnPet(0);
        }

        private void SpawnButton1_Click(object sender, RoutedEventArgs e)
        {
            SpawnPet(1);
        }


    }

    class Pet
    {
        public int Id { get; set; }
        public required Image Sprite { get; set; }
        public double VelocityY { get; set; } = 0;
        public double VelocityX { get; set; } = 2;
        public int Direction { get; set; } = 1;
        public bool IsGrounded { get; set; } = false;

        // stavový systém
        public PetState State { get; set; } = PetState.Walk;
        public double StateTime { get; set; } = 0; // v sekundách
    }

    enum PetState
    {
        Idle,
        Walk
    }

}