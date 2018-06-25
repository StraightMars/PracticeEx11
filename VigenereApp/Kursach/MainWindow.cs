using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Kursach
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// массив, содержащий различные знаки и пробел
        /// </summary>
        static string[] signs = new string[] {"!", "?", ".", ",", "@", "_", "-", ":", ";", "(", ")", "*", "^", "$", "%",
                                              "#", "№", "—", "/", "»", "«",
                                              "\n", "[", "]", "=", "+", " "};
        /// <summary>
        /// массив, содержащий символы кириллицы обоих регистров и цифры
        /// </summary>
        string[] alphabet = new string[] {"а", "б", "в", "г", "д", "е", "ж", "з", "и", "й", "к", "л", "м", "н", "о",
                    "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я", "А", "Б", "В", "Г",
                    "Д", "Е", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч",
                    "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"};
        /// <summary>
        /// функция, проверяющая, является ли символ знаком, возвращающая "да", если является и "нет", если не является
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        static bool IsSign(string a)
        {
            for (int i = 0; i < signs.Length; i++)
            {
                if (a == signs[i])
                    return true;
            }
            return false;
        }
        /// <summary>
        /// функция, отвечающая за открытие файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string str = File.ReadAllText(ofd.FileName, Encoding.Default);
                //создание регулярного выражения, разрешающего ввод символов кириллицы, цифр, знаков пунктуации и пробелов
                Regex rgx = new Regex(@"(^([а-яА-Я0-9\p{P}\s])+$)");
                //если удовлетворяет регулярному выражению, то записываем в текстбокс
                if (rgx.IsMatch(str))
                {
                    richTextBox1.Text = str;
                }
                //иначе программа выдает ошибку
                else
                {
                    MessageBox.Show("В файле есть недопустимые символы! Измените файл или откройте другой.", "Ошибка!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        /// <summary>
        /// функция по очистке всех трех боксов: исходный текст, зашифрованный текст и бокс для ввода ключа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
            KeyBox.Clear();
        }
        /// <summary>
        /// функция, сохраняющая текст из второго текстбокса в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile_Click(object sender, EventArgs e)
        {
            if (richTextBox2.Text == "")
            {
                MessageBox.Show("Нет преобразованного текста!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Создайте новый файл или выберите существующий для перезаписи", "Новый файл",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "*.txt";
                sfd.Filter = "Text files|*.txt";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
                sfd.FileName.Length > 0)
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.Default))
                    {
                        sw.WriteLine(richTextBox2.Text, Encoding.Default);
                        sw.Close();
                    }
                }
            }
        }
        /// <summary>
        /// функция, отвечающая за процесс шифрования
        /// </summary>
        public void CaesarEncryption()
        {
            richTextBox2.Text = "";
            string input = "";
            int letterNumber;
            input = richTextBox1.Text;
            string tempKey = KeyBox.Text;
            int key = Int32.Parse(tempKey);
            //если оба основных бокса не пустые
            if (richTextBox1.Text != "" && KeyBox.Text != "")
            {
                //создание рег. выр. для поля "исходный текст"
                Regex rgx = new Regex(@"(^([а-яА-Я0-9\p{P}\s])+$)");
                //создание рег. выр для поля ввода ключа
                Regex keyRgx = new Regex(@"(?<=^|\s)([1-9]|([1-6]\d)|(7[0-5]))(?=\s|$)");
                //если хотя бы в одном из полей введены неверные символы
                if (!rgx.IsMatch(richTextBox1.Text) || !keyRgx.IsMatch(KeyBox.Text)) 
                {
                    MessageBox.Show("Были введены неверные символы! Введите другой текст.", "Ошибка!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    richTextBox1.Clear();
                    KeyBox.Clear();
                }
                else
                {
                    //создание массива для символов вводимого текста
                    string[] inputArr = new string[input.Length];
                    //создание массива для номеров символов исходного текста
                    int[] inputIndex = new int[input.Length];
                    //создание массива для записи результатов шифрования
                    string[] encrypted = new string[input.Length];
                    //разбиваем исходный текст на символы типа string
                    for (int i = 0; i < input.Length; i++)
                    {
                        inputArr[i] = Convert.ToString(input[i]);
                    }
                    for (int i = 0; i < input.Length; i++)
                    {
                        for (int j = 0; j < alphabet.Length; j++)
                        {
                            //если символ исходного текста есть в алфавите
                            if (inputArr[i] == alphabet[j]) 
                            {
                                //запоминается номер этого символа в алфавите
                                inputIndex[i] = j; 
                            }
                        }
                    }
                    //проверка, является ли знаком элемент, если является, переходим на след. итерацию
                    for (int i = 0; i < inputArr.Length; i++)
                    {
                        if (IsSign(inputArr[i]))
                        {
                            encrypted[i] = inputArr[i];
                            continue;
                        }
                        //вычисляем номер символа, получившегося в результате с помощью формулы: 
                        //(номер символа исх. текста + номер символа ключа) / длину алфавита
                        letterNumber = (inputIndex[i] + key) % alphabet.Length;
                        //запоминаем этот символ
                        encrypted[i] = alphabet[letterNumber];
                        //обнуляем переменную, куда записывали номер элемента
                        letterNumber = 0; 
                    }
                    //в текстбокс записываем результат шифрования
                    for (int i = 0; i < inputIndex.Length; i++)
                    {
                        richTextBox2.Text += encrypted[i];
                    }
                }
            }
            else
            {
                MessageBox.Show("Не все обязательные поля заполнены!", "Ошибка!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        public void CaesarDecryption()
        {
            richTextBox2.Text = "";
            string input = "";
            int letterNumber;
            input = richTextBox1.Text;
            string tempKey = KeyBox.Text;
            int key = Int32.Parse(tempKey);
            if (richTextBox1.Text != "" && KeyBox.Text != "")
            {
                Regex rgx = new Regex(@"(^([а-яА-Я0-9\p{P}\s])+$)");
                Regex keyRgx = new Regex(@"(?<=^|\s)([1-9]|([1-6]\d)|(7[0-5]))(?=\s|$)");
                if (!rgx.IsMatch(richTextBox1.Text) || !keyRgx.IsMatch(KeyBox.Text))
                {
                    MessageBox.Show("Были введены неверные символы! Введите другой текст.", "Ошибка!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    richTextBox1.Clear();
                    KeyBox.Clear();
                }
                else
                {
                    string[] inputArr = new string[input.Length];
                    int[] inputIndex = new int[input.Length];
                    string[] decrypted = new string[input.Length]; //создание массива для записи результатов дешифрования

                    for (int i = 0; i < input.Length; i++)
                    {
                        inputArr[i] = Convert.ToString(input[i]);
                    }
                    for (int i = 0; i < input.Length; i++)
                    {
                        for (int j = 0; j < alphabet.Length; j++)
                        {
                            if (inputArr[i] == alphabet[j])
                            {
                                inputIndex[i] = j;
                            }
                        }
                    }
                    for (int i = 0; i < inputArr.Length; i++)
                    {
                        if (IsSign(inputArr[i]))
                        {
                            decrypted[i] = inputArr[i];
                            continue;
                        }
                        //вычисляем номер символа, получившегося в результате с помощью формулы: 
                        //(номер символа зашифр. текста - ключ + длина алфавита) / длина алфавита
                        letterNumber = (inputIndex[i] - key + alphabet.Length) % alphabet.Length;
                        decrypted[i] = alphabet[letterNumber];
                        letterNumber = 0;
                    }
                    for (int i = 0; i < inputIndex.Length; i++)
                    {
                        richTextBox2.Text += decrypted[i];
                    }
                }
            }
            else
            {
                MessageBox.Show("Не все обязательные поля заполнены!", "Ошибка!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// функция при нажатии основной кнопки "Выполнить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaesarEncryption_Click(object sender, EventArgs e)
        {
            //очистка поля "Исходный текст"
            richTextBox2.Clear();
            //если стоит маркер "Зашифровать"
            if (radioButton1.Checked) 
            {
                //вызов функции шифрования
                CaesarEncryption(); 
            }
            else
            {
                //если не стоит ни один маркер
                if (radioButton1.Checked == false && radioButton2.Checked == false) 
                    MessageBox.Show(@"Выберите, что нужно сделать:
зашифровать или расшифровать?", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //если стоит маркер "Расшифровать"
                if (radioButton2.Checked) 
                {
                    //вызов функции дешифрования
                    CaesarDecryption(); 
                }
            }

        }
        /// <summary>
        /// Закрытие программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, EventArgs e) 
        {
            Application.Exit();
        }
        private void TurnAboutProgram(object sender, EventArgs e)
        {
            help.Enabled = !help.Enabled;
        }
        /// <summary>
        /// открытие формы со справкой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void help_Click(object sender, EventArgs e)
        {
            Help newForm = new Help();
            TurnAboutProgram(null, null);
            newForm.Closed += TurnAboutProgram;
            newForm.Show();
        }
        /// <summary>
        /// ограничение на ввод недопустимых символов в поле "Исходный текст"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e) 
        {
            if (e.KeyChar != 8 && e.KeyChar != 32 && !Char.IsPunctuation(e.KeyChar) && !Char.IsDigit(e.KeyChar)
                && (e.KeyChar < 1072 || e.KeyChar > 1103) && (e.KeyChar < 1040 || e.KeyChar > 1071))
                e.Handled = true;
        }
        /// <summary>
        /// ограничение на ввод недопустимых символов в поле "Введите ключ" (ключевое слово)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyBox_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;
        }
    }
}
