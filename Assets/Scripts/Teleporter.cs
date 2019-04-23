using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public static List<Teleporter> Teleporters = new List<Teleporter>();
    public Vector3Int Position;
    public int LinkID;
    private Teleporter linkInternal;
    public Teleporter LinkedTeleporter
    {
        get
        {
            if (linkInternal == null)
            {
                foreach (var tele in Teleporters)
                {
                    if (tele != this && tele.LinkID == LinkID)
                    {
                        linkInternal = tele;
                        break;
                    }
                }
            }
            return linkInternal;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    public static void LoadInitialization()
    {
        GameManager.OnLevelEnd += () => Teleporters.Clear();
    }


    //Returns the teleporter at the position
    public static Teleporter GetTeleporter(Vector3Int position)
    {
        Debug.Log("TEST POSITION = " + position);
        foreach (var tele in Teleporters)
        {
            Debug.Log("PORTAL POSITION = " + tele.Position);
            if (tele.Position == position)
            {
                return tele;
            }
        }
        return null;
    }
}
