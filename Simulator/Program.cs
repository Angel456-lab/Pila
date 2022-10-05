using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simulator.AsmTools;

namespace Simulator
{
    internal static class Program
    {
        public static int cantAsm = 0;
        public static Assembler Asm;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Tools());
        }
    }
}
