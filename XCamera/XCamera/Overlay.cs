using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

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

            grdOverlay.Children.Add(label, 0 + 1, iRow + 1);
            grdOverlay.Children.Add(entry, 1 + 1, iRow + 1);
            iRow++;
            return entry;
        }
        public Picker AddPicker(string szId, string szLabel)
        {
            var label = new Label { Text = szLabel };
            var picker = new Picker { };
            picker.ClassId = "level";

            picker.StyleId = szId;
            grdOverlay.Children.Add(label, 0 + 1, iRow + 1);
            grdOverlay.Children.Add(picker, 1 + 1, iRow + 1);
            iRow++;
            return picker;
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
            var button = new Button { Text = szLabel };
            
            grdOverlay.Children.Add(button, 1+1, iRow+1);
            iRow++;
            return button;
        }
        public void AddLabel( string szLabel)
        {
            var label = new Label { Text = szLabel };

            grdOverlay.Children.Add(label, 0+1, iRow+1);
            iRow++;

        }
        public void AddCancelX()
        {
            var button = new Button { Text = "X" };
            button.StyleId = "OverlayCancelX";
            grdOverlay.Children.Add(button,3,0);
            button.Clicked += (senderx, e2) =>
            {
                ((ContentView)grdOverlay.Parent).IsVisible = false;
            };
        }
    }
}
