using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPCA;
using MATRIX;

namespace STATISTICS
{
    public class Statistics
    {
        //Среднее значение
        public static double Average(double[] massive)
        {
            double mean = 0;
            for (int i = 0; i < massive.Length; i++)
                mean += massive[i];
            return mean / massive.Length;
        }
        public static double[] Average(Matrix massive)
        {
            double[] res = new double[massive.getRows()];
            for(int i=0;i<res.Length;i++)
                res[i] = Average(massive.getRow(i));
            return res;
        }
        //несмещенная Дисперсия
        public static double Variance(double[] massive)
        {
            double variance = 0;
            double mean = Statistics.Average(massive);

            foreach (double d in massive)
                variance += Math.Pow((d - mean), 2);
            return variance / (massive.Length - 1);
        }
        //выборочная дисперсия
        public static double offsetVariance(double[] massive)
        {
            double variance = 0;
            double mean = Statistics.Average(massive);

            foreach (double d in massive)
                variance += Math.Pow((d - mean), 2);
            return variance / (massive.Length);
        }

        //Стандартное отклонение
        public static double StandardDeviation(double[] x) { return Math.Sqrt(Variance(x)); }
        public static double[] StandardDeviation(Matrix matrix)
        {
            double[] res = new double[matrix.getRows()];
            for(int i=0;i<res.Length;i++)
                res[i] = StandardDeviation(matrix.getRow(i));
            return res;
        }

        //Ковариация
        public static double Covarience(double[] x, double[] y)
        {
            if (x.Length != y.Length) throw new ArgumentException("values must be the same length");

            double covarience = 0;
            double xAverage = Statistics.Average(x);
            double yAverage = Statistics.Average(y);

            for (int i = 0; i < x.Length; i++)
                for (int j = 0; j < x.Length; j++)
                    covarience += (x[i] - xAverage) * (y[i] - yAverage) / x.Length;
            return covarience / (x.Length-1);
        }
        //Коэффициент корреляции
        public static double Correlation(double[] x, double[] y)
        {
            if (x.Length != y.Length) throw new ArgumentException("values must be the same length");
            return Covarience(x, y) / (StandardDeviation(x) * StandardDeviation(y));
        }
        //Матрица корреляций
        public static Matrix getCorrelationMatrix(Matrix inputData)
        {
            Matrix covarianceMatrix = new Matrix(inputData.getRows(), inputData.getRows());

            for (int i = 0; i < covarianceMatrix.getRows(); i++)
            {
                for (int j = 0; j < covarianceMatrix.getColumns(); j++)
                {
                    if (i == j)
                    {
                        covarianceMatrix[i, j] = 1;
                        continue;
                    }
                    else if (j < i)
                    {
                        covarianceMatrix[i, j] = Correlation(inputData.getRow(i), inputData.getRow(j));
                        covarianceMatrix[j, i] = covarianceMatrix[i, j];
                    }
                }
            }
            return covarianceMatrix;
        }
        //Коэффициент детерминации
        public static double Determination(double correlation) { return Math.Pow(correlation, 2); }
    }
}
