using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Shamai_R4.Class
{
    public static class Autokick
    {
        #region declare

        public static List<Kick> Kicks = new List<Kick>();

        #endregion

        #region methods
        /// <summary>
        /// adciona o usuario lista temp de kick
        /// </summary>
        /// <param name="user"></param>
        public static void AddUser(string user)
        {
            if (user == MainWindow.Configuration.StConfiguration.Owner || user == @"system" || user == @"azubu") { return; }
            foreach (Kick i in Kicks)
            {
                if(i.User == user) { return;}
            }

            Kick k = new Kick()
            {
                User = user
            };
            Kicks.Add(k);

            Application.Current.Dispatcher.Invoke(
                                DispatcherPriority.Normal, (Action)delegate
                                {
                                    MyDelegates.OnChatCallBack(@"system :", @"AddUser => " + user);
                                });
            //Console.WriteLine(@"add " + user);
        }
        /// <summary>
        /// remove o usuario da lista temp kick
        /// </summary>
        /// <param name="user"></param>
        public static void RemoveUser(string user)
        {
            if (user == MainWindow.Configuration.StConfiguration.Owner || user == @"system" || user == @"azubu" || Kicks.Count == 0) { return; }

            try
            {
                Kick us = new Kick();
                foreach (Kick k in Kicks)
                {
                    if (k.User == user)
                    {
                        us = k;
                        //Console.WriteLine(@"remove " + user);
                    }
                }
                Kicks.Remove(us);

                Application.Current.Dispatcher.Invoke(
                              DispatcherPriority.Normal, (Action)delegate
                              {
                                  MyDelegates.OnChatCallBack(@"system :", @"RemoveUser => " + user);
                              });
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine(ex.ToString());    
            }

           
        }
        /// <summary>
        /// remove todos os usuarios da lista
        /// </summary>
        public static void ClearAll()
        {
            Kicks.Clear();
            //Console.WriteLine(@"clear");
            Application.Current.Dispatcher.Invoke(
                              DispatcherPriority.Normal, (Action)delegate
                              {
                                  MyDelegates.OnChatCallBack(@"system :", @"ClearAllKick");
                              });
        }

        #endregion
    }

    public class Kick
    {
        public string User { get; set; }
    }
}
