using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;//讀寫檔按記得要加這個
using System.Diagnostics;

namespace GA_TsaiHung
{
        class Program : GA

        {
            //============================================================================================================================================================================================
            int numDimensions;  //維度數
            double processTime;  //使用時間

            double[,] ShekelAijMatrix = new double[4, 10] { { 4, 1, 8, 6, 3, 2, 5, 8, 6, 7 }, { 4, 1, 8, 6, 7, 9, 5, 1, 2, 3.6 }, { 4, 1, 8, 6, 3, 2, 3, 8, 6, 7 }, { 4, 1, 8, 6, 7, 9, 3, 1, 2, 3.6 } };
            double[] ShekelCiMatrix = new double[10] { 0.1, 0.2, 0.2, 0.4, 0.4, 0.6, 0.3, 0.7, 0.5, 0.5 };
            public double dimensions_LowerBound;
            public double dimensions_UpperBound;
            
            public int num_Repetitive_Runs = 10;  //重複執行次數
            public int num_Functions = 23; //函數的數量

            public string functionName;  //函數名稱
            public string[] functionNames = new string[23];  //所有函數名稱
            public int functionNumber; //函數編號

            public double[] best_Solution_Averages = new double[23];  //所有Best Solution的平均
            public double[] bestSolutionSDs = new double[23];  //所有Best Solution的標準差

            public double[] function_Average_ProcessTime = new double[23];  //所有的平均使用時間
            public double[] function_Best_Solutions = new double[23];  //所有Best Solution中最佳的

            public int ShekelM = 5;
            //==============================================================================================================================================================================================

            static void Main(string[] args)
            {
                Program ga = new Program();
                Stopwatch stopWatch = new Stopwatch(); //建立計時器


                //寫入EXCEL的做法==========================================================

                StreamWriter fileWriter = new StreamWriter("GA-23_Self.csv");

                //=========================================================================

                fileWriter.WriteLine("Function(Dim), BS, SA, SSD, Time(s)");

                //=========================================================================

                for (int function = 0; function < ga.num_Functions; function++)
                {   

                    //對於23個function
                    stopWatch.Reset();  //將計算的時間歸零
                    stopWatch.Start();  //開始計算時間

                    ga.InitFunction(function + 1);  //呼叫初始函數設定變數內容5
                    ga.functionNames[function] = ga.functionName;  //紀錄函數名稱
                    ga.best_Solution_Averages[function] = 0;

                    ga.processTime = 0;

                    double[] tempFunctionAllGlobalBestFitness = new double[ga.num_Repetitive_Runs];

                    ga.function_Best_Solutions[function] = Double.MaxValue;

                    for (int run = 0; run < ga.num_Repetitive_Runs; run++)
                    {
                        ga.Init(50, ga.numDimensions, ga.dimensions_LowerBound, ga.dimensions_UpperBound);
                        ga.SetStrategy("Tournament", "Arithmetical", "Mutation");
                        ga.Run(160000, 0.9, 0.2);//原為交配律0.9，突變律0.2
                        //160000
                        tempFunctionAllGlobalBestFitness[run] = ga.GBestFitness;  //紀錄每一次的GBestFitness
                        ga.best_Solution_Averages[function] += ga.GBestFitness;  //將每一次的GBestFitness紀錄下來，後面可用來計算10次的平均

                        //===============================================================================================================

                        if (ga.GBestFitness < ga.function_Best_Solutions[function])
                        {  //紀錄最小(最佳)的GBestFitness
                            ga.function_Best_Solutions[function] = ga.GBestFitness;
                        }

                        //===============================================================================================================
                    }

                    stopWatch.Stop();  //停止計算時間

                    ga.processTime = (double)(stopWatch.Elapsed.TotalMilliseconds / 1000);  //紀錄每一個function執行10次的總時間

                     //Console.WriteLine("process time: " + ga.processTime);

                    ga.best_Solution_Averages[function] /= ga.num_Repetitive_Runs;  //計算平均的GBestFitness

                    ga.bestSolutionSDs[function] = sd(tempFunctionAllGlobalBestFitness);  //計算10次GBestFitness的標準差

                    ga.function_Average_ProcessTime[function] = ga.processTime / ga.num_Repetitive_Runs;

                    Console.WriteLine(ga.functionNames[function] + ", " + ga.function_Best_Solutions[function] + ", " + ga.best_Solution_Averages[function] + ", " + ga.bestSolutionSDs[function] + ", " + ga.function_Average_ProcessTime[function]);

                    fileWriter.WriteLine(ga.functionNames[function] + "," + ga.function_Best_Solutions[function] + "," + ga.best_Solution_Averages[function] + "," + ga.bestSolutionSDs[function] + "," + ga.function_Average_ProcessTime[function]);

                    Console.WriteLine("current_resource = " + ga.currentEvaluation);
              
                
                }
                Console.Read();
                fileWriter.Close();//關閉檔案
                //Console.Read();
            }

