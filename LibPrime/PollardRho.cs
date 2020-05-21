using System;
using System.Numerics;
using System.Security.Cryptography;

namespace LibPrime
{
    public class PollardRho_
    {
        public BigInteger CustomFunct(BigInteger param, BigInteger mod)//Custom number generator
        {
            return ((param * param + 1) % mod);
        }

        public BigInteger PollardRho(BigInteger number)
        {
            BigInteger a = 2, b = 2, tmp;// initial starting values
            while (true)
            {
                a = CustomFunct(a, number);//get first number
                b = CustomFunct(CustomFunct(b, number), number);//get second double run custom func
                var w = (a - b) < 0 ? (b - a) : (a - b);
                tmp = GCD(w, number);// if a =  b mod d(divisor one of the divisors of our number) then a-b is multiple of d and number is multiple of d 
                if (tmp > 1)
                    break;
            }
            return tmp;

        }
        public BigInteger GCD(BigInteger x, BigInteger y)//Greatest Common Divisor
        {
            if (x == 0)//Find it? return
                return y;
            if (y == 0)//Find it? return
                return x;
            x = x % y;// take the mod of the large val to small one if the order is wrong in the next recursive loop it will be correct
            return GCD(y, x);//Parameters changed place
        }
    }
}