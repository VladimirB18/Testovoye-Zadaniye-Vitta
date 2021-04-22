using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testovoe_Zadaniye.Commands;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;
using System.Data.Entity;
using System.Threading;

namespace Testovoe_Zadaniye.ViewModels
{
   internal class MainWindowViewModel : Base.ViewModel , INotifyPropertyChanged
    {
        #region WorksWithGodsBlessing

        #region INotifyPropertyChanged_Implementation
        /* public event PropertyChangedEventHandler PropertyChanged;
         private void OnPropertyChanged(string propertyName)
         {
             if (PropertyChanged != null)
                 PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }*/
        #endregion
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        public ObservableCollection<Orders> Groups { get; set; }
        public ObservableCollection<Orders> GroupsPayed { get; set; }
        public static Views.Windows.MainWindow MW; 
        private double balance { get; set; }
        public double Balance
        {
            get
            {
                return this.balance;
            }
            set
            {
                if (value != this.balance)
                {
                    this.balance = value;
                    NotifyPropertyChanged("Balance");
                }
            }
        }




    
        #region Commands
        #region Add new order
        public ICommand CreateOrderCommand { get; }
        private bool CanReloadCommandExecute(object p) => true;

        private void OnReloadCommandExecuted(object p)
        {
            Random rnd = new Random();
            double price = rnd.Next(999, 2001) + Math.Round(rnd.NextDouble(),2);
            Orders NewOrder = new Orders() { Date = DateTime.Now, Summa = price, SummaOplati = 0 };

            using (var context = new DataBase())
            {
                context.Orders.Add(NewOrder);
                context.SaveChanges();
                Groups.Add(NewOrder);
            }
            MW.TopUpMenu.Visibility = Visibility.Hidden;
            MW.ListGrid.Visibility = Visibility.Visible;
            MW.ListPayedGrid.Visibility = Visibility.Hidden;
        }
        #endregion
        #region HideTopUpMenuCommand
        public ICommand HideTopUpMenuCommand { get; }
        private bool CanHideTopUpMenuCommandExecute(object p) => true;

        private void OnHideTopUpMenuCommandExecuted(object p)
        {
            MW.TopUpMenu.Visibility = Visibility.Hidden;
            MW.ListGrid.Visibility = Visibility.Visible;
        }
        #endregion
        #region PayOrder
        public ICommand PayOrderCommand { get; }
        private bool CanPayOrderCommandExecute(object p) => true;

        private void OnPayOrderCommandExecuted(object p)
        {
            if (MW.ListBoxName.SelectedItem != null)
            {
                Orders SelectedObj = MW.ListBoxName.SelectedItem as Orders;
                using (var context = new DataBase())
                {
               
                    double sum = 0;
                    var getNotEmpty = context.MoneyIncome.Where(s => s.Balance != 0);

                    List<MoneyIncome> wallet = new List<MoneyIncome>(getNotEmpty);
            
                    foreach (var item in wallet) if (item.Balance > 0) // check if there is enougn money
                        {
                            sum = sum + Convert.ToDouble(item.Balance);
                        }

                    if (sum >= SelectedObj.Summa - SelectedObj.SummaOplati)
                    {
                        double deltaPay = 0;
                        foreach (var item in wallet)
                        {
                            if (item.Balance >= SelectedObj.Summa - SelectedObj.SummaOplati) //check if there is enough money in single one
                            {
                                deltaPay = Convert.ToDouble(SelectedObj.Summa - SelectedObj.SummaOplati);
                                #region Create payment table
                                Payments payment = new Payments() { IdOfOrder = SelectedObj.Id };
                                payment.IdOfMoneyIncome = item.Id;
                                payment.Payment = deltaPay;
                                context.Payments.Add(payment);
                                context.SaveChanges();
                                #endregion
                                #region Balance fixation
                                item.Balance = item.Balance - deltaPay;
                                var result = context.MoneyIncome.SingleOrDefault(s => s.Id == item.Id);
                                if (result != null)
                                {
                                    result.Balance = item.Balance;
                                    context.SaveChanges();
                                }
                                #endregion
                                SelectedObj.SummaOplati = SelectedObj.Summa;
                                Groups.Remove(SelectedObj); // remove order from list
                                GroupsPayed.Add(SelectedObj);
                                break;
                            }
                            else
                            {
                                #region Create payment table
                                Payments payment = new Payments() { IdOfOrder = SelectedObj.Id };
                                payment.IdOfMoneyIncome = item.Id;
                                payment.Payment = Convert.ToDouble(item.Balance);
                                context.Payments.Add(payment);
                                context.SaveChanges();
                                #endregion
                                #region Balance fixation
                                deltaPay = Convert.ToDouble(SelectedObj.Summa - SelectedObj.SummaOplati);
                                deltaPay = deltaPay - Convert.ToDouble(item.Balance);
                                SelectedObj.SummaOplati = SelectedObj.SummaOplati + item.Balance;
                                item.Balance = 0;

                                var result = context.MoneyIncome.SingleOrDefault(s => s.Id == item.Id);
                                if (result != null)
                                {
                                    result.Balance = item.Balance;
                                    context.SaveChanges();
                                }
                                #endregion
                                #region Hide order
                                if (SelectedObj.SummaOplati == SelectedObj.Summa)
                                {
                                  //  MW.ListBoxName.Items.Remove(MW.ListBoxName.SelectedItem);
                                    Groups.Remove(SelectedObj);
                                    GroupsPayed.Add(SelectedObj);
                                    break;
                                }
                                #endregion
                            }

                        }
                   
                    }
                    else
                    {
                        MessageBox.Show("Not enough money to pay the order", "Payment", MessageBoxButton.OK);
                        MW.TopUpMenu.Visibility = Visibility.Hidden;
                        MW.ListGrid.Visibility = Visibility.Visible;
                    }
                    var wallety = context.MoneyIncome.ToList();
                    Balance = 0;
                    foreach (var item in wallet)
                    {
                        Balance = Balance + Convert.ToDouble(item.Balance);
                    }
                }
               
            }
            else
            {
                MessageBox.Show("Choose an order first", "Payment", MessageBoxButton.OK);
            }
            MW.TopUpMenu.Visibility = Visibility.Hidden;
            MW.ListGrid.Visibility = Visibility.Visible;
        }
        #endregion
        #region TopUpAnAccount
        public ICommand TopUpAnAccountCommand { get; }
        private bool CanTopUpAnAccountCommandExecute(object p) => true;

