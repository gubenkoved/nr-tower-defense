using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace NRTowerDefense
{
    public abstract class GameObjectDecorator: GameObject
    {
        /// <summary>
        /// Статус у ядра декораторов
        /// </summary>
        public new GameObjectState State
        {
            set
            {
                Kernel.State = value;
            }
            get
            {
                return Kernel.State;
            }
        }
        /// <summary>
        /// Радиус игрового объекта в ядре декораторов
        /// </summary>
        public new GameCell Radius
        {
            set
            {
                Kernel.Radius = value;
            }
            get
            {
                return Kernel.Radius;
            }
        }
        /// <summary>
        /// Позиция игрового объекта в дре декораторов на поле
        /// </summary>
        public new Point Position
        {
            set
            {
                Kernel.Position = value;
            }
            get
            {
                return Kernel.Position;
            }
        }
        /// <summary>
        /// Флаг, "включено" ли отображение декоратора
        /// </summary>
        public bool Enabled;
        /// <summary>
        /// Декорируемый GameObject
        /// </summary>
        protected GameObject component;
        /// <summary>
        /// Возвращает реальное "ядро" декораторов, учитавая возможность их вложенности
        /// </summary>
        public GameObject Kernel
        {
            get
            {
                if (component is GameObjectDecorator)
                    return (component as GameObjectDecorator).Kernel;
                else
                    return component;
            }
        }
        /// <summary>
        /// Возвращает агрегируемый компонент
        /// </summary>
        public GameObject Component
        {
            get
            {
                return component;
            }
        }

        public GameObjectDecorator(GameObject gameObject)
        {
            component = gameObject;
        }

        // делаем декоратор "прозрачным" по интерфейсу
        public override void Tick(double interval)
        {
            component.Tick(interval);
        }
        public override void Draw(Canvas canvas)
        {
            component.Draw(canvas);
        }
        public override void ClearFromCanvas(Canvas canvas)
        {
            component.ClearFromCanvas(canvas);
        }
    }
}
