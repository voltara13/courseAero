using System;
using System.Collections.Generic;
using System.Linq;

namespace courseAero
{
    [Serializable]
    /*Класс данных*/
    public class Data
    {
        /*Поле генератора случайных чисел*/
        public readonly Random random = new Random();
        /*Поле текста моделирования*/
        public string RichTextBoxString;
        /*Поля настроек склада*/
        public double CapacityMax;
        public double Capacity;
        public double CargoAvg;
        public int LifeMax;
        /*Поля результатов моделирования склада*/
        public int CargoTotal;
        public int CargoAccepted;
        public int CargoShipped;
        public int CargoCanceled;
        /*Поля доходов*/
        public double TariffDay;
        public double TariffMonth;
        public double TariffAll;
        public double TariffAvg;
        /*Поля расходов*/
        public double CostDay;
        public double CostMonth;
        public double CostAll;
        public double CostAvg;
        /*Списки расходов и тарифов, а также грузов*/
        public readonly List<Price> ListPrice = new List<Price>();
        private readonly List<Cargo> _listCargo = new List<Cargo>();
        /// <summary>
        /// Метод генерирования груза
        /// </summary>
        /// <returns></returns>
        private Cargo GenerateCargo()
        {
            return new Cargo(
                Math.Round(random.NextDouble() * 5, 2),
                Math.Round(random.NextDouble() * 5, 2),
                Math.Round(random.NextDouble() * 5, 2),
                Math.Round(random.NextDouble() * 1000, 2),
                random.NextDouble() < 0.5,
                random.Next(1, LifeMax));
        }
        /// <summary>
        /// Метод подсчёта доходов и расходов за груз
        /// </summary>
        /// <param name="cargo"></param>
        private void CalculatePrice(Cargo cargo)
        {
            foreach (var price in ListPrice)
            {
                switch (price.Name)
                {
                    case "День хранения груза на складе:":
                        price.ChangeStatistics(this, cargo.DayLife);
                        break;
                    case "Надбавка за каждый м3 груза:":
                        price.ChangeStatistics(this, cargo.GetVolume());
                        break;
                    case "Надбавка за каждый кг груза:":
                        price.ChangeStatistics(this, cargo.Weight);
                        break;
                    case "Надбавка за хрупкость груза:":
                        price.ChangeStatistics(this, cargo.IsFragile ? 1 : 0);
                        break;
                }
            }
        }
        /// <summary>
        /// Метод добавления груза
        /// </summary>
        /// <param name="cargo"></param>
        private void AddCargo(Cargo cargo)
        {
            /*Добавляем груз в список грузов*/
            _listCargo.Add(cargo);
            /*Изменяем показатели склада*/
            CargoTotal++;
            CargoAccepted++;
            Capacity += cargo.GetVolume();
            /*Считаем доходы и затраты груза*/
            CalculatePrice(cargo);
            /*Изменяем текстовое поля моделирования*/
            RichTextBoxString += $"Груз загружен на {cargo.DayLife} {cargo.GetDayString()}. Оставшееся место на складе: {Math.Round(CapacityMax - Capacity, 2)} м3\n";
        }
        /// <summary>
        /// Метод отмены добавления груза
        /// </summary>
        /// <param name="cargo"></param>
        private void CancelCargo(Cargo cargo)
        {
            /*Изменяем показатели склада*/
            CargoCanceled++;
            /*Изменяем текстовое поля моделирования*/
            RichTextBoxString += $"Груз не может быть загружен, так как недостаточно места. Размер груза: {Math.Round(cargo.GetVolume(), 2)} м3, " +
                                 $"оставшееся место на складе: {Math.Round(CapacityMax - Capacity, 2)} м3\n";
        }
        /// <summary>
        /// Метод проверки отправленных грузов со склада
        /// </summary>
        private void CheckCargoEnd()
        {
            /*Считаем объёмы и количество отправленного груза*/
            var volumeFree = _listCargo.Where(cargo => cargo.DayLife == 0).Sum(cargo => cargo.GetVolume());
            var countRemoveCargo = _listCargo.RemoveAll(cargo => cargo.DayLife == 0);
            /*Изменяем показатели склада*/
            Capacity -= volumeFree;
            CargoTotal -= countRemoveCargo;
            CargoShipped += countRemoveCargo;
        }
        /// <summary>
        /// Метод изменения оставшихся дней у груза
        /// </summary>
        private void CargoDayLeft()
        {
            foreach (var cargo in _listCargo)
                cargo.DayLife--;
        }
        /// <summary>
        /// Метод проверки возможности загрузки груза
        /// </summary>
        /// <param name="cargo"></param>
        /// <returns></returns>
        private bool IsLoading(Cargo cargo)
        {
            return Capacity + cargo.GetVolume() <= CapacityMax;
        }
        /// <summary>
        /// Метод сброса статистики за день
        /// </summary>
        private void ResetDayStatistics()
        {
            TariffDay = 0;
            CostDay = 0;
        }
        /// <summary>
        /// Метод моделирования одного дня
        /// </summary>
        /// <param name="dayNum"></param>
        /// <param name="cargoCount"></param>
        /// <param name="isLastMonthStart"></param>
        public void ModelingDay(int dayNum, int cargoCount, bool isLastMonthStart)
        {
            /*Моделируем один день*/
            RichTextBoxString += $"День {dayNum}\n\n";
            ResetDayStatistics();
            for (var cargoNum = 0; cargoNum < cargoCount; ++cargoNum)
            {
                var cargo = GenerateCargo();
                if (IsLoading(cargo))
                    AddCargo(cargo);
                else
                    CancelCargo(cargo);
            }
            CargoDayLeft();
            CheckCargoEnd();
            /*Считаем средние показатели*/
            TariffAvg = TariffAll / dayNum;
            CostAvg = CostAll / dayNum;
            /*Проверяем, является ли данный день одним из последних 30-ти дней*/
            if (isLastMonthStart) return;
            TariffMonth = 0;
            TariffMonth = 0;
        }
        /// <summary>
        /// Метод сброса результатов моделирования
        /// </summary>
        public void ResetStatistics()
        {
            /*Сбрасываем статистику склада*/
            Capacity = 0;
            CargoTotal = 0; 
            CargoAccepted = 0; 
            CargoShipped = 0; 
            CargoCanceled = 0;
            /*Сбрасываем статистику доходов*/
            TariffDay = 0; 
            TariffMonth = 0; 
            TariffAll = 0; 
            TariffAvg = 0;
            /*Сбрасываем статистику расходов*/
            CostDay = 0; 
            CostMonth = 0; 
            CostAll = 0; 
            CostAvg = 0;
            /*Очищаем список грузов*/
            _listCargo.Clear();
            /*Очищаем текстовое поле моделирования*/
            RichTextBoxString = "";
        }
        /// <summary>
        /// Метод сброса настроек склада
        /// </summary>
        public void ResetData()
        {
            Capacity = 0;
            CapacityMax = 0;
            CargoAvg = 0;
            LifeMax = 0;
            ListPrice.Clear();
        }
    }
}