using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IEatable
{
    //Called when the muncher eats any object that impliments this interface
    void OnEat(Muncher muncher);
}
