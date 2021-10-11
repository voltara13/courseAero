using System;

namespace courseAero
{
    [Serializable]
    /*Класс тарифов*/
    public class Tariff : Price
    {
        /// <summary>
        /// Конструктор тарифа
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priceValue"></param>
        public Tariff(string name, double priceValue) : base(name, priceValue) {}
        /// <summary>
        /// Перегруженный метод изменения статистики склада
        /// </summary>
        /// <param name="data"></param>
        /// <param name="factor"></param>
        public override void ChangeStatistics(Data data, double factor)
        {
            data.TariffDay += factor * PriceValue;
            data.TariffMonth += factor * PriceValue;
            data.TariffAll += factor * PriceValue;
        }
    }
}