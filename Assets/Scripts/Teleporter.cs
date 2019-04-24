using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public static List<Teleporter> Teleporters = new List<Teleporter>(); //The list of all the teleporters in the game
    public Vector3Int Position; //The position of the teleporter
    public int LinkID; //The link id of the teleporter
    private Teleporter linkInternal; //The linked teleporter
    public Teleporter LinkedTeleporter //The public interface for accessing the linked teleporter
    {
        get
        {
            //Get the linked teleporter if there is one in the level
            if (linkInternal == null)
            {
                foreach (var tele in Teleporters)
                {
                    //If the link ids match
                    if (tele != this && tele.LinkID == LinkID)
                    {
                        //Set it to be linked
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
        //Add an event to clear the teleporters link when the level unloads
        GameManager.OnLevelUnload += () => Teleporters.Clear();
    }


    //Returns the teleporter at the position
    //Returns null if there is no teleporter at the position
    public static Teleporter GetTeleporter(Vector3Int position)
    {
        foreach (var tele in Teleporters)
        {
            if (tele.Position == position)
            {
                return tele;
            }
        }
        return null;
    }
}
