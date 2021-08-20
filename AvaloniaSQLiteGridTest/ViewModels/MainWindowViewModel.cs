using System;
using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaSQLiteGridTest.DatabaseStuff;
using ReactiveUI;
using Splat;

namespace AvaloniaSQLiteGridTest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _dataText;

        public ObservableCollection<DataViewModel> Data { get; } = new()
        {
            new DataViewModel(1, "Garrus"),
            new DataViewModel(2, "Samara"),
            new DataViewModel(3, "Wrex"),
            new DataViewModel(4, "Grunt"),
        };

        public string DataText
        {
            get => _dataText;
            set => this.RaiseAndSetIfChanged(ref _dataText, value);
        }

        public MainWindowViewModel()
        {
            Handler = Locator.Current.GetService<HandleStuffWorker>();
            DataText = $"Letzte Änderung: ";
            Handler.Handle += HandleThis;
        }

        public HandleStuffWorker Handler { get; }

        private void HandleThis(object sender, HandleStuffEventHandlerArgs args)
        {
            var count = 0;
            using (var db = new DataContext())
            {
                if (db.Datas.Any(x => x.Id == args.Data.Id))
                    db.Datas.Update(args.Data);
                else
                    db.Datas.Add(args.Data);
                db.SaveChanges();
                count = db.Datas.Count();
            }
            DataText = $"Letzte Änderung: {DateTime.Now:T} | Objects: {count}";
        }
    }

    public class HandleStuffWorker
    {
        public event HandleStuffEventHandler Handle;

        public HandleStuffWorker()
        {
        }

        public void GoHandleThis(DataViewModel data)
        {
            Handle?.Invoke(this, new HandleStuffEventHandlerArgs()
            {
                Data = data
            });
        }
    }

    public delegate void HandleStuffEventHandler(object sender, HandleStuffEventHandlerArgs args);

    public class HandleStuffEventHandlerArgs
    {
        public DataViewModel Data { get; set; }
    }

    public class DataViewModel : ReactiveObject
    {
        private int _id;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }
        
        private string _name;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public DataViewModel() : this(-1, String.Empty)
        {
            
        }

        private void HandleThis()
        {
            var handler = Locator.Current.GetService<HandleStuffWorker>();
            handler.GoHandleThis(this);
        }

        public DataViewModel(int id, string name)
        {
            _id = id;
            _name = name;
            this.WhenAnyValue(x => x.Name)
                .Subscribe(x => HandleThis());
        }

    }
}
