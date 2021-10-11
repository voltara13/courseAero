using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace courseAero
{
    /*Класс главного окна*/
    public partial class MainWindow : Form
    {
        private Data _data = new Data();
        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Метод применения надписей показателей
        /// </summary>
        private void ApplyLabel()
        {
            /*Надписи статистики склада*/
            labelCargoTotal.Text = _data.CargoTotal.ToString();
            labelCargoAccepted.Text = _data.CargoAccepted.ToString();
            labelCargoShipped.Text = _data.CargoShipped.ToString();
            labelCargoCanceled.Text = _data.CargoCanceled.ToString();
            /*Надписи доходов*/
            labelTariffDay.Text = Math.Round(_data.TariffDay, 2).ToString(CultureInfo.CurrentCulture);
            labelTariffMonth.Text = Math.Round(_data.TariffMonth, 2).ToString(CultureInfo.CurrentCulture);
            labelTariffAll.Text = Math.Round(_data.TariffAll, 2).ToString(CultureInfo.CurrentCulture);
            labelTariffAvg.Text = Math.Round(_data.TariffAvg, 2).ToString(CultureInfo.CurrentCulture);
            /*Надписи расходов*/
            labelCostDay.Text = Math.Round(_data.CostDay, 2).ToString(CultureInfo.CurrentCulture);
            labelCostMonth.Text = Math.Round(_data.CostMonth, 2).ToString(CultureInfo.CurrentCulture);
            labelCostAll.Text = Math.Round(_data.CostAll, 2).ToString(CultureInfo.CurrentCulture);
            labelCostAvg.Text = Math.Round(_data.CostAvg, 2).ToString(CultureInfo.CurrentCulture);
            /*Поле моделирования*/
            richTextBox.Text = _data.RichTextBoxString;
        }
        /// <summary>
        /// Метод применения текстовых полей настроек
        /// </summary>
        private void ApplyTextBox()
        {
            /*Текстовые поля настроек склада*/
            textBoxCapacityMax.Text = _data.CapacityMax.ToString();
            textBoxCargoAvg.Text = _data.CargoAvg.ToString();
            textBoxLifeMax.Text = _data.LifeMax.ToString();
            /*Текстовые поля тарифов и расходов*/
            foreach (var price in _data.ListPrice)
            {
                foreach (var textBox in GetControlsSettings<TextBox>(price.GetType() == typeof(Tariff) ? 1 : 2, 1))
                {
                    var tableLayoutPanel = (TableLayoutPanel) textBox.Parent;
                    var row = tableLayoutPanel.GetRow(textBox);
                    var control = tableLayoutPanel.GetControlFromPosition(0, row);
                    /*Если надпись не имеет такого же текста как и имя позиция тарифа или расхода, то пропускаем*/
                    if (control.Text != price.Name) continue;
                    /*В ином случае заменяем текстовое поле и отмечаем чек-бокс, если возможно*/
                    textBox.Text = price.PriceValue.ToString();
                    if (control is CheckBox checkBox)
                        checkBox.Checked = true;
                }
            }
        }
        /// <summary>
        /// Метод вывода предупреждающего окна с сообщением
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private DialogResult ShowMessageBox(string message)
        {
            return MessageBox.Show(
                this,
                message,
                "Предупреждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
        }
        /// <summary>
        /// Метод получения количества контейнеров нормальным распределением случайной величины за день
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        private int GetCargoCount(Random random)
        {
            var u1 = 1.0 - random.NextDouble();
            var u2 = 1.0 - random.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);
            return Convert.ToInt32(Math.Round(_data.CargoAvg + randStdNormal));
        }
        /// <summary>
        /// Метод моделирования
        /// </summary>
        private void Modeling()
        {
            if (_data.ListPrice.Count == 0)
            {
                MessageBox.Show("Для начала настройте склад, тарифы и расходы");
                return;
            }
            _data.ResetStatistics();
            /*Считаем количество дней и день начала последних 30-ти дней*/
            var dayCount = Convert.ToInt32(numericUpDown.Value);
            var dayLastMonthStart = dayCount - 30 < 1 ? 1 : dayCount - 30;
            /*Проводим моделирование*/
            for (var dayNum = 1; dayNum <= dayCount; ++dayNum)
            {
                _data.ModelingDay(dayNum, GetCargoCount(_data.random), dayNum >= dayLastMonthStart);
                _data.RichTextBoxString += "\n";
            }
            _data.RichTextBoxString += "Моделирование успешно завершено!";
        }
        /// <summary>
        /// Метод получения элементов на вкладке "Настройки"
        /// </summary>
        /// <param name="groupBoxIndex"></param>
        /// <param name="groupBoxCount"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IEnumerable<T> GetControlsSettings<T>(int groupBoxIndex, int groupBoxCount)
        {
            return tableLayoutPanelGroupBox.Controls.OfType<GroupBox>().ToList()
                .GetRange(groupBoxIndex, groupBoxCount)
                .Select(groupBox => groupBox.Controls
                    .OfType<TableLayoutPanel>().ToList()[0])
                .SelectMany(tableLayoutPanel => tableLayoutPanel.Controls.OfType<T>());
        }
        /// <summary>
        /// Метод проверки текстовых полей
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private void CheckTextBox()
        {
            if (!double.TryParse(textBoxCapacityMax.Text.Replace(".", ","), out var capacity) || capacity <= 0 ||
                !int.TryParse(textBoxCargoAvg.Text, out var avgCargo) || avgCargo <= 0 ||
                !int.TryParse(textBoxLifeMax.Text, out var maxLife) || maxLife <= 0 ||
                GetControlsSettings<TextBox>(1, 2).Any(textBox => textBox.Enabled && (textBox.Text == string.Empty || !double.TryParse(textBox.Text.Replace(".", ","), out var price) || price <= 0)))
            {
                throw new FormatException();
            }
        }
        /// <summary>
        /// Метод применения настроек
        /// </summary>
        private void AcceptSettings()
        {
            /*Сбрасываем настройки*/
            _data.ResetData();
            /*Применяем настройки склада*/
            _data.CapacityMax = Convert.ToDouble(textBoxCapacityMax.Text);
            _data.CargoAvg = Convert.ToInt32(textBoxCargoAvg.Text);
            _data.LifeMax = Convert.ToInt32(textBoxLifeMax.Text);
            /*Применяем тарифы и расходы*/
            for (var i = 1; i < 3; ++i)
            {
                foreach (var textBox in GetControlsSettings<TextBox>(i, 1))
                {
                    if (textBox.Enabled)
                    {
                        var tableLayoutPanel = (TableLayoutPanel) textBox.Parent;
                        var row = tableLayoutPanel.GetRow(textBox);
                        var control = tableLayoutPanel.GetControlFromPosition(0, row);
                        switch (i)
                        {
                            case 1:
                                _data.ListPrice.Add(new Tariff(control.Text, Convert.ToDouble(textBox.Text)));
                                break;
                            case 2:
                                _data.ListPrice.Add(new Cost(control.Text, Convert.ToDouble(textBox.Text)));
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Метод изменения отметки чек-бокса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox) sender;
            var tableLayoutPanel = (TableLayoutPanel) checkBox.Parent;
            tableLayoutPanel.GetControlFromPosition(1, tableLayoutPanel.GetPositionFromControl(checkBox).Row).Enabled = checkBox.Checked;
        }
        /// <summary>
        /// Метод нажатия на кнопку "Применить" в окне настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAcceptSettings_Click(object sender, EventArgs e)
        {
            try
            {
                /*Проверяем введённые данные*/
                CheckTextBox();
                /*Применяем настройки*/
                AcceptSettings();
                MessageBox.Show("Настройки успешно применены.",
                    "Применение настроек",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Введены неверные значения, либо не все поля заполнены.",
                    "Применение настроек",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        /// <summary>
        /// Метод нажатия на кнопку "Сбросить" в окне настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonResetSettings_Click(object sender, EventArgs e)
        {
            if (ShowMessageBox("Сбросятся значения всех настроек, продолжить?") == DialogResult.No) return;
            /*Очищаем textBox'ы*/
            foreach (var textBox in GetControlsSettings<TextBox>(0, 3))
                textBox.Text = "";
            /*Очищаем checkBox'ы*/
            foreach (var checkBox in GetControlsSettings<CheckBox>(0, 3))
                checkBox.Checked = false;
            /*Сбрасываем настройки*/
            _data.ResetData();
        }
        /// <summary>
        /// Метод нажатия на кнопку "Старт" в окне моделирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonStartModeling_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(Modeling);
            }
            catch
            {
                MessageBox.Show("Произошла ошибка во время моделирования");
            }
            finally
            {
                ApplyLabel();
            }
        }
        /// <summary>
        /// Метод нажатия на кнопку "Сброс" в окне моделирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonResetModeling_Click(object sender, EventArgs e)
        {
            if (ShowMessageBox("Сбросятся значения всех показателей моделирования, продолжить?") == DialogResult.No) return;
            _data.ResetStatistics();
            ApplyLabel();
        }
        /// <summary>
        /// Метод нажатия на кнопку "Загрузить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            /*Диалоговое окно загрузки файла*/
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "bin files (*.bin)|*.bin"
            };
            if (openFileDialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                /*Десериализация*/
                using (var fs = openFileDialog.OpenFile())
                {
                    var formatter = new BinaryFormatter();
                    _data = (Data)formatter.Deserialize(fs);
                    ApplyLabel();
                    ApplyTextBox();
                }
                MessageBox.Show("Состояние успешно загружено.",
                    "Загрузка состояния",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                MessageBox.Show("Произошла ошибка во время десериализации");
            }
        }
        /// <summary>
        /// Метод нажатия на кнопку "Сохранить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            /*Диалоговое окно сохранения файла*/
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "bin files (*.bin)|*.bin"
            };
            if (saveFileDialog.ShowDialog(this) != DialogResult.OK) return;
            try
            {
                /*Сериализация*/
                using (var fs = saveFileDialog.OpenFile())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(fs, _data);
                }
                MessageBox.Show("Состояние успешно сохранено.",
                    "Сохранение состояния",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                MessageBox.Show("Произошла ошибка во время сериализации");
            }
        }
    }
}