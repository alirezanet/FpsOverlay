using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FpsOverlay.lib;
using FpsOverlay.Lib.Data;
using FpsOverlay.Lib.Features;
using FpsOverlay.Lib.Gfx;
using static FpsOverlay.lib.GameSettings;
using Graphics = FpsOverlay.Lib.Gfx.Graphics;

namespace FpsOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private CancellationTokenSource ctx;

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Exit += (sender, args) => Dispose();
        }

        private GameProcess GameProcess { get; set; }


        private GameData GameData { get; set; }


        private WindowOverlay WindowOverlay { get; set; }


        private Graphics Graphics { get; set; }


        private TriggerBot TriggerBot { get; set; }

        private AimBot AimBot { get; set; }

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

            return new GameSettings
            {
                WallHackMode = whMode,
                ShowFps = ChkShowFps?.IsChecked ?? false,
                ShowAimCrossHair = ChkShowAimCrossHair?.IsChecked ?? false,
                ShowOverlayBorder = ChkShowBorder?.IsChecked ?? false,
                BorderColor = Color.Green,
                CtWallHackColor = Color.FromArgb(100, 0, 178, 255),
                TrWallHackColor = Color.FromArgb(100, 255, 189, 0),
                AimSetting = new AimSettings
                {
                    Fov = Convert.ToSingle(sldFov.Value),
                    Smoothness = Convert.ToInt16(sldSmoothness.Value),
                    BoneId = Convert.ToInt16(txtBoneId.Text)
                }
            };
        }


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
            sldFov.Value = 4f;
            sldSmoothness.Value = 5;
            txtBoneId.Text = "7";
        }

        private async void BtnCheckForUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            // stop if its running
            BtnCheckForUpdate.IsEnabled = false;
            try
            {
                var result = await DownloadNewOffsets();
                var newOffsets = GetNewOffsetContent(result);
                SaveNewOffsets(newOffsets);
                MessageBox.Show("Offsets Updated to the latest version ;)");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Can not update", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnCheckForUpdate.IsEnabled = true;
            }
        }

        private void SaveNewOffsets(string newOffsets)
        {
            // backup old offsets  
            File.Move("offsets.txt", "offsets.txt.backup");
            File.Delete("offsets.txt");
            File.WriteAllText("offsets.txt", newOffsets);
        }

        private List<string> OffsetNames()
        {
            return new List<string>
            {
                "dwClientState",
                "dwClientState_ViewAngles",
                "dwEntityList",
                "dwLocalPlayer",
                "dwViewMatrix",
                "m_aimPunchAngle",
                "m_bDormant",
                "m_dwBoneMatrix",
                "m_iFOVStart",
                "m_iHealth",
                "m_iTeamNum",
                "m_lifeState",
                "m_pStudioHdr",
                "m_vecOrigin",
                "m_vecViewOffset"
            };
        }

// https://github.com/frk1/hazedumper/blob/master/csgo.cs
        private string GetNewOffsetContent(string result)
        {
            var offsets = OffsetNames();
            var content = new StringBuilder();
            var counter = 0;
            foreach (var line in result.Split(new[] {Environment.NewLine, "\n", "\"r"}, StringSplitOptions.None))
            {
                var regex = new Regex(@"(public\sconst\sInt32\s*)(\w*)(\s*=\s*\w+)");
                var match = regex.Match(line);
                if (!match.Success) continue;
                var offset = match.Groups[2].Value;
                if (!offsets.Contains(offset)) continue;

                content.AppendLine(match.Groups[2].Value + match.Groups[3].Value);
                counter++;
                offsets.Remove(offset);
            }

            if (counter != 15)
                throw new Exception("Auto update failed. " + string.Join(",", offsets) + " not found.");
            return content.ToString();
        }

        private static async Task<string> DownloadNewOffsets()
        {
            var client = new WebClient();
            var uri = new Uri("https://raw.githubusercontent.com/frk1/hazedumper/master/csgo.cs");
            var result = await client.DownloadStringTaskAsync(uri);
            return result;
        }
    }
}