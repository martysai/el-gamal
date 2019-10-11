using System;
using System.Collections.Generic;

namespace PolynomialLibrary
{
    public static class Data
    {
        // Список хранит в себе все простые числа от [1, 500];
        public static List<int> primeNumbers;

        public static readonly Random randomizer = new Random(); // генератор случайных чисел

        public static Dictionary<char, int> codesOfSymbols; // Коды символов в конечном поле

        public static string CreatePrimePolynomialMessage; // 

        public static string Message { get; set; } // Храним исходное передаваемое сообщение

        public static Field FieldOfCryptosystem { get; set; }
        // поле, в котором будут производиться все операции после выбора элементов;

        static Data()
        {
            codesOfSymbols = new Dictionary<char, int>();
            primeNumbers = new List<int>();
            CreatePrimePolynomialMessage = "";
            OutputPolynomialList = new List<KeyValuePair<Polynomial, Polynomial>>(); // Для вывода операций
            // Считаем, что работа программы предусматривает не инициализированные поля; 
        }

        public static void ClearData()
        {
            primeNumbers = new List<int>();
            codesOfSymbols = new Dictionary<char, int>();
            CreatePrimePolynomialMessage = "";
            Message = "";
            Account.CommonKey = "";
            Account.InversedPower = new Polynomial();
        }

        /// <summary>
        /// Заполняем простые числа изначально
        /// </summary>
        private static void FulfilNumbers()
        {
            for (int i = 2; i < 500; i++) // здесь 500 - количество простых чисел
                if (IsPrime(i))
                    primeNumbers.Add(i);
        }

        /// <summary>
        /// Нужно создать таблицу кодировок, предварительно создав таблицу простых чисел
        /// </summary>
        /// <param name="message">Входное сообщение</param>
        public static void CreateCodes(string message)
        {
            ClearData();
            FulfilNumbers();
            
            Message = message;
            codesOfSymbols = new Dictionary<char, int>();
            char[] cp = new char[message.Length];
            int counter = 0; // Счетчик используется для установления соответствия между символами и их кодами 
            message.CopyTo(0, cp, 0, message.Length);
            Array.Sort(cp);
            HashSet<char> buffer = new HashSet<char>();
            foreach (char symbol in cp)
            {
                if (!buffer.Contains(symbol))
                    codesOfSymbols[symbol] = counter++;
                buffer.Add(symbol);
            }
        }

        /// <summary>
        /// Возведение числа в степень по модулю. Оценка по времени: O(log c);
        /// </summary>
        /// <param name="basis">Основание искомого числа</param>
        /// <param name="pow">Степень искомого числа</param>
        /// <param name="modulus">Модуль</param>
        /// <returns>Возвращает величину a^c mod modulus</returns>
        public static int Power(int basis, int pow, int modulus)
        {
            int res = 1;
            while (pow > 0)
            {
                if ((pow % 2) == 1)
                {
                    res *= basis;
                    --pow;
                }
                else
                {
                    basis *= basis;
                    pow /= 2;
                }
            }
            return res;
        }

        /// <summary>
        ///  Проверка числа на простоту (тривиальный перебор за О(sqrt(n)));
        /// </summary>
        private static bool IsPrime(int number)
        {
            for (int i = 2; i * i <= number; i++)
                if (number % i == 0)
                    return false;
            return true;
        }

        /// <summary>
        /// Получает входной параметр, по которому ищет ближайшее простое число.
        /// </summary>
        public static void FindPrime(int counter, ref int order)
        {
            for (int i = 0; i < primeNumbers.Count; i++)
                if (primeNumbers[i] >= counter)
                {
                    order = primeNumbers[i];
                    return;
                }
        }

        // Здесь описаны методы, ведущие к поиску неприводимого многочлена над фиксированным полем
        #region Prime Polynomials

        public static List<KeyValuePair<Polynomial, Polynomial>> OutputPolynomialList; // Список хранит промежуточные многочлены (нужно для вывода в DisplayWindow.cs)

