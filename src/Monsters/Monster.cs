using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Helper;
using System.ComponentModel;

namespace NRTowerDefense
{
    /// <summary>
    /// Абстрактный класс монстр
    /// </summary>
    public abstract class Monster : GameObject, INotifyPropertyChanged
    {
        /// <summary>
        /// Текствовое описание монстра
        /// </summary>
        public static string Description = "Описание монстра";
        /// <summary>
        /// Реализуем интерфейс INotifyPropertyChanged, для поддержки уведомлений о изменении основных свойств
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        /// <summary>
        /// Принудительный пересчет маршрута
        /// </summary>
        public void RecalculatePath()
        {
            SetPath( findPath(Position, Target) );
        }
        /// <summary>
        /// Принудительная установка маршрута
        /// </summary>
        /// <param name="path"></param>
        public void SetPath(List<Point> path)
        {
            this.path = path;
            nextPointNumber = 0;
        }
        /// <summary>
        /// Возвращает путь монстра
        /// </summary>
        public List<Point> GetPath()
        {
            return path;
        }
        /// <summary>
        /// Поиск пути от точки до точки
        /// </summary>
        protected List<Point> findPath(Point current, Point target)
        {
            return PathFinder.FindPath(current, target);
        }
        private double angle;
        /// <summary>
        /// Угол поворота монстра
        /// </summary>
        public double Angle
        {
            private set
            {
                angle = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Angle"));
            }
            get
            {
                return angle;
            }
        }
        private GameCell speed;
        /// <summary>
        /// Скорость передвижения монстра
        /// </summary>
        public GameCell Speed
        {
            private set 
            {
                speed = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Speed"));
            }
            get
            {
                
                return speed;
            }
        }
        public double life;
        /// <summary>
        /// Жизни монстра
        /// </summary>
        public double Life
        {
            private set
            {
                life = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Life"));
            }
            get
            {
                return life;
            }
        }
        private double startLife;
        /// <summary>
        /// Начальное количество жизней
        /// </summary>
        public double StartLife
        {
            private set
            {
                startLife = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StartLife"));
            }
            get
            {
                return startLife;
            }
        }
        private double cost;
        /// <summary>
        /// Вознаграждение за убийство
        /// </summary>
        public double Cost
        {
            private set
            {
                cost = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Cost"));
            }
            get
            {
                return cost;
            }
        }
        private double armour;
        /// <summary>
        /// Броня монстра (повреждение = max(0,сила - броня))
        /// </summary>
        public double Armour
        {
            private set
            {
                armour = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Armour"));
            }
            get
            {
                return armour;
            }
        }
        private Point target;
        /// <summary>
        /// Целевое местоназначение монстра
        /// </summary>
        public Point Target
        {
            private set
            {
                target = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Target"));
            }
            get
            {
                return target;
            }
        }
        /// <summary>
        /// Путь - связка точек, конечной в которой является target
        /// </summary>
        private List<Point> path;
        /// <summary>
        /// Номер точки в пути, к которой следует монстр
        /// </summary>
        private int nextPointNumber;
        /// <summary>
        /// Скорость поворота [град/сек]
        /// </summary>
        private double rotationSpeed;
        public double RotationSpeed
        {
            private set
            {
                rotationSpeed = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RotationSpeed"));
            }
            get
            {
                return rotationSpeed;
            }
        }
        private bool killed;
        /// <summary>
        /// Был ли убит монстр
        /// </summary>
        public bool Killed
        {
            private set
            {
                killed = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Killed"));
            }
            get
            {
                return killed;
            }
        }
        private double freezeFactor;
        /// <summary>
        /// Степень замороженности в долях( н-р 0.1 означает замедление на 10% )
        /// </summary>
        public double FreezeFactor
        {
            private set
            {
                freezeFactor = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FreezeFactor"));
            }
            get
            {
                return freezeFactor;
            }
        }
        private double freezeTime;
        /// <summary>
        /// Время до конца заморозки
        /// </summary>
        public double FreezeTime
        {
            private set
            {
                freezeTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FreezeTime"));
            }
            get
            {
                return freezeTime;
            }
        }
        /// <summary>
        /// "Жирный" конструктор
        /// </summary>
        public Monster(Point position, double angle, GameCell speed, double life, double cost, double armour, Point target, double rotationSpeed)
        {
            this.Position = position;
            this.Angle = angle;
            this.Speed = speed;
            this.Life = life;
            this.StartLife = life;
            this.Cost = cost;
            this.Armour = armour;
            this.Target = target;
            this.RotationSpeed = rotationSpeed;
        }
        /// <summary>
        /// Преопределяемая реакция на тик
        /// </summary>
        public override void Tick(double interval)
        {
            #region Блок учёта заморозки
            FreezeTime = Math.Max(0.0, FreezeTime - interval);

            if (FreezeTime == 0.0 && FreezeFactor != 0.0)
            {
                FreezeFactor = 0.0;
            } 
            #endregion

            #region Обработка path
            if (Position == Game.Field.TargetMonsterPosition || nextPointNumber == path.Count)
            {
                Pass();
                return;
            }

            if (path == null)
                return;

            if (path[nextPointNumber] == Position)
            {
                ++nextPointNumber;
                Tick(interval);
                return;
            } 
            #endregion

            #region Повороты
            double needAngle = HelperFunctions.GetAngleFromPointToPoint(Position, path[nextPointNumber]);
            if (Angle != needAngle)
            {
                double needTurn = needAngle - Angle;
                double mayTurn = RotationSpeed * interval;

                while (Math.Abs(needTurn) > 180)
                {
                    needTurn += -360 * Math.Sign(needTurn);
                }

                if (mayTurn < Math.Abs(needTurn))
                {
                    Angle += mayTurn * Math.Sign(needTurn);
                    return;
                }
                else
                {
                    Angle = needAngle;
                    Tick(interval - needTurn / RotationSpeed);
                    return;
                }
            } 
            #endregion

            #region Движение
            double mayGoing = getDistanceWhichMonsterMayGoing(interval);
            double needGoing = HelperFunctions.GetLength(path[nextPointNumber], Position);

            if (mayGoing < needGoing)
            {
                Position = Position
                    + new Vector(
                            +mayGoing * Math.Sin(Math.PI * needAngle / 180.0),
                            -mayGoing * Math.Cos(Math.PI * needAngle / 180.0)
                        );
                return;
            }
            else
            {
                // дошли до очередной метки
                Position = new Point
                    (
                        path[nextPointNumber].X, 
                        path[nextPointNumber].Y
                    );

                ++nextPointNumber;
                
                Tick(interval - needGoing / Speed.ToPixels());
                return;
            } 
            #endregion
        }
        /// <summary>
        /// Действия на попадание ракеты
        /// </summary>
        /// <param name="bullet"></param>
        public virtual void Hit(Bullet bullet)
        {
            TryFreeze(bullet);

            Life -= CalculateDamage( bullet );

            if (Life <= 0)
            {
                Killed = true;
                Game.OnMonsterDied(this);

                TextGameAnimation increaseMoneyAnimation = new TextGameAnimation(Cost.ToString("+#"), Position, Colors.Gold, 3.0, 18);
                Game.Field.AddAnimation(increaseMoneyAnimation);

                DisappearanceImageGameAnimation deathAnimation = new DisappearanceImageGameAnimation(new Uri("/images/skull.png", UriKind.Relative), Position, 10.0);
                Game.Field.AddAnimation(deathAnimation);
            }
        }
        /// <summary>
        /// Подсчёт урона от ракеты
        /// </summary>
        /// <param name="bullet"></param>
        /// <returns></returns>
        public double CalculateDamage(Bullet bullet)
        {
            return Math.Max(0, bullet.Damage - Armour);
        }
        /// <summary>
        /// Метод запускается когда монстр прошёл до точки назначения
        /// </summary>
        public void Pass()
        {
            Killed = true;    // убираем его с поля
            Game.OnMonsterPass(this);

            TextGameAnimation decreaseLifeAnimation = new TextGameAnimation( "-1", Position, Colors.Red, 3.0, 20);
            Game.Field.AddAnimation(decreaseLifeAnimation);
        }
        /// <summary>
        /// Возвращает копию монстра
        /// </summary>
        /// <returns></returns>
        public abstract Monster Clone();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeInterval"></param>
        /// <returns></returns>
        protected double getDistanceWhichMonsterMayGoing(double timeInterval)
        {
            return timeInterval * (1 - FreezeFactor) * Speed.ToPixels();
        }
        /// <summary>
        /// Попробовать заморозить
        /// </summary>
        /// <param name="bullet"></param>
        public virtual void TryFreeze(Bullet bullet)
        {
            if (bullet is IFreezeBullet)
            {
                FreezeFactor = Math.Max((bullet as IFreezeBullet).FreezeFactor, FreezeFactor);
                FreezeTime = Math.Max((bullet as IFreezeBullet).FreezeTime, FreezeTime);
            }
        }
    }
}
