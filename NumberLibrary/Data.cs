using System;
using System.Numerics;
using System.Collections.Generic;

namespace NumberLibrary
{ 
    public static class Data
    {
        public static Dictionary<BigInteger, bool> Primes; // Словарь простых чисел, доступ за O(logn)
        
        public static BigInteger Modulus { get; set; } // порядок поля вычетов

        public static BigInteger Order { get; set; } // порядок (используется для кодирования/декодирования)
        // в этой реализации порядок не обязан быть простым числом

        public static BigInteger PublicNumber { get; set; } // публичное число g

        public static string Message { get; set; } // Сохраняем сообщение для последующей обработки

        public static Dictionary<char, int> codesOfSymbols; // Коды символов в конечном поле

        private static BigInteger Quantity { get; set; } // Обозначает значение p^m;

        private static Random Randomizer = new Random();

        public static void ClearData()
        {
            Modulus = 0;
            Order = 0;
            Message = "";
            codesOfSymbols = new Dictionary<char, int>();
            Quantity = 0;
        }

        /// <summary>
        /// Устанавливает значение порядка поля
        /// </summary>
        public static void SetModulus()
        {
            BigInteger ctr = Quantity;
            while (ctr < (BigInteger)Primes.Count)
            {
                if (Primes[ctr])
                {
                    Modulus = ctr;
                    break;
                }
                ++ctr;
            }
        }

        /// <summary>
        /// Метод создает таблицу простых чисел от 1..n;
        /// </summary>
        /// <param name="n">Входное число , в промежутке [0..n] ищем простые числа</param>
        public static void Eratospenes(BigInteger n)
        {
            Primes = new Dictionary<BigInteger, bool>();
            for (BigInteger i = 2; i <= n; i++)
                Primes.Add(i, true);

            Primes[0] = Primes[1] = false;
            for (BigInteger i = 2; i * i <= n; ++i)
                if (Primes[i])
                    for (BigInteger j = i * i; j <= n; j += i)
                        Primes[j] = false;
        }

        private static BigInteger FindSqrt(BigInteger n)
        {
            BigInteger res = 0;
            for (; res * res <= n; res++) ;
            return res;
        } 

        /// <summary>
        /// Метод возведения в степень по модулю
        /// </summary>
        public static BigInteger PowerOnModulus(BigInteger a, int b)
        {
            if (b == 0)
                return 1;
            if (b % 2 == 1)
            {
                return (a * PowerOnModulus(a, b - 1)) % Modulus;
            }
            else
            {
                BigInteger res = PowerOnModulus(a, b / 2) % Modulus;
                return (res * res) % Modulus;
            }
        }

        /// <summary>
        /// Генерирует случайное число в диапазонах на BigInteger;
        /// </summary>
        public static BigInteger BigIntegerRandom(BigInteger min, BigInteger max)
        {
            byte[] buf = new byte[8];
            Randomizer.NextBytes(buf);
            BigInteger BigIntegerRand = BitConverter.ToUInt64(buf, 0);

            return ((BigInteger)Math.Abs((float)(BigIntegerRand % (max - min))) + min);
        }

        /// <summary>
        /// Для начала работы требуется передать сообщение в этот метод
        /// </summary>
        public static void CreateData(string message)
        {
            Message = message; // Сообщение сохраняется для дальнейших действий
            Client.CommonKey = 0; // Общий ключ Алисы и Боба

            BigInteger ctr = 0; // в этом счетчике хранится количество различных символов входной строки
                                // Чтобы записать в счетчик хорошее значение, достаточно использовать HashSet
            HashSet<char> cash = new HashSet<char>(message);
            ctr = cash.Count;
            if (ctr > 500)
                throw new ArgumentOutOfRangeException("Слишком много различных символов");

            Order = ctr; // задаем порядок (уже не простое число)

            Quantity = (BigInteger)Math.Pow((double)Order, 5); // Находим число различных слов длины 5
            Eratospenes(Quantity + (int)1e3); // некоторая константа, обозначающая промежуток поиска
            SetModulus(); // Благодаря решету Эратосфена можно найти модуль

            CreateCodes(); // Можно установить кодировки

            PublicNumber = BigIntegerRandom(Modulus / 2, Modulus - 1);
        }

