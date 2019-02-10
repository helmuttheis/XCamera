using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XCamera.Util;

namespace XCamera
{
    public class Overlay
    {
        private Grid grdOverlay;
        public int iRow { get; set; }
        public Overlay(Grid grdOverlay)
        {
            this.grdOverlay = grdOverlay;
        }
        public void Reset()
        {
            grdOverlay.Children.Clear();
            iRow = 0;
        }
        public void Close()
        {
            ((ContentView)grdOverlay.Parent).IsVisible = false;
        }
        public void Show()
        {
            ((ContentView)grdOverlay.Parent).IsVisible = true;
        }
        public Boolean bIsVisible()
        {
            return ((ContentView)grdOverlay.Parent).IsVisible;
        }
        public void ShowQuestion(string szQuestion , Action onYes)
        {
            Reset();

            AddLabel(szQuestion);
            Button btnYes = AddButton("Ja");
            AddCancelX();
            btnYes.Clicked += (s, e) => {
                ((ContentView)grdOverlay.Parent).IsVisible = false;
                onYes();
            };
            ((ContentView)grdOverlay.Parent).IsVisible = true;

        }
        public void ShowMessage(string szMessage)
        {
            Reset();

            AddLabel(szMessage);
            Button btnYes = AddButton("weiter");
            AddCancelX();
            btnYes.Clicked += (s, e) => {
                ((ContentView)grdOverlay.Parent).IsVisible = false;
            };
            ((ContentView)grdOverlay.Parent).IsVisible = true;

        }
        // public static void ShowWait(ContentView overlay, Grid grdOverlay,string szMessage)
        // {
        //     grdOverlay.Children.Clear();
        // 
        //     int iRow = 0;
        //     AddLabel(grdOverlay, iRow++, szMessage);
        // 
        //     overlay.IsVisible = true;
        // }
        public void AddRowDefinitions()
        {
            RowDefinition rd;
            grdOverlay.RowDefinitions.Clear();
            
            for (int r=0;r<= iRow+1;r++)
            {
                rd = new RowDefinition();
                rd.Height = new GridLength(1,GridUnitType.Auto);
                grdOverlay.RowDefinitions.Add(rd);
            }
        }
        public Entry AddInput(string szLabel, string szPlaceholder, string szDefaultValue)
        {
            var label = new Label { Text = szLabel };
            var entry = new Entry { Placeholder = szPlaceholder };
            if (!string.IsNullOrWhiteSpace(szDefaultValue))
            {
                entry.Text = szDefaultValue;
            }

            grdOverlay.Children.Add(label, 0, iRow + 1);
            grdOverlay.Children.Add(entry, 1, iRow + 1);
            iRow++;
            return entry;
        }
        
        public Picker AddPicker(string szId, string szLabel, Boolean bEditable=false, Action<Picker,string> addItem =null)
        {
            var label = new Label { Text = szLabel };
            var picker = new Picker {  };
            var entry = new Entry
            {
                IsVisible = false
            };
            var button = new Button { Text = "+", IsVisible = bEditable };

            entry.Completed += (sender, e) => {
                entry.IsVisible = false;
                picker.IsVisible = true;
                button.IsEnabled = true;

                if (entry.Text != null)
                {
                    // get the new entry
                    string szEntry = entry.Text.Trim();
                    // add it to the list
                    if (!string.IsNullOrWhiteSpace(szEntry))
                    {
                        addItem?.Invoke(picker, szEntry);
                    }
                }
            };
            picker.ClassId = "level";

            picker.StyleId = szId;
            grdOverlay.Children.Add(label, 0, iRow + 1);
            grdOverlay.Children.Add(picker, 1, iRow + 1);
            grdOverlay.Children.Add(entry, 1, iRow + 1);
            if ( bEditable)
            {
                
                button.Clicked += (s, e) => {
                    entry.IsVisible = true;
                    picker.IsVisible = false;
                    button.IsEnabled = false;
                    entry.Focus();
                };
                grdOverlay.Children.Add(button, 2, iRow + 1);
            }
            iRow++;
            return picker;
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void FillPicker(Picker picker, Dictionary<string,string> dict)
        {
            foreach (string val in dict.Keys)
            {
                picker.Items.Add(val);
            }
        }
        public Button AddButton( string szLabel)
        {
            var button = new Button { Text = szLabel, Margin = new Thickness(0,0,15,0) };
            
            grdOverlay.Children.Add(button, 0, iRow+1);
            Grid.SetColumnSpan(button, 3);
            iRow++;
            return button;
        }
        public void AddLabel( string szLabel)
        {
            var label = new Label { Text = szLabel };

            grdOverlay.Children.Add(label, 0, iRow+1);
            iRow++;

        }
        public void AddCancelX()
        {
            var button = new Button { Text = "X" };
            button.StyleId = "OverlayCancelX";
            grdOverlay.Children.Add(button,2,0);
            button.Clicked += (senderx, e2) =>
            {
                ((ContentView)grdOverlay.Parent).IsVisible = false;
            };
        }
    }
}