        /// <summary>
        /// Ищет неприводимый многочлен степени 5 в поле вычетов по модулю order
        /// Асимптотика - O(p^8)
        /// </summary>
        /// <param name="order">Простое число, относительно которого ищется многочлен</param>
        /// <returns>Возвращает неприводимый многочлен</returns>
        public static Polynomial CreatePrimePolynomial(int order)
        {
            List<int> counter = new List<int> { 1, 0, 0, 0, 0, 1 }; // Счетчик начинает идти от первого многочлена пятой степени (нетривиального)
            Polynomial buffer = new Polynomial(counter); // Многочлен на основе счетчика
            while (buffer.CoefList[0] != 0)
            {
                Polynomial divisor = new Polynomial(); // divisor - потенциальный делитель
                if (IsPrimePolynomial(buffer, order, ref divisor))
                {
                    return buffer;
                }
                OutputPolynomialList.Add(new KeyValuePair<Polynomial, Polynomial>(buffer, divisor));
                Increment(ref counter, order); // Увеличение счетчика происходит по специальным правилам
                buffer = new Polynomial(counter); // Нужно выполнить "инкремент" и для buffer;
            }
            throw new IndexOutOfRangeException("Допущена ошибка в написании программы");
        }

        /// <summary>
        /// Алгоритм проверки многочлена пятой степени на простоту.
        /// Использует проверку на делимость на линейные и на квадратичные многочлены.
        /// Асимптотика: O(p^3)
        /// </summary>
        public static bool IsPrimePolynomial(Polynomial poly, int order, ref Polynomial second)
        {
            // Для начала проверим, делится ли многочлен на какой-нибудь линейный двучлен
            for (int i = 0; i < order; i++)
                if (poly.GetValueOf(i) % order == 0) // Для этого достаточно посмотреть на значение многочлена в точке
                {
                    int k = i; // Локальная переменная, значит свободную часть в линейном члене (x - k)
                    k = -k;
                    Abduction(ref k, order); // Приведем ее к каноническому виду (т.е. вычет)
                    second = new Polynomial(new List<int> { 1, k }); // Проверим на делимость
                    return false;
                }
            for (int i = 1; i < order; i++)
                for (int j = 0; j < order; j++)
                    for (int k = 0; k < order; k++)
                    {
                        Polynomial current = new Polynomial(new List<int> { i, j, k });
                        Polynomial mod = Modulus(poly, current, order);
                        Abduction(ref mod, order);
                        if (mod == Polynomial.Zero || mod[0] == 0)
                        {
                            second = current;
                            return false;
                        }
                    }
            second = Polynomial.Zero;
            return true;
        }

        /// <summary>
        /// Вспомогательная функция для увеличения счетчика
        /// </summary>
        public static void Increment(ref List<int> counter, int order)
        {
            int cnt = 0;
            while (cnt != 6)
            {
                counter[counter.Count - cnt - 1]++;
                counter[counter.Count - cnt - 1] %= order;
                if (counter[counter.Count - cnt - 1] == 0)
                    ++cnt;
                else
                    break;
            }
        }

        #endregion

        // Здесь записаны методы, позволяющие оперировать вычетами
        #region Operations in the field on numbers

        /*** Во всех операциях применяется Abduction, т.е. вычеты приводятся к нормальному виду ***/

        /// <summary>
        /// Выполняет приведение вычета к привычному виду 
        /// </summary>
        public static void Abduction(ref int a, int order)
        {
            if (a >= order)
                a %= order;
            else if (a < 0)
            {
                a = -a;
                a %= order;
                a = order - a;
                a %= order;
            }
        }

        /// <summary>
        /// Выполняет умножение вычетов
        /// </summary>
        public static int Multiply(int a, int b, int order)
        {
            Abduction(ref a, order);
            Abduction(ref b, order);
            return (a * b) % order;
        }

        /// <summary>
        /// Складывает два вычета
        /// </summary>
        /// <returns></returns>
        public static int Sum(int a, int b, int order)
        {
            Abduction(ref a, order);
            Abduction(ref b, order);
            return (a + b) % order;
        }

        /// <summary>
        /// Берет обратный к данному вычет
        /// </summary>
        public static int Inversed(int a, int order)
        {
            Abduction(ref a, order);
            return (order - a) % order;
        }

