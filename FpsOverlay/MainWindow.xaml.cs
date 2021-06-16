using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FpsOverlay.Lib.Data;
using FpsOverlay.Lib.Features;
using FpsOverlay.Lib.Gfx;

namespace FpsOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private GameProcess GameProcess { get; set; }


        private GameData GameData { get; set; }


        private WindowOverlay WindowOverlay { get; set; }


        private Graphics Graphics { get; set; }


        private TriggerBot TriggerBot { get; set; }

        private AimBot AimBot { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Exit += (sender, args) => Dispose();
        }


        private void Start()
        {
            var whMode = Graphics.WhMode.Disable;
            if (chkSkeletonWh?.IsChecked ?? false) whMode = Graphics.WhMode.Skeleton;
            if (chkHitboxesWh?.IsChecked ?? false) whMode = Graphics.WhMode.Hitboxes;

            ctx = new CancellationTokenSource();
            // your Legacy code
            GameProcess = new GameProcess();
            GameData = new GameData(GameProcess);
            WindowOverlay = new WindowOverlay(GameProcess);
            Graphics = new Graphics(WindowOverlay, GameProcess, GameData,
                chkShowFps?.IsChecked ?? false,
                chkShowAimCrossHair?.IsChecked ?? false,
                whMode,
                chkShowBorder?.IsChecked ?? false);

            Run();
        }

        private CancellationTokenSource ctx;

        private void Run()
        {
            var token = ctx.Token;
            //TriggerBot = new TriggerBot(GameProcess, GameData);
            //AimBot = new AimBot(GameProcess, GameData);
            GameProcess.Start(token);
            GameData.Start(token);
            WindowOverlay.Start(token);
            Graphics.Start(token);
            //TriggerBot.Start();
            //AimBot.Start(); 
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            Start();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            btnStop.IsEnabled = false;
            btnStart.IsEnabled = true;
            ctx.Cancel();
            Cleanup();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            // close the app
            Application.Current.Shutdown();
        }

        private void Cleanup()
        {
            AimBot?.Dispose();
            AimBot = default;

            TriggerBot?.Dispose();
            TriggerBot = default;

            Graphics?.Dispose();
            Graphics = default;

            WindowOverlay?.Dispose();
            WindowOverlay = default;

            GameData?.Dispose();
            GameData = default;

            GameProcess?.Dispose();
            GameProcess = default;
        }

        private void Dispose()
        {
            Cleanup();
        }
    }
}