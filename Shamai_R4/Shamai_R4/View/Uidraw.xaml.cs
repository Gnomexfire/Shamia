using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Shamai_R4.Class;

namespace Shamai_R4.View
{
    /// <summary>
    /// Interaction logic for Uidraw.xaml
    /// </summary>
    public partial class Uidraw : MetroWindow
    {
        #region declare


        #endregion

        public Uidraw()
        {
            InitializeComponent();
        }

        private void Uidraw_OnLoaded(object sender, RoutedEventArgs e)
        {
            _enum();
            _addresource();
            _GetFonts();
        }

        #region internal

        internal void _enum()
        {
            List<ColorsHexDefault> t = MyDelegates.ColorsHex;
            foreach (ColorsHexDefault i in t)
            {
                CboxColors.Items.Add(i.Color);
            }
            



            List<FontDefault> f = MyDelegates.FontDefaults;
            foreach (FontDefault myf in f)
            {
                CboxFont.Items.Add(myf.Name);
            }

            for (int i = 5; i < 16; i++)
            {
                CboxFontSize.Items.Add(i);
            }


        }

        /// <summary>
        /// add ResourceDictionary
        /// </summary>
        internal void _addresource()
        {
            try
            {
                List<TemplateLang> lang = Clanguage.Langs;
                ResourceDictionary d = new ResourceDictionary();
                foreach (TemplateLang i in lang)
                {
                    if (i.Lang.ToUpper() == Clanguage.GetLangInClient())
                    {
                        d.Source = new Uri(i.FileLink, UriKind.Absolute);
                        Resources.MergedDictionaries.Add(d);
                    }
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }

        }
        public UiFontConf MyFont { get; set; }
        internal void _GetFonts()
        {
            try
            {
                MyFont = new UiFontConf();
                foreach (UiFontConf i in MainWindow.SqliteSource.EnumUiFont())
                {
                    MyFont.Brush = i.Brush;
                    MyFont.FontSize = i.FontSize;
                    MyFont.Fontname = i.Fontname;
                    MyFont.Isbold = i.Isbold;
                    MyFont.Isitalic = i.Isitalic;
                    MyFont.ColorName = i.ColorName;
                }
                ChkIsBold.IsChecked = MyFont.Isbold;
                ChkIsItalic.IsChecked = MyFont.Isitalic;

                foreach (var i in CboxFont.Items)
                {
                    if (i.Equals(MyFont.Fontname))
                    {
                        CboxFont.SelectedItem = i;
                    }
                }

                foreach (var i in CboxFontSize.Items)
                {
                    if (i.Equals(MyFont.FontSize))
                    {
                        CboxFontSize.SelectedItem = i;
                    }
                }

                foreach (var i in CboxColors.Items)
                {
                    var n = i.ToString().ToUpper();
                    if (n.Equals(MyFont.ColorName.ToUpper()))
                    {
                        CboxColors.SelectedItem = i;
                    }
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.ToString());    
            }
           
        }
        #endregion

        private void CmdSave_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.SqliteSource.UpdateUiConf(CboxFont.SelectionBoxItem.ToString(),Convert.ToInt32(CboxFontSize.SelectionBoxItem.ToString()),ChkIsBold.IsChecked != null && ChkIsBold.IsChecked.Value,ChkIsItalic.IsChecked != null && ChkIsItalic.IsChecked.Value,CboxColors.SelectionBoxItem.ToString());
            this.ShowMessageAsync(@"Sucess", @"Saved");
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
