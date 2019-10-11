using System;
using System.Collections.Generic;

namespace PolynomialLibrary
{
    public class Account
    {
        // *** Секретное значение каждого аккаунта сервера ***
        public int privatePower; // секретная степень (a)

        // *** Пара значений, которые позволяют получить исходный вид сообщения ***
        public Polynomial EncryptedMessage { get; set; } // зашифрованное сообщение
        public Polynomial Mask { get; set; } // маска

        public Polynomial publicPolynomial; // открытый ключ адресата / адресанта (g^a)
        public Polynomial MessagePolynomial; // Принятое секретное сообщение

        public string MessagePowerPolynomial; // хранится запись возведения многочлена в степень
        // скобки, которые используются в CipherWindow2

        /// <summary>
        /// Публичный конструктор, конструирующий закрытые приватные ключи.
        /// </summary>
        /// <param name="field">Поле, в котором будут производиться все необходимые операции</param>
        public Account()
        {

            privatePower = Data.randomizer.Next(10, 50); // вскоре нужно задать промежутки
                                                          // Запишем и сохраним приватный многочлен в поле класса, чтобы не возводить в степень многократно
            MessagePowerPolynomial = "g"; // "Базис" этого отображения степени - просто g;
            Polynomial temp = PowPublicPolynomial(privatePower); // Временная переменная, возведение в степень без применения операции взятия по модулю 
            publicPolynomial = Data.Modulus(temp, Data.FieldOfCryptosystem.Modulus, Data.FieldOfCryptosystem.Order);
            Data.Abduction(ref publicPolynomial, Data.FieldOfCryptosystem.Order);
            // изначально неизестно, какие данные этому аккаунту будут передаваться
            // поэтому выделим память для переменных, и на этом ограничимся
            // Нужно проинициализировать:
            Mask = new Polynomial(); // Маска
            EncryptedMessage = new Polynomial(); // Принятое секретное сообщение
            MessagePolynomial = new Polynomial();
        }
        
        public List<Polynomial> BasicPowers = new List<Polynomial>() { new Polynomial(new List<int> { 1 }) };
        // Некоторые необходимые сведения об аккаунте для вывода информации подсчета открытого ключа на экран

        private Polynomial PowPublicPolynomial(int c)
        {
            if (c < 0)
                throw new ArgumentOutOfRangeException("Значение степени должно быть неотрицательным.");

            // определим базис рекурсии
            if (c == 0)
                return new Polynomial(new List<int> { 1 });


            Polynomial res = new Polynomial(); // Результирующий многочлен
            Polynomial last = PowPublicPolynomial(c / 2); // храним рекурсивно вычисленный результат
            if ((c % 2) == 0)
            {
                res = Data.Multiply(last, last, Data.FieldOfCryptosystem.Order);
                MessagePowerPolynomial = "(" + MessagePowerPolynomial + @")^{2}"; // нужно дополнить представление в скобках до нужного вида
                res = Data.Modulus(res, Data.FieldOfCryptosystem.Modulus, Data.FieldOfCryptosystem.Order);
                BasicPowers.Add(res); // BasicPowers хранит текущее представление многочлена
                return res;
            }
            else
            {
                Polynomial squareOfLast = Data.Multiply(last, last, Data.FieldOfCryptosystem.Order); // храним квадрат последнего результата
                res = Data.Multiply(Data.FieldOfCryptosystem.OpenPolynomial, last, Data.FieldOfCryptosystem.Order); // результатом является a * last^2
                if (c != 1)
                    MessagePowerPolynomial = "g*(" + MessagePowerPolynomial + @")^{2}"; // нужно дополнить представление в скобках до нужного вида
                res = Data.Modulus(res, Data.FieldOfCryptosystem.Modulus, Data.FieldOfCryptosystem.Order); // Необходимо взять результат по модулю
                BasicPowers.Add(res); // BasicPowers хранит текущее представление многочлена
                return res;
            }
        }

        /// <summary>
        /// Создается многочлен g^a, являющийся открытым ключом;
        /// </summary>
        /// <param name="power">Степень a</param>
        private void CreatePublicPolynomial(int power)
        {
            Polynomial temp = PowPublicPolynomial(power); // временное возведение в степень (без взятия модуля)
            publicPolynomial = Data.Modulus(temp, Data.FieldOfCryptosystem.Modulus,
                Data.FieldOfCryptosystem.Order); // приведение к нормальному виду
        }

        /// <summary>
        /// Сообщение, которое выбирается отправителем, должно быть конвертировано в многочлен
        /// </summary>
        public void CreateMessage(string message)
        {
            MessagePolynomial = Data.StringToPolynomial(message);
        }

        /// <summary>
        /// Передача данных от адресата к адресанту, т.е. необходимая пара (шифр, маска).
        /// Именно отправление сообщения!
        /// </summary>
        /// <param name="getter">Адресат, получатель</param>
        public void TransportTo(Account getter)
        {
            int order = Data.FieldOfCryptosystem.Order; // Запишем для упрощения записи
            Polynomial mod = new Polynomial(Data.FieldOfCryptosystem.Modulus.CoefList);

            Polynomial g = new Polynomial(Data.FieldOfCryptosystem.OpenPolynomial.CoefList); // Копируем открытый многочлен
            // нужно установить значение полученной пары по явной формуле
            Polynomial commonKey = Data.Power(getter.publicPolynomial, privatePower, order, mod); // общий ключ (т.е. g^ab)

            Polynomial encryptedMessage = Data.Multiply(MessagePolynomial, commonKey, order); // Получаем сообщение без применения операции взятия по модулю
            Polynomial canonicalEncryptedMessage = Data.Modulus(encryptedMessage, mod, order); // Приведение к нормальному виду
            Data.Abduction(ref canonicalEncryptedMessage, order);
            getter.EncryptedMessage = canonicalEncryptedMessage;
            getter.Mask = publicPolynomial; // Маска - открытый многочлен отправителя
        }

        public static string CommonKey { get; set; }
        public static Polynomial InversedPower { get; set; } // Обратный к общему многочлен

        /// <summary>
        /// Функция, выполняющая дешифрование сообщения
        /// </summary>
        /// <returns>Возвращает исходную строку</returns>
        public Polynomial Decipher()
        {
            int order = Data.FieldOfCryptosystem.Order;
            Polynomial mod = Data.FieldOfCryptosystem.Modulus;
            Polynomial commonKey = Data.Power(Mask, privatePower, order, mod); // возведение в степень маски
            Polynomial inversedElement = Polynomial.InversedElement(commonKey); // Используем обратный элемент в общему
            
            // Обратный многочлен необходимо сделать таким, что НОД с общим = 1, т.к. из взаимной простоты многочленов следует, что НОД=с=const
            Polynomial gcd = Data.PolynomialGCD(commonKey, mod, order)[0];
            inversedElement = Data.Divide(inversedElement, gcd, order);
            InversedPower = inversedElement;
            Polynomial decipheredPart = Data.Multiply(inversedElement, EncryptedMessage, order); // Получаем дешифрованный вид
            Polynomial deciphredCanonicalPart = Data.Modulus(decipheredPart, mod, order); // Приводим к нормальному виду
            Data.Abduction(ref deciphredCanonicalPart, order);
            return deciphredCanonicalPart;
        }
    }
}
