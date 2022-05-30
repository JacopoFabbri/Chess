using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Util.Stockfish
{
    public class Stockfish
    {
        private ProcessStartInfo info;
        public string MossaSelezionata { get; set; }
        public string Move { get; set; }
        public Stockfish()
        {
            info = new ProcessStartInfo("C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Stockfish\\stockfish.exe");
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;
            Move = String.Empty;
        }
        public void StartMatch()
        {
            using (Process process = Process.Start(info))
            {
                StreamWriter sw = process.StandardInput;
                process.OutputDataReceived += (sender, args) => UpdateText(args.Data);
                sw.WriteLine("position startpos");
                sw.WriteLine("go");
                sw.Close();
            }
        }
        private void UpdateText(string data)
        {
            if(data.Contains("info depth 30 currmove"))
            {
                MossaSelezionata = data;
            }
            else
            {
                Move = data;
            }
        }
    }
}
