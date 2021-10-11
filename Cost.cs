using System;

namespace courseAero
{
    [Serializable]
    /*Класс расходов*/
    public class Cost : Price
    {
        /// <summary>
        /// Конструктор расхода
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priceValue"></param>
        public Cost(string name, double priceValue) : base(name, priceValue) {}
        /// <summary>
        /// Перегруженный метод изменения статистики склада
        /// </summary>
        /// <param name="data"></param>
        /// <param name="factor"></param>
        public override void ChangeStatistics(Data data, double factor)
        {
            data.CostDay += factor * PriceValue;
            data.CostMonth += factor * PriceValue;
            data.CostAll += factor * PriceValue;
        }
    }
}