        /// <summary>
        /// Метод заполняет таблицу с кодами
        /// </summary>
        public static void CreateCodes()
        {

            codesOfSymbols = new Dictionary<char, int>();
            char[] cp = new char[Message.Length];
            int counter = 0; // Счетчик используется для установления соответствия между символами и их кодами 
            Message.CopyTo(0, cp, 0, Message.Length);
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
        /// Метод кодирует строку. Получая на вход строку, составляет вычет по следующему правилу:
        /// Вычисляет выражение P(p), где p - порядок в поле, P(x) = a_0 * x^n + ... + a_n-1 * x + a_n
        /// a_i = кодировка i-го символа
        /// </summary>
        public static BigInteger Encode(string s)
        {
            if (s.Length > 5)
                throw new ArgumentOutOfRangeException("Передано слшиком длинное сообщение.");
            BigInteger res = 0;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                BigInteger pow = PowerOnModulus(Order, s.Length - i - 1); // Степень мощности алфавита
                BigInteger exp = codesOfSymbols[s[i]] * pow;
                Abduction(ref exp, Modulus);
                BigInteger add = res + exp; // Необходимый результат
                Abduction(ref add, Modulus);
                res = add;
            }
            return res;
        }

        public static string EncodeAndConvert(string s)
        {
            BigInteger cash = 0;
            bool flag = false;
            string res = "";
            if (s.Length == 1)
                return $"{codesOfSymbols[s[0]]}";
            for (int i = s.Length - 1; i >= 0; i--)
            {
                BigInteger pow = PowerOnModulus(Order, s.Length - i - 1);
                BigInteger exp = codesOfSymbols[s[i]] * pow;
                if (codesOfSymbols[s[i]] != 0) // Добавить, если не 0
                {
                    if (s.Length - i - 1 != 1)
                        res += $"{codesOfSymbols[s[i]]}*{Order}^{s.Length - i - 1} +";
                    else
                        res += $"{codesOfSymbols[s[i]]}*{Order} +";
                }
                else
                    flag |= (s.Length - i - 1 == 0); // Проверка на то, является ли нулевой член 0
                Abduction(ref exp, Modulus);
                BigInteger add = cash + exp;
                Abduction(ref add, Modulus);
                cash = add;
            }
            if (!flag)
            {
                res = res.Substring(res.IndexOf('+') + 1);
                res = $"{codesOfSymbols[s[s.Length - 1]]} + " + res;
            }
            res = res.Substring(0, res.Length - 1);
            return res.Length == 0 ? "0" : res;
        }

        /// <summary>
        /// Метод декодирует вычет.
        /// Т.к. операции производятся по модулю порядка, то применять Abduction необязательно.
        /// </summary>
        public static string Decode(BigInteger n)
        {
            string res = "";
            BigInteger pk = 1;
            for (int i = 0; i < 5; i++)
            {
                BigInteger a = n % (pk * Order);
                int digit = (int)(a / pk);
                res += FindSymbol(digit);
                pk *= Order;
            }
            return res;
        }

        /// <summary>
        /// Вспомогательный метод для поиска символа с определенным кодом
        /// </summary>
        public static char FindSymbol(int n)
        {
            foreach (KeyValuePair<char, int> pr in codesOfSymbols)
                if (pr.Value == n)
                    return pr.Key;
            throw new ArgumentOutOfRangeException("Символ не обнаружен в таблице");
        }

        /// <summary>
        /// Выполняет приведение вычета к привычному виду 
        /// </summary>
        public static void Abduction(ref BigInteger a, BigInteger order)
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
        /// Расширенный Алгоритм Евклида для больших вычетов
        /// </summary>
        public static List<BigInteger> Euclidus(BigInteger a, BigInteger b)
        {
            BigInteger x = 0, y = 0;
            if (a == 0)
            {
                x = 0; y = 1;
                return new List<BigInteger> { b, x, y };
            }
            BigInteger x1, y1;
            List<BigInteger> ls = Euclidus(b % a, a);
            x1 = ls[1];
            y1 = ls[2];

            // Чтобы вычислить новое значение x, нужно повозиться с модулями
            // Выражение для х: y1 - (b div a) * x1;

            BigInteger divisor = (b / a); // (b div a)
            Abduction(ref divisor, Modulus);

            BigInteger mlt = divisor * x1; // (b div a) * x1
            Abduction(ref mlt, Modulus);

            BigInteger res = y1 - mlt; // y1 - (b div a) * x1
            Abduction(ref res, Modulus);

            x = res;
            y = x1;
            return new List<BigInteger> { ls[0], x, y };
        }
    }
}
