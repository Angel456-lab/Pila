using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Simulator.AsmTools;

namespace Simulator
{
    public partial class Tools : Form
    {
        
        
        public Tools()
        {
            InitializeComponent();
            
        }

        private void BtnEnd_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnOpenFile_Click(object sender, EventArgs e)
        {
            if (Program.cantAsm == 0)  // Si no se ha clickado antes...
            {
                Program.cantAsm++;

                Program.Asm = new Assembler(@"C:\Prj\Simulator\sample.asm");

                var cantErr = Program.Asm.AssembleSourceFile();

                textBox1.Lines = Program.Asm.inputFileContent;
                listBox1.DataSource = Program.Asm.LeanFileContent;
                dataGridView1.DataSource = Program.Asm.Code;
                dataGridView2.DataSource = Program.Asm.SymbolTable;
                dataGridView3.DataSource = Program.Asm.ProgramConstructs;
                dataGridView4.DataSource = Program.Asm.Errors;


                if (cantErr > 0)
                {
                    MessageBox.Show("Ensablado no fue del todo exitoso. Se detectaron " + Program.Asm.LexErrors.ToString() + " Errores Léxicos y " +
                        Program.Asm.ParseErrors.ToString() + " Errores de parseo.", "Resultado del Ensamblado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ensamblado realizado con éxito.", "Resultado del Ensamblado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                button1.Click += button1_Click; //Habilita el evento Click del Botón CPU
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (MayOpenWindow("CPU") && Program.Asm.result )
            {
                var cpu = new CPU();
                cpu.Show();
            }
        }

        private bool MayOpenWindow(string windowName)
        {
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                if (Application.OpenForms[i].Name.Equals(windowName))
                {
                    Application.OpenForms[i].BringToFront();
                    return false;
                }
            }
            return true;
        }

        private void Tools_Load(object sender, EventArgs e)
        {
            button1.Click -= button1_Click; // Deshabilita el Evento Click del botón CPU 
                                            // Será habilitado por el Evento Click del botón Ensamblar Archivo
        }
    }
}