        /// <summary>
        /// Берет разность от двух вычетов
        /// </summary>
        public static int Substract(int a, int b, int order)
        {
            Abduction(ref a, order);
            Abduction(ref b, order);
            return Sum(a, Inversed(b, order), order);
        }

        /// <summary>
        /// Ищет модулю двух вычетов
        /// </summary>
        public static int Modulus(int a, int b, int order)
        {
            Abduction(ref a, order);
            Abduction(ref b, order);
            if (b == 0)
                throw new DivideByZeroException("Нельзя брать модуль по модулю ноль, даже в вычетах");
            return Substract(a, Multiply(b,
                Divide(a, b, order), order), order); // выполним композицию 3-х операций (что есть модуль)
        }

        /// <summary>
        /// Делятся вычеты друг на друга;
        /// </summary>
        public static int Divide(int a, int b, int order)
        {
            Abduction(ref a, order);
            Abduction(ref b, order);
            if (b == 0)
                throw new DivideByZeroException("Нельзя делить на ноль, даже в вычетах");
            List<int> res = NumbersGCD(b, order);
            return Multiply(a, res[1], order);
        }

        /// <summary>
        /// Не забыть, что на выходе нужно применить Abduction к коэффициентам.
        /// </summary>
        /// <returns>Возвращает список из НОД'а, коэффициенте при a, коэффициенте при b</returns>
        public static List<int> NumbersGCD(int a, int b)
        {
            int x = 0, y = 0; // Коэффициенты при данных выражениях
            if (a == 0) // базис рекурсии
            {
                x = 0; y = 1;
                return new List<int> { b, x, y };
            }
            int x1, y1;
            List<int> ls = NumbersGCD(b % a, a);
            x1 = ls[1];
            y1 = ls[2];
            x = y1 - (b / a) * x1; // Применяем формулу расширенного алгоритма Евклида
            y = x1;
            return new List<int> { ls[0], x, y };
        }

        #endregion

        // Здесь описываются методы, позволяющие оперировать многочленами
        #region Operations in the field on polynomials

        /// <summary>
        /// Приводим многочлен к нормальному виду
        /// </summary>
        public static void Abduction(ref Polynomial p, int order)
        {
            for (int i = 0; i <= p.Dimension; i++)
            {
                int a = p[i];
                Abduction(ref a, order);
                p[i] = a;
            }
            p = CheckIt(p);
        }

        /// <summary>
        /// Деление многочленов
        /// </summary>
        public static Polynomial Divide(Polynomial p1, Polynomial p2, int order)
        {
            Polynomial res = new Polynomial(); // Локальная переменная, обозначающая результат

            Abduction(ref p1, order);
            Abduction(ref p2, order);

            if (p2 == Polynomial.Zero || p2[0] == 0)
                throw new DivideByZeroException("Попытка деления на 0.");

            if (p2.CoefList.Count == 1)
            {
                int inversedNumber = NumbersGCD(p2[0], order)[1];
                for (int i = 0; i < p1.CoefList.Count; i++)
                    res.Add(Multiply(p1[i], inversedNumber, order));
                return res;
            }

            if (p1.Dimension < p2.Dimension) // Если степень делимого меньше степении делителя, то ответ = 0
                return new Polynomial(new int[1] { 0 });

            // Заведем переменную, обозначающую промежуточное делимое
            Polynomial temp = new Polynomial(p1.CoefList);
            // Алгоритм работает до тех пор, пока степень делимого не меньше, чем степень делителя
            while (temp.Dimension >= p2.Dimension)
            {
                int coef = Divide(temp[0], p2[0], order); // Коэффициент для сокращения старшей степени
                int a = temp.Dimension;
                for (int i = 0; i <= p2.Dimension; i++)
                { // В этом цикле уже происходит сдвиг
                    int x = temp[i] - coef * p2[i];
                    Abduction(ref x, order);
                    temp[i] = x;
                }
                Abduction(ref temp, order); // отбросим впереди стоящие нули
                int b = temp.Dimension;
                res.Add(coef);
                for (int i = 0; i < a - b - 1 && res.Dimension < p1.Dimension - p2.Dimension; i++)
                    res.Add(0); // Нулями заполним промежуток в частном.
            }

            return res;
        }

