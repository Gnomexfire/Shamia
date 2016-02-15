using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Shamai_R4.View;

namespace Shamai_R4.Class
{
    public static class Clanguage
    {
        #region declare
        /// <summary>
        /// stores laguage disponible created object TemplateLang
        /// </summary>
        public static List<TemplateLang> Langs = new List<TemplateLang>(); 
        #endregion
        /// <summary>
        /// load mergerdictionary default language
        /// </summary>
        public static void LoadDefaultLang()
        {
            string i = "..\\Translate\\Default.xaml";
            ResourceDictionary d = new ResourceDictionary()
            {
                Source = new Uri(i, UriKind.Relative)
            };
            ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries.Add(d);
            var r = new ResourceDictionary { Source = new Uri(i, UriKind.Relative) };

            // creat object list all language
            //_ListLangs();
        }

        /// <summary>
        /// add dictionary langs
        /// </summary>
        public static void _ListLangs()
        {
            string[] files = null;
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory +
                                 "Translation"))
            {
                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory +
                                         "Translation", "*xaml");
                foreach (string i in files)
                {
                    ResourceDictionary temp = new ResourceDictionary()
                    {
                        Source = new Uri(i,UriKind.Absolute)
                    };

                    Langs.Add(new TemplateLang()
                    {
                        Flag = temp["F"].ToString(),
                        Language = temp["L"].ToString(),
                        FileLink = i,
                        Lang = temp["Lang"].ToString()
                    });
                }
            }
        }
        /// <summary>
        /// return flag lang used in client
        /// </summary>
        /// <returns></returns>
        public static string GetLangInClient()
        {
            return ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["Lang"].ToString().ToUpper();
        }

        /// <summary>
        /// update resourcedictionary from files translated .xaml folder Translation
        /// </summary>
        /// <param name="flag"></param>
        public static void SetLanguage(string flag)
        {
            // check if exist flag em object flags
            foreach (TemplateLang i in Langs)
            {
                // procura a flag dentro o objeto langs para atualizar o resourcedictionary
                if (i.Flag.ToUpper() == flag.ToUpper())
                {
                    // load resourcedictionary correct
                    ResourceDictionary d = new ResourceDictionary()
                    {
                        Source = new Uri(i.FileLink)
                    };
                    // clear 
                    ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries.Clear();
                    // add new object
                    ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries.Add(d);
                }
            }
        }

        /// <summary>
        /// simples metodo que converte lang para o flag do idioma
        /// ex : English => EN
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static string ConvertLangInFlag(string lang)
        {
            foreach (TemplateLang i in Clanguage.Langs)
            {
                if (i.Lang.ToUpper() == lang.ToUpper())
                {
                    return i.Flag;
                }
            }
            return string.Empty;
        }
    }

    #region ClassTemplate

    public class TemplateLang
    {
        /// <summary>
        /// flag id EN
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// language id MenuUI
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// archive link
        /// </summary>
        public string FileLink { get; set; }
        /// <summary>
        /// language
        /// </summary>
        public string Lang { get; set; }
    }

    #endregion
}
