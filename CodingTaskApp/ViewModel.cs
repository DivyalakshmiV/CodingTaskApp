using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CodingTaskApp
{
    public class ViewModel: NotifyPropertyChanged
    {
        private Queue<Student> StudentsQueue = new Queue<Student>();

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();             

            }
        }

        private int? id = null;
        public int? Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged();

            }
        }

        private ObservableCollection<Student> resultItems=new ObservableCollection<Student>();
        public ObservableCollection<Student> ResultItems
        {
            get
            {
                return resultItems;
            }
            set
            {
                resultItems = value;
                OnPropertyChanged();

            }
        }

        private RelayCommand getResultCommand;
        public ICommand GetResultCommand
        {
            get
            {
                if (getResultCommand == null)
                {
                    getResultCommand = new RelayCommand(GetResult);
                }
                return getResultCommand;
            }
        }
        private RelayCommand removeResultCommand;
        public ICommand RemoveResultCommand
        {
            get
            {
                if (removeResultCommand == null)
                {
                    removeResultCommand = new RelayCommand(GetResult);
                }
                return removeResultCommand;
            }
        }


        private void GetResult(object obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name) || Id == null )
                {
                    MessageBox.Show("Please enter valid Name and Id");
                }
                else if (ResultItems.ToList().Find(x => x.ID == Id) != null)
                {
                    MessageBox.Show("This Id either waiting for result or already got a result. Please enter different Id");
                }
                else
                {
                    ResultItems.Add(new Student() { ID = id.Value, Name = name, Status = Status.WAITING_FOR_RESULT });
                    Task.Run(() =>
                    {
                        AddStudent(name, id.Value);
                    });
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to get result. Please try again.");
            }
           
        }

        private void AddStudent(string name, int id)
        {
            lock (StudentsQueue)
            {
                StudentsQueue.Enqueue(new Student() { Name = name, ID = id });
            }

            GetResults();

        }

        private void GetResults()
        {
            lock (StudentsQueue)
            {
                foreach (var queue in StudentsQueue)
                {
                    Thread.Sleep(2000);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        Student student = ResultItems.ToList().Find(x => x.ID == queue.ID);
                        int index = ResultItems.IndexOf(student);
                        ResultItems.RemoveAt(index);
                        queue.Result = student.Result = Result.PASS;
                        queue.Status = student.Status = Status.GOT_RESULT;
                        ResultItems.Insert(index, student);
                    }));
                

                    //StudentsQueue.Dequeue();
                    Console.WriteLine(queue.Name + " Passed");
                }
                int count = StudentsQueue.Count(x => x.Result != Result.NA && x.Status == Status.GOT_RESULT);
                for (int i = 0; i < count; i++)
                {
                    StudentsQueue.Dequeue();
                }
            }
        }

    }

    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
