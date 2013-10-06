using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA_TsaiHung
{
    class GA
    {
        //Initial Population and Encoding
        private int Population;//Population Size;
        //private int halfPopulation;
        private int Dimension;//Dimesion Size
        double P_Crossover;//Probability_Crossover;
        double P_Mutation; //Probability_Mutation;
        double LowerBound; //Function Range - LowerBound;
        double UpperBound; //Function Range - UpperBound;
        public double GBestFitness = double.MaxValue;// Global Best Fitness
        double[][] curremtSolution;// Population  
        private double[][] tempSolution;
        int MaxEvalution;              //可執行Finess的上限
        public int currentEvaluation;  //目前計算Fitness的次數
        public double[] currentFitness;//Fitness
        private double[,] newChrom;//交配池 大小為 母體群/2 (只抓出一半的優良基因)
        private double[] newChromFitness;
        Random rd = new Random(DateTime.Now.Millisecond);
        private int stillThenBest = 0;
        string selectStrategy;    //Set Selection  Strategy;
        string crossoverStrategy; //Set Crossover  Strategy;
        string mutationStrategy;  //Set Mutation   Strategy;
        int elitist;
        public void Init(int Population_Range, int Dimension_Range, double LowerBound_Range, double UpperBound_Range)
        {//初始化母體群
            currentEvaluation = 0;//初始化計算Fitness次數

            Population = Population_Range;//母體群大小(預設為 50 )
            Dimension = Dimension_Range;  //母體群的維度(依據各問題的維度)
            LowerBound = LowerBound_Range;//基因編碼的下限(依據各問題的下限)
            UpperBound = UpperBound_Range;//基因編碼的上限(依據各問題的上限)
            curremtSolution = new double[Population][];//初始化原始母體群陣列
            //newChromosome = new double[Population][];//初始化新子代母體群陣列                  
            GBestFitness = double.MaxValue;//Global Best Fitness(Local Best Fitness *10  only oen Best is GBest)
            currentFitness = new double[Population];
	        for (int i = 0; i < Population; i++)
            {//Initial Population and Encoding is Real Number
                curremtSolution[i] = new double[Dimension];//初始化母體群的維度
                for (int j = 0; j < Dimension; j++)
                {//初始化母體群 = 下限 + 隨機亂數 * ( 上限 - 下限 ) ;
                    curremtSolution[i][j] = LowerBound + (rd.NextDouble() * (UpperBound - LowerBound));
                }
                currentFitness[i] = Fitness(curremtSolution[i]);
            }
            currentEvaluation += Population;
            sort();
            findGbestFitness();
        }
        public void SetStrategy(string Selection, string Crossover, string Mutation)
        {//設定使用的策略  
            selectStrategy = Selection;//使用菁英策略 + 競賽策略
            crossoverStrategy = Crossover;//使用算術交配策略
            mutationStrategy = Mutation;//使用高斯突變策略
        }
        public void Run(int nIteration, double GAcrossoverRate, double GAmutationRate)
        {//每個Function 執行10次 
            P_Crossover = GAcrossoverRate;//crossoverRate
            P_Mutation = GAmutationRate;//mutationRate
            MaxEvalution = nIteration;//MaxEvalution count

           
            while (currentEvaluation < nIteration)
            {
                currentEvaluation += Population - (int)(Population *0.2);

                Selection();//ElitistStrategy  and  Tournament
                Crossover();//Arithmetical Crossover
                Mutation(); //Gaussian Mutation
                evalution();
            } 
            Console.WriteLine("GBestFit" + GBestFitness);
        }
        private void evalution()
        {//fitness  20%  ~ 100% solution fitness 
            replace();
            sort();
            findGbestFitness();     
        }

        private void sort()
        {
            double[] tempFitness=new double [Population];
            double[][] tempSolution= new double [Population][];
            for (int i = 0; i < Population; i++)
            {
                tempFitness[i] = currentFitness[i];
                tempSolution[i]=new double[Dimension];
            }
            Array.Sort(tempFitness);
            for (int i = 0; i < Population; i++)
            {
                for (int j = 0; j < Population; j++)
                {
                    if(tempFitness[i] == currentFitness[j])
                    {
                        for (int k = 0; k < Dimension;k++ )
                            tempSolution[i][k] = curremtSolution[j][k];    //copy Chromosome to new Chromosome                                                                     
                        break;
                    }
                }
            }

            //Replace

            for (int i = 0; i < Population; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    curremtSolution[i][j] = tempSolution[i][j];
                }
                currentFitness[i] = tempFitness[i];
            }

        }//output is sort currentGeneticSolution  and currentFitness

        private void findGbestFitness()
        {//評估適合度
            //Computing Fitness and Store it                         
            if (currentFitness[0] < GBestFitness)
            {//判斷是否比原本的Gbest更好 是的話取代                                    
                GBestFitness = currentFitness[0];   //這裡注意的是 因為run預設執行10 所以只會回傳10次GBest給Program.cs

            }
        }//End of Evalution

        private void Selection()
        {
            newChrom = new double[Population, Dimension];
            newChromFitness = new double[Population];
            //ElitistStrategy
           
            elitist = (int)(Population * 0.2);            
            for (int i = 0; i < elitist; i++)
            {
                for (int j = 0; j < Dimension; j++)
                    newChrom[i, j] = curremtSolution[i][j];
            }          
            //End of ElitistStrategy
            
            int AdvancePop = (int)(Population *0.6);
            int random_i,random_j;
                if (selectStrategy.Equals("Tournament"))
                {

                    for (int i = elitist; i < Population; i++)
                    {
                        do
                        {
                            random_i = rd.Next(/*Population*/AdvancePop);
                            random_j = rd.Next(/*Population*/AdvancePop);
                        }
                        while (random_i == random_j);
                        int chose=0;
                        if (currentFitness[random_i] < currentFitness[random_j])
                        {
                            chose = random_i;
                        }
                        else
                        {
                            chose = random_j;
                        }
                        for (int j = 0; j < Dimension; j++)
                        {
                            newChrom[i, j] = curremtSolution[chose][j];
                        }
                        newChromFitness[i] = currentFitness[chose];
                    }

                }
                else if (selectStrategy.Equals("RouletteWheel"))
                {

                }
        }
        //Crossover
        private void Crossover()
        {
            int newChromcount = Population - elitist;//defailt 40
            tempSolution = new double[newChromcount][];
            for (int i = 0; i < newChromcount; i++)
                 tempSolution[i] = new double[Dimension];
            double CrossoverRandomRate = rd.NextDouble();
            if (CrossoverRandomRate < P_Crossover)
            {
              if (crossoverStrategy.Equals("Arithmetical"))
              {
                  for (int j = 0; j < newChromcount; j+=2)
                  {
                        double[] Arith1 = new double[Dimension];//算數陣列1
                        double[] Arith2 = new double[Dimension];//算術陣列2 (為算術陣列1的倒數)
                        double randomPercent = rd.NextDouble();
                        int PopRandom1, PopRandom2;
                        do
                        {
                            PopRandom1 = rd.Next(0, newChromcount);
                            PopRandom2 = rd.Next(0, newChromcount);
                        } while (PopRandom1 == PopRandom2);
                     
                            for (int i = 0; i < Dimension; i++)
                            {
                                Arith1[i] = rd.NextDouble();//Random 0.0 ~ 1.0
                                Arith2[i] = 1 - Arith1[i];
                                tempSolution[j][i] = newChrom[PopRandom1, i] * randomPercent + newChrom[PopRandom2, i] * (1 - randomPercent);
                                tempSolution[j+1][i] = newChrom[PopRandom1, i] * (1 - randomPercent) + newChrom[PopRandom2, i] * randomPercent;
                            }
                        }
              }
              else if (crossoverStrategy.Equals("SinglePoint"))
              {
              }
            }
            else
            {
                for (int i = 0; i < newChromcount;i++ )
                    for (int j = 0; j < Dimension; j++)
                    {
                        tempSolution[i][j] = newChrom[i, j];
                    }              
            }
        }
        private void Mutation()
        {//Mutation 
            int newChromcount = Population - elitist;
            //for (int i = 0; i < 2; i++)
            //{
                double randomRate = rd.NextDouble();
                if (randomRate < P_Mutation  /*||  stillThenBest >=10*/)
                {
                    for (int i = 0; i < newChromcount; i++)
                    {
                         double randomRates = rd.NextDouble();
                         int ramdomPosition = rd.Next(Dimension);
                        tempSolution[i][ramdomPosition] = LowerBound + (randomRates * (UpperBound - LowerBound));
                    }
                    stillThenBest = 0;
                }
        }
        private void replace()
        {
            for (int i = 0,k=elitist; i < Population-elitist; i++,k++)
            {
                currentFitness[k] = Fitness(tempSolution[i]);
                for (int j = 0; j < Dimension; j++)
                {
                    curremtSolution[k][j] = tempSolution[i][j];
                }
            }
        }
        public virtual double Fitness(double[] f)
        {
            return -1;
        }
    }
}
