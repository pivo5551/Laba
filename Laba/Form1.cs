using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


//https://gist.github.com/pivo5551/0e03683553c7903305f5bef0bd7a3ba6
namespace Laba
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static int[] GenArray(int len)
        {
            Random random = new Random();
            int[] arr = new int[len];
            for (int i=0; i<len; i++)
            {
                arr[i] = random.Next(9);
            }
            return arr;
        }
        //для сортировки одним потоком
        public static void ViborSortMax(int[] mas)
        {
            for (int i = mas.Length - 1; i >= 1; i--)
            {
                int max = i;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (mas[max] < mas[j])
                    {
                        max = j;
                    }
                }
                int temp = mas[i];
                mas[i] = mas[max];
                mas[max] = temp;
            }
        }

        //пузырька для многопоточной(Без Lock)
        public static int[] BubbleSort(int[] mas)
        {
            int temp;
            for (int i = 0; i < mas.Length; i++)
            {
                for (int j = i + 1; j < mas.Length; j++)
                {
                    if (mas[i] > mas[j])
                    {
                        temp = mas[i];
                        mas[i] = mas[j];
                        mas[j] = temp;
                    }
                    Thread.Sleep(50);
                }
            }
            return mas;
        }
        //выбора для многопоточной(Без Lock)
        public static int[] ViborSort(int[] mas)
        {

            for (int i = 0; i < mas.Length - 1; i++)
            {
                //поиск минимального числа
                int min = i;
                for (int j = i + 1; j < mas.Length; j++)
                {
                    if (mas[j] < mas[min])
                    {
                        min = j;
                    }
                    Thread.Sleep(50);
                }
                //обмен элементов
                int temp = mas[min];
                mas[min] = mas[i];
                mas[i] = temp;
            }
            return mas;
        }









        private static readonly object lockObj = new object();
        //пузырька для многопоточной(C Lock)
        public static int[] BubbleSort_L(int[] mas)
        {
            int temp;
            lock (lockObj)
            {
                for (int i = 0; i < mas.Length; i++)
                {
                    for (int j = i + 1; j < mas.Length; j++)
                    {
                        if (mas[i] > mas[j])
                        {
                            temp = mas[i];
                            mas[i] = mas[j];
                            mas[j] = temp;
                        }
                        Thread.Sleep(50);
                    }
                }
            }
            return mas;
        }
        //выбора для многопоточной(C Lock)
        public static int[] ViborSort_L(int[] mas)
        {
            lock (lockObj)
            { 
                for (int i = 0; i < mas.Length - 1; i++)
                {
                    //поиск минимального числа
                    int min = i;
                    for (int j = i + 1; j < mas.Length; j++)
                    {
                        if (mas[j] < mas[min])
                        {
                            min = j;
                        }
                        Thread.Sleep(50);
                    }
                    //обмен элементов
                    int temp = mas[min];
                    mas[min] = mas[i];
                    mas[i] = temp;
                }
            }
            return mas;
        }



        public static string ArrayToString(int[] mas)
        {
            string result = string.Empty;
            for(int i=0; i< mas.Length; i++)
            {
                result += mas[i] + " ";
            }
            return result;
        }

        public static int[] array;
        private void Form1_Load(object sender, EventArgs e)
        {
            array = GenArray(10);

            listBox1.Items.Clear();
            listBox1.Items.Add(ArrayToString(array));
        }

        //Generate
        private void button1_Click(object sender, EventArgs e)
        {
            array = GenArray(10);

            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            listBox1.Items.Add(ArrayToString(array));
        }

        //Проверить их работоспособность в обычном, (без поточном)  режиме.
        private void button3_Click(object sender, EventArgs e)
        {

            //BubbleSort(array);
            //ViborSort(array);
            ViborSortMax(array);

            listBox2.Items.Clear();
            listBox2.Items.Add(ArrayToString(array));
            //----------------
        }

        //Debug
        private void button4_Click(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
            listBox3.Items.Add(ArrayToString(array));
        }


        //maximumCount = сколько потоков могут войти в крит секцию (выполнять метод)
        //Счетчик для семафора уменьшается каждый раз, когда поток входит в семафор,
        //и увеличивается, когда поток освобождает семафор. 0==блокировка до освобождения семафора другими потоками
        public static Semaphore semaphoreObject = new Semaphore(initialCount: 2, maximumCount: 2);

        public static Semaphore semaphoreObject1 = new Semaphore(initialCount: 1, maximumCount: 1);
        public static Semaphore semaphoreObject2 = new Semaphore(initialCount: 1, maximumCount: 1);

        //Сортировать в 2 потока с семафором
        private void button5_Click(object sender, EventArgs e)
        {

            if (checkBox1.Checked)//lock on
            {
                listBox2.Items.Clear();

                Thread thread1 = new Thread(() =>
                {
                    semaphoreObject.WaitOne();
                    BubbleSort_L(array);
                    semaphoreObject.Release();
                });

                Thread thread2 = new Thread(() =>
                {
                    semaphoreObject.WaitOne();
                    ViborSort_L(array);
                    semaphoreObject.Release();
                });

                thread1.Start(); thread2.Start();

                thread1.Join(); thread2.Join();
                listBox2.Items.Add(ArrayToString(array));
            }
            else //lockoff
            {
                listBox2.Items.Clear();

                Thread thread1 = new Thread(() =>
                {
                    semaphoreObject1.WaitOne();
                    BubbleSort(array);
                    semaphoreObject1.Release();
                });

                Thread thread2 = new Thread(() =>
                {
                    semaphoreObject2.WaitOne();
                    ViborSort(array);
                    semaphoreObject2.Release();
                });

                thread1.Start(); thread2.Start();

                thread1.Join(); thread2.Join();
                listBox2.Items.Add(ArrayToString(array));

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //
        }
    }
}
