using Chess.Modelli;
using System.Diagnostics;
using Chess.Util.Stockfish;

namespace Chess
{
    public partial class SchermataDiGioco : Form
    {
        Dictionary<string, PictureBox> Posizioni = new Dictionary<string, PictureBox>();
        Dictionary<string, Pezzo?> Scacchiera = new Dictionary<string, Pezzo?>();
        StreamWriter streamWriter;
        String TestoConsole = "...";
        String BestMove = "...";
        String Move = "";
        Boolean bot = false;
        Process Processo;
        Boolean Check = false;
        Boolean Bianco = false;
        Boolean MessaggioDiVittoria = false;
        List<String> MosseFatte = new List<String>();
        String currentFen = "";
        int Profondita = 30;
        public SchermataDiGioco()
        {
            try
            {
                InitializeComponent();
                PopolamentoPosizioni();
                PopolamentoScacchiera();
                var info = new ProcessStartInfo("C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Stockfish\\stockfish.exe");
                info.UseShellExecute = false;
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.CreateNoWindow = true;
                if (info != null)
                {
                    this.Processo = Process.Start(info);
                    if (this.Processo != null)
                    {
                        Processo.OutputDataReceived += UpdateText;
                        Processo.BeginOutputReadLine();
                    }
                }
                checkUpdate.Enabled = true;
                streamWriter = Processo.StandardInput;
                foreach (var p in Posizioni)
                {
                    p.Value.Click += SetMove;
                }
                profondita.Value = Profondita;
                valore.Text = "" + Profondita;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SetMove(object? sender, EventArgs e)
        {
            try
            {
                if (Move != null)
                {
                    if (Move.Count() == 2)
                    {
                        Move += ((PictureBox)sender).Name;
                        MosseFatte.Add(Move);
                        nuovaMossa();
                        Move = "";
                    }
                    else if (Move.Count() == 4)
                    {
                        Move = "";
                    }
                    else
                    {
                        Move = ((PictureBox)sender).Name;
                        ((PictureBox)sender).BackColor = Color.LightPink;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SchermataDiGioco_Load(object sender, EventArgs e)
        {
        }
        private void scacchiera_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if ((e.Column + e.Row) % 2 == 1)
                e.Graphics.FillRectangle(Brushes.LightSteelBlue, e.CellBounds);
            else
                e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
        }
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            streamWriter.WriteLine("stop");
            streamWriter.WriteLine("ucinewgame");
            streamWriter.WriteLine("position startpos");
            streamWriter.WriteLine("d");
            MosseFatte.Clear();
            currentFen = "";
            startToolStripMenuItem.Visible = true;
            bot = false;
        }
        private void nuovaMossa()
        {
            string comando = "position startpos moves ";
            foreach (var m in MosseFatte)
            {
                comando += m + " ";
            }
            streamWriter.WriteLine(comando);
            streamWriter.WriteLine("d");
            if (Bianco)
            {
                Bianco = false;
            }
            else
            {
                Bianco = true;
            }
        }
        private void UpdateText(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (e != null)
                {
                    if (e.Data != null)
                    {
                        if (bot)
                        {
                            if (e.Data.Contains("bestmove"))
                            {
                                var miglioreMossa = GetBestMove(e.Data);
                                BestMove = "Best Move: " + miglioreMossa;
                                MosseFatte.Add(miglioreMossa);
                                nuovaMossa();
                                streamWriter.WriteLine("go depth " + Profondita);
                            }
                            else if (e.Data.Contains("seldepth"))
                            {
                                var listaMosse = GetMosse(e.Data);
                                String testo = "Profondita: " + GetProfondita(e.Data) + "\nMosse: ";
                                foreach (var mosse in listaMosse)
                                {
                                    testo += mosse + "   ";
                                }
                                TestoConsole = testo;
                            }
                            else if (e.Data.Contains("Fen"))
                            {
                                var tempFen = GetFenPosition(e.Data);
                                if (!tempFen.Equals(currentFen))
                                {
                                    SetpiecePositionFromFen(tempFen);
                                    currentFen = tempFen;
                                }
                                else
                                {
                                    MosseFatte.Remove(MosseFatte.Last());
                                }
                            }
                            else if (e.Data.Contains("Checkers"))
                            {
                                var checkt = e.Data.Split(" ");
                                if (checkt.Length > 1)
                                {
                                    if (!checkt[1].Equals(""))
                                    {
                                        Check = true;
                                        streamWriter.WriteLine("go perft 1");
                                    }
                                }
                            }
                            else if (e.Data.Contains("Nodes searched:"))
                            {
                                var split = e.Data.Split(" ")[2];
                                if (Check && split.Equals("0"))
                                {
                                    if (!MessaggioDiVittoria)
                                    {
                                        if (!Bianco)
                                            MessageBox.Show("Ha vinto il Bianco");
                                        else
                                            MessageBox.Show("Ha vinto il Nero");
                                        MessaggioDiVittoria = true;
                                    }
                                    bot = false;
                                    streamWriter.WriteLine("stop");
                                }
                            }
                            if(MosseFatte.Count > 6)
                            {
                                var mosse = MosseFatte.TakeLast(6).ToList();
                                if((mosse[0].Equals(mosse[2]) && mosse[2].Equals(mosse[4]))&&(mosse[1].Equals(mosse[3]) && mosse[3].Equals(mosse[5]))){

                                    streamWriter.WriteLine("stop");
                                    MessageBox.Show("Patta");
                                    MessaggioDiVittoria = true;
                                }
                            }
                        }
                        else
                        {
                            if (e.Data.Contains("bestmove"))
                            {
                                var miglioreMossa = GetBestMove(e.Data);
                                BestMove = "Best Move: " + miglioreMossa;
                                MosseFatte.Add(miglioreMossa);
                                nuovaMossa();
                            }
                            else if (e.Data.Contains("seldepth"))
                            {
                                var listaMosse = GetMosse(e.Data);
                                String testo = "Profondita: " + GetProfondita(e.Data) + "\nMosse: ";
                                foreach (var mosse in listaMosse)
                                {
                                    testo += mosse + "   ";
                                }
                                TestoConsole = testo;
                            }
                            else if (e.Data.Contains("Fen"))
                            {
                                var tempFen = GetFenPosition(e.Data);
                                if (!tempFen.Equals(currentFen))
                                {
                                    SetpiecePositionFromFen(tempFen);
                                    currentFen = tempFen;
                                }
                                else
                                {
                                    MosseFatte.Remove(MosseFatte.Last());
                                }
                            }
                            else if (e.Data.Contains("Checkers"))
                            {
                                var checkt = e.Data.Split(" ");
                                if (checkt.Length > 1)
                                {
                                    if (!checkt[1].Equals(""))
                                    {
                                        Check = true;
                                        streamWriter.WriteLine("go perft 1");
                                    }
                                }
                            }
                            else if (e.Data.Contains("Nodes searched:"))
                            {
                                var split = e.Data.Split(" ")[2];
                                if (Check && split.Equals("0"))
                                {
                                    if (!MessaggioDiVittoria)
                                    {
                                        if (!Bianco)
                                            MessageBox.Show("Ha vinto il Bianco");
                                        else
                                            MessageBox.Show("Ha vinto il Nero");
                                        MessaggioDiVittoria = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PuliziaIcone()
        {
            try
            {
                foreach (var p in Posizioni)
                {
                    p.Value.Image = null;
                    p.Value.BackColor = Color.Transparent;
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SetpiecePositionFromFen(String fen)
        {
            try
            {
                PuliziaIcone();
                PopolamentoScacchiera();
                int riga = 8;
                var righe = fen.Split("/");
                foreach (var r in righe)
                {
                    int colonna = 1;
                    foreach (var c in r)
                    {
                        if (!char.IsDigit(c))
                        {
                            var posizione = "";
                            switch (colonna)
                            {
                                case 1:
                                    posizione = "a" + riga;
                                    break;
                                case 2:
                                    posizione = "b" + riga;
                                    break;
                                case 3:
                                    posizione = "c" + riga;
                                    break;
                                case 4:
                                    posizione = "d" + riga;
                                    break;
                                case 5:
                                    posizione = "e" + riga;
                                    break;
                                case 6:
                                    posizione = "f" + riga;
                                    break;
                                case 7:
                                    posizione = "g" + riga;
                                    break;
                                case 8:
                                    posizione = "h" + riga;
                                    break;
                            }

                            var path = "";
                            Pezzo pezzo = null;
                            switch (c)
                            {
                                case 'r':
                                    pezzo = new TorreNero();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'n':
                                    pezzo = new CavaliereNero();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'b':
                                    pezzo = new AlfiereNero();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'q':
                                    pezzo = new ReginaNera();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'k':
                                    pezzo = new ReNero();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'p':
                                    pezzo = new PedoneNero();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'R':
                                    pezzo = new TorreBianca();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'N':
                                    pezzo = new CavaliereBianco();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'B':
                                    pezzo = new AlfiereBianco();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'Q':
                                    pezzo = new ReginaBianca();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'K':
                                    pezzo = new ReBianco();
                                    path = pezzo.IcoPath;
                                    break;
                                case 'P':
                                    pezzo = new PedoneBianco();
                                    path = pezzo.IcoPath;
                                    break;
                            }
                            Posizioni[posizione].Image = Image.FromFile(path);
                            Scacchiera[posizione] = pezzo;
                            colonna++;
                        }
                        else
                        {
                            colonna += int.Parse("" + c);
                        }
                        if (colonna > 8)
                        {
                            break;
                        }
                    }
                    riga--;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private List<string> GetMosse(String s)
        {
            return s.Split(" ").Skip(21).ToList();
        }
        private int GetProfondita(String s)
        {
            return int.Parse(s.Split(" ").ToList()[2]);
        }
        private string GetBestMove(String s)
        {
            return s.Split(" ").ToList()[1];
        }
        private string GetFenPosition(String s)
        {
            return s.Split(" ").ToList()[1];
        }
        private void checkUpdate_Tick(object sender, EventArgs e)
        {
            if (!Mosse.Equals(TestoConsole))
            {
                Mosse.Text = TestoConsole;
            }
            if (!MossaSelezionata.Equals(BestMove))
            {
                MossaSelezionata.Text = BestMove;
            }
            if (listBox1.Items.Count != MosseFatte.Count)
            {
                listBox1.Items.Clear();
                listBox1.Items.AddRange(MosseFatte.ToArray());
            }
        }
        private void PopolamentoPosizioni()
        {
            Posizioni["a1"] = a1;
            Posizioni["a2"] = a2;
            Posizioni["a3"] = a3;
            Posizioni["a4"] = a4;
            Posizioni["a5"] = a5;
            Posizioni["a6"] = a6;
            Posizioni["a7"] = a7;
            Posizioni["a8"] = a8;

            Posizioni["b1"] = b1;
            Posizioni["b2"] = b2;
            Posizioni["b3"] = b3;
            Posizioni["b4"] = b4;
            Posizioni["b5"] = b5;
            Posizioni["b6"] = b6;
            Posizioni["b7"] = b7;
            Posizioni["b8"] = b8;

            Posizioni["c1"] = c1;
            Posizioni["c2"] = c2;
            Posizioni["c3"] = c3;
            Posizioni["c4"] = c4;
            Posizioni["c5"] = c5;
            Posizioni["c6"] = c6;
            Posizioni["c7"] = c7;
            Posizioni["c8"] = c8;

            Posizioni["d1"] = d1;
            Posizioni["d2"] = d2;
            Posizioni["d3"] = d3;
            Posizioni["d4"] = d4;
            Posizioni["d5"] = d5;
            Posizioni["d6"] = d6;
            Posizioni["d7"] = d7;
            Posizioni["d8"] = d8;

            Posizioni["e1"] = e1;
            Posizioni["e2"] = e2;
            Posizioni["e3"] = e3;
            Posizioni["e4"] = e4;
            Posizioni["e5"] = e5;
            Posizioni["e6"] = e6;
            Posizioni["e7"] = e7;
            Posizioni["e8"] = e8;

            Posizioni["f1"] = f1;
            Posizioni["f2"] = f2;
            Posizioni["f3"] = f3;
            Posizioni["f4"] = f4;
            Posizioni["f5"] = f5;
            Posizioni["f6"] = f6;
            Posizioni["f7"] = f7;
            Posizioni["f8"] = f8;

            Posizioni["g1"] = g1;
            Posizioni["g2"] = g2;
            Posizioni["g3"] = g3;
            Posizioni["g4"] = g4;
            Posizioni["g5"] = g5;
            Posizioni["g6"] = g6;
            Posizioni["g7"] = g7;
            Posizioni["g8"] = g8;

            Posizioni["h1"] = h1;
            Posizioni["h2"] = h2;
            Posizioni["h3"] = h3;
            Posizioni["h4"] = h4;
            Posizioni["h5"] = h5;
            Posizioni["h6"] = h6;
            Posizioni["h7"] = h7;
            Posizioni["h8"] = h8;
        }
        private void PopolamentoScacchiera()
        {
            Scacchiera["a1"] = null;
            Scacchiera["a2"] = null;
            Scacchiera["a3"] = null;
            Scacchiera["a4"] = null;
            Scacchiera["a5"] = null;
            Scacchiera["a6"] = null;
            Scacchiera["a7"] = null;
            Scacchiera["a8"] = null;

            Scacchiera["b1"] = null;
            Scacchiera["b2"] = null;
            Scacchiera["b3"] = null;
            Scacchiera["b4"] = null;
            Scacchiera["b5"] = null;
            Scacchiera["b6"] = null;
            Scacchiera["b7"] = null;
            Scacchiera["b8"] = null;

            Scacchiera["c1"] = null;
            Scacchiera["c2"] = null;
            Scacchiera["c3"] = null;
            Scacchiera["c4"] = null;
            Scacchiera["c5"] = null;
            Scacchiera["c6"] = null;
            Scacchiera["c7"] = null;
            Scacchiera["c8"] = null;

            Scacchiera["d1"] = null;
            Scacchiera["d2"] = null;
            Scacchiera["d3"] = null;
            Scacchiera["d4"] = null;
            Scacchiera["d5"] = null;
            Scacchiera["d6"] = null;
            Scacchiera["d7"] = null;
            Scacchiera["d8"] = null;

            Scacchiera["e1"] = null;
            Scacchiera["e2"] = null;
            Scacchiera["e3"] = null;
            Scacchiera["e4"] = null;
            Scacchiera["e5"] = null;
            Scacchiera["e6"] = null;
            Scacchiera["e7"] = null;
            Scacchiera["e8"] = null;

            Scacchiera["f1"] = null;
            Scacchiera["f2"] = null;
            Scacchiera["f3"] = null;
            Scacchiera["f4"] = null;
            Scacchiera["f5"] = null;
            Scacchiera["f6"] = null;
            Scacchiera["f7"] = null;
            Scacchiera["f8"] = null;

            Scacchiera["g1"] = null;
            Scacchiera["g2"] = null;
            Scacchiera["g3"] = null;
            Scacchiera["g4"] = null;
            Scacchiera["g5"] = null;
            Scacchiera["g6"] = null;
            Scacchiera["g7"] = null;
            Scacchiera["g8"] = null;

            Scacchiera["h1"] = null;
            Scacchiera["h2"] = null;
            Scacchiera["h3"] = null;
            Scacchiera["h4"] = null;
            Scacchiera["h5"] = null;
            Scacchiera["h6"] = null;
            Scacchiera["h7"] = null;
            Scacchiera["h8"] = null;
        }
        private string GetFenFromChessboard()
        {
            var fen = "";
            for (int x = 8; x >= 1; x--)
            {
                var spaziVuoti = 0;
                for (int y = 1; y <= 8; y++)
                {
                    var posizione = "";
                    switch (y)
                    {
                        case 1:
                            posizione = "a" + x;
                            break;
                        case 2:
                            posizione = "b" + x;
                            break;
                        case 3:
                            posizione = "c" + x;
                            break;
                        case 4:
                            posizione = "d" + x;
                            break;
                        case 5:
                            posizione = "e" + x;
                            break;
                        case 6:
                            posizione = "f" + x;
                            break;
                        case 7:
                            posizione = "g" + x;
                            break;
                        case 8:
                            posizione = "h" + x;
                            break;
                    }
                    if (Scacchiera[posizione] == null)
                    {
                        spaziVuoti++;
                    }
                    else
                    {
                        if (spaziVuoti != 0)
                        {
                            fen += spaziVuoti + Scacchiera[posizione].Symbol;
                            spaziVuoti = 0;
                        }
                        else
                        {
                            fen += Scacchiera[posizione].Symbol;
                        }
                    }
                }
                if (spaziVuoti != 0)
                {
                    fen += spaziVuoti;
                    spaziVuoti = 0;
                }
                fen += "/";
            }
            return fen;
        }
        private void getFenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(GetFenFromChessboard());
        }
        private void bOTPLAYALONEToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bot = true;
            streamWriter.WriteLine("go depth " + Profondita);
        }
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bot = false;
            streamWriter.WriteLine("stop");
        }
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index % 2 != 0)
            {
                BackColor = Color.Black;
                ForeColor = Color.White;
            }
            else
            {
                BackColor = Color.White;
                ForeColor = Color.Black;
            }
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Profondita = profondita.Value;
            valore.Text = "" + Profondita;
        }
    }
}