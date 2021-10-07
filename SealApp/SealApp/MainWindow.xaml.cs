using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
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
using System.Xml.Serialization;
using Excel=Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace SealApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StockList.MaxHeight = 400;
            ReceiptList.MaxHeight = 400;
            ShipmentList.MaxHeight = 400;
            Deserialize("OOOSealReceipt.xml", ReceiptList);
            Deserialize("OOOSealShipment.xml",ShipmentList);
        }

        //Загрузка данных из XML
        private void Deserialize(string path, ListView listView)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Response));

            using (StreamReader reader = new StreamReader(path))
            {
                var response = (Response)serializer.Deserialize(reader);
                Debug.WriteLine(response.Storages.Count);
                foreach(var storage in response.Storages)
                {
                    foreach(var product in storage.Products)
                    {
                        product.StorageName = storage.Name;
                      //  Debug.WriteLine(product.Mass);
                        listView.Items.Add(product);
                    }
                }
            }
            
        }
        //Выгрузка данных в .xls файл
        private void DLoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (datePicker.SelectedDate != null)
            {
                Excel.Application app = new Excel.Application();
                app.Visible = true;
                Workbook wb = app.Workbooks.Add(XlSheetType.xlWorksheet);

                var ws = (Worksheet)app.ActiveSheet;
                ws.Name = "Запасы товаров";
                ws.Cells[1, 1] = "";
                ws.Cells[1, 2] = "Количество товаров, шт";
                ws.Cells[1, 3] = "Масса всего, кг";

                for (int i = 2; i < StockList.Items.Count + 2; i++)
                {
                    var item = (Stock)StockList.Items.GetItemAt(i - 2);
                    ws.Cells[i, 1] = item.ProductName;
                    ws.Cells[i, 2] = item.Quantity;
                    ws.Cells[i, 3] = item.Mass;

                }

                ws = (Worksheet)wb.Worksheets.Add();
                ws.Name = "Отгрузка товаров";
                ws.Cells[1, 1] = "Название склада";
                ws.Cells[1, 2] = "Наименование товара";
                ws.Cells[1, 3] = "Количество товара, шт";
                ws.Cells[1, 4] = "Масса 1 шт, кг";
                ws.Cells[1, 5] = "Хрупкое да/нет";
                ws.Cells[1, 6] = "Дата отгрузки со склада";
                FillWorkSheet(ws, ShipmentList);

                ws = (Worksheet)wb.Worksheets.Add();
                ws.Name = "Поступление товаров";
                ws.Cells[1, 1] = "Название склада";
                ws.Cells[1, 2] = "Наименование товара";
                ws.Cells[1, 3] = "Количество товара, шт";
                ws.Cells[1, 4] = "Масса 1 шт, кг";
                ws.Cells[1, 5] = "Хрупкое да/нет";
                ws.Cells[1, 6] = "Дата поступления на склад";
                FillWorkSheet(ws, ReceiptList);
          
            }
            else MessageBox.Show("Таблица запасы товаров не расчитана");
        }

        private void FillWorkSheet(Worksheet ws, ListView view)
        {
            
            for(int i=2; i < view.Items.Count+2; i++)
            {
                var item = (Product)view.Items.GetItemAt(i-2);
                ws.Cells[i, 1] = item.StorageName;
                ws.Cells[i, 2] = item.Name;
                ws.Cells[i, 3] = item.Quantity;
                ws.Cells[i, 4] = item.Mass;
                ws.Cells[i, 5] = item.IsFragile;
                ws.Cells[i, 6] = item.Date;
            }
        }

        //Обновление таблицы с остатком товара при изменении даты
        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            StockList.Items.Clear();
            var receipt = ReceiptList.Items.OfType<Product>().ToList().Where(pos => DateTime.Parse(pos.Date) <= datePicker.SelectedDate).GroupBy(pos => pos.Name);
            var shipment = ShipmentList.Items.OfType<Product>().ToList().Where(pos => DateTime.Parse(pos.Date)<=datePicker.SelectedDate).GroupBy(pos => pos.Name);
            List<Stock> stock = new List<Stock>();
            foreach(var group in receipt)
            {
                int _Quantity = 0;
                float _Mass = 0;
                foreach(var pos in group)
                {
                    _Quantity += pos.Quantity;
                    _Mass = pos.Mass;
                }
                stock.Add(new Stock
                {
                    ProductName = group.Key,
                    Mass = _Mass*_Quantity,
                    Quantity = _Quantity

                });   
            }
            foreach(var group in shipment)
            {
                int _Quantity = 0;
                float _Mass = 0;
                foreach(var pos in group)
                {
                    _Quantity += pos.Quantity;
                    _Mass = pos.Mass;       
                }
                stock.Find(p => p.ProductName == group.Key).Quantity -= _Quantity;
                stock.Find(p => p.ProductName == group.Key).ComputeMass(_Mass);
            }
            int q = 0;
            float mass = 0;
            foreach (var pos in stock)
            {
                
                StockList.Items.Add(pos);
                q += pos.Quantity;
                mass += pos.Mass;
            }
            StockList.Items.Add(new Stock { ProductName = "Итого", Mass = mass, Quantity = q });
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(layoutGrid.ActualHeight  > 70) StockList.MaxHeight = layoutGrid.ActualHeight-70;
            if (layoutGrid.ActualHeight > 30) ShipmentList.MaxHeight = layoutGrid.ActualHeight - 30;
            if (layoutGrid.ActualHeight > 30) ReceiptList.MaxHeight = layoutGrid.ActualHeight - 30;
            

        }
    }
}
