using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Modelli
{
    public class Pezzo
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Bianco { get; set; }
        public string IcoPath { get; set; }
        public string Symbol { get; set; }
    }
    public class CavaliereNero : Pezzo
    {
        public CavaliereNero()
        {
            Symbol = "n";
            Bianco = false;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Nero\\knight.png";
        }
    }
    public class TorreNero : Pezzo
    {
        public TorreNero()
        {
            Symbol = "r";
            Bianco = false;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Nero\\rook.png";
        }
    }
    public class AlfiereNero : Pezzo
    {
        public AlfiereNero()
        {
            Symbol = "b";
            Bianco = false;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Nero\\bishop.png";
        }
    }
    public class ReNero : Pezzo
    {
        public ReNero()
        {
            Symbol = "k";
            Bianco = false;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Nero\\king.png";
        }
    }
    public class ReginaNera : Pezzo
    {
        public ReginaNera()
        {
            Symbol = "q";
            Bianco = false;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Nero\\queen.png";
        }
    }
    public class PedoneNero : Pezzo
    {
        public PedoneNero()
        {
            Symbol = "p";
            Bianco = false;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Nero\\pawn.png";
        }
    }
    public class CavaliereBianco : Pezzo
    {
        public CavaliereBianco()
        {
            Bianco = true;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Bianco\\knight.png";
        }
    }
    public class TorreBianca : Pezzo
    {
        public TorreBianca()
        {
            Symbol = "R";
            Bianco = false;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Bianco\\rook.png";
        }
    }
    public class AlfiereBianco : Pezzo
    {
        public AlfiereBianco()
        {
            Symbol = "B";
            Bianco = true;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Bianco\\bishop.png";
        }
    }
    public class ReBianco : Pezzo
    {
        public ReBianco()
        {
            Symbol = "K";
            Bianco = true;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Bianco\\king.png";
        }
    }
    public class ReginaBianca : Pezzo
    {
        public ReginaBianca()
        {
            Symbol = "Q";
            Bianco = true;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Bianco\\queen.png";
        }
    }
    public class PedoneBianco : Pezzo
    {
        public PedoneBianco()
        {
            Symbol = "P";
            Bianco = true;
            IcoPath = "C:\\Users\\Jacopo\\source\\repos\\Chess\\Chess\\Resource\\Img\\Bianco\\pawn.png";
        }
    }
}
