using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamai_R4.Class
{
    /// <summary>
    /// Games.cs file part Shamia
    /// </summary>
    public class Games
    {
        /// <summary>
        /// nome do jogo
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// define se habilitado para o jogo
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// total de linhas mensagens aparecem na tela
        /// </summary>
        //public int DisplayMessageCount { get; set; }
    }
    /// <summary>
    /// Game.cs file part Shamia
    /// </summary>
    public static class Game
    {
        public static List<Games> GamesAvailable = new List<Games>()
        {
            // League Of Legends
            //new Games()
            //{
            //    // default league of legends 
            //    DisplayMessageCount = 10,
            //    IsEnabled = true,
            //    Name = "League Of Legends"
            //}
        };
        /// <summary>
        /// lista todos os games SQLite
        /// </summary>
        public static void EnumGamesIn()
        {
            GamesAvailable = MainWindow.SqliteSource.EnumGamesSaved();
        }
    }
}
