using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
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
using System.Xml;
using System.Xml.Linq;

namespace FridgeShop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XDocument myProdLoad = GetXml("Products.xml");
        XDocument myBuyersLoad = GetXml("Buyers.xml");
        XDocument mySellersLoad = GetXml("Sellers.xml");
        XDocument myCountLoad = GetXml("Count.xml");
        XDocument orderDoc;
        double total = 0;
        string pathOrder = null;
        public MainWindow()
        {
            InitializeComponent();
            // BuildXmlDocWithDOM();
            // заполнение таварами в comboBox 
            foreach (XElement item in myProdLoad.Element("Products").Elements("Product"))
            {
                comboBox.Items.Add(string.Format("{0}", item.Element("Make").Value));
            }
            // заполнение Покупатель в comboBox 
            foreach (XElement item in myBuyersLoad.Element("Buyers").Elements("Buyer"))
            {
                comboBox1.Items.Add(string.Format("{0}", item.Element("Name").Value));
            }
            // заполнение продавцов в comboBox 
            foreach (XElement item in mySellersLoad.Element("Sellers").Elements("Seller"))
            {
                comboBox2.Items.Add(string.Format("{0}", item.Element("Name").Value));
            }
            // заполнение НОМЕРА ЧЕКА в comboBox 
            foreach (XElement item in myCountLoad.Element("Count").Elements("Count1"))
            {
                label1.Content = string.Format("{0}", item.Attribute("CheckNumber").Value);
            }




        }


        public static XDocument GetXml(string path)
        {
            try
            {
                XDocument Doc = XDocument.Load(path);
                return Doc;
            }
            catch (System.IO.FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }


        private static void BuildXmlDocWithDOM()
        {// Заполнить документ корневым элементом
            XElement products = new XElement("Products",
                             new XElement("Product", new XAttribute("ID", "1000"),
                                 new XElement("Color", "White"),
                                 new XElement("Make", "Atlant"),
                                 new XElement("Price", "90000")
                                 ),
                             new XElement("Product", new XAttribute("ID", "1001"),
                                 new XElement("Color", "White"),
                                 new XElement("Make", "Indesit"),
                                 new XElement("Price", "120000")
                                 ),
                             new XElement("Product", new XAttribute("ID", "1002"),
                                 new XElement("Color", "White"),
                                 new XElement("Make", "Samsung"),
                                 new XElement("Price", "140000")
                                 )
                             );
            // Сохранить документ в файл.
            products.Save("Products.xml");  //в папку  bin\Debug

            // Заполнить документ корневым элементом
            XElement buyers = new XElement("Buyers",
                             new XElement("Buyer", new XAttribute("ID", "2000"),
                             new XElement("Name", "John Conor")
                                 ),
                             new XElement("Buyer", new XAttribute("ID", "2001"),
                             new XElement("Name", "Terminator")
                                 ),
                             new XElement("Buyer", new XAttribute("ID", "2002"),
                             new XElement("Name", "Sara Conor")
                                 )
                             );
            // Сохранить документ в файл.
            buyers.Save("Buyers.xml");  //в папку  bin\Debug

            // Заполнить документ корневым элементом
            XElement sellers = new XElement("Sellers",
                             new XElement("Seller", new XAttribute("ID", "3000"),
                             new XElement("Name", "Gary Potter")
                                 ),
                             new XElement("Seller", new XAttribute("ID", "3001"),
                             new XElement("Name", "Albus Dumbledore")
                                 ),
                             new XElement("Seller", new XAttribute("ID", "3002"),
                             new XElement("Name", "Ronald Weasley")
                                 )
                             );
            // Сохранить документ в файл.
            sellers.Save("Sellers.xml");  //в папку  bin\Debug

            // Заполнить документ корневым элементом
            XElement count = new XElement("Count",
                             new XElement("Count1", new XAttribute("CheckNumber", "10000")
                                   )
                             );
            // Сохранить документ в файл.
            count.Save("Count.xml");  //в папку  bin\Debug
        }

        #region constructor


        #endregion
        //Сформировать чек
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Check check = new Check();
            XDocument newPathForOrder = orderDoc;
            if (textBox2.Text == "") { MessageBox.Show("Укажите путь для сохранения чека"); }
            else
            {
                newPathForOrder.Save(textBox2.Text.ToString() + @"\order_" + label1.Content.ToString() + ".xml"); // сохранить по укзаному пути пользователя

                check.label1.Content = label1.Content;

                label1.Content = (Convert.ToInt32(label1.Content) + 1).ToString();  // увеличить номер чека на 1
                                                                                    // в файле Count номер чека увеличиваем на 1
                foreach (XElement item in myCountLoad.Element("Count").Elements("Count1"))
                {
                    item.Attribute("CheckNumber").Value = (Convert.ToInt32(label1.Content) + 1).ToString();
                }
                myCountLoad.Save("Count.xml");

                listBox.Items.Clear();
                check.Show();
            }

        }
        //добавить товар в листбокс
        private void button_Click(object sender, RoutedEventArgs e)
        {
            double quantityPrice = Convert.ToDouble(textBox1.Text) * Convert.ToInt32(textBox.Text);

            // Высчитать Итого
            total += quantityPrice;
            label8.Content = total;

            pathOrder = @"Order_" + label1.Content.ToString() + ".xml";  ///!!!!!!!!!!!!
            //Запись в файл Ордера
            if (listBox.Items.IsEmpty)
            {   // Заполнить документ корневым элементом
                XElement checkToXML = new XElement("Order",
                                 new XElement("Check", new XAttribute("Id", label1.Content)),
                                 new XElement("Company", label2.Content),
                                 new XElement("Products",
                                 new XElement("Product", comboBox.Text),
                                 new XElement("Quantity", textBox.Text),
                                 new XElement("Price", quantityPrice.ToString())
                                 ),
                                 new XElement("Buyer", comboBox1.Text),
                                 new XElement("Seller", comboBox2.Text)

                                 );
                // Сохранить документ в файл.

                checkToXML.Save(pathOrder);  //в папку  bin\Debug
                orderDoc = GetXml(pathOrder);
            }
            else
            {
                // Загрузить текущий документ.
                orderDoc = GetXml(pathOrder);
                // Создать новый объект XElement на основе входных параметров.
                XElement newElement = new XElement("Products",
                                 new XElement("Product", comboBox.Text),
                                 new XElement("Quantity", textBox.Text),
                                 new XElement("Price", quantityPrice.ToString())
                                 );
                // Добавить к объекту XDocument в памяти.
                orderDoc.Descendants("Order").First().Add(newElement);

                // Сохранить документ в файл.                
                orderDoc.Save(pathOrder);  //в папку  bin\Debug
            }
            //Добавление  товара в лист

            string contentProduct = string.Format("{0}. {1} -- {2} -- {3}", listBox.Items.Count + 1,
                comboBox.Text,
                textBox.Text, quantityPrice.ToString());
            listBox.Items.Add(contentProduct);
        }

        // после выбора товара отобразить цену
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedValue != null)
            {
                string make = comboBox.SelectedValue.ToString();

                //XDocument xdoc = XDocument.Load("Products.xml");
                var items = from xe in myProdLoad.Element("Products").Elements("Product")
                            where xe.Element("Make").Value == make
                            select new Product
                            {
                                Name = xe.Element("Make").Value,
                                Price = Convert.ToDouble(xe.Element("Price").Value)
                            };


                textBox1.Text = items.FirstOrDefault().Price.ToString(); //Console.WriteLine("{0} - {1}", item.Name, item.Price);
            }
        }
    }
}
