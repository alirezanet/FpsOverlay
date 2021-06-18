using System;
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
            if (ChkSkeletonWh?.IsChecked ?? false) whMode = Graphics.WhMode.Skeleton;
            if (ChkHitBoxesWh?.IsChecked ?? false) whMode = Graphics.WhMode.HitBoxes;

            ctx = new CancellationTokenSource();
            // your Legacy code
            GameProcess = new GameProcess();
            GameData = new GameData(GameProcess);
            WindowOverlay = new WindowOverlay(GameProcess);

            WindowOverlay.MustBeCanceled += (s,e) =>  ctx.Cancel();
            
            Graphics = new Graphics(WindowOverlay, GameProcess, GameData,
                ChkShowFps?.IsChecked ?? false,
                ChkShowAimCrossHair?.IsChecked ?? false,
                whMode,
                ChkShowBorder?.IsChecked ?? false);
            
            var token = ctx.Token;
 
            GameProcess.Start(token);
            GameData.Start(token);
            WindowOverlay.Start(token);
            Graphics.Start(token);

            if (ChkAimAssist?.IsChecked ?? false)
            {
                AimBot = new AimBot(GameProcess, GameData);
                AimBot.Start(token);
            }
                
            if (ChkTriggerBot?.IsChecked ?? false)
            {
                TriggerBot = new TriggerBot(GameProcess, GameData);
                TriggerBot.Start(token);
            }

        }

        private CancellationTokenSource ctx;


        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            BtnStart.IsEnabled = false;
            BtnStop.IsEnabled = true;
            Sp1.IsEnabled = false;
            Start();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            BtnStop.IsEnabled = false;
            BtnStart.IsEnabled = true;
            Sp1.IsEnabled = true;
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