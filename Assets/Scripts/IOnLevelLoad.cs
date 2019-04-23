using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IOnLevelLoad
{
    void OnLevelLoad(Vector3Int position);
}

public interface IOnLevelLoadWithPost
{
    Action OnLevelLoad(Vector3Int position);
}
