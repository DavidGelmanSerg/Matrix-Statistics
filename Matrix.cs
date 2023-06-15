using Accord;
using STATISTICS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using Accord.Math.Decompositions;

namespace MATRIX
{
    public enum MatrixFillConstants
    {
        RANDOM
    }

    public class Matrix
    {

        private double[,] matrix;           //матрица
        private int m;                              //строки
        private int n;                               //столбцы
        public Matrix EigenValues;     //Собственные значения
        public Matrix EigenVectors; // Собственные векторы

        //Конструктор
        public Matrix(int m, int n)
        {
            this.m = m; this.n = n;
            matrix = new double[m, n];
            setToZero();
        }
        public Matrix() { }

        //get и set методы
        public int getRows() { return m; }
        public int getColumns() { return n; }
        public double getElement(int i, int j) { return matrix[i, j]; }
        public double[] getRow(int index)
        {
            double[] res = new double[getColumns()];

            for (int j = 0; j < getColumns(); j++)
                res[j] = getElement(index, j);
            return res;
        }
        public double[] getColumn(int index)
        {
            double[] res = new double[getRows()];

            for (int i = 0; i < getRows(); i++)
                res[i] = getElement(i, index);
            return res;
        }
        public void setRow(double[]row,int index)
        {
            for(int i=0; i<getColumns(); i++) {
                matrix[index, i] = row[i];
            }
        }
        public void setColumn(double[]column,int index)
        {
            for (int i = 0; i < getColumns(); i++)
            {
                matrix[i, index] = column[i];
            }
        }
        public void setRows(int rows) { m = rows; }
        public void setColumns(int cols) { n = cols; }

        //Индексатор
        public double this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }

        private void setToZero()
        {
            for (int i = 0; i < getRows(); i++)
                for (int j = 0; j < getColumns(); j++)
                    matrix[i, j] = 0;
        }

        //операции
        //Умножение матрицы на число
        public static Matrix operator *(Matrix operand, double value)
        {
            Matrix res = new Matrix(operand.m, operand.n);

            for (int i = 0; i < operand.getRows(); i++)
                for (int j = 0; j < operand.getColumns(); j++)
                    res[i, j] = operand[i, j] * value;
            return res;
        }
        //Умножение числа на матрицу
        public static Matrix operator *(double value, Matrix operand)
        {
            Matrix res = new Matrix(operand.m, operand.n);

            for (int i = 0; i < operand.getRows(); i++)
                for (int j = 0; j < operand.getColumns(); j++)
                    res[i, j] = operand[i, j] * value;
            return res;
        }
        //Умножение матрицы на матрицу
        public static Matrix operator *(Matrix operand1, Matrix operand2)
        {
            if (operand1.getColumns() != operand2.getRows())
            {
                throw new ArgumentException("matrixes can not be multiplied");
            }
            Matrix res = new Matrix(operand1.getRows(), operand2.getColumns());
            for (int i = 0; i < operand1.getRows(); i++)
                for (int j = 0; j < operand2.getColumns(); j++)
                    for (int k = 0; k < operand1.getColumns(); k++)
                        res[i, j] += operand1[i, k] * operand2[k, j];
            return res;
        }
        //Сумма матриц
        public static Matrix operator +(Matrix summand1, Matrix summand2)
        {
            if (summand1.getRows() != summand2.getRows() || summand1.getColumns() != summand2.getColumns())
            {
                throw new IndexOutOfRangeException("matrixes cannot be summed. They have different size");
            }
            Matrix res = new Matrix(summand1.getRows(), summand1.getColumns());
            for (int i = 0; i < res.getRows(); i++)
                for (int j = 0; j < res.getColumns(); j++)
                    res[i, j] = summand1[i, j] + summand2[i, j];
            return res;
        }
        //Вычитание матриц
        public static Matrix operator -(Matrix summand1, Matrix summand2)
        {
            if (summand1.getRows() != summand2.getRows() || summand1.getColumns() != summand2.getColumns())
            {
                throw new IndexOutOfRangeException("matrixes cannot be summed. They have different size");
            }
            Matrix res = new Matrix(summand1.getRows(), summand1.getColumns());
            for (int i = 0; i < res.getRows(); i++)
                for (int j = 0; j < res.getColumns(); j++)
                    res[i, j] = summand1[i, j] - summand2[i, j];
            return res;
        }
        //Транспонирование
        public Matrix Transpose()
        {
            Matrix res = this;

            for (int i = 0; i < getRows(); i++)
                for (int j = 0; j < getColumns(); j++)
                    res[j, i] = matrix[i, j];
            return res;
        }

