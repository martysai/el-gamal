using System;
using System.Collections.Generic;

namespace PolynomialLibrary
{
    public class Field
    {
        public int Dimension { get; set; }
        public int Order { get; set; }

        public Polynomial OpenPolynomial { get; set; } // открытый многочлен для всех пользователей сервера;

        public Polynomial Modulus { get; set; } // Многочлен, относительно которого происходит факторизация;

        /// <summary>
        /// Каждое поле определяется своей мощностью. Достаточно посмотреть на исходное сообщение.
        /// </summary>
        /// <param name="input">Входная строка</param>
        public Field(string input)
        {
            int counter = 0; // в этом счетчике хранится количество различных символов входной строки
                             // Чтобы записать в счетчик хорошее значение, достаточно использовать HashSet
            HashSet<char> cash = new HashSet<char>(input);
            counter = cash.Count;
            
            // Этот счетчик нужен, чтобы найти ближайшее сверху простое число
            int ord = 0;
            Data.FindPrime(counter, ref ord);

            Order = ord;
            Dimension = 5; // Размерность наибольшего многочлена

            OpenPolynomial = Data.RandomPolynomial(Order, Dimension); // Метод найдет хороший многочлен

            Modulus = Data.CreatePrimePolynomial(Order); // Нужно найти идеал, по которому будем факторизовать
        }
    }
}
