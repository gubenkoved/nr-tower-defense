using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Windows.Media.Animation;

namespace NRTowerDefense
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string lastTowerTypeClassName = "None";

        public MainWindow()
        {
            InitializeComponent();
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Game.Init(GameCanvas);
            Game.SetInformationControls(LifeLabel, MoneyLabel, ScoreLabel, TimeLabel, LengthLabel);
            
            Game.Timer.Tick += new GameTimer.TickEventHandler( (o,a) => gameFPSPresenterControl.Tick());
            
            Game.TowerSelected          += new Game.TowerHandler(ShowTowerInfo);
            Game.MonsterDied            += new Game.MonsterHandler( (a) => UpdateTowerUpgradePossibility());
            Game.MonsterSelected        += new Game.MonsterHandler(ShowMonsterInfo);
            Game.BlockDetected          += new Game.BlockDetectedHandler(BlockDetectedAlert);
            Game.GameOver               += new GameLevel.LevelHandler(GameOver);
            Game.GameStarted            += new Game.GameEventHandler(GameStarted);
            Game.CurrentLevelCompeted   += new GameLevel.LevelHandler(LevelCompleted);

            ShowTowerInfo(new TowerArg(null));
            AddTowerBtn_MouseLeave(sender, null);

            LevelDefinition.CreateLevels();
            gameMenuControl.SetLevelSource(LevelDefinition.MainGameLevelList);
            gameMenuControl.SetLevelPresenter(gameLevelPresenterControl);
            ShowGameMenu("Новая игра", string.Format("Здравствуй, {0}!", Environment.UserName), 0.0, 2.0);
        }

        void ShowGameMenu(object header, object message, double gameFadeOutTime = 2.0, double menuFadeInTime = 1.0 )
        {
            DoubleAnimation fadeOut = new DoubleAnimation(0.0, TimeSpan.FromSeconds(gameFadeOutTime));
            
            DoubleAnimation fadeIn = new DoubleAnimation(1.0, TimeSpan.FromSeconds(menuFadeInTime));
            fadeIn.BeginTime = TimeSpan.FromSeconds(gameFadeOutTime);

            GameCanvas.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            gameMenuControl.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            gameMenuControl.SetHeader(header);
            gameMenuControl.SetMessage(message);

            gameMenuControl.IsHitTestVisible = true;
        }

        void HideGameMenu(double menuFadeOutTime = 2.0, double gameFadeInTime = 1.0)
        {
            DoubleAnimation fadeOut = new DoubleAnimation(0.0, TimeSpan.FromSeconds(menuFadeOutTime));

            DoubleAnimation fadeIn = new DoubleAnimation(1.0, TimeSpan.FromSeconds(gameFadeInTime));
            fadeIn.BeginTime = TimeSpan.FromSeconds(menuFadeOutTime);

            GameCanvas.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            gameMenuControl.BeginAnimation(UIElement.OpacityProperty, fadeOut);

            gameMenuControl.IsHitTestVisible = false;
        }

        void GameOver(LevelArg arg)
        {
            ShowGameMenu("Игра окончена", "Вы набрали " + Game.Score + " очков");

            FixRecord(arg.Level);
        }

        void GameStarted()
        {
            HideGameMenu();
        }

        void LevelCompleted(LevelArg arg)
        {
            ShowGameMenu(string.Format("Ура! Вы прошли \"{0}\"", arg.Level.Name), "и набрали " + Game.Score + " очков");

            FixRecord(arg.Level);
        }

        void FixRecord(GameLevel level)
        {
            Game.UpdateInformationControls();

            try
            {
                NameAskerWindows nameAsker = new NameAskerWindows();
                GameRecord record = new GameRecord();
                
                nameAsker.ShowDialog();
                record.SetData(nameAsker.PlayerName, level.Name, Game.Score, DateTime.Now);

                GameRecords records = new GameRecords(GameRecords.DEFAULT_FILE_NAME);
                records.AddRecord(record);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ошибка сохранения результатов: " + exc.Message);
            }
            
        }

        void BlockDetectedAlert(BlockDetectedArg e)
        {
            AlertControl alert = new AlertControl();

            string text = "";

            switch (e.BlockTestResult)
            {
                case BlockTestResult.StartToTargetBlock:
                    text = "Блокировка! Нельзя полность закрывать путь монстрам";
                    break;
                case BlockTestResult.MonsterBlock:
                    text = "Блокировка! Некоторые монстры заперты";
                    break;
            }

            alert.ShowPopup(GameCanvas, text, 3.0);
        }

        void ShowMonsterInfo(MonsterArg e)
        {
            SelectedMonsterInformation.Monster = e.Monster;
        }

        void ShowTowerInfo(TowerArg e)
        {
            var tower = e.Tower;

            UpdateTowerInfo(tower);
            UdpateTowerUpgradeInfo(tower);
            UpdateTowerUpgradePossibility();
            UpdateSellLabel();
            UpdateExplodePossibility();
        }

        void FillTowerIncreaseInfo(double d)
        {
            foreach (GameParametrControl parametrControl in TowerParameters.Children)
            {
                parametrControl.Increment = d;
            }
        }

        void FillTowerInfo(double d)
        {
            foreach (GameParametrControl parametrControl in TowerParameters.Children)
            {
                parametrControl.Value = d;
            }
        }

        void UdpateTowerUpgradeInfo(Tower tower)
        {
            if (tower is IUpgradeableTower)
            {
                var uptower = tower as IUpgradeableTower;
                if (uptower.Level < uptower.MaxLevel)
                {
                    CostParametr.Increment = uptower.CostUpgradeScale[uptower.Level] - tower.Cost;
                    RadiusParametr.Increment = (uptower.AttackRadiusUpgradeScale[uptower.Level] - tower.AttackRadius).Amount;
                    PowerParametr.Increment = uptower.BulletDamageUpgradeScale[uptower.Level] - tower.Bullet.Damage;
                    ReloadingSpeedParametr.Increment = uptower.ReloadingSpeedUpgradeScale[uptower.Level] - tower.ReloadingSpeed;
                    BulletSpeedParametr.Increment = (uptower.BulletSpeedUpgradeScale[uptower.Level] - tower.Bullet.Speed).Amount;

                    if (tower is IUpgradeableTowerWithFreezeBullet && tower.Bullet is IFreezeBullet)
                    {
                        var ftower = tower as IUpgradeableTowerWithFreezeBullet;
                        var fbullet = tower.Bullet as IFreezeBullet;

                        FreezeTimeParametr.Increment = ftower.FreezeTimeUpgradeScale[uptower.Level] - fbullet.FreezeTime;
                        FreezeFactorParametr.Increment = ftower.FreezeFactorUpgradeScale[uptower.Level] - fbullet.FreezeFactor;
                    }
                }
                else
                {
                    FillTowerIncreaseInfo(double.PositiveInfinity);
                }
            }
            else
            {
                FillTowerIncreaseInfo(double.NaN);
            }
        }

        void UpdateTowerInfo(Tower tower)
        {
            if (tower != null)
            {
                CostParametr.Value = tower.Cost;
                RadiusParametr.Value = tower.AttackRadius.Amount;
                PowerParametr.Value = tower.Bullet.Damage;
                ReloadingSpeedParametr.Value = tower.ReloadingSpeed;
                BulletSpeedParametr.Value = tower.Bullet.Speed.Amount;

                if (tower.Bullet is IFreezeBullet)
                {
                    FreezeTimeParametr.Visibility = System.Windows.Visibility.Visible;
                    FreezeFactorParametr.Visibility = System.Windows.Visibility.Visible;

                    FreezeTimeParametr.Value = (tower.Bullet as IFreezeBullet).FreezeTime;
                    FreezeFactorParametr.Value = (tower.Bullet as IFreezeBullet).FreezeFactor;
                }
                else
                {
                    FreezeTimeParametr.Visibility = System.Windows.Visibility.Collapsed;
                    FreezeFactorParametr.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                FillTowerInfo(double.NaN);
            }
        }

        void UpdateTowerUpgradePossibility()
        {
            bool upgradeIsPossible = false;

            var tower = Game.Field.GetSelectedObject(typeof(Tower)) as Tower;

            if (tower is IUpgradeableTower)
            {
                var uptower = tower as IUpgradeableTower;

                if (uptower.Level < uptower.MaxLevel)
                {
                    if (Game.Money >= uptower.CostUpgradeScale[uptower.Level] - tower.Cost)
                    {
                        upgradeIsPossible = true;
                    }
                }
            }

            UpgradeBtn.IsEnabled = upgradeIsPossible;
        }

        void UpdateExplodePossibility()
        {
            var tower = Game.Field.GetSelectedObject(typeof(Tower)) as Tower;

            ExplodeBtn.IsEnabled = tower is IExplodeTower;
        }

        void UpdateSellLabel()
        {
            var tower = Game.Field.GetSelectedObject(typeof(Tower)) as Tower;
            
            SellBtn.ActionText = "Продать";

            if (tower != null)
            {
                SellBtn.IsEnabled = true;
                SellBtn.ActionText += Game.Field.GetSellCost(tower).ToString(" (+#)");
            }
            else
            {
                SellBtn.IsEnabled = false;
            }
        }

        void AddTowerByTowerName(string towerName)
        {
            Point initPoint = new Point(0, 0);
            
            Assembly assembly = Assembly.GetAssembly(typeof(Tower));
            Tower tower = (Tower) assembly.GetType("NRTowerDefense." + towerName).GetConstructor(new Type[] { typeof(Point) }).Invoke(new object[] {initPoint});

            Game.Field.AddTower(new TowerPrototype(tower));
        }

        void GameField_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Game.Field.HasBuildablePrototype())
                {
                    if (Game.Field.BuildPrototype() == BlockTestResult.NoBlock)
                    {
                        AddTowerByTowerName(lastTowerTypeClassName);
                    }
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                ArrowBtn_Click(this, new RoutedEventArgs());
            }
        }

        void ArrowBtn_Click(object sender, RoutedEventArgs e)
        {
            Game.Field.DeselectAll(typeof(Tower));
            Game.Field.ClearPrototype();

            ShowTowerInfo(new TowerArg(null));
        }

        void UprgadeBtn_Click(object sender, RoutedEventArgs e)
        {
            var t = Game.Field.GetSelectedObject(typeof(Tower)) as Tower;

            if (t is IUpgradeableTower)
            {
                (t as IUpgradeableTower).Upgrade();
                Game.UpdateInformationControl(Game.ControlType.MoneyControl);
            }

            ShowTowerInfo(new TowerArg(t));
        }

        void SellBtn_Click(object sender, RoutedEventArgs e)
        {
            var t = Game.Field.GetSelectedObject(typeof(Tower)) as Tower;

            if (t != null)
            {
                Game.Field.SellTower(t);
            }

            ShowTowerInfo(new TowerArg(null));
        }

        void TitleLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void CloseLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        void MinimizeLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        void ExplodeBtn_Click(object sender, RoutedEventArgs e)
        {
            var t = Game.Field.GetSelectedObject(typeof(Tower)) as Tower;

            if (t is IExplodeTower)
            {
                (t as IExplodeTower).Explode();
            }

            ShowTowerInfo(new TowerArg(null));
        }

        void AddTower_Click(object sender, RoutedEventArgs e)
        {
            Game.Field.ClearPrototype();
            Game.Field.DeselectAll(typeof(Tower));

            lastTowerTypeClassName = (sender as Button).Tag as string;
            AddTowerByTowerName((sender as Button).Tag as string);
        }

        void AddTowerBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            string towerClassName = (sender as Button).Tag as string;
            TowerInformationTextBlock.Text = GetTowerDescriptionByClassName(towerClassName);
        }

        void AddTowerBtn_MouseLeave(object sender, MouseEventArgs e)
        {            
            TowerInformationTextBlock.Text = "Для получения информации наведите курсор на башню...";
        }

        string GetTowerDescriptionByClassName(string towerName)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Tower));
            return assembly.GetType("NRTowerDefense." + towerName).GetField("Description").GetValue(null) as string;
        }

        void AbouLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Game.Timer.Pause();
            
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
            
            if (Game.Status != Game.GameStatus.NotStarted)
                Game.Timer.Unpause();
        }
    }
}