        //Минор матрицы
        public Matrix minor(int i, int j)
        {
            Matrix res = new Matrix(m - 1, n - 1);
            for (int row = 0; row < getRows(); row++)
            {
                for (int col = 0; col < getColumns(); col++)
                {
                    if (row == i || col == j) continue;
                    else res[row - (row >= i ? 1 : 0), col - (col >= j ? 1 : 0)] = matrix[row, col];
                }
            }
            return res;
        }

        //Определитель матрицы
        public double det()
        {
            double det = 0.0;
            if (getColumns() == 1)
                det = matrix[0, 0];
            else if (getColumns() == 2)
                det = matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            else
                for (int j = 0; j < getColumns(); j++)
                    det += Math.Pow(-1, j) * matrix[0, j] * minor(0, j).det();
            return det;
        }

        //Обратная матрица
        public Matrix Inverse()
        {
            Matrix inverse = new Matrix(getRows(), getColumns());
            if (det() != 0 && isSquare())
            {
                Matrix al_complements = new Matrix(getRows(), getColumns());
                for (int i = 0; i < getRows(); i++)
                {
                    for (int j = 0; j < getColumns(); j++)
                    {
                        al_complements[i, j] = Math.Pow(-1, i + j) * minor(i, j).det();
                        inverse[j, i] = 1 / det() * al_complements[i, j];
                    }
                }
            }
            return inverse;
        }

        //Булевые функции-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Сравнение матриц
        public bool isEqual(Matrix operand)
        {
            for (int i = 0; i < getRows(); i++)
                for (int j = 0; j < getColumns(); j++)
                    if (matrix[i, j] != operand[i, j]) return false;
            return true;
        }
        //проверка на квадратность
        public bool isSquare() { return getRows() == getColumns(); }

        //проверка на единичность
        public bool isSingle()
        {
            for (int i = 0; i < getRows(); i++)
                if (matrix[i, i] != 1) return false;
            return true;
        }
        //проверка на симметричность
        public bool isSymmetric()
        {
            for (int i = 0; i < getRows(); i++)
                for (int j = 0; j < getColumns(); j++)
                    if (matrix[i, j] != matrix[j, i]) return false;
            return true;
        }
        //Проверка на зануленность
        public bool isZero()
        {
            for (int i = 0; i < getRows(); i++)
                for (int j = 0; j < getColumns(); j++)
                    if (matrix[i, j] != 0) return false;
            return true;
        }

