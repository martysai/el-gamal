using System;
using System.Collections.Generic;

namespace PolynomialLibrary
{
    public class Polynomial
    {
        public List<int> CoefList { get; set; } // если многочлен представим в виде a0 + a1x + ... + an * x^n,
                                                // то хранятся данные в виде (an, ..., a0);

        private static Polynomial zero; // Закрытый нулевой многочлен;

        public static Polynomial Zero { get => zero; }

        public int Dimension { get => CoefList.Count - 1; } // Свойство возвращает размерность многочлена

        static Polynomial()
        {
            zero = new Polynomial(0); // Создаст нулевой многочлен
        }

        public Polynomial()
        {
            CoefList = new List<int>();
        }

        public Polynomial(int dim)
        {
            CoefList = new List<int>(dim + 1);
            for (int i = 0; i <= dim; i++)
                CoefList.Add(0);
        }

        public Polynomial(ICollection<int> input)
        {
            CoefList = new List<int>(input.Count);
            foreach (var elem in input)
            {
                Add(elem);
            }
        }

        public int this[int ind]
        {
            get => CoefList[ind];
            set => CoefList[ind] = value;
        }

        /// <summary>
        /// Вычисляет значение многочлена в заданной точке
        /// </summary>
        public int GetValueOf(int x)
        {
            int res = 0;
            for (int i = 0; i <= Dimension; i++)
                res += CoefList[i] * (int)Math.Pow(x, Dimension - i);
            return res;
        }

        public void Add(int x)
        {
            CoefList.Add(x);
        }

        /// <summary>
        /// Используем метод быстрого возведения в квадрат
        /// </summary>
        /// <returns>Возвращает степень многочлена по определенному модулю</returns>
        public static Polynomial Power(Polynomial a, int c, int order)
        {
            if (c < 0)
                throw new ArgumentOutOfRangeException("Значение степени должно быть неотрицательным.");

            Polynomial temp;

            // определим базис рекурсии
            if (c == 0)
                return new Polynomial(new List<int> { 1 });

            temp = Power(a, c / 2, order);
            if ((c % 2) == 0)
                return Data.Multiply(temp, temp, order);
            else
            {
                Polynomial m = Data.Multiply(a, temp, order);
                return Data.Multiply(m, temp, order);
            }
        }

        /// <summary>
        /// Возвращает обратный к данному многочлену в поле
        /// </summary>
        public static Polynomial InversedElement(Polynomial inputPolynomial)
        {
            Polynomial mod = Data.FieldOfCryptosystem.Modulus;
            List<Polynomial> ls = Data.PolynomialGCD(inputPolynomial, mod, Data.FieldOfCryptosystem.Order);
            return ls[1];
        }

        // Здесь описаны операции над многочленами
        #region Operations

        public static Polynomial operator +(Polynomial p1, Polynomial p2)
        {
            int mx = Math.Max(p1.Dimension, p2.Dimension);
            Polynomial res = new Polynomial(mx);
            for (int i = 0; i <= mx; i++)
            {
                if (i <= p1.Dimension)
                    res[i] += p1[i];
                if (i <= p2.Dimension)
                    res[i] += p2[i];
            }
            return res;
        }

        public static Polynomial operator *(Polynomial p1, Polynomial p2)
        {
            Polynomial res = new Polynomial(p1.Dimension + p2.Dimension);

            for (int i = 0; i <= p1.Dimension; i++)
                for (int j = 0; j <= p2.Dimension; j++)
                {
                    res[i + j] += p1[i] * p2[j];
                }
            return res;
        }

        public static Polynomial operator /(Polynomial p1, Polynomial p2)
        {
            Polynomial res = new Polynomial(); // Локальная переменная, обозначающая результат

            if (p1.Dimension < p2.Dimension) // Если степень делимого меньше степении делителя, то ответ = 0
                return new Polynomial(new int[1] { 0 });

            // Заведем переменную, обозначающую промежуточное делимое
            Polynomial temp = new Polynomial(p1.CoefList);
            // Алгоритм работает до тех пор, пока степень делимого не меньше, чем степень делителя
            while (temp.Dimension >= p2.Dimension)
            {
                int coef = temp[0] / p2[0]; // Коэффициент для сокращения старшей степени
                int a = temp.Dimension;
                for (int i = 0; i <= p2.Dimension; i++) // В этом цикле уже происходит сдвиг
                    temp[i] -= coef * p2[i];
                temp = Data.CheckIt(temp); // отбросим впереди стоящие нули
                int b = temp.Dimension;
                res.Add(coef);
                for (int i = 0; i < a - b - 1 && res.Dimension < p1.Dimension - p2.Dimension; i++) // Нулями заполним промежуток в частном.
                    res.Add(0);
            }

            return res;
        }

        public static Polynomial operator -(Polynomial p)
        {
            Polynomial res = new Polynomial(p.Dimension);
            for (int i = 0; i <= res.Dimension; i++)
                res[i] = (-p[i]);
            return res;
        }

        public static Polynomial operator -(Polynomial p1, Polynomial p2)
        {
            return p1 + (-p2);
        }

        public static Polynomial operator %(Polynomial p1, Polynomial p2)
        {
            return p1 - (p1 / p2) * p2;
        }

        public static bool operator ==(Polynomial p1, Polynomial p2)
        {
            p1 = Data.CheckIt(p1);
            p2 = Data.CheckIt(p2);
            return p1.CoefList == p2.CoefList;
        }

        public static bool operator !=(Polynomial p1, Polynomial p2)
        {
            return !(p1 == p2);
        }

        #endregion

        /// <summary>
        /// Переопределенный метод ToString(). Заметим, что вывод строится в формате
        /// an*x^n + ... + a1*x + a0
        /// </summary>
        /// <returns>Возвращает необходимую строку</returns>
        public override string ToString()
        {
            string res = "";
            int counter = Dimension; // счетчик, позволяющий выводить текущую степень
            foreach (int coef in CoefList)
            {
                if (!Data.IsSimilar(coef, 0))
                    res += string.Format("{0}*x^{1}\t", coef, counter);
                --counter;
            }
            return res.Length == 0 ? "0" : res;
        }
    }
}
