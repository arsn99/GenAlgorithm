using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab3Intel
{


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GenAlgorithm gen = new GenAlgorithm();
        }
        
        class Person:IComparable<Person>
        {
            public double fenotip;
            public double objectiveFunction;//целевая функция
            public double fitness;//приспособленность
            public const int lenght = 20;
            public StringBuilder code;
            public static Random random=new Random();
            static double h = 2.0 / Math.Pow(2.0, lenght);
            public static double fitnessSum=0.0;
            public double fitnessOtn;

            public Person()
            {
                code = GetCode();
                CalcFenotip();
                CalcFunction();
                CalcFitness();
            }
            public Person(StringBuilder str)
            {
                code = str;
                CalcFenotip();
                CalcFunction();
                CalcFitness();
            }
            public static Person Addition(Person p1, Person p2, int k)
            {
                StringBuilder code = new StringBuilder(20);

                for (int i = 0; i < k; i++)
                {
                    code.Append(p1.code[i]);
                }
                for (int i = k; i < lenght; i++)
                {
                    code.Append(p2.code[i]);
                }
                return new Person(code);
            }
            StringBuilder GetCode()
            {
                StringBuilder code = new StringBuilder(lenght);

                for (int j = 0; j < lenght; j++)
                {
                    code.Append(random.Next(0, 2));
                }

                return code;
            }
            public void Mutation()
            {
                int num = random.Next(0, lenght);
                if (code[num] == '1')
                {
                    code[num] = '0';
                }
                else
                {
                    code[num] = '1';
                }
            }
            int ToInt(StringBuilder str)
            {
                int number = 0;
                for (int j = 0; j < lenght; j++)
                {
                    number += Int32.Parse(str[j].ToString()) * (1 << (lenght - 1 - j));
                }
                return number;
            }
            public void CalcFitnessOtn()
            {
                fitnessOtn = fitness / fitnessSum;
            }
            void CalcFenotip()
            {
                fenotip = -1.0 + ToInt(code) * h;
            }
            void CalcFitness()
            {
                fitness= objectiveFunction + 31;
            }
            void CalcFunction()
            {
                objectiveFunction= -Math.Exp(-3.4 * fenotip) * Math.Sign(Math.Sin(43 * fenotip));
            }
            public override string ToString()
            {
                return $"{fenotip} {code} {objectiveFunction} {fitness}";
            }

            public int CompareTo(Person other)
            {
                if (other.fitness==this.fitness)
                {
                    return 0;
                }
                if (other.fitness > this.fitness)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
                    
            }
        }


        class GenAlgorithm
        {
            double Pc = 0.85;
            double Pm = 0.15;
            List<Person> persons;
            const int countPerson=50;
            
            public GenAlgorithm()
            {
                persons = new List<Person>();
                
                for (int i = 0; i < countPerson; i++)
                {
                    persons.Add(new Person());
                }
                double last = 0;
                int counter = 0;
                while (true)
                {
                    Merge();
                    Selection();

                    if (((Person.fitnessSum - last) / Person.fitnessSum) < 0.00001)
                    {
                        break;
                    }
                    else
                    {
                        last = Person.fitnessSum;
                    }
                    Console.WriteLine(last);
                    counter++;
                }
                persons.Sort();
                foreach (var item in persons)
                {
                    Console.WriteLine(item);
                }

                MLApp.MLApp matlab = new MLApp.MLApp();
                object result = null;
                double[] mas = new double[persons.Count];
                double[] mas1 = new double[persons.Count];
                for (int i = 0; i < persons.Count; i++)
                {
                    mas[i] = persons[i].fenotip;
                    mas1[i] = persons[i].objectiveFunction;
                }
                matlab.Execute(@"./lab3Intel");
                matlab.Feval("one", 1, out result,mas,mas1);

                object[] res = result as object[];

                Console.WriteLine(res[0]);
                Console.ReadLine();

            }
            void Selection()
            {
                var tempPersons = new List<Person>();
                for (int j = persons.Count; j > countPerson; j--)
                {
                    Person.fitnessSum = 0;
                    foreach (var item in persons)
                    {
                        Person.fitnessSum += item.fitness;
                    }
                    for (int i = 0; i < persons.Count; i++)
                    {
                        persons[i].CalcFitnessOtn();
                    }
                    double sum = 0;
                    double ball = Person.random.NextDouble();

                    int count = 0;
                    for ( ; sum < ball;count++ )
                    {
                        sum += persons[count].fitnessOtn;
                    }
                    tempPersons.Add(persons[--count]);
                    persons.RemoveAt(count);
                    
                }
                persons = tempPersons;
                
            }
            void Merge()
            {
                var spisok = new List<int>();
                for (int i = 0; i < countPerson; i++)
                {
                    spisok.Add(i);         
                }

                int countChild = 0;
                int first;
                int second;
                int k;
                for (int count =countPerson;spisok.Count>0;count--,countChild++)
                {
                    first = Person.random.Next(0, spisok.Count);
                    spisok.RemoveAt(first);

                    second = Person.random.Next(0, spisok.Count);
                    spisok.RemoveAt(second);

                    k = Person.random.Next(0, Person.lenght - 1);
                    
                    persons.Add(Person.Addition(persons[first], persons[second], k));
                    persons.Add(Person.Addition(persons[second], persons[first], k));
                }
                for (int i = countChild; i > 0; i--)
                {
                    if (Person.random.NextDouble()<Pm)
                    {
                        persons[i].Mutation();
                    }
                }
  
            }
            
        }
    }
}
