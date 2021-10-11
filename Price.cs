using System;

namespace courseAero
{
    [Serializable]
    /*Абстрактный класс тарифов и расходов*/
    public abstract class Price
    {
        /*Поля названия тарифа или расхода и его цены*/
        public readonly string Name;
        public readonly double PriceValue;
        /// <summary>
        /// Конструктор тарифа или расхода
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priceValue"></param>
        protected Price(string name, double priceValue)
        {
            Name = name;
            PriceValue = priceValue;
        }
        /// <summary>
        /// Абстрактный метод изменения статистики склада
        /// </summary>
        /// <param name="data"></param>
        /// <param name="factor"></param>
        public abstract void ChangeStatistics(Data data, double factor);
    }
}