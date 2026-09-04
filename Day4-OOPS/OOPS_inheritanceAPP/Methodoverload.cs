namespace MyCalculations
{
    public class Calculation
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public double Add(double a, double b)
        {
            return a + b;
        }

        public int Add(int a, int b, int c)
        {
            return a + b + c;
        }

        public int Add(int a, int b, int c, int d)
        {
            return a + b + c + d;
        }

        public string Add(string name, string lastname)
        {
            return name + " " + lastname;
        }
        //params array
        public int Add(int num1, int num2, params int[] moreNumbers)
        {
            int sum = num1 + num2;
            for (int i = 0; i < moreNumbers.Length; i++)
            {
                sum += moreNumbers[i];
            }
            return sum;
        }
        
    }
}