using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XCamera
{
    public static class Overlay
    {
        public static void ShowWait(ContentView overlay, Grid grdOverlay,string szMessage)
        {
            grdOverlay.Children.Clear();

            int iRow = 0;
            AddLabel(grdOverlay, iRow++, szMessage);

            overlay.IsVisible = true;
        }
        public static void AddRowDefinitions(Grid grdOverlay, int iRowCnt)
        {
            RowDefinition rd;
            grdOverlay.RowDefinitions.Clear();
            
            for (int r=0;r<= iRowCnt+1;r++)
            {
                rd = new RowDefinition();
                rd.Height = new GridLength(1,GridUnitType.Auto);
                grdOverlay.RowDefinitions.Add(rd);
            }
        }
        public static Entry AddInput(Grid grdOverlay, int iRow, string szLabel, string szPlaceholder, string szDefaultValue)
        {
            var label = new Label { Text = szLabel };
            var entry = new Entry { Placeholder = szPlaceholder };
            if (!string.IsNullOrWhiteSpace(szDefaultValue))
            {
                entry.Text = szDefaultValue;
            }

            grdOverlay.Children.Add(label, 0 + 1, iRow + 1);
            grdOverlay.Children.Add(entry, 1 + 1, iRow + 1);

            return entry;
        }
        public static Picker AddPicker(Grid grdOverlay,string szId, int iRow, string szLabel)
        {
            var label = new Label { Text = szLabel };
            var picker = new Picker { };
            picker.ClassId = "level";

            picker.StyleId = szId;
            grdOverlay.Children.Add(label, 0 + 1, iRow + 1);
            grdOverlay.Children.Add(picker, 1 + 1, iRow + 1);

            return picker;
        }
        public static void FillPicker(Picker picker, Dictionary<string,string> dict)
        {
            foreach (string val in dict.Keys)
            {
                picker.Items.Add(val);
            }
        }
        public static Button AddButton(Grid grdOverlay, int iRow, string szLabel)
        {
            var button = new Button { Text = szLabel };
            
            grdOverlay.Children.Add(button, 1+1, iRow+1);
            return button;
        }
        public static void AddLabel(Grid grdOverlay, int iRow, string szLabel)
        {
            var label = new Label { Text = szLabel };

            grdOverlay.Children.Add(label, 0+1, iRow+1);

        }
        public static void AddCancelX(Grid grdOverlay)
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
