using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simulator.AsmTools;

namespace Simulator
{
    public partial class CPU : Form
    {

       
        public CPU()
        {
            
            InitializeComponent();
        }

        private void CPU_Load(object sender, EventArgs e)
        {
            Program.Asm.Registers = new List<Register>();
            Program.Asm.MemoryCells = new List<MemoryCell>();

            for (int i = 0; i < 32; i++) 
            {
                Program.Asm.Registers.Add(new Register("R" + i.ToString(), 0));
            }
            for (int i = 0; i < 1024; i++)
            {
                Program.Asm.MemoryCells.Add(new MemoryCell(i.ToString().PadLeft(4, '0'),"0"));
            }
            int pc = Program.Asm.LoadProgram();
            
            BR.DataSource = Program.Asm.Registers;
            Memory.DataSource = Program.Asm.MemoryCells;
            MAR.Text = "0000";
            MDR.Text = "0000";
            PC.Text = pc.ToString().PadLeft(4, '0');
            IR.Text = "0000";
            X.Text = "0000";
            Y.Text = "0000";
            Z.Text = "0000";

        }
    }
}