        /// <summary>
        /// Взятие многочлена по модулю
        /// </summary>
        public static Polynomial Modulus(Polynomial p1, Polynomial p2, int order)
        {
            Abduction(ref p1, order);
            Abduction(ref p2, order);
            if (p2 == Polynomial.Zero || p2[0] == 0)
                throw new DivideByZeroException("Попытка деления на ноль.");
            if (p2.CoefList.Count == 1)
                return new Polynomial(0);
            Polynomial copyFirst = new Polynomial(p1.CoefList); // Копируем первый многочлен
            Polynomial d = Divide(p1, p2, order); // Делим первый на второй
            Polynomial m = Multiply(p2, d, order); // Умножаем второй на частное от деления
            Polynomial r = Substract(copyFirst, m, order); // Возвращаем разницу
            return r;
        }

        /// <summary>
        /// Алгоритм сложения двух многочленов
        /// </summary>
        public static Polynomial Sum(Polynomial p1, Polynomial p2, int order)
        {
            Polynomial p3 = p1 + p2;
            Abduction(ref p3, order);
            return p3;
        }

        /// <summary>
        /// Алгоритм вычитания двух многочленов
        /// </summary>
        public static Polynomial Substract(Polynomial p1, Polynomial p2, int order)
        {
            // Изначальное предположение, что degP1 >= degP2
            bool flag = false; // Показывает, применили ли мы swap
            if (p1.Dimension < p2.Dimension)
            {
                flag = true;
                Swap(ref p1, ref p2);
            }

            Polynomial res = new Polynomial(p1.CoefList);
            for (int i = res.Dimension; i >= 0; i--)
            {
                if (res.Dimension - i <= p2.Dimension)
                    res[i] -= p2[i - (p1.Dimension - p2.Dimension)];
                int x = res[i];
                Abduction(ref x, order);
                res[i] = x;
            }
            if (flag)
            {
                for (int i = 0; i < res.CoefList.Count; i++)
                    res[i] = (res[i] != 0) ? order - res[i] : 0;
            }
            return res;
        }

        /// <summary>
        /// Алгоритм быстрого возведения в степень многочлена в конечном поле;
        /// </summary>
        public static Polynomial Power(Polynomial a, int c, int order, Polynomial mod)
        {
            if (c < 0)
                throw new ArgumentOutOfRangeException("Значение степени должно быть неотрицательным.");
            // определим базис рекурсии
            if (c == 0)
                return new Polynomial(new List<int> { 1 });

            Polynomial last = Power(a, c / 2, order, mod);
            if ((c % 2) == 0)
            {
                Polynomial squareOfLast = Multiply(last, last, order);
                Polynomial res = Modulus(squareOfLast, mod, order);
                return res;
            }
            else
            {
                Polynomial firstMultiplication = Multiply(a, last, order); // перемножение a * last^2
                Polynomial canonicalView = Modulus(firstMultiplication, mod, order);
                Polynomial secondMultiplication = Multiply(canonicalView, last, order);
                Polynomial res = Modulus(secondMultiplication, mod, order);
                return res;
            }
        }

        /// <summary>
        /// Алгоритм умножения двух многочленов
        /// </summary>
        public static Polynomial Multiply(Polynomial p1, Polynomial p2, int order)
        {
            Abduction(ref p1, order);
            Abduction(ref p2, order);
            Polynomial res = p1 * p2;
            Abduction(ref res, order);
            return res;
        }

        #endregion

