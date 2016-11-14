using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace NRTowerDefense
{
    public class LevelTowerDecorator : TowerDecorator
    {
        private int lastLevel = -1;
        private Label level;
        private IUpgradeableTower upgradeableKernel;

        private static string towerLevelToStringMapping( int i )
        {
            var mapping = new string[] { "-", "1", "2", "3", "4", "5", "6", "7", "8" };
            //var mapping = new string[] { "-", "α", "β", "γ", "δ", "λ", "μ", "π", "ρ" };
            //var mapping = new string[] { "-", "I", "II", "III", "IV", "V", "VI", "VII", "VIII" };
            //var mapping = new string[] { "-", "○", "□", "∆", "×","≡" };
            return mapping[ i ];
        }

        private static Color towerToColorMapping(Tower t)
        {
            if (t is BombTower)
                return Colors.White;
            else if (t is SimpleTower)
                return Colors.White;
            else
                return Colors.Black;
        }

        private static Color extrapolation(Color from, Color to, int level, int maxLevel)
        {
            double progress = (level - 1.0) / (maxLevel - 1.0);

            return Color.FromArgb
                (
                    (byte)(from.A * (1.0 - progress) + to.A * progress),
                    (byte)(from.R * (1.0 - progress) + to.R * progress),
                    (byte)(from.G * (1.0 - progress) + to.G * progress),
                    (byte)(from.B * (1.0 - progress) + to.B * progress)
                );
        }

        private void createLevel()
        {
            level = new Label();            
            level.IsHitTestVisible = false;
            level.FontFamily = new FontFamily("Calibri");
            level.FontSize = 10.0;            
            level.Width = level.Height = 2 * Radius.ToPixels();
            level.VerticalContentAlignment = VerticalAlignment.Center;
            level.HorizontalContentAlignment = HorizontalAlignment.Center;

            TextOptions.SetTextFormattingMode(level, TextFormattingMode.Display);
            Panel.SetZIndex(level, GameProperties.TowersLevelZIndex);
            Canvas.SetLeft(level, Position.X - Radius.ToPixels());
            Canvas.SetTop(level, Position.Y - Radius.ToPixels());
        }

        private void updateLevel()
        {
            if ( upgradeableKernel != null && upgradeableKernel.Level != lastLevel)
            {
                level.Foreground = new SolidColorBrush(extrapolation(towerToColorMapping(Kernel), Colors.Red, upgradeableKernel.Level, upgradeableKernel.MaxLevel));
                level.Content = towerLevelToStringMapping(upgradeableKernel.Level);
                lastLevel = upgradeableKernel.Level;
            }
        }

        public LevelTowerDecorator(Tower component, bool enable = true)
            : base(component)
        {
            Enabled = enable;

            upgradeableKernel = component as IUpgradeableTower;

            createLevel();

            Game.Field.ConnectedCanvas.Children.Add(level);
        }

        public LevelTowerDecorator(TowerDecorator component, bool enable = true)
            : base(component)
        {
            Enabled = enable;

            upgradeableKernel = component.Kernel as IUpgradeableTower;

            createLevel();

            Game.Field.ConnectedCanvas.Children.Add(level);
        }

        public override void Tick(double interval)
        {
            base.Tick(interval);

            updateLevel();
        }
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if (Enabled)
            {
                if (level.Visibility != Visibility.Visible)
                    level.Visibility = Visibility.Visible;
            }
            else
            {
                if (level.Visibility == Visibility.Visible)
                    level.Visibility = Visibility.Hidden;
            }
        }
        public override void ClearFromCanvas(Canvas canvas)
        {
            base.ClearFromCanvas(canvas);

            Game.Field.ConnectedCanvas.Children.Remove(level);
        }
    }
}