        private void OnTopUpAnAccountCommandExecuted(object p)
        {
            double sum = Convert.ToDouble(MW.TopUpValue.Text);
            using (var context = new DataBase())
            {
                if (MW.TopUpValue.Text != "")
                {
                    MoneyIncome moneyDrop = new MoneyIncome() { Date = DateTime.Now, Summa = sum, Balance = sum };
                    context.MoneyIncome.Add(moneyDrop);
                    context.SaveChanges();
                }
            }
            
       
               

            Balance = Balance + sum;
                
                
            
             MW.TopUpValue.Text = "";
             MW.TopUpMenu.Visibility = Visibility.Hidden;
             MW.ListGrid.Visibility = Visibility.Visible;


        }
        #endregion
        #region OpenTopUpMenuCommand
        public ICommand OpenTopUpMenuCommand { get; }
        private bool CanOpenTopUpMenuCommandExecute(object p) => true;

        private void OnOpenTopUpMenuCommandExecuted(object p)
        {
            MW.TopUpMenu.Visibility = Visibility.Visible;
            MW.ListGrid.Visibility = Visibility.Hidden;
            MW.ListPayedGrid.Visibility = Visibility.Hidden;
        }
        #endregion
        #region OpenPayedList
        public ICommand OpenPayedListCommand { get; }
        private bool CanOpenPayedListCommandExecute(object p) => true;

        private void OnOpenPayedListCommandExecuted(object p)
        {
            MW.TopUpMenu.Visibility = Visibility.Hidden;
            MW.ListGrid.Visibility = Visibility.Hidden;
            MW.ListPayedGrid.Visibility = Visibility.Visible;
        }
        #endregion
        #region OpenOrderList
        public ICommand OpenOrderListCommand { get; }
        private bool CanOpenOrderListCommandExecute(object p) => true;

        private void OnOpenOrderListCommandExecuted(object p)
        {
            MW.TopUpMenu.Visibility = Visibility.Hidden;
            MW.ListGrid.Visibility = Visibility.Visible;
            MW.ListPayedGrid.Visibility = Visibility.Hidden;
        }
        #endregion
        #endregion
        public MainWindowViewModel()
        {
            using (var Context = new DataBase())
            {
                try
                {
                    if (!Context.Database.Exists())
                    {
                        Context.Database.Create();
                 
                       // Context.Database.SqlQuery<DataBase>("CREATE TRIGGER Payments_INSERT ON dbo.Payments AFTER INSERT AS BEGIN UPDATE dbo.Orders SET[SummaOplati] = [SummaOplati] + [Payment] FROM inserted WHERE[IdOfOrder] = dbo.Orders.Id END");
                        string com = "CREATE TRIGGER Payments_INSERT ON dbo.Payments AFTER INSERT AS BEGIN UPDATE dbo.Orders SET[SummaOplati] = [SummaOplati] + [Payment] FROM inserted WHERE[IdOfOrder] = dbo.Orders.Id END";
                        Context.Database.ExecuteSqlCommand(com);
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }


            #region GetData
            using ( var context = new DataBase())
             {
                
                List<Orders> orders = context.Orders.ToList();
                Groups = new ObservableCollection<Orders>(orders);
                List<Orders> list = new List<Orders>();
                GroupsPayed = new ObservableCollection<Orders>(list);
                foreach (var item in orders)
                {
                    if (item.SummaOplati == item.Summa)
                    {
                        Groups.Remove(item);
                        GroupsPayed.Add(item);
                    }
                }

                
                var wallet = context.MoneyIncome.ToList();
                foreach (var item in wallet)
                {
                    Balance = Balance + Convert.ToDouble(item.Balance);
                }
               
            }
            #endregion
            #region Commands
            CreateOrderCommand = new LambdaExpression(OnReloadCommandExecuted, CanReloadCommandExecute);
            PayOrderCommand = new LambdaExpression(OnPayOrderCommandExecuted, CanPayOrderCommandExecute);
            TopUpAnAccountCommand = new LambdaExpression(OnTopUpAnAccountCommandExecuted, CanTopUpAnAccountCommandExecute);
            OpenTopUpMenuCommand = new LambdaExpression(OnOpenTopUpMenuCommandExecuted, CanOpenTopUpMenuCommandExecute);
            HideTopUpMenuCommand = new LambdaExpression(OnHideTopUpMenuCommandExecuted, CanHideTopUpMenuCommandExecute);
            OpenPayedListCommand = new LambdaExpression(OnOpenPayedListCommandExecuted, CanOpenPayedListCommandExecute);
            OpenOrderListCommand = new LambdaExpression(OnOpenOrderListCommandExecuted, CanOpenOrderListCommandExecute);
            #region Crutch

            #endregion
            #endregion


        } 
        
         
    }
}