        /// <summary>
        /// Алгоритм Евклида поиска НОД
        /// </summary>
        /// <returns>Возвращает список из 3-х элементов: {НОД, u, v}</returns>
        public static List<Polynomial> PolynomialGCD(Polynomial p1, Polynomial p2, int order)
        {
            Polynomial x = new Polynomial(), y = new Polynomial();
            Abduction(ref p1, order);
            Abduction(ref p2, order);
            if (p1 == Polynomial.Zero || p1.CoefList[0] == 0)
            { // Базис рекурсии
                x.Add(0);
                y.Add(1);
                return new List<Polynomial> { p2, x, y };
            }
            Polynomial x1 = new Polynomial(), y1 = new Polynomial(); // Результаты рекурсивного запуска
            Polynomial mod = Modulus(p2, p1, order);
            List<Polynomial> ls = PolynomialGCD(mod, p1, order);
            x1 = ls[1];
            y1 = ls[2];
            Polynomial p1_copy = new Polynomial(p1.CoefList);
            Polynomial p2_copy = new Polynomial(p2.CoefList);
            Polynomial d = Divide(p2_copy, p1_copy, order); // Применение формулы расширенного алгоритма Евклида
            Polynomial m = Multiply(d, x1, order);
            x = Substract(y1, m, order);
            y = x1;
            return new List<Polynomial> { ls[0], x, y };
        }

        /// <summary>
        /// Генератор случайного многочлена в заданном конечном поле
        /// </summary>
        public static Polynomial RandomPolynomial(int order, int dim)
        {
            Polynomial res = new Polynomial(dim / 2);
            res[0] = 1;
            // Пусть степень многочлена меньше, чем длина входной строки, вдвое.
            for (int i = 1; i <= dim / 2; i++)
                res[i] = randomizer.Next(0, order); // новый коэффициент многочлена - очередной вычет
            return res;
        }

        /// <summary>
        /// Метод изменяет входной многочлен, отбрасывая впереди стоящие нули;
        /// </summary>
        public static Polynomial CheckIt(Polynomial p)
        {
            if (p.CoefList.Count == 0)
                return new Polynomial(0);

            int ctr = 0;
            while (p[ctr] == 0 && ctr++ != p.Dimension) ; // проверка на нулевые элементы

            if (ctr > p.Dimension)
                return new Polynomial(0);

            Polynomial res = new Polynomial(p.Dimension - ctr);
            for (int i = 0; i <= p.Dimension - ctr; i++) // Заполнит нулем, если все пусто
                res[i] = p[i + ctr];
            return res;
        }

        /// <summary>
        /// Декодируем многочлен. Возвращаем строку
        /// </summary>
        public static string PolynomialToString(Polynomial poly)
        {
            string res = "";
            for (int i = 0; i < poly.CoefList.Count; i++)
            {
                int k = 0;
                foreach (KeyValuePair<char, int> pr in codesOfSymbols)
                {
                    if (poly[i] == pr.Value)
                    {
                        res += pr.Key;
                        break;
                    }
                    ++k;
                }
                if (k == codesOfSymbols.Count)
                    res += '@'; // Показывает, что для вывода это сообщение не очень пригодно.
                                //throw new IndexOutOfRangeException("Произошла ошибка.");
            }
            return res;
        }

        public static string PolynomialToLatexString(Polynomial p)
        {
            if (p.CoefList.Count == 0 || p[0] == 0)
                return "0";
            string res = "";
            for (int i = 0; i < p.Dimension; i++)
            {
                if (p[i] == 0)
                    continue;
                string digit = (p[i] != 1) ? Convert.ToString(p[p.Dimension]) : "";
                if (p.Dimension - i != 1)
                    res += $"{digit}x^{p.Dimension - i}+";
                else
                    res += $"{digit}x+";
            }
            if (p[p.Dimension] != 0)
                res += Convert.ToString(p[p.Dimension]);
            else
                res = res.Substring(0, res.Length - 1);
            //res = "$" + res + "$";
            return res;
        }

        /// <summary>
        /// Кодируем строку. Возвращаем многочлен.
        /// </summary>
        public static Polynomial StringToPolynomial(string message)
        {
            Polynomial res = new Polynomial(message.Length - 1);
            for (int i = 0; i < message.Length; i++)
                res[i] = codesOfSymbols[message[i]];
            return res;
        }

        public static bool IsSimilar(double a, double b)
        {
            double eps = 1e-8;
            return (Math.Abs(a - b) < eps);
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}
