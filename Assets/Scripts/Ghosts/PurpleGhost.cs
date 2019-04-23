using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PurpleGhost : Ghost
{
    protected override float Speed => Muncher.MainMuncher.Speed;
}
