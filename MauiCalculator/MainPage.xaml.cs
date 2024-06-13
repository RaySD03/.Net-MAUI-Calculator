using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiCalculator;

public partial class MainPage : ContentPage
{
    double num1 = 0;
    double num2 = 0;
    int state = 1;  // state 1: waiting for number selection
                    // state 2: arguments are ok, ready to calculate
    double result = 0;
    string entry = "";
    string Operator = "";
    int digitCount = 0;
    bool decimalPoint = false; // true if decimal point is present

    public static ObservableCollection<string> history = new ObservableCollection<string>();

    public MainPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        HistoryList.ItemsSource = history;
    }
    private async void Show_History(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HistoryPage());
    }
    private void pressed(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        button.BackgroundColor = Color.FromArgb("#848496");
    }
    private void released(object sender, EventArgs e)
    {
        Button button = (Button)sender;

        if (button.Text == "=")
        {
            button.BackgroundColor = Color.FromArgb("#C9641F");
        }

        else if (button.Text == "×" || button.Text == "-"
            || button.Text == "+" || button.Text == "÷"
            || button.Text == "1/x" || button.Text == "sqrt"
            || button.Text == "x^2" || button.Text == "Del"
            || button.Text == "CE" || button.Text == "C" || button.Text == "%")
            button.BackgroundColor = Color.FromArgb("#41414D");
        else
        {
            button.BackgroundColor = Color.FromArgb("#575767");
        }
    } 
    private string formatOutput(double num)
    {
        if (num >= 1e+15)
        {
            return num.ToString();
        }
        else
        {
            return num.ToString("##,#0.###########");
        }
    }
    private void resize(object sender, EventArgs e)
    {
        if (Window.Width < 680)
        {
            Keys.ColumnDefinitions[4].Width = 0;
            HistoryButton.IsVisible = true;
            CurrentOperation.FontSize = 15;
            ResultWindow.FontSize = Window.Width * 0.075;
        }
        else if (Window.Width >= 680)
        {
            Keys.ColumnDefinitions[4].Width = new GridLength(2.5, GridUnitType.Star);
            HistoryButton.IsVisible = false;
            HistoryList.WidthRequest = Window.Width * 0.34;
            CurrentOperation.FontSize = 18;
            ResultWindow.FontSize = 32;
        }
    }
    private void Calculate(object sender, EventArgs e)
    {
        // state -2: num1 is not empty, accept the number in result as num2
        // state 2: num1 is not empty, accept a new number selected as num2
        if (state == 2 || state == -2) 
        {
            if (num2 == 0)
            {
                HandleValues(ResultWindow.Text.ToString());
            }

            switch (Operator)
            {
                case "+": 
                    result = num1 + num2;
                    break;
                case "-":
                    result = num1 - num2;
                    break;
                case "×":
                    result = num1 * num2;
                    break;
                case "÷":
                    if (this.ResultWindow.Text == "0")
                    {
                        Clear(sender, e);
                        ResultWindow.Text = "Can not divide by zero";          
                        return;
                    }
                    result = num1 / num2;
                    break;
                default:
                    break;
            }

            ResultWindow.Text = formatOutput(result);    
            CurrentOperation.Text = formatOutput(num1) + " " + Operator + " " + formatOutput(num2) + " =";
            history.Insert(0, CurrentOperation.Text.ToString() + "\n" + ResultWindow.Text + "\n");

            digitCount = 0;
            num1 = result;
            state = -1;
            entry = "";
            decimalPoint = false;
        }
    }
    public async void SelectNumber(object sender, EventArgs e)
    {
        if (digitCount <= 16)
        {
            num2 = 0;
            Button button = (Button)sender;
            string selected = button.Text;
            entry += selected;

            if (state == -1) // Reset current operation since there already was a calculation
            {
                CurrentOperation.Text = "";
            }

            if ((this.ResultWindow.Text == "0" && selected == "0")
                || (entry.Length <= 1 && selected != "0")
                || state < 0)
            {
                this.ResultWindow.Text = "";
                if (state < 0)
                {
                    state *= -1;
                }
            }

            if (selected == "." && decimalPoint == false) // decimal point append if first time
            {
                decimalPoint = true;
                this.ResultWindow.Text += ".";
            }

            if (selected == "." && decimalPoint == true) // decimal point already added, ignore
            {
                return;
            }

            this.ResultWindow.Text += selected;
            digitCount++;
            //ResultWindow.Text = double.Parse(ResultWindow.Text).ToString("##,#0.###########");
        }
    }

    public void Inverse(object sender, EventArgs e)
    {
        if (this.ResultWindow.Text == "0")
        {
            return;
        }
        else
        {
            if (state != -1 && state != 1 && entry != "") // state -1: user select a new number for num2
            {
                HandleValues(ResultWindow.Text);
                CurrentOperation.Text = num1 + " " + Operator + " " + "1/(" + num2 + ")";
                num2 = 1 / num2;
                ResultWindow.Text = num2.ToString("#,0.#####");
            }
            else if (state == -2)
            {
                HandleValues(ResultWindow.Text);
                this.CurrentOperation.Text = num1 + " " + Operator + " " + "1/(" + num2.ToString("#,0.#####" + ")");
                num2 = 1 / num2;
                this.ResultWindow.Text = num2.ToString("#,0.#####");
            }
            else
            {
                CurrentOperation.Text = "";
                num1 = double.Parse(this.ResultWindow.Text.ToString());
                this.CurrentOperation.Text = "1/(" + num1.ToString("#,0.#####" + ")");
                num1 = 1 / num1;
                this.ResultWindow.Text = num1.ToString("#,0.#####");
                entry = "";
                Operator = "";
            }
        }
    }
    public void Square(object sender, EventArgs e)
    {
        if (this.ResultWindow.Text == "0")
        {
            return;
        }
        else
        {
            if (state != -1 && state != 1 && entry != "") // state -1: user selected a new number for num2
            {
                num2 = double.Parse(ResultWindow.Text.ToString());
                CurrentOperation.Text = num1 + " " + Operator + " " + "sqr(" + num2 + ")";
                num2 *= num2;
                ResultWindow.Text = num2.ToString("#,0.#####");
            }
            else if (state == -2) // state -2: user did not select a number, use current number as num2
            {
                HandleValues(ResultWindow.Text);
                this.CurrentOperation.Text = num1 + " " + Operator + " " + "sqr(" + num2.ToString("#,0.#####" + ")");
                num2 *= num2;
                this.ResultWindow.Text = num2.ToString("#,0.#####");
            }
            else
            {
                CurrentOperation.Text = "";
                num1 = double.Parse(this.ResultWindow.Text.ToString());
                this.CurrentOperation.Text = "sqr(" + num1.ToString("#,0.#####" + ")");
                num1 *= num1;
                this.ResultWindow.Text = num1.ToString("#,0.#####");
                entry = "";
                Operator = "";
            }
        }
    }
    public void Sqrt(object sender, EventArgs e)
    {
        if (this.ResultWindow.Text == "0")
        {
            return;
        }
        else
        {
            if (state != -1 && state != 1 && entry != "") // state -1: user select a new number for num2
            {
                num2 = double.Parse(ResultWindow.Text.ToString());
                CurrentOperation.Text = num1 + " " + Operator + " " + "√(" + num2 + ")";
                num2 = Math.Sqrt(num2);
                ResultWindow.Text = num2.ToString("#,0.#####");
            }
            else if (state == -2)
            {
                HandleValues(ResultWindow.Text);
                this.CurrentOperation.Text = num1 + " " + Operator + " " + "√(" + num2.ToString("#,0.#####" + ")");
                num2 = Math.Sqrt(num2);
                this.ResultWindow.Text = num2.ToString("#,0.#####");
            }
            // Compute the sqrt of the number in the result window without any additional requirements
            else
            {
                CurrentOperation.Text = "";
                num1 = double.Parse(this.ResultWindow.Text.ToString());
                this.CurrentOperation.Text = "√(" + num1.ToString("#,0.#####" + ")");
                num1 = Math.Sqrt(num1);
                this.ResultWindow.Text = num1.ToString("#,0.#####");
                entry = "";
                Operator = "";
            }
        }
    }
    public void SelectOperation(object sender, EventArgs e)
    {
        // Avoid pressing '=', if another operator is selected, compute result and move to next operation 
        // Possible only if entry is not empty (num2 is not null)
        if (entry != null && state != -1 && state != -2)
        {
            Calculate(sender, e);
        }

        HandleValues(ResultWindow.Text);
        state = -2;
        Button button = (Button)sender;
        string selected = button.Text;
        this.Operator = selected;

        if (num1.ToString().Length > 15)
        {
            this.CurrentOperation.Text = num1.ToString("G2") + " " + Operator;
        }
        else
        {
            this.CurrentOperation.Text = num1.ToString("#,0.#####") + " " + Operator;
        }

        decimalPoint = false;
        digitCount = 0;
    }
    public void HandleValues(string input)
    {
        double num;
        if (double.TryParse(input, out num))
        {
            if (state == 1)
            {
                num1 = num;
            }
       
            else 
            {
                num2 = num;
            }
            entry = "";
        }
    }
    private void Delete(object sender, EventArgs e)
    {
        // state -2 is after selecting an operation
        // delete key should not operate when an operand is selected and new number is not selected
        if (ResultWindow.Text.Length >= 1 && state != -1 && state != -2)
        {
            if (ResultWindow.Text.Length == 1)
            {
                ResultWindow.Text = "0";
                entry = "";
                return;
            }
            if (ResultWindow.Text == "Can not divide by zero")
            {
                ResultWindow.Text = "";
            }
            ResultWindow.Text = ResultWindow.Text.ToString().Remove(ResultWindow.Text.Length - 1);
            //ResultWindow.Text = double.Parse(ResultWindow.Text).ToString("##,#0.###########");
            entry.Remove(entry.Length - 1);
        }
        if (ResultWindow.Text.ToString().Contains('.'))
        {
            decimalPoint = true;
        }
        if (ResultWindow.Text.ToString().Contains('.') == false)
        {
            decimalPoint = false;
        }
        // state -1 is after selecting =, only delete current operation
        if (state == -1) 
        {
            CurrentOperation.Text = "";
        }
       
    }
    private void ClearHistory(object sender, EventArgs e)
    {
        history.Clear();
    }
    private void percent(object sender, EventArgs e)
    {
        double percent = double.Parse(this.ResultWindow.Text);

        if (state != -1 && state != 1 && entry != "") // state -1: user select a new number for num2
        {
            HandleValues(ResultWindow.Text);

            ResultWindow.Text = (percent / 100).ToString("#,0.#####");
            num2 = double.Parse(this.ResultWindow.Text.ToString());
            this.CurrentOperation.Text = num1 + " " + Operator + " " + num2.ToString("#,0.#####");
        }
        // In the middle of operation, compute percent of second argument
        else if (state == -2)
        {
            HandleValues(ResultWindow.Text);

            ResultWindow.Text = (percent / 100).ToString("#,0.#####");
            num2 = double.Parse(this.ResultWindow.Text.ToString());               
            this.CurrentOperation.Text = num1 + " " + Operator + " " + num2.ToString("#,0.#####");
        }  
        // Compute the percentage of current number
        else
        {
            ResultWindow.Text = (percent / 100).ToString();
            num1 = double.Parse(this.ResultWindow.Text.ToString());
            this.ResultWindow.Text = num1.ToString("#,0.#####");
            entry = "";
            Operator = "";
        }
    }
    private void Negate(object sender, EventArgs e)
    {
        if (ResultWindow.Text != "0")
        {
            // state -1: user selects a new number for num2
            // state 1: user did not select a new number, num2 is current number
            if (state != -1 && state != 1) 
            {
                num2 = -1 * double.Parse(ResultWindow.Text);
                ResultWindow.Text = num2.ToString();
                
                this.CurrentOperation.Text = num1 + " " + Operator + " " + num2.ToString("#,0.#####");
            }
            // In the middle of operation, negate second argument
            else if (state == -2)
            {
                num2 = -1 * double.Parse(ResultWindow.Text);
                ResultWindow.Text = num2.ToString();
               
                this.CurrentOperation.Text = num2 + " " + Operator + " " + num2.ToString("#,0.#####");
            }
            // negate current number
            else
            {
                double negate = -1 * double.Parse(ResultWindow.Text);
                ResultWindow.Text = negate.ToString();
                num1 = negate;
            }
        }
    }
    private void ClearEntry(object sender, EventArgs e)
    {
        if (state == -1)
        {
            CurrentOperation.Text = "";
        }

        else if (state != 1)
        {
            CurrentOperation.Text = num1 + " " + Operator;
        }

        ResultWindow.Text = "0";
        entry = "";
        decimalPoint = false;
    }
    private void Clear(object sender, EventArgs e)
    {
        ResultWindow.Text = "0";
        CurrentOperation.Text = "";
        digitCount = 0;
        entry = "";
        num1 = 0;
        num2 = 0;
        state = 1;
        decimalPoint = false;
    }
}

