using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MyComboBox
{
    public partial class MainWindow : Window
    {
        class HocSinh
        {
            public string Name { get; set; }
            public int MSSV { get; set; }
        };

        public MainWindow()
        {
            InitializeComponent();

            var data = new List<HocSinh> {
                new HocSinh { MSSV = 20172605, Name = "Le Quang Huy" },
                new HocSinh { MSSV = 20172538, Name = "Nguyen Thi Hien" },
            };

            var sp = new StackPanel
            {
                Background = Brushes.AliceBlue,
                Orientation = Orientation.Horizontal,
            };
            var mcb = new MyComboBox();
            foreach (var item in data)
            {
                var mcbi = new MyComboBoxItem(item, "Name", "MSSV");
                mcb.Add(mcbi);
            }

            var bt = new Button
            {
                Content = "Submit",
                Width = 50,
                Height = 25,
            };

            bt.Click += (s, e) =>
            {
                MessageBox.Show(mcb.SelectedItem.ReturnValue.ToString());
            };

            sp.Children.Add(mcb);
            sp.Children.Add(bt);
            Content = sp;
        }

        public class MyComboBoxItem : UserControl
        {
            public bool IsFocus;

            public string DisplayValue;
            public object ReturnValue;

            public Border BorderItem;
            public TextBlock Item;

            public MyComboBoxItem(object itemSource, string displayProp, string returnProp)
            {
                var typeItem = itemSource.GetType();
                DisplayValue = (string)typeItem.GetProperty(displayProp).GetValue(itemSource);
                ReturnValue = typeItem.GetProperty(returnProp).GetValue(itemSource);

                BorderItem = new Border
                {
                    CornerRadius = new CornerRadius(2),
                };

                Item = new TextBlock
                {
                    Text = DisplayValue,
                    Padding = new Thickness(5),
                };

                BorderItem.Child = Item;
                Content = BorderItem;
            }
        }

        public class MyComboBox : UserControl
        {
            private static Window w = Application.Current.MainWindow;
            public Popup Popup;
            public Grid ComboBox;
            public Border BorderInput;
            public Grid DropDownButton;
            public Border Arrow;
            public Border BorderDropDown;
            public ScrollViewer ScrollView;

            public StackPanel Items;
            public TextBox Input;

            public MyComboBoxItem SelectedItem;

            public MyComboBox()
            {

                Items = new StackPanel();

                // PART_Editable TextBox components
                ComboBox = new Grid
                {
                    Margin = new Thickness(20),
                    Width = 250,
                    Height = 75,
                };

                BorderInput = new Border
                {
                    Background = Brushes.White,
                    Padding = new Thickness(10, 7, 10, 7),
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center,
                    CornerRadius = new CornerRadius(2),
                };

                Input = new TextBox
                {
                    Margin = new Thickness(0, 0, 20, 0),
                    BorderThickness = new Thickness(0),
                };

                DropDownButton = new Grid
                {
                    Background = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(5),
                    Cursor = Cursors.Hand,
                    Height = 25,
                };

                Arrow = new Border
                {
                    BorderThickness = new Thickness(1, 0, 0, 1),
                    BorderBrush = Brushes.Gray,
                    Width = 8,
                    Height = 8,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(8),
                };
                Arrow.RenderTransform = new RotateTransform(-45);

                //structure PART_Editable TextBox
                BorderInput.Child = Input;

                DropDownButton.Children.Add(Arrow);

                ComboBox.Children.Add(BorderInput);
                ComboBox.Children.Add(DropDownButton);

                //PART_Popup components
                Popup = new Popup
                {
                    AllowsTransparency = true,
                };

                BorderDropDown = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1, 0, 1, 1),
                    CornerRadius = new CornerRadius(2),
                    Background = new SolidColorBrush(SystemColors.WindowColor),
                };

                ScrollView = new ScrollViewer
                {
                    MaxHeight = 100,
                };

                //PART_Popup structure
                DropDownButton.Children.Add(Popup);

                ScrollView.Content = Items;

                BorderDropDown.Child = ScrollView;

                Popup.Child = BorderDropDown;

                //Show/Hide popup
                DropDownButton.MouseLeftButtonDown += (s, e) =>
                {
                    if (Popup.IsOpen ^= true)
                    {
                        Popup.Width = BorderInput.ActualWidth;
                        Popup.PlacementTarget = BorderInput;
                        Input.Focus();
                    }
                };

                this.LostFocus += (s, e) =>
                {
                    Popup.IsOpen = false;
                };

                //Popup depend on window

                w.LocationChanged += (s, e) =>
                {
                    var offset = Popup.HorizontalOffset;
                    Popup.HorizontalOffset = offset + 1;
                    Popup.HorizontalOffset = offset;
                };

                w.Deactivated += (s, e) =>
                {
                    Popup.IsOpen = false;
                };

                w.StateChanged += (s, e) =>
                {
                    Popup.IsOpen = false;
                };

                this.PreviewKeyDown += (s, e) =>
                {
                    int currentIndex = GetFocusItemIndex();
                    MyComboBoxItem currentItem;
                    MyComboBoxItem nextItem;
                    MyComboBoxItem previewItem;

                    if (e.Key == Key.Down)
                    {
                        if (Popup.IsOpen == false)
                        {
                            Popup.Width = BorderInput.ActualWidth;
                            Popup.PlacementTarget = BorderInput;
                            Popup.IsOpen = true;
                        }

                        if (currentIndex > -1 && currentIndex < Items.Children.Count)
                        {
                            currentItem = Items.Children[currentIndex] as MyComboBoxItem;
                            currentItem.IsFocus = false;
                            currentItem.BorderItem.Background = Brushes.White;
                            if (currentIndex == Items.Children.Count - 1)
                            {
                                currentIndex = -1;
                            }
                        }

                        nextItem = Items.Children[currentIndex + 1] as MyComboBoxItem;
                        nextItem.IsFocus = true;
                        nextItem.BorderItem.Background = Brushes.LightGray;
                    }

                    else if (e.Key == Key.Up)
                    {
                        if (Popup.IsOpen == false)
                        {
                            Popup.Width = BorderInput.ActualWidth;
                            Popup.PlacementTarget = BorderInput;
                            Popup.IsOpen = true;
                            currentIndex = Items.Children.Count;
                        }

                        if (currentIndex > -1 && currentIndex < Items.Children.Count)
                        {
                            currentItem = Items.Children[currentIndex] as MyComboBoxItem;
                            currentItem.IsFocus = false;
                            currentItem.BorderItem.Background = Brushes.White;
                            if (currentIndex == 0)
                            {
                                currentIndex = Items.Children.Count;
                            }
                        }

                        previewItem = Items.Children[currentIndex - 1] as MyComboBoxItem;
                        previewItem.IsFocus = true;
                        previewItem.BorderItem.Background = Brushes.LightGray;
                    }    
                };

                Content = ComboBox;
            }

            public int GetFocusItemIndex()
            {
                foreach (MyComboBoxItem item in Items.Children)
                {
                    if (item.IsFocus == true)
                    {
                        return Items.Children.IndexOf(item);
                    }
                }
                return -1;
            }

            public void Add(MyComboBoxItem i)
            {

                i.MouseMove += (s, e) =>
                {
                    i.IsFocus = true;
                    i.BorderItem.Background = Brushes.LightGray;
                };

                i.MouseLeave += (s, e) =>
                {
                    i.IsFocus = false;
                    i.BorderItem.Background = Brushes.White;
                };

                i.MouseLeftButtonDown += (s, e) =>
                {
                    Input.Text = i.DisplayValue;
                    SelectedItem = i;
                };

                this.KeyDown += (s, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        if (i.IsFocus == true)
                        {
                            Input.Text = i.DisplayValue;
                            SelectedItem = i;
                            Popup.IsOpen = false;
                        }
                    }
                };


                Items.Children.Add(i);
            }
        }
    }
}
