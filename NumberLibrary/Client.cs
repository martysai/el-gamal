using System.Collections.Generic;
using System.Numerics;
using System;

namespace NumberLibrary
{
    public class Client
    {

        public int PrivatePower { get; set; } // Закрытый ключ

        public BigInteger OpenNumber { get; set; } // Открытый ключ

        public string SentMessage { get; set; } // Сообщение, которое хотим передать
        public string RecievedMessage { get; set; } // Получаемое сообщение

        public BigInteger Mask { get; set; } // Маска, получаемая от отправителя

        public BigInteger CipheredNumber { get; set; } // Зашифрованное сообщение, полученное от отправителя

        public List<BigInteger> BasicPowers; // промежуточные степени (для быстрого возведения в квадрат)

        private static Random Randomizer = new Random();
        
        public Client()
        {
            PrivatePower = Randomizer.Next(1, 100); // Предполагаемый диапазон закрытого ключа - 1..100
            BasicPowers = new List<BigInteger>();
            BasicPowers.Add(1); // Необходимо заполнить значениями промежуточных величин, в частности, единицей
            OpenNumber = PowerOnModulus(Data.PublicNumber, PrivatePower);
        }

        public static BigInteger CommonKey { get; set; }
        public static BigInteger InversedCommonKey { get; set; }

        public static void CreateCommonKey(BigInteger OpenNumber, int PrivatePower)
        {
            CommonKey = Data.PowerOnModulus(OpenNumber, PrivatePower);
        }

        public static void CreateInversedCommonKey()
        {
            List<BigInteger> dataOfGCD = Data.Euclidus(CommonKey, Data.Modulus); // Получим информацию о р. алгоритме Евклида
            InversedCommonKey = dataOfGCD[1]; // обратная степень числа
        }

        /// <summary>
        /// Алгоритм шифрования и передачи
        /// </summary>
        public void TransportTo(Client getter)
        {
            BigInteger t = Data.Encode(SentMessage); // Закодируем сообщение

            // Передача информации
            getter.CipheredNumber = t * Data.PowerOnModulus(getter.OpenNumber, PrivatePower);
            getter.Mask = OpenNumber;

            BigInteger buffer = getter.CipheredNumber; // Приведем число к модулю
            Data.Abduction(ref buffer, Data.Modulus);
            getter.CipheredNumber = buffer;
        }

        /// <summary>
        /// Алгоритм дешифрования
        /// </summary>
        public void Decipher()
        {
            BigInteger commonPower = Data.PowerOnModulus(Mask, PrivatePower); // Благодаря маске
            // Получаем общий ключ

            List<BigInteger> dataOfGCD = Data.Euclidus(commonPower, Data.Modulus); // Получим информацию о р. алгоритме Евклида
            BigInteger inversedCommonPower = dataOfGCD[1]; // обратная степень числа

            Data.Abduction(ref inversedCommonPower, Data.Modulus);
            BigInteger cipheredNumber = CipheredNumber;
            Data.Abduction(ref cipheredNumber, Data.Modulus);

            BigInteger EncodedMessage = cipheredNumber * inversedCommonPower;
            Data.Abduction(ref EncodedMessage, Data.Modulus);

            RecievedMessage = Data.Decode(EncodedMessage);
        }

        /// <summary>
        /// Метод возведения в степень по модулю, учитывается сохранение степеней для вывода
        /// </summary>
        public BigInteger PowerOnModulus(BigInteger a, int b)
        {
            if (b == 0)
                return 1;
            BigInteger expression = PowerOnModulus(a, b / 2);
            if (b % 2 == 1)
            {
                BigInteger res = (a * expression * expression) % Data.Modulus;
                BasicPowers.Add(res);
                return res;
            }
            else
            {
                BigInteger res = (expression * expression) % Data.Modulus;
                BasicPowers.Add(res);
                return res;
            }
        }
    }
}