            public static double sd(double[] fit)
            {
                double sum = 0.0;
                double average;

                for (int i = 0; i < fit.Length; i++)
                {
                    sum += fit[i];
                }

                average = sum / fit.Length;

                sum = 0.0;

                for (int i = 0; i < fit.Length; i++)
                {
                    sum += (Math.Pow(fit[i] - average, 2));
                }

                return Math.Pow(sum / fit.Length, 0.5);
            }

            public override double Fitness(double[] pos)
            {
                double fitness = 0;
                // fitness count == currentEvalution ; //直接在計算Fitness時進行++;
                //currentEvaluation++;
                //======================================================================================

                if (functionNumber == 1)
                {  //Easom
                    double ePow = -Math.Pow(pos[0] - Math.PI, 2) - Math.Pow(pos[1] - Math.PI, 2);

                    fitness = -Math.Cos(pos[0]) * Math.Cos(pos[1]) * Math.Pow(Math.E, ePow);
                }

                //======================================================================================

                else if (functionNumber == 2)
                {  //Shubert
                    double fitness1 = 0, fitness2 = 0;

                    for (int j = 1; j <= 5; j++)
                    {
                        fitness1 = fitness1 + j * Math.Cos((j + 1) * pos[0] + j);
                        fitness2 = fitness2 + j * Math.Cos((j + 1) * pos[1] + j);
                    }

                    fitness = fitness1 * fitness2;
                }

                //=================================================================================================

                else if (functionNumber == 3 | functionNumber == 10 | functionNumber == 15 | functionNumber == 20)
                {  //Rosenbrock
                    for (int j = 0; j < numDimensions - 1; j++)
                    {
                        fitness = fitness + 100 * Math.Pow(pos[j + 1] - Math.Pow(pos[j], 2), 2) + Math.Pow(pos[j] - 1, 2);
                    }
                }

                //=================================================================================================

                else if (functionNumber == 4 | functionNumber == 13 | functionNumber == 18 | functionNumber == 23)
                {  //Zakharov
                    double fitness1 = 0, fitness2 = 0;

                    for (int j = 0; j < numDimensions; j++)
                    {
                        fitness1 = fitness1 + Math.Pow(pos[j], 2);
                        fitness2 = fitness2 + 0.5 * (j + 1) * pos[j];
                    }
                    fitness = fitness1 + Math.Pow(fitness2, 2) + Math.Pow(fitness2, 4);
                }

                //=================================================================================================

                else if (functionNumber == 5 | functionNumber == 9 | functionNumber == 14 | functionNumber == 19)
                {  //Sphere
                    for (int j = 0; j < numDimensions; j++)
                    {
                        fitness = fitness + Math.Pow(pos[j], 2);
                    }
                }

                //======================================================================================

                else if (functionNumber == 6 | functionNumber == 7 | functionNumber == 8)
                {  //Shekel
                    double sum;

                    for (int n = 0; n < ShekelM; n++)
                    {
                        sum = 0;

                        for (int j = 0; j < numDimensions; j++)
                        {
                            sum = sum + Math.Pow(pos[j] - ShekelAijMatrix[j, n], 2);
                        }

                        fitness = fitness + 1 / (sum + ShekelCiMatrix[n]);
                    }

                    fitness = -fitness;
                }

                //======================================================================================

                else if (functionNumber == 11 | functionNumber == 16 | functionNumber == 21)
                {  //Rastrigin
                    for (int j = 0; j < numDimensions; j++)
                    {
                        fitness = fitness + Math.Pow(pos[j], 2) - (10 * Math.Cos(2 * Math.PI * pos[j])) + 10;
                    }
                }

                //======================================================================================

                else if (functionNumber == 12 | functionNumber == 17 | functionNumber == 22)
                {  //Griewank
                    double fitness1 = 0;
                    double fitness2 = 1;

                    for (int j = 0; j < numDimensions; j++)
                    {
                        fitness1 = fitness1 + Math.Pow(pos[j], 2);
                        fitness2 = fitness2 * Math.Cos(pos[j] / Math.Sqrt(j + 1));
                    }

                    fitness = fitness1 / 4000 - fitness2 + 1;
                }

                return fitness;
            }
            public void InitFunction(int number)
            {
                switch (number)
                {
                    //2 numDimensions------------------------------
                    case (1):
                        //Easom(2)
                        this.functionName = "Easom(2)";

                        this.functionNumber = 1;

                        this.numDimensions = 2;

                        this.dimensions_UpperBound = 10;
                        this.dimensions_LowerBound = -10;
                     
                        break;

                    case (2):
                        //Shubert(2)
                        this.functionName = "Shubert(2)";

                        this.functionNumber = 2;

                        this.numDimensions = 2;

                        this.dimensions_UpperBound = 10;
                        this.dimensions_LowerBound = -10;

                        break;

                    case (3):
                        //Rosenbrock(2)
                        this.functionName = "Rosenbrock(2)";

                        this.functionNumber = 3;

                        this.numDimensions = 2;

                        this.dimensions_UpperBound = 30;
                        this.dimensions_LowerBound = -30;

                        break;

                    case (4):
                        //Zakharov(2)
                        this.functionName = "Zakharov(2)";

                        this.functionNumber = 4;

                        this.numDimensions = 2;

                        this.dimensions_UpperBound = 10;
                        this.dimensions_LowerBound = -5;

                        break;

                    //3 numDimensions------------------------------
                    case (5):
                        //De Joung(3)
                        this.functionName = "De Joung(3)";

                        this.functionNumber = 5;

                        this.numDimensions = 3;

                        this.dimensions_UpperBound = 5.12;
                        this.dimensions_LowerBound = -5.12;

                        break;

                    //4 numDimensions------------------------------
                    case (6):
                        //Shekel(4,5)
                        this.functionName = "Shekel(4.5)";

                        this.functionNumber = 6;

                        this.numDimensions = 4;

                        this.dimensions_UpperBound = 10;
                        this.dimensions_LowerBound = 0;

                        this.ShekelM = 5;

                        break;

                    case (7):
                        //Shekel(4,7)
                        this.functionName = "Shekel(4.7)";

                        this.functionNumber = 7;

                        this.numDimensions = 4;

                        this.dimensions_UpperBound = 10;
                        this.dimensions_LowerBound = 0;

                        this.ShekelM = 7;

                        break;

                    case (8):
                        //Shekel(4,10)
                        this.functionName = "Shekel(4.10)";

                        this.functionNumber = 8;

                        this.numDimensions = 4;

                        this.dimensions_UpperBound = 10;
                        this.dimensions_LowerBound = 0;

                        this.ShekelM = 10;

                        break;

                    //10 numDimensions------------------------------
                    case (9):
                        //Sphere(10)
                        this.functionName = "Sphere(10)";

                        this.functionNumber = 9;

                        this.numDimensions = 10;

                        this.dimensions_UpperBound = 100;
                        this.dimensions_LowerBound = -100;

                        break;

                    case (10):
                        //Rosenbrock(10)
                        this.functionName = "Rosenbrock(10)";

                        this.functionNumber = 10;

                        this.numDimensions = 10;

                        this.dimensions_UpperBound = 30;
                        this.dimensions_LowerBound = -30;

                        break;

                    case (11):
                        //Rastrigin(10)
                        this.functionName = "Rastrigin(10)";

                        this.functionNumber = 11;

                        this.numDimensions = 10;

                        this.dimensions_UpperBound = 5.12;
                        this.dimensions_LowerBound = -5.12;

                        break;

                    case (12):
                        //Griewank(10)
                        this.functionName = "Griewank(10)";

                        this.functionNumber = 12;

                        this.numDimensions = 10;

                        this.dimensions_UpperBound = 600;
                        this.dimensions_LowerBound = -600;

                        break;

                    case (13):
                        //Zakharov(10)
                        this.functionName = "Zakharov(10)";

                        this.functionNumber = 13;

                        this.numDimensions = 10;

                        this.dimensions_UpperBound = 10;
                        this.dimensions_LowerBound = -5;

                        break;

                    //20 numDimensions------------------------------
                    case (14):
                        //Sphere(20)
                        this.functionName = "Sphere(20)";

                        this.functionNumber = 14;

                        this.numDimensions = 20;

                        this.dimensions_UpperBound = 100;
                        this.dimensions_LowerBound = -100;

                        break;

                    case (15):
                        //Rosenbrock(20)
                        this.functionName = "Rosenbrock(20)";

                        this.functionNumber = 15;

                        this.numDimensions = 20;

                        this.dimensions_UpperBound = 30;
                        this.dimensions_LowerBound = -30;

                        break;

                    case (16):
                        //Rastrigin(20)
                        this.functionName = "Rastrigin(20)";

                        this.functionNumber = 16;

                        this.numDimensions = 20;

                        this.dimensions_UpperBound = 5.12;
                        this.dimensions_LowerBound = -5.12;

                        break;

                    case (17):
                        //Griewank(20)
                        this.functionName = "Griewank(20)";

                        this.functionNumber = 17;

                        this.numDimensions = 20;

                        this.dimensions_UpperBound = 600;
                        this.dimensions_LowerBound = -600;

                        break;

                    case (18):
                        //Zakharov(20)
                        this.functionName = "Zakharov(20)";

                        this.functionNumber = 18;

                        this.numDimensions = 20;

                        this.dimensions_UpperBound = 10;
                        this.dimensions_LowerBound = -5;

                        break;

                    //30 numDimensions------------------------------
                    case (19):
                        //Sphere(30)
                        this.functionName = "Sphere(30)";

                        this.functionNumber = 19;

                        this.numDimensions = 30;

                        this.dimensions_UpperBound = 100;
                        this.dimensions_LowerBound = -100;

                        break;

                    case (20):
                        //Rosenbrock(30)
                        this.functionName = "Rosenbrock(30)";

                        this.functionNumber = 20;

                        this.numDimensions = 30;

                        this.dimensions_UpperBound = 30;
                        this.dimensions_LowerBound = -30;

                        break;

                    case (21):
                        //Rastrigin(30)
                        this.functionName = "Rastrigin(30)";

                        this.functionNumber = 21;

                        this.numDimensions = 30;

                        this.dimensions_UpperBound = 5.12;
                        this.dimensions_LowerBound = -5.12;

                        break;

                    case (22):
                        //Griewank(30)
                        this.functionName = "Griewank(30)";

                        this.functionNumber = 22;

                        this.numDimensions = 30;

                        this.dimensions_UpperBound = 600;
                        this.dimensions_LowerBound = -600;

                        break;

                    case (23):
                        //Zakharov(30)
                        this.functionName = "Zakharov(30)";

                        this.functionNumber = 23;

                        this.numDimensions = 30;

                        this.dimensions_UpperBound = 10;
                        this.dimensions_LowerBound = -5;

                        break;

                    default:
                        break;
                }
            }
            public static double std(double[] fit)
            {
                double sum = 0.0, average;
                for (int i = 0; i < fit.Length; i++)
                    sum += fit[i];
                average = sum / fit.Length;
                sum = 0.0;
                for (int i = 0; i < fit.Length; i++)
                    sum += (Math.Pow(fit[i] - average, 2));
                return Math.Pow(sum / fit.Length, 0.5);
            }
        }
    }

