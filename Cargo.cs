using System;

namespace courseAero
{
    [Serializable]
    /*Класс груза*/
    public class Cargo
    {
        /*Поля габаритов груза*/
        private readonly double _lengths;
        private readonly double _width;
        private readonly double _height;
        /*Поля массы, хрупкости и количества дней груза*/
        public readonly double Weight;
        public readonly bool IsFragile;
        public int DayLife;
        /// <summary>
        /// Конструктор груза
        /// </summary>
        /// <param name="lengths"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="isFragile"></param>
        /// <param name="dayLife"></param>
        public Cargo(double lengths, double width, double height, double weight, bool isFragile, int dayLife)
        {
            _lengths = lengths;
            _width = width;
            _height = height;
            Weight = weight;
            IsFragile = isFragile;
            DayLife = dayLife;
        }
        /// <summary>
        /// Метод получения объёма груза
        /// </summary>
        /// <returns></returns>
        public double GetVolume()
        {
            return _lengths * _width * _height;
        }
        /// <summary>
        /// Метод получения склонения слова "день" в зависимости от количества оставшихся дней
        /// </summary>
        /// <returns></returns>
        public string GetDayString()
        {
            return 
                DayLife % 100 <= 10 || DayLife % 100 >= 20
                ? DayLife % 100 % 10 <= 1 || DayLife % 100 % 10 >= 5
                    ? DayLife % 100 % 10 == 1 ? "день" : "дней"
                    : "дня"
                : "дней";
        }
    }
}