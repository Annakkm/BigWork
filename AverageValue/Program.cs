using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;


namespace BigWork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding(1251); 

            string txt_file = "D:\\Навчання\\КПІ Навчання\\ОТтаП\\BigWork\\Development\\BigProject\\BigProject\\AverageValue\\вар1_TB1_29_ACV.txt";


            Console.WriteLine("1. Середнє значення\n" +
                "2. Вибіркова дисперсія\n" +
                "3. Стандартна помилка\n" +
                "4. Mедіана\n" +
                "5. Мода\n" +
                "6. Стандартне відхилення\n" +
                "7. Ексцес (В розробці)\n" +
                "8. Асиметрія\n");

            Console.WriteLine("Оберіть пункт:");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    var averageValue = AverageValue(txt_file);
                    Console.WriteLine($"Середнє значення: {averageValue}"); //Середнє значення
                    break;
                case "2":
                    double variance = Variance(txt_file);
                    Console.WriteLine($"Вибіркова дисперсія: {variance}");
                    break;
                case "3":
                    double standartError = StandardError(txt_file); //Стандартна помилка
                    Console.WriteLine($"Стандартна помилка: {standartError}");
                    break;
                case "4":
                    double median = Median(txt_file); //Медіана
                    Console.WriteLine($"Медіана: {median}");
                    break;
                case "5":
                    double mode = Mode(txt_file); // Мода
                    Console.WriteLine($"Мода: {mode}");
                    break;
                case "6":
                    double standartDeviation = StandartDeviation(txt_file); // стандартне відхилення
                    Console.WriteLine($"Стандартне відхилення: {standartDeviation}");
                    break;
                case "7":
                    //double excess = Excess(txt_file); // ексцес
                    //Console.WriteLine($"Ексцес: {excess}");
                    Console.WriteLine("На жаль, поки що ексцес в розробці, спробуйте пізніше");
                    break;
                case "8":
                    double asymmetry = Asymmetry(txt_file);
                    Console.WriteLine($"Асиметрія: {asymmetry}");
                    break;
                default:
                    Console.WriteLine("Некоректний вибір!");
                    break;
            }

            Console.ReadKey();
        }


        static double Asymmetry(string txt_file)
        {
            double[] data = ReadData(txt_file);
            int N = data.Length;

            double standart_deviation = StandartDeviation(txt_file); // стандартне відхилення
            double AverageValue = data.Average();// середнє значення 

            double sum_cubed_diffs = data.Sum(x => Math.Pow(x - AverageValue, 3));

            double asymmetry = (N * sum_cubed_diffs) / ((N - 1) * (N - 2) * Math.Pow(standart_deviation, 3));

            return asymmetry;
        }

        static double Excess(string txt_file)
        {
            double[] data = ReadData(txt_file);
            int N = data.Length;

            //Середнє значення
            double averageValue = AverageValue(txt_file);

            //Стандартне відхилення
            double standartDeviation = StandartDeviation(txt_file);

            // Нормалізоване значення
            double[] normalizedValue = NormalizedValue(txt_file, averageValue, standartDeviation);

            // сума нормалізованих значень
            double sumNormalizedValue = 0.0;
            foreach (double value in normalizedValue)
            {
                sumNormalizedValue += value;
            }
            Console.WriteLine("Сума нормалізованих значень: " + sumNormalizedValue);
            Console.WriteLine("N: " + N);

            //обрахунок множника перед сумою 
            double numerator = N * (N + 1);
            double denominator = (N - 1) * (N - 2) * (N - 3);
            double multiplier = numerator / denominator;

            //(double)(N * (N + 1)) / (N - 1) * (N - 2) * (N - 3);
            //CalculateMultiplier(N);
            return multiplier; //не дороблено 
        }

        //static double CalculateMultiplier(int N)
        //{
        //    double multiplier = (double)(N * (N + 1)) / ((N - 1) * (N - 2) * (N - 3));
        //    return multiplier;
        //}

        static double[] NormalizedValue(string txt_file, double average, double deviation)
        {
            double[] data = ReadData(txt_file);

            double[] NormalazedArray = new double[data.Length];

            for (int i = 0; i < data.Length; i++)
{
                double normalizedValue = (data[i] - average) / deviation;
                NormalazedArray[i] = Math.Pow(normalizedValue, 4);
            }
            return NormalazedArray; 
        }

        static double[] ReadData(string txt_file)
        {
            var culture = new CultureInfo("uk-UA");
            culture.NumberFormat.NumberDecimalSeparator = ",";
            return File.ReadAllLines(txt_file).Select(line => double.Parse(line, culture))
                                           .ToArray();
        }

        static double StandartDeviation(string txt_file) //стандартне відхилення
        {
            double resVariance = Variance(txt_file);
            double standartdeviation = Math.Sqrt(resVariance);
            return standartdeviation;
        }

        static double Mode(string txt_file)
        {
            double[] data = ReadData(txt_file);

            double mode = data[0];
            int maxCount = 0;

            for (int i = 0; i < data.Length; i++)
            {
                // рахуємо скільки разів поточне число зустрічається в масиві
                int count = 0; //перед обчисленням кількості поточного числа в масиві очищуєм count до нуля
                    
                for (int j = 0; j < data.Length; j++)
                {
                    if(data[i] == data[j])
                    {
                        count++;
                    }
                }

                if (count > maxCount)
                {
                    mode = data[i];  
                    maxCount = count; 
                }

            }
            return mode;
        }

        static double Median(string txt_file)
        {
            double[] data = ReadData(txt_file);

            for (int i = 0; i < data.Length - 1; i++)
            {
                for(int j = 0; j < data.Length - i - 1; j++)
                {
                    if (data[j] > data[j + 1])
                    {
                        double temp = data[j]; 
                        data[j] = data[j + 1];
                        data[j + 1] = temp;
                    }
                }
            }

            int length = data.Length;
            double median;

            if(length % 2 == 0) //якщо парна кількість
            {
                median = (data[(length / 2) - 1] + data[length / 2]) / 2.0; // середнє арифм, 2-х центральних елементів
            }
            else // якщо не парна
            {
                median = data[length / 2];
            }

            return median;
        }

        static double StandardError(string txt_file)
        {
            double variance = Variance(txt_file);

            double[] data = ReadData(txt_file);

            double standartError = Math.Sqrt(variance / data.Length);

            return standartError;
        }

        static double Variance(string txt_file)
        {
            double[] data = ReadData(txt_file);

            double AverageValue = data.Average();

            double Variance = data.Sum(x => Math.Pow(x - AverageValue, 2) / (data.Length - 1));

            return Variance;
        }

        static double AverageValue(string txt_file)
        {
            double[] data = ReadData(txt_file);

            Console.WriteLine("Зчитані значення:");
            foreach (var value in data)
            {
                Console.WriteLine(value);
            }
            Console.WriteLine("--------------------------");

            double AverageValue = data.Average();

            return AverageValue;
        }
    }
}
