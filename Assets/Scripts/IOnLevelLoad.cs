using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IOnLevelLoad
{
    //Used on a tile to execute code when the level loads
    void OnLevelLoad(Vector3Int position);
}

public interface IOnLevelLoadWithPost
{
    //Used on a tile to execute code when the level loads
    //It also gives you the option to run code after everything else has loaded
    Action OnLevelLoad(Vector3Int position);
}