        public Matrix fill(double value)
        {
            for (int i = 0; i < getRows(); i++)
                for (int j = 0; j < getColumns(); j++)
                    matrix[i, j] = value;
            return this;
        }
        public Matrix fill(MatrixFillConstants constant)
        {
            if (constant == MatrixFillConstants.RANDOM)
                for (int i = 0; i < getRows(); i++)
                    for (int j = 0; j < getColumns(); j++)
                        matrix[i, j] = new Random().NextDouble();
            return this;
        }
        //Функция преобразования массива в DataTable
        public DataTable AsDataTable()
        {
            DataTable dt = new DataTable();

            for (int j = 0; j < getColumns(); j++)
                dt.Columns.Add();

            for (int i = 0; i < getRows(); i++)
            {
                object[] matrix_string = new object[getColumns()];
                for (int j = 0; j < getColumns(); j++)
                    matrix_string[j] = matrix[i, j];

                dt.Rows.Add(matrix_string);
            }
            return dt;
        }
        //Получение данных из dataTables
        public static Matrix getData(DataTable data)
        {
            Matrix res = new Matrix(data.Rows.Count, data.Columns.Count - 1);
            for (int i = 0; i < res.getRows(); i++)
            {
                DataRow dt = data.Rows[i];
                for (int j = 0; j < res.getColumns(); j++)
                {
                    res[i, j] = (double)dt[j + 1];
                }
            }

            return res;
        }
        //Стандартизация матрицы
        public Matrix standardized()
        {
            Matrix res = this;

            double[] averageValues = Statistics.Average(res);
            double[] stdDeviations = Statistics.StandardDeviation(res);

            for (int i = 0; i < res.getRows(); i++)
                for (int j = 0; j < res.getColumns(); j++)
                    res[i, j] = (res[i, j] - averageValues[i]) / stdDeviations[i];

            averageValues = Statistics.Average(res);
            stdDeviations = Statistics.StandardDeviation(res);

            return res;
        }
        public Matrix standardized(Matrix data) { return data.standardized(); }
        public Matrix Single()
        {
            for (int i = 0; i < getRows(); i++)
                matrix[i, i] = 1;
            return this;
        }
        public Matrix getTrace()
        {
            Matrix res = new Matrix();
            for (int i = 0; i < getRows(); i++)
                res[i, i] = matrix[i, i];
            return res;
        }

        /*Нахождение собственных значений и собственных векторов матрицы: метод якоби----------------------------------*/
        private int maxElemRow = 0;
        private int maxElemCol = 0;
        private const double epsilon = 0.3;
        public Matrix getEigenValues()
        {
            double[,] mass = new double[getRows(), getColumns()];
            for (int i = 0; i < getRows(); i++)
            {
                for (int j = 0; j < getRows(); j++)
                {
                    mass[i, j] = matrix[i, j];
                }
            }
            var evd = new EigenvalueDecomposition(mass);
            var eigenValues = evd.DiagonalMatrix;
            var eigenVectors = evd.Eigenvectors;

            EigenValues = new Matrix(getRows(), getColumns());
            EigenVectors = new Matrix(getRows(), getColumns());


            for (int i = 0; i < getRows(); i++)
            {
                EigenValues[i, i] = eigenValues[i, i];
                for (int j = 0; j < getColumns(); j++)
                {
                    EigenVectors[i, j] = eigenVectors[i, j];
                }
            }
            sortFactores();
            return EigenValues;

        }
        public Matrix getFactors(int numFactors, Matrix Loadings)
        {
            //sortFactores();
            Matrix res = new Matrix(Loadings.getRows(), numFactors);

            for(int i=0;i<res.getRows();i++)
                for(int j=0;j<numFactors;j++)
                    res[i,j]=Loadings[i,j];

            return res;
        }
        public Matrix getScores(int numScores, Matrix Loadings,Matrix K)
        {
            Matrix res = new Matrix(numScores, Loadings.getColumns());
            for(int i=0;i<numScores;i++)
            {
                for(int j=0; j<res.getColumns();j++)
                {
                    res[i,j] = Loadings[i,j]*K[i,j];
                }
            }


            return this;
        }
        private void sortFactores()
        {
            double max = 0;
            int indexOfMax, indexOfMin = 0;
            for(int i=0; i<getRows()-1; i++)
            {
                for(int j=i+1; j<getRows(); j++) { 
                    if (EigenValues[i, i]> EigenValues[j, j])
                    {
                        //Сортировка собствнных значений
                        double temp = EigenValues[i, i];
                        EigenValues[i, i] = EigenValues[j, j];
                        EigenValues[j, j] = temp;

                        //Сортировка собственных векторов
                        double[] r1 = EigenVectors.getRow(i);
                        double[] r2 = EigenVectors.getRow(j);
                        double [] tempV = r1;
                        r1 = r2;
                        r2 = tempV;
                        EigenVectors.setRow(r1, i);
                        EigenVectors.setRow(r2, j);
                    }
                }
            }
        }
        public void Round(int points)
        {
            for (int i = 0; i < getRows(); i++)
                for (int j = 0; j < getColumns(); j++)
                    matrix[i, j] = Math.Round(matrix[i, j], points);
        }
    }
}
