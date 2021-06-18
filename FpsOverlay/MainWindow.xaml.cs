using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FpsOverlay.lib;
using FpsOverlay.Lib.Data;
using FpsOverlay.Lib.Features;
using FpsOverlay.Lib.Gfx;
using static FpsOverlay.lib.GameSettings;

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
  
            // your Legacy code
            GameProcess = new GameProcess(GetGameSettings());
            GameData = new GameData(GameProcess);
            WindowOverlay = new WindowOverlay(GameProcess);
            WindowOverlay.MustBeCanceled += (s, e) => ctx.Cancel();
            Graphics = new Graphics(WindowOverlay, GameProcess, GameData);


            ctx = new CancellationTokenSource();
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

        private GameSettings GetGameSettings()
        {
            var whMode = WallHackModes.Disable;
            if (ChkSkeletonWh?.IsChecked ?? false) whMode = WallHackModes.Skeleton;
            if (ChkHitBoxesWh?.IsChecked ?? false) whMode = WallHackModes.HitBoxes;

            return new GameSettings()
            {
                WallHackMode = whMode,
                ShowFps = ChkShowFps?.IsChecked ?? false,
                ShowAimCrossHair = ChkShowAimCrossHair?.IsChecked ?? false,
                ShowOverlayBorder = ChkShowBorder?.IsChecked ?? false,
                BorderColor = System.Drawing.Color.Green,
                CtWallHackColor = System.Drawing.Color.FromArgb(100, 0, 178, 255),
                TrWallHackColor = System.Drawing.Color.FromArgb(100, 255, 189, 0),
                AimSetting = new AimSettings()
                {
                    Fov = Convert.ToSingle(sldFov.Value),
                    Smoothness = Convert.ToInt16(sldSmoothness.Value),
                    BoneId = Convert.ToInt16(txtBoneId.Text)
                }
        };
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
            Cleanup();
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

        private void btnResetAimBotConfig_Click(object sender, RoutedEventArgs e)
        {
            sldFov.Value = 7f;
            sldSmoothness.Value = 3;
            txtBoneId.Text = "8";
        }
    }
}