using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReedSolomon;

namespace TD2_Projet_Info_Maxence_Raballand
{
    class Program
    {
        static void Main(string[] args)
        {
            MyImage img = new MyImage("taxi.bmp");
            img.RedFilter();
            img.Save("taxi.bmp");
            Console.ReadKey();
        }
    }
}
