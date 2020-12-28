using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public struct Repository
    {
        private TaskPlaner[] taskPlaners;
        private TaskPlaner[] tempArray;

        //private string path;

        //удалили новую строку. Првоеряем в IDE

        int index;
        string[] titles;

        /// <summary>
        /// Конструктор структуры репозитория
        /// </summary>
        /// <param name="Path">Путь к файлу с данными</param>
        public Repository(string Path)
        {
            //this.path = Path;
            this.index = 0;
            this.titles = new string[5];
            this.taskPlaners = new TaskPlaner[1];
            this.tempArray = new TaskPlaner[1];
        }

        #region Methods
        /// <summary>
        /// Изменения размера массива с задачами
        /// </summary>
        /// <param name="Flag">Условие для увеличения размера массива с задачами</param>
        private void Resize(bool Flag)
        {
            if (Flag)   
            {
                Array.Resize(ref this.taskPlaners, this.taskPlaners.Length + 10);   // увеличиваем массив на 10 элементов (с запасом)
            }
        }

        /// <summary>
        /// Добавление задачи в список
        /// </summary>
        /// <param name="ConcreteTask">Структура новой задачи</param>
        public void AddNewTask( TaskPlaner ConcreteTask)
        {
            this.Resize(index >= this.taskPlaners.Length);  // увеличиваем размер массива при выходе за границы массива
            this.taskPlaners[index] = ConcreteTask;         // присваиваем элементу новую структуру с данными
            this.taskPlaners[index].id = index + 1;         // увеличиваем ID задачи на 1
            this.index++;                                   // увеличиваем текущий индекс
        }

        /// <summary>
        /// Загрузка данных из файла
        /// </summary>
        //public void Load()
        //{
        //    using (StreamReader sr =  new StreamReader(this.path))
        //    {
        //        titles = sr.ReadLine().Split(',');

        //        while (!sr.EndOfStream)
        //        {
        //            string[] args = sr.ReadLine().Split(',');
        //            AddNewTask(new TaskPlaner(Convert.ToInt32(args[0]), args[1], args[2], Convert.ToDateTime(args[3])));
        //        }
        //    }
        //}

        public void Load(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                titles = sr.ReadLine().Split(',');

                while (!sr.EndOfStream)
                {
                    string[] args = sr.ReadLine().Split(',');
                    AddNewTask(new TaskPlaner(this.Count + 1, args[1], args[2], Convert.ToDateTime(args[3])));
                }
            }
        }

        /// <summary>
        /// Печать данных в консоль
        /// </summary>
        public void PrintToConsole()
        {
            //Console.WriteLine($"{this.titles[0],5} {this.titles[1],20} {this.titles[2],30} {this.titles[3],15} {this.titles[4],10}");

            for (int i = 0; i < index; i++)
            {
                Console.WriteLine(this.taskPlaners[i].Print()); //печать в консоль текущей задачи
            }
        }

        /// <summary>
        /// Обновление списка задач в консоли (на экране)
        /// </summary>
        public void UpdateConsoleData()
        {
            Console.Clear();
            //Console.WriteLine($"{"1. Add / \"+\"",-15}  - to add new task\n" +
            //                  $"{"2. Edit",-15}  - to edit task by ID\n" +
            //                  $"{"3. Delete / \"-\"",-15}  - to delete selected tasks by ID\n" +
            //                  $"{"4. Save",-15}  - to save current task list to file\n" +
            //                  $"{"5. Load",-15}  - to load tasks from file\n" +
            //                  $"{"6. Sort",-15}  - to sort task lisk by \"TimeLeft\"\n" +
            //                  $"{"7. Update",-15}  - to update data on console\n" +
            //                  $"{"0. Exit",-15}  - to close the programm\n");
            Console.WriteLine($"{"Id",5} {"Title",20} {"Description",30} {"Deadline",15} {"Days left",15}");
            this.PrintToConsole();
        }

        /// <summary>
        /// Сохранение списке задач в указанный файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="header">Заголовки колонок в таблице</param>
        public void SaveDataToFile(string path, string header = "Id,Header,Description,Deadline,Days left")
        {
            using (StreamWriter saveData = new StreamWriter(path))
            {
                saveData.WriteLine(header);
                int i = 0;
                while (i < this.index)
                {
                    saveData.WriteLine($"{this.taskPlaners[i].id},{this.taskPlaners[i].header},{this.taskPlaners[i].description},{this.taskPlaners[i].deadline:%d.MM.yyyy},{this.taskPlaners[i].timeLeft:%d} days");
                    i++;
                }
            }
        }

        /// <summary>
        /// Удаление указанных задачи из списка
        /// </summary>
        /// <param name="TaskID">Список задач, которые нужно удалить</param>
        public void DeleteTaskFromListByID(params int[] TaskID)
        {
            int index;
            Array.Sort(TaskID);    //сортируем массив, чтобы ID задач для удаления шли по порядку. Т.е. пользователь может вводить задачи в любом порядке
            //var max = this.taskPlaners.id.Max();
            for (int i = 0; i < TaskID.Length; i++)    //проходим цикл по Id задач, которые хотим удалить
            {
                if (TaskID[i] > this.taskPlaners[this.taskPlaners.Length-1].id)    //если индекс больше, чем индекс последней задачи - выходим из цикла. Завершаем работу
                    break;
                for (int j = 0; j < this.taskPlaners.Length; j++)   // проходим по массиву задач.
                {
                    if (this.taskPlaners[j].id == TaskID[i])   //если индекс задачи совпадает с номером из "списка на удаление" - работает часть удаления задачи
                    {
                        if (i != 0)
                            TaskID[i] -= i;    //с каждой удаленной задачей все последующие задачи уменьшают свой индекс на 1. Если задач на удаление несколько последняя задача будет иметь индекс меньше на количество удаленных задач
                        index = Array.IndexOf(this.taskPlaners,TaskID[i]); //////////////CHECK !!!!!!!!
                        Array.Resize(ref this.tempArray, this.taskPlaners.Length - 1);      //задаем размер временному массиву на 1 меньше, чем у текущего
                        Array.Copy(this.taskPlaners, 0, this.tempArray, 0, TaskID[i] - 1); //копируем задачи из исходного массива во временный с 0 по "№ задачи - 1"
                        Array.Copy(this.taskPlaners, TaskID[i], this.tempArray, TaskID[i] - 1, this.taskPlaners.Length - TaskID[i]); //копируем оставшуюся часть массива во временный

                        Array.Resize(ref this.taskPlaners, this.tempArray.Length);
                        this.index--;   //индекс текущей задачи уменьшается на 1. Чтобы не печатались пустые элементы массива
                        this.taskPlaners = this.tempArray;  //присвоение временного массива с результатами исходному массиву
                        break;  //т.к. больше ID не совпадут - можно выходить из цикла
                    }
                }
            }


            //for (int i = 0; i < this.taskPlaners.Length; i++)   //переопределяем ID задач после чистки
            //{
            //    this.taskPlaners[i].id = i + 1;
            //}                                                 //можно не переприсваивать ID задачам

        }


        /// <summary>
        /// Сортирует задачи по срочности выполнения
        /// </summary>
        public void SortListByTimeLeft()      // ПЕРЕСМОТРЕТЬ УРОК "ИНДЕКСАТОРЫ"
        {
            TimeSpan[] tempArray = new TimeSpan[this.index];
            for (int i = 0; i < this.index; i++)
            {
                tempArray[i] = this.taskPlaners[i].timeLeft;        //присвоение временному массиву текущих значений timeLeft
            }

            Array.Sort(tempArray);  //сортировка временного массива по возрастанию
            TaskPlaner temp;    //создание временной структуры для хранения данных
            for (int i = 0; i < tempArray.Length; i++)
            {
                for (int j = i; j < this.taskPlaners.Length; j++)
                {
                    if (this.taskPlaners[j].timeLeft == tempArray[i])   //если ID задачи совпадает с ID из временного массива 
                    {
                        temp = this.taskPlaners[i];                     // меняем элементы массива местами
                        this.taskPlaners[i] = this.taskPlaners[j];
                        this.taskPlaners[j] = temp;
                    }
                }
            }

            //var list = from user in this.taskPlaners          //попытка использовать LINQ
            //             orderby user.deadline
            //             select user;

        }

        /// <summary>
        /// Позволяет отредактировать параметры в указанной задаче
        /// </summary>
        /// <param name="Id">ID задачи, которую нужно отредактировать</param>
        public void EditTask(int Id)    
        {
            for (int i = 0; i < this.taskPlaners.Length; i++)
            {
                if (Id == this.taskPlaners[i].id)           // если ID совпал - редактируем элементы структуры
                {
                    Console.Write("Input task name: ");
                    this.taskPlaners[i].header = Console.ReadLine();        //задаем новое имя задачи
                    Console.Write("Input task description: ");
                    this.taskPlaners[i].description = Console.ReadLine();   //задаем новое описание задачи
                    Console.Write("Input task deadline (YYYY.MM.DD): ");
                    this.taskPlaners[i].deadline = Convert.ToDateTime(Console.ReadLine());          // вводим новую дату deadline
                    this.taskPlaners[i].timeLeft = this.taskPlaners[i].deadline - DateTime.Now;     // считаем новые сроки на выполнение задачи
                }
            }

            this.SortListByTimeLeft();  //сортируем по оставшемуся времени после измения задачи
        }
        #endregion

        /// <summary>
        /// Счетчик. Сколько всего задач в файле
        /// </summary>
        public int Count { get { return this.index; } }

    }
}
