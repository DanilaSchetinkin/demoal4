using Avalonia.Controls;
using Avalonia.Media.Imaging;
using demoAl.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace demoAl
{
    public class Cheliki
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Sale { get; set; }
        public string Discount { get; set; }
        public string Phone { get; set; }
        public string Logo { get; set; }
        public string AgentType { get; set; }
        public string Email { get; set; }
        public Bitmap LogoAgent { get; set; }
        public int Priority { get; set; }
    }

    public partial class MainWindow : Window
    {
        ObservableCollection<Cheliki> agents;
        public MainWindow()
        {
            InitializeComponent();
            using var context = new User20Context();

            
            var halyava = context.Productsales
                .Include(ps => ps.Product)
                .GroupBy(g => g.Agentid)
                .Select(s => new Discount
                {
                    Id = s.Key,
                    sum = s.Sum(g => g.Productcount * g.Product.Mincostforagent)
                })
                .ToList();

           
            var result = context.Agents
                .Include(a => a.Productsales)
                .Include(a => a.Agenttype)
                .Select(a => new
                {
                    a.Id,
                    Sale = a.Productsales.Sum(ps => ps.Productcount),
                    a.Title,
                    a.Phone,
                    a.Email,
                    a.LogoAgent,
                    a.Priority,
                    AgentType = a.Agenttype.Title
                })
                .AsEnumerable() 
                .Select(a => new Cheliki
                {
                    Id = a.Id,
                    Sale = a.Sale,
                    Title = a.Title,
                    Phone = a.Phone,
                    Email = a.Email,
                    Priority = a.Priority,
                    Discount = Disc(halyava.FirstOrDefault(h => h.Id == a.Id)?.sum ?? 0).ToString() + "%",
                    LogoAgent = a.LogoAgent,
                    AgentType = a.AgentType
                })
                .ToList();

           
            agents = new ObservableCollection<Cheliki>(result);
            AgentBox.ItemsSource = agents;


            SearchBox.TextChanged += SearchBoxChanging;
            titleComboBox.SelectionChanged += sortBoxTitle;
            discountComboBox.SelectionChanged += sortBoxDiscount;
            priorityComboBox.SelectionChanged += sortBoxPriority;



        }


        public void SearchBoxChanging(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchBox.Text?.ToLower() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                
                AgentBox.ItemsSource = agents;
            }
            else
            {
                
                var filteredAgents = agents
                    .Where(s =>
                        (s.Title?.ToLower().Contains(searchText) ?? false) ||
                        (s.Email?.ToLower().Contains(searchText) ?? false) ||
                        (s.Phone?.ToLower().Contains(searchText) ?? false)
                    )
                    .ToList();

                AgentBox.ItemsSource = filteredAgents;
            }
        }



        public static int Disc(decimal sum)
        {
            if (sum < 1000) return 0;
            if (sum < 5000) return 5;
            if (sum < 150000) return 10;
            if (sum < 500000) return 20;
            return 25;
        }

        public class Discount
        {
            public int Id { get; set; }
            public decimal sum { get; set; }
        }

        public void sortBoxTitle(object sender, SelectionChangedEventArgs e)
        {
            var selItem = titleComboBox.SelectedItem as ComboBoxItem;
            string select = selItem.Content?.ToString();

            if (select == "возраст")
            {
                var sortAsc = agents
                    .OrderBy(a => a.Title)
                    .ToList();
                agents.Clear();
                foreach (var item in sortAsc)
                {
                    agents.Add(item);
                }

            }
            else
            {
                var sortDesc = agents
                    .OrderByDescending(a => a.Title)
                    .ToList();
                agents.Clear();
                foreach (var item in sortDesc)
                {
                    agents.Add(item);
                }
            }

        }

        public void sortBoxDiscount(object sender, SelectionChangedEventArgs e)
        {
            var selItem = discountComboBox.SelectedItem as ComboBoxItem;
            string select = selItem.Content?.ToString();

            if (select == "возраст")
            {
                var sortAsc = agents
                    .OrderBy(a =>int.Parse(a.Discount.Replace("%","")))
                    .ToList();
                agents.Clear();
                foreach (var item in sortAsc)
                {
                    agents.Add(item);
                }

            }
            else
            {
                var sortDesc = agents
                    .OrderByDescending(a => int.Parse(a.Discount.Replace("%", "")))
                    .ToList();
                agents.Clear();
                foreach (var item in sortDesc)
                {
                    agents.Add(item);
                }
            }

        }


        public void sortBoxPriority(object sender, SelectionChangedEventArgs e)
        {
            var selItem = priorityComboBox.SelectedItem as ComboBoxItem;
            string select = selItem.Content?.ToString();

            if (select == "возраст")
            {
                var sortAsc = agents
                    .OrderBy(a => a.Priority)
                    .ToList();
                agents.Clear();
                foreach (var item in sortAsc)
                {
                    agents.Add(item);
                }

            }
            else
            {
                var sortDesc = agents
                    .OrderByDescending(a => a.Priority)
                    .ToList();
                agents.Clear();
                foreach (var item in sortDesc)
                {
                    agents.Add(item);
                }
            }




        }



    }
}